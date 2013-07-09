using System;
using System.Net;
using Microsoft.TeamFoundation.Client;

namespace TFSteno.Services
{
    public static class TeamService
    {
        public static TfsTeamProjectCollection GetCollection(string url, string username, string password)
        {
            var userNameTokens = username.Split(new[] { '\\' }, 2, StringSplitOptions.RemoveEmptyEntries);
            var networkCred = userNameTokens.Length > 1
                ? new NetworkCredential(userNameTokens[1], password, userNameTokens[0])
                : new NetworkCredential(username, password);

            var basicCred = new BasicAuthCredential(networkCred);
            var tfsCred = new TfsClientCredentials(basicCred) { AllowInteractive = false };

            var teamProjectColl = new TfsTeamProjectCollection(new Uri(url), tfsCred);
            teamProjectColl.Authenticate();

            return teamProjectColl;
        }
    }
}