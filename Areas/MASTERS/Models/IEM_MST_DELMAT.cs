using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using IEM.Common;
using System.Data;
using System.IO;


namespace IEM.Areas.MASTERS.Models
{

    public class IEM_MST_DELMAT : Iiem_mst_delmat
    {
        SqlConnection con = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter();
        CmnFunctions objCmnFunctions = new CmnFunctions();
        CommonIUD objCommonIUD = new CommonIUD();
        private void GetCon()
        {
            if (con.State == ConnectionState.Closed)
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["connectionstring"].ToString();
                con.Open();
            }
        }
        public string HoldFileUploadUrl()
        {
            string x = "";
            try
            {
                x = System.Configuration.ConfigurationManager.AppSettings["HoldFileUpload"].ToString();
            }
            catch { x = ""; }
            return (x == null || x.Trim() == "") ? "" : x;
        }
        public IEnumerable<iem_mst_delmat> getMakerdelmat()
        {
            try
            {
                List<iem_mst_delmat> objOrgType = new List<iem_mst_delmat>();
                iem_mst_delmat objModel;
                GetCon();
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand("pr_iem_mst_delmat", con);
                cmd.Parameters.AddWithValue("@action", "selectDelmat");
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    objModel = new iem_mst_delmat();
                    objModel.delmat_gid = Convert.ToInt32(dt.Rows[i]["delmat_gid"].ToString());
                    objModel.delmat_name = Convert.ToString(dt.Rows[i]["delmat_name"].ToString());
                    //add delmat summary list
                    objModel.Doc_Type = Convert.ToString(dt.Rows[i]["delmattype_name"].ToString());
                    objModel.Slab = Convert.ToString(dt.Rows[i]["slab_name"].ToString());
                    objModel.Branch = Convert.ToString(dt.Rows[i]["Branch"].ToString());
                    objModel.BranchType = Convert.ToString(dt.Rows[i]["BranchType"].ToString());
                    objModel.Claim_Type = Convert.ToString(dt.Rows[i]["delmat_claim_flag"].ToString());
                    objModel.Dsa_Flag = Convert.ToString(dt.Rows[i]["DSA"].ToString());
                    objModel.Requestfor = Convert.ToString(dt.Rows[i]["Pipit"].ToString());
                    objModel.IT = Convert.ToString(dt.Rows[i]["IT"].ToString());
                    objModel.Expenditure = Convert.ToString(dt.Rows[i]["Expenture"].ToString());
                    objModel.Budget = Convert.ToString(dt.Rows[i]["Budget"].ToString());
                    objModel.Jump = Convert.ToString(dt.Rows[i]["Jump"].ToString());
                    objModel.Status = Convert.ToString(dt.Rows[i]["Active"].ToString());
                    objModel.delmat_Actionstatus = Convert.ToString(dt.Rows[i]["Actionstatus"].ToString()); //ramya added on 11 Apr 22
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

            //throw new NotImplementedException();
        }
        public IEnumerable<iem_mst_delmat> getCheckerdelmat()
        {
            try
            {
                List<iem_mst_delmat> objOrgType = new List<iem_mst_delmat>();
                iem_mst_delmat objModel;
                GetCon();
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand("pr_iem_mst_delmat", con);
                cmd.Parameters.AddWithValue("@action", "DelmatChecker");
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    objModel = new iem_mst_delmat();
                    objModel.delmat_gid = Convert.ToInt32(dt.Rows[i]["delmat_gid"].ToString());
                    objModel.delmat_name = Convert.ToString(dt.Rows[i]["delmat_name"].ToString());
                    //add delmat summary list
                    objModel.Doc_Type = Convert.ToString(dt.Rows[i]["delmattype_name"].ToString());
                    objModel.Slab = Convert.ToString(dt.Rows[i]["slab_name"].ToString());
                    objModel.Branch = Convert.ToString(dt.Rows[i]["Branch"].ToString());
                    objModel.BranchType = Convert.ToString(dt.Rows[i]["BranchType"].ToString());
                    objModel.Claim_Type = Convert.ToString(dt.Rows[i]["delmat_claim_flag"].ToString());
                    objModel.Dsa_Flag = Convert.ToString(dt.Rows[i]["DSA"].ToString());
                    objModel.Requestfor = Convert.ToString(dt.Rows[i]["Pipit"].ToString());
                    objModel.IT = Convert.ToString(dt.Rows[i]["IT"].ToString());
                    objModel.Expenditure = Convert.ToString(dt.Rows[i]["Expenture"].ToString());
                    objModel.Budget = Convert.ToString(dt.Rows[i]["Budget"].ToString());
                    objModel.Jump = Convert.ToString(dt.Rows[i]["Jump"].ToString());
                    objModel.Status = Convert.ToString(dt.Rows[i]["Active"].ToString());
                    objModel.delmat_Actionstatus = Convert.ToString(dt.Rows[i]["ActionStatus"].ToString());//ramya added on 11 Apr 22
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

            //throw new NotImplementedException();
        }
        public void DeleteDelmat(int delmatmatrixgid, int delmatsetflowgid)
        {
            CommonIUD delecomm = new CommonIUD();
            string col_value = string.Empty;
            string col_temp = string.Empty;
            try
            {
                int flowdelmat1 = 0;
                GetCon();
                DataTable dt1 = new DataTable();
                SqlCommand cmd11 = new SqlCommand("select * from iem_mst_tdelmatflow_adt where  delmatflow_gid=" + delmatsetflowgid + 
                    " and delmatflow_isremoved='N' order by delmatflow_order asc", con);
                SqlDataAdapter da1 = new SqlDataAdapter(cmd11);
                da1 = new SqlDataAdapter(cmd11);
                da1.Fill(dt1);
                int flowdelmat = 0;
                if (dt1.Rows.Count > 0)
                {
                    flowdelmat1 = Convert.ToInt32(dt1.Rows[0]["delmatflow_delmat_gid"].ToString());
                    string dlflow = dt1.Rows[0]["delmatflow_order"].ToString();
                    GetCon();
                    DataTable dt = new DataTable();
                    SqlCommand cmd2 = new SqlCommand("select * from iem_mst_tdelmatflow_adt where  delmatflow_delmat_gid=" + flowdelmat1 + 
                        " and  delmatflow_order>=" + dlflow + " and delmatflow_isremoved='N' order by delmatflow_order asc", con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd2);
                    da = new SqlDataAdapter(cmd2);
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        
                        for (int k = 0; k < dt.Rows.Count; k++)
                        {
                            flowdelmat = Convert.ToInt32(dt.Rows[k]["delmatflow_delmat_gid"].ToString());
                            int flow = Convert.ToInt32(dt.Rows[k]["delmatflow_order"].ToString());
                            string name = dt.Rows[k]["delmatflow_title_value"].ToString();
                            flow--;

                            //if (k != 0)
                            //{
                            //    flow = flow + k;
                            //}
                            CommonIUD updatecomm = new CommonIUD();
                            string[,] codes1 = new string[,]
	                  {
                          {"delmatflow_order",  Convert.ToString (flow)}                         
                         
	                  };
                            flow = flow - 1;
                            string[,] whrcol1 = new string[,]
	                 {
                          {"delmatflow_delmat_gid", Convert.ToString (flowdelmat)},                         
                          {"delmatflow_title_value",  name}
            
                     };
                            string tblname1 = "iem_mst_tdelmatflow_adt";

                            string updcomm1 = updatecomm.UpdateCommon(codes1, whrcol1, tblname1);

                            
                             
                        }
                    }
                }


                string[,] codes = new string[,]
	       {
                 {"delmatflow_isremoved", "Y"}
	            
           };
                string[,] whrcol = new string[,]
	        {
                {"delmatflow_gid", delmatsetflowgid.ToString ()}
            };
                string tblname = "iem_mst_tdelmatflow_adt";

                string deletetbl = delecomm.DeleteCommon(codes, whrcol, tblname);

                SqlCommand cmd = new SqlCommand("pr_iem_mst_delmat", con);
                GetCon();
                cmd.Parameters.AddWithValue("@action", "SelectMatrixById");
                cmd.Parameters.AddWithValue("@gid", delmatmatrixgid);
                cmd.Parameters.Add("@res", SqlDbType.VarChar, 45).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                string res1 = cmd.Parameters["@res"].Value.ToString();

                if (res1 == "Exist")
                {
                    //GetCon();
                    //cmd = new SqlCommand("delete from iem_mst_tdelmatmatrix_adt where delmatmatrix_delmatflow_gid='" + delmatmatrixgid + "'", con);

                    //cmd.ExecuteNonQuery();

                    // delete tran data ramya added on 17 jun 22
                    
                    DataTable dc = new DataTable();
                    SqlCommand cmd1 = new SqlCommand("pr_iem_mst_delmat", con);
                    GetCon();
                    cmd1.Parameters.AddWithValue("@action", "DeleteDelmatmatrixtmp");
                    cmd1.Parameters.AddWithValue("@gid", flowdelmat);
                    cmd1.Parameters.AddWithValue("@delmatflowgid", delmatsetflowgid);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.ExecuteNonQuery();
                    SqlDataAdapter da = new SqlDataAdapter(cmd1);
                    da = new SqlDataAdapter(cmd1);
                    da.Fill(dc);
                }

                if (flowdelmat1 != 0)
                {
                    //ramya added on 18 apr 22
                    string[,] codes2 = new string[,]   
	               {   
                        {"delmat_update_by",Convert.ToString ("0")},
                        {"delmat_update_date","sysdatetime()"},	 
                        {"delmat_checkerby","0"},	 
                        {"delmat_checkerinsert_date","1900-01-01"},
                        {"delmat_checker_remark",""},
                        {"delmat_status","0"}
                  };
                    string[,] whrcol2 = new string[,]
	                 {
                          {"delmat_gid", flowdelmat1.ToString()}
            
                     };
                    string tblname2 = "iem_mst_tdelmat_adt";

                    string updcomm2 = delecomm.UpdateCommon(codes2, whrcol2, tblname2);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
            //throw new NotImplementedException();
        }

        public IEnumerable<GetDelmat> GetDelmat()
        {
            try
            {
                List<GetDelmat> objCountrytype = new List<GetDelmat>();
                GetDelmat objModel;
                GetCon();
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand("pr_iem_mst_delmat", con);
                cmd.Parameters.AddWithValue("@action", "select");
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    objModel = new GetDelmat();
                    objModel.delmattype_gid = Convert.ToInt32(dt.Rows[i]["delmattype_gid"].ToString());
                    objModel.delmattype_name = Convert.ToString(dt.Rows[i]["delmattype_name"].ToString());
                    objCountrytype.Add(objModel);
                }

                return objCountrytype;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
            // throw new NotImplementedException();
        }

        public GetDelmatById GetDelmatTypeById(int DelmatId)
        {
            throw new NotImplementedException();
        }

        public GetDepartmentById GetDepartmentById(int DepatId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<GetSlab> GetSlab()
        {
            try
            {
                List<GetSlab> objCountrytype = new List<GetSlab>();
                GetSlab objModel;
                GetCon();
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand("select slab_gid,slab_name from iem_mst_tslab where slab_isremoved='N'", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    objModel = new GetSlab();
                    objModel.slab_gid = Convert.ToInt32(dt.Rows[i]["slab_gid"].ToString());
                    objModel.slab_name = Convert.ToString(dt.Rows[i]["slab_name"].ToString());
                    objCountrytype.Add(objModel);
                }

                return objCountrytype;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
            //throw new NotImplementedException();
        }

        public DelmatVisisbleById GetdelmatvisibleById(int DelmaytypeId)
        {
            try
            {
                List<DelmatVisisbleById> objOrgType = new List<DelmatVisisbleById>();
                DelmatVisisbleById objModel;
                GetCon();
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand("select * from iem_mst_tdelmattype where delmattype_gid='" + DelmaytypeId + "' and delmattype_isremoved='N' ", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                objModel = new DelmatVisisbleById()
                {
                    delmattype_branch_visible = Convert.ToString(dt.Rows[0]["delmattype_branch_visible"].ToString()),
                    delmattype_branchtype_visible = Convert.ToString(dt.Rows[0]["delmattype_branchtype_visible"].ToString()),
                    delmattype_claim_visible = Convert.ToString(dt.Rows[0]["delmattype_claim_visible"].ToString()),
                    delmattype_dsa_visible = Convert.ToString(dt.Rows[0]["delmattype_dsa_visible"].ToString()),
                    delmattype_pipit_visible = Convert.ToString(dt.Rows[0]["delmattype_pipit_visible"].ToString()),
                    delmattype_it_visible = Convert.ToString(dt.Rows[0]["delmattype_it_visible"].ToString()),
                    delmattype_exp_visible = Convert.ToString(dt.Rows[0]["delmattype_exp_visible"].ToString()),
                    delmattype_budget_visible = Convert.ToString(dt.Rows[0]["delmattype_budget_visible"].ToString()),
                    delmattype_jump_visible = Convert.ToString(dt.Rows[0]["delmattype_jump_visible"].ToString()),
                    //delmat changes
                    delmattype_ecftype_visible = Convert.ToString(dt.Rows[0]["delmattype_ecftype_visible"].ToString()),
                    delmattype_parpr_visible = Convert.ToString(dt.Rows[0]["delmattype_parpr_visible"].ToString()),
                    delmattype_withwithoutparpr_visible = Convert.ToString(dt.Rows[0]["delmattype_wwoparpr_visible"].ToString()),


                };
                return objModel;


                //return objOrgType;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
            //throw new NotImplementedException();
        }

        public DataTable GetDepartmentTable()
        {
            try
            {
                List<GetDepartment> objCountrytype = new List<GetDepartment>();
                GetDepartment objModel;
                GetCon();
                DataTable dt = new DataTable();

                SqlCommand cmd = new SqlCommand("select dept_gid,dept_name from iem_mst_tdept where dept_isremoved='N'", con);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                return dt;
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

        public IEnumerable<GetDepartment> GetDepartment()
        {
            try
            {
                List<GetDepartment> objCountrytype = new List<GetDepartment>();
                GetDepartment objModel;
                GetCon();
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand("select dept_gid,dept_name from iem_mst_tdept where dept_isremoved='N'", con);
                //cmd.Parameters.AddWithValue("@action", "select");
                //cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    objModel = new GetDepartment();
                    objModel.dept_gid = Convert.ToInt32(dt.Rows[0]["dept_gid"].ToString());
                    objModel.dept_name = Convert.ToString(dt.Rows[0]["dept_name"].ToString());
                    objCountrytype.Add(objModel);
                }

                return objCountrytype;
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

        public string AddDelmatdepartment(iem_mst_delmat Delmatdetails, string[] Selecteddepartment)
        {
            string insertcommend = string.Empty;
            try
            {
                CommonIUD comm = new CommonIUD();

                string result = string.Empty;
                string editsection = HttpContext.Current.Session["function"].ToString();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "";
                
                GetCon();
                cmd = new SqlCommand("delete from iem_mst_tdelmatdept_adt where delmatdept_delmat_gid='" + Delmatdetails.delmat_gid + "'", con);
                cmd.ExecuteNonQuery();

                foreach (string selctedepartment in Selecteddepartment)
                {
                    cmd = new SqlCommand();
                    cmd = new SqlCommand("pr_iem_mst_delmatdept", con);
                    //GetCon();
                    cmd.Parameters.AddWithValue("@action", "Exist");
                    cmd.Parameters.AddWithValue("@delmatdept_delmat_gid", Delmatdetails.delmat_gid);
                    if (selctedepartment != "")
                    {
                        cmd.Parameters.AddWithValue("@delmatdept_dept_gid", SqlDbType.Int).Value = Convert.ToInt32(selctedepartment);
                        cmd.Parameters.Add("@res", SqlDbType.VarChar, 45).Direction = ParameterDirection.Output;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.ExecuteNonQuery();
                        string res1 = cmd.Parameters["@res"].Value.ToString();
                        if (res1 == "Not There")
                        {
                                    string[,] codes = new string[,]            
                            {
                            {"delmatdept_delmat_gid",Convert.ToString (Delmatdetails.delmat_gid)},
                            {"delmatdept_dept_gid",Convert.ToString (selctedepartment)},                    
	    
                            };
                                    string tname = "iem_mst_tdelmatdept_adt";
                                    insertcommend = comm.InsertCommon(codes, tname);
                        }
                        else if (res1 == "Already Exist")
                        {
                            cmd.CommandText = "";
                            //if (editsection == "Edit")
                            //{
                            //    cmd = new SqlCommand("delete from iem_mst_tdelmatdept_adt where delmatdept_delmat_gid='" + Delmatdetails.delmat_gid + "' and delmatdept_dept_gid='" + selctedepartment + "'", con);
                            //}
                            //else
                            //{
                            //    cmd = new SqlCommand("delete from iem_mst_tdelmatdept_adt where delmatdept_delmat_gid='" + Delmatdetails.delmat_gid + "'", con);
                            //}
                            cmd = new SqlCommand("delete from iem_mst_tdelmatdept_adt where delmatdept_delmat_gid='" + Delmatdetails.delmat_gid + "' and delmatdept_dept_gid='" + selctedepartment + "'", con);
                            cmd.ExecuteNonQuery();
                            string[,] codes = new string[,]            
	                   {
                        {"delmatdept_delmat_gid",Convert.ToString (Delmatdetails.delmat_gid)},
                        {"delmatdept_dept_gid",Convert.ToString (selctedepartment)},                    
	    
                       };
                            string tname = "iem_mst_tdelmatdept_adt";
                            insertcommend = comm.InsertCommon(codes, tname);
                        }
                    }
                }

                return insertcommend;


            }

            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }
            return insertcommend;
            // throw new NotImplementedException();
        }

        public IEnumerable<GetTitle> GetTitle()
        {
            try
            {
                List<GetTitle> objCountrytype = new List<GetTitle>();
                GetTitle objModel;
                GetCon();
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand("select title_gid,title_name from iem_mst_ttitle where title_isremoved='N'", con);
                //cmd.Parameters.AddWithValue("@action", "select");
                //cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    objModel = new GetTitle();
                    objModel.title_gid = Convert.ToInt32(dt.Rows[i]["title_gid"].ToString());
                    objModel.title_name = Convert.ToString(dt.Rows[i]["title_name"].ToString());
                    objCountrytype.Add(objModel);
                }

                return objCountrytype;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
            // throw new NotImplementedException();
        }

        public IEnumerable<GetEmployee> GetEmployee()
        {
            try
            {
                List<GetEmployee> objCountrytype = new List<GetEmployee>();
                GetEmployee objModel;
                GetCon();
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand("select employee_gid,employee_code,employee_name from iem_mst_temployee where employee_isremoved='N'", con);
                //cmd.Parameters.AddWithValue("@action", "select");
                //cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    objModel = new GetEmployee();
                    objModel.employee_gid = Convert.ToInt32(dt.Rows[i]["employee_gid"].ToString());
                    objModel.employee_code = Convert.ToString(dt.Rows[i]["employee_code"].ToString()) + "-" + Convert.ToString(dt.Rows[i]["employee_name"].ToString());
                    objCountrytype.Add(objModel);
                }

                return objCountrytype;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
            //  throw new NotImplementedException();
        }

        public IEnumerable<GetGrade> GetGrade()
        {
            try
            {
                List<GetGrade> objCountrytype = new List<GetGrade>();
                GetGrade objModel;
                GetCon();
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand("select grade_gid,grade_code,grade_name from iem_mst_tgrade where grade_isremoved='N'", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    objModel = new GetGrade();
                    objModel.employee_gid = Convert.ToInt32(dt.Rows[i]["grade_gid"].ToString());
                    objModel.employee_code = Convert.ToString(dt.Rows[i]["grade_name"].ToString());
                    objCountrytype.Add(objModel);
                }

                return objCountrytype;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
            // throw new NotImplementedException();
        }

        public IEnumerable<GetDesignation> GetDesignation()
        {
            try
            {
                List<GetDesignation> objCountrytype = new List<GetDesignation>();
                GetDesignation objModel;
                GetCon();
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand("select designation_gid,designation_code,designation_name from iem_mst_tdesignation where designation_isremoved='N' and designation_type='I'", con);
                //cmd.Parameters.AddWithValue("@action", "select");
                //cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    objModel = new GetDesignation();
                    objModel.employee_gid = Convert.ToInt32(dt.Rows[i]["designation_gid"].ToString());
                    objModel.employee_code = Convert.ToString(dt.Rows[i]["designation_name"].ToString());
                    objCountrytype.Add(objModel);
                }

                return objCountrytype;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
            // throw new NotImplementedException();
        }

        public IEnumerable<GetRole> GetRole()
        {
            try
            {
                List<GetRole> objCountrytype = new List<GetRole>();
                GetRole objModel;
                GetCon();
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand("select role_gid,role_code,role_name from iem_mst_trole where role_isremoved='N'", con);
                //cmd.Parameters.AddWithValue("@action", "select");
                //cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    objModel = new GetRole();
                    objModel.employee_gid = Convert.ToInt32(dt.Rows[i]["role_gid"].ToString());
                    objModel.employee_code = Convert.ToString(dt.Rows[i]["role_name"].ToString());
                    objCountrytype.Add(objModel);
                }

                return objCountrytype;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
            // throw new NotImplementedException();
        }

        public DataTable GetTitleById(int TitleId)
        {
            try
            {
                List<GetDepartment> objCountrytype = new List<GetDepartment>();
                GetDepartment objModel;
                GetCon();
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand("select title_flag,title_name  from iem_mst_ttitle where title_gid='" + TitleId + "' and title_isremoved='N'", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
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

        public DataTable InsertData(int TitleId, int ValueId)
        {
            throw new NotImplementedException();
        }

        public DataTable GetValueDetailsById(int EmpGid, int flag)
        {
            DataTable valuedetails = new DataTable();
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            if (flag == 1)
            {
                cmd = new SqlCommand("select employee_code,employee_name from iem_mst_temployee where employee_gid='" + EmpGid + "' and employee_isremoved='N'", con);
                GetCon();
                da = new SqlDataAdapter(cmd);
                da.Fill(valuedetails);

            }
            if (flag == 2)
            {
                cmd = new SqlCommand("select grade_code,grade_name from iem_mst_tgrade where grade_gid='" + EmpGid + "' and grade_isremoved='N'", con);
                GetCon();
                da = new SqlDataAdapter(cmd);
                da.Fill(valuedetails);


            }
            if (flag == 3)
            {
                cmd = new SqlCommand("select designation_code,designation_name from iem_mst_tdesignation where designation_gid='" + EmpGid + "' and designation_isremoved='N'", con);
                GetCon();
                da = new SqlDataAdapter(cmd);
                da.Fill(valuedetails);

            }
            if (flag == 4)
            {
                cmd = new SqlCommand("select role_code,role_name from iem_mst_trole where role_gid='" + EmpGid + "' and role_isremoved='N'", con);
                GetCon();
                da = new SqlDataAdapter(cmd);
                da.Fill(valuedetails);
                //return valuedetails;
            }
            return valuedetails;

        }

        public DataTable GetrowsOnlyByID(int slabranheId)
        {
            DataTable dt = new DataTable();
            try
            {
                List<GetDepartment> objCountrytype = new List<GetDepartment>();
                GetDepartment objModel;
                GetCon();
                DataTable dc = new DataTable();
                SqlCommand cmd = new SqlCommand("select slabrange_name from iem_mst_tslabrange where slabrange_slab_gid=" + slabranheId + " and  slabrange_isremoved='N'", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
            return dt;
            //throw new NotImplementedException();
        }

        public DataTable GettileGidByname(string Titlename)
        {
            DataTable dt = new DataTable();
            try
            {
                List<GetDepartment> objCountrytype = new List<GetDepartment>();
                GetDepartment objModel;
                GetCon();
                SqlCommand cmd = new SqlCommand("select title_gid  from iem_mst_ttitle where title_name='" + Titlename + "' and title_isremoved='N'", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();

            }
            return dt;
            // throw new NotImplementedException();
        }

        public DataTable GettitlevalueByName(string Titlevaluename, int flag)
        {
            DataTable titlevaluedetails = new DataTable();
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            if (flag == 1)
            {
                cmd = new SqlCommand("select employee_code,employee_gid from iem_mst_temployee where employee_code='" + Titlevaluename + "' and employee_isremoved='N'", con);
                GetCon();
                da = new SqlDataAdapter(cmd);
                da.Fill(titlevaluedetails);

            }
            if (flag == 2)
            {
                cmd = new SqlCommand("select grade_code,grade_gid from iem_mst_tgrade where grade_code='" + Titlevaluename + "' and grade_isremoved='N'", con);
                GetCon();
                da = new SqlDataAdapter(cmd);
                da.Fill(titlevaluedetails);


            }
            if (flag == 3)
            {
                cmd = new SqlCommand("select designation_code,designation_gid from iem_mst_tdesignation where designation_code='" + Titlevaluename + "' and designation_type='I' and designation_isremoved='N' ", con);
                GetCon();
                da = new SqlDataAdapter(cmd);
                da.Fill(titlevaluedetails);

            }
            if (flag == 4)
            {
                cmd = new SqlCommand("select role_code,role_gid from iem_mst_trole where role_code='" + Titlevaluename + "' and role_isremoved='N'", con);
                GetCon();
                da = new SqlDataAdapter(cmd);
                da.Fill(titlevaluedetails);
                //return valuedetails;
            }
            return titlevaluedetails;
        }

        public void InsertDelmatsetflow(iem_mst_delmat Delmatsetflow)
        {
            GetCon();
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand("select * from iem_mst_tdelmatflow_adt where  delmatflow_delmat_gid=" + Delmatsetflow.delmat_gid + " and  delmatflow_order>=" + Delmatsetflow.Flow + " and delmatflow_isremoved='N' order by delmatflow_order asc", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                string dlflow = dt.Rows[0]["delmatflow_order"].ToString();
                // if (Convert.ToInt32(dlflow) == Delmatsetflow.Flow)
                {
                    for (int k = 0; k < dt.Rows.Count; k++)
                    {
                        int flow = Convert.ToInt32(dt.Rows[k]["delmatflow_order"].ToString());
                        string name = dt.Rows[k]["delmatflow_title_value"].ToString();
                        flow++;

                        CommonIUD updatecomm = new CommonIUD();
                        string[,] codes1 = new string[,]
	                  {
                          {"delmatflow_order",  Convert.ToString (flow)}                         
                         
	                  };
                        Delmatsetflow.InFlowCount = flow;
                        flow = flow - 1;
                        string[,] whrcol = new string[,]
	                 {
                          {"delmatflow_delmat_gid", Convert.ToString (Delmatsetflow.delmat_gid)},                         
                          {"delmatflow_title_value",  name}
            
                     };
                        string tblname = "iem_mst_tdelmatflow_adt";

                        string updcomm = updatecomm.UpdateCommon(codes1, whrcol, tblname);
                    }
                }
            }
            Delmatsetflow.TitleName = Delmatsetflow.TitleName.Trim().Replace("'", "''");
            CommonIUD comm = new CommonIUD();
            string[,] codes = new string[,]              
	               {        
        {"delmatflow_delmat_gid",Convert.ToString (Delmatsetflow.delmat_gid)},
        {"delmatflow_order",Convert.ToString(Delmatsetflow.Flow)},     //Flow value
        {"delmatflow_title_gid",Convert.ToString(Delmatsetflow.TitleNamegid)},
        {"delmatflow_title_value",Convert.ToString (Delmatsetflow.TitleName)},
        {"delmatflow_title_ref_gid",Convert.ToString(Delmatsetflow.title_gid)},
        {"delmatflow_additional_approval",Convert.ToString (Delmatsetflow.AddApporoval)},
         
                  };
            string tname = "iem_mst_tdelmatflow_adt";
            string insertcommend = comm.InsertCommon(codes, tname);


            //ramya added on 18 apr 22
            string[,] codes2 = new string[,]   
	               {        
                        {"delmat_update_by",Convert.ToString (Delmatsetflow.delmat_update_by)},
                        {"delmat_update_date","sysdatetime()"},	 
                        {"delmat_checkerby","0"},	 
                        {"delmat_checkerinsert_date","1900-01-01"},
                        {"delmat_checker_remark",""},
                        {"delmat_status","0"}
                  };
            string[,] whrcol1 = new string[,]
	                 {
                          {"delmat_gid", Delmatsetflow.delmat_gid.ToString ()}
            
                     };
            string tblname1 = "iem_mst_tdelmat_adt";

            string updcomm1 = comm.UpdateCommon(codes2, whrcol1, tblname1);
               


        }

        public void updateDelmatsetflowedit(iem_mst_delmat Delmatsetflow)
        {
            GetCon();
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand("select * from iem_mst_tdelmatflow_adt where  delmatflow_delmat_gid=" + Delmatsetflow.delmat_gid + " and  delmatflow_order>=" + Delmatsetflow.Flow + " and delmatflow_isremoved='N'  order by delmatflow_order asc", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                int cntvl = 0;
                string dlflow = dt.Rows[0]["delmatflow_order"].ToString();
                // if (Convert.ToInt32(dlflow) == Delmatsetflow.Flow)
                {
                    for (int k = 0; k < dt.Rows.Count; k++)
                    {
                        int flow = Convert.ToInt32(dt.Rows[k]["delmatflow_order"].ToString());
                        string name = dt.Rows[k]["delmatflow_title_value"].ToString();
                        if (Delmatsetflow.TitleName != name)
                        {
                            if (cntvl == 0)
                            {
                                flow++;
                                CommonIUD updatecomm = new CommonIUD();
                                string[,] codes1 = new string[,]
	                  {
                          {"delmatflow_order",  Convert.ToString (flow)}                         
                         
	                  };
                                flow = flow - 1;
                                string[,] whrcol = new string[,]
	                 {
                          {"delmatflow_delmat_gid", Convert.ToString (Delmatsetflow.delmat_gid)},                         
                          {"delmatflow_title_value",  name}
            
                     };
                                string tblname = "iem_mst_tdelmatflow_adt";

                                string updcomm = updatecomm.UpdateCommon(codes1, whrcol, tblname);
                            }
                        }
                        else
                        {
                            cntvl = 1;
                        }
                    }
                }
            }
            CommonIUD updatecomm1 = new CommonIUD();
            string[,] codes11 = new string[,]
	                  {
                          {"delmatflow_isremoved", "Y"}                         
                         
	                  };

            string[,] whrcol1 = new string[,]
	                 {
                          {"delmatflow_delmat_gid", Convert.ToString (Delmatsetflow.delmat_gid)},                         
                          {"delmatflow_title_value",  Delmatsetflow.TitleName},
                            {"delmatflow_title_gid",Convert.ToString(Delmatsetflow.TitleNamegid)}
            
                     };
            string tblname1 = "iem_mst_tdelmatflow_adt";

            string updcomm1 = updatecomm1.UpdateCommon(codes11, whrcol1, tblname1);
            Delmatsetflow.TitleName = Delmatsetflow.TitleName.Trim().Replace("'", "''");
            CommonIUD comm = new CommonIUD();
            string[,] codes = new string[,]              
	               {        
        {"delmatflow_delmat_gid",Convert.ToString (Delmatsetflow.delmat_gid)},
        {"delmatflow_order",Convert.ToString(Delmatsetflow.Flow)},     //Flow value
        {"delmatflow_title_gid",Convert.ToString(Delmatsetflow.TitleNamegid)},
        {"delmatflow_title_value",Convert.ToString (Delmatsetflow.TitleName)},
        {"delmatflow_title_ref_gid",Convert.ToString(Delmatsetflow.title_gid)},
        {"delmatflow_additional_approval",Convert.ToString (Delmatsetflow.AddApporoval)},
         
                  };
            string tname = "iem_mst_tdelmatflow_adt";
            string insertcommend = comm.InsertCommon(codes, tname);

            //ramya added on 18 apr 22
            string[,] codes2 = new string[,]   
          
	               {        
        
        {"delmat_update_by",Convert.ToString (Delmatsetflow.delmat_update_by)},
        {"delmat_update_date","sysdatetime()"},	 
                    {"delmat_checkerby","0"},	 
           {"delmat_checkerinsert_date","1900-01-01"},
                   {"delmat_checker_remark",""},
                   {"delmat_status","0"}
                  };
            string[,] whrcol2 = new string[,]
	                 {
                          {"delmat_gid", Delmatsetflow.delmat_gid.ToString ()}
            
                     };
            string tblname2 = "iem_mst_tdelmat_adt";

            string updcomm2 = comm.UpdateCommon(codes2, whrcol2, tblname2);
               
        }

        public void updateDelmatsetflow(iem_mst_delmat Delmatsetflow)
        {
            GetCon();
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand("select * from iem_mst_tdelmatflow_adt where  delmatflow_delmat_gid=" + Delmatsetflow.delmat_gid + " and  delmatflow_order>=" + Delmatsetflow.Flow + " and delmatflow_isremoved='N' order by delmatflow_order asc", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                string dlflow = dt.Rows[0]["delmatflow_order"].ToString();
                // if (Convert.ToInt32(dlflow) == Delmatsetflow.Flow)
                {
                    int k = 0;
                    // for (int k = 0; k < dt.Rows.Count; k++)
                    {
                        int flow = Convert.ToInt32(dt.Rows[k]["delmatflow_order"].ToString());
                        string name = dt.Rows[k]["delmatflow_title_value"].ToString();


                        //if (k != 0)
                        //{
                        //    flow = flow + k;
                        //}
                        CommonIUD updatecomm = new CommonIUD();
                        string[,] codes1 = new string[,]
	                  {
                            {"delmatflow_order",  Convert.ToString (flow)}   ,
                            {"delmatflow_additional_approval",Convert.ToString (Delmatsetflow.AddApporoval)},
                            {"delmatflow_title_value",Convert.ToString (Delmatsetflow.TitleName)},
                            {"delmatflow_title_ref_gid",Convert.ToString(Delmatsetflow.title_gid)}
	                  };

                        string[,] whrcol = new string[,]
	                 {
                          {"delmatflow_gid", dt.Rows[0]["delmatflow_gid"].ToString()} 
                     };
                        string tblname = "iem_mst_tdelmatflow_adt";

                        string updcomm = updatecomm.UpdateCommon(codes1, whrcol, tblname);


                        //ramya added on 18 apr 22
                        string[,] codes2 = new string[,]   
          
	               {        
        
        {"delmat_update_by",Convert.ToString (Delmatsetflow.delmat_update_by)},
        {"delmat_update_date","sysdatetime()"},	 
        //add delmat changes
              
                    {"delmat_checkerby","0"},	 
           {"delmat_checkerinsert_date","1900-01-01"},
                   {"delmat_checker_remark",""},
                   {"delmat_status","0"}
                  };
                        string[,] whrcol1 = new string[,]
	                 {
                          {"delmat_gid", Delmatsetflow.delmat_gid.ToString ()}
            
                     };
                        string tblname1 = "iem_mst_tdelmat_adt";

                        updcomm = updatecomm.UpdateCommon(codes2, whrcol1, tblname1);
                

                    }
                }
            }
            //        CommonIUD comm = new CommonIUD();
            //        string[,] codes = new string[,]              
            //           {        
            //{"delmatflow_delmat_gid",Convert.ToString (Delmatsetflow.delmat_gid)},
            //{"delmatflow_order",Convert.ToString(Delmatsetflow.Flow)},     //Flow value
            //{"delmatflow_title_gid",Convert.ToString(Delmatsetflow.TitleNamegid)},
            //{"delmatflow_title_value",Convert.ToString (Delmatsetflow.TitleName)},
            //{"delmatflow_title_ref_gid",Convert.ToString(Delmatsetflow.title_gid)},
            //{"delmatflow_additional_approval",Convert.ToString (Delmatsetflow.AddApporoval)},

            //          };
            //        string tname = "iem_mst_tdelmatflow_adt";
            //        string insertcommend = comm.InsertCommon(codes, tname);
        }

        public DataTable GettitleGidByName(string Titlevaluename, int flag)
        {
            DataTable titlevaluedetails = new DataTable();
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            if (flag == 1)
            {
                cmd = new SqlCommand("select title_gid from iem_mst_ttitle where title_name='" + Titlevaluename + "' and title_isremoved='N'", con);
                GetCon();
                da = new SqlDataAdapter(cmd);
                da.Fill(titlevaluedetails);
            }
            if (flag == 2)
            {
                cmd = new SqlCommand("select title_gid from iem_mst_ttitle where title_name='" + Titlevaluename + "' and title_isremoved='N'", con);
                GetCon();
                da = new SqlDataAdapter(cmd);
                da.Fill(titlevaluedetails);


            }
            if (flag == 3)
            {
                cmd = new SqlCommand("select title_gid from iem_mst_ttitle where title_name='" + Titlevaluename + "' and title_isremoved='N'", con);
                GetCon();
                da = new SqlDataAdapter(cmd);
                da.Fill(titlevaluedetails);

            }
            if (flag == 4)
            {
                cmd = new SqlCommand("select title_gid from iem_mst_ttitle where title_name='" + Titlevaluename + "' and title_isremoved='N'", con);
                GetCon();
                da = new SqlDataAdapter(cmd);
                da.Fill(titlevaluedetails);
                //return valuedetails;
            }
            return titlevaluedetails;
            // throw new NotImplementedException();
        }

        public DataTable GetSetFlowId()
        {
            DataTable dt = new DataTable();
            try
            {
                List<GetSlab> objCountrytype = new List<GetSlab>();
                GetSlab objModel;
                GetCon();
                SqlCommand cmd = new SqlCommand("select MAX(delmatflow_gid) from iem_mst_tdelmatflow_adt where delmatflow_isremoved='N'", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
            return dt;
            // throw new NotImplementedException();
        }

        public void DeleteDelmatException(int EceptionID)
        {
            CommonIUD delecomm = new CommonIUD();
            string col_value = string.Empty;
            string col_temp = string.Empty;
            try
            {

                string[,] codes = new string[,]
	       {
                 {"delmatexception_isremoved", "Y"}
	            
           };
                string[,] whrcol = new string[,]
	        {
                {"delmatexception_gid", EceptionID.ToString ()}
            };
                string tblname = "iem_mst_tdelmatexception_adt";

                string deletetbl = delecomm.DeleteCommon(codes, whrcol, tblname);
                GetCon();
                DataTable dt = new DataTable();
                DataTable dc = new DataTable();
                SqlCommand cmd = new SqlCommand("select delmatexception_delmat_gid as delmat_gid from iem_mst_tdelmatexception_adt where delmatexception_gid=" + EceptionID, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                //ramya added on 18 apr 22
                string[,] codes1 = new string[,]   
          
	               {        
        
        {"delmat_update_by","0"},
        {"delmat_update_date","sysdatetime()"},	 
        //add delmat changes
              
                    {"delmat_checkerby","0"},	 
           {"delmat_checkerinsert_date","1900-01-01"},
                   {"delmat_checker_remark",""},
                   {"delmat_status","0"}
                  };
                string[,] whrcol1 = new string[,]
	                 {
                          {"delmat_gid", dt.Rows[0]["delmat_gid"].ToString ()}
            
                     };
                string tblname1 = "iem_mst_tdelmat_adt";

              string  updcomm = delecomm.UpdateCommon(codes1, whrcol1, tblname1);

                //throw new NotImplementedException();
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }

        public iem_mst_delmat GetdelmatExceptionById(int DematId)
        {
            try
            {
                List<iem_mst_delmat> objOrgType = new List<iem_mst_delmat>();
                iem_mst_delmat objModel;
                GetCon();
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand("pr_iem_mst_delmat", con);
                cmd.Parameters.AddWithValue("@action", "selectDelmatExceptionById");
                cmd.Parameters.AddWithValue("@gid", DematId);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                objModel = new iem_mst_delmat()
                {
                    delmatexception_gid = Convert.ToInt32(dt.Rows[0]["delmatexception_gid"].ToString()),
                    delmatexception_to = Convert.ToInt32(dt.Rows[0]["delmatexception_to"].ToString()),
                    delmatexception_limit = Convert.ToDecimal(dt.Rows[0]["delmatexception_limit"].ToString()),
                };
                return objModel;

                //return objOrgType;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
            // throw new NotImplementedException();
        }

        public void UpdateDelmatException(iem_mst_delmat Updatedelmatexception)
        {
            try
            {
                CommonIUD updatecomm = new CommonIUD();
                string[,] codes = new string[,]
	                  {
                          {"delmatexception_to", Updatedelmatexception.delmatexception_to.ToString ()},
                          {"delmatexception_limit", Updatedelmatexception.delmatexception_limit.ToString ()},
                         
	                  };
                string[,] whrcol = new string[,]
	                 {
                          {"delmatexception_gid", Updatedelmatexception.delmatexception_gid.ToString ()}
            
                     };
                string tblname = "iem_mst_tdelmatexception_adt";

                string updcomm = updatecomm.UpdateCommon(codes, whrcol, tblname);

                //ramya added on 18 apr 22
                string[,] codes1 = new string[,]   
          
	               {        
        
        {"delmat_update_by",Convert.ToString (Updatedelmatexception.delmat_update_by)},
        {"delmat_update_date","sysdatetime()"},	 
        //add delmat changes
              
                    {"delmat_checkerby","0"},	 
           {"delmat_checkerinsert_date","1900-01-01"},
                   {"delmat_checker_remark",""},
                   {"delmat_status","0"}
                  };
                string[,] whrcol1 = new string[,]
	                 {
                          {"delmat_gid", Updatedelmatexception.delmat_gid.ToString ()}
            
                     };
                string tblname1 = "iem_mst_tdelmat_adt";

                updcomm = updatecomm.UpdateCommon(codes1, whrcol1, tblname1);
                


            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
            // throw new NotImplementedException();
        }

        public DataTable GetDelmatGid(string Name)
        {
            DataTable dt = new DataTable();
            try
            {
                List<GetDelmat> objCountrytype = new List<GetDelmat>();
                GetDelmat objModel;
                GetCon();

                DataTable dc = new DataTable();
                SqlCommand cmd = new SqlCommand("select max(delmat_gid) as delmat_gid from iem_mst_tdelmat_adt where delmat_name='" + Name + "' and delmat_isremoved='N'", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);

            }
            catch (Exception ex)
            {
                con.Close();

            }
            return dt;
        }

        public DataTable Getrows(string slabid)
        {
            DataTable dt = new DataTable();
            DataTable dc = new DataTable();
            try
            {
                if (slabid == "")
                {
                    slabid = "1";
                }
                List<GetDepartment> objCountrytype = new List<GetDepartment>();
                GetCon();
                SqlCommand cmd = new SqlCommand("select slabrange_name from iem_mst_tslabrange where slabrange_slab_gid=" + slabid + " and  slabrange_isremoved='N'", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                int rowcount = dt.Rows.Count;
                for (int t = 0; t < dt.Rows.Count; t++)
                {
                    if (!dc.Columns.Contains("Title") || !dc.Columns.Contains("Value") || !dc.Columns.Contains("Flow"))
                    {
                        //dc.Columns.Add("Gid");
                        dc.Columns.Add("SNo");
                        dc.Columns.Add("Title");
                        dc.Columns.Add("Value");
                        dc.Columns.Add("AddApproval");
                        dc.Columns.Add("Flow");
                        t--;
                    }
                    else
                    {
                        dc.Columns.Add(dt.Rows[t][0].ToString());
                    }

                }

                return dc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();

            }
            // throw new NotImplementedException();
        }

        public DataTable GetslabRangeByID(int slabrangegid)
        {
            DataTable dt = new DataTable();
            DataTable dtn = new DataTable();
            try
            {
                List<GetslabRangeByID> objCountrytype = new List<GetslabRangeByID>();
                GetslabRangeByID objModel;
                GetCon();
                SqlCommand cmd = new SqlCommand("select slabrange_name,slabrange_range_from,slabrange_range_to from iem_mst_tslabrange where slabrange_slab_gid='" + slabrangegid + "' and slabrange_isremoved='N'", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                DataColumn column = new DataColumn("SNo");
                column.DataType = System.Type.GetType("System.Int32");
                column.AutoIncrement = true;
                column.AutoIncrementSeed = 1;
                column.AutoIncrementStep = 1;
                dt.Columns.Add(column);
                int index = 0;
                foreach (DataRow row in dt.Rows)
                {
                    row.SetField(column, ++index);
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
            return dt;
        }

        public DataTable GetSlabById(int slabid)
        {
            DataTable dt = new DataTable();
            try
            {
                List<GetSlab> objCountrytype = new List<GetSlab>();
                GetCon();
                SqlCommand cmd = new SqlCommand("select slab_name from iem_mst_tslab where slab_gid=" + slabid + " and slab_isremoved='N'", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
            return dt;
        }

        public string checkduplicateFlow(iem_mst_delmat Delmatflow)
        {
            string result = string.Empty;
            try
            {
                GetCon();
                SqlCommand cmd = new SqlCommand("pr_iem_mst_delmat", con);
                cmd.Parameters.AddWithValue("@action", "CheckduplicateForDelmatFlow");
                cmd.Parameters.AddWithValue("@gid", Delmatflow.delmat_gid);
                cmd.Parameters.AddWithValue("@delmatflow_order", Delmatflow.Flow);
                cmd.Parameters.Add("@res", SqlDbType.VarChar, 45).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                result = cmd.Parameters["@res"].Value.ToString();


            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                con.Close();
            }
            return result;

        }
        public string checkFlowedit(iem_mst_delmat Delmatflow)
        {
            string result = string.Empty;
            string value = "";
            try
            {
                GetCon();
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand(" select * from iem_mst_tdelmatflow_adt where  delmatflow_gid!=" + Delmatflow.delmatsetflowgid + " and delmatflow_delmat_gid=" + Delmatflow.delmat_gid + "  and  delmatflow_order=" + Delmatflow.Flow + " and delmatflow_isremoved='N'", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    result = dt.Rows[0]["delmatflow_order"].ToString();
                }
                con.Close();
                if (result == "")
                {
                    GetCon();
                    DataTable dt1 = new DataTable();
                    SqlCommand cmd1 = new SqlCommand("select MAX(delmatflow_order) as delmatflow_order from iem_mst_tdelmatflow_adt where  delmatflow_delmat_gid=" + Delmatflow.delmat_gid + "  and delmatflow_isremoved='N'", con);
                    SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                    da1 = new SqlDataAdapter(cmd1);
                    da1.Fill(dt1);
                    if (dt1.Rows.Count > 0)
                    {
                        value = dt1.Rows[0]["delmatflow_order"].ToString();
                    }
                    if (value == "")
                    {
                        value = "0";
                    }
                    if (Convert.ToInt32(value) >= Convert.ToInt32(Delmatflow.Flow))
                    {
                        result = "Please  Enter Correct Flow Order !";

                    }
                    if (result == "")
                    {
                        value = Convert.ToString(Convert.ToInt32(value) + 1);
                        if (Convert.ToInt32(value) != Convert.ToInt32(Delmatflow.Flow))
                        {
                            result = "Please  Enter Correct Flow Order !";
                        }
                    }
                    else
                    {
                        result = "Not There";
                    }
                }
                else
                {
                    result = "Duplicate Flow Order !";
                }
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                con.Close();
            }
            return result;

        }
        public string checkFlow(iem_mst_delmat Delmatflow)
        {
            string result = string.Empty;
            string value = "";
            try
            {
                GetCon();
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand("select delmatflow_order from iem_mst_tdelmatflow_adt where  delmatflow_delmat_gid=" + Delmatflow.delmat_gid + " and  delmatflow_order=" + Delmatflow.Flow + " and delmatflow_isremoved='N'", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    result = dt.Rows[0]["delmatflow_order"].ToString();
                }
                con.Close();
                if (result == "")
                {
                    GetCon();
                    DataTable dt1 = new DataTable();
                    SqlCommand cmd1 = new SqlCommand("select MAX(delmatflow_order) as delmatflow_order from iem_mst_tdelmatflow_adt where  delmatflow_delmat_gid=" + Delmatflow.delmat_gid + "  and delmatflow_isremoved='N'", con);
                    SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                    da1 = new SqlDataAdapter(cmd1);
                    da1.Fill(dt1);
                    if (dt1.Rows.Count > 0)
                    {
                        value = dt1.Rows[0]["delmatflow_order"].ToString();
                    }
                    if (value == "")
                    {
                        value = "0";
                    }
                    if (Convert.ToInt32(value) > Convert.ToInt32(Delmatflow.Flow))
                    {
                        result = "Please  Enter Correct Flow Order !";

                    }
                    if (result == "")
                    {
                        value = Convert.ToString(Convert.ToInt32(value) + 1);
                        if (Convert.ToInt32(value) != Convert.ToInt32(Delmatflow.Flow))
                        {
                            result = "Please  Enter Correct Flow Order !";
                        }
                    }
                    else
                    {
                        result = "Flow Duplicate";
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            finally
            {
                con.Close();
            }
            return result;

        }
        public string InsertDelmatException(iem_mst_delmat Delmatexception)
        {
            CommonIUD comm = new CommonIUD();

            SqlCommand cmd = new SqlCommand("pr_iem_mst_delmat", con);
            GetCon();
            cmd.Parameters.AddWithValue("@action", "CheckduplicateForDelmatException");
            cmd.Parameters.AddWithValue("@delmatexceptiongid", Delmatexception.delmat_gid);
            cmd.Parameters.AddWithValue("@gid", Delmatexception.delmatexception_to);

            cmd.Parameters.Add("@res", SqlDbType.VarChar, 45).Direction = ParameterDirection.Output;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.ExecuteNonQuery();
            string res1 = cmd.Parameters["@res"].Value.ToString();
            if (res1 != "Exist")
            {


                string[,] codes = new string[,]    
          
	               {        
        {"delmatexception_delmat_gid",Convert.ToString (Delmatexception.delmat_gid)},
        {"delmatexception_to",Convert.ToString(Delmatexception.delmatexception_to)},     //Flow value
        {"delmatexception_limit",Convert.ToString(Delmatexception.delmatexception_limit)},
       
                  };
                string tname = "iem_mst_tdelmatexception_adt";
                string insertcommend = comm.InsertCommon(codes, tname);

                //ramya added on 18 apr 22
                string[,] codes1 = new string[,]  
	               {        

        {"delmat_update_by",Convert.ToString (Delmatexception.delmat_update_by)},
        {"delmat_update_date","sysdatetime()"},	 
        //add delmat changes
              
                    {"delmat_checkerby","0"},	 
           {"delmat_checkerinsert_date","1900-01-01"},
                   {"delmat_checker_remark",""},
                   {"delmat_status","0"}
                  };
                string[,] whrcol = new string[,]
	                 {
                          {"delmat_gid", Delmatexception.delmat_gid.ToString ()}
            
                     };
                string tblname = "iem_mst_tdelmat_adt";

                string updcomm = comm.UpdateCommon(codes1, whrcol, tblname);
                
                return res1;
            }
            return res1;
        }


        public string InsertDelmat(iem_mst_delmat Delmat, string[] Selecteddepartment)
        {
            DataTable dt = new DataTable();
            CommonIUD comm = new CommonIUD();
            string res1 = string.Empty;
            try
            {
                //SqlCommand cmdslab = new SqlCommand("pr_iem_mst_delmat", con);
                //GetCon();
                //cmdslab.Parameters.AddWithValue("@action", "Duplicate_slab");
                //cmdslab.Parameters.AddWithValue("@gid", Delmat.delmat_slab_gid);              
                //cmdslab.Parameters.Add("@res", SqlDbType.VarChar, 45).Direction = ParameterDirection.Output;
                //cmdslab.CommandType = CommandType.StoredProcedure;
                //cmdslab.ExecuteNonQuery();
                //string res1slab = cmdslab.Parameters["@res"].Value.ToString();
                //if (res1slab == "Not There")
                //{

                if (Delmat.delmat_type_ID == 7 && Delmat.delmat_wwoparpr == "O") // ramya added on 09 jun 22,modified on 13 jun 22
                {
                    Delmat.delmat_pipit_flag = "I";
                    Delmat.delmat_it_flag = "A";
                }
                
                foreach (string selctedepartment in Selecteddepartment)
                {

                    SqlCommand cmd = new SqlCommand("pr_iem_mst_delmat", con);
                    GetCon();
                    cmd.Parameters.AddWithValue("@action", "Exist");
                    cmd.Parameters.AddWithValue("@delmat_branchtype_flag", Delmat.delmat_branchtype_flag);
                    cmd.Parameters.AddWithValue("@delmat_slab_gid", Delmat.delmat_slab_gid);
                    cmd.Parameters.AddWithValue("@delmat_delmattype_gid", Delmat.delmat_type_ID);
                    cmd.Parameters.AddWithValue("@delmat_branch_flag", Delmat.delmat_branch_flag);
                    cmd.Parameters.AddWithValue("@delmat_claim_flag", Delmat.delmat_claim_flag);
                    cmd.Parameters.AddWithValue("@delmat_dsa_flag", Delmat.delmat_dsa_flag);
                    cmd.Parameters.AddWithValue("@delmat_pipit_flag", Delmat.delmat_pipit_flag);
                    cmd.Parameters.AddWithValue("@delmat_it_flag", Delmat.delmat_it_flag);
                    cmd.Parameters.AddWithValue("@delmat_exp_flag", Delmat.delmat_exp_flag);
                    cmd.Parameters.AddWithValue("@delmat_active_flag", Delmat.delmat_active);
                    cmd.Parameters.AddWithValue("@delmat_budget_flag", Delmat.delmat_budget_flag);
                    cmd.Parameters.AddWithValue("@delmat_parpr", Delmat.delmat_parpr_flag);
                    cmd.Parameters.AddWithValue("@delmat_woparpr", Delmat.delmat_wwoparpr);
                    cmd.Parameters.AddWithValue("@delmat_ecftype", Delmat.delmat_ecftype); // Muthu Added on 25-May-2022 
                    if (selctedepartment != "")
                    {
                        cmd.Parameters.AddWithValue("@delmatdept_dept_gid", SqlDbType.Int).Value = Convert.ToInt32(selctedepartment);
                    }
                    cmd.Parameters.Add("@res", SqlDbType.VarChar, 45).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                    string res = cmd.Parameters["@res"].Value.ToString();
                    if (res == "Exist")
                    {
                        res1 = "Exist";
                    }
                }
                //get delmat status

                int delmatstatus = 0;
                GetCon();
                DataTable dt1 = new DataTable();
                SqlCommand cmd1 = new SqlCommand("pr_delmatstatus ", con);
                cmd1.Parameters.AddWithValue("@action", "Maker");

                cmd1.CommandType = CommandType.StoredProcedure;

                da = new SqlDataAdapter(cmd1);
                da.Fill(dt1);
                if (dt1.Rows.Count > 0)
                {
                    delmatstatus = Convert.ToInt32(dt1.Rows[0]["ecfstatus_status"].ToString());
                }

                GetCon();
                
                if (res1 != "Exist")
                {
                    string[,] codes = new string[,]    
          
	               {        
        {"delmat_slab_gid",Convert.ToString (Delmat.delmat_slab_gid)},
        {"delmat_delmattype_gid",Convert.ToString(Delmat.delmat_type_ID)},
        {"delmat_name",Convert.ToString(Delmat.delmat_name)},
        {"delmat_branch_flag",Convert.ToString (Delmat.delmat_branch_flag)},
        {"delmat_branchtype_flag",Convert.ToString(Delmat.delmat_branchtype_flag)},
        {"delmat_claim_flag",Convert.ToString (Delmat.delmat_claim_flag)},
        {"delmat_dsa_flag",Convert.ToString(Delmat.delmat_dsa_flag)},
        {"delmat_pipit_flag",Convert.ToString (Delmat.delmat_pipit_flag)},
        {"delmat_it_flag",Convert.ToString(Delmat.delmat_it_flag)},
        {"delmat_exp_flag",Convert.ToString (Delmat.delmat_exp_flag)},
        {"delmat_budget_flag",Convert.ToString(Delmat.delmat_budget_flag)},
        {"delmat_jump_flag",Convert.ToString (Delmat.delmat_jump_flag)},
        {"delmat_active",Convert.ToString (Delmat.delmat_active)},
        {"delmat_insert_by",Convert.ToString (Delmat.delmat_insert_by)},
        {"delmat_insert_date","sysdatetime()"}, 
        //add delmat changes
                 {"delmat_ecftype",Convert.ToString (Delmat.delmat_ecftype)},	 
           {"delmat_parpr_flag",Convert.ToString (Delmat.delmat_parpr_flag)},
           {"delmat_status",Convert.ToString (delmatstatus)},
                   {"delmat_wwoparpr",Convert.ToString (Delmat.delmat_wwoparpr)},
                    {"delmat_checkerby","0"},	 
           {"delmat_checkerinsert_date","1900-01-01"},
                   {"delmat_checker_remark",""} 
                   //ramya added on 18 apr 22

                  };
                    string tname = "iem_mst_tdelmat_adt";
                    string insertcommend = comm.InsertCommon(codes, tname);

                }
                else
                {
                    return "Duplicate record !";
                }
                //}
                //else
                //{
                //    return "Duplicate record !";
                //}
            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }
            return "success";
        }


        public DelmatVisisbleById GetdelmaBranchActivityById(int DelmatId)
        {
            try
            {
                List<DelmatVisisbleById> objOrgType = new List<DelmatVisisbleById>();
                DelmatVisisbleById objModel;
                GetCon();
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand("select * from iem_mst_tdelmat_adt where delmat_gid='" + DelmatId + "' and delmat_isremoved='N'", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                objModel = new DelmatVisisbleById()
                {
                    delmattype_branch_visible = Convert.ToString(dt.Rows[0]["delmat_branch_flag"].ToString()),
                    delmattype_branchtype_visible = Convert.ToString(dt.Rows[0]["delmat_branchtype_flag"].ToString()),
                    delmattype_claim_visible = Convert.ToString(dt.Rows[0]["delmat_claim_flag"].ToString()),
                    delmattype_dsa_visible = Convert.ToString(dt.Rows[0]["delmat_dsa_flag"].ToString()),
                    delmattype_pipit_visible = Convert.ToString(dt.Rows[0]["delmat_pipit_flag"].ToString()),
                    delmattype_it_visible = Convert.ToString(dt.Rows[0]["delmat_it_flag"].ToString()),
                    delmattype_exp_visible = Convert.ToString(dt.Rows[0]["delmat_exp_flag"].ToString()),
                    delmattype_budget_visible = Convert.ToString(dt.Rows[0]["delmat_budget_flag"].ToString()),
                    delmattype_jump_visible = Convert.ToString(dt.Rows[0]["delmat_jump_flag"].ToString()),
                    delmattype_active_visible = Convert.ToString(dt.Rows[0]["delmat_active"].ToString()),

                    //add delmat change

                    delmattype_ecftype_visible = Convert.ToString(dt.Rows[0]["delmat_ecftype"].ToString()),
                    delmattype_parpr_visible = Convert.ToString(dt.Rows[0]["delmat_parpr_flag"].ToString()),
                    delmattype_withwithoutparpr_visible = Convert.ToString(dt.Rows[0]["delmat_wwoparpr"].ToString()),

                };
                return objModel;


                //return objOrgType;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
            // throw new NotImplementedException();
        }


        public DataTable GetDelmatnameById(int delmatId)
        {
            DataTable dt = new DataTable();
            try
            {
                List<GetSlab> objCountrytype = new List<GetSlab>();
                GetCon();
                // ramya added delmat_ecftype,delmat_parpr_flag,delmat_wwoparpr on 18 may 22
                SqlCommand cmd = new SqlCommand("select delmat_name,delmat_gid,delmat_ecftype,delmat_parpr_flag,delmat_wwoparpr from iem_mst_tdelmat_adt where delmat_gid=" + delmatId + " and delmat_isremoved='N'", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
            return dt;
            //throw new NotImplementedException();
        }


        public DataTable Getdelmattypename(int id)
        {
            DataTable dc = new DataTable();
            try
            {
                List<GetDelmat> objCountrytype = new List<GetDelmat>();
                SqlCommand cmd = new SqlCommand("pr_iem_mst_delmat", con);
                GetCon();
                cmd.Parameters.AddWithValue("@action", "Getdelmattypename");
                cmd.Parameters.AddWithValue("@gid", id);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dc);
            }
            catch (Exception ex)
            {
                con.Close();

            }
            return dc;
            // throw new NotImplementedException();
        }


        public DataTable Getdelmattyprgid(int id)
        {
            DataTable dc = new DataTable();
            try
            {
                List<GetDelmat> objCountrytype = new List<GetDelmat>();
                SqlCommand cmd = new SqlCommand("pr_iem_mst_delmat", con);
                GetCon();
                cmd.Parameters.AddWithValue("@action", "GetdelmattypeGid");
                cmd.Parameters.AddWithValue("@gid", id);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dc);
            }
            catch (Exception ex)
            {
                con.Close();

            }
            return dc;
            //throw new NotImplementedException();
        }


        public DataTable GetSlabGid(int slaid)
        {
            DataTable dc = new DataTable();
            try
            {
                List<GetDelmat> objCountrytype = new List<GetDelmat>();
                SqlCommand cmd = new SqlCommand("pr_iem_mst_delmat", con);
                GetCon();
                cmd.Parameters.AddWithValue("@action", "GetSlabGid");
                cmd.Parameters.AddWithValue("@gid", slaid);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dc);
            }
            catch (Exception ex)
            {
                con.Close();

            }
            return dc;
            // throw new NotImplementedException();
        }

        public DataTable Getslabname(int slabid)
        {
            DataTable dc = new DataTable();
            try
            {
                List<GetDelmat> objCountrytype = new List<GetDelmat>();
                SqlCommand cmd = new SqlCommand("pr_iem_mst_delmat", con);
                GetCon();
                cmd.Parameters.AddWithValue("@action", "GetSlabName");
                cmd.Parameters.AddWithValue("@gid", slabid);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dc);
            }
            catch (Exception ex)
            {
                con.Close();

            }
            return dc;
            // throw new NotImplementedException();
        }


        public DataTable Getdelmatdeptgid(int delmat_gid)
        {
            DataTable dc = new DataTable();
            try
            {
                List<GetDelmat> objCountrytype = new List<GetDelmat>();
                SqlCommand cmd = new SqlCommand("pr_iem_mst_delmat", con);
                GetCon();
                cmd.Parameters.AddWithValue("@action", "Getdelmatdeptgid");
                cmd.Parameters.AddWithValue("@gid", delmat_gid);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dc);
            }
            catch (Exception ex)
            {
                con.Close();

            }
            return dc;
            //  throw new NotImplementedException();
        }


        public string UpdateDelmat(iem_mst_delmat Delmat, string[] Selecteddepartment)
        {
            DataTable dt = new DataTable();
            CommonIUD comm = new CommonIUD();
            string res1 = string.Empty;
            try
            {
                //SqlCommand cmdslab = new SqlCommand("pr_iem_mst_delmat", con);
                //GetCon();
                //cmdslab.Parameters.AddWithValue("@action", "Duplicate_slab");
                //cmdslab.Parameters.AddWithValue("@gid", Delmat.delmat_slab_gid);              
                //cmdslab.Parameters.Add("@res", SqlDbType.VarChar, 45).Direction = ParameterDirection.Output;
                //cmdslab.CommandType = CommandType.StoredProcedure;
                //cmdslab.ExecuteNonQuery();
                //string res1slab = cmdslab.Parameters["@res"].Value.ToString();
                //if (res1slab == "Not There")
                //{

                if (Delmat.delmat_type_ID == 7 && Delmat.delmat_wwoparpr == "O") // ramya added on 09 jun 22,modified on 13 jun 22
                {
                    Delmat.delmat_pipit_flag = "I";
                    Delmat.delmat_it_flag = "A";
                }

                foreach (string selctedepartment in Selecteddepartment)
                {

                    SqlCommand cmd = new SqlCommand("pr_iem_mst_delmat", con);
                    GetCon();
                    cmd.Parameters.AddWithValue("@action", "UpdateExist");
                    cmd.Parameters.AddWithValue("@gid", Delmat.delmat_gid.ToString());
                    cmd.Parameters.AddWithValue("@delmat_branchtype_flag", Delmat.delmat_branchtype_flag);
                    cmd.Parameters.AddWithValue("@delmat_slab_gid", Delmat.delmat_slab_gid);
                    cmd.Parameters.AddWithValue("@delmat_delmattype_gid", Delmat.delmat_type_ID);
                    cmd.Parameters.AddWithValue("@delmat_branch_flag", Delmat.delmat_branch_flag);
                    cmd.Parameters.AddWithValue("@delmat_claim_flag", Delmat.delmat_claim_flag);
                    cmd.Parameters.AddWithValue("@delmat_dsa_flag", Delmat.delmat_dsa_flag);
                    cmd.Parameters.AddWithValue("@delmat_pipit_flag", Delmat.delmat_pipit_flag);
                    cmd.Parameters.AddWithValue("@delmat_it_flag", Delmat.delmat_it_flag);
                    cmd.Parameters.AddWithValue("@delmat_exp_flag", Delmat.delmat_exp_flag);
                    cmd.Parameters.AddWithValue("@delmat_active_flag", Delmat.delmat_active);
                    cmd.Parameters.AddWithValue("@delmat_budget_flag", Delmat.delmat_budget_flag);
                    cmd.Parameters.AddWithValue("@delmat_parpr", Delmat.delmat_parpr_flag);
                    cmd.Parameters.AddWithValue("@delmat_woparpr", Delmat.delmat_wwoparpr);
                    cmd.Parameters.AddWithValue("@delmat_ecftype", Delmat.delmat_ecftype); // Muthu Added on 25-May-2022 
                    if (selctedepartment != "")
                    {
                        cmd.Parameters.AddWithValue("@delmatdept_dept_gid", SqlDbType.Int).Value = Convert.ToInt32(selctedepartment);
                    }
                    cmd.Parameters.Add("@res", SqlDbType.VarChar, 45).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                    string res = cmd.Parameters["@res"].Value.ToString();
                    if (res == "Exist")
                    {
                        res1 = "Exist";
                    }
                }
                //get delmat status

                int delmatstatus = 0;
                GetCon();
                DataTable dt1 = new DataTable();
                SqlCommand cmd1 = new SqlCommand("pr_delmatstatus ", con);
                cmd1.Parameters.AddWithValue("@action", "Maker");

                cmd1.CommandType = CommandType.StoredProcedure;

                da = new SqlDataAdapter(cmd1);
                da.Fill(dt1);
                if (dt1.Rows.Count > 0)
                {
                    delmatstatus = Convert.ToInt32(dt1.Rows[0]["ecfstatus_status"].ToString());
                }

                GetCon();

                if (res1 != "Exist")
                {

                    string[,] codes = new string[,]   
          
	               {        
        {"delmat_slab_gid",Convert.ToString (Delmat.delmat_slab_gid)},
        {"delmat_delmattype_gid",Convert.ToString(Delmat.delmat_type_ID)},
        {"delmat_name",Convert.ToString(Delmat.delmat_name)},
        {"delmat_branch_flag",Convert.ToString (Delmat.delmat_branch_flag)},
        {"delmat_branchtype_flag",Convert.ToString(Delmat.delmat_branchtype_flag)},
        {"delmat_claim_flag",Convert.ToString (Delmat.delmat_claim_flag)},
        {"delmat_dsa_flag",Convert.ToString(Delmat.delmat_dsa_flag)},
        {"delmat_pipit_flag",Convert.ToString (Delmat.delmat_pipit_flag)},
        {"delmat_it_flag",Convert.ToString(Delmat.delmat_it_flag)},
        {"delmat_exp_flag",Convert.ToString (Delmat.delmat_exp_flag)},
        {"delmat_budget_flag",Convert.ToString(Delmat.delmat_budget_flag)},
        {"delmat_jump_flag",Convert.ToString (Delmat.delmat_jump_flag)},
        {"delmat_active",Convert.ToString (Delmat.delmat_active)},
        {"delmat_update_by",Convert.ToString (Delmat.delmat_update_by)},
        {"delmat_update_date","sysdatetime()"},	 
        //add delmat changes
                 {"delmat_ecftype",Convert.ToString (Delmat.delmat_ecftype)},	 
           {"delmat_parpr_flag",Convert.ToString (Delmat.delmat_parpr_flag)},
                   {"delmat_wwoparpr",Convert.ToString (Delmat.delmat_wwoparpr)},
                     {"delmat_checkerby","0"},	 
           {"delmat_checkerinsert_date","1900-01-01"},
                   {"delmat_checker_remark",""},
                   {"delmat_status","0"}
                  };
                    string[,] whrcol = new string[,]
	                 {
                          {"delmat_gid", Delmat.delmat_gid.ToString ()}
            
                     };
                    string tblname = "iem_mst_tdelmat_adt";

                    string updcomm = comm.UpdateCommon(codes, whrcol, tblname);
                    //}
                }
                else
                {
                    return "Duplicate record !";
                }


            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }
            return "success";
            //  throw new NotImplementedException();
        }


        public DataTable GetdelmatexceptionById(int exceptionid)
        {
            DataTable dc = new DataTable();
            try
            {
                List<GetDelmat> objCountrytype = new List<GetDelmat>();
                SqlCommand cmd = new SqlCommand("pr_iem_mst_delmat", con);
                GetCon();
                cmd.Parameters.AddWithValue("@action", "GetdelmatexceptionByID");
                cmd.Parameters.AddWithValue("@gid", exceptionid);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dc);
            }
            catch (Exception ex)
            {
                con.Close();

            }
            return dc;
            //throw new NotImplementedException();
        }

        //getdelmatflowinformation

        public DataTable GetflowgidCount(int delmatgid)
        {
            DataTable dc = new DataTable();
            try
            {
                List<GetDelmat> objCountrytype = new List<GetDelmat>();
                SqlCommand cmd = new SqlCommand("pr_iem_mst_delmat", con);
                GetCon();
                cmd.Parameters.AddWithValue("@action", "GetflowgidCount");
                cmd.Parameters.AddWithValue("@gid", delmatgid);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dc);
            }
            catch (Exception ex)
            {
                con.Close();

            }
            return dc;
            // throw new NotImplementedException();
        }
        public DataTable GetflowgidCountdel(int delmatgid)
        {
            DataTable dc = new DataTable();
            try
            {
                List<GetDelmat> objCountrytype = new List<GetDelmat>();
                SqlCommand cmd = new SqlCommand("pr_iem_mst_delmat", con);
                GetCon();
                cmd.Parameters.AddWithValue("@action", "GetflowgidCountedit");
                cmd.Parameters.AddWithValue("@gid", delmatgid);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dc);
            }
            catch (Exception ex)
            {
                con.Close();

            }
            return dc;
            // throw new NotImplementedException();
        }
        public DataTable SelectDelmatGidCount(int delmatgid)
        {
            DataTable dc = new DataTable();
            try
            {
                List<GetDelmat> objCountrytype = new List<GetDelmat>();
                SqlCommand cmd = new SqlCommand("pr_iem_mst_delmat", con);
                GetCon();
                cmd.Parameters.AddWithValue("@action", "SelectDelmatGidCount");
                cmd.Parameters.AddWithValue("@gid", delmatgid);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dc);
            }
            catch (Exception ex)
            {
                con.Close();

            }
            return dc;
            // throw new NotImplementedException();
        }



        public DataTable SelectDelmatFlowOrder(int InDelamtGid)
        {
            DataTable dc = new DataTable();
            try
            {
                List<GetDelmat> objCountrytype = new List<GetDelmat>();
                SqlCommand cmd = new SqlCommand("pr_iem_mst_delmat", con);
                GetCon();
                cmd.Parameters.AddWithValue("@action", "SelectDelmatFlowOrder");
                cmd.Parameters.AddWithValue("@gid", InDelamtGid);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dc);
            }
            catch (Exception ex)
            {
                con.Close();

            }
            return dc;
            //throw new NotImplementedException();
        }

        public DataTable SelectTitleGid(int InDelamtGid)
        {
            DataTable dc = new DataTable();
            try
            {
                List<GetDelmat> objCountrytype = new List<GetDelmat>();
                SqlCommand cmd = new SqlCommand("pr_iem_mst_delmat", con);
                GetCon();
                cmd.Parameters.AddWithValue("@action", "SelectTitleGid");
                cmd.Parameters.AddWithValue("@gid", InDelamtGid);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dc);
            }
            catch (Exception ex)
            {
                con.Close();

            }
            return dc;
            //  throw new NotImplementedException();
        }

        public DataTable SelectDelmatTitleRefGid(int InDelamtGid)
        {
            DataTable dc = new DataTable();
            try
            {
                List<GetDelmat> objCountrytype = new List<GetDelmat>();
                SqlCommand cmd = new SqlCommand("pr_iem_mst_delmat", con);
                GetCon();
                cmd.Parameters.AddWithValue("@action", "SelectDelmatTitleRefGid");
                cmd.Parameters.AddWithValue("@gid", InDelamtGid);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dc);
            }
            catch (Exception ex)
            {
                con.Close();

            }
            return dc;
            //throw new NotImplementedException();
        }



        public DataTable GetSlabRangeNameBySlabrangeId(int SlabrangeId, int delmatgid)
        {
            DataTable dc = new DataTable();
            try
            {
                List<GetDelmat> objCountrytype = new List<GetDelmat>();
                SqlCommand cmd = new SqlCommand("pr_iem_mst_delmat", con);
                GetCon();
                cmd.Parameters.AddWithValue("@action", "SelectdelmatmatrixAccess");
                cmd.Parameters.AddWithValue("@gid", delmatgid);
                cmd.Parameters.AddWithValue("@slabrangegid", SlabrangeId);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dc);
                if (dc.Rows.Count < 0)
                {
                    dc.Rows.Add("N");
                }
            }
            catch (Exception ex)
            {
                con.Close();

            }
            return dc;
            // throw new NotImplementedException();
        }


        public string DeleteDelmatInformation(iem_mst_delmat Delmatinformation)
        {
            CommonIUD delecomm = new CommonIUD();
            string col_value = string.Empty;
            string col_temp = string.Empty;
            try
            {
                //delete for delmattable
                string[,] codes = new string[,]
	       {
                 {"delmat_isremoved", "Y"}
	            
           };
                string[,] whrcol = new string[,]
	        {
                {"delmat_gid", Delmatinformation.delmat_gid.ToString ()}
            };
                string tblname = "iem_mst_tdelmat_adt";


                string deletetbl = delecomm.DeleteCommon(codes, whrcol, tblname);

                //delete for delmatflow
                string[,] codes1 = new string[,]
	       {
                 {"delmatflow_isremoved", "Y"}
	            
           };
                string[,] whrcol1 = new string[,]
	        {
                {"delmatflow_delmat_gid", Delmatinformation.delmatsetflowgid.ToString ()}
            };
                string tblname1 = "iem_mst_tdelmatflow_adt";


                string deletetbl1 = delecomm.DeleteCommon(codes1, whrcol1, tblname1);

                //delete for delmatmatrix
                string[,] codes2 = new string[,]
	       {
                 {"delmatmatrix_isremoved", "Y"}
	            
           };
                string[,] whrcol2 = new string[,]
	        {
                {"delmatmatrix_delmat_gid", Delmatinformation.delmatmatrixgid.ToString ()}
            };
                string tblname2 = "iem_mst_tdelmatmatrix_adt";


                string deletetbl2 = delecomm.DeleteCommon(codes2, whrcol2, tblname2);


                string[,] codes21 = new string[,]
	       {
                 {"delmatexception_isremoved", "Y"}
	            
           };
                string[,] whrcol21 = new string[,]
	        {
                {"delmatexception_delmat_gid", Delmatinformation.delmatmatrixgid.ToString ()}
            };
                string tblname21 = "iem_mst_tdelmatexception_adt";


                string deletetbl21 = delecomm.DeleteCommon(codes21, whrcol21, tblname21);


                string[,] codes212 = new string[,]
	       {
                 {"delmatdept_isremoved", "Y"}
	            
           };
                string[,] whrcol212 = new string[,]
	        {
                {"delmatdept_delmat_gid", Delmatinformation.delmatmatrixgid.ToString ()}
            };
                string tblname212 = "iem_mst_tdelmatdept_adt";


                string deletetbl212 = delecomm.DeleteCommon(codes212, whrcol212, tblname212);
                // ramya added on 17 jun 22 to delete tran 
                DataTable dc = new DataTable();
                SqlCommand cmd = new SqlCommand("pr_iem_mst_delmat", con);
                GetCon();
                cmd.Parameters.AddWithValue("@action", "DelmatDelete");
                cmd.Parameters.AddWithValue("@gid", Delmatinformation.delmat_gid.ToString()); 
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
            return "Record Deleted Successfully !";
            //  throw new NotImplementedException();
        }

        public string delcount(int InDelamtGid)
        {
            try
            {
                string result = "";
                GetCon();
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand("select count(slabrange_name) as slabrange_name from iem_mst_tslabrange join iem_mst_tdelmat_adt on slabrange_slab_gid=delmat_slab_gid where slabrange_isremoved='N'  and delmat_isremoved='N' and delmat_gid=" + InDelamtGid + "", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    result = dt.Rows[0]["slabrange_name"].ToString();
                }
                return result;
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

        public DataTable SelectAddApproval(int InDelamtGid, int flowgid)
        {
            DataTable dc = new DataTable();
            try
            {
                List<GetDelmat> objCountrytype = new List<GetDelmat>();
                SqlCommand cmd = new SqlCommand("pr_iem_mst_delmat", con);
                GetCon();
                cmd.Parameters.AddWithValue("@action", "SelectMatrixAccess");
                cmd.Parameters.AddWithValue("@gid", InDelamtGid);
                cmd.Parameters.AddWithValue("@delmatflowgid", flowgid);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dc);
            }
            catch (Exception ex)
            {
                con.Close();

            }
            return dc;
            // throw new NotImplementedException();
        }
        // throw new NotImplementedException();


        public void UpdateDelmatMatrix(iem_mst_delmat Delmatmatrix)
        {
            bool flag = true;
            CommonIUD comm = new CommonIUD();
            string res1;
            string Mataccess = string.Empty;
            try
            {
                SqlCommand cmd = new SqlCommand("pr_iem_mst_delmat", con);
                GetCon();
                cmd.Parameters.AddWithValue("@action", "CheckexistdelmatgidInDelmatFlow");
                cmd.Parameters.AddWithValue("@gid", Delmatmatrix.delmat_gid);
                cmd.Parameters.AddWithValue("@slabrangegid", Delmatmatrix.slabrange_gid);
                cmd.Parameters.AddWithValue("@delmatflowgid", Delmatmatrix.GID);
                cmd.Parameters.Add("@res", SqlDbType.VarChar, 45).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                res1 = cmd.Parameters["@res"].Value.ToString();

                if (res1 == "Not There")
                {
                    flag = true;
                }
                if (res1 == "Exist")
                {
                    flag = false;
                }



                if (res1 == "Exist")
                {

                    string[,] codes = new string[,]    
          
	               {              
         
                   {"delmatmatrix_access",Delmatmatrix.slabrange_name},  
              
                  };

                    string[,] whrcol = new string[,]
	                     {
                         
                          {"delmatmatrix_delmatflow_gid",Delmatmatrix.GID.ToString ()},
                           {"delmatmatrix_slabrange_gid", Delmatmatrix.slabrange_gid.ToString ()},

                         };
                    string tblname = "iem_mst_tdelmatmatrix_adt";
                    string updcomm = comm.UpdateCommon(codes, whrcol, tblname);


                    flag = false;
                }

                if (flag == true)
                {

                    string[,] codes = new string[,]                     
	               {        
        {"delmatmatrix_delmat_gid",Convert.ToString (Delmatmatrix.delmat_gid)},
        {"delmatmatrix_slabrange_gid",Convert.ToString(Delmatmatrix.slabrange_gid)},     
        {"delmatmatrix_delmatflow_gid",Convert.ToString(Delmatmatrix.GID)},       
        {"delmatmatrix_access",Delmatmatrix.slabrange_name},  
        
             
                  };
                    string tname = "iem_mst_tdelmatmatrix_adt";
                    string insertcommend = comm.InsertCommon(codes, tname);
                    flag = false;
                }

                //ramya added on 18 apr 22
                string[,] codes2 = new string[,]   
          
	               {        
        
        {"delmat_update_by",Convert.ToString (Delmatmatrix.delmat_update_by)},
        {"delmat_update_date","sysdatetime()"},	 
                     {"delmat_checkerby","0"},	 
           {"delmat_checkerinsert_date","1900-01-01"},
                   {"delmat_checker_remark",""},
                   {"delmat_status","0"}
                  };
                string[,] whrcol1 = new string[,]
	                 {
                          {"delmat_gid", Delmatmatrix.delmat_gid.ToString ()}
            
                     };
                string tblname1 = "iem_mst_tdelmat_adt";

              string updcomm1 = comm.UpdateCommon(codes2, whrcol1, tblname1);
                                
            }
            catch (Exception ex)
            {

            }
            finally
            { }
            //  throw new NotImplementedException();
        }


        public DataTable getdelmatException(string delmatgid)
        {
            DataTable dt = new DataTable();
            try
            {
                List<iem_mst_delmat> objOrgType = new List<iem_mst_delmat>();

                GetCon();
                SqlCommand cmd = new SqlCommand("pr_iem_mst_delmat", con);
                cmd.Parameters.AddWithValue("@gid", delmatgid);
                cmd.Parameters.AddWithValue("@action", "selectDelmatException");
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
            return dt;
        }


        public DataTable GetslabrangeGid(int SlabGid)
        {
            DataTable dt = new DataTable();
            try
            {
                List<GetSlab> objCountrytype = new List<GetSlab>();
                GetSlab objModel;
                GetCon();
                // slanrangename.Replace("\n", "");
                //slanrangename.Replace
                // SqlCommand cmd = new SqlCommand("select slabrange_gid from iem_mst_tslabrange where slabrange_slab_gid='" + SlabGid + "' and slabrange_name='"+slanrangename+"'", con);
                SqlCommand cmd = new SqlCommand("select slabrange_gid from iem_mst_tslabrange where slabrange_slab_gid='" + SlabGid + "' and slabrange_isremoved='N'", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
            return dt;
            // throw new NotImplementedException();
        }


        public string InsertDelmatMatrix(iem_mst_delmat Delmatmatrix)
        {
            string insertcommend;
            try
            {
                CommonIUD comm = new CommonIUD();
                string[,] codes = new string[,]    
          
	               {        
        {"delmatmatrix_delmat_gid",Convert.ToString (Delmatmatrix.delmat_gid)},
        {"delmatmatrix_slabrange_gid",Convert.ToString(Delmatmatrix.Slabrangegid)},     
        {"delmatmatrix_delmatflow_gid",Convert.ToString(Delmatmatrix.GID)},
        {"delmatmatrix_access",Convert.ToString (Delmatmatrix.slabrange_name)},         
                  };
                string tname = "iem_mst_tdelmatmatrix_adt";
                insertcommend = comm.InsertCommon(codes, tname);


                //ramya added on 18 apr 22
                string[,] codes2 = new string[,]   
          
	               {        
        
        {"delmat_update_by",Convert.ToString (Delmatmatrix.delmat_update_by)},
        {"delmat_update_date","sysdatetime()"},	 
                     {"delmat_checkerby","0"},	 
           {"delmat_checkerinsert_date","1900-01-01"},
                   {"delmat_checker_remark",""},
                   {"delmat_status","0"}
                  };
                string[,] whrcol1 = new string[,]
	                 {
                          {"delmat_gid", Delmatmatrix.delmat_gid.ToString ()}
            
                     };
                string tblname1 = "iem_mst_tdelmat_adt";

                string updcomm1 = comm.UpdateCommon(codes2, whrcol1, tblname1);
               

            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
            return "success";
            // throw new NotImplementedException();
        }
        public DataTable UpdateDelmateStatus(string delmatgid)
        {
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                int delmatstatus = 0;
                Int64 loginid = Convert.ToInt64(objCmnFunctions.GetLoginUserGid().ToString());
                GetCon();
                DataTable dt1 = new DataTable();
                SqlCommand cmd1 = new SqlCommand("pr_delmatstatus ", con);
                cmd1.Parameters.AddWithValue("@action", "MakerSubmit");
                cmd1.CommandType = CommandType.StoredProcedure;
                da = new SqlDataAdapter(cmd1);
                da.Fill(dt1);
                if (dt1.Rows.Count > 0)
                {
                    delmatstatus = Convert.ToInt32(dt1.Rows[0]["ecfstatus_status"].ToString());
                }

                GetCon();
                // ramya modified update query on 11 Apr 22
                SqlCommand cmd = new SqlCommand("update iem_mst_tdelmat_adt set delmat_checkerinsert_date=null,delmat_checkerby=null,delmat_checker_remark=null,delmat_update_date=getdate(),delmat_update_by=" + loginid + ", delmat_status='" + delmatstatus + "'" + " where delmat_gid='" + delmatgid + "' and delmat_isremoved='N'", con);
                //SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);


                DataTable dt2 = new DataTable();
                SqlCommand cmd2 = new SqlCommand("pr_trn_set_tdelmataudittrail ", con);
                cmd2.Parameters.AddWithValue("@delmat_gid", delmatgid);
                cmd2.Parameters.AddWithValue("@delmat_Actionby", loginid);
                cmd2.Parameters.AddWithValue("@delmat_role", "Maker");
                cmd2.Parameters.AddWithValue("@delmat_remarks", "");
                cmd2.Parameters.AddWithValue("@delmat_status", "Submitted");
                cmd2.CommandType = CommandType.StoredProcedure;
                da = new SqlDataAdapter(cmd2);
                da.Fill(dt2);
                 
            }
            catch (Exception ex)
            {
                con.Close();

            }
            return dt;
        }
        
        public IEnumerable<iem_mst_delmat> AutocompleteEmployee(string EmployeeName, string EmployeeCode)
        {
            List<iem_mst_delmat> emp = new List<iem_mst_delmat>();
            try
            {
                DataTable dt = new DataTable();
                iem_mst_delmat objModel;
                cmd = new SqlCommand("pr_iem_trn_tcentralinward", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@RAISERNAME", SqlDbType.VarChar).Value = EmployeeName;
                cmd.Parameters.Add("@RAISERCODE", SqlDbType.VarChar).Value = EmployeeCode;
                //cmd.Parameters.Add("@DEPARTMENT", SqlDbType.VarChar).Value = EmployeeName;
                cmd.Parameters.Add("@ACTION", SqlDbType.VarChar).Value = "AutocompleteEmployee";
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    objModel = new iem_mst_delmat();
                    objModel.employee_gid = Convert.ToInt32(row["employee_gid"].ToString());
                    objModel.employee_code = row["employee_name"].ToString();

                    emp.Add(objModel);
                }

            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }
            return emp;
        }
        
        DataSet Iiem_mst_delmat.AutocompleteEmployee(string EmployeeName, string EmployeeCode)
        {
            throw new NotImplementedException();
        }
        
        //delmat Attachment summary
        public IEnumerable<AttachFile> GetDelmatAttachment(string delmatgid)
        {
            //  throw new NotImplementedException();
            try
            {
                List<AttachFile> objOrgType = new List<AttachFile>();
                AttachFile objModel;

                GetCon();
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand("Pr_Iem_delmat_get_Attachment", con);
                cmd.Parameters.AddWithValue("@Delmat_gid", delmatgid);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    objModel = new AttachFile();
                    objModel.AttachmentFilenameId = Convert.ToInt32(dt.Rows[i]["attachment_gid"].ToString());
                    objModel.AttachmentFilename = Convert.ToString(dt.Rows[i]["attachment_filename"].ToString());
                    objModel.AttachmentDescription = Convert.ToString(dt.Rows[i]["attachment_desc"].ToString());
                    objModel.attachment_by = Convert.ToString(dt.Rows[i]["attachment_by"].ToString());
                    objModel.AttachmentDate = Convert.ToString(dt.Rows[i]["attachment_date"].ToString());
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
        //AuditDelmat
        public IEnumerable<AuditFile> GetDelmatAudit(string delmatgid)
        {
            //  throw new NotImplementedException();
            try
            {
                List<AuditFile> objOrgType = new List<AuditFile>();
                AuditFile objModel;

                GetCon();
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand("[PR_FS_Get_DelmatAuditTrail]", con);
                cmd.Parameters.AddWithValue("@Delmat_gid", delmatgid);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    objModel = new AuditFile();
                    objModel.DeliD = Convert.ToInt32(dt.Rows[i]["DeliD"].ToString());
                    objModel.EmpCode = Convert.ToString(dt.Rows[i]["EmpCode"].ToString());
                    objModel.EmpName = Convert.ToString(dt.Rows[i]["EmpName"].ToString());
                    objModel.ActionDate = Convert.ToString(dt.Rows[i]["ActionDate"].ToString());
                    objModel.ActionType = Convert.ToString(dt.Rows[i]["ActionType"].ToString());
                    objModel.Remarks = Convert.ToString(dt.Rows[i]["Remarks"].ToString());
                    objModel.Designation = Convert.ToString(dt.Rows[i]["Designation"].ToString());
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

        //insert attachment
        public string InsertEmpAtt(HttpPostedFileBase savefile, AttachFile EmployeeeExpense, string delmatgid, string EmployeeeGid)
        {
            string Emp_Msg = "";
            string filename = "";
            string AttachmentDescriptiont = "";
            try
            {
                if (EmployeeeExpense.AttachmentDescription != null)
                {
                    AttachmentDescriptiont = objCmnFunctions.Getreplacesinglequotes(EmployeeeExpense.AttachmentDescription);
                }
                //filename = "\\192.168.71.119\RemoteShare\temp\CRP Attachments\E-CLAIMS\Invoi.xls";
                filename = savefile.FileName.ToString();
                int index = filename.LastIndexOf("\\");
                if (index != -1)
                {
                    string[] seqNum = new string[] { filename.Substring(0, index), filename.Substring(index + 1) };
                    if (seqNum.Length == 2)
                    {
                        filename = seqNum[1].ToString();
                    }
                    else
                    {
                        filename = savefile.FileName.ToString();
                    }
                }
                else
                {
                    filename = savefile.FileName.ToString();
                }

                //if (filename.Contains("Invoice") == true)
                //{

                //GSTPhase3_1

                string filenamenew = Path.GetFileNameWithoutExtension(savefile.FileName);
                if (EmployeeeExpense.AttachmentTypeName == "1")
                {
                    filenamenew = filenamenew.ToLower().Replace("_invoice", "").ToString();
                }


                GetCon();
                DataTable dt2 = new DataTable();
                int ref_gid = 0;
                cmd = new SqlCommand("Pr_DelmatAttachmentUpdate", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@action", SqlDbType.VarChar).Value = "Ref_flag";
                da = new SqlDataAdapter(cmd);
                da.Fill(dt2);
                if (dt2.Rows.Count > 0)
                {
                    ref_gid = Convert.ToInt32(dt2.Rows[0]["ref_gid"].ToString());
                }
                GetCon();
                DataTable dt1 = new DataTable();
                int Attachtype_gid = 0;
                cmd = new SqlCommand("Pr_DelmatAttachmentUpdate", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@action", SqlDbType.VarChar).Value = "Attachtype_flag";
                da = new SqlDataAdapter(cmd);
                da.Fill(dt1);
                if (dt1.Rows.Count > 0)
                {
                    Attachtype_gid = Convert.ToInt32(dt1.Rows[0]["attachmenttype_gid"].ToString());
                }


                string[,] codes = new string[,]
	               {
        {"attachment_ref_flag", Convert.ToInt32(ref_gid).ToString() },
	    {"attachment_ref_gid", delmatgid},
        {"attachment_filename",objCmnFunctions.Getreplacesinglequotes(filename.ToString()) },
        {"attachment_attachmenttype_gid",Convert.ToInt32(Attachtype_gid).ToString() },
	    {"attachment_desc", AttachmentDescriptiont},
	    {"attachment_date",  "sysdatetime()"},
        {"attachment_by", objCmnFunctions.GetLoginUserGid().ToString()},
        {"attachment_isremoved", "N"}
       //  {"attachment_invoice_gid", Convert.ToInt32(invoicegid).ToString()}
                  };
                string tname = "iem_trn_tattachment";

                string insertcommend = objCommonIUD.InsertCommon(codes, tname);

                GetCon();
                DataTable dt = new DataTable();
                cmd = new SqlCommand("pr_eow_mst_NatureofExpenses", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@para1", SqlDbType.VarChar).Value = delmatgid;
                cmd.Parameters.Add("@para2", SqlDbType.VarChar).Value = objCmnFunctions.GetLoginUserGid().ToString();
                cmd.Parameters.Add("@action", SqlDbType.VarChar).Value = "DelmatAttachmentDetailMaxgid";
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    Emp_Msg = Convert.ToString(dt.Rows[0]["attachment_gid"].ToString());
                }


                return Emp_Msg;
            }
            catch (Exception ex)
            {
                //objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return "";
            }
            finally
            {
                con.Close();
                da.Dispose();
            }
        }

        //download Delmat attachment
        public string downloadAttachment(int AttachmentGID, string delmatgid)
        {
            string Emp_Msg = "";
            try
            {
                GetCon();
                DataTable dt = new DataTable();
                cmd = new SqlCommand("pr_eow_mst_NatureofExpenses", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@para1", SqlDbType.VarChar).Value = delmatgid;
                cmd.Parameters.Add("@para2", SqlDbType.VarChar).Value = AttachmentGID;
                cmd.Parameters.Add("@action", SqlDbType.VarChar).Value = "DelmatAttachmentDetailMaxgname";
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    Emp_Msg = Convert.ToString(dt.Rows[0]["attachment_filename"].ToString());
                }
                return Emp_Msg;
            }
            catch (Exception ex)
            {
                //objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return "";
            }
            finally
            {
                con.Close();
                da.Dispose();
            }
        }

        //delete delmat attachment
        public string DeleteDelmatAttachment(int AttachmentGID, string delmatgid)
        {
            string delant = "0";
            try
            {
                string[,] codes = new string[,]
	               {
        {"attachment_isremoved","Y" }
                  };
                string[,] whcos = new string[,]
	               {
        {"attachment_gid",AttachmentGID.ToString() },
        {"attachment_ref_gid",delmatgid }
                  };
                string tname = "iem_trn_tattachment";

                string insertcommend = objCommonIUD.UpdateCommon(codes, whcos, tname);

                return delant;
            }
            catch (Exception ex)
            {
                //objErrorLog.WriteErrorLog(ex.Message.ToString(), ex.ToString());
                return "";
            }
            finally
            {

            }
        }
        // edit attachment

        public List<iem_mst_delmat> GetdelmatAttachmentById(string DematId)
        {
            try
            {
                List<iem_mst_delmat> objAttachment = new List<iem_mst_delmat>();
                iem_mst_delmat objModel;
                GetCon();
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand("pr_iem_mst_delmat", con);
                cmd.Parameters.AddWithValue("@action", "selectDelmatAttachmentById");
                cmd.Parameters.AddWithValue("@gid", DematId);
                /// cmd.Parameters.AddWithValue("@demattype_gid", Attachmentgid);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                //objOrgType = new AttachFile()
                //{
                //    AttachmentFilenameId = Convert.ToInt32(dt.Rows[0]["attachment_gid"].ToString()),
                //    AttachmentFilename = Convert.ToString(dt.Rows[0]["attachment_filename"].ToString()),
                //    AttachmentDescription = Convert.ToString(dt.Rows[0]["attachment_desc"].ToString()),
                //    attachment_by = Convert.ToString(dt.Rows[0]["attachment_by"].ToString()),
                //    AttachmentDate = Convert.ToString(dt.Rows[0]["attachment_date"].ToString()),

                //};
                //return objOrgType;





                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        objModel = new iem_mst_delmat();
                        objModel.AttachmentFilenameId = Convert.ToInt32(dt.Rows[i]["attachment_gid"].ToString());
                        objModel.AttachmentFilename = Convert.ToString(dt.Rows[i]["attachment_filename"].ToString());
                        // objModel.AttachmentTypeName = Convert.ToString(dt.Rows[i]["attachmenttype_name"].ToString());
                        objModel.AttachmentDate = Convert.ToString(dt.Rows[i]["attachment_date"].ToString());
                        objModel.AttachmentDescription = Convert.ToString(dt.Rows[i]["attachment_desc"].ToString());
                        //objModel.AttachmentBy = Convert.ToString(dt.Rows[i]["codename"].ToString());
                        objModel.attachment_by = Convert.ToString(dt.Rows[i]["attachment_by"].ToString());
                        objAttachment.Add(objModel);
                    }
                }
                return objAttachment;

                //return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
            // throw new NotImplementedException();
        }

        //Checker Action
        public string Insertapprovedaction(iem_mst_delmat delmat, string delmatgid, string empgid)
        {
            try
            {
                string Emp_Msg = "";
                if (delmat.Status == "Approve")
                {
                    int Delmatapprove = 0;
                    DataTable dt = new DataTable();
                    GetCon();
                    SqlCommand cmd = new SqlCommand("pr_delmatstatus", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@action", SqlDbType.VarChar).Value = "CheckerApp";
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        Delmatapprove = Convert.ToInt32(dt.Rows[0]["ecfstatus_status"].ToString());
                    }

                    DataTable dt1 = new DataTable();
                    GetCon();
                    SqlCommand cmd1 = new SqlCommand("pr_delmatCheckerUpdate", con);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.Add("@action", SqlDbType.VarChar).Value = "Approve";
                    cmd1.Parameters.Add("@delmatatatus", SqlDbType.Int).Value = Delmatapprove;
                    cmd1.Parameters.Add("@delmatgid", SqlDbType.Int).Value = delmatgid;
                    cmd1.Parameters.Add("@empgid", SqlDbType.Int).Value = empgid;
                    cmd1.Parameters.Add("@delmat_remarks", SqlDbType.VarChar).Value = String.IsNullOrEmpty(delmat.Remarks) ? "" : delmat.Remarks.ToString();
                    da = new SqlDataAdapter(cmd1);
                    da.Fill(dt1);

                    DataTable dt2 = new DataTable();
                    SqlCommand cmd2 = new SqlCommand("pr_trn_set_tdelmataudittrail ", con);
                    cmd2.Parameters.AddWithValue("@delmat_gid", delmatgid);
                    cmd2.Parameters.AddWithValue("@delmat_Actionby", empgid);
                    cmd2.Parameters.AddWithValue("@delmat_role", "Checker");
                    cmd2.Parameters.AddWithValue("@delmat_remarks", String.IsNullOrEmpty(delmat.Remarks) ? "" : delmat.Remarks.ToString());
                    cmd2.Parameters.AddWithValue("@delmat_status", "Approved");
                    cmd2.CommandType = CommandType.StoredProcedure;
                    da = new SqlDataAdapter(cmd2);
                    da.Fill(dt2);
                    if (dt1.Rows.Count > 0)
                    {
                        Emp_Msg = "Delmat Checker Submitted Successfully";
                    }
                }

                else if (delmat.Status == "Reject")
                {
                    int Delmatreject = 0;
                    DataTable dt = new DataTable();
                    GetCon();
                    cmd = new SqlCommand("pr_delmatstatus", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@action", SqlDbType.VarChar).Value = "CheckerRej";
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        Delmatreject = Convert.ToInt32(dt.Rows[0]["ecfstatus_status"].ToString());
                    }

                    DataTable dt1 = new DataTable();
                    GetCon();
                    cmd = new SqlCommand("pr_delmatCheckerUpdate", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@action", SqlDbType.VarChar).Value = "Reject";
                    cmd.Parameters.Add("@delmatatatus", SqlDbType.Int).Value = Delmatreject;
                    cmd.Parameters.Add("@delmatgid", SqlDbType.Int).Value = delmatgid;
                    cmd.Parameters.Add("@empgid", SqlDbType.Int).Value = empgid;
                    cmd.Parameters.Add("@delmat_remarks", SqlDbType.VarChar).Value = String.IsNullOrEmpty(delmat.Remarks) ? "" : delmat.Remarks.ToString();
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dt1);

                    DataTable dt2 = new DataTable();
                    SqlCommand cmd2 = new SqlCommand("pr_trn_set_tdelmataudittrail ", con);
                    cmd2.Parameters.AddWithValue("@delmat_gid", delmatgid);
                    cmd2.Parameters.AddWithValue("@delmat_Actionby", empgid);
                    cmd2.Parameters.AddWithValue("@delmat_role", "Checker");
                    cmd2.Parameters.AddWithValue("@delmat_remarks", String.IsNullOrEmpty(delmat.Remarks) ? "" : delmat.Remarks.ToString());
                    cmd2.Parameters.AddWithValue("@delmat_status", "Rejected");
                    cmd2.CommandType = CommandType.StoredProcedure;
                    da = new SqlDataAdapter(cmd2);
                    da.Fill(dt2);

                    if (dt1.Rows.Count > 0)
                    {
                        Emp_Msg = "Delmat Checker Rejected";
                    }

                }
                return Emp_Msg;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string IsDelmatMaker()
        {
            string IsDelmatRoleFlag = "";
            try
            {
                DataTable dt1 = new DataTable();
                GetCon();
                cmd = new SqlCommand("pr_iem_mst_delmat", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@action", SqlDbType.VarChar).Value = "DelamatRole";
                cmd.Parameters.Add("@gid", SqlDbType.Int).Value = objCmnFunctions.GetLoginUserGid().ToString();
                da = new SqlDataAdapter(cmd);
                da.Fill(dt1);
                if (dt1.Rows.Count > 0)
                {
                    IsDelmatRoleFlag = Convert.ToString(dt1.Rows[0]["DelmatRole"].ToString());
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return IsDelmatRoleFlag;
        }

        public string ValidateDelmat(iem_mst_delmat Delmat, string[] Selecteddepartment)
        {
            DataTable dt = new DataTable();
            CommonIUD comm = new CommonIUD();
            string res1 = string.Empty;
            string result = "success";
            try
            {
                //SqlCommand cmdslab = new SqlCommand("pr_iem_mst_delmat", con);
                //GetCon();
                //cmdslab.Parameters.AddWithValue("@action", "Duplicate_slab");
                //cmdslab.Parameters.AddWithValue("@gid", Delmat.delmat_slab_gid);              
                //cmdslab.Parameters.Add("@res", SqlDbType.VarChar, 45).Direction = ParameterDirection.Output;
                //cmdslab.CommandType = CommandType.StoredProcedure;
                //cmdslab.ExecuteNonQuery();
                //string res1slab = cmdslab.Parameters["@res"].Value.ToString();
                //if (res1slab == "Not There")
                //{

                if (Delmat.delmat_type_ID == 7 && Delmat.delmat_wwoparpr == "O") // ramya added on 09 jun 22,modified on 13 jun 22
                {
                    Delmat.delmat_pipit_flag = "I";
                    Delmat.delmat_it_flag = "A";
                }

                foreach (string selctedepartment in Selecteddepartment)
                {

                    SqlCommand cmd = new SqlCommand("pr_iem_mst_delmat", con);
                    GetCon();
                    cmd.Parameters.AddWithValue("@action", "UpdateExist");
                    cmd.Parameters.AddWithValue("@gid", Delmat.delmat_gid.ToString());
                    cmd.Parameters.AddWithValue("@delmat_branchtype_flag", Delmat.delmat_branchtype_flag);
                    cmd.Parameters.AddWithValue("@delmat_slab_gid", Delmat.delmat_slab_gid);
                    cmd.Parameters.AddWithValue("@delmat_delmattype_gid", Delmat.delmat_type_ID);
                    cmd.Parameters.AddWithValue("@delmat_branch_flag", Delmat.delmat_branch_flag);
                    cmd.Parameters.AddWithValue("@delmat_claim_flag", Delmat.delmat_claim_flag);
                    cmd.Parameters.AddWithValue("@delmat_dsa_flag", Delmat.delmat_dsa_flag);
                    cmd.Parameters.AddWithValue("@delmat_pipit_flag", Delmat.delmat_pipit_flag);
                    cmd.Parameters.AddWithValue("@delmat_it_flag", Delmat.delmat_it_flag);
                    cmd.Parameters.AddWithValue("@delmat_exp_flag", Delmat.delmat_exp_flag);
                    cmd.Parameters.AddWithValue("@delmat_active_flag", Delmat.delmat_active);
                    cmd.Parameters.AddWithValue("@delmat_budget_flag", Delmat.delmat_budget_flag);
                    cmd.Parameters.AddWithValue("@delmat_parpr", Delmat.delmat_parpr_flag);
                    cmd.Parameters.AddWithValue("@delmat_woparpr", Delmat.delmat_wwoparpr);
                    cmd.Parameters.AddWithValue("@delmat_ecftype", Delmat.delmat_ecftype); // Muthu Added on 25-May-2022 
                    if (selctedepartment != "")
                    {
                        cmd.Parameters.AddWithValue("@delmatdept_dept_gid", SqlDbType.Int).Value = Convert.ToInt32(selctedepartment);
                    }
                    cmd.Parameters.Add("@res", SqlDbType.VarChar, 45).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                    string res = cmd.Parameters["@res"].Value.ToString();
                    if (res == "Exist")
                    {
                        res1 = "Exist";
                    }
                }
                //get delmat status

                
                if (res1 == "Exist")
                {
                    result = "Duplicate Delmat Attributes Found!";
                } 
            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }

            return result;
        }


    }
}
