using BarberShop.Domain;
using BarberShop.Services.Interfaces;
using BarberShop.Utils;
using BarberShopWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BarberShopWeb.Controllers
{
    [Authorize(Roles = UserRoles.Role_Admin)]
    public class ServiceController : Controller
    {
        private readonly IServiceService serviceService;
        private readonly IServiceCategoryService serviceCategory;

        public ServiceController(IServiceService serviceService,IServiceCategoryService serviceCategory)
        {
            this.serviceService = serviceService;
            this.serviceCategory = serviceCategory;
        }
        public IActionResult Index()
        {
            return View(serviceService.Services);
        }
        public IActionResult Create()
        {
            AddServiceVM vm = new AddServiceVM
            {
                Service=new Service(),
                ServiceCategory = serviceCategory.ServiceCategories.Select(sc => new SelectListItem
                {
                    Text = sc.Name,
                    Value = sc.Id.ToString()
                }).ToList()
			};
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AddServiceVM addServiceVM)
        {
            if (ModelState.IsValid)
            {
                serviceService.Add(new Service
                {
                    Name = addServiceVM.Service.Name,
                    Price = addServiceVM.Service.Price,
                    Duration = addServiceVM.Service.Duration,
                    ServiceCategory = serviceCategory.ServiceCategories.Single(sc => sc.Id == addServiceVM.Service.ServiceCategory.Id)
                });
                TempData["success"] = "Service created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                addServiceVM.Service=new Service();
                return View(addServiceVM);
            }

        }
		public IActionResult Update(int id)
		{
			AddServiceVM vm = new AddServiceVM
			{
				Service = serviceService.Get(id),
				ServiceCategory = serviceCategory.ServiceCategories.Select(sc => new SelectListItem
				{
					Text = sc.Name,
					Value = sc.Id.ToString()
				}).ToList()
			};
			return View(vm);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Update(AddServiceVM addServiceVM)
		{
			if (ModelState.IsValid)
			{
                Service service = addServiceVM.Service;
                service.ServiceCategory = serviceCategory.ServiceCategories.Single(sc => sc.Id == addServiceVM.Service.ServiceCategory.Id);
                serviceService.Update(service);
				TempData["success"] = "Service updated successfully";
				return RedirectToAction("Index");
			}
			else
			{
				addServiceVM.Service = new Service();
				return View(addServiceVM);
			}

		}
        public IActionResult Delete(int id)
        {
            serviceService.Delete(id);
            return RedirectToAction("Index");
        }

	}
}
