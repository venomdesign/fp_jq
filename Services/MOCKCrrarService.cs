using netApi.Repositories.CRRAR.Interfaces;
using netApi.Repositories.CRRAR.Repositories;
using NetEasyPay.Interfaces;
using System;

namespace NetEasyPay.Services
{
    public class MOCKCrrarService : ICrrarService
    {
        private readonly ICrrarRepository _repository;

        public MOCKCrrarService()
        {
            _repository = new MOCKCrrarRepository();
        }

        public object GetContactByAttnName(string attnName)
        {
            return _repository.GetContactByAttnName(attnName);
        }

        public object GetContactByCompanyName(string contactName)
        {
            return _repository.GetContactByCompanyName(contactName);
        }

        public object GetContactByCompanyNameAndAttnName(string companyName, string attnName)
        {
            return _repository.GetContactByCompanyNameAndAttnName(companyName, attnName);
        }

        public object GetContactByContactId(int contactId)
        {
            return _repository.GetContactByContactId(contactId);
        }

        public object GetContactByContactIdAndAttnName(int contactId, string attnName)
        {
            return _repository.GetContactByContactIdAndAttnName(contactId, attnName);
        }

        public object GetContactByInvoiceReference(string InvoiceRef)
        {
            return _repository.GetContactByInvoiceReference(InvoiceRef);
        }

        public object GetInvoiceHistory(int contactId)
        {
            return _repository.GetInvoiceHistory(contactId);
        }

        public object GetPaidInvoices(int contactId)
        {
            return _repository.GetPaidInvoices(contactId);
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
            return _repository.MarkInvoicePaid(invoiceDate, InvoiceRef, confirmationNumber, transactionNumber);
        }
    }
}