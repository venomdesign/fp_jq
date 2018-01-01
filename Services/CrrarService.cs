using netApi.Repositories.Authorization.Model;
using netApi.Repositories.CRRAR.Interfaces;
using netApi.Repositories.CRRAR.Models;
using netApi.Repositories.CRRAR.Repositories;
using NetEasyPay.Interfaces;
using System;
using System.Collections.Generic;

namespace NetEasyPay.Services
{
    public class CrrarService : ICrrarService
    {
        private readonly ICrrarRepository _repository;
        private readonly FopsAuthorization _context = new FopsAuthorization();

        public CrrarService()
        {
            _repository = new CrrarRepository(_context);
        }

        public object GetContactByAttnName(string attnName)
        {
            throw new System.NotImplementedException();
        }

        public object GetContactByCompanyName(string companyName)
        {
            throw new System.NotImplementedException();
        }

        public object GetContactByCompanyNameAndAttnName(string companyName, string attnName)
        {
            throw new System.NotImplementedException();
        }

        public object GetContactByContactId(int contactId)
        {
            return _repository.GetContactByContactId(contactId);
        }

        public object GetContactByContactIdAndAttnName(int contactId, string attnName)
        {
            throw new System.NotImplementedException();
        }

        public object GetContactByInvoiceReference(string InvoiceRef)
        {
            throw new System.NotImplementedException();
        }

        public List<TRANSACTION_HISTORY> GetInvoiceHistory(int contactId)
        {
            return _repository.GetInvoiceHistory(contactId);
        }

        public object GetPaidInvoices(int contactId)
        {
            throw new System.NotImplementedException();
        }

        public object GetPendingInvoices(int contactId)
        {
            return _repository.GetPendingInvoices(contactId);
        }

        public object GetPendingInvoices(int contactId, int attnId)
        {
            return _repository.GetPendingInvoices(contactId, attnId);
        }

        public List<Invoice> GetUnpaidInvoices(int contactId)
        {
            return _repository.GetUnpaidInvoices(contactId);
        }

        public object MarkInvoicePaid(DateTime invoiceDate, string InvoiceRef, string confirmationNumber, string transactionNumber)
        {
            throw new System.NotImplementedException();
        }
    }
}