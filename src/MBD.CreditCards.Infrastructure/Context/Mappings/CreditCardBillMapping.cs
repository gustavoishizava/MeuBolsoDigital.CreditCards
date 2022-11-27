using System.Diagnostics.CodeAnalysis;
using DotNet.MongoDB.Context.Mapping;
using MBD.CreditCards.Domain.Entities;
using MongoDB.Bson.Serialization;

namespace MBD.CreditCards.Infrastructure.Context.Mappings
{
    [ExcludeFromCodeCoverage]
    public sealed class CreditCardBillMapping : BsonClassMapConfiguration
    {
        public CreditCardBillMapping() : base("credit_card_billds")
        {
        }

        public override BsonClassMap GetConfiguration()
        {
            var map = new BsonClassMap<CreditCardBill>();

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

            return map;
        }
    }
}