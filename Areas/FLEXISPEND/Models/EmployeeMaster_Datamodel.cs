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
    public class EmployeeMaster_Datamodel : EmployeeRepository
    {
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

        //Employee master Getsummary list
        public List<EmployeeMaster_Model> GetEmployeeList(string action)
        {
            try
            {
                List<EmployeeMaster_Model> objOrgType = new List<EmployeeMaster_Model>();
                EmployeeMaster_Model objModel;
                GetConnection();
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand("FS_Get_Employeemaster", con);
               // cmd.Parameters.AddWithValue("@suppliergst_supplierheader_gid", Convert.ToInt64(HttpContext.Current.Session["SupplierHeaderGid"]));
                cmd.Parameters.AddWithValue("@action", action);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    objModel = new EmployeeMaster_Model();

                    objModel.Employee_gid = Convert.ToInt32(dt.Rows[i]["employee_gid"]);
                    objModel.Employee_Code = dt.Rows[i]["employee_code"].ToString();
                    objModel.Employee_Name = dt.Rows[i]["employee_name"].ToString();
                    objModel.Employee_dob = dt.Rows[i]["employee_dob"].ToString();
                    objModel.Employee_doj = dt.Rows[i]["employee_doj"].ToString();
                    objModel.Employee_designation = dt.Rows[i]["employee_iem_designation"].ToString();
                    objModel.Employee_hris_designation = dt.Rows[i]["employee_hris_designation"].ToString();
                    objModel.Employee_grade_code = dt.Rows[i]["employee_grade_code"].ToString();
                    objModel.Employee_office_email = dt.Rows[i]["employee_office_email"].ToString();
                    objModel.Employee_mobile_no = dt.Rows[i]["employee_mobile_no"].ToString();
                    objModel.Employee_era_bankcode = dt.Rows[i]["employee_era_bank_code"].ToString();
                    objModel.Employee_account_no = dt.Rows[i]["employee_era_acc_no"].ToString();
                    objModel.Employee_ifsc_code = dt.Rows[i]["employee_era_ifsc_code"].ToString();
                    objModel.Employee_supervisor = dt.Rows[i]["supervisorname"].ToString();
                    objModel.Employee_status = dt.Rows[i]["employee_status"].ToString(); 
                   
                    objOrgType.Add(objModel);
                }
                return objOrgType;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
        }

        //Employee Master Details with id
        public EmployeeMaster_Model GetEmployeeDetailbyid(int id)
        {
            EmployeeMaster_Model objModel = new EmployeeMaster_Model();

            try
            {
                List<EmployeeMaster_Model> objOrgType = new List<EmployeeMaster_Model>();
               
                GetConnection();
                DataTable dt = new DataTable();

                SqlCommand cmd = new SqlCommand("FS_Get_EmployeeDetailsbyId", con);
                 
                cmd.Parameters.AddWithValue("@Employee_id", id);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    objModel.Employee_gid = Convert.ToInt32(dt.Rows[i]["employee_gid"]);
                    objModel.Employee_Code = dt.Rows[i]["employee_code"].ToString();
                    objModel.Employee_Name = dt.Rows[i]["employee_name"].ToString();
                    objModel.Employee_dob = dt.Rows[i]["employee_dob"].ToString();
                    objModel.Employee_doj = dt.Rows[i]["employee_doj"].ToString();
                    objModel.Employee_designation = dt.Rows[i]["employee_iem_designation"].ToString();
                    objModel.Employee_hris_designation = dt.Rows[i]["employee_hris_designation"].ToString();
                    objModel.Employee_grade_code = dt.Rows[i]["employee_grade_code"].ToString();
                    objModel.Employee_office_email = dt.Rows[i]["employee_office_email"].ToString();
                    objModel.Employee_mobile_no = dt.Rows[i]["employee_mobile_no"].ToString();
                    objModel.Employee_era_bankcode = dt.Rows[i]["employee_era_bank_code"].ToString();
                    objModel.Employee_account_no = dt.Rows[i]["employee_era_acc_no"].ToString();
                    objModel.Employee_ifsc_code = dt.Rows[i]["employee_era_ifsc_code"].ToString();
                    objModel.Employee_supervisor = dt.Rows[i]["supervisorname"].ToString();
                    objModel.Employee_status = dt.Rows[i]["employee_status"].ToString();
                    objModel.Employee_iswrlupdreq = dt.Rows[i]["employee_iswklupdatereq"].ToString(); 
                  }

                return objModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
        }
            // Employee Master grade update

        public string Employee_Grade_Edit(string employeegid, string employeegrade, string iswrklupdreq)
            {
                 string Emp_Msg = "";
                GetConnection();
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand("FS_EmployeeGradeUpdate", con);
                cmd.Parameters.AddWithValue("@Employee_Gid", employeegid);
               cmd.Parameters.AddWithValue("@Grade_Name", employeegrade);
               cmd.Parameters.AddWithValue("@iswrklupdreq", iswrklupdreq);
            
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                 if (dt.Rows.Count > 0)
                    {
                       Emp_Msg = dt.Rows[0]["msg"].ToString();
                 }

                 Emp_Msg = "Success";
                return Emp_Msg;
            }


            
        }

    }
 