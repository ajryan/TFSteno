using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TFSteno.Models;
using TFSteno.Services;

namespace TFSteno.ApiControllers
{
    public class EmailController : ApiController
    {
        // Handle POST from SendGrid. Message content is irrelevant, just need to send 200 OK,
        // otherwise SendGrid will retry for several days.

        public async Task<HttpResponseMessage> Post(HttpRequestMessage request)
        {
            var workItemEmail = new WorkItemEmail();

            var requestMulti = await request.Content.ReadAsMultipartAsync();
            for (int contentIndex = 0; contentIndex < requestMulti.Contents.Count; contentIndex++)
            {
                var part = requestMulti.Contents[contentIndex];

                string partName = part.Headers.ContentDisposition.Name;
                string partText = await part.ReadAsStringAsync();

                workItemEmail.ParsePart(partName, partText);

                Trace.TraceInformation("Part {0} - {1}: {2}", contentIndex, partName,
                    partText.Substring(0, Math.Min(partText.Length, 25)));
            }
            Trace.TraceInformation("WorkItemEmail: " + workItemEmail.ToString());

            try
            {
                if (workItemEmail.Admin)
                {
                    Trace.TraceInformation("Admin mail received: " + workItemEmail.ToString());
                }
                else if (workItemEmail.WorkItemId == -1)
                {
                    Trace.TraceInformation("Bad or missing work item ID in to address.");
                }
                else
                {
                    Trace.TraceInformation("Save history to workitem id " + workItemEmail.WorkItemId);
                    workItemEmail.Save();
                }
            }
            catch (ConfirmationException ex)
            {
                Trace.TraceInformation("Got work item email from un-confirmed email.");
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("Failed to save work item with exception: " + ex);
                EmailService.SendFailureEmail(workItemEmail);
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}

