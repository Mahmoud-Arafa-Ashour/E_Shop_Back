using E_Shop.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Security.Claims;

namespace E_Shop.Core.Persistent;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options , IHttpContextAccessor httpContextAccessor) : 
    IdentityDbContext<ApplicationUser , ApplicationRole , string>(options)
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    public DbSet<Product> Products { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entryEntites = ChangeTracker.Entries<AuditableEntites>();
        foreach (var item in entryEntites) 
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (item.State == EntityState.Added)
            {
                item.Property(x => x.CreatedById).CurrentValue = currentUserId!;
                item.Property(x => x.CreatedOn).CurrentValue = DateTime.UtcNow;
            }
            else if (item.State == EntityState.Modified)
            {
                item.Property(x => x.UpdatedById).CurrentValue = currentUserId!;
                item.Property(x => x.UpdatedOn).CurrentValue = DateTime.UtcNow;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
