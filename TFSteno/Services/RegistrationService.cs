using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using TFSteno.Models;

namespace TFSteno.Services
{
    public static class RegistrationService
    {
        private static readonly string _ConnectionString =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public enum RegistrationOutcome
        {
            Success,
            Conflict,
            OtherError
        }

        public enum ConfirmationOutcome
        {
            NotExist,
            AlreadyConfirmed,
            Success
        }

        public class RegistrationResult
        {
            public RegistrationOutcome Outcome;
            public int Id;
            public string ConfirmationCode;
            public string ErrorMessage;
            public Exception Exception;
        }

        public static RegistrationResult SaveRegistration(Registration registration)
        {
            var confirmationCode = Guid.NewGuid().ToString("N");

            try
            {
                using (var conn = new SqlConnection(_ConnectionString))
                {
                    conn.Open();

                    int id = conn.Execute(
                        "INSERT [Registrations] (Email, TfsUrl, TfsUsername, TfsPassword, ConfirmationCode) VALUES (@Email, @TfsUrl, @TfsUsername, @TfsPassword, @ConfirmationCode); SELECT cast(scope_identity() as int);",
                        new
                        {
                            Email = registration.Email,
                            TfsUrl = Crypt.Encrypt(registration.TfsUrl),
                            TfsUsername = Crypt.Encrypt(registration.TfsUsername),
                            TfsPassword = Crypt.Encrypt(registration.TfsPassword),
                            ConfirmationCode = confirmationCode.ToString()
                        });

                    return new RegistrationResult
                    {
                        Outcome = RegistrationOutcome.Success,
                        Id = id,
                        ConfirmationCode = confirmationCode
                    };
                }
            }
            catch (Exception ex)
            {
                var result = new RegistrationResult
                {
                    Exception = ex
                };
                var sqlEx = ex as SqlException;
                if (sqlEx != null && sqlEx.Errors.Cast<SqlError>().Any(e => e.Number == 2601))
                {
                    result.Outcome = RegistrationOutcome.Conflict;
                    result.ErrorMessage = String.Format("A user with the address {0} is already signed up.",
                        registration.Email);
                }
                else
                {
                    result.Outcome = RegistrationOutcome.OtherError;
                    result.ErrorMessage = "An unexpected error occurred.";
                }
                return result;
            }
        }

        public static Registration GetRegistration(string email)
        {
            Registration registration;
            
            using (var conn = new SqlConnection(_ConnectionString))
            {
                conn.Open();
                registration = conn
                    .Query<Registration>("SELECT * FROM Registrations WHERE [Email] = @Email", new { Email = email })
                    .Single();
            }

            registration.TfsUrl = Crypt.Decrypt(registration.TfsUrl);
            registration.TfsUsername = Crypt.Decrypt(registration.TfsUsername);
            registration.TfsPassword = Crypt.Decrypt(registration.TfsPassword);

            return registration;
        }

        public static ConfirmationOutcome ConfirmRegistration(string confirmationCode)
        {
            using (var conn = new SqlConnection(_ConnectionString))
            {
                conn.Open();
                
                var registration = conn
                    .Query<Registration>("SELECT * FROM Registrations WHERE [ConfirmationCode] = @ConfirmationCode", new { ConfirmationCode = confirmationCode })
                    .SingleOrDefault();

                if (registration == null)
                    return ConfirmationOutcome.NotExist;

                if (registration.Confirmed)
                    return ConfirmationOutcome.AlreadyConfirmed;

                conn.Execute("UPDATE [Registrations] SET [Confirmed] = 1 WHERE [ConfirmationCode] = @ConfirmationCode", new { ConfirmationCode = confirmationCode });
                return ConfirmationOutcome.Success;
            }
        }
    }
}