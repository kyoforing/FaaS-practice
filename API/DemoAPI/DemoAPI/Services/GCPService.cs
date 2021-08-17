﻿using System;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DemoAPI.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Http;
using static Google.Apis.Auth.OAuth2.GoogleWebAuthorizationBroker;

namespace DemoAPI.Services
{
    public class GCPService : IProvider
    {
        public async Task SendMail(Mail mail)
        {
            var service = new GmailService(await GoogleOAuthInitializer());

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var msg = new AE.Net.Mail.MailMessage
            {
                Subject = "Test Mail",
                Body = mail.Content,
                From = new MailAddress("kyoforing@gmail.com")
            };

            msg.To.Add(new MailAddress(mail.ReceiverMail));
            msg.ReplyTo.Add(msg.From);

            var msgStr = new StringWriter();
            msg.Save(msgStr);
            var newMsg = new Message { Raw = Base64UrlEncode(msgStr.ToString()) };

            await service.Users.Messages.Send(newMsg, "me").ExecuteAsync();
        }

        public Task UploadImage(IFormFile image)
        {
            throw new NotImplementedException();
        }

        public static string Base64UrlEncode(string input)
        {
            var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(inputBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }

        private static async Task<BaseClientService.Initializer> GoogleOAuthInitializer()
        {
            return new BaseClientService.Initializer
            {
                HttpClientInitializer = await LoadUserCredential(),
                ApplicationName = "Mail Sender",
            };
        }

        private static async Task<UserCredential> LoadUserCredential()
        {
            await using var stream = new FileStream("Properties/client_secret.json", FileMode.Open, FileAccess.Read);

            var credential = await AuthorizeAsync(
                (await GoogleClientSecrets.FromStreamAsync(stream)).Secrets,
                new[] { GmailService.Scope.GmailSend },
                "user",
                CancellationToken.None,
                new FileDataStore("Properties/token.json", true));

            return credential;
        }
    }
}