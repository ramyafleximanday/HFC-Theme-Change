using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using IEM.Models;
using IEM.Common;
using IEM.Areas.FLEXIBUY.Models;
using IEM.Areas.MASTERS.Controllers;
using Newtonsoft.Json;
using IEM.Helper;
using IEM.Areas.EOW.Models;
namespace IEM.Controllers
{
    public class MailApproveController : Controller
    {
        MailApproveRepository rep;
        proLib plib = new proLib();
        dbLib db = new dbLib();
        private PRNewRepository objModel;
        private WORepository objWOModel;
        private PARRepository objPARModel;
        private IRepositoryKIR objprModel;
        private EOW_IRepository objeowModel;
        CmnFunctions objCmnFunctions = new CmnFunctions();
        ErrorLog objErrorLog = new ErrorLog();
        DataTable dt = new DataTable();
        public MailApproveController()
            : this(new MailApproveDataModel(), new PRNewDataModel(), new prsummodel(), new WODataModel(), new PARDataModel(), new EOW_DataModel())
        {

        }

        public MailApproveController(MailApproveRepository Irep, PRNewRepository objM, IRepositoryKIR objpr, WORepository objW, PARRepository objPAR, EOW_IRepository objeowM)
        {
            rep = Irep;
            objModel = objM;
            objprModel = objpr;
            objWOModel = objW;
            objPARModel = objPAR;
            objeowModel = objeowM;
        }

        //
        // GET: /MailApprove/

