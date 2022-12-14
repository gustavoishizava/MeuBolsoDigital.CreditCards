using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using MBD.CreditCards.API.Identity;
using MBD.CreditCards.API.Workers;
using MBD.CreditCards.Application.IntegrationEvents.Events;
using MBD.CreditCards.Application.IntegrationEvents.Handlers;
using MBD.CreditCards.Application.Interfaces;
using MBD.CreditCards.Application.Services;
using MBD.CreditCards.Domain.Interfaces.Repositories;
using MBD.CreditCards.Infrastructure;
using MBD.CreditCards.Infrastructure.Repositories;
using MediatR;
using MeuBolsoDigital.Core.Interfaces.Identity;
using MeuBolsoDigital.Core.Interfaces.Repositories;
using MeuBolsoDigital.RabbitMQ.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MBD.CreditCards.API.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjectionConfiguration
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAppServices()
                    .AddRepositories()
                    .AddConsumers()
                    .AddMessageBus(configuration)
                    .AddConfigurations(configuration)
                    .AddIntegrationEvents();

            services.AddHttpContextAccessor();
            services.AddScoped<ILoggedUser, WebAppUser>();
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddAutoMapper(Assembly.Load("MBD.CreditCards.Application"));

            return services;
        }

        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<ICreditCardAppService, CreditCardAppService>();

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<ICreditCardRepository, CreditCardRepository>();
            services.AddScoped<IBankAccountRepository, BankAccountRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        public static IServiceCollection AddConsumers(this IServiceCollection services)
        {
            services.AddHostedService<RabbitMqConsumerWorker>();

            return services;
        }

        public static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRabbitMqConnection(configuration);

            return services;
        }

        public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }

        public static IServiceCollection AddIntegrationEvents(this IServiceCollection services)
        {
            services.AddScoped<INotificationHandler<TransactionLinkedToCreditCardBillIntegrationEvent>, TransactionLinkedToCreditCardBillIntegrationEventHandler>();
            services.AddScoped<INotificationHandler<BankAccountCreatedIntegrationEvent>, BankAccountCreatedIntegrationEventHandler>();

            return services;
        }
    }
}