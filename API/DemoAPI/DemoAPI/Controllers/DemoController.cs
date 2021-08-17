using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DemoAPI.Enums;
using DemoAPI.Models;
using DemoAPI.Resolver;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DemoAPI.Controllers
{
    [ApiController]
    public class DemoController : ControllerBase
    {
        private readonly ProviderResolver _providerResolver;

        public DemoController(ProviderResolver providerResolver)
        {
            _providerResolver = providerResolver;
        }

        [HttpPost("api/v1/mail")]
        public async Task<IActionResult> SendMail([FromBody] Mail mail)
        {
            await _providerResolver(mail.Provider).SendMail(mail);
            return Ok();
        }

        [HttpPost("api/v1/image"), DisableRequestSizeLimit]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            await _providerResolver(Provider.AWS).UploadImage(file);
            return Ok();
        }
    }
}