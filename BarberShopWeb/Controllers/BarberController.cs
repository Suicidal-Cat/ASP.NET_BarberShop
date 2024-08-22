using BarberShop.Domain;
using BarberShop.Services.ImplementationDatabase;
using BarberShop.Services.Interfaces;
using BarberShop.Utils;
using BarberShopWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BarberShopWeb.Controllers
{
    [Authorize(Roles = UserRoles.Role_Admin)]
    public class BarberController : Controller
    {
        private readonly IBarberService barberService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;

        public BarberController(IBarberService barberService, IWebHostEnvironment webHostEnvironment, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.barberService = barberService;
            this.webHostEnvironment = webHostEnvironment;
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
        }
        public IActionResult Index(int pageNumber = 1, string search = "")
        {
            const int perPage = 3;
            search = search ?? "";
            int maxPages = 1;
            IEnumerable<Barber> model;

            if (search == "") model = barberService.Barbers;
            else model = barberService.SearchByName(search);

            maxPages = (int)Math.Ceiling((double)model.Count() / perPage);
            model = model.Skip((pageNumber - 1) * perPage).Take(perPage);

            IndexBarbersPaginationVM vm = new IndexBarbersPaginationVM();
            vm.Barbers = model;
            vm.CurrentPage = pageNumber;
            vm.Search = search;
            if (pageNumber > 1) vm.Prev = true;
            else vm.Prev = false;
            if (pageNumber < maxPages) vm.Next = true;
            else vm.Next = false;

            return View(vm);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Barber barber, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(barber.StartWorkingHours) && string.IsNullOrEmpty(barber.EndWorkingHours) || (string.Compare(barber.StartWorkingHours, barber.EndWorkingHours) < 0))

                {
                    //string wwwRootPath = webHostEnvironment.WebRootPath;
                    if (file != null)
                    {
                        /*string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
						string imagePath = Path.Combine(wwwRootPath, @"images\barber");
						using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
						{
							file.CopyTo(fileStream);
						}
						barber.ImageUrl = @"\images\barber\" + fileName;
						barberService.Add(barber);
						TempData["success"] = "Barber created successfully";
						return RedirectToAction("Index");*/
                        var client = httpClientFactory.CreateClient();
                        string url = $"{configuration["JWT:Issuer"]}/Drive/upload/{file.FileName}";

                        using (var memoryStream = new MemoryStream())
                        {
                            await file.CopyToAsync(memoryStream);
                            memoryStream.Seek(0, SeekOrigin.Begin);

                            var streamContent = new StreamContent(memoryStream);
                            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

                            var response = await client.PostAsync(url, streamContent);

                            if (response.IsSuccessStatusCode==false)
                            {
                                TempData["error"] = "Error while uploading the file!";
                                return View(barber);
                            }
                            else
                            {
                                var jsonString = await response.Content.ReadAsStringAsync();
                                var jsonDocument = JsonDocument.Parse(jsonString);
                                string path = jsonDocument.RootElement.GetProperty("path").GetString();


                                barber.ImageUrl = path;
                                barberService.Add(barber);
                                TempData["success"] = "Barber created successfully";
                                return RedirectToAction("Index");
                            }
                        }

                    }
                    else
                    {
                        TempData["error"] = "Please select image!";
                        return View(barber);
                    }
                }
                else
                {
                    TempData["error"] = "Please select valid time!";
                    return View(barber);
                }

            }
            return View(barber);
        }
        public IActionResult Update(int id)
        {
            return View(barberService.Get(id));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Barber barber, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                //string wwwRootPath = webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    /*string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string imagePath = Path.Combine(wwwRootPath, @"images\barber");*/

                    /*if (!string.IsNullOrEmpty(barber.ImageUrl))
                    {
                        //delete image
                        var oldImage = Path.Combine(wwwRootPath, barber.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImage))
                        {
                            System.IO.File.Delete(oldImage);
                        }
                    }*/

                    /*using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    barber.ImageUrl = @"\images\barber\" + fileName;*/


                    var client = httpClientFactory.CreateClient();

                    string urlDelete= $"{configuration["JWT:Issuer"]}/Drive/delete/{barber.ImageUrl}";
                    await client.DeleteAsync(urlDelete);
                    string url = $"{configuration["JWT:Issuer"]}/Drive/upload/{file.FileName}";

                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);

                        var streamContent = new StreamContent(memoryStream);
                        streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

                        var response = await client.PostAsync(url, streamContent);

                        if (response.IsSuccessStatusCode == false)
                        {
                            TempData["error"] = "Error while uploading the file!";
                            return View(barber);
                        }
                        else
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var jsonDocument = JsonDocument.Parse(jsonString);
                            string path = jsonDocument.RootElement.GetProperty("path").GetString();
                            barber.ImageUrl = path;
                        }
                    }
                }
                barberService.Update(barber);
                TempData["success"] = "Barber updated successfully";
                return RedirectToAction("Index");

            }
            return View(barber);
        }

    }
}
