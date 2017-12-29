using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public int Cid { get; set; }
        public string InvoiceNo { get; set; }
        public double TAmount { get; set; }
        public string Comments { get; set; }
        public string InvSendDt { get; set; }
        public string Sent { get; set; }
        public string SendingDt { get; set; }
        public string SendMode { get; set; }
        public string FullPayment { get; set; }
        public string PostedNature { get; set; }
        public int Emailed { get; set; }
        public int TotalPrinted { get; set; }
        public string Invalid { get; set; }
        public string UploadedPaymentStatus { get; set; }
        public string MoneyRecieptNo { get; set; }
        public string CompanyName { get; set; }
        public Int64 TotalInvoices { get; set; }
    }
}