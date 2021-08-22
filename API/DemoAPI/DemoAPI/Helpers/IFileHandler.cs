using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DemoAPI.Helpers
{
    public interface IFileHandler
    {
        Task<(string, string)> GetFilePath(IFormFile file);
    }
}