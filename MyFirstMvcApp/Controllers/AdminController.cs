using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MyFirstMvcApp.Models;

namespace MyFirstMvcApp.Controllers
{
    public class AdminController : Controller
    {

        private readonly IConfiguration _configuration;

        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddFaQ()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddFaQ(FAQ obj)
        {
            string conStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(conStr))
            {
                string query = @"INSERT INTO FAQ
                         (Question, Answer)
                         VALUES
                         (@Question, @Answer)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Question", obj.Question);
                cmd.Parameters.AddWithValue("@Answer", obj.Answer);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            ViewBag.Message = "FAQ Added Successfully";
            return View();
        }

        public IActionResult FaQdetails()
        {
            return View();
        }
    }
}
