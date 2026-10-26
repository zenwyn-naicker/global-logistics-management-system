/*Author: Zenwyn Naicker*/
/*Code Attribution: */
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2023
 * Title: Getting Started - EF Core
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli> [Accessed 6 April 2026]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2023
 * Title: One-to-many relationships
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/ef/core/modeling/relationships/one-to-many> [Accessed 6 April 2026]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2023
 * Title: Value Conversions
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/ef/core/modeling/value-conversions> [Accessed 6 April 2026]
 **************************************************************************************************************/

using GLMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GLMS.Api.Data;

public class GlmsDbContext : DbContext
{
    public GlmsDbContext(DbContextOptions<GlmsDbContext> options) : base(options)
    {
    }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(client => client.Id);
            entity.Property(client => client.Name).IsRequired().HasMaxLength(150);
            entity.Property(client => client.ContactDetails).IsRequired().HasMaxLength(250);
            entity.Property(client => client.Region).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(contract => contract.Id);
            entity.Property(contract => contract.ServiceLevel).IsRequired().HasMaxLength(100);
            entity.Property(contract => contract.SignedAgreementPath).HasMaxLength(500);

            entity.Property(contract => contract.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.HasOne(contract => contract.Client)
                .WithMany(client => client.Contracts)
                .HasForeignKey(contract => contract.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ServiceRequest>(entity =>
        {
            entity.HasKey(request => request.Id);
            entity.Property(request => request.Description).IsRequired().HasMaxLength(1000);
            entity.Property(request => request.Cost).HasPrecision(18, 2);

            entity.Property(request => request.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.HasOne(request => request.Contract)
                .WithMany(contract => contract.ServiceRequests)
                .HasForeignKey(request => request.ContractId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
