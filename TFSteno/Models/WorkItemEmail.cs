﻿using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using TFSteno.Services;

namespace TFSteno.Models
{
    public class WorkItemEmail
    {
        public bool Admin { get; private set; }
        public int WorkItemId { get; private set; }
        public string From { get; private set; }

        public string HistoryText
        {
            get
            {
                return String.Format("<h2>{0} UTC - {1}</h2><div>{2}</div>", DateTime.UtcNow, _subject, _body);
            }
        }

        private string _body;
        private string _subject;

        public WorkItemEmail()
        {
            WorkItemId = -1;
        }

        public void ParsePart(string partName, string partText)
        {
            switch (partName.Replace("\"", String.Empty).ToUpper())
            {
                case "TO":
                    if (partText.Equals("admin", StringComparison.OrdinalIgnoreCase))
                    {
                        Admin = true;
                    }
                    else
                    {
                        int workItemId;
                        string trimmedTo = partText.Replace("\"", String.Empty);
                        Trace.TraceInformation("Parsing ID from " + trimmedTo);
                        if (Int32.TryParse(trimmedTo.Substring(0, trimmedTo.IndexOf('@')), out workItemId))
                            WorkItemId = workItemId;
                    }
                    break;
                case "FROM":
                    From = partText;
                    break;
                case "SUBJECT":
                    _subject = partText;
                    break;
                case "TEXT":
                    if (String.IsNullOrEmpty(_body))
                        _body = partText;
                    break;
                case "HTML":
                    _body = partText;
                    break;
            }
        }

        public void Save()
        {
            var fromAddress = new MailAddress(From);
            var registration = RegistrationService.GetRegistration(fromAddress.Address);

            if (!registration.Confirmed)
            {
                EmailService.SendConfirmationEmail(registration.Email, registration.ConfirmationCode);
                throw new ConfirmationException(registration.Email);
            }
            var teamProjectColl = TeamService.GetCollection(registration.TfsUrl, registration.TfsUsername, registration.TfsPassword);

            var workItemService = teamProjectColl.GetService<WorkItemStore>();
            var workItem = workItemService.GetWorkItem(WorkItemId);
            workItem.History = HistoryText;
            workItem.Save();
        }

        public override string ToString()
        {
            return String.Format("ID: {0}; From: {1}; Text: {2}", WorkItemId, From, HistoryText);
        }
    }
}