using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using Thinkspace.Controllers;

namespace Thinkspace.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        // Defining the data format and types for transer
        public class ResponseMessage
        {
            public bool IsSuccess { get; set; }
            public List<string> ErrorList { get; set; }
            public string PrimaryId { get; set; }
            public string Message { get; set; }
            public string Data { get; set; }
        }
        public class ApiResponseMessage
        {
            public bool IsSuccess { get; set; }
            public string Message { get; set; }
            public int StatusCode { get; set; }
            public List<Dictionary<string, object>> Data { get; set; } // Dictionary is a multidimensional list cappable of holding data as key value pairs of specified format... Here it is used for defining data type of written procedure's outputs

        }
        // create an object to store the database connection
        public static string StoreConnection = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
        //Establish database connection
        //public  ApiResponseMessage GetApiResponseExecuteReaderProcedure(SqlParameter[] paramValues, string ProcedureName)
        //{
        //    ApiResponseMessage responseEntity = new ApiResponseMessage();
        //    using (SqlConnection sqlconn = new SqlConnection(StoreConnection))
        //    {
        //        using (SqlCommand cmd = new SqlCommand(ProcedureName, sqlconn))
        //        {
        //            try
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                if (paramValues != null && paramValues.Length > 0)
        //                    cmd.Parameters.AddRange(paramValues);
        //                cmd.Connection.Open();

        //                DataTable dt = new DataTable();
        //                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
        //                {
        //                    DataSet ds = new DataSet();
        //                    da.Fill(ds);
        //                    dt = ds.Tables[0];
        //                    cmd.Connection.Close();
        //                }

        //                if (dt.Rows.Count == 0)
        //                {
        //                    responseEntity.Message = "No records found.";
        //                }
        //                else
        //                {
        //                    responseEntity.Message = dt.Rows.Count.ToString() + " records found.";
        //                }
        //                responseEntity.IsSuccess = true;
        //                responseEntity.StatusCode = (int)Enum.Parse(typeof(HttpStatusCode), HttpStatusCode.OK.ToString());
        //                responseEntity.Data = DataTableToDictionary(dt);
        //            }
        //            catch (Exception ex)
        //            {
        //                responseEntity.StatusCode = (int)Enum.Parse(typeof(HttpStatusCode), HttpStatusCode.BadRequest.ToString());
        //                responseEntity.IsSuccess = false;
        //                responseEntity.Message = ex.Message.ToString();
        //            }
        //            finally
        //            {
        //                cmd.Connection.Close();
        //            }
        //        }
        //        return responseEntity;
        //    }
        //}
        public static List<Dictionary<string, object>> DataTableToDictionary(DataTable table)
        {
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in table.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    if (col.DataType == System.Type.GetType("System.DateTime"))
                    {
                        string strval = dr[col].ToString();
                        if (string.IsNullOrEmpty(strval) || strval == "1/1/1900 12:00:00 AM")
                        {
                            row.Add(col.ColumnName, "");
                        }
                        else
                        {
                            DateTime dt = Convert.ToDateTime(strval);
                            row.Add(col.ColumnName, dt.ToString("dd-MMM-yyyy HH:mm:ss"));
                        }
                    }
                    else
                    {
                        row.Add(col.ColumnName, dr[col]);
                    }
                }
                rows.Add(row);
            }
            return rows;
        }
        public ActionResult GetSensorDetails(string SensorId)
        {
            ApiResponseMessage RespEntity = new ApiResponseMessage();
            SqlParameter[] paramValues = {

             new SqlParameter("@SensorId", SensorId)
                };
            RespEntity = GetApiResponseExecuteReaderProcedure("LVL.GetSensorDetails", paramValues);

            return Json(RespEntity, JsonRequestBehavior.AllowGet);
        }

        private ApiResponseMessage GetApiResponseExecuteReaderProcedure(string ProcedureName, SqlParameter[] paramValues)
        {
            ApiResponseMessage responseEntity = new ApiResponseMessage();
            using (SqlConnection sqlconn = new SqlConnection(StoreConnection))
            {
                using (SqlCommand cmd = new SqlCommand(ProcedureName, sqlconn))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (paramValues != null && paramValues.Length > 0)
                            cmd.Parameters.AddRange(paramValues);
                        cmd.Connection.Open();

                        DataTable dt = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            dt = ds.Tables[0];
                            cmd.Connection.Close();
                        }

                        if (dt.Rows.Count == 0)
                        {
                            responseEntity.Message = "No records found.";
                        }
                        else
                        {
                            responseEntity.Message = dt.Rows.Count.ToString() + " records found.";
                        }
                        responseEntity.IsSuccess = true;
                        responseEntity.StatusCode = (int)Enum.Parse(typeof(HttpStatusCode), HttpStatusCode.OK.ToString());
                        responseEntity.Data = DataTableToDictionary(dt);
                    }
                    catch (Exception ex)
                    {
                        responseEntity.StatusCode = (int)Enum.Parse(typeof(HttpStatusCode), HttpStatusCode.BadRequest.ToString());
                        responseEntity.IsSuccess = false;
                        responseEntity.Message = ex.Message.ToString();
                    }
                    finally
                    {
                        cmd.Connection.Close();
                    }
                }
                return responseEntity;
            }
        }

        [HttpGet]
        public ActionResult UpdateSensorDataDetails(string SensorData, string SensorId)
        {

            ApiResponseMessage responseEntity = new ApiResponseMessage();
            SqlParameter[] paramValues = {
             new SqlParameter("@SensorData", SensorData),
             new SqlParameter("@SensorId", SensorId)
                };

            responseEntity = GetApiResponseExecuteReaderProcedure("LVL.UpdateSensorDataDetails", paramValues);


            return Json(responseEntity, JsonRequestBehavior.AllowGet);
        }



        public ActionResult SensorGraphData()
        {
            ApiResponseMessage responseEntity = new ApiResponseMessage();
            //SqlParameter[] paramValues =
            //{
            //    new SqlParameter("@SensorName",SensorName)
            //};

            responseEntity = GetApiResponseExecuteReaderProcedure("LVL.getGraphDetails", null);

            return Json(responseEntity, JsonRequestBehavior.AllowGet);
        }
    }
}
