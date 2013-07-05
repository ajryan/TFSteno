using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TFSteno.Models;

namespace TFSteno.ApiControllers
{
    public class EmailController : ApiController
    {
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
                string responseContent;
                if (workItemEmail.WorkItemId == -1)
                {
                    responseContent = "To address must be WorkItemId@tfssteno.aidanjryan.com";
                    Trace.TraceInformation("Bad or missing work item ID in to address.");
                }
                else
                {
                    responseContent = "Appended to history of work item ID " + workItemEmail.WorkItemId;
                    workItemEmail.Save();
                    Trace.TraceInformation("Saved history to workitem id " + workItemEmail.WorkItemId);
                }

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(responseContent)
                };
            }
            catch (Exception ex)
            {
                string errorMessage = "Failed to save work item with exception: " + ex;
                Trace.TraceInformation(errorMessage);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(errorMessage)
                };
            }
        }
    }
}

