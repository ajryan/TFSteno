using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dapper;
using TFSteno.Models;

namespace TFSteno.ApiControllers
{
    public class RegistrationController : ApiController
    {
        private static readonly string _ConnectionString = 
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public int Post([FromBody] Registration registration)
        {
#if !DEBUG
            // Require HTTPS
            if (Request.RequestUri.Scheme != Uri.UriSchemeHttps)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
#endif

            // TODO: validate syntax. web api dataannotations vaildate support?
            // TODO: validate TFS is real
            // TODO: send a verification email. SendGrid API?
            try
            {
                registration.TfsUrl = Crypt.Encrypt(registration.TfsUrl);
                registration.TfsUsername = Crypt.Encrypt(registration.TfsUsername);
                registration.TfsPassword = Crypt.Encrypt(registration.TfsPassword);

                using (var conn = new SqlConnection(_ConnectionString))
                {
                    conn.Open();

                    int id = conn.Execute(
                        "INSERT [Registrations] (Email, TfsUrl, TfsUsername, TfsPassword) VALUES (@Email, @TfsUrl, @TfsUsername, @TfsPassword);" +
                        "SELECT cast(scope_identity() as int);", registration);

                    return id;
                }
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Errors.Cast<SqlError>().Any(e => e.Number == 2601))
                {
                    throw new HttpResponseException(
                        new HttpResponseMessage(HttpStatusCode.Conflict)
                        {
                            Content = new StringContent(String.Format("A user with the address {0} is already signed up.", registration.Email))
                        });
                }
                
                ThrowGenericError("A database error occurred.", sqlEx);
            }
            catch (Exception ex)
            {
                ThrowGenericError("An unexpected error occurred.", ex);
            }
            return -1;
        }

        private void ThrowGenericError(string message, Exception exception)
        {
            var errorGuid = Guid.NewGuid();

            Trace.TraceError("ERROR {0}:\r\n{1}", errorGuid, exception);

            string fullMessage = String.Format(
                "{0}\r\nIf this error persists, please email ryan.aidan@gmail.com and reference error ID {1}.",
                message, errorGuid);

            throw new HttpResponseException(
                new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content= new StringContent(fullMessage)
                });
        }
    }
}