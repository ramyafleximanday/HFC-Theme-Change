using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IEM.Areas.FLEXISPEND.Models
{
    public class GSTCreditNote_Model
    {
        public SelectList HSN { get; set; }
        public int HsnId { get; set; }
        public string HsnCode { get; set; }
        public SelectList ReceiverLocation { get; set; }
        public int ReceiverLocationid { get; set; }
        public string receiverlocationName { get; set; }

        public SelectList ProviderLocation { get; set; }

        public int ProviderLocationid { get; set; }
        public string Cygnet_Provider_Location_Name { get; set; } 
        // Gst Add Details
        public string GstApplicable { get; set; }
        public string Hsncode { get; set; }
        public string HsnDesc { get; set; }
        public string Subtax { get; set; }
        public decimal TaxableAmt { get; set; }
        public decimal GstRate { get; set; }
        public decimal TaxAmt { get; set; }
        public int InvoiceTaxGid { get; set; }
       
    }
}