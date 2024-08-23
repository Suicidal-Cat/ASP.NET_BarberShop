﻿using BarberShop.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace BarberShopWeb.MobileControllers
{
    [Route("[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DriveController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;

        public DriveController(IHttpClientFactory httpClientFactory,IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
        }

        [HttpPost("upload/{fileName}")]
        public async Task<IActionResult> UploadFile(string fileName)
        {
            // Read the request body
            HttpContext.Request.EnableBuffering();
            var requestBodyStream = new MemoryStream();
            await HttpContext.Request.Body.CopyToAsync(requestBodyStream);
            requestBodyStream.Seek(0, SeekOrigin.Begin);

            // Create a new HTTP request to forward the body
            var client = httpClientFactory.CreateClient();
            string id = Guid.NewGuid().ToString() + "." + fileName.Split('.')[1];
            var forwardRequest = new HttpRequestMessage(HttpMethod.Post, $"{configuration["Drive:Host"]}/api/v2/firms/files/{configuration["Drive:Root"]}/{id}")
            {
                Content = new StreamContent(requestBodyStream)
            };

            // Copy headers if needed
            foreach (var header in HttpContext.Request.Headers)
            {
                if (!forwardRequest.Headers.TryAddWithoutValidation(header.Key, (IEnumerable<string>)header.Value))
                {
                    forwardRequest.Content?.Headers.TryAddWithoutValidation(header.Key, (IEnumerable<string>)header.Value);
                }
            }

            var response = await client.SendAsync(forwardRequest);
            if (response.IsSuccessStatusCode)
                return Ok(new {path= id });
            else
                return BadRequest("Error uploading the file");
        }

        [HttpDelete("delete/{fileName}")]
        public async Task<IActionResult> DeleteImageFromDrive(string fileName)
        {
            var client = httpClientFactory.CreateClient();
            string url = $"{configuration["Drive:Host"]}/api/v2/firms/files/{configuration["Drive:Root"]}/{fileName}";
            var response=await client.DeleteAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return Ok(new { message = "File is successfully deleted" });
            }
                
            else
                return BadRequest("Error deleting the file");
        }

        [HttpGet("downloadLink/{fileName}")]
        public async Task<IActionResult> GetFileContent(string fileName)
        {
            var client = httpClientFactory.CreateClient();
            string url = $"{configuration["Drive:Host"]}/api/v2/firms/files/{configuration["Drive:Root"]}/{fileName}";
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();

                using (JsonDocument document = JsonDocument.Parse(jsonResponse))
                {
                    JsonElement root = document.RootElement;

                    if (root.TryGetProperty("@microsoft.graph.downloadUrl", out JsonElement downloadUrlElement))
                    {
                        string downloadUrl = downloadUrlElement.GetString();
                        return Ok(downloadUrl);
                    }
                };
                return BadRequest("Error while downloading content");
            }
            else return BadRequest("Error while downloading content");
        }

    }
}