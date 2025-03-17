using BarberShop.Domain;
using BarberShop.Services.ImplementationDatabase;
using BarberShop.Services.Interfaces;
using BarberShop.Services.JWT;
using BarberShop.Utils;
using BarberShopWeb.Hateoas;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Net.Http;
using System.Numerics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BarberShopWeb.MobileControllers
{
    [Route("mobile/[controller]")]
    [ApiController]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class BarberMobileController : Controller
    {
        private readonly IBarberService barberService;
        private readonly LinkGenerator linkGenerator;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;

        public BarberMobileController(IBarberService barberService,LinkGenerator linkGenerator, IHttpClientFactory httpClientFactory,IConfiguration configuration)
        {
            this.barberService = barberService;
            this.linkGenerator = linkGenerator;
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
        }


        [HttpGet("")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Role_Admin)]
        public async Task<IActionResult> GetBarbersPagination(int pageNumber = 1, string? search = "")
        {
            const int perPage = 4;
            search = search ?? "";
            int maxPages = 1;
            IEnumerable<Barber> model;

            if (search == "") model = barberService.Barbers;
            else model = barberService.SearchByName(search);

            maxPages = (int)Math.Ceiling((double)model.Count() / perPage);
            model = model.Skip((pageNumber - 1) * perPage).Take(perPage);

            List<LinkCollectionWrapper<Barber>> result = new List<LinkCollectionWrapper<Barber>>();

            foreach (Barber barber in model)
            {
                var client = httpClientFactory.CreateClient();
                string url = $"{configuration["JWT:Issuer"]}/Drive/downloadLink/{barber.ImageUrl}";
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var imagePath = await response.Content.ReadAsStringAsync();
                    barber.ImageUrl = imagePath;
                }
                else
                {
                    barber.ImageUrl = "";
                }
                result.Add(new LinkCollectionWrapper<Barber>(barber, GenerateLinksForBarber(barber)));
            }

            List<Link> paginationLinks = new List<Link>();

            string queryParameters = $"?pageNumber={pageNumber + 1}";
            queryParameters += search != "" ? $"&search={search}" : "";
            paginationLinks.Add(new Link()
            {
                Method = "GET",
                Rel = "curr",
                Href = linkGenerator.GetUriByAction(HttpContext, nameof(GetBarbersPagination)) + queryParameters
            });


            if (pageNumber < maxPages)
            {
                queryParameters = $"?pageNumber={pageNumber + 1}";
                queryParameters += search != "" ? $"&search={search}" : "";

                paginationLinks.Add(new Link()
                {
                    Method = "GET",
                    Rel = "next",
                    Href = linkGenerator.GetUriByAction(HttpContext, nameof(GetBarbersPagination))+ queryParameters
                });
            }
            if (pageNumber > 1)
            {
                queryParameters = $"?pageNumber={pageNumber - 1}";
                queryParameters += search != "" ? $"&search={search}" : "";
                paginationLinks.Add(new Link()
                {
                    Method = "GET",
                    Rel = "prev",
                    Href = linkGenerator.GetUriByAction(HttpContext, nameof(GetBarbersPagination)) + queryParameters
                });
            }


            LinkCollectionWrapper<List<LinkCollectionWrapper<Barber>>> r = new LinkCollectionWrapper<List<LinkCollectionWrapper<Barber>>>(result, paginationLinks);

            HttpContext.Response.Headers.Add("maxPages", maxPages.ToString());
            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "maxPages");

            return Ok(r);
        }


        [HttpPost("create")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Role_Admin)]
        public IActionResult Create(Barber barber)
        {
            barberService.Add(barber);

            return Ok(new { message = "Barber is successfully created" });
        }



        [HttpPut("update")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Role_Admin)]
        public IActionResult Update(Barber barber)
        {
            if (barber.BarberId == 0) return BadRequest();
            barberService.Update(barber);
            return Ok(new { message = "Barber is successfully updated" });
        }

        [HttpGet("{id}")]
        public IActionResult GetBarber(int id)
        {
            Barber barber = barberService.Get(id);
            return Ok(new LinkCollectionWrapper<Barber>(barber, GenerateLinksForBarber(barber)));

        }

        [HttpGet("all")]
        public async Task<IActionResult> GetBarbers()
        {
            List<Barber> barbers = barberService.Barbers.ToList();
            LinkCollectionWrapper<List<LinkCollectionWrapper<Barber>>> result = new LinkCollectionWrapper<List<LinkCollectionWrapper<Barber>>>();
            result.Value = new List<LinkCollectionWrapper<Barber>>();
            foreach (Barber barber in barbers)
            {
                var client = httpClientFactory.CreateClient();
                string url = $"{configuration["JWT:Issuer"]}/Drive/downloadLink/{barber.ImageUrl}";
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var imagePath = await response.Content.ReadAsStringAsync();
                    barber.ImageUrl = imagePath;
                }
                else
                {
                    barber.ImageUrl = "";
                }
                result.Value.Add(new LinkCollectionWrapper<Barber>(barber, GenerateLinksForBarber(barber)));
            }
            return Ok(result);
        }

        ///////////////////////////////////////////////////
        private List<Link> GenerateLinksForBarber(Barber barber)
        {
            List<Link> links = new List<Link>()
                {
                    new Link(){
                        Method = "GET",
                        Rel = "get",
                        Href = linkGenerator.GetUriByAction(HttpContext,action: nameof(GetBarber),
                                controller: "BarberMobile",
                                values: new { id = barber.BarberId })
                    },
                    new Link(){
                        Method = "PUT",
                        Rel = "update",
                        Href = linkGenerator.GetUriByAction(HttpContext, nameof(Update))
                    },
                    new Link(){
                        Method = "GET",
                        Rel = "getAvailableAppointments",
                        Href = linkGenerator.GetUriByAction(HttpContext,action: "GetAvailableAppointments",
                                controller: "AppointmentMobile",
                                values: new { barberId = barber.BarberId,startDate= "startDate",endDate="endDate"}),
					},
					new Link(){
						Method = "GET",
						Rel = "getAvailableTimes",
						Href = linkGenerator.GetUriByAction(HttpContext,action: "GetAvailableTimes",
								controller: "AppointmentMobile",
								values: new { barberId = barber.BarberId,date= "date",duration="duration"}),
					},
                    new Link()
                    {
                        Method = "POST",
                        Rel = "createAppointment",
                        Href = linkGenerator.GetUriByAction(HttpContext, action: "CreateAppointmnet", controller: "AppointmentMobile"),
                    },
			};
            return links;
        }

}
}
