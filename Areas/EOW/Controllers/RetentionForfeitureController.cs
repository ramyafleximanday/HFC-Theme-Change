﻿using IEM.Areas.EOW.Models;
using IEM.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Data;
using System.Configuration;

namespace IEM.Areas.EOW.Controllers
{
    public class RetentionForfeitureController : Controller
    {
        //
        // GET: /EOW/RetentionForfeiture/

        private RetentionForfeitureRepository objModel;
        CmnFunctions objCmnFunctions = new CmnFunctions();
        public RetentionForfeitureController()
            : this(new RetentionForfeiture())
        {

        }
        public RetentionForfeitureController(RetentionForfeitureRepository objM)
        {
            objModel = objM;
        }

        public ActionResult Index()
        {
            List<Eow_RetentionForfeiture> Releaserecords = new List<Eow_RetentionForfeiture>();
            Releaserecords = objModel.RetentionReleaseGrid().ToList();
            if (Releaserecords.Count == 0)
            {
                ViewBag.records = "No Records Found";
            }

            return View(Releaserecords);
        }
        [HttpPost]
        public ActionResult Index(string filter_name = null, string filter_name1 = null, string ECFNo = null, string InvoiceNo = null, string Suppliercode = null, string Suppliername = null, string extendeddate = null, string command = null)
        {
            List<Eow_RetentionForfeiture> Releaserecords = new List<Eow_RetentionForfeiture>();
            if (command == "Search" || command == "Refresh")
            {
                Releaserecords = objModel.RetentionReleaseGrid(filter_name, filter_name1, ECFNo, InvoiceNo, Suppliercode, Suppliername, extendeddate).ToList();

                ViewBag.filter_name = filter_name;
                ViewBag.filter_name1 = filter_name1;
                ViewBag.ECFNo = ECFNo;
                ViewBag.InvoiceNo = InvoiceNo;
                ViewBag.Suppliercode = Suppliercode;
                ViewBag.Suppliername = Suppliername;
                ViewBag.extendeddate = extendeddate;
                if (Releaserecords.Count == 0)
                {
                    ViewBag.records = "No Records Found";
                }
            }
            if (command == "Clear")
            {
                Releaserecords = objModel.RetentionReleaseGrid().ToList();
            }
            return View(Releaserecords);
        }
        [HttpGet]
        public PartialViewResult RetentionFutureDoc(int id)
        {
            Eow_RetentionForfeiture typemodel = new Eow_RetentionForfeiture();
            typemodel = objModel.GetRetentionExtendRecordById(id);
            return PartialView(typemodel);
        }
        [HttpPost]
        public JsonResult InsertFutureDetails(Eow_RetentionForfeiture Extendinformation)
        {
            string res = string.Empty;
            int retentionserialno = 0;
            DataTable getretionlogserialno = new DataTable();
            DataTable getInformation = new DataTable();
            DataTable checkduplicate = new DataTable();
            DataTable getserilnobyid = new DataTable();
            checkduplicate = objModel.CheckData(Extendinformation.invoice_gid);
            if (checkduplicate.Rows.Count == 0)
            {
                getretionlogserialno = objModel.GetSerialNo();
                if (getretionlogserialno.Rows[0]["Serial No"].ToString() != "")
                {
                    retentionserialno = int.Parse(getretionlogserialno.Rows[0]["Serial No"].ToString());
                    retentionserialno++;
                    Extendinformation.retention_serialno = retentionserialno;
                }
                else
                {
                    retentionserialno++;
                    Extendinformation.retention_serialno = retentionserialno;
                }
                getInformation = objModel.GetRetentioninformation(Extendinformation.invoice_gid);
                if (getInformation.Rows.Count > 0)
                {
                    Extendinformation.retention_release_gid = int.Parse(getInformation.Rows[0]["retention_release_gid"].ToString());
                    Extendinformation.retention_releaseamount = decimal.Parse(getInformation.Rows[0]["retention_releaseamount"].ToString());
                    Extendinformation.invoice_gid = Extendinformation.invoice_gid;
                    Extendinformation.retention_exception = decimal.Parse(getInformation.Rows[0]["invoice_retention_exception"].ToString());
                    res = objModel.Insertextenddetails(Extendinformation);
                }
                else
                {
                    Extendinformation.retention_release_gid = 0;
                    Extendinformation.retention_releaseamount = 0;
                    Extendinformation.invoice_gid = Extendinformation.invoice_gid; 
                    Extendinformation.retention_exception = 0;
                    res = objModel.Insertextenddetails(Extendinformation);
                }
            }
            else
            {
                getretionlogserialno = objModel.GetSerialNo();
                if (getretionlogserialno.Rows[0]["Serial No"].ToString() != "")
                {
                    retentionserialno = int.Parse(getretionlogserialno.Rows[0]["Serial No"].ToString());
                    retentionserialno++;
                    Extendinformation.retention_serialno = retentionserialno;
                }
                else
                {
                    retentionserialno++;
                    Extendinformation.retention_serialno = retentionserialno;
                }
                getInformation = objModel.GetRetentioninformation(Extendinformation.invoice_gid);
                if (getInformation.Rows.Count > 0)
                {
                    Extendinformation.retention_release_gid = int.Parse(getInformation.Rows[0]["retention_release_gid"].ToString());
                    Extendinformation.retention_releaseamount = decimal.Parse(getInformation.Rows[0]["retention_releaseamount"].ToString());
                    Extendinformation.invoice_gid = Extendinformation.invoice_gid;
                    Extendinformation.retention_exception = decimal.Parse(getInformation.Rows[0]["invoice_retention_exception"].ToString());
                    res = objModel.Insertextenddetails(Extendinformation);
                }
                else
                {
                    Extendinformation.retention_release_gid = 0;
                    Extendinformation.retention_releaseamount = 0;
                    Extendinformation.invoice_gid = Extendinformation.invoice_gid;
                    Extendinformation.retention_exception = 0;
                    res = objModel.Insertextenddetails(Extendinformation);
                }
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
      
    }
}
