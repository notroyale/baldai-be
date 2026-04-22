using baldai_be.Application.Data;
using baldai_be.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace baldai_be.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<ChatThread> ChatThreads { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Offer> Offers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ComplexProperty(e => e.Dimensions);
            entity.Property(e => e.RowVersion).IsRowVersion();
            entity.HasOne(e => e.Seller)
                  .WithMany(u => u.Products)
                  .HasForeignKey(e => e.SellerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Product)
                  .WithMany()
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Buyer)
                  .WithMany()
                  .HasForeignKey(e => e.BuyerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ChatThread>()
            .HasOne(e => e.Product)
            .WithMany()
            .HasForeignKey(e => e.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<ChatThread>()
            .HasOne(e => e.Buyer)
            .WithMany()
            .HasForeignKey(e => e.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<ChatThread>()
            .HasOne(e => e.Seller)
            .WithMany()
            .HasForeignKey(e => e.SellerId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<Message>()
            .HasOne(e => e.Sender)
            .WithMany()
            .HasForeignKey(e => e.SenderId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<Offer>()
            .HasOne(e => e.Buyer)
            .WithMany()
            .HasForeignKey(e => e.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
