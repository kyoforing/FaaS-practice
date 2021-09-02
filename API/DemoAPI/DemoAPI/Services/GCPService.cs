using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DemoAPI.Controllers;
using DemoAPI.Helpers;
using DemoAPI.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Cloud.Functions.V1;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerUI;
using static Google.Apis.Auth.OAuth2.GoogleWebAuthorizationBroker;

namespace DemoAPI.Services
{
    public class GCPService : IProvider
    {
        private readonly IFileHandler _fileHandler;

        public GCPService(IFileHandler fileHandler)
        {
            _fileHandler = fileHandler;
        }

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

        public async Task UploadImage(IFormFile file)
        {
            var (fileName, filePath) = await _fileHandler.GetFilePath(file);
            var googleCredential = GoogleCredential.FromFile("Properties/storage-secret.json").CreateScoped("https://www.googleapis.com/auth/cloud-platform");
            var storage = await StorageClient.CreateAsync(googleCredential);
            await using var fileStream = File.OpenRead(filePath);
            storage.UploadObject("kyo-demo", fileName, null, fileStream);
        }

        public async Task<string> GetEncryptPayload(AkontoWithdrawEncryptPayload akontoWithdrawEncryptPayload)
        {
            var functionUrl = "https://asia-east1-helpful-kingdom-308211.cloudfunctions.net/akonto-withdraw-encrypt-payload";
            var requestBody = JsonConvert.SerializeObject(akontoWithdrawEncryptPayload);
            var token = await GetJwtToken(functionUrl);

            return await ReadAsStringAsync(token, requestBody, functionUrl);
        }

        public async Task<string> GetDecryptPayload(AkontoWithdrawDecryptPayload akontoWithdrawDecryptPayload)
        {
            var functionUrl = "https://asia-east1-helpful-kingdom-308211.cloudfunctions.net/akonto-withdraw-decrypt-payload";
            var requestBody = JsonConvert.SerializeObject(akontoWithdrawDecryptPayload);
            var token = await GetJwtToken(functionUrl);

            return await ReadAsStringAsync(token, requestBody, functionUrl);
        }

        private static async Task<string> ReadAsStringAsync(string token, string serializeObject, string functionUrl)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var requestContent = new StringContent(serializeObject, Encoding.UTF8, "application/json");
            var httpResponseMessage = await client.PostAsync(functionUrl, requestContent);
            var readAsStringAsync = await httpResponseMessage.Content.ReadAsStringAsync();
            return readAsStringAsync;
        }

        private static async Task<string> GetJwtToken(string functionUrl)
        {
            var oidcToken = await GoogleCredential
                .FromFile("Properties/cloud-storage.json")
                .GetOidcTokenAsync(OidcTokenOptions.FromTargetAudience(functionUrl));

            return await oidcToken.GetAccessTokenAsync();
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