        public ActionResult Index(string module = null, string usercode = null, string queuegid = null, string lnkfor = null, string nextapprover = null)
        {
            try
            {
                string Newmodule = Decode(module).ToString().Trim();
                //string Newmodule = "EOW";
                if (Newmodule == "EOW")
                {
                    string approvalmail = "";
                    string ecfgids = "";
                    string empgids = "";
                    string IsConcurrentApproval = "";
                    string ecftype = ""; 
                    string Newusercode = Decode(usercode).ToString().Trim();
                    string Newqueuegid = Decode(queuegid).ToString().Trim();

                    //Newusercode = "175340";
                    //string Newqueuegid = "32639";

                    string alreadyapproved = rep.alreadyapproveby(Newqueuegid);
                    if (alreadyapproved == "")
                    {
                        MailApprovaleowModel objMExpenseEdit = new MailApprovaleowModel();
                        List<MailApprovaleowModel> objobjMExpense = new List<MailApprovaleowModel>();
                        objobjMExpense = rep.Getecfqueuedetails(Newusercode, Newqueuegid).ToList();

                        ecfgids = objobjMExpense[0]._EcfID.ToString();
                        empgids = objobjMExpense[0]._ApproveID.ToString();
                        //ecftype = objobjMExpense[0]._ECFtype.ToString(); // rmaya added on 26 oct 22
                        //ECFAmount = objobjMExpense[0]._ECFAmount.ToString();

                        IsConcurrentApproval = rep.GetEowConcurrentApproval(ecfgids, Newqueuegid);
                        Approveraction AppModel = new Approveraction();

                        if (IsConcurrentApproval=="C")
                        {
                            approvalmail = rep.InsertConcurrentApproval(ecfgids, empgids, Newqueuegid, lnkfor);
                        }
                        else
                        {
                            ecftype = rep.GetDocSubtypebyID(ecfgids);
                            AppModel.Concurrentmsg = "Ok";
                            AppModel.Rejecthold = "OK";
                            if (lnkfor == "1")
                            {
                                 AppModel.status = "Approve";
                            }
                            else{
                                AppModel.status = "Reject";
                            }
                            approvalmail = objeowModel.Insertapprovedaction(AppModel, ecfgids, "0", empgids, ecftype, Newqueuegid, "0", "", "");
                             
                        }
                       
                        ViewBag.ApproveErrMsg = approvalmail;
                    }
                    else
                    {
                        ViewBag.ApproveErrMsg = alreadyapproved;
                    }
                }
                else if (Newmodule == "FB")
                {
                    string approvalmail = "";
                    string reqgid = "";
                    string empgids = "";
                    string refname = "";
                    string refstatus = "";
                    string IsRejectFlag = "0";
                    string Remarks = "";
                    string Newusercode = Decode(usercode).ToString().Trim();
                    string Newqueuegid = Decode(queuegid).ToString().Trim();

                    string alreadyapproved = rep.alreadyapproveby(Newqueuegid);
                    if (alreadyapproved == "")
                    {
                        MailApprovalfbModel objfbEdit = new MailApprovalfbModel();
                        List<MailApprovalfbModel> objfbEditList = new List<MailApprovalfbModel>();
                        objfbEditList = rep.Getfbqueuedetails(Newusercode, Newqueuegid).ToList();
                        reqgid = objfbEditList[0]._RequestID.ToString();
                        empgids = objfbEditList[0]._ApproveID.ToString();
                        refname = objfbEditList[0]._RefName.ToString();
                        refstatus = objfbEditList[0]._RefStatus.ToString();

                        // approvalmail = rep.Insertapprovalmail(reqgid, empgids, Newqueuegid, lnkfor);

                        if (refname == "PR")
                        {


                            IsRejectFlag = "0";
                            Remarks = "PR approved from Mail";
                            if (lnkfor == "0")
                            {
                                IsRejectFlag = "1";
                                Remarks = "PR Rejected From Mail";
                            }
                            PRNewEntity PRParentDetailsInsert=new PRNewEntity();
                            PRParentDetailsInsert.PRGid = Convert.ToInt32(reqgid);
                            PRParentDetailsInsert.PRApprovedBy = Convert.ToString(refstatus);
                            PRParentDetailsInsert.PRApprovalRemarks = Remarks;
                            approvalmail = PRApprovalRequestMail(PRParentDetailsInsert, IsRejectFlag);
                            ViewBag.ApproveErrMsg = approvalmail; 
                        }
                        else if (refname == "CBF")
                        {
                            IsRejectFlag = "0";
                            Remarks = "CBF approved from Mail";
                            if (lnkfor == "0")
                            {
                                IsRejectFlag = "1";
                                Remarks = "CBF Rejected From Mail";
                            }

                            ViewBag.ApproveErrMsg = CBFApprovalRequestMail(reqgid, refstatus, IsRejectFlag, Remarks);
                        }

                        else if (refname == "PO")
                        {

                            //  string POHeaderGId, POEndDate, ProjectOwnerGId, VendorGId, Type, VendorNote, TemplateGId, AddedTermAndCondtion, IsRemoved,ViewType,VendorcontactId;

                            IsRejectFlag = "0";
                            Remarks = "PO approved from Mail";
                            if (lnkfor == "0")
                            {
                                IsRejectFlag = "1";
                                Remarks = "PO Rejected From Mail";
                            }

                            ViewBag.ApproveErrMsg = POApprovalRequestMail(reqgid, "", "", "", "", "", "", "", "", refstatus, IsRejectFlag, Remarks, "");
                        }
                        else if (refname == "WO")
                        {

                            IsRejectFlag = "0";
                            Remarks = "WO approved from Mail";
                            if (lnkfor == "0")
                            {
                                IsRejectFlag = "1";
                                Remarks = "WO Rejected From Mail";
                            }

                            WOEntity ObjWOEntity = new WOEntity();
                            ObjWOEntity.WOGid = Convert.ToInt32(reqgid);
                            ObjWOEntity.WOApprovedBy = Convert.ToString(refstatus);
                            ObjWOEntity.WOApprovalRemarks = Remarks;

                            approvalmail = WOApprovalRequestMail(ObjWOEntity, IsRejectFlag);
                            ViewBag.ApproveErrMsg = approvalmail;

                        }

                        else if (refname == "PAR")
                        {

                            IsRejectFlag = "0";
                            Remarks = "PAR approved from Mail";
                            if (lnkfor == "0")
                            {
                                IsRejectFlag = "1";
                                Remarks = "PAR Rejected From Mail";
                            }
                            PAREntity PARParentDetailsInsert = new PAREntity();
                            PARParentDetailsInsert.PARGid = Convert.ToInt32(reqgid);
                            PARParentDetailsInsert.PARApprovedBy = Convert.ToString(refstatus);
                            PARParentDetailsInsert.PARDescription = Remarks;
                            approvalmail = PARApprovalRequestMail(PARParentDetailsInsert, IsRejectFlag);
                            ViewBag.ApproveErrMsg = approvalmail;
                        }
                    }
                    else
                    {
                        ViewBag.ApproveErrMsg = alreadyapproved;
                    }
                }

                else
                {
                    string EmpCode = Decode(usercode).ToString().Trim();
                    queuegid = Decode(queuegid).ToString().Trim();
                    int actionstatus = Convert.ToInt32(lnkfor);
                    string skipflag = "0";
                    string remarks = "";
                    string queueto = "0";
                    if (nextapprover != null)
                    {
                        if (nextapprover == "0")
                        {
                            skipflag = "1";
                        }
                        else
                        {
                            queueto = nextapprover;
                        }
                    }
                    DataTable dt = new DataTable();
                    string ReturnAction = string.Empty;
                    IEMDataModel dm = new IEMDataModel();
                    dt = dm.GetLoginUserDetails(EmpCode.ToUpper().Trim());
                    int empgid = Convert.ToInt32(dt.Rows[0]["employee_gid"].ToString());
                    Session["EmployeeGidApprove"] = empgid;
                    //------To Check Queue Is Approved By Others & Get All Details----------//
                    DataTable dtQueue = new DataTable();
                    dtQueue = rep.IsQueueClosed(Convert.ToInt32(queuegid));
                    if (dtQueue.Rows.Count > 0)
                    {
                        var queue_isremoved = dtQueue.Rows[0]["queue_isremoved"].ToString();
                        if (queue_isremoved == "Y")
                        {
                            var queue_action_by = dtQueue.Rows[0]["queue_action_by"].ToString();
                            var queue_action_date = dtQueue.Rows[0]["queue_action_date"].ToString();
                            var queue_action_remark = dtQueue.Rows[0]["queue_action_remark"].ToString();
                            string CurApprovalStage = rep.GetCurrentApprovalStage(dtQueue.Rows[0]["supplierheader_requesttype"].ToString(), queuegid);
                            ViewBag.ApproveErrMsg = CurApprovalStage + " Approval already Done by " + queue_action_by + " on " + queue_action_date;
                        }
                        else
                        {
                            var suppliercode = dtQueue.Rows[0]["supplierheader_suppliercode"].ToString();
                            Session["SupplierCodeApprove"] = suppliercode;
                            string isSupplierLocked = rep.CheckSupplierIsLocked(suppliercode, empgid);
                            if (isSupplierLocked != "approve")
                            {
                                ViewBag.ApproveErrMsg = isSupplierLocked;
                            }
                            else
                            {
                                int suppliergid = Convert.ToInt32(dtQueue.Rows[0]["queue_ref_gid"].ToString());
                                Session["SupplierGidApprove"] = suppliergid;
                                string isExistingApprover = rep.IsExistingApprover(suppliergid, empgid).ToUpper().Trim();
                                if (isExistingApprover != "N")
                                {
                                    ViewBag.ApproveErrMsg = "This Supplier was already Approved By " + isExistingApprover;
                                }
                                else
                                {
                                    var curapprovalstage = dtQueue.Rows[0]["supplierheader_currentapprovalstage"].ToString();
                                    var reqtype = dtQueue.Rows[0]["supplierheader_requesttype"].ToString();
                                    MailApprovalModel objApprovalModel = new MailApprovalModel();
                                    objApprovalModel._QueueTo = Convert.ToString(queueto);
                                    objApprovalModel._RequestType = reqtype.ToUpper().Trim();
                                    objApprovalModel._ActionRemarks = remarks.Trim();
                                    objApprovalModel._ActionStatus = Convert.ToString(actionstatus).Trim();
                                    objApprovalModel._SkipFlag = skipflag.Trim();
                                    string re = SubmitToQueueMail(objApprovalModel);
                                    ViewBag.SubmitFor = Convert.ToString(actionstatus).Trim();
                                    if (re.ToUpper().Trim() == "SUCCESS")
                                    {

                                        ViewBag.ApproveErrMsg = "approve";
                                    }
                                    else
                                    {
                                        ViewBag.ApproveErrMsg = re;
                                    }

                                }
                            }
                        }

                    }
                    else
                    {
                        ViewBag.ApproveErrMsg = "error";
                    }
                }

                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }

        }

