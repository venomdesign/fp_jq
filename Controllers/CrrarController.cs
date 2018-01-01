using Microsoft.Web.Http;
using netApi.Repositories.Authorization.Model;
using netApi.Repositories.CRRAR.Models;
using NetEasyPay.Interfaces;
using NetEasyPay.Services;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Helpers;
using System.Web.Http;
using Newtonsoft.Json.Serialization;

namespace NetEasyPay.Controllers
{
    [ApiVersion("1.0")]
    public class CrrarController : ApiController
    {
        private ICrrarService _service;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public CrrarController()
        {
            _service = new CrrarService();

            log.Info(string.Format("Started CRRARController at {0}", DateTime.UtcNow));
        }

        [HttpGet]
        [Route("api/v{version:apiVersion}/Crrar/GetUnpaidInvoices")]
        public object GetUnpaidInvoices(int contactId)
        {
            try
            {
                return _service.GetUnpaidInvoices(contactId);
            }
            catch (Exception e)
            {
                var message = $"Error retreiving unpaid invoices for Contact ID: {contactId}";
                log.Error(message, e);

                return BadRequest(message);
            }
        }

        [HttpGet]
        [Route("api/v{version:apiVersion}/Crrar/GetPaidInvoices")]
        public object GetPaidInvoices(int contactId)
        {
            try
            {
                return _service.GetPaidInvoices(contactId);
            }
            catch (Exception e)
            {
                var message = $"Error retreiving paid invoices for Contact ID: {contactId}";
                log.Error(message, e);

                return BadRequest(message);
            }
        }

        [HttpGet]
        [Route("api/v{version:apiVersion}/Crrar/GetPendingInvoices")]
        public object GetPendingInvoices(int contactId)
        {
            try
            {
                var result = (List<Invoice>)_service.GetPendingInvoices(contactId);

                return Ok(result);
            }
            catch (Exception e)
            {
                var message = $"Error retreiving pending invoices for Contact ID: {contactId}";
                log.Error(message, e);

                return BadRequest(message);
            }
        }

        [HttpGet]
        [Route("api/v{version:apiVersion}/Crrar/GetPendingInvoices")]
        public object GetPendingInvoices(int contactId, int attnId)
        {
            try
            {
                var result = (List<Invoice>)_service.GetPendingInvoices(contactId, attnId);

                return Ok(result);
            }
            catch (Exception e)
            {
                var message = $"Error retreiving pending invoices for Contact ID: {contactId} and Attention ID: {attnId}";
                log.Error(message, e);

                return BadRequest(message);
            }
        }

        [HttpGet]
        [Route("api/v{version:apiVersion}/Crrar/GetHistory")]
        public object GetInvoiceHistory(int contactId)
        {
            try
            {
                _service = new CrrarService();
                List <TRANSACTION_HISTORY> resp = _service.GetInvoiceHistory(contactId);
                return JsonConvert.SerializeObject(resp);
            }
            catch (Exception e)
            {
                var message = $"Error retreiving invoice history for Contact ID: {contactId}";
                log.Error(message, e);

                return null;
            }
        }

        [HttpGet]
        [Route("api/v{version:apiVersion}/Crrar/MarkInvoicePaid")]
        public object MarkInvoicePaid(DateTime invoiceDate, string invoiceRef, string confirmationNumber, string transactionNumber)
        {
            try
            {
                return _service.MarkInvoicePaid(invoiceDate, invoiceRef, confirmationNumber, transactionNumber);
            }
            catch (Exception e)
            {
                var message = $"Error calling Mark Invoice Paid for Invoice Date: {invoiceDate}, Invoice Ref: {invoiceRef}, Confirmation Number: {confirmationNumber}, and Transaction Number: {transactionNumber}";
                log.Error(message, e);

                return BadRequest(message);
            }
        }

        #region Searches

        //These are just stubs.  We will have to get better information on how the search feature in CRRAR will talk with us.  We may be able
        //to add all of this to an XML/JSON object and send it as one and they parse for what information it contains.

        [HttpGet]
        [Route("api/v{version:apiVersion}/Crrar/GetContactByCompanyName")]
        public object GetContactByCompanyName(string companyName)
        {
            try
            {
                return _service.GetContactByCompanyName(companyName);
            }
            catch (Exception e)
            {
                var message = $"Error retreiving Contact By Company for Company Name: {companyName}";
                log.Error(message, e);

                return BadRequest(message);
            }
        }

        [HttpGet]
        [Route("api/v{version:apiVersion}/Crrar/GetContactByAttnName")]
        public object GetContactByAttnName(string attnName)
        {
            try
            {
                return _service.GetContactByAttnName(attnName);
            }
            catch (Exception e)
            {
                var message = $"Error retreiving Contact by AttnName for Attention Name: {attnName}";
                log.Error(message, e);

                return BadRequest(message);
            }
        }

        [HttpGet]
        [Route("api/v{version:apiVersion}/Crrar/GetContactByCompanyNameAndAttnName")]
        public object GetContactByCompanyNameAndAttnName(string companyName, string attnName)
        {
            try
            {
                return _service.GetContactByCompanyNameAndAttnName(companyName, attnName);
            }
            catch (Exception e)
            {
                var message = $"Error retreiving Contact by Company Name and AttnName for Company Name: {companyName} and Attn Name: {attnName}";
                log.Error(message, e);

                return BadRequest(message);
            }
        }

        [HttpGet]
        [Route("api/v{version:apiVersion}/Crrar/GetContactByContactId")]
        public object GetContactByContactId(int contactId)
        {
            try
            {
                Contact ct = (Contact)_service.GetContactByContactId(contactId);

                return Ok(ct);
            }
            catch (Exception e)
            {
                var message = $"Error retreiving Contact by Contact ID for Contact ID: {contactId}";
                log.Error(message, e);

                return BadRequest(message);
            }
        }

        [HttpGet]
        [Route("api/v{version:apiVersion}/Crrar/GetContactByContactIdAndAttnName")]
        public object GetContactByContactIdAndAttnName(int contactId, string attnName)
        {
            try
            {
                return _service.GetContactByContactIdAndAttnName(contactId, attnName);
            }
            catch (Exception e)
            {
                var message = $"Error retreiving Contact By Contact ID and Attn Name for Contact ID: {contactId} and Attn Name : {attnName}";
                log.Error(message, e);

                return BadRequest(message);
            }
        }

        [HttpGet]
        [Route("api/v{version:apiVersion}/Crrar/GetContactByInvoiceReference")]
        public object GetContactByInvoiceReference(string invoiceRef)
        {
            try
            {
                return _service.GetContactByInvoiceReference(invoiceRef);
            }
            catch (Exception e)
            {
                var message = $"Error retreiving Contact By Invoice Reference for Invoice Ref: {invoiceRef}";
                log.Error(message, e);

                return BadRequest(message);
            }
        }

        #endregion

    }
}
