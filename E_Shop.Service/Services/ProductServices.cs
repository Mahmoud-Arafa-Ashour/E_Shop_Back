using E_Shop.Core.Abstractions;
using E_Shop.Core.Persistent;
using E_Shop.Service.IServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using E_Shop.Models.Entities;
using static E_Shop.Core.Abstractions.Errors;
using E_Shop.Models.Custumes.Contracts.Product;
using Mapster;

namespace E_Shop.Service.Services
{
    public class ProductServices(ApplicationDbContext dbContext , IWebHostEnvironment environment) : IProductServices
    {
        #region Members
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly IWebHostEnvironment _environment = environment;
        #endregion
        #region Methods
        public async Task<Result<PaginatedData<ProductResponse>>> GetAllProductsAsync(RequestedFilters filters, CancellationToken cancellationToken)
        {
            var result = _dbContext.Products.ProjectToType<ProductResponse>();
            var paginatedData = await PaginatedData<ProductResponse>.CreateAsync(result, filters.PageNumber, filters.PageSize , cancellationToken);
            return Result.Success(paginatedData);
        }
        public async Task<Result<ProductResponse>> GetProductAsync(int Id , CancellationToken cancellationToken)
        {
            var response = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == Id , cancellationToken);
            if (response is null)
                return Result.Failure<ProductResponse>(ProductErrors.EmptyProduct);
            var Product = response.Adapt<ProductResponse>();
            return Result.Success(Product);
        }
        public async Task<Result> AddProduct(ProductRequest request)
        {
            var isExistedProduct = await _dbContext.Products.AnyAsync(c => c.Name == request.Name);
            if (isExistedProduct)
            {
                return Result.Failure<ProductResponse>(new Error("Product.InvalidData", "This Product is already Existed", StatusCodes.Status409Conflict));
            }
            try
            {
                string? imageUrl = null;

                if (request.Image is not null)
                {
                    string uploadsFolder = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Image.FileName)}";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = File.Create(filePath))
                    {
                        await request.Image.CopyToAsync(fileStream);
                    }

                    imageUrl = $"/{uniqueFileName}";
                }

                var Product = new Product()
                {
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price,
                    ImagePath = imageUrl
                };

                await _dbContext.Products.AddAsync(Product);
                await _dbContext.SaveChangesAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error("Invalid Operation", ex.Message, StatusCodes.Status500InternalServerError));
            }

        }
        public async Task<Result> DeleteProduct(int id, CancellationToken cancellationToken)
        {
            var Product = await _dbContext.Products
                .FirstOrDefaultAsync(x => x.Id == id , cancellationToken);

            if (Product is null)
                return Result.Failure(ProductErrors.EmptyProduct);

            try
            {
                if (!string.IsNullOrWhiteSpace(Product.ImagePath))
                {
                    string uploadsFolder = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    string imagePath = Path.Combine(uploadsFolder, Path.GetFileName(Product.ImagePath));

                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                }

                _dbContext.Products.Remove(Product);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error("Delete.Invalid", ex.Message, StatusCodes.Status500InternalServerError));
            }
        }
        public async Task<Result<ProductResponse>> UpdateProductAsync(int id, ProductRequest request, CancellationToken cancellationToken)
        {
            try
            {

                var Product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
                if (Product is null)
                {
                    return Result.Failure<ProductResponse>(ProductErrors.EmptyProduct);
                }

                if (string.IsNullOrWhiteSpace(request.Name) ||
                    string.IsNullOrWhiteSpace(request.Description) ||
                    request.Price <= 0)
                {
                    return Result.Failure<ProductResponse>(
                        new Error("InvalidRequest", "Invalid Product details provided.", StatusCodes.Status400BadRequest));
                }

                string imageUrl = Product.ImagePath ?? string.Empty;
                string uploadsFolder = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (request.Image is not null)
                {
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Image.FileName)}";
                    string newFilePath = Path.Combine(uploadsFolder, uniqueFileName);

                    if (!string.IsNullOrWhiteSpace(Product.ImagePath))
                    {
                        string oldFilePath = Path.Combine(uploadsFolder, Path.GetFileName(Product.ImagePath));
                        if (File.Exists(oldFilePath))
                        {
                            File.Delete(oldFilePath);
                        }
                    }

                    await using var fileStream = File.Create(newFilePath);
                    await request.Image.CopyToAsync(fileStream, cancellationToken);

                    imageUrl = $"/{uniqueFileName}";
                }

                Product.Name = request.Name;
                Product.Description = request.Description;
                Product.Price = request.Price;
                Product.ImagePath = imageUrl;
                _dbContext.Products.Update(Product);
                await _dbContext.SaveChangesAsync(cancellationToken);

                
                var response = new ProductResponse(Product.Id,Product.Name, Product.Description, Product.Price, imageUrl);
                return Result.Success(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpdateProductAsync Error] {ex.Message}");
                return Result.Failure<ProductResponse>(new Error("Update.Invalid", ex.Message, StatusCodes.Status500InternalServerError));
            }
        }
        #endregion
    }
}
