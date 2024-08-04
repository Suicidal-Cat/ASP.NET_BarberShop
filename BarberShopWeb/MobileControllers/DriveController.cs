using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;

namespace BarberShopWeb.MobileControllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
    }
}
