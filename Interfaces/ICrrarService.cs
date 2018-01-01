using netApi.Repositories.Authorization.Model;
using netApi.Repositories.CRRAR.Models;
using System;
using System.Collections.Generic;

namespace NetEasyPay.Interfaces
{
    public interface ICrrarService
    {
        List<Invoice> GetUnpaidInvoices(int contactId);
        object GetPaidInvoices(int contactId);
        object GetPendingInvoices(int contactId);
        object GetPendingInvoices(int contactId, int attnId);
        List<TRANSACTION_HISTORY> GetInvoiceHistory(int contactId);
        object MarkInvoicePaid(DateTime invoiceDate, string InvoiceRef, string confirmationNumber, string transactionNumber);
        object GetContactByCompanyName(string contactName);
        object GetContactByAttnName(string attnName);
        object GetContactByCompanyNameAndAttnName(string companyName, string attnName);
        object GetContactByContactId(int contactId);
        object GetContactByContactIdAndAttnName(int contactId, string attnName);
        object GetContactByInvoiceReference(string InvoiceRef);
    }
}
