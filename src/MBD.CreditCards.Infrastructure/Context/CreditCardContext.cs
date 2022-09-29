using System.Diagnostics.CodeAnalysis;
using DotNet.MongoDB.Context.Configuration;
using DotNet.MongoDB.Context.Context;
using DotNet.MongoDB.Context.Context.ModelConfiguration;
using MBD.CreditCards.Domain.Entities;
using MBD.CreditCards.Domain.Entities.Common;

namespace MBD.CreditCards.Infrastructure.Context
{
    [ExcludeFromCodeCoverage]
    public class CreditCardContext : DbContext
    {
        public CreditCardContext(MongoDbContextOptions options) : base(options)
        {
        }

        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<CreditCardBill> CreditCardBills { get; set; }

        protected override void OnModelConfiguring(ModelBuilder modelBuilder)
        {
            modelBuilder.AddModelMap<BaseEntity>(map =>
            {
                map.MapIdProperty(x => x.Id);

                map.MapProperty(x => x.CreatedAt)
                    .SetElementName("created_at");

                map.MapProperty(x => x.UpdatedAt)
                    .SetElementName("updated_at");
            });

            modelBuilder.AddModelMap<BankAccount>("bank_accounts", map =>
            {
                map.MapIdProperty(x => x.Id);

                map.MapProperty(x => x.TenantId)
                    .SetElementName("tenant_id");

                map.MapProperty(x => x.Description)
                    .SetElementName("description");
            });

            modelBuilder.AddModelMap<CreditCard>("credit_cards", map =>
            {
                map.MapProperty(x => x.TenantId)
                    .SetElementName("tenant_id");

                map.MapProperty(x => x.BankAccount)
                    .SetElementName("bank_account");

                map.MapProperty(x => x.Name)
                    .SetElementName("name");

                map.MapProperty(x => x.ClosingDay)
                    .SetElementName("closing_day");

                map.MapProperty(x => x.DayOfPayment)
                    .SetElementName("day_of_payment");

                map.MapProperty(x => x.Limit)
                    .SetElementName("limit");

                map.MapProperty(x => x.Brand)
                    .SetElementName("brand");

                map.MapProperty(x => x.Status)
                    .SetElementName("status");

                // TODO: mapear bills
            });

            modelBuilder.AddModelMap<CreditCardBill>("credit_card_bills", map =>
            {
                map.MapProperty(x => x.CreditCardId)
                    .SetElementName("credit_card_id");

                map.MapProperty(x => x.ClosesIn)
                    .SetElementName("closes_in");

                map.MapProperty(x => x.DueDate)
                    .SetElementName("due_date");

                map.MapProperty(x => x.Reference)
                    .SetElementName("reference");

                map.MapField("_transactions")
                    .SetElementName("transactions");
            });

            modelBuilder.AddModelMap<Transaction>(map =>
            {
                map.MapProperty(x => x.Id)
                    .SetElementName("id");

                map.MapProperty(x => x.CreditCardBillId)
                    .SetElementName("credit_card_bill_id");

                map.MapProperty(x => x.CreatedAt)
                    .SetElementName("created_at");

                map.MapProperty(x => x.Value)
                    .SetElementName("value");
            });
        }
    }
}