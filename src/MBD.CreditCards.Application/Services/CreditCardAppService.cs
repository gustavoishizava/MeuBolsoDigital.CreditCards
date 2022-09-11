using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MBD.CreditCards.Application.Interfaces;
using MBD.CreditCards.Application.Requests;
using MBD.CreditCards.Application.Responses;
using MBD.CreditCards.Domain.Entities;
using MBD.CreditCards.Domain.Entities.Common;
using MBD.CreditCards.Domain.Interfaces.Repositories;
using MeuBolsoDigital.Application.Utils.Responses;
using MeuBolsoDigital.Application.Utils.Responses.Interfaces;
using MeuBolsoDigital.Core.Interfaces.Identity;
using MeuBolsoDigital.Core.Interfaces.Repositories;

namespace MBD.CreditCards.Application.Services
{
    public class CreditCardAppService : ICreditCardAppService
    {
        private readonly ILoggedUser _loggedUser;
        private readonly IMapper _mapper;
        private readonly ICreditCardRepository _repository;
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreditCardAppService(ILoggedUser loggedUser, IMapper mapper, ICreditCardRepository repository, IBankAccountRepository bankAccountRepository, IUnitOfWork unitOfWork)
        {
            _loggedUser = loggedUser;
            _mapper = mapper;
            _repository = repository;
            _bankAccountRepository = bankAccountRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult<CreditCardResponse>> CreateAsync(CreateCreditCardRequest request)
        {
            var validation = request.Validate();
            if (!validation.IsValid)
                return Result<CreditCardResponse>.Fail(validation.ToString());

            var bankAccount = await _bankAccountRepository.GetByIdAsync(request.BankAccountId);
            if (bankAccount == null)
                return Result<CreditCardResponse>.Fail("Conta bancária inválida.");

            var creditCard = new CreditCard(_loggedUser.UserId,
                                            bankAccount,
                                            request.Name,
                                            request.ClosingDay,
                                            request.DayOfPayment,
                                            request.Limit,
                                            request.Brand);

            await _repository.AddAsync(creditCard);
            await _unitOfWork.CommitAsync();

            return Result<CreditCardResponse>.Success(_mapper.Map<CreditCardResponse>(creditCard));
        }

        public async Task<IEnumerable<CreditCardResponse>> GetAllAsync()
        {
            return _mapper.Map<IEnumerable<CreditCardResponse>>(await _repository.GetAllAsync());
        }

        public async Task<IResult<CreditCardResponse>> GetByIdAsync(Guid id)
        {
            var creditCard = await _repository.GetByIdAsync(id);
            if (creditCard == null)
                return Result<CreditCardResponse>.Fail();

            return Result<CreditCardResponse>.Success(_mapper.Map<CreditCardResponse>(creditCard));
        }

        public async Task<IResult> RemoveAsync(Guid id)
        {
            var creditCard = await _repository.GetByIdAsync(id);
            if (creditCard == null)
                return Result.Fail("Cartão de crédito inválido.");

            await _repository.RemoveAsync(creditCard);
            await _unitOfWork.CommitAsync();

            return Result.Success();
        }

        public async Task<IResult> UpdateAsync(UpdateCreditCardRequest request)
        {
            var validation = request.Validate();
            if (!validation.IsValid)
                return Result<CreditCardResponse>.Fail(validation.ToString());

            var creditCard = await _repository.GetByIdAsync(request.Id);
            if (creditCard == null)
                return Result.Fail("Cartão de crédito inválido.");

            var bankAccount = await _bankAccountRepository.GetByIdAsync(request.BankAccountId);
            if (bankAccount == null)
                return Result<CreditCardResponse>.Fail("Conta bancária inválida.");

            creditCard.SetName(request.Name);
            creditCard.SetBankAccount(bankAccount);
            creditCard.SetBrand(request.Brand);
            creditCard.SetClosingDay(request.ClosingDay);
            creditCard.SetDayOfPayment(request.DayOfPayment);
            creditCard.SetLimit(request.Limit);
            if (request.Status == Status.Active)
                creditCard.Activate();
            else
                creditCard.Deactivate();

            await _repository.UpdateAsync(creditCard);
            await _unitOfWork.CommitAsync();

            return Result.Success();
        }
    }
}