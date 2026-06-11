using Microsoft.AspNetCore.Mvc;
using MyFirstMvcApp.Models;
using System.Diagnostics;
using Microsoft.Data.SqlClient;


namespace MyFirstMvcApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {

            List<FAQ> faqList = new List<FAQ>();

            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            con.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM FAQ ORDER BY ID DESC", con);
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                FAQ faq = new FAQ();

                faq.ID = Convert.ToInt32(dr["ID"]);
                faq.Question = dr["Question"].ToString();
                faq.Answer = dr["Answer"].ToString();
                faq.TimeSpan = Convert.ToDateTime(dr["TimeSpan"]);

                faqList.Add(faq);
            }

            con.Close();

            return View(faqList);
        }

        public IActionResult serviceDetail()
        {
            return View();
        }

        public IActionResult gallery()
        {
            return View();
        }

        public IActionResult career()
        {
            return View();
        }

        [HttpPost]
        public IActionResult career(JobApplicant obj)
        {
            return View();
        }

        public IActionResult BlogDetail()
        {
            return View();
        }

        public IActionResult Blog() => View();
        public IActionResult BlogDetail() => View();
        public IActionResult Admin() => View();

        [HttpPost]
        public IActionResult Admin(Admin obj)
        {
            string conStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(conStr))
            {
                string query = @"SELECT COUNT(*)
                         FROM Admin
                         WHERE UserName=@UserName
                         AND Password=@Password";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@UserName", obj.UserName);
                cmd.Parameters.AddWithValue("@Password", obj.Password);

                con.Open();

                int count = (int)cmd.ExecuteScalar();

                if (count > 0)
                {
                    return RedirectToAction("Index", "Admin");
                }
            }

            ViewBag.Error = "Username or Password is incorrect";
            return View();
        }


        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(Contact obj)
        {
            string conStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(conStr))
            {
                string query = @"INSERT INTO Contact
                        (Name, Email, Subject, Message)
                        VALUES
                        (@Name, @Email, @Subject, @Message)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Name", obj.Name);
                cmd.Parameters.AddWithValue("@Email", obj.Email);
                cmd.Parameters.AddWithValue("@Subject", obj.Subject);
                cmd.Parameters.AddWithValue("@Message", obj.Message);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            ViewBag.Message = "Message Sent Successfully";

            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
