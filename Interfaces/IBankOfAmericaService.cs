using netApi.Repositories.BankOfAmerica.Models;

namespace NetEasyPay.Interfaces
{
    public interface IBankOfAmericaService
    {
        BOAPaymentResponse MakePaymentWithBOA(BOAPayment pymt);
    }
}
