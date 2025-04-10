// ProductApi/Services/IProductService.cs
using ProductApi.Helpers;
using ProductApi.Models; 
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductApi.Services
{
    public interface IProductService
    {
        Task<PagedResult<Product>> GetAllProductsAsync(int pageNumber = 1, int pageSize = 10);
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product);
        Task<Product?> UpdateProductAsync(int id, Product product);
        Task<bool> DeleteProductAsync(int id); 
    }
}