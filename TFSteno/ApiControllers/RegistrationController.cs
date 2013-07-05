using System;
using System.Configuration;
using System.Data.SqlClient;
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

        public Registration Post([FromBody] Registration registration)
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
            // TODO: unique constraint on email, catch specific error number and return specific message
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

                    registration.Id = id;
                    return registration;
                }
            }
            catch (SqlException sqlEx)
            {
                string errorMessage = sqlEx.Errors.Cast<SqlError>().Any(e => e.Number == 2601)
                    ? String.Format("A user with the address {0} is already signed up.", registration.Email)
                    : "A database error occurred.";

                throw new HttpResponseException(
                    new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(errorMessage) });
            }
            catch (Exception)
            {
                // TODO: log with an ID and give the ID in response so they can ask about it. ELMAH?
                throw new HttpResponseException(
                    new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content= new StringContent("Unexpected error") });
            }
        }
    }
}