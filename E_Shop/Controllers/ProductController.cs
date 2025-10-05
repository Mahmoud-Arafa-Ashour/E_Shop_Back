using E_Shop.Core.Abstractions;
using E_Shop.Core.Authorization;
using E_Shop.Models.Custumes.Contracts.Product;
using E_Shop.Service.IServices;
using Microsoft.AspNetCore.Mvc;

namespace E_Shop.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class ProductController(IProductServices ProductServices) : ControllerBase
{
    private readonly IProductServices _ProductServices = ProductServices;

    [HttpGet]
    [HasPermission(Permissions.GetAllProducts)]
    public async Task<IActionResult> GetAll([FromQuery] RequestedFilters filters , CancellationToken cancellationToken)
    {
        var result = await _ProductServices.GetAllProductsAsync(filters , cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HasPermission(Permissions.GetProduct)]
    [HttpGet]
    public async Task<IActionResult> Get(int Id , CancellationToken cancellationToken)
    {
        var result = await _ProductServices.GetProductAsync(Id , cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HasPermission(Permissions.AddProduct)]
    [HttpPost]
    public async Task<IActionResult> Add([FromForm] ProductRequest request)
    {
        var response = await _ProductServices.AddProduct(request);
        return response.IsSuccess ? Created() : response.ToProblem();
    }
    [HasPermission(Permissions.DeleteProduct)]
    [HttpDelete]
    public async Task<IActionResult> Delete(int Id, CancellationToken cancellationToken)
    {
        var response = await _ProductServices.DeleteProduct( Id, cancellationToken);
        return response.IsSuccess ? NoContent() : response.ToProblem();
    }
    [HasPermission(Permissions.UpdateProduct)]
    [HttpPut]
    public async Task<IActionResult> Update(int Id, [FromForm] ProductRequest request, CancellationToken cancellationToken)
    {
        var response = await _ProductServices.UpdateProductAsync(Id, request, cancellationToken);
        return response.IsSuccess ? NoContent() : response.ToProblem();
    }
}
