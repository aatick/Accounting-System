using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountingSystem.Models.ViewModel
{
    public class AssetItem
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string AssetType { get; set; }
        public string AssetName { get; set; }
    }
}