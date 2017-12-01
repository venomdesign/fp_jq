using netApi.Repositories.CRRAR.Interfaces;
using netApi.Repositories.CRRAR.Repositories;
using NetEasyPay.Interfaces;
using System;

namespace NetEasyPay.Services
{
    public class CrrarService : ICrrarService
    {
        private readonly ICrrarRepository _repository;

        public CrrarService()
        {
            _repository = new CrrarRepository();
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

        public object GetInvoiceHistory(int contactId)
        {
            throw new System.NotImplementedException();
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

        public object GetUnpaidInvoices(int contactId)
        {
            return _repository.GetUnpaidInvoices(contactId);
        }

        public object MarkInvoicePaid(DateTime invoiceDate, string InvoiceRef, string confirmationNumber, string transactionNumber)
        {
            throw new System.NotImplementedException();
        }
    }
}