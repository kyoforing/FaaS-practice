using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DemoAPI.Helpers
{
    public class FileHandler : IFileHandler
    {
        public async Task<(string, string)> GetFilePath(IFormFile file)
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