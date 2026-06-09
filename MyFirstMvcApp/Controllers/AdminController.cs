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
            List<Contact> contactList = new List<Contact>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Contact", con);
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                contactList.Add(new Contact
                {
                    ID = Convert.ToInt32(dr["ID"]),
                    Name = dr["Name"].ToString(),
                    Email = dr["Email"].ToString(),
                    Subject = dr["Subject"].ToString(),
                    Message = dr["Message"].ToString(),
                    TimeSpan = Convert.ToDateTime(dr["TimeSpan"])
                });
            }

            con.Close();
            return View(contactList);
        }



        public IActionResult AddFaQ(int? id)
        {
            FAQ obj = new FAQ();

            if (id != null)
            {
                string conStr = _configuration.GetConnectionString("DefaultConnection");

                SqlConnection con = new SqlConnection(conStr);
                con.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM FAQ WHERE ID=@ID", con);
                cmd.Parameters.AddWithValue("@ID", id);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    obj.ID = Convert.ToInt32(dr["ID"]);
                    obj.Question = dr["Question"].ToString();
                    obj.Answer = dr["Answer"].ToString();
                }

                con.Close();
            }

            return View(obj);
        }

 

        [HttpPost]
        public IActionResult AddFaQ(FAQ obj)
        {
            if (string.IsNullOrWhiteSpace(obj.Question))
            {
                TempData["Message"] = "Please enter a question.";
                return View(obj);
            }

            if (string.IsNullOrWhiteSpace(obj.Answer))
            {
                TempData["Message"] = "Please enter an answer.";
                return View(obj);
            }

            string conStr = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            if (obj.ID > 0)
            {
                // UPDATE
                SqlCommand cmd = new SqlCommand(
                    @"UPDATE FAQ
              SET Question = @Question,
                  Answer = @Answer
              WHERE ID = @ID", con);

                cmd.Parameters.AddWithValue("@ID", obj.ID);
                cmd.Parameters.AddWithValue("@Question", obj.Question);
                cmd.Parameters.AddWithValue("@Answer", obj.Answer);

                cmd.ExecuteNonQuery();

                TempData["Message"] = "FAQ Updated Successfully";
            }
            else
            {
                // INSERT
                SqlCommand cmd = new SqlCommand(
                    @"INSERT INTO FAQ
              (Question, Answer)
              VALUES
              (@Question, @Answer)", con);

                cmd.Parameters.AddWithValue("@Question", obj.Question);
                cmd.Parameters.AddWithValue("@Answer", obj.Answer);

                cmd.ExecuteNonQuery();

                TempData["Message"] = "FAQ Added Successfully";
            }

            con.Close();

            
            return RedirectToAction("AddFaQ");
        }

        public IActionResult FaQdetails()
        {
            List<FAQ> faqList = new List<FAQ>();

            string conStr = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM FAQ ORDER BY ID DESC", con);

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                faqList.Add(new FAQ
                {
                    ID = Convert.ToInt32(dr["ID"]),
                    Question = dr["Question"].ToString(),
                    Answer = dr["Answer"].ToString(),
                    TimeSpan = Convert.ToDateTime(dr["TimeSpan"])
                });
            }

            con.Close();

            return View(faqList);
        }

        public IActionResult DeleteFAQ(int id)
        {
            string conStr = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            SqlCommand cmd = new SqlCommand("DELETE FROM FAQ WHERE ID=@ID", con);
            cmd.Parameters.AddWithValue("@ID", id);

            cmd.ExecuteNonQuery();

            con.Close();

            return RedirectToAction("FaQdetails");
        }

    }
}
