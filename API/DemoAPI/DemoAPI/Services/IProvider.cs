using System.Threading.Tasks;
using DemoAPI.Controllers;
using DemoAPI.Models;
using Microsoft.AspNetCore.Http;

namespace DemoAPI.Services
{
    public interface IProvider
    {
        Task SendMail(Mail mail);
        Task UploadImage(IFormFile image);
    }
}