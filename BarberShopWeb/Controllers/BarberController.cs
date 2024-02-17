using BarberShop.Domain;
using BarberShop.Services.Interfaces;
using BarberShop.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BarberShopWeb.Controllers
{
    [Authorize(Roles = UserRoles.Role_Admin)]
    public class BarberController : Controller
	{
		private readonly IBarberService barberService;
		private readonly IWebHostEnvironment webHostEnvironment;

		public BarberController(IBarberService barberService,IWebHostEnvironment webHostEnvironment)
        {
			this.barberService = barberService;
			this.webHostEnvironment = webHostEnvironment;
		}
        public IActionResult Index()
		{

			return View(barberService.Barbers);
		}
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Barber barber,IFormFile? file)
        {
            if(ModelState.IsValid)
            {
                string wwwRootPath = webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string imagePath = Path.Combine(wwwRootPath, @"images\barber");
                    using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    barber.ImageUrl = @"\images\barber\" + fileName;
					barberService.Add(barber);
					TempData["success"] = "Barber created successfully";
					return RedirectToAction("Index");
				}
                else
                {
					TempData["error"] = "Please select image!";
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
        public IActionResult Update(Barber barber, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string imagePath = Path.Combine(wwwRootPath, @"images\barber");

                    if(!string.IsNullOrEmpty(barber.ImageUrl))
                    {
                        //delete image
                        var oldImage = Path.Combine(wwwRootPath, barber.ImageUrl.TrimStart('\\'));
                        if(System.IO.File.Exists(oldImage))
                        {
                            System.IO.File.Delete(oldImage);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    barber.ImageUrl = @"\images\barber\" + fileName;
                }
				barberService.Update(barber);
				TempData["success"] = "Barber updated successfully";
				return RedirectToAction("Index");

			}
            return View(barber);
        }

    }
}
