using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using DemoAPI.Controllers;
using DemoAPI.Helpers;
using DemoAPI.Models;
using Google.Cloud.Functions.V1;
using Microsoft.AspNetCore.Http;

namespace DemoAPI.Services
{
    public class AWSService : IProvider
    {
        private readonly IFileHandler _fileHandler;

        public AWSService(IFileHandler fileHandler)
        {
            _fileHandler = fileHandler;
        }

        public async Task SendMail(Mail mail)
        {
            using var client = new AmazonSimpleEmailServiceClient(RegionEndpoint.APNortheast1);

            await client.SendEmailAsync(new SendEmailRequest
            {
                Source = "kyoforing@gmail.com",
                Destination = new Destination
                {
                    ToAddresses =
                        new List<string> { mail.ReceiverMail }
                },
                Message = new Message
                {
                    Subject = new Content("Test mail"),
                    Body = new Body
                    {
                        Html = new Content
                        {
                            Charset = "UTF-8",
                            Data = mail.Content
                        }
                    }
                }
            });
        }

        public async Task UploadImage(IFormFile file)
        {
            var (fileName, filePath) = await _fileHandler.GetFilePath(file);

            var objectRequest = new PutObjectRequest
            {
                BucketName = "kyo-demo",
                FilePath = filePath,
                Key = fileName,
                CannedACL = S3CannedACL.PublicRead
            };
            var s3Client = new AmazonS3Client(RegionEndpoint.APNortheast1);
            await s3Client.PutObjectAsync(objectRequest);
        }

        public Task<HttpResponseMessage> GetEncryptPayload(AkontoWithdrawPayload akontoWithdrawPayload)
        {
            throw new System.NotImplementedException();
        }
    }
}