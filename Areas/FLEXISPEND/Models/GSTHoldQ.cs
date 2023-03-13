using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IEM.Areas.FLEXISPEND.Models
{
    public class GSTHoldQ
    {
        public string ecfId { get; set; }
        public string DocNo { get; set; }
        public string Ecf_Amount { get; set; }
        public string Ecf_Date { get; set; }
        public string Ecf_Base_Amount { get; set; }
        public string Ecf_Gst_Amount { get; set; }
        public string Raiser { get; set; }
        public string SupplierEmployee { get; set; }
        public string ClaimType { get; set; }
        public string fccode { get; set; }
        public SelectList SunkCost { get; set; }

        public int SunkCostGid { get; set; }
        public string SunkCostName { get; set; }
        public string Remark { get; set; }


        public string ecf_docsubtype_gid { get; set; }
        public string GSTNStatus { get; set; }
        public string RasierDept { get; set; }

        public string inVid { get; set; }
        public string inVno { get; set; }
        public string inVamT { get; set; }
        public string inVDate { get; set; }


    }
}