﻿using Microsoft.Web.Http;
using netApi.Repositories.CRRAR.Models;
using NetEasyPay.Interfaces;
using NetEasyPay.Services;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NetEasyPay.Controllers
{
    [ApiVersion("1.0")]
    public class CrrarController : ApiController
    {
        private ICrrarService _service;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public CrrarController()
        {
//#if DEBUG
//            _service = new MOCKCrrarService();
//#else
            _service = new CrrarService();
//#endif 
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
                throw e;
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
                throw e;
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
                throw e;
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
                throw e;
            }
        }

        [HttpGet]
        [Route("api/v{version:apiVersion}/Crrar/GetHistory")]
        public object GetInvoiceHistory(int contactId)
        {
            try
            {
                //TODO:: TAKE THIS BACK OUT ONCE WE HAVE A HISTORY TABLE SETUP
                _service = new MOCKCrrarService();
                return _service.GetInvoiceHistory(contactId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpGet]
        [Route("api/v{version:apiVersion}/Crrar/MarkInvoicePaid")]
        public object MarkInvoicePaid(DateTime invoiceDate, string InvoiceRef, string confirmationNumber, string transactionNumber)
        {
            try
            {
                return _service.MarkInvoicePaid(invoiceDate, InvoiceRef, confirmationNumber, transactionNumber);
            }
            catch (Exception e)
            {
                throw e;
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
                throw e;
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
                throw e;
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
                throw e;
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
                log.Error(e);

                throw e;
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
                throw e;
            }
        }

        [HttpGet]
        [Route("api/v{version:apiVersion}/Crrar/GetContactByInvoiceReference")]
        public object GetContactByInvoiceReference(string InvoiceRef)
        {
            try
            {
                return _service.GetContactByInvoiceReference(InvoiceRef);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

    }
}
