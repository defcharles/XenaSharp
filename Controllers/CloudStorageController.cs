using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Linq;

namespace XenaSharp.Controllers
{
    [ApiController]
    public class CloudStorageController : ControllerBase
    {
        private string HotfixDirectory = Path.Combine(Directory.GetCurrentDirectory(), "static", "hotfixes");

        [HttpGet("/fortnite/api/cloudstorage/system")]
        public IActionResult GetCloudStorageSystem()
        {
            try
            {
                var files = Directory.GetFiles(HotfixDirectory)
                    .Select(filePath =>
                    {
                        var file = new FileInfo(filePath);
                        var content = System.IO.File.ReadAllBytes(filePath);
                        var sha1Hash = BitConverter.ToString(SHA1.Create().ComputeHash(content)).Replace("-", "").ToLower();
                        var sha256Hash = BitConverter.ToString(SHA256.Create().ComputeHash(content)).Replace("-", "").ToLower();

                        return new
                        {
                            uniqueFilename = file.Name,
                            filename = file.Name,
                            hash = sha1Hash,
                            hash256 = sha256Hash,
                            length = file.Length,
                            contentType = "application/octet-stream",
                            uploaded = file.CreationTimeUtc.ToString("o"),
                            storageType = "S3",
                            storageIds = new { },
                            doNotCache = true
                        };
                    })
                    .ToList();

                return Ok(files);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching cloudstorage: " + ex.Message);
                return NotFound();
            }
        }

        [HttpGet("/fortnite/api/cloudstorage/system/config")]
        public IActionResult GetCloudStorageSystemConfig()
        {
            try
            {
                var files = Directory.GetFiles(HotfixDirectory)
                    .Select(filePath =>
                    {
                        var file = new FileInfo(filePath);
                        var content = System.IO.File.ReadAllBytes(filePath);
                        var sha1Hash = BitConverter.ToString(SHA1.Create().ComputeHash(content)).Replace("-", "").ToLower();
                        var sha256Hash = BitConverter.ToString(SHA256.Create().ComputeHash(content)).Replace("-", "").ToLower();

                        return new
                        {
                            uniqueFilename = file.Name,
                            filename = file.Name,
                            hash = sha1Hash,
                            hash256 = sha256Hash,
                            length = file.Length,
                            contentType = "application/octet-stream",
                            uploaded = file.CreationTimeUtc.ToString("o"),
                            storageType = "S3",
                            storageIds = new { },
                            doNotCache = true
                        };
                    })
                    .ToList();

                return Ok(files);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching cloudstorage config: " + ex.Message);
                return NotFound();
            }
        }

        [HttpGet("/fortnite/api/cloudstorage/system/{file}")]
        public IActionResult GetCloudStorageFile(string file)
        {
            try
            {
                var filePath = Path.Combine(HotfixDirectory, file);
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound();
                }

                var content = System.IO.File.ReadAllText(filePath);
                return Content(content, "application/octet-stream");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching file: " + ex.Message);
                return NotFound();
            }
        }

        [HttpGet("/fortnite/api/cloudstorage/user/{accountId}")]
        public IActionResult GetUserCloudStorage(string accountId)
        {
            return Ok(new
            {
                status = "OK",
                code = 200
            });
        }

        [HttpPut("/fortnite/api/cloudstorage/user/{accountId}/{fileName}")]
        public IActionResult UpdateUserCloudStorage(string accountId, string fileName)
        {
            return Ok(new
            {
                status = "OK",
                code = 200
            });
        }
    }
}
