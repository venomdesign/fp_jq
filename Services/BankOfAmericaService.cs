using netApi.Repositories.BankOfAmerica.Interfaces;
using netApi.Repositories.BankOfAmerica.Models;
using NetEasyPay.Interfaces;
using System;

namespace NetEasyPay.Services
{
    public class BankOfAmericaService : IBankOfAmericaService
    {
        private readonly IBankOfAmericaRepository _repository;

        public BOAPaymentResponse MakePaymentWithBOA(BOAPayment pymt)
        {
            throw new NotImplementedException();
        }
    }
}