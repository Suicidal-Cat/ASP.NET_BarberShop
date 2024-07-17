using BarberShop.Domain;
using BarberShop.Services.Interfaces;
using BarberShop.Utils;
using BarberShopWeb.Hateoas;
using BarberShopWeb.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;

namespace BarberShopWeb.MobileControllers
{
    [Route("mobile/[controller]")]
    [ApiController]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

	public class ServiceMobileController : Controller
    {
        private readonly IServiceService serviceService;
        private readonly LinkGenerator linkGenerator;
        private readonly IConfiguration configuration;
        private readonly IServiceCategoryService serviceCategory;

        public ServiceMobileController(IServiceService serviceService,LinkGenerator linkGenerator,IConfiguration configuration,IServiceCategoryService serviceCategory)
        {
            this.serviceService = serviceService;
            this.linkGenerator = linkGenerator;
            this.configuration = configuration;
            this.serviceCategory = serviceCategory;
        }

        [HttpGet("")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Role_Admin)]
        public IActionResult GetServicesPagination(int pageNumber = 1, string? search = "", string? category = "")
        {
            const int perPage = 4;
            search = search ?? "";
            category = category ?? "";
            IEnumerable<Service> services;
            int maxPages = 1;
            if (search == "")
            {
                if (category != "") services = serviceService.Services.Where(s => s.ServiceCategory.Name.ToLower() == category.ToLower());
                else services = serviceService.Services;
            }
            else
            {
                services = serviceService.SearchByName(search);
                if (category != "") services = services.Where(services => services.ServiceCategory.Name.ToLower() == category.ToLower());
            }
            maxPages = (int)Math.Ceiling((double)services.Count() / perPage);
            services = services.Skip((pageNumber - 1) * perPage).Take(perPage);
            List<LinkCollectionWrapper<Service>> result = new List<LinkCollectionWrapper<Service>>();

            foreach (var service in services)
            {
                result.Add(new LinkCollectionWrapper<Service>(service,GenerateLinksForService(service)));

            }

            List<Link> paginationLinks = new List<Link>();

            string queryParameters = $"?pageNumber={pageNumber}";
            queryParameters += search != "" ? $"&search={search}" : "";
            queryParameters += category != "" ? $"&category={category}" : "";
            paginationLinks.Add(new Link()
            {
                Method = "GET",
                Rel = "curr",
                Href = linkGenerator.GetUriByAction(HttpContext, nameof(GetServicesPagination)) + queryParameters
            });


            if (pageNumber < maxPages)
            {
                queryParameters = $"?pageNumber={pageNumber + 1}";
                queryParameters += search != "" ? $"&search={search}" : "";
                queryParameters += category != "" ? $"&category={category}" : "";
                paginationLinks.Add(new Link()
                {
                    Method = "GET",
                    Rel = "next",
                    Href = linkGenerator.GetUriByAction(HttpContext, nameof(GetServicesPagination)) + queryParameters
                });
            }

            if (pageNumber > 1)
            {
                queryParameters = $"?pageNumber={pageNumber - 1}";
                queryParameters += search != "" ? $"&search={search}" : "";
                queryParameters += category != "" ? $"&category={category}" : "";

                paginationLinks.Add(new Link()
                {
                    Method = "GET",
                    Rel = "prev",
                    Href = linkGenerator.GetUriByAction(HttpContext, nameof(GetServicesPagination))+queryParameters
                });
            }

            LinkCollectionWrapper<List<LinkCollectionWrapper<Service>>> r=new LinkCollectionWrapper<List<LinkCollectionWrapper<Service>>>(result,paginationLinks);

            return Ok(r);
        }
        [HttpPost("create")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Role_Admin)]
        public IActionResult Create(Service service)
        {
            service.ServiceCategory = serviceCategory.ServiceCategories.Single(sc => sc.Id == service.ServiceCategory.Id);
            serviceService.Add(service);

            return Ok(new{message="Service is successfully created"});
        }

        [HttpPut("update")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Role_Admin)]
        public IActionResult Update(Service service)
        {
            if(service.ServiceId==0) return BadRequest();
            service.ServiceCategory = serviceCategory.ServiceCategories.Single(sc => sc.Id == service.ServiceCategory.Id);
            serviceService.Update(service);

            return Ok(new { message = "Service is successfully updated" });
        }

        [HttpDelete("delete/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Role_Admin)]
        public IActionResult Delete(int id)
        {
            try
            {
                serviceService.Delete(id);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Unable to delete selected service!" });
            }

            return Ok(new { message = "Service is successfully deleted" });
        }

        [HttpGet("getServiceCategories")]
        public IActionResult GetServiceCategories()
        {
            return Ok(new LinkCollectionWrapper<IEnumerable<ServiceCategory>>(serviceCategory.ServiceCategories, null));

        }
        [HttpGet("{id}")]
        public IActionResult GetService(int id)
        {
            Service service=serviceService.Get(id);
            return Ok(new LinkCollectionWrapper<Service>(service, GenerateLinksForService(service)));

        }

        [HttpGet("all")]
        public IActionResult GetServices()
        {
            List<Service> services = serviceService.Services.ToList();
            LinkCollectionWrapper<List<LinkCollectionWrapper<Service>>> result=new LinkCollectionWrapper<List<LinkCollectionWrapper<Service>>>();
            result.Value = new List<LinkCollectionWrapper<Service>>();
            foreach(Service service in services)
            {
                result.Value.Add(new LinkCollectionWrapper<Service>(service,GenerateLinksForService(service)));
            }
            return Ok(result);
        }




        ///////////////////////////////
        private List<Link> GenerateLinksForService(Service service)
        {
            List<Link> links = new List<Link>()
                {
                    new Link(){
                        Method = "GET",
                        Rel = "get",
                        Href = linkGenerator.GetUriByAction(HttpContext,action: nameof(GetService),
                                controller: "ServiceMobile",
                                values: new { id = service.ServiceId })
                    },
                    new Link(){
                        Method = "PUT",
                        Rel = "update",
                        Href = linkGenerator.GetUriByAction(HttpContext, nameof(Update))
                    },
                    new Link(){
                        Method = "DELETE",
                        Rel = "delete",
                        Href = linkGenerator.GetUriByAction(httpContext: HttpContext,
                                action: nameof(Delete),
                                controller: "ServiceMobile",
                                values: new { id = service.ServiceId })
                    }
            };
            return links;
        }
    }
}
