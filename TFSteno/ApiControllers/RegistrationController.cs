using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TFSteno.Models;
using TFSteno.Services;

namespace TFSteno.ApiControllers
{
    public class RegistrationController : ApiController
    {
        public int Post([FromBody] Registration registration)
        {
#if !DEBUG
            // Require HTTPS
            if (Request.RequestUri.Scheme != Uri.UriSchemeHttps) throw new HttpResponseException(HttpStatusCode.Forbidden);
#endif

            // TODO: validate syntax. web api dataannotations vaildate support?
            // TODO: validate TFS is real
            // TODO: send a verification email. SendGrid API?
            
            var registerResult = RegistrationService.SaveRegistration(registration);
            if (registerResult.Outcome != RegistrationService.RegistrationOutcome.Success)
            {
                ThrowRegistrationError(registerResult);
            }

            EmailService.SendConfirmationEmail(registration.Email, registerResult.ConfirmationCode);

            return registerResult.Id;
        }

        private static void ThrowRegistrationError(RegistrationService.RegistrationResult registerResult)
        {
            var httpCode = (registerResult.Outcome == RegistrationService.RegistrationOutcome.EmailConflict)
                ? HttpStatusCode.Conflict
                : HttpStatusCode.InternalServerError;
            string fullMessage = registerResult.ErrorMessage;

            if (registerResult.Outcome == RegistrationService.RegistrationOutcome.OtherError)
            {
                var errorGuid = Guid.NewGuid();

                Trace.TraceError("ERROR {0}:\r\n{1}", errorGuid, registerResult.Exception);

                fullMessage = String.Format(
                    "{0}\r\nIf this error persists, please email ryan.aidan@gmail.com and reference error ID {1}.",
                    registerResult.ErrorMessage, errorGuid);
            }

            throw new HttpResponseException(new HttpResponseMessage(httpCode) {Content = new StringContent(fullMessage)});
        }
    }
}