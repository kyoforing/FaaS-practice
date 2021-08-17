using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using DemoAPI.Models;
using Microsoft.AspNetCore.Http;

namespace DemoAPI.Services
{
    public class AWSService : IProvider
    {
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
            var (fileName, filePath) = await GetFilePath(file);

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

        private async Task<(string, string)> GetFilePath(IFormFile file)
        {
            var folderName = Path.Combine("Resources", "Images");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"');
            var filePath = Path.Combine(pathToSave, fileName!);

            if (file.Length > 0)
            {
                await using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);
            }

            return (fileName, filePath);
        }
    }
}