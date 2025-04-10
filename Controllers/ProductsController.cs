// ProductApi/Controllers/ProductsController.cs
using Microsoft.AspNetCore.Mvc;
using ProductApi.Models;
using ProductApi.Services;
using ProductApi.Helpers; // Using PagedResult
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // Sẽ dùng ở phần 2

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

        // GET: api/products?pageNumber=1&pageSize=10
        [HttpGet]
        // Thay đổi kiểu trả về thành ActionResult<PagedResult<Product>>
        // Thêm [FromQuery] để nhận tham số từ query string
        public async Task<ActionResult<PagedResult<Product>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var pagedResult = await _productService.GetAllProductsAsync(pageNumber, pageSize);
            // Không cần check null vì PagedResult luôn được tạo
            return Ok(pagedResult);
        }

        // Các action khác giữ nguyên hoặc sẽ được cập nhật ở phần sau...
         // GET: api/products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        // POST: api/products - Sẽ thêm [Authorize] ở phần 2
        [HttpPost]
        [Authorize] 
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

         // PUT: api/products/5
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
             if (updatedProduct == null) return NotFound();
             return Ok(updatedProduct);
        }

        // DELETE: api/products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}