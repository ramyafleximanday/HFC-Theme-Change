using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IEM.Areas.MASTERS.Models;
using IEM.Common;
using System.Data;
using IEM.Helper;
using IEM.App_Start;
using ClosedXML.Excel;
using System.IO;
namespace IEM.Areas.MASTERS.Controllers
{
    public class IEM_DELMATController : Controller
    {
        //
        // GET: /IEM_DELMAT/
        public dbLib db = new dbLib();
        public proLib plib = new proLib();
        private Iiem_mst_delmat Delmat;
        public int count = 0;
        private FileServer Cmnfs = new FileServer();
        CmnFunctions objCmnFunctions = new CmnFunctions();
        public IEM_DELMATController() :
            this(new IEM_MST_DELMAT()) { }

        public IEM_DELMATController(Iiem_mst_delmat Objist)
        {
            Delmat = Objist;
        }
        //[HttpPost]
        //public JsonResult AutocompleteEmployeeDelmat(string RaiserName)
        //{
        //    List<iem_mst_delmat> obj = new List<iem_mst_delmat>();
        //    obj = Delmat.AutocompleteEmployee(RaiserName,"").ToList();
        //    return Json(obj, JsonRequestBehavior.AllowGet);
        //}
     
        //Delmat Auto Complete
        public JsonResult GetAutoCompleteDelmat(string txt, string typeid)
        {
            try
            {
                List<string> result = new List<string>();
                
                DataSet ds = db.GetAutoComplete(txt, "20", typeid, plib.LoginUserId);
               
                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        result.Add(string.Format("{0}~{1}", dr[0], dr[1]));
                    }
                }
                iem_mst_delmat Delmatsetflow = new iem_mst_delmat();
                DataTable titleflag = new DataTable();
                int titleflagvalue;
                Delmatsetflow.title_gid = Convert.ToInt32(typeid);
                titleflag = Delmat.GetTitleById(Delmatsetflow.title_gid);
                titleflagvalue = int.Parse(titleflag.Rows[0]["title_flag"].ToString());
                Session["Titleflag"] = titleflagvalue;
                ds.Dispose();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch { return null; }
        }
        public JsonResult GetAutoCompleteDelmatexpection(string txt, string typeid)
        {
            try
            {
                List<string> result = new List<string>();

                DataSet ds = db.GetAutoComplete(txt, "20", typeid, plib.LoginUserId);

                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        result.Add(string.Format("{0}~{1}", dr[0], dr[1]));
                    }
                }
               
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch { return null; }
        }
        public ActionResult Index()
        {
            List<iem_mst_delmat> g = new List<iem_mst_delmat>();
            ViewBag.IsDelmatRole = Delmat.IsDelmatMaker();
            var delrole = ViewBag.IsDelmatRole;
            Session["RoleDelmat"] = delrole;
            iem_mst_delmat TypeModel = new iem_mst_delmat();
            TypeModel.GetDelmat = new SelectList(Delmat.GetDelmat(), "delmattype_gid", "delmattype_name");
            DataTable dtf = Delmat.GetDepartmentTable();
            List<SelectListItem> namess = new List<SelectListItem>();
            if (dtf.Rows.Count > 0)
            {
                foreach (DataRow row in dtf.Rows)
                {
                    namess.Add(new SelectListItem { Text = row["dept_name"].ToString(), Value = row["dept_gid"].ToString() });
                    TypeModel.Department = namess;
                }
            }
            ViewBag.viewdetail = "false";
            TypeModel.GetTitle = new SelectList(Delmat.GetTitle(), "title_gid", "title_name");
            TypeModel.GetSlab = new SelectList(Delmat.GetSlab(), "slab_gid", "slab_name");
            TypeModel.GetEmployee = new SelectList(Delmat.GetEmployee(), "employee_gid", "employee_code");
            string idvl = Convert.ToString( Session["delmatgid"]);
            Session["sno"] = "1";
            if (idvl != "")
            {
                ViewBag.viewdetail = "True";
                DataTable dt = new DataTable();
                DataTable getdelmatname = new DataTable();
                DataTable getdelmattype = new DataTable();
                DataTable getslabname = new DataTable();
                DataTable getslabgid = new DataTable();
                DataTable getdelmatypegid = new DataTable();
                DataTable getdeprtid = new DataTable();
                string delmatyname = string.Empty;
                int delmatypegid = 0;
                int slabgid = 0;
                DelmatVisisbleById VisibleId = Delmat.GetdelmaBranchActivityById(Convert.ToInt32(idvl));
                TypeModel.delmattype_branch_visible = Convert.ToString(VisibleId.delmattype_branch_visible);
                TypeModel.delmattype_branchtype_visible = Convert.ToString(VisibleId.delmattype_branchtype_visible);
                TypeModel.delmattype_claim_visible = Convert.ToString(VisibleId.delmattype_claim_visible);
                TypeModel.delmattype_dsa_visible = Convert.ToString(VisibleId.delmattype_dsa_visible);
                TypeModel.delmattype_pipit_visible = Convert.ToString(VisibleId.delmattype_pipit_visible);
                TypeModel.delmattype_it_visible = Convert.ToString(VisibleId.delmattype_it_visible);
                TypeModel.delmattype_exp_visible = Convert.ToString(VisibleId.delmattype_exp_visible);
                TypeModel.delmattype_budget_visible = Convert.ToString(VisibleId.delmattype_budget_visible);
                TypeModel.delmattype_jump_visible = Convert.ToString(VisibleId.delmattype_jump_visible);
                TypeModel.delmattype_active_visible = Convert.ToString(VisibleId.delmattype_active_visible);
                //ramya add delmat change on 19 Apr 22
                TypeModel.delmattype_ecftype_visible = Convert.ToString(VisibleId.delmattype_ecftype_visible);
                TypeModel.delmattype_parpr_visible = Convert.ToString(VisibleId.delmattype_parpr_visible);
                TypeModel.delmattype_withwithoutparpr_visible = Convert.ToString(VisibleId.delmattype_withwithoutparpr_visible);
                getdeprtid = Delmat.Getdelmatdeptgid(Convert.ToInt32(idvl));
                for (int i = 0; i < getdeprtid.Rows.Count; i++)
                {
                    string jn = string.IsNullOrEmpty(getdeprtid.Rows[i]["delmatdept_dept_gid"].ToString()) ? "" : getdeprtid.Rows[i]["delmatdept_dept_gid"].ToString();
                    TypeModel.lstSelecteddepartmentGid[i] = string.IsNullOrEmpty(getdeprtid.Rows[i]["delmatdept_dept_gid"].ToString()) ? "" : getdeprtid.Rows[i]["delmatdept_dept_gid"].ToString();
                }
                dt = Delmat.GetDepartmentTable();
                List<SelectListItem> names = new List<SelectListItem>();
                if (dt.Rows.Count > 0)
                {


                    foreach (DataRow row in dt.Rows)
                    {
                        names.Add(new SelectListItem { Text = row["dept_name"].ToString(), Value = row["dept_gid"].ToString() });
                        TypeModel.Department = names;
                    }


                }

                //editdelmatypegid
                getdelmatypegid = Delmat.Getdelmattyprgid(Convert.ToInt32(idvl));
                delmatypegid = int.Parse(getdelmatypegid.Rows[0]["delmat_delmattype_gid"].ToString());
                TypeModel.delmattype_gid = int.Parse(getdelmatypegid.Rows[0]["delmat_delmattype_gid"].ToString());
                //edit slab name
                getslabname = Delmat.GetDelmatnameById(Convert.ToInt32(idvl));
                TypeModel.delmat_name = getslabname.Rows[0]["delmat_name"].ToString();
                TypeModel.delmat_gid = Convert.ToInt32(getslabname.Rows[0]["delmat_gid"].ToString());
                //ramya added on 18 may 22
                TypeModel.delmat_ecftype = getslabname.Rows[0]["delmat_ecftype"].ToString();
                TypeModel.delmat_wwoparpr = getslabname.Rows[0]["delmat_wwoparpr"].ToString();
                TypeModel.delmat_parpr_flag = getslabname.Rows[0]["delmat_parpr_flag"].ToString();
                //editdelmatype
                getdelmattype = Delmat.Getdelmattypename(delmatypegid);
                TypeModel.delmattype_name = getdelmattype.Rows[0]["delmattype_code"].ToString();

                //editslabname
                getslabgid = Delmat.GetSlabGid(Convert.ToInt32(idvl));
                slabgid = int.Parse(getslabgid.Rows[0]["delmat_slab_gid"].ToString());
                Session["SlabGid"] = slabgid;
                TypeModel.slab_gid = int.Parse(getslabgid.Rows[0]["delmat_slab_gid"].ToString());
                //edit slabname
                getslabname = Delmat.Getslabname(slabgid);
                TypeModel.slab_name = getslabname.Rows[0]["slab_name"].ToString();
                Session["slabname"] = TypeModel.slab_name;
                Session["EditSlabgid"] = slabgid.ToString();

                TypeModel.GetDelmat = new SelectList(Delmat.GetDelmat(), "delmattype_gid", "delmattype_name", delmatypegid);
                TypeModel.GetTitle = new SelectList(Delmat.GetTitle(), "title_gid", "title_name");
                TypeModel.GetSlab = new SelectList(Delmat.GetSlab(), "slab_gid", "slab_name", slabgid);
                TypeModel.GetEmployee = new SelectList(Delmat.GetEmployee(), "employee_gid", "employee_code");
                ViewBag.id = idvl; // ramya modified on 20 jul 22
                Session["Update_delmat_gid"] = ViewBag.id;
               
            }
            return PartialView(TypeModel);


        }
        [HttpGet]
        public ActionResult Main()
        {
            List<iem_mst_delmat> g = new List<iem_mst_delmat>();
            ViewBag.IsDelmatRole = Delmat.IsDelmatMaker();
            var delrole = ViewBag.IsDelmatRole;
            Session["RoleDelmat"] = delrole;
         
            if(delrole=="C")
            {
                g = Delmat.getCheckerdelmat().ToList();
            }
            else
            {
                g = Delmat.getMakerdelmat().ToList();
            }
          
            Session["delmatgid"] = "";
            return PartialView(g);
        }

        [HttpPost]
        public JsonResult GetFunction(iem_mst_delmat delmatfunction)
        {
            Session["function"] = delmatfunction.Viewfor.ToString();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public PartialViewResult EditDelmat(iem_mst_delmat EditDelmat, int id)
        {

            DataTable dt = new DataTable();
            DataTable getdelmatname = new DataTable();
            DataTable getdelmattype = new DataTable();
            DataTable getslabname = new DataTable();
            DataTable getslabgid = new DataTable();
            DataTable getdelmatypegid = new DataTable();
            DataTable getdeprtid = new DataTable();
            string delmatyname = string.Empty;
            int delmatypegid = 0;
            int slabgid = 0;
            ViewBag.IsDelmatRole = Delmat.IsDelmatMaker();
            DelmatVisisbleById VisibleId = Delmat.GetdelmaBranchActivityById(id);
            EditDelmat.delmattype_branch_visible = Convert.ToString(VisibleId.delmattype_branch_visible);
            EditDelmat.delmattype_branchtype_visible = Convert.ToString(VisibleId.delmattype_branchtype_visible);
            EditDelmat.delmattype_claim_visible = Convert.ToString(VisibleId.delmattype_claim_visible);
            EditDelmat.delmattype_dsa_visible = Convert.ToString(VisibleId.delmattype_dsa_visible);
            EditDelmat.delmattype_pipit_visible = Convert.ToString(VisibleId.delmattype_pipit_visible);
            EditDelmat.delmattype_it_visible = Convert.ToString(VisibleId.delmattype_it_visible);
            EditDelmat.delmattype_exp_visible = Convert.ToString(VisibleId.delmattype_exp_visible);
            EditDelmat.delmattype_budget_visible = Convert.ToString(VisibleId.delmattype_budget_visible);
            EditDelmat.delmattype_jump_visible = Convert.ToString(VisibleId.delmattype_jump_visible);
            EditDelmat.delmattype_active_visible = Convert.ToString(VisibleId.delmattype_active_visible);

            //add delmat change
            EditDelmat.delmattype_ecftype_visible = Convert.ToString(VisibleId.delmattype_ecftype_visible);
            EditDelmat.delmattype_parpr_visible = Convert.ToString(VisibleId.delmattype_parpr_visible);
            EditDelmat.delmattype_withwithoutparpr_visible = Convert.ToString(VisibleId.delmattype_withwithoutparpr_visible);



            getdeprtid = Delmat.Getdelmatdeptgid(id);
            for (int i = 0; i < getdeprtid.Rows.Count; i++)
            {
                EditDelmat.lstSelecteddepartmentGid[i] = string.IsNullOrEmpty(getdeprtid.Rows[i]["delmatdept_dept_gid"].ToString()) ? "" : getdeprtid.Rows[i]["delmatdept_dept_gid"].ToString();
            }
            dt = Delmat.GetDepartmentTable();
            List<SelectListItem> names = new List<SelectListItem>();
            if (dt.Rows.Count > 0)
            {


                    foreach (DataRow row in dt.Rows)
                    {
                        names.Add(new SelectListItem { Text = row["dept_name"].ToString(), Value = row["dept_gid"].ToString() });
                        EditDelmat.Department = names;
                    }


                }

                //editdelmatypegid
                getdelmatypegid = Delmat.Getdelmattyprgid(id);
                delmatypegid = int.Parse(getdelmatypegid.Rows[0]["delmat_delmattype_gid"].ToString());
                EditDelmat.delmattype_gid = int.Parse(getdelmatypegid.Rows[0]["delmat_delmattype_gid"].ToString());
                //edit slab name
                getslabname = Delmat.GetDelmatnameById(id);
                EditDelmat.delmat_name = getslabname.Rows[0]["delmat_name"].ToString();
                EditDelmat.delmat_gid = Convert.ToInt32(getslabname.Rows[0]["delmat_gid"].ToString());
            //ramya added on 18 may 22
                EditDelmat.delmat_ecftype = getslabname.Rows[0]["delmat_ecftype"].ToString();
                EditDelmat.delmat_wwoparpr = getslabname.Rows[0]["delmat_wwoparpr"].ToString();
                EditDelmat.delmat_parpr_flag = getslabname.Rows[0]["delmat_parpr_flag"].ToString();
                //editdelmatype
                getdelmattype = Delmat.Getdelmattypename(delmatypegid);
                EditDelmat.delmattype_name = getdelmattype.Rows[0]["delmattype_code"].ToString();

                //editslabname
                getslabgid = Delmat.GetSlabGid(id);
                slabgid = int.Parse(getslabgid.Rows[0]["delmat_slab_gid"].ToString());
                Session["SlabGid"] = slabgid;
                EditDelmat.slab_gid = int.Parse(getslabgid.Rows[0]["delmat_slab_gid"].ToString());
                //edit slabname
                getslabname = Delmat.Getslabname(slabgid);
                EditDelmat.slab_name = getslabname.Rows[0]["slab_name"].ToString();
                Session["slabname"] = EditDelmat.slab_name;
                Session["EditSlabgid"] = slabgid.ToString();

            EditDelmat.GetDelmat = new SelectList(Delmat.GetDelmat(), "delmattype_gid", "delmattype_name", delmatypegid);
            EditDelmat.GetTitle = new SelectList(Delmat.GetTitle(), "title_gid", "title_name");
            EditDelmat.GetSlab = new SelectList(Delmat.GetSlab(), "slab_gid", "slab_name", slabgid);
            EditDelmat.GetEmployee = new SelectList(Delmat.GetEmployee(), "employee_gid", "employee_code");
            Session["Update_delmat_gid"] = id;
            Session["delmatgid"] = id;
            //attachment
            DataTable dt1 = new DataTable();
            List<AttachFile> attach1 = EditDelmat.AttachFile;
            //  EditDelmat = Delmat.GetdelmatAttachmentById(id);

            //EditDelmatAttachmentId(id);

            // List<AttachFile> attach1 = Delmat.GetdelmatAttachmentById(id);
            //foreach (DataRow dr in EditDelmat.Rows)
            //{
            //    attach1.Add(
            //    new iem_mst_delmat
            //    {
            //        delmatexception_name = (dr["employee_name"].ToString()),
            //        delmatexception_gid = int.Parse(dr["delmatexception_gid"].ToString()),
            //        delmatexception_to = int.Parse(dr["delmatexception_to"].ToString()),
            //        delmatexception_limit = decimal.Parse(dr["delmatexception_limit"].ToString())

            //    });
            //};
            //if (dt1.Rows.Count > 0)
            //{

            //    foreach (DataRow row in dt.Rows)
            //    {
            //        attach = new AttachFile();

            //        attach.AttachmentFilenameId = Convert.ToInt32(row["attachment_gid"].ToString());
            //        attach.AttachmentFilename = row["attachment_filename"].ToString();
            //        attach.AttachmentDescription = row["attachment_desc"].ToString();
            //        attach.AttachmentBy = row["attachment_by"].ToString();
            //        attach.AttachmentDate = row["attachment_date"].ToString();
            //       // EditDelmat.Add(attach);
            //        EditDelmat.AttachFile(attach);
            //    }



            // }


            if (Convert.ToString(Session["function"]) == "Edit")
            {
                ViewBag.viewfor = "";
            }
            else
            {
                ViewBag.viewfor = "disabled";
            }
            if (Convert.ToString(Session["function"]) == "Delete")
            {
                ViewBag.viewfor1 = "Delete";
            }

            return PartialView(EditDelmat);
        }

        [HttpPost]
        public ActionResult Main(string delmatnamefilter = null)
        {
            List<iem_mst_delmat> g = new List<iem_mst_delmat>();
         
            ViewBag.IsDelmatRole = Delmat.IsDelmatMaker();
            var delrole = ViewBag.IsDelmatRole;
            Session["RoleDelmat"] = delrole;

            if (delrole == "C")
            {
                g = Delmat.getCheckerdelmat().ToList();
            }
            else
            {
                g = Delmat.getMakerdelmat().ToList();
            }
            if (string.IsNullOrEmpty(delmatnamefilter) == false || string.IsNullOrWhiteSpace(delmatnamefilter) == false)
            {
                ViewBag.filter = delmatnamefilter.ToString();
                g = g.Where(x => delmatnamefilter == null ||
                (x.delmat_name.ToUpper().Contains(delmatnamefilter.ToUpper()))).ToList();
            }
            return View(g);

        }

        [HttpGet]
        public PartialViewResult Create()
        {

            return PartialView();
        }


        [HttpPost]
        public JsonResult GetValue(iem_mst_delmat Delmatsetflow)
        {
            iem_mst_delmat TypeModel = new iem_mst_delmat();

            DataTable titleflag = new DataTable();
            int titleflagvalue;
            titleflag = Delmat.GetTitleById(Delmatsetflow.title_gid);
            titleflagvalue = int.Parse(titleflag.Rows[0]["title_flag"].ToString());
            Session["Titleflag"] = titleflagvalue;
            if (titleflagvalue == 1)
            {
                TypeModel.GetEmployee = new SelectList(Delmat.GetEmployee(), "employee_gid", "employee_code");
                return Json(Delmat.GetEmployee(), JsonRequestBehavior.AllowGet);
            }
            else if (titleflagvalue == 2)
            {
                TypeModel.GetGrade = new SelectList(Delmat.GetGrade(), "employee_gid", "employee_code");
                return Json(Delmat.GetGrade(), JsonRequestBehavior.AllowGet);
            }
            else if (titleflagvalue == 3)
            {
                TypeModel.GetDesignation = new SelectList(Delmat.GetDesignation(), "employee_gid", "employee_code");
                return Json(Delmat.GetDesignation(), JsonRequestBehavior.AllowGet);
            }
            else if (titleflagvalue == 4)
            {
                TypeModel.GetRole = new SelectList(Delmat.GetRole(), "employee_gid", "employee_code");
                return Json(Delmat.GetRole(), JsonRequestBehavior.AllowGet);
            }

            return Json(Delmat.GetEmployee(), JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public PartialViewResult AddDelmatflow(iem_mst_delmat Delmatexception)
        {
            iem_mst_delmat TypeModel = new iem_mst_delmat();
            TypeModel.GetTitle = new SelectList(Delmat.GetTitle(), "title_gid", "title_name");
            TypeModel.GetEmployee = new SelectList(Delmat.GetEmployee(), "employee_gid", "employee_code");

            return PartialView(TypeModel);
        }
        [HttpGet]
        public PartialViewResult AddDelmatflowById(iem_mst_delmat Delmatexception)
        {
            iem_mst_delmat TypeModel = new iem_mst_delmat();
            TypeModel.GetTitle = new SelectList(Delmat.GetTitle(), "title_gid", "title_name");
            TypeModel.GetEmployee = new SelectList(Delmat.GetEmployee(), "employee_gid", "employee_code");

            return PartialView(TypeModel);
        }
        [HttpGet]
        public PartialViewResult EditDelmatflowById(int id, string viewfor)
        {
            DataTable SelectDelmatGidCount = new DataTable();
            iem_mst_delmat TypeModel = new iem_mst_delmat();
            SelectDelmatGidCount = Delmat.GetflowgidCountdel(id);
            foreach (DataRow dr in SelectDelmatGidCount.Rows)
            {
                TypeModel.delmatsetflowgid = id;
                TypeModel.delmat_gid = int.Parse(dr["delmatflow_delmat_gid"].ToString());
                TypeModel.employee_gid = int.Parse(dr["delmatflow_title_ref_gid"].ToString());
                TypeModel.InFlowCount = int.Parse(dr["delmatflow_order"].ToString());
                TypeModel.lsdelmatflowtitlevalue = dr["delmatflow_title_gid"].ToString();
                TypeModel.lsdelmatflowaddapprovalvalue = dr["delmatflow_additional_approval"].ToString();
                TypeModel.lsTitlename = dr["title_name"].ToString();
            }
            Session["maxflow"] = TypeModel.InFlowCount;
            TypeModel.GetTitle = new SelectList(Delmat.GetTitle(), "title_gid", "title_name", TypeModel.lsdelmatflowtitlevalue);           
            int titleflagvalue;
            titleflagvalue = int.Parse(TypeModel.lsdelmatflowtitlevalue);
            Session["Titleflag"] = titleflagvalue;
            if (titleflagvalue == 1)
            {
                TypeModel.GetEmployee = new SelectList(Delmat.GetEmployee(), "employee_gid", "employee_code", TypeModel.employee_gid);             
            }
            else if (titleflagvalue == 2)
            {
                TypeModel.GetGrade = new SelectList(Delmat.GetGrade(), "employee_gid", "employee_code", TypeModel.employee_gid);                
            }
            else if (titleflagvalue == 3)
            {
                TypeModel.GetDesignation = new SelectList(Delmat.GetDesignation(), "employee_gid", "employee_code", TypeModel.employee_gid);              
            }
            else if (titleflagvalue == 4)
            {
                TypeModel.GetRole = new SelectList(Delmat.GetRole(), "employee_gid", "employee_code", TypeModel.employee_gid);               
            }         

            return PartialView(TypeModel);
        }
        [HttpGet]
        public PartialViewResult AddDelmatException()
        {
            iem_mst_delmat TypeModel = new iem_mst_delmat();
            //TypeModel.GetEmployee = new SelectList(Delmat.GetEmployee(), "employee_gid", "employee_code");
            return PartialView(TypeModel);
        }
        [HttpGet]
        public PartialViewResult AddDelmatExceptionById()
        {
            iem_mst_delmat TypeModel = new iem_mst_delmat();
           // TypeModel.GetEmployee = new SelectList(Delmat.GetEmployee(), "employee_gid", "employee_code");
            return PartialView(TypeModel);
        }

        [HttpPost]
        public JsonResult InsertDelmatException(iem_mst_delmat Insertdelmat)
        {
            string res = string.Empty;

            if (ModelState.IsValid)
            {
                string dlmt = Convert.ToString(Session["delmatgid"]);
                if (dlmt != "")
                {
                    int delmatgid = int.Parse(Session["delmatgid"].ToString());
                    Insertdelmat.delmat_gid = delmatgid;
                    res = Delmat.InsertDelmatException(Insertdelmat);
                }
                else
                {
                    res = "Please Save Set Flow And Then Proceed";
                }

            }

            return Json(res, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult InsertDelmatExceptionById(iem_mst_delmat InserDelmatExceptionbyid)
        {
            string res = string.Empty;

            if (ModelState.IsValid)
            {

                int delmatgid = int.Parse(Session["Update_delmat_gid"].ToString());
                // int delmatgid = int.Parse(Session["delmatgid"].ToString());
                InserDelmatExceptionbyid.delmat_gid = delmatgid;
                res = Delmat.InsertDelmatException(InserDelmatExceptionbyid);

            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult GetDelmatException()
        {

            return PartialView();
        }


        [HttpPost]
        public JsonResult InsertDelmat(iem_mst_delmat DelmatInformation, string[] lstSelecteddepartmentGid)
        {
            Session["function"] = "Create"; // ramya added on 07 Mar 22 for Found Duplicate Delmat msg issue.
            DataTable slabrangegid = new DataTable();
            DataTable delmatgid = new DataTable();
            DataTable getslabname = new DataTable();
            string result = string.Empty;

            string lstDelmat = string.Empty;
            string slabname = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    int delmatgidval=0;
                    DelmatInformation.delmat_insert_by = Convert.ToInt32(objCmnFunctions.GetLoginUserGid().ToString()); 
                    
                    result = Delmat.InsertDelmat(DelmatInformation, lstSelecteddepartmentGid);
                    if (result == "success")
                    {
                        delmatgid = Delmat.GetDelmatGid(DelmatInformation.delmat_name);

                        //if (delmatgid.Rows.Count>1)
                        //{
                        //    delmatgidval = int.Parse(delmatgid.Rows[0]["delmat_gid"].ToString());
                        //}
                        //else
                        //{
                        //    delmatgidval = 0;
                        //}
                        delmatgidval = int.Parse(delmatgid.Rows[0]["delmat_gid"].ToString());
                        DelmatInformation.delmat_gid = delmatgidval;
                        lstDelmat = Delmat.AddDelmatdepartment(DelmatInformation, lstSelecteddepartmentGid);
                        getslabname = Delmat.GetSlabById(DelmatInformation.delmat_slab_gid);
                        Session["slabname"] = getslabname.Rows[0]["slab_name"].ToString();
                        Session["SlabGid"] = DelmatInformation.delmat_slab_gid;
                        Session["delmatgid"] = delmatgidval;
                        Session["SNo"] = int.Parse(count.ToString());
                        ViewBag.setflow = "True";
                       
                    }
                    else
                    {
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }

                   
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
            return Json(lstDelmat, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public PartialViewResult editsetflow()
        {

            return PartialView();
        }
        [HttpGet]
        public PartialViewResult SetFlow()
        {

            return PartialView();
        }
        [HttpPost]
        public JsonResult CreateNewDelmatselflow(iem_mst_delmat Delmatsetflow)
        {
            int countval;
            iem_mst_delmat Typemodel = new iem_mst_delmat();
            DataTable getvalue = new DataTable();
            DataTable gettitlevalue = new DataTable();
            DataTable getsetflowId = new DataTable();
            string empcode;
            string gradecode;
            string designationcode;
            string departmentcode;
            string titlevalue;
            string dlmt = Convert.ToString(Session["SlabGid"]);
            string nm = Convert.ToString(Session["delmatgid"]);
            if (dlmt != "" && nm!="")
            {
            int slabgid = int.Parse(Session["SlabGid"].ToString());

            int setflowId = 0;
            int delmatgid = int.Parse(Session["delmatgid"].ToString());
            
                DataTable rowscount = new DataTable();
                rowscount = Delmat.GetrowsOnlyByID(slabgid);

                Delmatsetflow.delmat_gid = delmatgid;
                Session["Flow"] = Delmatsetflow.Flow;
                string result = string.Empty;
                result = Delmat.checkduplicateFlow(Delmatsetflow);
                if (result == "Not There")
                {
                    string flowvl = string.Empty;
                    flowvl = Delmat.checkFlow(Delmatsetflow);
                    if (flowvl != "Please  Enter Correct Flow Order !")
                    {
                        //  flowvl = Convert.ToString(Convert.ToInt32(flowvl)+1);
                        if (flowvl != "Please  Enter Correct Flow Order !")
                        {
                            int Sno = int.Parse(Session["SNo"].ToString());
                            countval = Sno + 1;
                            Delmatsetflow.Countvalue = Sno;
                            Session["SNo"] = Delmatsetflow.Countvalue;
                            Session["sno"] = countval;
                            Delmatsetflow.slabrangecount = rowscount.Rows.Count;
                            Delmatsetflow.slabrange_gid = int.Parse(slabgid.ToString());
                            Delmatsetflow.delmat_gid = delmatgid;

                            Session["rowcount"] = rowscount.Rows.Count.ToString();
                            Session["SlabGid"] = Delmatsetflow.slabrange_gid.ToString();
                            Session["delmatgid"] = Delmatsetflow.delmat_gid.ToString();
                            try
                            {
                                int titleflag = int.Parse(Session["Titleflag"].ToString());
                                int valuegid = Delmatsetflow.value_gid;
                                gettitlevalue = Delmat.GetTitleById(Delmatsetflow.title_gid);
                                titlevalue = gettitlevalue.Rows[0]["title_name"].ToString();

                                if (titleflag == 1)
                                {
                                    getvalue = Delmat.GetValueDetailsById(Delmatsetflow.value_gid, titleflag);
                                    empcode = getvalue.Rows[0][0].ToString() + "-" + getvalue.Rows[0][1].ToString();
                                    Delmatsetflow.employee_code = empcode;
                                   // Delmatsetflow.employee_gid = Convert.ToInt32(getvalue.Rows[0][1].ToString());
                                    Delmatsetflow.TitleNamegid = Delmatsetflow.title_gid;
                                    Delmatsetflow.TitleName = empcode;
                                    Delmatsetflow.title_gid = Delmatsetflow.value_gid;

                                    Delmat.InsertDelmatsetflow(Delmatsetflow);

                                    getsetflowId = Delmat.GetSetFlowId();
                                    setflowId = int.Parse(getsetflowId.Rows[0]["Column1"].ToString());
                                    Session["GID"] = setflowId;
                                    Delmatsetflow.GID = setflowId;

                                }
                                else if (titleflag == 2)
                                {
                                    getvalue = Delmat.GetValueDetailsById(Delmatsetflow.value_gid, titleflag);
                                    gradecode = getvalue.Rows[0][1].ToString();
                                    Delmatsetflow.grade_code = gradecode;
                                  //  Delmatsetflow.grade_gid = Convert.ToInt32(getvalue.Rows[0][1].ToString());
                                    Delmatsetflow.TitleNamegid = Delmatsetflow.title_gid;
                                    Delmatsetflow.TitleName = gradecode;
                                    Delmatsetflow.title_gid = Delmatsetflow.value_gid;

                                    Delmat.InsertDelmatsetflow(Delmatsetflow);
                                    getsetflowId = Delmat.GetSetFlowId();
                                    setflowId = int.Parse(getsetflowId.Rows[0]["Column1"].ToString());
                                    Session["GID"] = setflowId;
                                    Delmatsetflow.GID = setflowId;
                                }
                                else if (titleflag == 3)
                                {
                                    getvalue = Delmat.GetValueDetailsById(Delmatsetflow.value_gid, titleflag);
                                    designationcode = getvalue.Rows[0][1].ToString();
                                    Delmatsetflow.designation_code = designationcode;
                                    //Delmatsetflow.designation_gid = Convert.ToInt32(getvalue.Rows[0][1].ToString());

                                    Delmatsetflow.TitleNamegid = Delmatsetflow.title_gid;
                                    Delmatsetflow.TitleName = designationcode;
                                    Delmatsetflow.title_gid = Delmatsetflow.value_gid;
                                    Delmat.InsertDelmatsetflow(Delmatsetflow);
                                    getsetflowId = Delmat.GetSetFlowId();
                                    setflowId = int.Parse(getsetflowId.Rows[0]["Column1"].ToString());
                                    Session["GID"] = setflowId;
                                    Delmatsetflow.GID = setflowId;
                                }
                                else if (titleflag == 4)
                                {
                                    getvalue = Delmat.GetValueDetailsById(Delmatsetflow.value_gid, titleflag);
                                    departmentcode =  getvalue.Rows[0][1].ToString();
                                    Delmatsetflow.role_code = departmentcode;
                                    //Delmatsetflow.role_gid = Convert.ToInt32(getvalue.Rows[0][1].ToString());

                                    Delmatsetflow.TitleNamegid = Delmatsetflow.title_gid;
                                    Delmatsetflow.TitleName = departmentcode;
                                    Delmatsetflow.title_gid = Delmatsetflow.value_gid;
                                    Delmat.InsertDelmatsetflow(Delmatsetflow);
                                    getsetflowId = Delmat.GetSetFlowId();
                                    setflowId = int.Parse(getsetflowId.Rows[0]["Column1"].ToString());
                                    Session["GID"] = setflowId;
                                    Delmatsetflow.GID = setflowId;

                                }
                                Delmatsetflow.title_name = titlevalue;
                                Delmatsetflow.title_flag = titleflag;


                            }
                            catch (Exception ex)
                            {

                            }
                            finally
                            {
                                getvalue.Rows.Clear();
                                gettitlevalue.Rows.Clear();
                            }

                            return Json(Delmatsetflow, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json("Please Enter Correct Flow Order !", JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json("Please Enter Correct Flow Order !", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json("Duplicate Flow Order !", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("Please Save Set Flow And Then Proceed", JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public JsonResult CreateNewDelmatselflowByIDdublicateedit(iem_mst_delmat Delmatsetflow)
        {
            int countval;
            iem_mst_delmat Typemodel = new iem_mst_delmat();
            DataTable getvalue = new DataTable();
            DataTable gettitlevalue = new DataTable();
            DataTable getsetflowId = new DataTable();
            string empcode;
            string gradecode;
            string designationcode;
            string departmentcode;
            string titlevalue;
            int slabgid = 0;
            int delmatgid = 0;
            int setflowId = 0;
            if (Convert.ToString(Session["EditSlabgid"]) != "")
            {
                slabgid = int.Parse(Session["EditSlabgid"].ToString());
            }
            else
            {
                slabgid = int.Parse(Session["SlabGid"].ToString());
            }
            if (Convert.ToString(Session["Update_delmat_gid"]) != "")
            {
                delmatgid = int.Parse(Session["Update_delmat_gid"].ToString());
            }
            else
            {
                delmatgid = int.Parse(Session["delmatgid"].ToString());
            }
            DataTable rowscount = new DataTable();
            rowscount = Delmat.GetrowsOnlyByID(slabgid);

            Delmatsetflow.delmat_gid = delmatgid;
            Session["Flow"] = Delmatsetflow.Flow;
            string result = string.Empty;
            result = "" ;
            if (result != "Not There")
            {
                int Sno = Delmatsetflow.Flow;
                countval = Sno;
                Delmatsetflow.Countvalue = countval;
                Session["SNo"] = Delmatsetflow.Countvalue;
                Session["sno"] = countval + 1;
                Delmatsetflow.slabrangecount = rowscount.Rows.Count;
                Delmatsetflow.slabrange_gid = int.Parse(slabgid.ToString());
                Delmatsetflow.delmat_gid = delmatgid;

                Session["rowcount"] = rowscount.Rows.Count.ToString();
                Session["SlabGid"] = Delmatsetflow.slabrange_gid.ToString();
                Session["delmatgid"] = Delmatsetflow.delmat_gid.ToString();
                try
                {
                    int titleflag = int.Parse(Session["Titleflag"].ToString());
                    int valuegid = Delmatsetflow.value_gid;
                    gettitlevalue = Delmat.GetTitleById(Delmatsetflow.title_gid);
                    titlevalue = gettitlevalue.Rows[0]["title_name"].ToString();

                    if (titleflag == 1)
                    {
                        getvalue = Delmat.GetValueDetailsById(Delmatsetflow.value_gid, titleflag);
                        empcode = getvalue.Rows[0][0].ToString() + "-" + getvalue.Rows[0][1].ToString();
                        Delmatsetflow.employee_code = empcode;
                        Delmatsetflow.TitleNamegid = Delmatsetflow.title_gid;
                        Delmatsetflow.TitleName = empcode;
                        Delmatsetflow.title_gid = Delmatsetflow.value_gid;

                        Delmat.updateDelmatsetflowedit(Delmatsetflow);

                        getsetflowId = Delmat.GetSetFlowId();
                        setflowId = int.Parse(getsetflowId.Rows[0]["Column1"].ToString());
                        Session["GID"] = setflowId;
                        Delmatsetflow.GID = setflowId;

                    }
                    else if (titleflag == 2)
                    {
                        getvalue = Delmat.GetValueDetailsById(Delmatsetflow.value_gid, titleflag);
                        gradecode = getvalue.Rows[0][1].ToString();
                        Delmatsetflow.grade_code = gradecode;
                        Delmatsetflow.TitleNamegid = Delmatsetflow.title_gid;
                        Delmatsetflow.TitleName = gradecode;
                        Delmatsetflow.title_gid = Delmatsetflow.value_gid;

                        Delmat.updateDelmatsetflowedit(Delmatsetflow);
                        getsetflowId = Delmat.GetSetFlowId();
                        setflowId = int.Parse(getsetflowId.Rows[0]["Column1"].ToString());
                        Session["GID"] = setflowId;
                        Delmatsetflow.GID = setflowId;
                    }
                    else if (titleflag == 3)
                    {
                        getvalue = Delmat.GetValueDetailsById(Delmatsetflow.value_gid, titleflag);
                        designationcode = getvalue.Rows[0][1].ToString();
                        Delmatsetflow.designation_code = designationcode;
                        Delmatsetflow.TitleNamegid = Delmatsetflow.title_gid;
                        Delmatsetflow.TitleName = designationcode;
                        Delmatsetflow.title_gid = Delmatsetflow.value_gid;
                        Delmat.updateDelmatsetflowedit(Delmatsetflow);
                        getsetflowId = Delmat.GetSetFlowId();
                        setflowId = int.Parse(getsetflowId.Rows[0]["Column1"].ToString());
                        Session["GID"] = setflowId;
                        Delmatsetflow.GID = setflowId;
                    }
                    else if (titleflag == 4)
                    {
                        getvalue = Delmat.GetValueDetailsById(Delmatsetflow.value_gid, titleflag);
                        departmentcode = getvalue.Rows[0][1].ToString();
                        Delmatsetflow.role_code = departmentcode;
                        Delmatsetflow.TitleNamegid = Delmatsetflow.title_gid;
                        Delmatsetflow.TitleName = departmentcode;
                        Delmatsetflow.title_gid = Delmatsetflow.value_gid;
                        Delmat.updateDelmatsetflowedit(Delmatsetflow);
                        getsetflowId = Delmat.GetSetFlowId();
                        setflowId = int.Parse(getsetflowId.Rows[0]["Column1"].ToString());
                        Session["GID"] = setflowId;
                        Delmatsetflow.GID = setflowId;

                    }
                    Delmatsetflow.title_name = titlevalue;
                    Delmatsetflow.title_flag = titleflag;


                }
                catch (Exception ex)
                {

                }
                finally
                {
                    getvalue.Rows.Clear();
                    gettitlevalue.Rows.Clear();
                }



            }

            return Json(Delmatsetflow, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult CreateNewDelmatselflowByIDdublicate(iem_mst_delmat Delmatsetflow)
        {
            int countval;
            iem_mst_delmat Typemodel = new iem_mst_delmat();
            DataTable getvalue = new DataTable();
            DataTable gettitlevalue = new DataTable();
            DataTable getsetflowId = new DataTable();
            string empcode;
            string gradecode;
            string designationcode;
            string departmentcode;
            string titlevalue;
            int slabgid = 0;
            int delmatgid = 0;
            int setflowId = 0;
            if (Convert.ToString(Session["EditSlabgid"]) != "")
            {
                 slabgid = int.Parse(Session["EditSlabgid"].ToString());
            }
            else
            {
                slabgid = int.Parse(Session["SlabGid"].ToString());
            }
            if (Convert.ToString(Session["Update_delmat_gid"]) != "")
            {               
                 delmatgid = int.Parse(Session["Update_delmat_gid"].ToString());
            }
            else
            {                
                 delmatgid = int.Parse(Session["delmatgid"].ToString());
            }
            DataTable rowscount = new DataTable();
            rowscount = Delmat.GetrowsOnlyByID(slabgid);

            Delmatsetflow.delmat_gid = delmatgid;
            Session["Flow"] = Delmatsetflow.Flow;
            string result = string.Empty;
            result = Delmat.checkduplicateFlow(Delmatsetflow);
            if (result != "Not There")
            {               
                int Sno = Delmatsetflow.Flow;
                countval = Sno;
                Delmatsetflow.Countvalue = countval;
                Session["SNo"] = Delmatsetflow.Countvalue;
                Session["sno"] = countval+1;
                Delmatsetflow.slabrangecount = rowscount.Rows.Count;
                Delmatsetflow.slabrange_gid = int.Parse(slabgid.ToString());
                Delmatsetflow.delmat_gid = delmatgid;

                Session["rowcount"] = rowscount.Rows.Count.ToString();
                Session["SlabGid"] = Delmatsetflow.slabrange_gid.ToString();
                Session["delmatgid"] = Delmatsetflow.delmat_gid.ToString();
                try
                {
                    int titleflag = int.Parse(Session["Titleflag"].ToString());
                    int valuegid = Delmatsetflow.value_gid;
                    gettitlevalue = Delmat.GetTitleById(Delmatsetflow.title_gid);
                    titlevalue = gettitlevalue.Rows[0]["title_name"].ToString();

                    if (titleflag == 1)
                    {
                        getvalue = Delmat.GetValueDetailsById(Delmatsetflow.value_gid, titleflag);
                        empcode = getvalue.Rows[0][0].ToString() + "-" + getvalue.Rows[0][1].ToString();
                        Delmatsetflow.employee_code = empcode;
                        Delmatsetflow.TitleNamegid = Delmatsetflow.title_gid;
                        Delmatsetflow.TitleName = empcode;
                        Delmatsetflow.title_gid = Delmatsetflow.value_gid;

                        Delmat.InsertDelmatsetflow(Delmatsetflow);
                        int flocntvl = Delmatsetflow.InFlowCount;
                        flocntvl++;
                        Session["sno"] = Convert.ToString(flocntvl);
                        getsetflowId = Delmat.GetSetFlowId();
                        setflowId = int.Parse(getsetflowId.Rows[0]["Column1"].ToString());
                        Session["GID"] = setflowId;
                        Delmatsetflow.GID = setflowId;

                    }
                    else if (titleflag == 2)
                    {
                        getvalue = Delmat.GetValueDetailsById(Delmatsetflow.value_gid, titleflag);
                        gradecode = getvalue.Rows[0][1].ToString();
                        Delmatsetflow.grade_code = gradecode;
                        Delmatsetflow.TitleNamegid = Delmatsetflow.title_gid;
                        Delmatsetflow.TitleName = gradecode;
                        Delmatsetflow.title_gid = Delmatsetflow.value_gid;

                        Delmat.InsertDelmatsetflow(Delmatsetflow);
                        int flocntvl = Delmatsetflow.InFlowCount;
                        flocntvl++;
                        Session["sno"] = Convert.ToString(flocntvl);
                        getsetflowId = Delmat.GetSetFlowId();
                        setflowId = int.Parse(getsetflowId.Rows[0]["Column1"].ToString());
                        Session["GID"] = setflowId;
                        Delmatsetflow.GID = setflowId;
                    }
                    else if (titleflag == 3)
                    {
                        getvalue = Delmat.GetValueDetailsById(Delmatsetflow.value_gid, titleflag);
                        designationcode =  getvalue.Rows[0][1].ToString();
                        Delmatsetflow.designation_code = designationcode;
                        Delmatsetflow.TitleNamegid = Delmatsetflow.title_gid;
                        Delmatsetflow.TitleName = designationcode;
                        Delmatsetflow.title_gid = Delmatsetflow.value_gid;
                        Delmat.InsertDelmatsetflow(Delmatsetflow);
                        int flocntvl = Delmatsetflow.InFlowCount;
                        flocntvl++;
                        Session["sno"] = Convert.ToString(flocntvl);
                        getsetflowId = Delmat.GetSetFlowId();
                        setflowId = int.Parse(getsetflowId.Rows[0]["Column1"].ToString());
                        Session["GID"] = setflowId;
                        Delmatsetflow.GID = setflowId;
                    }
                    else if (titleflag == 4)
                    {
                        getvalue = Delmat.GetValueDetailsById(Delmatsetflow.value_gid, titleflag);
                        departmentcode = getvalue.Rows[0][1].ToString();
                        Delmatsetflow.role_code = departmentcode;
                        Delmatsetflow.TitleNamegid = Delmatsetflow.title_gid;
                        Delmatsetflow.TitleName = departmentcode;
                        Delmatsetflow.title_gid = Delmatsetflow.value_gid;
                        Delmat.InsertDelmatsetflow(Delmatsetflow);
                        int flocntvl = Delmatsetflow.InFlowCount;
                        flocntvl++;
                        Session["sno"] = Convert.ToString(flocntvl);
                        getsetflowId = Delmat.GetSetFlowId();
                        setflowId = int.Parse(getsetflowId.Rows[0]["Column1"].ToString());
                        Session["GID"] = setflowId;
                        Delmatsetflow.GID = setflowId;

                    }
                    Delmatsetflow.title_name = titlevalue;
                    Delmatsetflow.title_flag = titleflag;


                }
                catch (Exception ex)
                {

                }
                finally
                {
                    getvalue.Rows.Clear();
                    gettitlevalue.Rows.Clear();
                }


               
            }

            return Json(Delmatsetflow, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult CreateNewDelmatselflowByIDedit(iem_mst_delmat Delmatsetflow)
        {
            int countval;
            iem_mst_delmat Typemodel = new iem_mst_delmat();
            DataTable getvalue = new DataTable();
            DataTable gettitlevalue = new DataTable();
            DataTable getsetflowId = new DataTable();
            string empcode;
            string gradecode;
            string designationcode;
            string departmentcode;
            string titlevalue;
            int slabgid = int.Parse(Session["EditSlabgid"].ToString());

            int setflowId = 0;
            int delmatgid = int.Parse(Session["Update_delmat_gid"].ToString());

            DataTable rowscount = new DataTable();
            rowscount = Delmat.GetrowsOnlyByID(slabgid);

            Delmatsetflow.delmat_gid = delmatgid;
            Session["Flow"] = Delmatsetflow.Flow;
            string result = string.Empty;
            string maxflow = Session["maxflow"].ToString();
            if (Convert.ToInt32(maxflow) >= Delmatsetflow.Flow)
            {
                result = Delmat.checkFlowedit(Delmatsetflow);
                if (result == "Not There" || result == "Duplicate Flow Order !")
                {
                    string flowvl = string.Empty;
                    flowvl = result;
                    if (flowvl != "Duplicate Flow Order !")
                    {
                        // flowvl = Convert.ToString(Convert.ToInt32(flowvl) + 1);
                        if (flowvl != "Please  Enter Correct Flow Order !")
                        {

                            int Sno = Delmatsetflow.Flow;
                            countval = Sno;
                            Delmatsetflow.Countvalue = countval;
                            Session["SNo"] = Delmatsetflow.Countvalue;
                            Session["sno"] = countval + 1;
                            Delmatsetflow.slabrangecount = rowscount.Rows.Count;
                            Delmatsetflow.slabrange_gid = int.Parse(slabgid.ToString());
                            Delmatsetflow.delmat_gid = delmatgid;

                            Session["rowcount"] = rowscount.Rows.Count.ToString();
                            Session["SlabGid"] = Delmatsetflow.slabrange_gid.ToString();
                            Session["delmatgid"] = Delmatsetflow.delmat_gid.ToString();
                            try
                            {
                                int titleflag = int.Parse(Session["Titleflag"].ToString());
                                int valuegid = Delmatsetflow.value_gid;
                                gettitlevalue = Delmat.GetTitleById(Delmatsetflow.title_gid);
                                titlevalue = gettitlevalue.Rows[0]["title_name"].ToString();

                                if (titleflag == 1)
                                {
                                    getvalue = Delmat.GetValueDetailsById(Delmatsetflow.value_gid, titleflag);
                                    empcode = getvalue.Rows[0][0].ToString() + "-" + getvalue.Rows[0][1].ToString();
                                    Delmatsetflow.employee_code = empcode;
                                    Delmatsetflow.TitleNamegid = Delmatsetflow.title_gid;
                                    Delmatsetflow.TitleName = empcode;
                                    Delmatsetflow.title_gid = Delmatsetflow.value_gid;

                                    Delmat.updateDelmatsetflow(Delmatsetflow);

                                    getsetflowId = Delmat.GetSetFlowId();
                                    setflowId = int.Parse(getsetflowId.Rows[0]["Column1"].ToString());
                                    Session["GID"] = setflowId;
                                    Delmatsetflow.GID = setflowId;

                                }
                                else if (titleflag == 2)
                                {
                                    getvalue = Delmat.GetValueDetailsById(Delmatsetflow.value_gid, titleflag);
                                    gradecode =  getvalue.Rows[0][1].ToString();
                                    Delmatsetflow.grade_code = gradecode;
                                    Delmatsetflow.TitleNamegid = Delmatsetflow.title_gid;
                                    Delmatsetflow.TitleName = gradecode;
                                    Delmatsetflow.title_gid = Delmatsetflow.value_gid;

                                    Delmat.updateDelmatsetflow(Delmatsetflow);
                                    getsetflowId = Delmat.GetSetFlowId();
                                    setflowId = int.Parse(getsetflowId.Rows[0]["Column1"].ToString());
                                    Session["GID"] = setflowId;
                                    Delmatsetflow.GID = setflowId;
                                }
                                else if (titleflag == 3)
                                {
                                    getvalue = Delmat.GetValueDetailsById(Delmatsetflow.value_gid, titleflag);
                                    designationcode =  getvalue.Rows[0][1].ToString();
                                    Delmatsetflow.designation_code = designationcode;
                                    Delmatsetflow.TitleNamegid = Delmatsetflow.title_gid;
                                    Delmatsetflow.TitleName = designationcode;
                                    Delmatsetflow.title_gid = Delmatsetflow.value_gid;
                                    Delmat.updateDelmatsetflow(Delmatsetflow);
                                    getsetflowId = Delmat.GetSetFlowId();
                                    setflowId = int.Parse(getsetflowId.Rows[0]["Column1"].ToString());
                                    Session["GID"] = setflowId;
                                    Delmatsetflow.GID = setflowId;
                                }
                                else if (titleflag == 4)
                                {
                                    getvalue = Delmat.GetValueDetailsById(Delmatsetflow.value_gid, titleflag);
                                    departmentcode = getvalue.Rows[0][1].ToString();
                                    Delmatsetflow.role_code = departmentcode;
                                    Delmatsetflow.TitleNamegid = Delmatsetflow.title_gid;
                                    Delmatsetflow.TitleName = departmentcode;
                                    Delmatsetflow.title_gid = Delmatsetflow.value_gid;
                                    Delmat.updateDelmatsetflow(Delmatsetflow);
                                    getsetflowId = Delmat.GetSetFlowId();
                                    setflowId = int.Parse(getsetflowId.Rows[0]["Column1"].ToString());
                                    Session["GID"] = setflowId;
                                    Delmatsetflow.GID = setflowId;

                                }
                                Delmatsetflow.title_name = titlevalue;
                                Delmatsetflow.title_flag = titleflag;


                            }
                            catch (Exception ex)
                            {

                            }
                            finally
                            {
                                getvalue.Rows.Clear();
                                gettitlevalue.Rows.Clear();
                            }


                            return Json(Delmatsetflow, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json("Please  Enter Correct Flow Order !", JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json("Duplicate Flow Order !", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json("Please  Enter Correct Flow Order !", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("Can't Change Current Flow Order To Greater Then Flow Order", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult CreateNewDelmatselflowByID(iem_mst_delmat Delmatsetflow)
        {
            int countval;
            iem_mst_delmat Typemodel = new iem_mst_delmat();
            DataTable getvalue = new DataTable();
            DataTable gettitlevalue = new DataTable();
            DataTable getsetflowId = new DataTable();
            string empcode;
            string gradecode;
            string designationcode;
            string departmentcode;
            string titlevalue;
            int slabgid = int.Parse(Session["EditSlabgid"].ToString());

            int setflowId = 0;
            int delmatgid = int.Parse(Session["Update_delmat_gid"].ToString());

            DataTable rowscount = new DataTable();
            rowscount = Delmat.GetrowsOnlyByID(slabgid);

            Delmatsetflow.delmat_gid = delmatgid;
            Session["Flow"] = Delmatsetflow.Flow;
            string result = string.Empty;
            result = Delmat.checkduplicateFlow(Delmatsetflow);
            if (result == "Not There")
            {
                 string flowvl = string.Empty;
                flowvl = Delmat.checkFlow(Delmatsetflow);
                if (flowvl != "Please  Enter Correct Flow Order !")
                {
                   // flowvl = Convert.ToString(Convert.ToInt32(flowvl) + 1);
                    if (flowvl != "Please  Enter Correct Flow Order !")
                    {
                       
                            int Sno = Delmatsetflow.Flow;
                            countval = Sno;
                            Delmatsetflow.Countvalue = countval;
                            Session["SNo"] = Delmatsetflow.Countvalue;
                            Session["sno"] = countval+1;
                            Delmatsetflow.slabrangecount = rowscount.Rows.Count;
                            Delmatsetflow.slabrange_gid = int.Parse(slabgid.ToString());
                            Delmatsetflow.delmat_gid = delmatgid;

                            Session["rowcount"] = rowscount.Rows.Count.ToString();
                            Session["SlabGid"] = Delmatsetflow.slabrange_gid.ToString();
                            Session["delmatgid"] = Delmatsetflow.delmat_gid.ToString();
                            try
                            {
                                int titleflag = int.Parse(Session["Titleflag"].ToString());
                                int valuegid = Delmatsetflow.value_gid;
                                gettitlevalue = Delmat.GetTitleById(Delmatsetflow.title_gid);
                                titlevalue = gettitlevalue.Rows[0]["title_name"].ToString();

                                if (titleflag == 1)
                                {
                                    getvalue = Delmat.GetValueDetailsById(Delmatsetflow.value_gid, titleflag);
                                    empcode = getvalue.Rows[0][0].ToString() + "-" + getvalue.Rows[0][1].ToString();
                                    Delmatsetflow.employee_code = empcode;
                                    Delmatsetflow.TitleNamegid = Delmatsetflow.title_gid;
                                    Delmatsetflow.TitleName = empcode;
                                    Delmatsetflow.title_gid = Delmatsetflow.value_gid;

                                    Delmat.InsertDelmatsetflow(Delmatsetflow);

                                    getsetflowId = Delmat.GetSetFlowId();
                                    setflowId = int.Parse(getsetflowId.Rows[0]["Column1"].ToString());
                                    Session["GID"] = setflowId;
                                    Delmatsetflow.GID = setflowId;

                                }
                                else if (titleflag == 2)
                                {
                                    getvalue = Delmat.GetValueDetailsById(Delmatsetflow.value_gid, titleflag);
                                    gradecode = getvalue.Rows[0][1].ToString();
                                    Delmatsetflow.grade_code = gradecode;
                                    Delmatsetflow.TitleNamegid = Delmatsetflow.title_gid;
                                    Delmatsetflow.TitleName = gradecode;
                                    Delmatsetflow.title_gid = Delmatsetflow.value_gid;

                                    Delmat.InsertDelmatsetflow(Delmatsetflow);
                                    getsetflowId = Delmat.GetSetFlowId();
                                    setflowId = int.Parse(getsetflowId.Rows[0]["Column1"].ToString());
                                    Session["GID"] = setflowId;
                                    Delmatsetflow.GID = setflowId;
                                }
                                else if (titleflag == 3)
                                {
                                    getvalue = Delmat.GetValueDetailsById(Delmatsetflow.value_gid, titleflag);
                                    designationcode = getvalue.Rows[0][1].ToString();
                                    Delmatsetflow.designation_code = designationcode;
                                    Delmatsetflow.TitleNamegid = Delmatsetflow.title_gid;
                                    Delmatsetflow.TitleName = designationcode;
                                    Delmatsetflow.title_gid = Delmatsetflow.value_gid;
                                    Delmat.InsertDelmatsetflow(Delmatsetflow);
                                    getsetflowId = Delmat.GetSetFlowId();
                                    setflowId = int.Parse(getsetflowId.Rows[0]["Column1"].ToString());
                                    Session["GID"] = setflowId;
                                    Delmatsetflow.GID = setflowId;
                                }
                                else if (titleflag == 4)
                                {
                                    getvalue = Delmat.GetValueDetailsById(Delmatsetflow.value_gid, titleflag);
                                    departmentcode =  getvalue.Rows[0][1].ToString();
                                    Delmatsetflow.role_code = departmentcode;
                                    Delmatsetflow.TitleNamegid = Delmatsetflow.title_gid;
                                    Delmatsetflow.TitleName = departmentcode;
                                    Delmatsetflow.title_gid = Delmatsetflow.value_gid;
                                    Delmat.InsertDelmatsetflow(Delmatsetflow);
                                    getsetflowId = Delmat.GetSetFlowId();
                                    setflowId = int.Parse(getsetflowId.Rows[0]["Column1"].ToString());
                                    Session["GID"] = setflowId;
                                    Delmatsetflow.GID = setflowId;

                                }
                                Delmatsetflow.title_name = titlevalue;
                                Delmatsetflow.title_flag = titleflag;


                            }
                            catch (Exception ex)
                            {

                            }
                            finally
                            {
                                getvalue.Rows.Clear();
                                gettitlevalue.Rows.Clear();
                            }
                          
                        
                        return Json(Delmatsetflow, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("Please  Enter Correct Flow Order !", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json("Please  Enter Correct Flow Order !", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("Duplicate Flow Order !", JsonRequestBehavior.AllowGet);         
            }

        }


        [HttpPost]
        public JsonResult InsertDelmatFlow(iem_mst_delmat CreateDelmatFlow, string[] lsselectedSlabrange)
        {

            iem_mst_delmat typemodel = new iem_mst_delmat();
            DataTable gettileinformation = new DataTable();
            DataTable getID = new DataTable();
            DataTable gettitle = new DataTable();
            DataTable Getslabrange = new DataTable();
            string result = string.Empty;
            string slabrangename = string.Empty;
            Getslabrange = Delmat.GetslabrangeGid(CreateDelmatFlow.slabrange_gid);

            try
            {
                for (int c = 0; c < Getslabrange.Rows.Count; c++)
                {

                    CreateDelmatFlow.Slabrangegid = int.Parse(Getslabrange.Rows[c]["slabrange_gid"].ToString());
                    if (CreateDelmatFlow.AddApporoval == "True") { CreateDelmatFlow.AddApporoval = "Y"; }

                    else { CreateDelmatFlow.AddApporoval = "N"; }

                    if (lsselectedSlabrange[c].ToString() == "True")
                    { CreateDelmatFlow.slabrange_name = "Y"; }
                    else { CreateDelmatFlow.slabrange_name = "N"; }


                    if (CreateDelmatFlow.Title.Trim() == "Employee")
                    {
                        getID = Delmat.GettitleGidByName(CreateDelmatFlow.Title.Trim(), 1);
                        string[] tilenm = CreateDelmatFlow.TitleName.Split('-');
                        gettileinformation = Delmat.GettitlevalueByName(tilenm[0].ToString(), 1);
                        CreateDelmatFlow.title_gid = int.Parse(gettileinformation.Rows[0]["employee_gid"].ToString());
                        CreateDelmatFlow.TitleNamegid = int.Parse(getID.Rows[0]["title_gid"].ToString());
                        // GID=int.Parse(Session["GID"].ToString ());
                    }
                    if (CreateDelmatFlow.Title.Trim() == "Grade")
                    {
                        getID = Delmat.GettitleGidByName(CreateDelmatFlow.Title.Trim(), 2);
                        string[] tilenm = CreateDelmatFlow.TitleName.Split('-');
                        gettileinformation = Delmat.GettitlevalueByName(tilenm[0].ToString(), 2);
                        CreateDelmatFlow.title_gid = int.Parse(gettileinformation.Rows[0]["grade_gid"].ToString());
                        CreateDelmatFlow.TitleNamegid = int.Parse(getID.Rows[0]["title_gid"].ToString());
                    }
                    if (CreateDelmatFlow.Title.Trim() == "Designation")
                    {
                        getID = Delmat.GettitleGidByName(CreateDelmatFlow.Title.Trim(), 3);
                        string[] tilenm = CreateDelmatFlow.TitleName.Split('-');
                        gettileinformation = Delmat.GettitlevalueByName(tilenm[0].ToString(), 3);
                        CreateDelmatFlow.title_gid = int.Parse(gettileinformation.Rows[0]["designation_gid"].ToString());
                        CreateDelmatFlow.TitleNamegid = int.Parse(getID.Rows[0]["title_gid"].ToString());
                    }
                    if (CreateDelmatFlow.Title.Trim() == "Role")
                    {
                        getID = Delmat.GettitleGidByName(CreateDelmatFlow.Title.Trim(), 4);
                        string[] tilenm = CreateDelmatFlow.TitleName.Split('-');
                        gettileinformation = Delmat.GettitlevalueByName(tilenm[0].ToString(), 4);
                        CreateDelmatFlow.title_gid = int.Parse(gettileinformation.Rows[0]["role_gid"].ToString());
                        CreateDelmatFlow.TitleNamegid = int.Parse(getID.Rows[0]["title_gid"].ToString());
                    }
                    result = Delmat.InsertDelmatMatrix(CreateDelmatFlow);
                    
                 
                   // Session["delmatgid"] = "";
                }
                CreateDelmatFlow.delmat_gid = Convert.ToInt32(CreateDelmatFlow.delmat_gid.ToString());
                //Delmat.UPdateDelMateStatus(CreateDelmatFlow.delmat_gid.ToString());
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
            return Json(CreateDelmatFlow);
        }


        [HttpPost]
        public JsonResult UpdateDelmatFlow(iem_mst_delmat CreateDelmatFlow, string[] lsselectedSlabrange)
        {

            iem_mst_delmat typemodel = new iem_mst_delmat();
            DataTable getdelmamatrixgid = new DataTable();
            DataTable Getslabrange = new DataTable();
            
            Getslabrange = Delmat.GetslabrangeGid(int.Parse(Session["EditSlabgid"].ToString()));

            for (int src = 0; src < Getslabrange.Rows.Count; src++)
            {
                if (typemodel.lsselectedSlabrange != null)
                {

                    try
                    {
                        CreateDelmatFlow.delmat_gid = int.Parse(Session["Update_delmat_gid"].ToString());
                        CreateDelmatFlow.delmatsetflowgid = CreateDelmatFlow.GID;
                        CreateDelmatFlow.slabrange_gid = int.Parse(Getslabrange.Rows[src]["slabrange_gid"].ToString());
                        if (lsselectedSlabrange[src].ToString() == "True")
                        {
                            CreateDelmatFlow.slabrange_name = "Y";
                        }
                        else
                        {
                            CreateDelmatFlow.slabrange_name = "N";
                        }
                        Delmat.UpdateDelmatMatrix(CreateDelmatFlow);

                        CreateDelmatFlow.delmat_gid = Convert.ToInt32(Session["Update_delmat_gid"].ToString());

                    }
                    catch (Exception ex)
                    {

                    }
                    finally
                    {

                    }

                }
            }
            
            //Delmat.UPdateDelMateStatus(Session["delmatgid"].ToString());
            
             return Json(CreateDelmatFlow);
             
        }
        [HttpPost]
        public JsonResult UpdateDelmatAuditTrial(string delmat_gid)
        {
            try
            {
                Delmat.UpdateDelmateStatus(delmat_gid.ToString());
            }
            catch(Exception ex)
            {

            }
            return Json(delmat_gid.ToString());
        }
        [HttpGet]
        public PartialViewResult ViewSlab(int id)
        {
            Session["slabname"] = "";
            Session["SlabGid"] = id;
            return PartialView();
        }


        [HttpPost]
        public JsonResult CheckVisible(iem_mst_delmat DelmatVisible)
        {
            iem_mst_delmat TypeModel = new iem_mst_delmat();
            if (ModelState.IsValid)
            {

                DelmatVisisbleById VisibleId = Delmat.GetdelmatvisibleById(DelmatVisible.delmattype_gid);
                DelmatVisible.delmattype_branch_visible = Convert.ToString(VisibleId.delmattype_branch_visible);
                DelmatVisible.delmattype_branchtype_visible = Convert.ToString(VisibleId.delmattype_branchtype_visible);
                DelmatVisible.delmattype_claim_visible = Convert.ToString(VisibleId.delmattype_claim_visible);
                DelmatVisible.delmattype_dsa_visible = Convert.ToString(VisibleId.delmattype_dsa_visible);
                DelmatVisible.delmattype_pipit_visible = Convert.ToString(VisibleId.delmattype_pipit_visible);
                DelmatVisible.delmattype_it_visible = Convert.ToString(VisibleId.delmattype_it_visible);
                DelmatVisible.delmattype_exp_visible = Convert.ToString(VisibleId.delmattype_exp_visible);
                DelmatVisible.delmattype_budget_visible = Convert.ToString(VisibleId.delmattype_budget_visible);
                DelmatVisible.delmattype_jump_visible = Convert.ToString(VisibleId.delmattype_jump_visible);
                //delmat change
                DelmatVisible.delmattype_ecftype_visible = Convert.ToString(VisibleId.delmattype_ecftype_visible);
                DelmatVisible.delmattype_parpr_visible = Convert.ToString(VisibleId.delmattype_parpr_visible);
                DelmatVisible.delmattype_withwithoutparpr_visible = Convert.ToString(VisibleId.delmattype_withwithoutparpr_visible);

            }

            return Json(DelmatVisible, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Updatedelmat(iem_mst_delmat UpdateDelmat, string[] lstSelecteddepartmentGid)
        {
            string result = string.Empty;
            try
            {
                string lstDelmat = string.Empty;
                DataTable delmatgid = new DataTable();
                DataTable getslabname = new DataTable();
                string slabgid = Session["EditSlabgid"].ToString();

                if (ModelState.IsValid)
                {
                    UpdateDelmat.delmat_gid = int.Parse(Session["Update_delmat_gid"].ToString());
                    //UpdateDelmat.delmat_gid = int.Parse(Session["delmatgid"].ToString());
                    UpdateDelmat.delmat_update_by = Convert.ToInt32(objCmnFunctions.GetLoginUserGid().ToString());
                    result = Delmat.UpdateDelmat(UpdateDelmat, lstSelecteddepartmentGid);
                    if (result == "success")
                    {
                        lstDelmat = Delmat.AddDelmatdepartment(UpdateDelmat, lstSelecteddepartmentGid);
                        getslabname = Delmat.Getslabname(int.Parse(slabgid));
                        Session["slabname"] = getslabname.Rows[0]["slab_name"].ToString();
                        Session["SlabGid"] = slabgid.ToString();
                        Session["SNo"] = int.Parse(count.ToString());
                        if (Session["function"].ToString() == "Edit")
                        {
                            ViewBag.viewfor = "";
                        }
                        else
                        {
                            ViewBag.viewfor = "disabled";
                        }
                        ViewBag.process = "Edit";
                    }
                    else { return Json(result, JsonRequestBehavior.AllowGet); }

                }
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult EditDelmatsetflow(iem_mst_delmat EditDelmat)
        {

            return PartialView(EditDelmat);
        }

        [HttpPost]
        public JsonResult DeleteDelmatsetflow(iem_mst_delmat DeleteDelmat)
        {
            int sno = Convert.ToInt32(Session["sno"].ToString());
            sno--;
            Session["sno"] = sno;
            Delmat.DeleteDelmat(DeleteDelmat.delmatmatrixgid, DeleteDelmat.delmatsetflowgid);
            return Json("Record deleted successfully !", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteDelmatException(iem_mst_delmat DeleteDelmat)
        {
            try
            {
                Delmat.DeleteDelmatException(DeleteDelmat.delmatexception_gid);

            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
            return Json("Record deleted successfully !", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteDelmat(iem_mst_delmat DeleteDelmat)
        {
            string result = string.Empty;
            try
            {
                result = Delmat.DeleteDelmatInformation(DeleteDelmat);

            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
            return Json("Record deleted successfully !", JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult DeleteDelmatExceptionById(iem_mst_delmat DeleteDelmat)
        {
            try
            {
                Delmat.DeleteDelmatException(DeleteDelmat.delmatexception_gid);

            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
            return Json("Record deleted successfully !", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public PartialViewResult Edit(int id, string viewfor)
        {
            if (viewfor == "edit")
            {
                ViewBag.viewfor = "edit";
            }
            else if (viewfor == "view")
            {
                ViewBag.viewfor = "view";
            }
            else if (viewfor == "Delete")
            {
                ViewBag.viewfor = "Delete";
            }
            iem_mst_delmat TypeModel = Delmat.GetdelmatExceptionById(id);
           // TypeModel.GetEmployee = new SelectList(Delmat.GetEmployee(), "employee_gid", "employee_code");
            TypeModel.employee_gid = TypeModel.delmatexception_to;
            TypeModel.GetEmployee = new SelectList(Delmat.GetEmployee(), "employee_gid", "employee_code", TypeModel.employee_gid);
            return PartialView(TypeModel);
        }

        [HttpGet]
        public PartialViewResult EditById(int id, string viewfor)
        {
            if (viewfor == "edit")
            {
                ViewBag.viewfor = "edit";
            }
            else if (viewfor == "view")
            {
                ViewBag.viewfor = "view";
            }
            else if (viewfor == "Delete")
            {
                ViewBag.viewfor = "Delete";
            }
            iem_mst_delmat TypeModel = Delmat.GetdelmatExceptionById(id);
            TypeModel.employee_gid = TypeModel.delmatexception_to;
            TypeModel.GetEmployee = new SelectList(Delmat.GetEmployee(), "employee_gid", "employee_code", TypeModel.employee_gid);
            return PartialView(TypeModel);
        }

        [HttpPost]
        public JsonResult UpdateDelmatException(iem_mst_delmat UpdateException)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    Delmat.UpdateDelmatException(UpdateException);

                }

                return Json("Updated Suceesfully", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
        }
        [HttpPost]
        public JsonResult Getdelmatcount(iem_mst_delmat Matrixaccess)
        {
            string result = "";
            if (Convert.ToString(Session["Update_delmat_gid"]) != "")
            {
                result = Delmat.delcount(int.Parse(Session["Update_delmat_gid"].ToString()));
            }
            else
            {
                result = Delmat.delcount(int.Parse(Session["delmatgid"].ToString()));
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetdelmatMatrix(iem_mst_delmat Matrixaccess)
        {
            List<iem_mst_delmat> Matrix = new List<iem_mst_delmat>();
            DataTable delmatrix = new DataTable();
            DataTable SelectDelmatGidCount = new DataTable();
            int n=Matrixaccess.s_no;
            n++;
            Session["sno"] =n ;
            if (Convert.ToString(Session["Update_delmat_gid"]) != "")
            {
                SelectDelmatGidCount = Delmat.SelectAddApproval(int.Parse(Session["Update_delmat_gid"].ToString()), Matrixaccess.delmatsetflowgid);
            }
            else
            {
                SelectDelmatGidCount = Delmat.SelectAddApproval(int.Parse(Session["delmatgid"].ToString()), Matrixaccess.delmatsetflowgid);
            }
            foreach (DataRow dr in SelectDelmatGidCount.Rows)
            {
                Matrix.Add(
                new iem_mst_delmat
                {
                    lsMatrixAccess = dr["delmatmatrix_access"].ToString(),

                });
            };
            ViewBag.IsDelmatRole = Delmat.IsDelmatMaker();
            var delrole = ViewBag.IsDelmatRole;
            Session["RoleDelmat"] = delrole;
            return Json(Matrix, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Getdelmatdata(iem_mst_delmat Delmatdata)
        {
            List<iem_mst_delmat> lst = new List<iem_mst_delmat>();
            string matrixaccess = string.Empty;
            DataTable SelectDelmatGidCount = new DataTable();
            DataTable GetflowgidCount = new DataTable();
            DataTable getslabrangename = new DataTable();
            //int i = -1;
            DataTable rowscount = new DataTable();
            rowscount = Delmat.GetrowsOnlyByID(int.Parse(Session["EditSlabgid"].ToString()));
            if (Convert.ToString(Session["Update_delmat_gid"]) != "")
            {
                SelectDelmatGidCount = Delmat.SelectDelmatGidCount(int.Parse(Session["Update_delmat_gid"].ToString()));
                GetflowgidCount = Delmat.GetflowgidCount(int.Parse(Session["Update_delmat_gid"].ToString()));
            }
            else
            {
                SelectDelmatGidCount = Delmat.SelectDelmatGidCount(int.Parse(Session["delmatgid"].ToString()));
                GetflowgidCount = Delmat.GetflowgidCount(int.Parse(Session["delmatgid"].ToString()));
            }

            DataColumn column = new DataColumn("MatrixAccess");
            column.DataType = System.Type.GetType("System.String");
            GetflowgidCount.Columns.Add(column);

            foreach (DataRow dr in GetflowgidCount.Rows)
            {

                lst.Add(
                new iem_mst_delmat
                {
                    delmat_gid = int.Parse(dr["delmatflow_delmat_gid"].ToString()),
                    Flowvalue = int.Parse(dr["delmatflow_gid"].ToString()),
                    InFlowCount = int.Parse(dr["delmatflow_order"].ToString()),
                    lsdelmatflowtitlevalue = dr["delmatflow_title_value"].ToString(),
                    lsdelmatflowaddapprovalvalue = dr["delmatflow_additional_approval"].ToString(),
                    lsTitlename = dr["title_name"].ToString(),
                    InRowscount = rowscount.Rows.Count,

                });

            };
            ViewBag.count = rowscount.Rows.Count;
            //}           

            ViewBag.IsDelmatRole = Delmat.IsDelmatMaker();
            var delrole = ViewBag.IsDelmatRole;
            Session["RoleDelmat"] = delrole;
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        //downloading Excel File for delmat
        [HttpPost]
        public JsonResult DownloadExcel(string ViewType, string FileName)
        {
            DataSet ds = db.GetCommonExcelDownload(ViewType, plib.LoginUserId);
            DataTable _downloadingData = ds.Tables[0];
            if (_downloadingData != null)
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(_downloadingData);
                    wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wb.Style.Font.Bold = true;

                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AddHeader("content-disposition", "attachment;filename= " + FileName + ".xls");

                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        //Attachment Delmat 05-01-2022
        [HttpGet]
        public PartialViewResult DelmatAttachment()
        {

            // AttachFile objMAttachment = new AttachFile();
            // objMAttachment.AttachmentTypeData = new SelectList(objModelTravel.AttachmentTypedata().ToList(), "AttachmentTypeId", "AttachmentTypeName");
            return PartialView();
        }

        //Delmat Attachment Details
        [HttpGet]
        public PartialViewResult DelmatAttachmentDetails()
        {
            // List<AttachFile> lstAttachment = new List<AttachFile>();
            List<AttachFile> lstAttachment = new List<AttachFile>();
            ViewBag.IsDelmatRole = Delmat.IsDelmatMaker();
            lstAttachment = Delmat.GetDelmatAttachment(Session["delmatgid"].ToString()).ToList();
            return PartialView(lstAttachment);
        }

        [HttpGet]
        public PartialViewResult DelmatAttachmentCreate()
        {
            AttachFile objMAttachment = new AttachFile();
            //objMAttachment.AttachmentTypeData = new SelectList(objModelTravel.AttachmentTypedata().ToList(), "AttachmentTypeId", "AttachmentTypeName");
            return PartialView();
        }

        [HttpPost]
        public JsonResult _AttachmentCreate(AttachFile EmployeeeExpenseModel)
        {
            string img = "No";
            try
            {
                if (TempData["_attFile"] != null)
                {
                    HttpPostedFileBase savefile = TempData["_attFile"] as HttpPostedFileBase;
                    string getname = Delmat.InsertEmpAtt(savefile, EmployeeeExpenseModel, Session["delmatgid"].ToString(), objCmnFunctions.GetLoginUserGid().ToString());
                    if (getname != "")
                    {


                        //upload the file to server.
                        HttpPostedFileBase _attFile = TempData["_attFile"] as HttpPostedFileBase;
                        var stream = _attFile.InputStream;
                        string uploaderUrl = string.Format("{0}{1}", Delmat.HoldFileUploadUrl(), getname + "." + _attFile.FileName.Split('.')[_attFile.FileName.Split('.').Length - 1]);
                        //using (var fileStream = System.IO.File.Create(uploaderUrl))
                        //{
                        //    stream.CopyTo(fileStream);
                        //}

                        //Muthu 2016-10-19
                        byte[] bytFile = null;
                        using (var memoryStream = new MemoryStream())
                        {
                            stream.Position = 0;
                            stream.CopyTo(memoryStream);
                            bytFile = memoryStream.ToArray();
                            ViewBag.Result = "inside File Stream";
                        }
                        var FileString = Convert.ToBase64String(bytFile);
                        var filename = getname + "." + _attFile.FileName.Split('.')[_attFile.FileName.Split('.').Length - 1];
                        var result = Cmnfs.SaveFileToServer(FileString, filename).Result;
                        //remove the temp data content.
                        TempData.Remove("_attFile");

                        img = "Yes";
                    }
                }
                else
                {
                    TempData.Remove("_attFile");
                    img = "Invalid File Format!";
                }
                ViewBag.SearchItems = null;
                return Json(img, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return Json("Error occurred..." + ex.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        //upload Files
        [HttpPost]
        public void UploadFiles(string attach = null, string attaching_format = null)
        {
            try
            {

                TempData["_attFile"] = null;
                // objErrorLog.WriteErrorLog("before Request.Files", "726");
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        string fileextension = Path.GetExtension(Path.GetFileName(fileContent.FileName));
                        string[] attaching_fileformat = attaching_format.Split(',');
                        if (attaching_fileformat.Contains(fileextension.ToLower()))
                        {
                            var stream = fileContent.InputStream;
                            byte[] bytFile = null;
                            using (var memoryStream = new MemoryStream())
                            {
                                stream.Position = 0;
                                stream.CopyTo(memoryStream);
                                bytFile = memoryStream.ToArray();
                            }
                            // objErrorLog.WriteErrorLog("before getmimetype", "744");
                            bool isEXE = CmnFunctions.GetMimeTypeFromAttachment(bytFile, attach, fileextension.ToLower());
                            if (isEXE == false)
                            {
                                // objErrorLog.WriteErrorLog("inside isexe", "748");
                                HttpPostedFileBase hpfBase = Request.Files[file] as HttpPostedFileBase;
                                TempData["_attFile"] = hpfBase;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
            }
        }

        //download delmat attachment
        public FileResult Download(int id)
        {
            string FileName = Delmat.downloadAttachment(id, Session["delmatgid"].ToString());

            string[] str = FileName.Split('.');
            string directory = id.ToString() + "." + str[str.Length - 1].ToString();

            var apiresult = Cmnfs.DownloadFile(directory).Result;
            if (apiresult == "Fail")
            {
                return File("", "");
            }
            else
            {
                byte[] filebyte = Convert.FromBase64String(apiresult);
                return File(filebyte, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
            }
            //return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
        }

        //Edit Attachment
        [HttpGet]
        public PartialViewResult EditDelmatAttachmentId()
        {

            AttachFile TypeModel = new AttachFile();
            return PartialView();
        }
        //Delete Attachment delmat
        [HttpPost]
        public JsonResult AttachmentDelete(AttachFile EmployeeeExpensemodel)
        {
            int AttachmentGID = (int)EmployeeeExpensemodel.AttachmentFilenameId;
            string delamt = Delmat.DeleteDelmatAttachment(AttachmentGID, Session["delmatgid"].ToString());
            return Json(EmployeeeExpensemodel, JsonRequestBehavior.AllowGet);
        }


        //Checker

        public JsonResult ApproverActionDelmat(iem_mst_delmat AppModel)
        {
            string Err = "";
            string delegates = "";
            try
            {
                if (ModelState.IsValid)
                {
                    Err = Delmat.Insertapprovedaction(AppModel, Session["delmatgid"].ToString(), objCmnFunctions.GetLoginUserGid().ToString());
                }
                ViewBag.SearchItems = null;

                return Json(Err, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult Validatedelmat(iem_mst_delmat UpdateDelmat, string[] lstSelecteddepartmentGid)
        {
            string result = string.Empty;
            try
            {
                string lstDelmat = string.Empty;
                DataTable delmatgid = new DataTable();
                DataTable getslabname = new DataTable();
                string slabgid = Session["EditSlabgid"].ToString();

                if (ModelState.IsValid)
                {
                    UpdateDelmat.delmat_gid = int.Parse(Session["Update_delmat_gid"].ToString());
                    //UpdateDelmat.delmat_gid = int.Parse(Session["delmatgid"].ToString());
                    UpdateDelmat.delmat_update_by = Convert.ToInt32(objCmnFunctions.GetLoginUserGid().ToString());
                    
                    result = Delmat.ValidateDelmat(UpdateDelmat, lstSelecteddepartmentGid);
                    //result = "success";

                    /*if (result == "success")
                    {
                        lstDelmat = Delmat.AddDelmatdepartment(UpdateDelmat, lstSelecteddepartmentGid);
                        getslabname = Delmat.Getslabname(int.Parse(slabgid));
                        Session["slabname"] = getslabname.Rows[0]["slab_name"].ToString();
                        Session["SlabGid"] = slabgid.ToString();
                        Session["SNo"] = int.Parse(count.ToString());
                        if (Session["function"].ToString() == "Edit")
                        {
                            ViewBag.viewfor = "";
                        }
                        else
                        {
                            ViewBag.viewfor = "disabled";
                        }
                        ViewBag.process = "Edit";
                    }
                    else { return Json(result, JsonRequestBehavior.AllowGet); }*/
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
