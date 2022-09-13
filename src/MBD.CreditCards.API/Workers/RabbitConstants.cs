using System.Diagnostics.CodeAnalysis;

namespace MBD.CreditCards.API.Workers
{
    [ExcludeFromCodeCoverage]
    public static class RabbitConstants
    {
        public const string BANK_ACCOUNT_CREATED = "TOPIC/bank_accounts.created";
    }
}