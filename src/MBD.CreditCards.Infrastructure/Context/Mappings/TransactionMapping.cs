using System.Diagnostics.CodeAnalysis;
using DotNet.MongoDB.Context.Mapping;
using MBD.CreditCards.Domain.Entities;
using MongoDB.Bson.Serialization;

namespace MBD.CreditCards.Infrastructure.Context.Mappings
{
    [ExcludeFromCodeCoverage]
    public sealed class TransactionMapping : BsonClassMapConfiguration
    {
        public override BsonClassMap GetConfiguration()
        {
            var map = new BsonClassMap<Transaction>();

            map.MapProperty(x => x.Id)
                    .SetElementName("id");

            map.MapProperty(x => x.CreditCardBillId)
                .SetElementName("credit_card_bill_id");

            map.MapProperty(x => x.CreatedAt)
                .SetElementName("created_at");

            map.MapProperty(x => x.Value)
                .SetElementName("value");

            return map;
        }
    }
}