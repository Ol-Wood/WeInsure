using Microsoft.EntityFrameworkCore;
using WeInsure.Domain.Entities;
using WeInsure.Domain.ValueObjects;

namespace WeInsure.Data;

public class WeInsureDbContext(DbContextOptions<WeInsureDbContext> options) : DbContext(options)
{
    public DbSet<Policy> Policies { get; set; }
    public DbSet<PolicyHolder> PolicyHolders { get; set; }
    public DbSet<InsuredProperty> InsuredProperties { get; set; }
    public DbSet<Payment> Payments { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>(builder =>
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Reference).IsRequired().HasMaxLength(50);
            builder.Property(p => p.PolicyId).IsRequired();
            builder.Property(x => x.PaymentType)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(50);
            builder.OwnsOne(p => p.Amount, money =>
                money.Property(p => p.Amount)
                    .HasColumnName("Amount")
                    .HasPrecision(18, 2)
                    .IsRequired());
        });

        modelBuilder.Entity<InsuredProperty>(builder =>
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.PolicyId).IsRequired();
            builder.OwnsOne(p => p.Address, address =>
            {
                address.Property(a => a.AddressLine1)
                    .IsRequired()
                    .HasMaxLength(100);
                address.Property(a => a.AddressLine2)
                    .HasMaxLength(100);
                address.Property(a => a.AddressLine3)
                    .HasMaxLength(100);
                address.Property(a => a.PostCode)
                    .IsRequired()
                    .HasMaxLength(8);
            });
        });

        modelBuilder.Entity<PolicyHolder>(builder =>
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.PolicyId).IsRequired();
            builder.Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(p => p.LastName)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(p => p.DateOfBirth)
                .IsRequired();
        });

        modelBuilder.Entity<Policy>(builder =>
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Canceled).HasDefaultValue(false);
            builder.Property(p => p.AutoRenew).IsRequired();
            builder.Property(p => p.StartDate).IsRequired();
            builder.Property(p => p.EndDate).IsRequired();
            builder.Property(p => p.PolicyType).IsRequired();

            builder.Property(x => x.Reference)
                .HasConversion(
                    r => r.Value,
                    r => PolicyReference.FromExisting(r));
                
            builder.OwnsOne(p => p.Price, money =>
                money.Property(p => p.Amount)
                    .HasColumnName("Price")
                    .HasPrecision(18, 2)
                    .IsRequired());
        
            builder.HasOne(p => p.Payment)
                .WithOne()
                .HasForeignKey<Payment>(p => p.PolicyId);
            builder
                .HasOne(p => p.InsuredProperty)
                .WithOne()
                .HasForeignKey<InsuredProperty>(x => x.PolicyId);
            builder
                .HasMany( p => p.PolicyHolders)
                .WithOne()
                .HasForeignKey(x => x.PolicyId);
        });
    }
}