using System.Diagnostics.CodeAnalysis;
using DotNet.MongoDB.Context.Mapping;
using MBD.CreditCards.Domain.Entities;
using MongoDB.Bson.Serialization;

namespace MBD.CreditCards.Infrastructure.Context.Mappings
{
    [ExcludeFromCodeCoverage]
    public sealed class CreditCardMapping : BsonClassMapConfiguration
    {
        public CreditCardMapping() : base("credit_cards")
        {

        }

        public override BsonClassMap GetConfiguration()
        {
            var map = new BsonClassMap<CreditCard>();

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

            return map;
        }
    }
}