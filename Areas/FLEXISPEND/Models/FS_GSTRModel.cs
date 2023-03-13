using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IEM.Areas.FLEXISPEND.Models
{
    public class FS_GSTRModel
    {
        public string GSTNumber { get; set; }
        public string GSTNSupplier { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceType { get; set; }
        public string InvoiceDate { get; set; }
        public string InvoiceValue { get; set; }
        public string PlaceofSupply { get; set; }
        public string ReverseCharge { get; set; }
        public string Rate { get; set; }
        public string TaxableValue { get; set; }
        public string integratedTax { get; set; }
        public string CentralTax { get; set; }
        public string StateUTTax { get; set; }
        public string Cess { get; set; }
        public string GSTR { get; set; }
        public string GSTRFillingDate { get; set; }
        public string ITCAvailability { get; set; }
        public string Reason { get; set; }
        public string TaxRate { get; set; }
        public string ProviderLocation { get; set; }
        public string ReceiverLocation { get; set; }
        public string ReceiverGSTN { get; set; }

        public string filename { get; set; }
        public string filepath { get; set; }
        public string headerno { get; set; }
        public string headergid { get; set; }
        public string makerid { get; set; }
        public string insertdate { get; set; }
        public string Remarks { get; set; }

        public string Suppliergid { get; set; }
        public string PANNo { get; set; }
        public string Tax { get; set; }
        public string TaxSubtype { get; set; }
        public string Status { get; set; }

    }
}