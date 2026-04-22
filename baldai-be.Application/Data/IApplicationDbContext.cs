using baldai_be.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace baldai_be.Application.Data;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Product> Products { get; }
    DbSet<Transaction> Transactions { get; }
    DbSet<ChatThread> ChatThreads { get; }
    DbSet<Message> Messages { get; }
    DbSet<Offer> Offers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
