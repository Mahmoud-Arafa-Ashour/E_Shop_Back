using E_Shop.Core.Abstractions;
using E_Shop.Models.Custumes.Contracts.Product;

namespace E_Shop.Service.IServices;

public interface IProductServices
{
    Task<Result<PaginatedData<ProductResponse>>> GetAllProductsAsync(RequestedFilters filters,CancellationToken cancellationToken);
    Task<Result<ProductResponse>> GetProductAsync(int Id ,CancellationToken cancellationToken);
    Task<Result> AddProduct(ProductRequest request);
    Task<Result> DeleteProduct(int id, CancellationToken cancellationToken);
    Task<Result<ProductResponse>> UpdateProductAsync(int id, ProductRequest request, CancellationToken cancellationToken);
}
