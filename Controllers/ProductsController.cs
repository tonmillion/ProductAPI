// ProductApi/Controllers/ProductsController.cs
using Microsoft.AspNetCore.Mvc;
using ProductApi.Models;   
using ProductApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductApi.Controllers
{
    [Route("api/[controller]")] 
    [ApiController]        
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService; 

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll() 
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products); 
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound(); 
            }

            return Ok(product); 
        }

       
        [HttpPost]
        public async Task<ActionResult<Product>> Create([FromBody] Product product) 
        {
            
            if (string.IsNullOrWhiteSpace(product.Name))
            {
                
                return BadRequest("Product name cannot be empty.");
            }

            
            if (product.Price < 0)
            {
                ModelState.AddModelError(nameof(product.Price), "Price cannot be negative.");
                return BadRequest(ModelState); 
            }

            var createdProduct = await _productService.CreateProductAsync(product);

            
            return CreatedAtAction(nameof(GetById), new { id = createdProduct.ProductId }, createdProduct);
        }

        
        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> Update(int id, [FromBody] Product product)
        {
            
            if (string.IsNullOrWhiteSpace(product.Name))
            {
                return BadRequest("Product name cannot be empty.");
            }
             if (product.Price < 0)
            {
                ModelState.AddModelError(nameof(product.Price), "Price cannot be negative.");
                return BadRequest(ModelState);
            }


            var updatedProduct = await _productService.UpdateProductAsync(id, product);

            if (updatedProduct == null)
            {
                return NotFound(); 
            }

            
            return Ok(updatedProduct); 
        }

      
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) 
        {
            var result = await _productService.DeleteProductAsync(id);

            if (!result)
            {
                return NotFound(); 
            }

            return NoContent(); 
        }
    }
}