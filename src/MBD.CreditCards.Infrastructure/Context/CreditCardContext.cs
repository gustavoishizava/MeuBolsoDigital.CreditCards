using System.Diagnostics.CodeAnalysis;
using DotNet.MongoDB.Context.Configuration;
using DotNet.MongoDB.Context.Context;
using MBD.CreditCards.Domain.Entities;
using MongoDB.Driver;

namespace MBD.CreditCards.Infrastructure.Context
{
    [ExcludeFromCodeCoverage]
    public class CreditCardContext : DbContext
    {
        public CreditCardContext(IMongoClient mongoClient, IMongoDatabase mongoDatabase, MongoDbContextOptions options) : base(mongoClient, mongoDatabase, options)
        {
        }

        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<CreditCardBill> CreditCardBills { get; set; }
    }
}