        public string SubmitToQueueMail(MailApprovalModel objApprovalModel)
        {
            DataTable dtForMail = new DataTable();
            dtForMail = rep.GetMailDetailsApprove();
            var reqType = dtForMail.Rows[0]["supplierheader_requesttype"].ToString();
            var requestfor = dtForMail.Rows[0]["requestfor"].ToString();
            var curapprovalstage = string.IsNullOrEmpty(dtForMail.Rows[0]["supplierheader_currentapprovalstage"].ToString()) ? "0" : dtForMail.Rows[0]["supplierheader_currentapprovalstage"].ToString();
            var queueto = objApprovalModel._QueueTo == null ? "" : objApprovalModel._QueueTo;
            var remarks = objApprovalModel._ActionRemarks == null ? "" : objApprovalModel._ActionRemarks;
            var requesttype = objApprovalModel._RequestType == null ? "" : objApprovalModel._RequestType.ToString();
            var actionstatus = Convert.ToInt32(objApprovalModel._ActionStatus);
            var skipflag = Convert.ToInt32(objApprovalModel._SkipFlag);
            requesttype = reqType.ToUpper().Trim();
            rep.SubmitToNextQueueMail(queueto, requesttype, remarks, actionstatus, skipflag);
            string gid = rep.GetSupIdForMail();

            if (requestfor != "S")
            {
                if (actionstatus == 1)
                {
                    requestfor = "A";
                }
                else if (actionstatus == 0)
                {
                    requestfor = "R";
                }
                if (curapprovalstage == "0")
                {
                    requestfor = "S";
                }

            }

            string resu = sendusermailapprove("ASMS", reqType.ToUpper().Trim(), gid, requestfor, curapprovalstage);

            return resu;
        }
        public string sendusermailapprove(string Module, string TransactionType, string gid, string request, string workflow)
        {
            try
            {
                string ids = "";
                string tomaigiv = "";
                string send_mail = "";
                string send_pw = "";
                string smpt_Mail = "";
                int smpt_Mailport = 0;
                string appurl = "";
                string rejurl = "";
                string ssll = "";
                string supplierheadergid = "0";
                ssll = ConfigurationManager.AppSettings["ssl"].ToString();
                send_mail = ConfigurationManager.AppSettings["sendmail"].ToString();
                send_pw = ConfigurationManager.AppSettings["sendpw"].ToString();
                smpt_Mail = ConfigurationManager.AppSettings["smptMail"].ToString();
                smpt_Mailport = Convert.ToInt16(ConfigurationManager.AppSettings["smptMailPort"].ToString());

                appurl = ConfigurationManager.AppSettings["Approvedclaimurl"].ToString();
                rejurl = ConfigurationManager.AppSettings["Rejectclaimurl"].ToString();

                List<MailApproveEntity> objForMailEntityloop = new List<MailApproveEntity>();
                List<MailApproveEntity> objForMailEntity = new List<MailApproveEntity>();
                objForMailEntity = rep.SelectMailtemplate(Module, TransactionType, gid, request, workflow).ToList();
                if (objForMailEntity.Count > 0)
                {
                    string myString = objForMailEntity[0].Content.ToString();
                    string data = objForMailEntity[0].TOid.ToString();
                    string[] mailsplit = data.Split(',');
                    if (data.Trim() != "")
                    {
                        for (int ii = 0; ii < mailsplit.Length; ii++)
                        {
                            string ordata = mailsplit[ii].ToString();

                            string[] mailsplit1 = ordata.Split('@');

                            if (ordata != "")
                            {
                                if (mailsplit1.Length > 1)
                                {
                                    if (tomaigiv == "")
                                    {
                                        tomaigiv = ordata;
                                    }
                                    else
                                    {
                                        tomaigiv = tomaigiv + "," + ordata;
                                    }
                                }
                                else
                                {
                                    if (tomaigiv == "")
                                    {
                                        tomaigiv = rep.SelectMailIdsForRoleGroup(mailsplit[ii].ToString(), gid, workflow, Module, TransactionType);
                                    }
                                    else
                                    {
                                        tomaigiv = tomaigiv + "," + rep.SelectMailIdsForRoleGroup(mailsplit[ii].ToString(), gid, workflow, Module, TransactionType);
                                    }
                                }
                            }

                        }
                    }

                    string ccmailids = "";
                    string ccdata = objForMailEntity[0].CCid.ToString();
                    if (ccdata.Trim() != "")
                    {
                        string[] mailsplitcc = ccdata.Split(',');
                        for (int jj = 0; jj < mailsplitcc.Length; jj++)
                        {
                            string ccdata1 = mailsplitcc[jj].ToString();
                            string[] mailsplitcc1 = ccdata1.Split('@');
                            if (ccdata1 != "")
                            {
                                if (mailsplitcc1.Length > 1)
                                {
                                    if (ccmailids == "")
                                    {
                                        ccmailids = ccdata1;
                                    }
                                    else
                                    {
                                        ccmailids = ccmailids + "," + ccdata1;
                                    }
                                }
                                else
                                {
                                    if (ccmailids == "")
                                    {
                                        ccmailids = rep.SelectMailIdsForRoleGroup(mailsplitcc[jj].ToString(), gid, workflow, Module, TransactionType);
                                    }
                                    else
                                    {
                                        ccmailids = ccmailids + "," + rep.SelectMailIdsForRoleGroup(mailsplitcc[jj].ToString(), gid, workflow, Module, TransactionType);
                                    }
                                }
                            }

                        }
                    }

                    string bccmailids = "";
                    string bccdata = objForMailEntity[0].BCCid.ToString();
                    if (bccdata.Trim() != "")
                    {
                        string[] mailsplitbcc = bccdata.Split(',');
                        for (int kk = 0; kk < mailsplitbcc.Length; kk++)
                        {
                            string bccdata1 = mailsplitbcc[kk].ToString();
                            string[] mailsplitbcc1 = bccdata1.Split('@');
                            if (bccdata1 != "")
                            {
                                if (mailsplitbcc1.Length > 1)
                                {
                                    if (bccmailids == "")
                                    {
                                        bccmailids = bccdata1;
                                    }
                                    else
                                    {
                                        bccmailids = bccmailids + "," + bccdata1;
                                    }
                                }
                                else
                                {
                                    if (bccmailids == "")
                                    {
                                        bccmailids = rep.SelectMailIdsForRoleGroup(mailsplitbcc[kk].ToString(), gid, workflow, Module, TransactionType);
                                    }
                                    else
                                    {
                                        bccmailids = ccmailids + "," + rep.SelectMailIdsForRoleGroup(mailsplitbcc[kk].ToString(), gid, workflow, Module, TransactionType);
                                    }
                                }
                            }

                        }
                    }

                    objForMailEntityloop = rep.Getmailselectdfield(objForMailEntity[0].TemplateId.ToString()).ToList();
                    if (objForMailEntityloop.Count > 0)
                    {
                        DataTable dtreqid = new DataTable();
                        dtreqid = rep.Selectbyid(Module, objForMailEntity[0].TransactionTypeName.ToString(), gid, objForMailEntity[0].TriggerTypeName.ToString());
                        if (dtreqid.Rows.Count > 0)
                        {
                            supplierheadergid = dtreqid.Rows[0]["supplierheader_gid"].ToString();
                            for (int tr = 0; objForMailEntityloop.Count > tr; tr++)
                            {
                                string col = objForMailEntityloop[tr].ToGetTypeName.ToString();
                                string colval = Convert.ToString(dtreqid.Rows[0][col].ToString());
                                string val = "[" + objForMailEntityloop[tr].ToGetTypeId.ToString() + "]";
                                myString = myString.Replace(val, colval);
                            }
                        }

                        SmtpClient Mail = new SmtpClient(smpt_Mail, smpt_Mailport);
                        MailMessage mailmsg = new MailMessage();
                        MailAddress mfrom = new MailAddress(send_mail.ToString());
                        mailmsg.From = mfrom;
                        string isfinalapprover = objForMailEntity[0].IsFinalApprover.ToString();
                        DataTable dtMailTo = new DataTable();
                        dtMailTo = rep.USERMailIDTo(Module, objForMailEntity[0].TransactionTypeName.ToString(), gid, objForMailEntity[0].TriggerTypeName.ToString(), isfinalapprover.ToString().ToUpper().Trim());
                        DataTable dtNextApproverLink = new DataTable();
                        dtNextApproverLink = rep.GetNextApproverLink(Module, objForMailEntity[0].TransactionTypeName.ToString(), gid);
                        string NextApproverRoleGroupName = rep.GetNextApproverLink1(Module, objForMailEntity[0].TransactionTypeName.ToString(), gid);
                        for (int maildt = 0; maildt < dtMailTo.Rows.Count; maildt++)
                        {
                            string str = dtMailTo.Rows[maildt]["employee_office_email"].ToString();
                            string mailidtousercode = dtMailTo.Rows[maildt]["employee_code"].ToString();
                            mailmsg.To.Add(new MailAddress(str));

                            mailmsg.Subject = objForMailEntity[0].Subject.ToString();

                            mailmsg.IsBodyHtml = true;
                            mailmsg.Body = "<br >  ";
                            mailmsg.Body = mailmsg.Body + myString;

                            if (objForMailEntity[0].Includeflg.ToString() == "Y")
                            {
                                string ActivationUrl = appurl + "?usercode=" + Encode(mailidtousercode) + "&queuegid=" + Encode(gid) + "&lnkfor=1";
                                string ActivationUrlreject = rejurl + "?usercode=" + Encode(mailidtousercode) + "&queuegid=" + Encode(gid) + "&lnkfor=0";
                                string appove = "<br /><table style='width: 100%; padding-left: 0%;'>";
                                appove += "<tr><td>Click Below to Submit Your Request...........<br><hr></td></tr>";
                                if (NextApproverRoleGroupName == "N")
                                {
                                    appove += "<tr><td style='padding-left: 20%;'>";
                                    appove += "  <br /> <a data-ved='0CAcQjRw' href='" + ActivationUrl + "' name='bvcbvc' jsaction='mousedown:irc.rl;keydown:irc.rlk;irc.il;' style='color: #FFF;background-color: #449D44;border-color: #398439;text-decoration: none;display: inline-block;padding: 6px 12px;margin-bottom: 0px;font-size: 14px;font-weight: 400;line-height: 1.42857;text-align: center;white-space: nowrap;vertical-align: middle;cursor: pointer;-moz-user-select: none;background-image: none;border: 1px solid transparent;border-radius: 4px;font-family: inherit;text-transform: none;overflow: visible;margin: 0px;font: inherit;' id='irc_mila'>Approve</a>";
                                    appove += "   &nbsp;&nbsp;&nbsp;&nbsp;";
                                    appove += "   <a data-ved='0CAcQjRw' href='" + ActivationUrlreject + "' name='bvcbvc' jsaction='mousedown:irc.rl;keydown:irc.rlk;irc.il;' style='color: #FFF;background-color: #EC971F;border-color: #D58512;text-decoration: none;display: inline-block;padding: 6px 12px;margin-bottom: 0px;font-size: 14px;font-weight: 400;line-height: 1.42857;text-align: center;white-space: nowrap;vertical-align: middle;cursor: pointer;-moz-user-select: none;background-image: none;border: 1px solid transparent;border-radius: 4px;font-family: inherit;text-transform: none;overflow: visible;margin: 0px;font: inherit;' id='irc_milr'>Reject</a>";
                                    appove += "  </td>";
                                    appove += "</tr>";
                                }
                                else
                                {
                                    if (dtNextApproverLink.Rows.Count > 0)
                                    {
                                        appove += "<tr><td>Select " + NextApproverRoleGroupName + "<br></td></tr>";
                                        for (int nxt = 0; nxt < dtNextApproverLink.Rows.Count; nxt++)
                                        {
                                            string empgid = dtNextApproverLink.Rows[nxt]["employee_gid"].ToString();
                                            string empcode = dtNextApproverLink.Rows[nxt]["employee_code"].ToString().ToUpper().Trim();
                                            string empname = dtNextApproverLink.Rows[nxt]["employee_name"].ToString().ToUpper().Trim();
                                            string emp = empcode + "-" + empname;
                                            string newurl = ActivationUrl + "&nextapprover=" + empgid;
                                            appove += " <tr><td> <a  href='" + newurl + "' name='bvcbvc' >Click Here To Select - " + emp + " </a></td></tr>";
                                        }
                                        string IsMandatoryApprover = rep.GetNextApproverIsMandatory(TransactionType, Module, gid);
                                        string newurl1 = ActivationUrl + "&nextapprover=0";
                                        appove += "<tr><td style='padding-left: 20%;'>";
                                        if (IsMandatoryApprover == "N")
                                        {
                                            appove += " <br /> <a data-ved='0CAcQjRw' href='" + newurl1 + "' name='bvcbvc' jsaction='mousedown:irc.rl;keydown:irc.rlk;irc.il;' style='color: #FFF;background-color: #449D44;border-color: #398439;text-decoration: none;display: inline-block;padding: 6px 12px;margin-bottom: 0px;font-size: 14px;font-weight: 400;line-height: 1.42857;text-align: center;white-space: nowrap;vertical-align: middle;cursor: pointer;-moz-user-select: none;background-image: none;border: 1px solid transparent;border-radius: 4px;font-family: inherit;text-transform: none;overflow: visible;margin: 0px;font: inherit;' id='irc_mila'>Skip  " + NextApproverRoleGroupName + "</a>";
                                            appove += "   &nbsp;&nbsp;&nbsp;&nbsp;";
                                        }
                                        appove += "   <a data-ved='0CAcQjRw' href='" + ActivationUrlreject + "' name='bvcbvc' jsaction='mousedown:irc.rl;keydown:irc.rlk;irc.il;' style='color: #FFF;background-color: #EC971F;border-color: #D58512;text-decoration: none;display: inline-block;padding: 6px 12px;margin-bottom: 0px;font-size: 14px;font-weight: 400;line-height: 1.42857;text-align: center;white-space: nowrap;vertical-align: middle;cursor: pointer;-moz-user-select: none;background-image: none;border: 1px solid transparent;border-radius: 4px;font-family: inherit;text-transform: none;overflow: visible;margin: 0px;font: inherit;' id='irc_milr'>Reject</a>";
                                        appove += "  </td>";
                                        appove += "</tr>";
                                    }
                                }

                                appove += "</table>";
                                mailmsg.Body = mailmsg.Body + appove;
                            }
                            string regardstag = "<br /><table style='width: 100%; padding-left: 0%;'>";
                            regardstag += "<tr><td style='font-weight:bold;font-size: 18px;'>Regards<br></td></tr>";
                            regardstag += "<tr><td>";
                            regardstag += "<span style='color: black;background-color: transparent;font-size: 18px;font-weight: bold;line-height: 1.42857;'>" + objForMailEntity[0].Signature.ToString() + "</a>";
                            regardstag += "  </td>";
                            regardstag += "</tr>";
                            regardstag += "</table>";
                            mailmsg.Body = mailmsg.Body + regardstag;

                            Mail.EnableSsl = Convert.ToBoolean(ssll);
                            NetworkCredential credentials = new NetworkCredential(send_mail, send_pw);
                            Mail.UseDefaultCredentials = false;
                            Mail.Credentials = credentials;
                            Mail.Send(mailmsg);
                            mailmsg.To.Remove(new MailAddress(str));

                            if (Module == "ASMS")
                            {
                                string sentmailflag = rep.mailsentmessage(objForMailEntity[0].TemplateId.ToString(), "6", supplierheadergid, "1", DateTime.Now.ToString().ToString(), str.ToString(), "", "", objForMailEntity[0].Subject.ToString(), objForMailEntity[0].Content.ToString(), objCmnFunctions.convertoDateTimeString(DateTime.Now.ToString()).ToString(), "Y", objCmnFunctions.convertoDateTimeString(DateTime.Now.ToString()).ToString(), "N");

                            }

                        }
                        SmtpClient Mail1 = new SmtpClient(smpt_Mail, smpt_Mailport);
                        MailMessage mailmsg1 = new MailMessage();
                        MailAddress mfrom1 = new MailAddress(send_mail.ToString());
                        mailmsg1.From = mfrom1;
                        string[] multinew = tomaigiv.Split(',');
                        foreach (string str in multinew)
                        {
                            mailmsg1.To.Add(new MailAddress(str));
                        }
                        if (ccmailids.Trim() != "")
                        {
                            string[] ccmailadd = ccmailids.Split(',');
                            foreach (string str1 in ccmailadd)
                            {
                                mailmsg1.CC.Add(new MailAddress(str1));
                            }
                        }
                        if (bccmailids.Trim() != "")
                        {
                            string[] bccmailadd = bccmailids.Split(',');
                            foreach (string str2 in bccmailadd)
                            {
                                mailmsg1.Bcc.Add(new MailAddress(str2));
                            }
                        }

                        mailmsg1.Subject = objForMailEntity[0].Subject.ToString();

                        mailmsg1.IsBodyHtml = true;
                        mailmsg1.Body = "<br >  ";
                        mailmsg1.Body = mailmsg1.Body + myString;

                        string regardstag1 = "<br /><table style='width: 100%; padding-left: 0%;'>";
                        regardstag1 += "<tr><td style='font-weight:bold;font-size: 18px;'>Regards<br></td></tr>";
                        regardstag1 += "<tr><td>";
                        regardstag1 += "<span style='color: black;background-color: transparent;font-size: 18px;font-weight: bold;line-height: 1.42857;'>" + objForMailEntity[0].Signature.ToString() + "</a>";
                        regardstag1 += "  </td>";
                        regardstag1 += "</tr>";
                        regardstag1 += "</table>";
                        mailmsg1.Body = mailmsg1.Body + regardstag1;

                        Mail1.EnableSsl = Convert.ToBoolean(ssll);
                        NetworkCredential credentials1 = new NetworkCredential(send_mail, send_pw);
                        Mail1.UseDefaultCredentials = false;
                        Mail1.Credentials = credentials1;
                        Mail1.Send(mailmsg1);

                        string sentmail = "SUCCESS";
                        if (sentmail == "SUCCESS")
                        {
                            if (Module == "ASMS")
                            {
                                sentmail = rep.mailsentmessage(objForMailEntity[0].TemplateId.ToString(), "6", supplierheadergid, "1", objCmnFunctions.convertoDateTimeString(DateTime.Now.ToString()).ToString(), mailmsg1.To.ToString(), mailmsg1.CC.ToString(), mailmsg1.Bcc.ToString(), objForMailEntity[0].Subject.ToString(), objForMailEntity[0].Content.ToString(), objCmnFunctions.convertoDateTimeString(DateTime.Now.ToString()).ToString(), "Y", objCmnFunctions.convertoDateTimeString(DateTime.Now.ToString()).ToString(), "N");
                                return sentmail;
                            }
                        }
                        Response.Write("<script>alert('Mail Sucessfully Sent To " + ids + "," + tomaigiv + "....');</script>");
                    }
                }
                return "template Not match";
            }
            catch (Exception ex)
            {
                return "faild";
            }
            finally
            {

            }
        }
        public void GetNextApproverGid(string nextapprovergid)
        {
            try
            {
                if (nextapprovergid != null)
                {
                    nextapprovergid = Decode(nextapprovergid);
                    Session["nextapprovergid"] = nextapprovergid;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string Encode(string encodeMe)
        {
            byte[] encoded = System.Text.Encoding.UTF8.GetBytes(encodeMe);
            return Convert.ToBase64String(encoded);
        }
        public static string Decode(string decodeMe)
        {
            byte[] encoded = Convert.FromBase64String(decodeMe);
            return System.Text.Encoding.UTF8.GetString(encoded);
        }


        public string PRApprovalRequestMail(PRNewEntity PRParentDetailsInsert,string rejectflag)
        {
            string ErrorMsg = string.Empty;
            string result1 = string.Empty;
            List<flexibuydashboard> obj = new List<flexibuydashboard>();

            try
            {

                string Data1 = "0";
                string ref_name = "PR";
                DataSet ds = new DataSet();
                DataTable dt1 = new DataTable();
                ErrorMsg = "1";
                DataSet ds1 = db.CheckAuthorize(Convert.ToString(PRParentDetailsInsert.PRGid), Convert.ToString(plib.LoginUserId), "PR");

                dt = ds1.Tables[0];

                if (dt.Rows.Count > 0)
                {
                    //result1 = dt.Rows[0]["Message"].ToString();
                    if (Convert.ToString(dt.Rows[0]["Result"]) == "Y")
                    {
                        result1 = Convert.ToString(dt.Rows[0]["Result"]);
                    }
                    else
                    {
                        result1 = Convert.ToString(dt.Rows[0]["Message"]);//JsonConvert.SerializeObject(dt);
                    }
                }
                else
                {

                    result1 = "Failure";//JsonConvert.SerializeObject(dt);
                }

                if (result1 == "Y")
                {
                    if (rejectflag == "1")
                    {

                        int result = objModel.RejectApprover(PRParentDetailsInsert);
                        if (result > 0)
                        {
                            ForMailController mailsender = new ForMailController();
                            CbfSumModel objMail = new CbfSumModel();
                            int refgid = Convert.ToInt32(PRParentDetailsInsert.PRGid);
                            string reqstatus = objMail.GetRequestStatus(refgid, "PR");
                            int queuegid = objMail.GetQueueGidForMail(refgid, "PR");
                            mailsender.sendusermail("FB", "PR", Convert.ToString(queuegid), reqstatus, "0");
                            ErrorMsg = "PR Rejected Successfully";
                        }
                    }
                    else
                    {

                    string result = objModel.SubmitApprover(PRParentDetailsInsert);

                    ds = objprModel.Getapprovaldetails(ref_name);
                    //dt1 = new DataTable(typeof(flexibuydashboard).Name);
                    dt1 = ds.Tables[0];

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        Data1 = JsonConvert.SerializeObject(dt1);
                    }

                    if (result.ToUpper() != "SUCCESS")
                    {
                        ErrorMsg = result;
                    }
                    else
                    {
                        ForMailController mailsender = new ForMailController();
                        CbfSumModel objMail = new CbfSumModel();
                        int refgid = Convert.ToInt32(PRParentDetailsInsert.PRGid);
                        string reqstatus = objMail.GetRequestStatus(refgid, "PR");
                        int queuegid = objMail.GetQueueGidForMail(refgid, "PR");
                        //old
                        //mailsender.sendusermail("FB", "PR", Convert.ToString(queuegid), reqstatus, "0");
                        //vadivu add
                        int PrGid = Convert.ToInt32(PRParentDetailsInsert.PRGid);
                        string EcfGid = "EcfGid";
                        string POgid = "POgid";
                        string cbfgid = "cbfgid";
                        int WoGid = 0;
                        mailsender.sendusermailEOW("FB", "PR", Convert.ToString(queuegid), reqstatus, "0", EcfGid, PrGid, POgid, cbfgid, WoGid);

                            //end
                            ErrorMsg = "Success";
                        }
                    }
                }
                else
                {
                    ErrorMsg = result1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ErrorMsg;
        }


        public string CBFApprovalRequestMail(string CBFHeaderGId, string Type, string IsReject, string Remarks)
        {

            try
            {
                string Data2 = "0";
                string ref_name = "CBF";
                DataSet ds2 = new DataSet();
                DataTable dt2 = new DataTable();

                string Data1 = "";
                string result = "";

                if (Type != "1" && Type != "7")
                {
                    DataSet ds1 = db.CheckAuthorize(CBFHeaderGId, Convert.ToString(plib.LoginUserId), "CBF");

                    dt = ds1.Tables[0];

                    if (dt.Rows.Count > 0 && Type != "1")
                    {
                        if (Convert.ToString(dt.Rows[0]["Result"]) == "Y")
                        {
                            result = Convert.ToString(dt.Rows[0]["Result"]);
                        }
                        else
                        {
                            result = "You are Not Authorized";
                            //  Data1 = JsonConvert.SerializeObject(dt);
                            Data1 = "You are Not Authorized";
                        }
                    }
                    else
                    {
                        result = "You are Not Authorized";
                        // Data1 = JsonConvert.SerializeObject(dt);
                        Data1 = "You are Not Authorized";
                    }
                }
                if ((Type == "1") || (Type != "1" && result == "Y") || (Type == "7"))
                {
                    DataSet ds = db.SubmitCBF(CBFHeaderGId, Type, IsReject, Remarks, plib.LoginUserId);

                    dt = ds.Tables[0];
                    string CBFHeaderGIds = CBFHeaderGId;
                    if (dt.Rows.Count > 0)
                    {
                        //string Cbfstatus = dt.Rows[0]["Clear"].ToString();
                        if (dt.Rows[0]["Clear"].ToString() == "True")
                        {
                            ForMailController mailsender = new ForMailController();
                            CbfSumModel objMail = new CbfSumModel();
                            //int refgid = objMail.GetRefGidForMailCbfheader("CBF", CBFHeaderGIds);
                            string reqstatus = objMail.GetRequestStatus(Convert.ToInt32(CBFHeaderGId), "CBF");
                            int queuegid = objMail.GetQueueGidForMail(Convert.ToInt32(CBFHeaderGId), "CBF");
                            //old
                            // mailsender.sendusermail("FB", "CBF", Convert.ToString(queuegid), reqstatus, "0");
                            //vadivu add
                            string cbfgid = CBFHeaderGId;
                            string EcfGid = "EcfGid";
                            string POgid = "POgid";
                            int PrGid = 0;
                            int Wogid = 0;
                            mailsender.sendusermailEOW("FB", "CBF", Convert.ToString(queuegid), reqstatus, "0", EcfGid, PrGid, POgid, cbfgid, Wogid);

                            //end
                        }
                        //Data1 = JsonConvert.SerializeObject(dt);
                        Data1 = Convert.ToString(dt.Rows[0]["Message"]);
                    }
                }

                return Data1;

            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog("CbfController_SubmitCBF" + ex.Message.ToString(), ex.ToString());
                return null;
            }
        }

        public string POApprovalRequestMail(string POHeaderGId, string POEndDate, string ProjectOwnerGId, string VendorGId, string Type, string VendorNote, string TemplateGId, string AddedTermAndCondtion, string IsRemoved, string ViewType, string IsReject, string Remarks, string VendorcontactId)
        {
            try
            {
                CbfSumModel objforMail = new CbfSumModel();
                string Data1 = "", Data2 = "0";
                string result = "";
                DataSet ds2 = new DataSet();
                DataTable dt2 = new DataTable();
                if (ViewType != "1")
                {
                    DataSet ds1 = db.CheckAuthorize(POHeaderGId, Convert.ToString(plib.LoginUserId), "PO");

                    dt = ds1.Tables[0];

                    if (dt.Rows.Count > 0 && ViewType != "1")
                    {
                        if (Convert.ToString(dt.Rows[0]["Result"]) == "Y")
                        {
                            result = Convert.ToString(dt.Rows[0]["Result"]);
                        }
                        else
                        {
                            result = Convert.ToString(dt.Rows[0]["Message"]);
                            Data1 = Convert.ToString(dt.Rows[0]["Message"]);//JsonConvert.SerializeObject(dt);
                        }
                    }
                    else
                    {
                        result = "Process Failed!";
                        Data1 = "Process Failed!";
                    }
                }
                if ((ViewType == "1") || (ViewType != "1" && result == "Y"))
                {


                    DataSet ds = db.SubmitPO(POHeaderGId, plib.ConvertDate(POEndDate), ProjectOwnerGId, VendorGId, Type, VendorNote, TemplateGId, AddedTermAndCondtion, IsRemoved, ViewType, IsReject, Remarks, plib.LoginUserId, VendorcontactId);

                    dt = ds.Tables[0];
                    if (dt.Rows.Count > 0)
                    {

                        if (dt.Rows[0]["Clear"].ToString() == "True")
                        {
                            ForMailController mailsender = new ForMailController();
                            //CbfSumModel objforMail = new CbfSumModel();
                            //int refgid = objforMail.GetRefGidForMail("PO");
                            string reqstatus = objforMail.GetRequestStatus(Convert.ToInt32(POHeaderGId), "PO");
                            int queuegid = objforMail.GetQueueGidForMail(Convert.ToInt32(POHeaderGId), "PO");
                            string curapprovalstage = "0";
                            if (reqstatus == "S")
                            {
                                curapprovalstage = "0";
                            }
                            else
                            {
                                curapprovalstage = "1";
                            }

                            string pogid = POHeaderGId;
                            string EcfGid = "EcfGid";
                            int PrGid = 0;
                            string cbfgid = "cbfgid";
                            int WoGid = 0;

                            mailsender.sendusermailEOW("FB", "PO", Convert.ToString(queuegid), reqstatus, curapprovalstage, EcfGid, PrGid, pogid, cbfgid, WoGid);

                        }
                    }
                    // Data1 = JsonConvert.SerializeObject(dt);
                    Data1 = Convert.ToString(dt.Rows[0]["Message"]);
                }
                return Data1;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return null;
            }

        }



        public string WOApprovalRequestMail(WOEntity ObjWOEntity, string rejectflag)
        {

            string ErrorMsg = string.Empty;
            string result = string.Empty;
            string Data2 = "0";

            try
            {

                ErrorMsg = "0";
                DataTable dt = new DataTable();
                DataSet ds1 = db.CheckAuthorize(Convert.ToString(ObjWOEntity.WOGid), Convert.ToString(plib.LoginUserId), "WO");

                dt = ds1.Tables[0];

                if (dt.Rows.Count > 0)
                {
                    if (Convert.ToString(dt.Rows[0]["Result"]) == "Y")
                    {
                        result = Convert.ToString(dt.Rows[0]["Result"]);
                    }
                    else
                    {

                        result = JsonConvert.SerializeObject(dt);
                    }
                }
                else
                {

                    result = JsonConvert.SerializeObject(dt);
                }

                if (result == "Y")
                {

                    if (rejectflag == "1")
                    {
                        ErrorMsg = objWOModel.RejectWO(ObjWOEntity);
                        if (ErrorMsg == "1")
                        {
                            CbfSumModel objforMail = new CbfSumModel();
                            //int refgid = ObjWOEntity.WOGid;
                            string reqstatus = objforMail.GetRequestStatus(Convert.ToInt32(ObjWOEntity.WOGid), "WO");
                            int queuegid = objforMail.GetQueueGidForMail(Convert.ToInt32(ObjWOEntity.WOGid), "WO");
                            string curapprovalstage = "0";
                            if (reqstatus == "S")
                            {
                                curapprovalstage = "0";
                            }
                            else
                            {
                                curapprovalstage = "1";
                            }
                            ForMailController mailsender = new ForMailController();
                            mailsender.sendusermail("FB", "WO", Convert.ToString(queuegid), reqstatus, curapprovalstage);
                            ErrorMsg = "WO Rejected Successfully";
                        }
                    }
                    else
                    {
                        ErrorMsg = objWOModel.GetDelmatemployeeForWo(ObjWOEntity);
                        if (ErrorMsg == "success" || ErrorMsg == "Success")
                        {
                            CbfSumModel objforMail = new CbfSumModel();
                            // int refgid = ObjWOEntity.WOGid;
                            string reqstatus = objforMail.GetRequestStatus(Convert.ToInt32(ObjWOEntity.WOGid), "WO");
                            int queuegid = objforMail.GetQueueGidForMail(Convert.ToInt32(ObjWOEntity.WOGid), "WO");
                            string curapprovalstage = "0";
                            if (reqstatus == "S")
                            {
                                curapprovalstage = "0";
                            }
                            else
                            {
                                curapprovalstage = "1";
                            }
                            ForMailController mailsender = new ForMailController();
                            //old
                            // mailsender.sendusermail("FB", "WO", Convert.ToString(queuegid), reqstatus, curapprovalstage);
                            //vadivu add
                            string EcfGid = "EcfGid";
                            int PrGid = 0;
                            string pogid = "pogid";
                            string cbfgid = "cbfgid";
                            mailsender.sendusermailEOW("FB", "WO", Convert.ToString(queuegid), reqstatus, curapprovalstage, EcfGid, PrGid, pogid, cbfgid, ObjWOEntity.WOGid);
                            ErrorMsg = "WO Approved Successfully";
                        }
                    }
                }
                return ErrorMsg;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return null;
            }
        }


        public string PARApprovalRequestMail(PAREntity PARParentDetailsInsert, string rejectflag)
        {
            string ErrorMsg = string.Empty;
            try
            {
                string Data1 = "0";
                string ref_name = "PAR";
                DataSet ds1 = new DataSet();
                DataTable dt1 = new DataTable();
                string result1 = string.Empty;
                ds1 = db.CheckAuthorize(Convert.ToString(PARParentDetailsInsert.PARGid), Convert.ToString(plib.LoginUserId), "PAR");

                dt = ds1.Tables[0];

                if (dt.Rows.Count > 0)
                {
                    if (Convert.ToString(dt.Rows[0]["Result"]) == "Y")
                    {
                        result1 = Convert.ToString(dt.Rows[0]["Result"]);
                    }
                    else
                    {

                        result1 = JsonConvert.SerializeObject(dt);
                    }
                }
                else
                {

                    result1 = JsonConvert.SerializeObject(dt);
                }
                if (result1 == "Y")
                {
                    if (rejectflag == "1")
                    {

                        int result = objPARModel.RejectApprover(PARParentDetailsInsert);
                        if (result > 0)
                        {
                            ForMailController mailsender = new ForMailController();
                            CbfSumModel objMail = new CbfSumModel();
                            int refgid = objMail.GetRefGidForMail("PAR");
                            string reqstatus = objMail.GetRequestStatus(PARParentDetailsInsert.PARGid, "PAR");
                            int queuegid = objMail.GetQueueGidForMail(PARParentDetailsInsert.PARGid, "PAR");
                            mailsender.sendusermail("FB", "PAR", Convert.ToString(queuegid), reqstatus, "0");
                            ErrorMsg = "PAR Rejected Successfully";
                        }
                    }
                    else
                    {
                        string res = objPARModel.SubmitApprover(PARParentDetailsInsert);
                        if (res.ToUpper() == "SUCCESS")
                        {
                            ForMailController mailsender = new ForMailController();
                            CbfSumModel objMail = new CbfSumModel();
                            int refgid = objMail.GetRefGidForMail("PAR");
                            string reqstatus = objMail.GetRequestStatus(PARParentDetailsInsert.PARGid, "PAR");
                            int queuegid = objMail.GetQueueGidForMail(PARParentDetailsInsert.PARGid, "PAR");
                            mailsender.sendusermail("FB", "PAR", Convert.ToString(queuegid), reqstatus, "0");
                            ErrorMsg = "PAR Approved Successfully";
                        }
                        else
                        {
                            ErrorMsg = "Not Approved";
                        }
                    }

                }

                else
                {
                    ErrorMsg = "Not Authorized";
                }
                return ErrorMsg;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return null;
            }


        }


    }
}
