using FluentValidation.Results;

namespace MBD.CreditCards.Application.Requests
{
    public abstract class BaseRequest
    {
        public abstract ValidationResult Validate();
    }
}