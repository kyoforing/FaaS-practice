using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DemoAPI.Enums;
using DemoAPI.Models;
using DemoAPI.Resolver;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrexDino.Extensions;

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
        public async Task<IActionResult> UploadImage(IFormFile file, string provider)
        {
            await _providerResolver(provider.ToEnumIgnoreCase<Provider>()).UploadImage(file);
            return Ok();
        }
    }
}