using netApi.Repositories.BankOfAmerica.Interfaces;
using netApi.Repositories.BankOfAmerica.Models;
using netApi.Repositories.BankOfAmerica.Repositories;
using NetEasyPay.Interfaces;

namespace NetEasyPay.Services
{
    public class MOCKBankOfAmericaService : IBankOfAmericaService
    {
        private readonly IBankOfAmericaRepository _repository;

        public MOCKBankOfAmericaService()
        {
            _repository = new MOCKBankOfAmericaRepository();
        }

        public BOAPaymentResponse MakePaymentWithBOA(BOAPayment pymt)
        {
            return _repository.MakePaymentWithBOA(pymt);
        }
    }
}