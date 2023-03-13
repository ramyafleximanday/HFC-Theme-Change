using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IEM.Areas.FLEXISPEND.Models
{
    public class EmployeeMaster_Model
    {
        public int Employee_gid { get; set; }
        public string Employee_Code { get; set; }
        public string Employee_Name { get; set; }
        public string Employee_dob { get; set; }
        public string Employee_doj { get; set; }
        public string Employee_designation { get; set; }
        public string Employee_hris_designation { get; set; }
        public string Employee_grade_code { get; set; }
        public string Employee_office_email { get; set; }
        public string Employee_mobile_no { get; set; }
        public string Employee_era_bankcode { get; set; }
        public string Employee_account_no { get; set; }
        public string Employee_ifsc_code { get; set; }
        public string Employee_supervisor { get; set; }
        public string Employee_status { get; set; }
        public string Employee_iswrlupdreq { get; set; }
    }
}