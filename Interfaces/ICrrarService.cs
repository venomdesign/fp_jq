using System;

namespace NetEasyPay.Interfaces
{
    public interface ICrrarService
    {
        object GetUnpaidInvoices(int contactId);
        object GetPaidInvoices(int contactId);
        object GetPendingInvoices(int contactId);
        object GetPendingInvoices(int contactId, int attnId);
        object GetInvoiceHistory(int contactId);
        object MarkInvoicePaid(DateTime invoiceDate, string InvoiceRef, string confirmationNumber, string transactionNumber);
        object GetContactByCompanyName(string contactName);
        object GetContactByAttnName(string attnName);
        object GetContactByCompanyNameAndAttnName(string companyName, string attnName);
        object GetContactByContactId(int contactId);
        object GetContactByContactIdAndAttnName(int contactId, string attnName);
        object GetContactByInvoiceReference(string InvoiceRef);
    }
}
