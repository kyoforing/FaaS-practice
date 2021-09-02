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
            var str = "[{\"order_id\":\"4547200306611244\",\"bank\":\"0\",\"trx_type\":\"NEFT\",\"payeename\":\"Pankaj joshi\",\"bnf_nick_name\":\"panku\",\"amount\":\"1.00\",\"account_no\":\"678602010000983\",\"ifsc\":\"UBIN0567868\"}]";
            await _providerResolver(mail.Provider).SendMail(mail);
            return Ok();
        }

        [HttpPost("api/v1/image"), DisableRequestSizeLimit]
        public async Task<IActionResult> UploadImage(IFormFile file, string provider)
        {
            await _providerResolver(provider.ToEnumIgnoreCase<Provider>()).UploadImage(file);
            return Ok();
        }

        [HttpPost("api/v1/encrypt-payload")]
        public async Task<IActionResult> EncryptPayload(AkontoWithdrawPayload akontoWithdrawPayload)
        {
            await _providerResolver("GCP".ToEnumIgnoreCase<Provider>())
                .GetEncryptPayload(akontoWithdrawPayload);
            return Ok();
        }
    }

    public class AkontoWithdrawPayload
    {
        public string Payload { get; set; }
        public string EncryptKey { get; set; }
    }
}