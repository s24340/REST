using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using REST.Models;

namespace REST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AnimalController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private List<Dictionary<string, object>> ConvertDataTableToList(DataTable dt)
        {
            var columns = dt.Columns.Cast<DataColumn>();
            return dt.Rows.Cast<DataRow>()
                .Select(row => columns.ToDictionary(column => column.ColumnName, column => row[column])).ToList();
        }

        [HttpGet]
        public IActionResult GetAnimals(string orderBy = "name")
        {
            try
            {
                string query = $"SELECT * FROM Animals ORDER BY {orderBy}";
                DataTable table = new DataTable();
                string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        using (SqlDataReader myReader = myCommand.ExecuteReader())
                        {
                            table.Load(myReader);
                        }
                    }
                }

                var list = ConvertDataTableToList(table);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult PostAnimal(Animal animal)
        {
            try
            {
                string query =
                    @"INSERT INTO Animals (Name, Description, Category, Area) VALUES (@Name, @Description, @Category, @Area)";
                DataTable table = new DataTable();
                string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");
                SqlDataReader myReader;

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@Name", animal.Name);
                        myCommand.Parameters.AddWithValue("@Description", animal.Description);
                        myCommand.Parameters.AddWithValue("@Category", animal.Category);
                        myCommand.Parameters.AddWithValue("@Area", animal.Area);
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);
                        myReader.Close();
                    }
                }

                return StatusCode(StatusCodes.Status201Created, "Added Successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public IActionResult PutAnimal(int id, Animal animal)
        {
            string query =
                @"UPDATE Animals SET Name = @Name, Description = @Description, Category = @Category, Area = @Area WHERE IdAnimal = @Id";
            string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@Id", id);
                    myCommand.Parameters.AddWithValue("@Name", animal.Name);
                    myCommand.Parameters.AddWithValue("@Description", animal.Description);
                    myCommand.Parameters.AddWithValue("@Category", animal.Category);
                    myCommand.Parameters.AddWithValue("@Area", animal.Area);

                    try
                    {
                        myCon.Open();
                        int rowsAffected = myCommand.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new JsonResult("Updated Successfully");
                        }
                        else
                        {
                            return NotFound($"Animal with id {id} was not found");
                        }
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
                    }
                }
            }
        }


        public IActionResult DeleteAnimal(int id)
        {
            string query = @"DELETE FROM Animals WHERE IdAnimal = @Id";
            string sqlDataSource = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@Id", id);

                        myCon.Open();
                        int rowsAffected = myCommand.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new JsonResult("Deleted Successfully");
                        }
                        else
                        {
                            return NotFound($"Animal with id {id} was not found");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }
    }
}
