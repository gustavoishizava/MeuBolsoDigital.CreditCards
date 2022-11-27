using System.Diagnostics.CodeAnalysis;
using DotNet.MongoDB.Context.Extensions;
using MBD.CreditCards.Infrastructure.Context;
using MBD.CreditCards.Infrastructure.Context.CustomSerializers;
using MBD.CreditCards.Infrastructure.Context.Mappings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;

namespace MBD.CreditCards.API.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class DatabaseConfiguration
    {
        public static IServiceCollection AddDatabaseContextConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMongoDbContext<CreditCardContext>(options =>
            {
                options.ConfigureConnection(configuration.GetConnectionString("Default"), configuration["DatabaseName"]);
                options.AddSerializer(new GuidSerializer(BsonType.String));
                options.AddSerializer(new StatusSerializer());
                options.AddSerializer(new BrandSerializer());

                options.AddBsonClassMap(new BaseEntityMapping());
                options.AddBsonClassMap(new BankAccountMapping());
                options.AddBsonClassMap(new CreditCardMapping());
                options.AddBsonClassMap(new CreditCardBillMapping());
                options.AddBsonClassMap(new TransactionMapping());
            });

            return services;
        }
    }
}