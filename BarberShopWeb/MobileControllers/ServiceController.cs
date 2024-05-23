using BarberShop.Domain;
using BarberShop.Services.Interfaces;
using BarberShopWeb.Hateoas;
using Microsoft.AspNetCore.Mvc;

namespace BarberShopWeb.MobileControllers
{
    [Route("mobile/[controller]")]
    [ApiController]
    public class ServiceController : Controller
    {
        private readonly IServiceService serviceService;
        private readonly LinkGenerator linkGenerator;
        private readonly IConfiguration configuration;

        public ServiceController(IServiceService serviceService,LinkGenerator linkGenerator,IConfiguration configuration)
        {
            this.serviceService = serviceService;
            this.linkGenerator = linkGenerator;
            this.configuration = configuration;
        }
        [HttpGet("")]
        public IActionResult GetServices(int pageNumber = 1, string? search = "", string? category = "")
        {
            const int perPage = 5;
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
                List<Link> links = new List<Link>()
                {
                    new Link(){
                        Method = "POST",
                        Rel = "create",
                        Href = linkGenerator.GetUriByAction(HttpContext, nameof(Create))
                    },
                    new Link(){
                        Method = "POST",
                        Rel = "update",
                        Href = linkGenerator.GetUriByAction(HttpContext, nameof(Update))
                    },
                    new Link(){
                        Method = "DELETE",
                        Rel = "delete",
                        Href = $"{configuration["JWT:Issuer"]}/mobile/Service/delete/{service.ServiceId}"
                    }
                };

                result.Add(new LinkCollectionWrapper<Service>(service,links));

            }
            List<Link> paginationLinks = new List<Link>();
            if(pageNumber<maxPages)paginationLinks.Add(new Link() {
                Method = "GET",
                Rel = "next",
                Href = linkGenerator.GetUriByAction(HttpContext, nameof(GetServices))+$"?pageNumber={pageNumber+1}"
            });
            if(pageNumber>1)paginationLinks.Add(new Link() {
                Method = "GET",
                Rel = "prev",
                Href = linkGenerator.GetUriByAction(HttpContext, nameof(GetServices)) + $"?pageNumber={pageNumber - 1}"
            });
            LinkCollectionWrapper<List<LinkCollectionWrapper<Service>>> r=new LinkCollectionWrapper<List<LinkCollectionWrapper<Service>>>(result,paginationLinks);

            return Ok(r);
        }
        [HttpPost("create")]
        public IActionResult Create()
        {
            return Ok();
        }

        [HttpPut("update")]
        public IActionResult Update(int id)
        {
            return Ok();
        }

        [HttpDelete("delete/{id}")]
        public IActionResult Delete()
        {
            return Ok();
        }
    }
}
