
//using IEM.Areas.EOW.Controllers;
//using IEM.Areas.MASTERS.Controllers;
using IEM.Common;
using IEM.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace IEM.Areas.FLEXISPEND.Models
{
    public class FlexispendDataModel : fsIreposiroty
    {
        ErrorLog objErrorLog = new ErrorLog();
        SqlConnection con = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter();
        CmnFunctions objCmnFunctions = new CmnFunctions();
        CommonIUD objCommonIUD = new CommonIUD();
        dbLib db = new dbLib();
        proLib plib = new proLib();
        CmnFunctions cmnfun = new CmnFunctions();

        //GSTR UPLOAD

        private void GetConnection()
        {
            if (con.State == ConnectionState.Closed)
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["connectionstring"].ToString();
                con.Open();
            }
        }

        //TO insert and update GSTR Excel File Upload
        public string ExcelGSTRUpload(DataTable dt, FS_GSTRModel gstrobjmodel)
        {
            string Emp_Msg = "";
            DataTable dt1 = new DataTable();

                int Gid = 0;
                dt1 = headergstr(gstrobjmodel);

            Gid = Convert.ToInt32(dt1.Rows[0][1].ToString());
            // ramya added on 19 jan 22
            DataTable Statusdt = new DataTable();
            Statusdt.Columns.Add("SNo");
            Statusdt.Columns.Add("Line");
            Statusdt.Columns.Add("GSTIN of supplier");
            Statusdt.Columns.Add("Trade/Legal name");
            Statusdt.Columns.Add("Invoice number");
            Statusdt.Columns.Add("Invoice Date");
            Statusdt.Columns.Add("Invoice Value");
            Statusdt.Columns.Add("Status");
            int sno = 1;
            int rowno = 1;
            try
            {                 
                // ["headergid"].ToString().Trim() = Gid;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    rowno = i;
                    GetConnection();
                    cmd = new SqlCommand("pr_eow_com_gstrupload", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    //To check Type regular or credit note
                    if (dt.Rows[i][3].ToString() == "Regular")
                    {
                        cmd.Parameters.Add("@invoicegid", SqlDbType.BigInt).Value = 0;
                        cmd.Parameters.Add("@GSTNumber", SqlDbType.VarChar).Value = string.IsNullOrEmpty(dt.Rows[i][0].ToString()) ? "" : Convert.ToString(dt.Rows[i][0].ToString());
                        cmd.Parameters.Add("@GSTNSupplier", SqlDbType.VarChar).Value = string.IsNullOrEmpty(dt.Rows[i][1].ToString()) ? "" : Convert.ToString(dt.Rows[i][1].ToString());

                        cmd.Parameters.Add("@InvoiceNo", SqlDbType.VarChar).Value = string.IsNullOrEmpty(dt.Rows[i][2].ToString()) ? "" : Convert.ToString(dt.Rows[i][2].ToString());
                        cmd.Parameters.Add("@InvoiceType", SqlDbType.Char).Value = string.IsNullOrEmpty(dt.Rows[i][3].ToString()) ? "" : Convert.ToString(dt.Rows[i][3].ToString().Substring(0, 1));
                        cmd.Parameters.Add("@InvoiceDate", SqlDbType.DateTime).Value = objCmnFunctions.convertoDateTime(dt.Rows[i][4].ToString());
                        cmd.Parameters.Add("@InvoiceAmount", SqlDbType.Decimal).Value = string.IsNullOrEmpty(dt.Rows[i][5].ToString()) ? 0 : Convert.ToDecimal(dt.Rows[i][5].ToString());
                        cmd.Parameters.Add("@PlaceOfSupply ", SqlDbType.VarChar).Value = string.IsNullOrEmpty(dt.Rows[i][6].ToString()) ? "" : Convert.ToString(dt.Rows[i][6].ToString());
                        cmd.Parameters.Add("@SupplyAttractReverseCharge", SqlDbType.Char).Value = (string.IsNullOrEmpty(dt.Rows[i][7].ToString()) ? "" : Convert.ToString(dt.Rows[i][7].ToString()));
                        cmd.Parameters.Add("@Cygnet_Tax_Rate", SqlDbType.Decimal).Value = string.IsNullOrEmpty(dt.Rows[i][8].ToString()) ? 0 : Convert.ToDecimal(dt.Rows[i][8].ToString());
                        cmd.Parameters.Add("@TaxRate", SqlDbType.Decimal).Value = string.IsNullOrEmpty(dt.Rows[i][9].ToString()) ? 0 : Convert.ToDecimal(dt.Rows[i][9].ToString());
                        cmd.Parameters.Add("@IGSTTax ", SqlDbType.Decimal).Value = string.IsNullOrEmpty(dt.Rows[i][10].ToString()) ? 0 : Convert.ToDecimal(dt.Rows[i][10].ToString());
                        cmd.Parameters.Add("@CGSTTax", SqlDbType.Decimal).Value = string.IsNullOrEmpty(dt.Rows[i][11].ToString()) ? 0 : Convert.ToDecimal(dt.Rows[i][11].ToString());
                        cmd.Parameters.Add("@SGSTTax ", SqlDbType.Decimal).Value = string.IsNullOrEmpty(dt.Rows[i][12].ToString()) ? 0 : Convert.ToDecimal(dt.Rows[i][12].ToString());
                        cmd.Parameters.Add("@CessAmount", SqlDbType.Decimal).Value = string.IsNullOrEmpty(dt.Rows[i][13].ToString()) ? 0 : Convert.ToDecimal(dt.Rows[i][13].ToString());
                        string fillingmonth = dt.Rows[i][14].ToString().Replace("'", " ");
                        fillingmonth = objCmnFunctions.convertoDateTimeString(objCmnFunctions.getconverttomonthtodatewith3chars(fillingmonth));
                        if (fillingmonth == "")
                        {
                            fillingmonth = "1900-01-01";
                        }
                        cmd.Parameters.Add("@GstrPeriod", SqlDbType.DateTime).Value = fillingmonth;
                        string fillingperiod = dt.Rows[i][15].ToString();
                        fillingperiod = objCmnFunctions.convertoDateTimeString(objCmnFunctions.getconverttomonthtodatewith3chars(fillingperiod)); // ramya modified on 18 jan 22
                        if(fillingperiod=="")
                        {
                            fillingperiod = "1900-01-01";
                        }
                        cmd.Parameters.Add("@FillingPeriod", SqlDbType.DateTime).Value = fillingperiod;
                        //cmd.Parameters.Add("@FillingPeriod", SqlDbType.DateTime).Value = objCmnFunctions.convertoDateTime((dt.Rows[i][15].ToString()));
                        cmd.Parameters.Add("@ITC", SqlDbType.Char).Value = string.IsNullOrEmpty(dt.Rows[i][16].ToString()) ? "" : Convert.ToString(dt.Rows[i][16].ToString());
                        cmd.Parameters.Add("@Reason", SqlDbType.VarChar).Value = string.IsNullOrEmpty(dt.Rows[i][17].ToString()) ? "" : Convert.ToString(dt.Rows[i][17].ToString());
                        cmd.Parameters.Add("@Cygnet_Applicable_TaxRate", SqlDbType.Decimal).Value = (string.IsNullOrEmpty(dt.Rows[i][18].ToString()) ? 0 : Convert.ToDecimal(dt.Rows[i][18].ToString()));
                        cmd.Parameters.Add("@HSN_Code", SqlDbType.VarChar).Value = (string.IsNullOrEmpty(dt.Rows[i][19].ToString()) ? "" : Convert.ToString(dt.Rows[i][19].ToString()));
                        cmd.Parameters.Add("@loginid", SqlDbType.VarChar).Value = HttpContext.Current.Session["employee_gid"].ToString().Trim();
                        cmd.Parameters.Add("@NoteSupplyType", SqlDbType.VarChar).Value = "";
                        cmd.Parameters.Add("@gstrheadergid", SqlDbType.VarChar).Value = Gid;
                    }
                    //Credit Note
                    else
                    {
                        cmd.Parameters.Add("@invoicegid", SqlDbType.BigInt).Value = 0;
                        cmd.Parameters.Add("@GSTNumber", SqlDbType.VarChar).Value = string.IsNullOrEmpty(dt.Rows[i][0].ToString()) ? "" : Convert.ToString(dt.Rows[i][0].ToString());
                        cmd.Parameters.Add("@GSTNSupplier", SqlDbType.VarChar).Value = string.IsNullOrEmpty(dt.Rows[i][1].ToString()) ? "" : Convert.ToString(dt.Rows[i][1].ToString());
                        cmd.Parameters.Add("@InvoiceNo", SqlDbType.VarChar).Value = string.IsNullOrEmpty(dt.Rows[i][2].ToString()) ? "" : Convert.ToString(dt.Rows[i][2].ToString());
                        cmd.Parameters.Add("@InvoiceType", SqlDbType.Char).Value = string.IsNullOrEmpty(dt.Rows[i][3].ToString()) ? "" : Convert.ToString(dt.Rows[i][3].ToString().Substring(0, 1));

                        cmd.Parameters.Add("@NoteSupplyType", SqlDbType.Char).Value = string.IsNullOrEmpty(dt.Rows[i][4].ToString()) ? "" : Convert.ToString(dt.Rows[i][4].ToString());
                        cmd.Parameters.Add("@InvoiceDate", SqlDbType.DateTime).Value = objCmnFunctions.convertoDateTime(dt.Rows[i][4].ToString());
                        cmd.Parameters.Add("@InvoiceAmount", SqlDbType.Decimal).Value = string.IsNullOrEmpty(dt.Rows[i][5].ToString()) ? 0 : Convert.ToDecimal(dt.Rows[i][5].ToString());
                        cmd.Parameters.Add("@PlaceOfSupply ", SqlDbType.VarChar).Value = string.IsNullOrEmpty(dt.Rows[i][7].ToString()) ? "" : Convert.ToString(dt.Rows[i][7].ToString());
                        cmd.Parameters.Add("@SupplyAttractReverseCharge", SqlDbType.Char).Value = (string.IsNullOrEmpty(dt.Rows[i][8].ToString()) ? "" : Convert.ToString(dt.Rows[i][8].ToString()));
                        cmd.Parameters.Add("@Cygnet_Tax_Rate", SqlDbType.Decimal).Value = string.IsNullOrEmpty(dt.Rows[i][10].ToString()) ? 0 : Convert.ToDecimal(dt.Rows[i][10].ToString());
                        cmd.Parameters.Add("@TaxRate", SqlDbType.Decimal).Value = string.IsNullOrEmpty(dt.Rows[i][9].ToString()) ? 0 : Convert.ToDecimal(dt.Rows[i][9].ToString());
                        cmd.Parameters.Add("@IGSTTax ", SqlDbType.Decimal).Value = string.IsNullOrEmpty(dt.Rows[i][10].ToString()) ? 0 : Convert.ToDecimal(dt.Rows[i][10].ToString());
                        cmd.Parameters.Add("@CGSTTax", SqlDbType.Decimal).Value = string.IsNullOrEmpty(dt.Rows[i][11].ToString()) ? 0 : Convert.ToDecimal(dt.Rows[i][11].ToString());
                        cmd.Parameters.Add("@SGSTTax ", SqlDbType.Decimal).Value = string.IsNullOrEmpty(dt.Rows[i][12].ToString()) ? 0 : Convert.ToDecimal(dt.Rows[i][12].ToString());
                        cmd.Parameters.Add("@CessAmount", SqlDbType.Decimal).Value = string.IsNullOrEmpty(dt.Rows[i][13].ToString()) ? 0 : Convert.ToDecimal(dt.Rows[i][13].ToString());
                        string fillingmonth = dt.Rows[i][14].ToString().Replace("'", " ");
                        cmd.Parameters.Add("@GstrPeriod", SqlDbType.DateTime).Value = objCmnFunctions.convertoDateTimeString(objCmnFunctions.getconverttomonthtodatewith3chars(fillingmonth));
                        cmd.Parameters.Add("@FillingPeriod", SqlDbType.DateTime).Value = objCmnFunctions.convertoDateTime((dt.Rows[i][15].ToString()));
                        cmd.Parameters.Add("@ITC", SqlDbType.Char).Value = string.IsNullOrEmpty(dt.Rows[i][16].ToString()) ? "" : Convert.ToString(dt.Rows[i][16].ToString());
                        cmd.Parameters.Add("@Reason", SqlDbType.VarChar).Value = string.IsNullOrEmpty(dt.Rows[i][17].ToString()) ? "" : Convert.ToString(dt.Rows[i][17].ToString());
                        cmd.Parameters.Add("@Cygnet_Applicable_TaxRate", SqlDbType.Decimal).Value = (string.IsNullOrEmpty(dt.Rows[i][18].ToString()) ? 0 : Convert.ToDecimal(dt.Rows[i][18].ToString()));
                        cmd.Parameters.Add("@HSN_Code", SqlDbType.VarChar).Value = (string.IsNullOrEmpty(dt.Rows[i][19].ToString()) ? "" : Convert.ToString(dt.Rows[i][19].ToString()));
                        cmd.Parameters.Add("@loginid", SqlDbType.VarChar).Value = HttpContext.Current.Session["employee_gid"].ToString().Trim();
                        cmd.Parameters.Add("@gstrheadergid", SqlDbType.VarChar).Value = Gid;

                    } 
                     
                    da = new SqlDataAdapter(cmd);
                    DataTable dtout = new DataTable();
                    da.Fill(dtout);
                    if (dtout.Rows.Count > 0)
                    {
                        Emp_Msg = dtout.Rows[0]["Msg"].ToString();

                        string clear = dtout.Rows[0]["Clear"].ToString(); 

                        Emp_Msg = string.Concat(dtout.Rows[0]["Msg"].ToString() + ';' + (dtout.Rows[0]["headergid"].ToString())); 
                    }
                    Statusdt.Rows.Add(sno, i + 1, string.IsNullOrEmpty(dt.Rows[i][0].ToString()) ? "" : Convert.ToString(dt.Rows[i][0].ToString()), string.IsNullOrEmpty(dt.Rows[i][1].ToString()) ? "" : Convert.ToString(dt.Rows[i][1].ToString()),
                        string.IsNullOrEmpty(dt.Rows[i][2].ToString()) ? "" : Convert.ToString(dt.Rows[i][2].ToString()),
                        objCmnFunctions.convertoDateTime(dt.Rows[i][4].ToString()),
                        string.IsNullOrEmpty(dt.Rows[i][5].ToString()) ? 0 : Convert.ToDecimal(dt.Rows[i][5].ToString()), Emp_Msg);
                    sno++;
                }
                return Emp_Msg;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                Statusdt.Rows.Add(sno, rowno + 1, string.IsNullOrEmpty(dt.Rows[rowno][0].ToString()) ? "" : Convert.ToString(dt.Rows[rowno][0].ToString()), string.IsNullOrEmpty(dt.Rows[rowno][1].ToString()) ? "" : Convert.ToString(dt.Rows[rowno][1].ToString()),
                        string.IsNullOrEmpty(dt.Rows[rowno][2].ToString()) ? "" : Convert.ToString(dt.Rows[rowno][2].ToString()),
                        objCmnFunctions.convertoDateTime(dt.Rows[rowno][4].ToString()),
                        string.IsNullOrEmpty(dt.Rows[rowno][5].ToString()) ? 0 : Convert.ToDecimal(dt.Rows[rowno][5].ToString()), ex.Message.ToString());
                sno++;
                //return "";
               return Emp_Msg = "error";
            }
            finally
            {
                con.Close();
                da.Dispose();
                
                HttpContext.Current.Session["Statusdt"] = Statusdt;
                
            }
        }
        public string TDSRejectedUpload(string remarks, string id, string action)
        {
            try
            {
                DataTable dt = new DataTable();
                string Emp_Msg = "";
                GetConnection();
                cmd = new SqlCommand("pr_asms_header_tdscheckerrejected", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@TDSHeadergid", SqlDbType.VarChar).Value = id;
                cmd.Parameters.Add("@Action", SqlDbType.VarChar).Value = action;
                cmd.Parameters.Add("@loginid", SqlDbType.VarChar).Value = HttpContext.Current.Session["employee_gid"].ToString().Trim();
                cmd.Parameters.Add("@Remarks", SqlDbType.VarChar).Value = remarks;
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    Emp_Msg = dt.Rows[0]["Msg"].ToString();
                }
                return Emp_Msg;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return "";
            }
            finally
            {
                con.Close();
                da.Dispose();
            }
        }
        //selva 20-01-2021
        public DataTable ExceValidationGSTRUpload(DataTable dt, FS_GSTRModel gstrobjmodel)
        {
            DataTable Errdatatabledetails = new DataTable();

            try
            {

                DataTable dt1 = new DataTable();

                Errdatatabledetails.Columns.Add("SNo");
                Errdatatabledetails.Columns.Add("InvoiceNo");
                Errdatatabledetails.Columns.Add("Error Description");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
					DataTable dtout = new DataTable();  // ramya modified on 30 Oct 21
                    GetConnection();
                    cmd = new SqlCommand("pr_eow_validation_gstrupload", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (dt.Rows[i][3].ToString() == "Regular")
                    {

                        cmd.Parameters.Add("@GSTNumber", SqlDbType.VarChar).Value = string.IsNullOrEmpty(dt.Rows[i][0].ToString()) ? "" : Convert.ToString(dt.Rows[i][0].ToString());
                        cmd.Parameters.Add("@InvoiceNo", SqlDbType.VarChar).Value = string.IsNullOrEmpty(dt.Rows[i][2].ToString()) ? "" : Convert.ToString(dt.Rows[i][2].ToString());
                        cmd.Parameters.Add("@InvoiceType", SqlDbType.Char).Value = string.IsNullOrEmpty(dt.Rows[i][3].ToString()) ? "" : Convert.ToString(dt.Rows[i][3].ToString().Substring(0, 1));

                    }
                    else
                    {
                        cmd.Parameters.Add("@GSTNumber", SqlDbType.VarChar).Value = string.IsNullOrEmpty(dt.Rows[i][0].ToString()) ? "" : Convert.ToString(dt.Rows[i][0].ToString());
                        cmd.Parameters.Add("@InvoiceNo", SqlDbType.VarChar).Value = string.IsNullOrEmpty(dt.Rows[i][2].ToString()) ? "" : Convert.ToString(dt.Rows[i][2].ToString());
                        cmd.Parameters.Add("@InvoiceType", SqlDbType.Char).Value = string.IsNullOrEmpty(dt.Rows[i][3].ToString()) ? "" : Convert.ToString(dt.Rows[i][3].ToString().Substring(0, 1));

                    }
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dtout);
                    if (dtout.Rows.Count > 0)
                    {
                        DataRow dr = Errdatatabledetails.NewRow();
                        dr["SNo"] = i;
                        dr["InvoiceNo"] = Convert.ToString(dt.Rows[i][2].ToString());
                        dr["Error Description"] = dtout.Rows[0]["Msg"].ToString();
                        Errdatatabledetails.Rows.Add(dr);
                    }
                }
                return Errdatatabledetails;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return Errdatatabledetails;
            }
            finally
            {
                con.Close();
                da.Dispose();
            }
        }
        public DataTable ExceValidationTDSUpload(DataTable dt, FS_GSTRModel tdsobjmodel)
        {
            DataTable Errdatatabledetails = new DataTable();
            try
            {
                DataTable dtout = new DataTable();
                DataTable dt1 = new DataTable();
                Errdatatabledetails.Columns.Add("SNo");
                Errdatatabledetails.Columns.Add("SupplierPanno");
                Errdatatabledetails.Columns.Add("Error Description");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    GetConnection();
                    cmd = new SqlCommand("pr_eow_validation_tdsupload", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@SupplierCode", SqlDbType.VarChar).Value = string.IsNullOrEmpty(dt.Rows[i][0].ToString()) ? "" : Convert.ToString(dt.Rows[i][0].ToString());
                    cmd.Parameters.Add("@SupplierPanno", SqlDbType.VarChar).Value = string.IsNullOrEmpty(dt.Rows[i][1].ToString()) ? "" : Convert.ToString(dt.Rows[i][1].ToString());
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dtout);
                    if (dtout.Rows.Count > 0)
                    {
                        DataRow dr = Errdatatabledetails.NewRow();
                        dr["SNo"] = i;
                        dr["InvoiceNo"] = Convert.ToString(dt.Rows[i][2].ToString());
                        dr["Error Description"] = dtout.Rows[0]["Msg"].ToString();
                        Errdatatabledetails.Rows.Add(dr);
                    }
                }
                return Errdatatabledetails;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return Errdatatabledetails;
            }
            finally
            {
                con.Close();
                da.Dispose();
            }
        }



        // To Check Excel Data Valid Or Not
        public string GetGSTRUploadStatusexcel(string valatate, string valatate1, string valatate2, string action)
        {
            string result = "";
            try
            {
                GetConnection();
                cmd = new SqlCommand("pr_fs_gstrupload_trn_excelvalate", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@gstnnumber", SqlDbType.VarChar).Value = valatate.ToString();
                cmd.Parameters.Add("@placeofsupply", SqlDbType.VarChar).Value = valatate1.ToString();
                cmd.Parameters.Add("@hsncode", SqlDbType.VarChar).Value = valatate2.ToString();
                cmd.Parameters.Add("@Result", SqlDbType.VarChar).Value = action;
                result = (string)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return "";
            }
            finally
            {
                con.Close();
                da.Dispose();
            }
            return result;
        }


        //selva Created 05-04-2021


        public string GetGSTRUploadStatusexcelVendor(string valatate, string valatate1, string valatate2, string action)
        {
            string result = "";
            try
            {
                GetConnection();
                cmd = new SqlCommand("pr_fs_gstrupload_trn_excelvalate", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@gstnnumber", SqlDbType.VarChar).Value = valatate.ToString();
                cmd.Parameters.Add("@placeofsupply", SqlDbType.VarChar).Value = valatate1.ToString();
                cmd.Parameters.Add("@hsncode", SqlDbType.VarChar).Value = valatate2.ToString();
                cmd.Parameters.Add("@Result", SqlDbType.VarChar).Value = action;
                result = (string)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return "";
            }
            finally
            {
                con.Close();
                da.Dispose();
            }
            return result;
        }

        //prema created 
        public string GetStatusexcel(string valatate, string valatate1, string valatate2, string action)
        {
            string result = "";
            try
            {
                GetConnection();
                cmd = new SqlCommand("pr_eow_trn_excelvalate", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@chkdata", SqlDbType.VarChar).Value = valatate.ToString();
                cmd.Parameters.Add("@chkdata1", SqlDbType.VarChar).Value = valatate1.ToString();
                cmd.Parameters.Add("@chkdata2", SqlDbType.VarChar).Value = valatate2.ToString();
                cmd.Parameters.Add("@Result", SqlDbType.VarChar).Value = action;
                result = (string)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return "";
            }
            finally
            {
                con.Close();
                da.Dispose();
            }
            return result;
        }





        //Get GSTR Upload Summary Details
        public IEnumerable<FS_GSTRModel> GetGSTRlist(string straction, string header_gid)
        {
            List<FS_GSTRModel> objGSTRModel = new List<FS_GSTRModel>();
            try
            {

                FS_GSTRModel objModel;
                GetConnection();
                DataTable dt = new DataTable();
                cmd = new SqlCommand("pr_fs_Get_gstrupload", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Action", SqlDbType.VarChar).Value = straction;
                cmd.Parameters.Add("@headergid", SqlDbType.VarChar).Value = header_gid;
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    objModel = new FS_GSTRModel();
                    objModel.GSTNumber = row["Cygnet_Supplier_GSTN_No"].ToString();
                    objModel.InvoiceNo = row["Cygnet_Invoice_No"].ToString();
                    objModel.InvoiceValue = row["Cygnet_Invoice_Amount"].ToString();
                    objModel.InvoiceDate = (row["Cygnet_Invoice_Date"].ToString());
                    objModel.TaxableValue = row["Cygnet_Taxable_Amount"].ToString();
                    objModel.ProviderLocation = row["Cygnet_Provider_Location_Name"].ToString();
                    objModel.ReceiverLocation = row["Cygnet_Receiver_Location_Name"].ToString();
                    objModel.CentralTax = (row["Cygnet_CGST_Amount"].ToString());
                    objModel.StateUTTax = row["Cygnet_SGST_Amount"].ToString();
                    objModel.integratedTax = row["Cygnet_IGST_Amount"].ToString();
                    objModel.ReceiverGSTN = row["Cygnet_Receiver_GSTN_No"].ToString();
                    objGSTRModel.Add(objModel);
                }
                return objGSTRModel;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return objGSTRModel;
            }
            finally
            {
                con.Close();
                da.Dispose();
            }
        }
        public IEnumerable<FS_GSTRModel> GetTDSlist(string straction, string header_gid)
        {
            List<FS_GSTRModel> objGSTRModel = new List<FS_GSTRModel>();
            try
            {
                FS_GSTRModel objModel;
                GetConnection();
                DataTable dt = new DataTable();
                cmd = new SqlCommand("pr_asms_Get_tdsuploaddetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Action", SqlDbType.VarChar).Value = straction;
                cmd.Parameters.Add("@headergid", SqlDbType.VarChar).Value = header_gid;
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    objModel = new FS_GSTRModel();
                    objModel.Suppliergid = row["Suppliercode"].ToString();
                    objModel.PANNo = row["NFDetail_PanNo"].ToString();
                    objModel.Tax = row["Tax"].ToString();
                    objModel.TaxSubtype = (row["TaxSubtype"].ToString());
                    objModel.Rate = row["NFDetail_TaxRate"].ToString();
                    objGSTRModel.Add(objModel);
                }
                return objGSTRModel;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return objGSTRModel;
            }
            finally
            {
                con.Close();
                da.Dispose();
            }
        }
        public IEnumerable<FS_GSTRModel> GetPopulateAuditList(string straction, string header_gid)
        {
            List<FS_GSTRModel> objGSTRModel = new List<FS_GSTRModel>();
            try
            {
                FS_GSTRModel objModel;
                GetConnection();
                DataTable dt = new DataTable();
                cmd = new SqlCommand("pr_asms_Get_tdsauditdetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Action", SqlDbType.VarChar).Value = straction;
                cmd.Parameters.Add("@headergid", SqlDbType.VarChar).Value = header_gid;
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    objModel = new FS_GSTRModel();
                    objModel.makerid = row["Actionby"].ToString();
                    objModel.insertdate = row["Actiondate"].ToString();
                    objModel.Status = row["StatusName"].ToString();
                    objGSTRModel.Add(objModel);
                }
                return objGSTRModel;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return objGSTRModel;
            }
            finally
            {
                con.Close();
                da.Dispose();
            }
        }
        public IEnumerable<FS_GSTRModel> GetTDSfilelist(string straction, string header_gid)
        {
            List<FS_GSTRModel> objGSTRModel = new List<FS_GSTRModel>();
            try
            {
                FS_GSTRModel objModel;
                GetConnection();
                DataTable dt = new DataTable();
                cmd = new SqlCommand("pr_asms_Get_tdsuploaddetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Action", SqlDbType.VarChar).Value = straction;
                cmd.Parameters.Add("@headergid", SqlDbType.VarChar).Value = header_gid;
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    objModel = new FS_GSTRModel();
                    objModel.filename = row["NFHeader_FileName"].ToString();
                    objModel.makerid = row["NFHeader_insertby"].ToString();
                    objModel.insertdate = row["NFHeader_insertdate"].ToString();
                    objModel.Status = row["StatusName"].ToString();
                    objModel.headergid = row["NFDetail_NFHeader_gid"].ToString();
                    objGSTRModel.Add(objModel);
                }
                return objGSTRModel;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return objGSTRModel;
            }
            finally
            {
                con.Close();
                da.Dispose();
            }
        }
        public IEnumerable<FS_GSTRModel> GetTDSfilelistChecker(string straction, string header_gid)
        {
            List<FS_GSTRModel> objGSTRModel = new List<FS_GSTRModel>();
            try
            {
                FS_GSTRModel objModel;
                GetConnection();
                DataTable dt = new DataTable();
                cmd = new SqlCommand("pr_asms_Get_tdsuploaddetailsCheckerSummary", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Action", SqlDbType.VarChar).Value = straction;
                cmd.Parameters.Add("@headergid", SqlDbType.VarChar).Value = "";
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    objModel = new FS_GSTRModel();
                    objModel.filename = row["NFHeader_FileName"].ToString();
                    objModel.makerid = row["NFHeader_insertby"].ToString();
                    objModel.insertdate = row["NFHeader_insertdate"].ToString();
                    objModel.Status = row["StatusName"].ToString();
                    objModel.headergid = row["NFDetail_NFHeader_gid"].ToString();
                    objGSTRModel.Add(objModel);
                }
                return objGSTRModel;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return objGSTRModel;
            }
            finally
            {
                con.Close();
                da.Dispose();
            }
        }
        //Get GSTR Upload Summary Details Muthu
        public DataTable ExportGSTRlist()
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnection();

                cmd = new SqlCommand("pr_fs_Get_gstrupload", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Action", SqlDbType.VarChar).Value = "Export";
                cmd.Parameters.Add("@headergid", SqlDbType.VarChar).Value = 0;
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return dt;
            }
            finally
            {
                con.Close();
                da.Dispose();
            }
            return dt;
        }
        //Header Insert
        public DataTable headergstr(FS_GSTRModel gstrobjmodel)
        {
            string result = "";
            //string filename, string filepath
            DataTable dt = new DataTable();
            try
            {
                GetConnection();
                cmd = new SqlCommand("[pr_fs_header_gstrupload]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Filename", SqlDbType.VarChar).Value = gstrobjmodel.filename;
                cmd.Parameters.Add("@Filepath", SqlDbType.VarChar).Value = gstrobjmodel.filepath;
                cmd.Parameters.Add("@loginid", SqlDbType.VarChar).Value = HttpContext.Current.Session["employee_gid"].ToString().Trim();

                cmd.Parameters.Add("@Action", SqlDbType.VarChar).Value = "insert";
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return dt;
            }
            finally
            {
                con.Close();
                da.Dispose();
            }
            return dt;

        }
        public DataTable TDSheadergstr(FS_GSTRModel gstrobjmodel)
        {
            string result = "";
            DataTable dt = new DataTable();
            try
            {
                GetConnection();
                cmd = new SqlCommand("pr_fs_header_tdsgstrupload", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Filename", SqlDbType.VarChar).Value = gstrobjmodel.filename;
                cmd.Parameters.Add("@Headerno", SqlDbType.VarChar).Value = "TDS05062021";
                cmd.Parameters.Add("@HeaderMakerid", SqlDbType.BigInt).Value = HttpContext.Current.Session["employee_gid"].ToString().Trim();
                cmd.Parameters.Add("@loginid", SqlDbType.VarChar).Value = HttpContext.Current.Session["employee_gid"].ToString().Trim();
                cmd.Parameters.Add("@Action", SqlDbType.VarChar).Value = "insert";
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return dt;
            }
            finally
            {
                con.Close();
                da.Dispose();
            }
            return dt;
        }

        public IEnumerable<GSTCreditNote_Model> SelectCreditnoeteSearch(string CreditnoteId)
        {
            List<GSTCreditNote_Model> Inv = new List<GSTCreditNote_Model>();
            try
            {
                GetConnection();
                DataTable dt = new DataTable();
                GSTCreditNote_Model objModel;
                cmd = new SqlCommand("PR_FLEXISPEND_GetGstCreditnoteDetails", con);
                cmd.Parameters.Add("@CreditnoteId", SqlDbType.VarChar).Value = CreditnoteId;
                cmd.CommandType = CommandType.StoredProcedure;

                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    objModel = new GSTCreditNote_Model();
                    //objModel.InvoiceTaxGid = Convert.ToInt32(dt.Rows[i]["invoicetaxgid"].ToString());
                    //objModel.Hsncode = (dt.Rows[i]["HsnCode"].ToString());
                    //objModel.HsnId = Convert.ToInt32(dt.Rows[i]["Hsngid"].ToString());
                    objModel.Subtax = dt.Rows[i]["TaxSubType"].ToString();
                    objModel.TaxableAmt = Convert.ToDecimal(dt.Rows[i]["TaxableAmt"].ToString());
                    objModel.TaxAmt = Convert.ToDecimal(dt.Rows[i]["TaxAmt"].ToString());
                    objModel.GstRate = Convert.ToDecimal(dt.Rows[i]["Rate"].ToString());

                    // objModel.HsnDesc = dt.Rows[i]["HsnDescription"].ToString();

                    //objModel.GstApplicable = dt.Rows[i]["GsnApplicable"].ToString();
                    Inv.Add(objModel);
                }
                return Inv;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return Inv;
            }
            finally
            {
                con.Close();
                da.Dispose();
            }
        }

        public IEnumerable<CygnetSearchModel> SelectInvoiceSearch(CygnetSearchModel data)
        {
            List<CygnetSearchModel> Inv = new List<CygnetSearchModel>();
            try
            {
                GetConnection();
                DataTable dt = new DataTable();

                CygnetSearchModel objModel;
                cmd = new SqlCommand("SP_EOW_Get_CygnetBySearch", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@PanNo", SqlDbType.VarChar).Value = data.Cygnet_SupplierPanNo;
                cmd.Parameters.Add("@SupplierName", SqlDbType.VarChar).Value = data.Cygnet_SupplierName;
                cmd.Parameters.Add("@GSTN", SqlDbType.VarChar).Value = data.Cygnet_Supplier_GSTNNo;
                cmd.Parameters.Add("@InvoiceNo", SqlDbType.VarChar).Value = data.Cygnet_InvoiceNo;
                cmd.Parameters.Add("@Supplier_Gid", SqlDbType.VarChar).Value = data.InvoiceSupplier_gid;
                if (data.Cygnet_InvoiceFromDate != "" && data.Cygnet_InvoiceFromDate != null)
                {
                    cmd.Parameters.Add("@InvoiceFromDate", SqlDbType.VarChar).Value = cmnfun.convertoDateTimeString(data.Cygnet_InvoiceFromDate).ToString();
                }
                else
                {
                    cmd.Parameters.Add("@InvoiceFromDate", SqlDbType.VarChar).Value = data.Cygnet_InvoiceFromDate;
                }

                if (data.Cygnet_InvoiceToDate != "" && data.Cygnet_InvoiceToDate != null)
                {
                    cmd.Parameters.Add("@InvoiceToDate", SqlDbType.VarChar).Value = cmnfun.convertoDateTimeString(data.Cygnet_InvoiceToDate).ToString();
                }
                else
                {
                    cmd.Parameters.Add("@InvoiceToDate", SqlDbType.VarChar).Value = data.Cygnet_InvoiceToDate;
                }

                cmd.Parameters.Add("@Action", SqlDbType.VarChar).Value = "SearchInvoice";
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    objModel = new CygnetSearchModel();
                    objModel.Cygnet_InvoiceNo = row["Cygnet_Invoice_No"].ToString();
                    objModel.Cygnet_InvoiceDate = row["Cygnet_Invoice_Date"].ToString();
                    objModel.Cygnet_TaxableAmt = row["Cygnet_Taxable_Amount"].ToString();
                    objModel.Cygnet_CGSTAmt = row["Cygnet_CGST_Amount"].ToString();
                    objModel.Cygnet_SGSTAmt = row["Cygnet_SGST_Amount"].ToString();
                    objModel.Cygnet_IGSTAmt = row["Cygnet_IGST_Amount"].ToString();
                    objModel.Cygnet_InvoiceAmt = row["Cygnet_Invoice_Amount"].ToString();
                    objModel.Cygnet_Gid = Convert.ToInt64(row["Cygnet_gid"]);
                    objModel.Cygnet_InvoiceType = row["Cygnet_InvoiceType"].ToString();
                    objModel.InvoiceSupplier_gid = row["InvoiceSupplier_gid"].ToString();
                    objModel.Cygnet_Supplier = row["Cygnet_Supplier"].ToString();
                    objModel.Cygnet_Provider = row["Cygnet_Provider_Location_Name"].ToString();
                    objModel.Cygnet_Receiver = row["Cygnet_Receiver_Location_Name"].ToString();
                    objModel.Cygnet_Provider_GSTN = row["Cygnet_Supplier_GSTN_No"].ToString();
                    objModel.Cygnet_Receiver_GSTN = row["Cygnet_Receiver_GSTN_No"].ToString();

                    Inv.Add(objModel);
                }
                return Inv;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return Inv;
            }
            finally
            {
                con.Close();
                da.Dispose();
            }
        }


        //

        //selva Populate GST Grid

        //public IEnumerable<GSTCreditNote_Model> SelectCreditnoeteSearch(string Creditnotegid)
        //{
        //    List<GSTCreditNote_Model> Inv = new List<GSTCreditNote_Model>();
        //    try
        //    {
        //        GetConnection();
        //        DataTable dt = new DataTable();
        //        GSTCreditNote_Model objModel;
        //        cmd = new SqlCommand("PR_FLEXISPEND_GetGstCreditnoteDetails", con);
        //        cmd.Parameters.Add("@CreditnoteId", SqlDbType.VarChar).Value = Creditnotegid;
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        da = new SqlDataAdapter(cmd);
        //        da.Fill(dt);
        //        for (int i = 0; i < dt.Rows.Count; i++)
        //        {
        //            objModel = new GSTCreditNote_Model();
        //            objModel.Subtax = dt.Rows[i]["TaxSubType"].ToString();
        //            objModel.TaxableAmt = Convert.ToDecimal(dt.Rows[i]["TaxableAmt"].ToString());
        //            objModel.TaxAmt = Convert.ToDecimal(dt.Rows[i]["TaxAmt"].ToString());
        //            objModel.GstRate = Convert.ToDecimal(dt.Rows[i]["Rate"].ToString());
        //            Inv.Add(objModel);
        //        }
        //        return Inv;
        //    }
        //    catch (Exception ex)
        //    {
        //        objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
        //        return Inv;
        //    }
        //    finally
        //    {
        //        con.Close();
        //        da.Dispose();
        //    }
        //}

        //public IEnumerable<GSTCreditNote_Model> SelectCreditnoeteSearch(string Creditnotegid)
        //{
        //    List<GSTCreditNote_Model> Inv = new List<GSTCreditNote_Model>();
        //    try
        //    {
        //        GetConnection();
        //        DataTable dt = new DataTable();
        //        GSTCreditNote_Model objModel;
        //        cmd = new SqlCommand("PR_FLEXISPEND_GetGstCreditnoteDetails", con);
        //        cmd.Parameters.Add("@CreditnoteId", SqlDbType.VarChar).Value = Creditnotegid;
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        da = new SqlDataAdapter(cmd);
        //        da.Fill(dt);
        //        for (int i = 0; i < dt.Rows.Count; i++)
        //        {
        //            objModel = new GSTCreditNote_Model();
        //            objModel.Subtax = dt.Rows[i]["TaxSubType"].ToString();
        //            objModel.TaxableAmt = Convert.ToDecimal(dt.Rows[i]["TaxableAmt"].ToString());
        //            objModel.TaxAmt = Convert.ToDecimal(dt.Rows[i]["TaxAmt"].ToString());
        //            objModel.GstRate = Convert.ToDecimal(dt.Rows[i]["Rate"].ToString());
        //            Inv.Add(objModel);
        //        }
        //        return Inv;
        //    }
        //    catch (Exception ex)
        //    {
        //        objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
        //        return Inv;
        //    }
        //    finally
        //    {
        //        con.Close();
        //        da.Dispose();
        //    }
        //}
        ////


        // Credit Note Maker invoice cygent search

        public IEnumerable<CygnetSearchModel> SelectCrdditNoteInvoiceSearch(CygnetSearchModel data)
        {
            List<CygnetSearchModel> Inv = new List<CygnetSearchModel>();
            try
            {
                GetConnection();
                DataTable dt = new DataTable();

                CygnetSearchModel objModel;
                cmd = new SqlCommand("SP_EOW_Get_CreditNoteCygnetBySearch", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@PanNo", SqlDbType.VarChar).Value = data.Cygnet_SupplierPanNo;
                cmd.Parameters.Add("@SupplierName", SqlDbType.VarChar).Value = data.Cygnet_SupplierName;
                cmd.Parameters.Add("@GSTN", SqlDbType.VarChar).Value = data.Cygnet_Supplier_GSTNNo;
                cmd.Parameters.Add("@InvoiceNo", SqlDbType.VarChar).Value = data.Cygnet_InvoiceNo;
                cmd.Parameters.Add("@Supplier_Gid", SqlDbType.VarChar).Value = data.InvoiceSupplier_gid;
                if (data.Cygnet_InvoiceFromDate != "" && data.Cygnet_InvoiceFromDate != null)
                {
                    cmd.Parameters.Add("@InvoiceFromDate", SqlDbType.VarChar).Value = cmnfun.convertoDateTimeString(data.Cygnet_InvoiceFromDate).ToString();
                }
                else
                {
                    cmd.Parameters.Add("@InvoiceFromDate", SqlDbType.VarChar).Value = data.Cygnet_InvoiceFromDate;
                }

                if (data.Cygnet_InvoiceToDate != "" && data.Cygnet_InvoiceToDate != null)
                {
                    cmd.Parameters.Add("@InvoiceToDate", SqlDbType.VarChar).Value = cmnfun.convertoDateTimeString(data.Cygnet_InvoiceToDate).ToString();
                }
                else
                {
                    cmd.Parameters.Add("@InvoiceToDate", SqlDbType.VarChar).Value = data.Cygnet_InvoiceToDate;
                }

                cmd.Parameters.Add("@Action", SqlDbType.VarChar).Value = "SearchInvoice";
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    objModel = new CygnetSearchModel();
                    objModel.Cygnet_InvoiceNo = row["Cygnet_Invoice_No"].ToString();
                    objModel.Cygnet_InvoiceDate = row["Cygnet_Invoice_Date"].ToString();
                    objModel.Cygnet_TaxableAmt = row["Cygnet_Taxable_Amount"].ToString();
                    objModel.Cygnet_CGSTAmt = row["Cygnet_CGST_Amount"].ToString();
                    objModel.Cygnet_SGSTAmt = row["Cygnet_SGST_Amount"].ToString();
                    objModel.Cygnet_IGSTAmt = row["Cygnet_IGST_Amount"].ToString();
                    objModel.Cygnet_InvoiceAmt = row["Cygnet_Invoice_Amount"].ToString();
                    objModel.Cygnet_Gid = Convert.ToInt64(row["Cygnet_gid"]);
                    objModel.Cygnet_InvoiceType = row["Cygnet_InvoiceType"].ToString();
                    objModel.InvoiceSupplier_gid = row["InvoiceSupplier_gid"].ToString();
                    objModel.Cygnet_Supplier = row["Cygnet_Supplier"].ToString();
                    objModel.Cygnet_Provider = row["Cygnet_Provider_Location_Name"].ToString();
                    objModel.Cygnet_Receiver = row["Cygnet_Receiver_Location_Name"].ToString();
                    objModel.Cygnet_Provider_GSTN = row["Cygnet_Supplier_GSTN_No"].ToString();
                    objModel.Cygnet_Receiver_GSTN = row["Cygnet_Receiver_GSTN_No"].ToString();

                    Inv.Add(objModel);
                }
                return Inv;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return Inv;
            }
            finally
            {
                con.Close();
                da.Dispose();
            }
        }







        //Credit Note GST Maker
        // DropDown Binding HSN Code
        public IEnumerable<GSTCreditNote_Model> Get_CreditnoteHsnDropdown()
        {
            List<GSTCreditNote_Model> objholdq = new List<GSTCreditNote_Model>();
            try
            {
                GSTCreditNote_Model objModel;
                GetConnection();
                DataTable dt = new DataTable();
                cmd = new SqlCommand("[PR_FS_Get_CreditNoteHsnNo]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@action", SqlDbType.VarChar).Value = "Get";
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    objModel = new GSTCreditNote_Model();
                    objModel.HsnId = Convert.ToInt32(dt.Rows[i]["HsnId"].ToString());
                    objModel.HsnCode = Convert.ToString(dt.Rows[i]["HsnCode"].ToString());
                    objholdq.Add(objModel);
                }
                return objholdq;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return objholdq;
            }
            finally
            {
                con.Close();
                da.Dispose();
            }
        }

        //Get_RecevierLocationDropdown


        public IEnumerable<GSTCreditNote_Model> Get_RecevierLocationDropdown()
        {
            List<GSTCreditNote_Model> objholdq = new List<GSTCreditNote_Model>();
            try
            {
                GSTCreditNote_Model objModel;
                GetConnection();
                DataTable dt = new DataTable();
                cmd = new SqlCommand("[PR_FS_Get_CreditNoteHsnNo]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@action", SqlDbType.VarChar).Value = "ReceiverLocation";
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    objModel = new GSTCreditNote_Model();
                    objModel.ReceiverLocationid = Convert.ToInt32(dt.Rows[i]["state_gid"].ToString());
                    objModel.receiverlocationName = Convert.ToString(dt.Rows[i]["Statename"].ToString());
                    objholdq.Add(objModel);
                }
                return objholdq;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return objholdq;
            }
            finally
            {
                con.Close();
                da.Dispose();
            }
        }
        //Get Provider Location
        public IEnumerable<GSTCreditNote_Model> Get_ProviderLocationDropdown()
        {
            List<GSTCreditNote_Model> objholdq = new List<GSTCreditNote_Model>();
            try
            {
                GSTCreditNote_Model objModel;
                GetConnection();
                DataTable dt = new DataTable();
                cmd = new SqlCommand("[PR_FS_Get_CreditNoteHsnNo]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@action", SqlDbType.VarChar).Value = "ProviderLocation";
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    objModel = new GSTCreditNote_Model();
                    objModel.ProviderLocationid = Convert.ToInt32(dt.Rows[i]["state_gid"].ToString());
                    objModel.Cygnet_Provider_Location_Name = Convert.ToString(dt.Rows[i]["Statename"].ToString());
                    objholdq.Add(objModel);
                }
                return objholdq;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return objholdq;
            }
            finally
            {
                con.Close();
                da.Dispose();
            }
        }

        //vadivu 08.12.2020

        //GSTHOLD Q summary
        public IEnumerable<GSTHoldQ> GetHoldQ()
        {
            List<GSTHoldQ> emp = new List<GSTHoldQ>();
            try
            {

                GSTHoldQ objModel;
                GetConnection();
                cmd = new SqlCommand("[PR_FS_Get_GSTHoldQ]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@ACTION", SqlDbType.VarChar).Value = "Get";
                da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                HttpContext.Current.Session["GSTHoldQ"] = dt;
                int i = 1;
                foreach (DataRow row in dt.Rows)
                {
                    objModel = new GSTHoldQ();
                    // objModel.ecfId = Convert.ToInt32(row["ecfId"].ToString());
                    objModel.ecfId = (row["ecfId"].ToString());
                    objModel.DocNo = row["DocNo"].ToString();
                    // objModel.SupplierId = Convert.ToInt32(row["centralinward_supplier_gid"].ToString());
                    objModel.Ecf_Date = row["DueDate"].ToString();
                    objModel.SupplierEmployee = row["Name"].ToString();
                    objModel.Raiser = row["Raiser"].ToString();
                    objModel.Ecf_Gst_Amount = cmnfun.GetINRAmount(row["GSTAmount"].ToString());
                    objModel.Ecf_Base_Amount = cmnfun.GetINRAmount(row["BaseAmount"].ToString());
                    objModel.ClaimType = row["ClaimType"].ToString();
                    objModel.fccode = row["fccode"].ToString();
                    objModel.Ecf_Amount = cmnfun.GetINRAmount(row["DocAmount"].ToString());
                    objModel.ecf_docsubtype_gid = row["ecf_docsubtype_gid"].ToString();
                    objModel.GSTNStatus = row["GSTNStatus"].ToString();
                    objModel.RasierDept = row["Rasier_Dept"].ToString();
                    objModel.inVid = (row["invoice_gid"].ToString());
                    objModel.inVno = (row["invoice_no"].ToString());
                    objModel.inVDate = (row["InvoiceDate"].ToString());
                    objModel.inVamT = (row["invoice_amount"].ToString());
                    emp.Add(objModel);
                    i = i + 1;
                }
                return emp;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return emp;
            }
            finally
            {
                con.Close();
            }
        }

      


        //GST Hold Q dropdown
        public IEnumerable<GSTHoldQ> Get_HoldQDropdown()
        {
            List<GSTHoldQ> objholdq = new List<GSTHoldQ>();
            try
            {
                GSTHoldQ objModel;
                GetConnection();
                DataTable dt = new DataTable();
                cmd = new SqlCommand("[PR_FS_Get_GSTHoldQDropdown]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@action", SqlDbType.VarChar).Value = "Get";
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    objModel = new GSTHoldQ();
                    objModel.SunkCostGid = Convert.ToInt32(dt.Rows[i]["employee_gid"].ToString());
                    objModel.SunkCostName = Convert.ToString(dt.Rows[i]["empcodename"].ToString());
                    objholdq.Add(objModel);
                }
                return objholdq;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return objholdq;
            }
            finally
            {
                con.Close();
                da.Dispose();
            }
        }

        //HOLD Q Save
        public string Save_HoldQList(GSTHoldQ objmdl)
        {
            try
            {
                DataTable dt = new DataTable();
                string Emp_Msg = "";
                DataTable dtout = new DataTable();

                GetConnection();
                cmd = new SqlCommand("[PR_FS_Set_HoldQ]", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ecfgid", SqlDbType.VarChar).Value = objmdl.ecfId;
                cmd.Parameters.Add("@queueto", SqlDbType.Int).Value = objmdl.SunkCostGid;
                cmd.Parameters.Add("@queuefrom", SqlDbType.Int).Value = HttpContext.Current.Session["employee_gid"].ToString().Trim();


                da = new SqlDataAdapter(cmd);
                da.Fill(dtout);
                if (dtout.Rows.Count > 0)
                {
                    Emp_Msg = dtout.Rows[0]["message"].ToString();

                    // string clear = dtout.Rows[0]["Clear"].ToString();

                    if (Emp_Msg.ToLower() == "GST HoldQ Updated successfully!")
                    {
                        Emp_Msg = "GST HoldQ Updated successfully!";
                    }


                }

                return Emp_Msg;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return "";
            }
            finally
            {
                con.Close();
                da.Dispose();
            }
        }
        //Sunk Cost Approval

        public IEnumerable<GSTHoldQ> Getsunkcost()
        {
            List<GSTHoldQ> emp = new List<GSTHoldQ>();
            try
            {

                GSTHoldQ objModel;
                GetConnection();
                cmd = new SqlCommand("[PR_FS_Get_SunkCostApproval]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@ACTION", SqlDbType.VarChar).Value = "Get";
                cmd.Parameters.Add("@loginid", SqlDbType.Int).Value = HttpContext.Current.Session["employee_gid"].ToString().Trim();

                da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                int i = 1;
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Session["SunkcostQ"] = dt;
                }
                foreach (DataRow row in dt.Rows)
                {
                    objModel = new GSTHoldQ();
                    // objModel.ecfId = Convert.ToInt32(row["ecfId"].ToString());
                    objModel.ecfId = (row["ecfId"].ToString());
                    objModel.DocNo = row["DocNo"].ToString();
                    // objModel.SupplierId = Convert.ToInt32(row["centralinward_supplier_gid"].ToString());
                    objModel.Ecf_Date = row["DueDate"].ToString();
                    objModel.SupplierEmployee = row["Name"].ToString();
                    objModel.Raiser = row["Raiser"].ToString();
                    objModel.Ecf_Gst_Amount = cmnfun.GetINRAmount(row["GSTAmount"].ToString());
                    objModel.Ecf_Base_Amount = cmnfun.GetINRAmount(row["BaseAmount"].ToString());
                    objModel.ClaimType = row["ClaimType"].ToString();
                    // objModel.fccode = row["fccode"].ToString();
                    objModel.Ecf_Amount = cmnfun.GetINRAmount(row["DocAmount"].ToString());
                    objModel.ecf_docsubtype_gid = row["ecf_docsubtype_gid"].ToString();
                    objModel.GSTNStatus = row["GSTNStatus"].ToString();
                    emp.Add(objModel);
                    i = i + 1;
                }
                return emp;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return emp;
            }
            finally
            {
                con.Close();
            }
        }


        //Save Sunkcost
        public string Save_Sunckcost(GSTHoldQ objmdl)
        {
            try
            {
                DataTable dt = new DataTable();
                string Emp_Msg = "";
                DataTable dtout = new DataTable();

                GetConnection();
                cmd = new SqlCommand("[PR_FS_Set_SunckcostApproval]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0; // pandi & ramya modified on 12 may 22
                cmd.Parameters.Add("@ecfgid", SqlDbType.VarChar).Value = objmdl.ecfId;
                cmd.Parameters.Add("@remark", SqlDbType.VarChar).Value = objmdl.Remark;
                cmd.Parameters.Add("@loginid", SqlDbType.VarChar).Value = HttpContext.Current.Session["employee_gid"].ToString().Trim();
                //cmd.Parameters.Add("@queueto", SqlDbType.Int).Value = objmdl.SunkCostGid;
                //cmd.Parameters.Add("@queuefrom", SqlDbType.Int).Value = HttpContext.Current.Session["employee_gid"].ToString().Trim();


                da = new SqlDataAdapter(cmd);
                da.Fill(dtout);
                if (dtout.Rows.Count > 0)
                {
                    Emp_Msg = dtout.Rows[0]["message"].ToString();

                     string clear = dtout.Rows[0]["Clear"].ToString();
                     Emp_Msg = clear;
                     

                }

                return Emp_Msg;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return "";
            }
            finally
            {
                con.Close();
                da.Dispose();
            }
        }

        //update forfeiture

        public string Save_forfeiture(GSTHoldQ objmdl)
        {
            try
            {
                DataTable dt = new DataTable();
                string Emp_Msg = "";
                DataTable dtout = new DataTable();

                GetConnection();
                cmd = new SqlCommand("[PR_FS_Set_Forfeiture]", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ecfgid", SqlDbType.VarChar).Value = objmdl.ecfId;
                cmd.Parameters.Add("@remark", SqlDbType.VarChar).Value = objmdl.Remark;
                //cmd.Parameters.Add("@queueto", SqlDbType.Int).Value = objmdl.SunkCostGid;
                cmd.Parameters.Add("@queuefrom", SqlDbType.Int).Value = HttpContext.Current.Session["employee_gid"].ToString().Trim();


                da = new SqlDataAdapter(cmd);
                da.Fill(dtout);
                if (dtout.Rows.Count > 0)
                {
                    Emp_Msg = dtout.Rows[0]["message"].ToString();

                    // string clear = dtout.Rows[0]["Clear"].ToString();

                    string clear = dtout.Rows[0]["Clear"].ToString();
                    Emp_Msg = clear;


                    //if (Emp_Msg.ToLower() == "GST Forfeiture Updated successfully!")
                    //{
                    //    Emp_Msg = "GST Forfeiture Updated successfully!";
                    //}
                }

                return Emp_Msg;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return "";
            }
            finally
            {
                con.Close();
                da.Dispose();
            }
        }


        // Forfeiture Summary
        public IEnumerable<GSTHoldQ> GetForfeiture()
        {
            List<GSTHoldQ> emp = new List<GSTHoldQ>();
            try
            {

                GSTHoldQ objModel;
                GetConnection();
                cmd = new SqlCommand("[PR_FS_Get_Forfeiture]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                ///   cmd.Parameters.Add("@ACTION", SqlDbType.VarChar).Value = "Get";
                cmd.Parameters.Add("@loginid", SqlDbType.Int).Value = HttpContext.Current.Session["employee_gid"].ToString().Trim();

                da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    HttpContext.Current.Session["ForeFietureQ"] = dt;
                }
                int i = 1;
                foreach (DataRow row in dt.Rows)
                {
                    objModel = new GSTHoldQ();
                    // objModel.ecfId = Convert.ToInt32(row["ecfId"].ToString());
                    objModel.ecfId = (row["ecfId"].ToString());
                    objModel.DocNo = row["DocNo"].ToString();
                    // objModel.SupplierId = Convert.ToInt32(row["centralinward_supplier_gid"].ToString());
                    objModel.Ecf_Date = row["DueDate"].ToString();
                    objModel.SupplierEmployee = row["Name"].ToString();
                    objModel.Raiser = row["Raiser"].ToString();
                    objModel.Ecf_Gst_Amount = cmnfun.GetINRAmount(row["GSTAmount"].ToString());
                    objModel.Ecf_Base_Amount = cmnfun.GetINRAmount(row["BaseAmount"].ToString());
                    objModel.ClaimType = row["ClaimType"].ToString();
                    // objModel.fccode = row["fccode"].ToString();
                    objModel.Ecf_Amount = cmnfun.GetINRAmount(row["DocAmount"].ToString());
                    objModel.ecf_docsubtype_gid = row["ecf_docsubtype_gid"].ToString();
                    objModel.GSTNStatus = row["GSTNStatus"].ToString();
                    emp.Add(objModel);
                    i = i + 1;
                }
                return emp;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return emp;
            }
            finally
            {
                con.Close();
            }
        }

        //selva 28-12-2020 
        public DataTable GetCygnetSearchInvDetailsCount(CygnetSearchModel cygmodels)
        {


            GetConnection();

            DataTable dt = new DataTable();

            // CygnetSearchModel objModel;
            //cmd = new SqlCommand("SP_EOW_Get_CygnetBySearch", con); //Ramya commentted
            cmd = new SqlCommand("SP_FS_Get_CygnetBySearch", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@PanNo", SqlDbType.VarChar).Value = cygmodels.Cygnet_SupplierPanNo;
            cmd.Parameters.Add("@SupplierName", SqlDbType.VarChar).Value = cygmodels.Cygnet_SupplierName;
            cmd.Parameters.Add("@GSTN", SqlDbType.VarChar).Value = cygmodels.Cygnet_Supplier_GSTNNo;
            cmd.Parameters.Add("@InvoiceNo", SqlDbType.VarChar).Value = cygmodels.Cygnet_InvoiceNo;
            cmd.Parameters.Add("@Supplier_Gid", SqlDbType.VarChar).Value = cygmodels.InvoiceSupplier_gid;
            cmd.Parameters.Add("@invoice_gid", SqlDbType.VarChar).Value = cygmodels.Cygnet_invoice_gid;
            if (cygmodels.Cygnet_InvoiceFromDate != "" && cygmodels.Cygnet_InvoiceFromDate != null)
            {
                cmd.Parameters.Add("@InvoiceFromDate", SqlDbType.VarChar).Value = objCmnFunctions.convertoDateTimeString(cygmodels.Cygnet_InvoiceFromDate).ToString();
            }
            else
            {
                cmd.Parameters.Add("@InvoiceFromDate", SqlDbType.VarChar).Value = cygmodels.Cygnet_InvoiceFromDate;
            }

            if (cygmodels.Cygnet_InvoiceToDate != "" && cygmodels.Cygnet_InvoiceToDate != null)
            {
                cmd.Parameters.Add("@InvoiceToDate", SqlDbType.VarChar).Value = objCmnFunctions.convertoDateTimeString(cygmodels.Cygnet_InvoiceToDate).ToString();
            }
            else
            {
                cmd.Parameters.Add("@InvoiceToDate", SqlDbType.VarChar).Value = cygmodels.Cygnet_InvoiceToDate;
            }

            cmd.Parameters.Add("@Action", SqlDbType.VarChar).Value = "SearchInvoice";
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            //if (dt.Rows.Count > 0)
            //{
            //    return dt;
            //}
            return dt;
        }

        public string GetTDSUploadStatusexcel(string valatate, string valatate1, string valatate2, string action, string SupplierCode)
        {
            string result = "";
            try
            {
                GetConnection();
                cmd = new SqlCommand("pr_asms_tdsupload_excelvalate", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@chkdata1", SqlDbType.VarChar).Value = valatate.ToString();
                cmd.Parameters.Add("@Result", SqlDbType.VarChar).Value = action;
                cmd.Parameters.Add("@SupplierCode", SqlDbType.VarChar).Value = SupplierCode;
                result = (string)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return "";
            }
            finally
            {
                con.Close();
                da.Dispose();
            }
            return result;
        }
        public IEnumerable<EOW_SupplierModelgridM> GetSupplierdata(string ecfid, string invoiceid, string traveltype)
        {
            List<EOW_SupplierModelgridM> objExpense = new List<EOW_SupplierModelgridM>();
            try
            {

                EOW_SupplierModelgridM objModel;
                DataTable dt = new DataTable();
                GetConnection();
                cmd = new SqlCommand("pr_eow_com_invoicedetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@ecf_gid", SqlDbType.VarChar).Value = ecfid;
                cmd.Parameters.Add("@invoice_type", SqlDbType.VarChar).Value = traveltype;
                cmd.Parameters.Add("@action", SqlDbType.VarChar).Value = "invoiceecfbase";
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    objModel = new EOW_SupplierModelgridM();
                    objModel.Invoicegid = Convert.ToInt32(dt.Rows[i]["invoice_gid"].ToString());
                    objModel.InvoiceDate = Convert.ToString(dt.Rows[i]["invoice_date"].ToString());
                    objModel.InvoiceNo = Convert.ToString(dt.Rows[i]["invoice_no"].ToString());
                    objModel.Description = Convert.ToString(dt.Rows[i]["invoice_desc"].ToString());
                    objModel.Amount = Convert.ToString(dt.Rows[i]["invoice_amount"].ToString());
                    string Provision = Convert.ToString(dt.Rows[i]["invoice_provision_flag"].ToString());
                    if (Provision == "N")
                    {
                        Provision = "No";
                    }
                    if (Provision == "Y")
                    {
                        Provision = "Yes";
                    }
                    objModel.Provision = Provision;
                    objModel.InvoiceReceiptDate = Convert.ToString(dt.Rows[i]["invoice_receipt_date"].ToString());
                    objModel.ReasonForDelay = Convert.ToString(dt.Rows[i]["reason_for_delay"].ToString());
                    objExpense.Add(objModel);
                }
                return objExpense;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return objExpense;
            }
            finally
            {
                con.Close();
                da.Dispose();
            }
        }
        public string TDSApproveRejectedUpload(string remarks, string id, string action)
        {
            try
            {
                DataTable dt = new DataTable();
                string Emp_Msg = "";
                GetConnection();
                cmd = new SqlCommand("pr_asms_header_tdscheckerrejected", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@TDSHeadergid", SqlDbType.VarChar).Value = id;
                cmd.Parameters.Add("@Action", SqlDbType.VarChar).Value = action;
                cmd.Parameters.Add("@loginid", SqlDbType.VarChar).Value = HttpContext.Current.Session["employee_gid"].ToString().Trim();
                cmd.Parameters.Add("@Remarks", SqlDbType.VarChar).Value = remarks;
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    Emp_Msg = dt.Rows[0]["Msg"].ToString();
                }
                return Emp_Msg;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return "";
            }
            finally
            {
                con.Close();
                da.Dispose();
            }
        }
        public string TDSExcelGSTRUpload(DataTable dt, FS_GSTRModel gstrobjmodel)
        {
            try
            {
                string Emp_Msg = "";
                DataTable dtout = new DataTable();
                DataTable dt1 = new DataTable();
                int Gid = 0;
                dt1 = TDSheadergstr(gstrobjmodel);
                Gid = Convert.ToInt32(dt1.Rows[0][1].ToString());
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    GetConnection();
                    cmd = new SqlCommand("pr_fs_details_tdsgstrupload", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@TDSHeadergid", SqlDbType.VarChar).Value = Gid;
                    cmd.Parameters.Add("@TDSSupplier", SqlDbType.VarChar).Value = string.IsNullOrEmpty(dt.Rows[i][0].ToString()) ? "" : Convert.ToString(dt.Rows[i][0].ToString());
                    cmd.Parameters.Add("@PANNo", SqlDbType.VarChar).Value = string.IsNullOrEmpty(dt.Rows[i][1].ToString()) ? "" : Convert.ToString(dt.Rows[i][1].ToString());
                    cmd.Parameters.Add("@TaxCode", SqlDbType.VarChar).Value = string.IsNullOrEmpty(dt.Rows[i][2].ToString()) ? "" : Convert.ToString(dt.Rows[i][2].ToString());
                    cmd.Parameters.Add("@Taxsubtypecode", SqlDbType.VarChar).Value = string.IsNullOrEmpty(dt.Rows[i][3].ToString()) ? "" : Convert.ToString(dt.Rows[i][3].ToString());
                    cmd.Parameters.Add("@TaxRate", SqlDbType.Decimal).Value = string.IsNullOrEmpty(dt.Rows[i][4].ToString()) ? 0 : Convert.ToDecimal(dt.Rows[i][4].ToString());
                    cmd.Parameters.Add("@loginid", SqlDbType.VarChar).Value = HttpContext.Current.Session["employee_gid"].ToString().Trim();
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dtout);
                    if (dtout.Rows.Count > 0)
                    {
                        Emp_Msg = dtout.Rows[0]["Msg"].ToString();
                        string clear = dtout.Rows[0]["Clear"].ToString();
                        Emp_Msg = string.Concat(dtout.Rows[0]["Msg"].ToString() + ';' + (dtout.Rows[0]["headergid"].ToString()));
                    }
                }
                return Emp_Msg;
            }
            catch (Exception ex)
            {
                objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return "";
            }
            finally
            {
                con.Close();
                da.Dispose();
            }
        }
    }
}