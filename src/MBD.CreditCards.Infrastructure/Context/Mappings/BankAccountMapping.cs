using System.Diagnostics.CodeAnalysis;
using DotNet.MongoDB.Context.Mapping;
using MBD.CreditCards.Domain.Entities;
using MongoDB.Bson.Serialization;

namespace MBD.CreditCards.Infrastructure.Context.Mappings
{
    [ExcludeFromCodeCoverage]
    public sealed class BankAccountMapping : BsonClassMapConfiguration
    {
        public BankAccountMapping() : base("bank_accounts")
        {
        }

        public override BsonClassMap GetConfiguration()
        {
            var map = new BsonClassMap<BankAccount>();

            map.MapIdProperty(x => x.Id);

            map.MapProperty(x => x.TenantId)
                .SetElementName("tenant_id");

            map.MapProperty(x => x.Description)
                .SetElementName("description");

            return map;
        }
    }
}