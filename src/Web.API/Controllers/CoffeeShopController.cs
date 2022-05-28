using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Web.API.Service;

namespace Web.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CoffeeShopController : Controller
    {
        private readonly ICoffeeShopService _coffeeShopService;
        private readonly ILogger<CoffeeShopController> _logger;

        public CoffeeShopController(ICoffeeShopService coffeeShopService, ILogger<CoffeeShopController> logger)
        {
            _coffeeShopService = coffeeShopService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var coffeeShop = await _coffeeShopService.List();

            return Ok(coffeeShop);
        }
    }
}
