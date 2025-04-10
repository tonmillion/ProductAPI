// ProductApi/Services/ProductService.cs
using Microsoft.EntityFrameworkCore;
using ProductApi.Data;
using ProductApi.Helpers; // Using PagedResult
using ProductApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductApi.Services
{
    public class ProductService : IProductService
    {
        private readonly ProductDbContext _context;

        public ProductService(ProductDbContext context)
        {
            _context = context;
        }

        // Cập nhật phương thức này
        public async Task<PagedResult<Product>> GetAllProductsAsync(int pageNumber = 1, int pageSize = 10)
        {
            // Đảm bảo pageNumber và pageSize hợp lệ
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 5, 50); // Giới hạn pageSize từ 5 đến 50

            // Tạo query cơ sở, vẫn sắp xếp theo CreatedAt giảm dần
            var query = _context.Products.OrderByDescending(p => p.CreatedAt);

            // Lấy tổng số lượng sản phẩm (chưa phân trang)
            var totalCount = await query.CountAsync();

            // Áp dụng phân trang bằng Skip và Take
            var items = await query
                .Skip((pageNumber - 1) * pageSize) // Bỏ qua các sản phẩm của trang trước
                .Take(pageSize)                  // Lấy số lượng sản phẩm cho trang hiện tại
                .ToListAsync();

            // Tạo và trả về đối tượng PagedResult
            return new PagedResult<Product>(items, totalCount, pageNumber, pageSize);
        }

        // Các phương thức khác giữ nguyên...
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }
        public async Task<Product> CreateProductAsync(Product product)
        {
             product.CreatedAt = DateTime.UtcNow;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }
        public async Task<Product?> UpdateProductAsync(int id, Product product)
        {
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null) return null;
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.DiscountPrice = product.DiscountPrice;
            existingProduct.Stock = product.Stock;
            existingProduct.Category = product.Category;
            existingProduct.Image = product.Image;
            existingProduct.IsActive = product.IsActive;
            await _context.SaveChangesAsync();
            return existingProduct;
        }
         public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}