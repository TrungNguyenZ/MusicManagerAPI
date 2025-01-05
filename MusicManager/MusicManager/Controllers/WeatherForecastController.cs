using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicManager.Models;
using MusicManager.Services;

namespace MusicManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IProductService _productService;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IProductService productService)
        {
            _productService = productService;

        }
        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            var isAdminClaim = User.FindFirst("isAdmin")?.Value;
            var products = _productService.GetAllProducts();
            return Ok(isAdminClaim);
        }


        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Product product)
        {
            _productService.CreateProduct(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }
            _productService.UpdateProduct(product);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _productService.DeleteProduct(id);
            return NoContent();
        }
    }
}