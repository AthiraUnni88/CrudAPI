using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace new_projectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrudoperationController : ControllerBase
    {
        private IConfiguration _configuration ;
        private string sqlDatasource;

        public CrudoperationController(IConfiguration configuration)
        {
            _configuration = configuration;

        }
        public static List<Employee> ConvertDataTableToList(DataTable dt)
        {
            var employees = new List<Employee>();

            foreach (DataRow row in dt.Rows)
            {
                employees.Add(new Employee
                {
                    ID = Convert.ToInt32(row["EMPLOYEE_ID"]),
                    EmployeeName = row["EMPLOYEE_NAME"].ToString(),
                    Designation = row["DESIGNATION"].ToString(),
                    Post = row["POST"].ToString(),
                    Place = row["PLACE"].ToString(),
                    Phone = row["PHONE"].ToString()
                });
            }

            return employees;
        }


        [HttpGet]
        [Route("GetEmployees")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            int flag = 1; 
            string storedProcedure = "CRUD_OPERATION_PROCEDURE";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("todoAppDBCon");

            using (SqlConnection mycon = new SqlConnection(sqlDatasource))
            {
                mycon.Open();
                using (SqlCommand mycommand = new SqlCommand(storedProcedure, mycon))
                {
                    mycommand.CommandType = CommandType.StoredProcedure;
                    mycommand.Parameters.AddWithValue("@Flag", flag);

                    using (SqlDataReader myreader = mycommand.ExecuteReader())
                    {
                        table.Load(myreader);
                    }
                }
            }
            var employees = ConvertDataTableToList(table);
            return Ok(employees);
        }



        [HttpPost]
        [Route("AddEmployee")]
        public async Task<IActionResult> AddEmployee(
    [FromForm] string employeeName,
    [FromForm] string designation,
    [FromForm] string post,
    [FromForm] string place,
    [FromForm] string phone)
        {
            string storedProcedure = "CRUD_OPERATION_PROCEDURE"; DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("todoAppDBCon");
            bool success = false;
    
            using (SqlConnection mycon = new SqlConnection(sqlDatasource))
            {
                mycon.Open();
                using (SqlCommand mycommand = new SqlCommand(storedProcedure, mycon))
                {
                    mycommand.CommandType = CommandType.StoredProcedure;
                    mycommand.Parameters.AddWithValue("@Flag", 2);
                    mycommand.Parameters.AddWithValue("@EMPLOYEE_NAME", employeeName ?? (object)DBNull.Value);
                    mycommand.Parameters.AddWithValue("@DESIGNATION", designation ?? (object)DBNull.Value);
                    mycommand.Parameters.AddWithValue("@POST", post ?? (object)DBNull.Value);
                    mycommand.Parameters.AddWithValue("@PLACE", place ?? (object)DBNull.Value);
                    mycommand.Parameters.AddWithValue("@PHONE", phone ?? (object)DBNull.Value);
                    int rowsAffected = mycommand.ExecuteNonQuery();
                    success = rowsAffected > 0;
                }
                
                mycon.Close();
            }

            return new JsonResult(new
            {
                success = success,
            });
        }



        [HttpDelete]
        [Route("DeleteEmployee")]
        public JsonResult DeleteEmployee(int id)
        {
            string storedProcedure = "CRUD_OPERATION_PROCEDURE";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("todoAppDBCon");
            bool success = false;

            using (SqlConnection mycon = new SqlConnection(sqlDatasource))
            {
                mycon.Open();
                using (SqlCommand mycommand = new SqlCommand(storedProcedure, mycon))
                {
                    mycommand.CommandType = CommandType.StoredProcedure;
                    mycommand.Parameters.AddWithValue("@Flag", 3);
                    mycommand.Parameters.AddWithValue("@EMPLOYEE_ID", id);
                    int rowsAffected = mycommand.ExecuteNonQuery();
                    success = rowsAffected > 0;
                }

                mycon.Close();
            }

            return new JsonResult(new
            {
                success = success,
            });
        }


        [HttpPut]
        [Route("UpdateEmployee")]
        public JsonResult UpdateEmployee([FromQuery] int? id, [FromQuery] string employeeName, [FromQuery] string designation, [FromQuery] string post, [FromQuery] string place, [FromQuery] string phone)
        {
            if (id.HasValue && id.Value > 0)
            {
                string storedProcedure = "CRUD_OPERATION_PROCEDURE";
                bool success = false;
                string sqlDatasource = _configuration.GetConnectionString("todoAppDBCon");

                using (SqlConnection mycon = new SqlConnection(sqlDatasource))
                {
                    mycon.Open();
                    using (SqlCommand mycommand = new SqlCommand(storedProcedure, mycon))
                    {
                        mycommand.CommandType = CommandType.StoredProcedure;
                        mycommand.Parameters.AddWithValue("@Flag", 4);
                        mycommand.Parameters.AddWithValue("@EMPLOYEE_ID", id.Value);
                        mycommand.Parameters.AddWithValue("@EMPLOYEE_NAME", employeeName ?? (object)DBNull.Value);
                        mycommand.Parameters.AddWithValue("@DESIGNATION", designation ?? (object)DBNull.Value);
                        mycommand.Parameters.AddWithValue("@POST", post ?? (object)DBNull.Value);
                        mycommand.Parameters.AddWithValue("@PLACE", place ?? (object)DBNull.Value);
                        mycommand.Parameters.AddWithValue("@PHONE", phone ?? (object)DBNull.Value);
                        int rowsAffected = mycommand.ExecuteNonQuery();
                        success = rowsAffected > 0;
                    }
                }

                return new JsonResult(new { Status = true });
            }
            else
            {
                return new JsonResult(new { status = false, message = "Invalid or missing employee ID." });
            }
        }


    }


}
