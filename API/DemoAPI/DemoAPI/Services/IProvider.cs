using System.Net.Http;
using System.Threading.Tasks;
using DemoAPI.Controllers;
using DemoAPI.Models;
using Google.Cloud.Functions.V1;
using Microsoft.AspNetCore.Http;

namespace DemoAPI.Services
{
    public interface IProvider
    {
        Task SendMail(Mail mail);
        Task UploadImage(IFormFile image);
        Task<HttpResponseMessage> GetEncryptPayload(AkontoWithdrawPayload akontoWithdrawPayload);
    }
}