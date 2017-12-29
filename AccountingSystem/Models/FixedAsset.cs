using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models
{
    public class FixedAsset
    {
        public int Id { get; set; }
        public string AssetCode { get; set; }
        public int AssetNo { get; set; }
        public string AssetType { get; set; }
        public string PurchasedDate { get; set; }
        public double Price { get; set; }
        public double DepRate { get; set; }
        public string DepStartDate { get; set; }
        public string DepEndDate { get; set; }
        public string DepLife { get; set; }
        public string Supplier { get; set; }
        public string InvoiceNo { get; set; }
        public string LabelNo { get; set; }
        public string Description { get; set; }
        public string LastPosted { get; set; }
        public bool NoDep { get; set; }
        public bool Approved { get; set; }
        public string StopDep { get; set; }
        public double SoldAmount { get; set; }
        public string DisposalDate { get; set; }
        public bool Sold { get; set; }
        public double Profit { get; set; }
        public string Remarks { get; set; }
    }
}