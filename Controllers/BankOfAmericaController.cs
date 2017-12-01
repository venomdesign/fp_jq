using Microsoft.Web.Http;
using netApi.Common;
using netApi.Repositories.BankOfAmerica.Models;
using NetEasyPay.Interfaces;
using NetEasyPay.Services;
using Newtonsoft.Json;
using System;
using System.Web.Http;

namespace NetEasyPay.Controllers
{
    [ApiVersion("1.0")]
    public class BankOfAmericaController : ApiController
    {
        private readonly IBankOfAmericaService _service;
        //log4Net being configured following the instructions on https://stackify.com/log4net-guide-dotnet-logging/

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public BankOfAmericaController()
        {
#if DEBUG
            _service = new MOCKBankOfAmericaService();
#else
            _service = new BankOfAmericaService();
#endif
            log.Info(string.Format("Started BankOfAmericaController at {0}", DateTime.UtcNow));
        }

        [Route("api/v{version:apiVersion}/BankOfAmerica/BOAMakePayment")]
        public object MakePaymentWithBOA(string pymt)
        {
            if (pymt == null) { return BadRequest("Payment cannot be null.  Please verify your data."); }

            try
            {
                var payment = JsonConvert.DeserializeObject<BOAPayment>(pymt);

                //Validate JOT token here from the header of the request
                //TODO:: JOT Token validation
                var reg = _service.MakePaymentWithBOA(payment);

                return Ok(reg);
            }
            catch (Exception e)
            {
                var msg = string.Format("Error submitting payment to BOA: {0}.\n\n{1}", pymt, e.Message);
                log.Error(e);

                return BadRequest(msg);
            }
        }
    }
}