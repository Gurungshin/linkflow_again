using Microsoft.AspNetCore.Mvc;
using MyFirstMvcApp.Models;
using System.Diagnostics;
using Microsoft.Data.SqlClient;


namespace MyFirstMvcApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public HomeController(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
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
            List<Gallery> list = new List<Gallery>();

            string conStr = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Gallery ORDER BY Id DESC", con);

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                Gallery obj = new Gallery();

                obj.Id = Convert.ToInt32(dr["Id"]);
                obj.ImagePath = dr["ImagePath"].ToString();
                obj.Caption = dr["Caption"].ToString();

                if (dr["TimeSpan"] != DBNull.Value)
                {
                    obj.TimeSpan = Convert.ToDateTime(dr["TimeSpan"]);
                }

                list.Add(obj);
            }

            con.Close();

            return View(list);
        }

        public IActionResult career()
        {
            List<JobCard> jobCardList = new List<JobCard>();
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            // The 'using' block guarantees the connection closes safely no matter what
            using (SqlConnection con = new SqlConnection(connStr))
            {
                // Specifying column names explicitly is faster and safer than 'SELECT *'
                string query = "SELECT ID, JobTitle, Role, JobType, Detail, Keyword, TimeSpan FROM JobCard ORDER BY ID DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            JobCard job = new JobCard
                            {
                                ID = Convert.ToInt32(dr["ID"]),
                                JobTitle = dr["JobTitle"]?.ToString(),
                                Role = dr["Role"]?.ToString(),
                                JobType = dr["JobType"]?.ToString(),
                                Detail = dr["Detail"]?.ToString(),
                                Keyword = dr["Keyword"]?.ToString(),

                                TimeSpan = dr["TimeSpan"] != DBNull.Value ? Convert.ToDateTime(dr["TimeSpan"]) : DateTime.MinValue
                            };

                            jobCardList.Add(job);
                        }
                    }
                }
            } 

            return View(jobCardList);
        }

        [HttpPost]
        public IActionResult Career(JobApplications obj)
        {
            try
            {
                string conStr = _configuration.GetConnectionString("DefaultConnection");

                // ======================
                // FILE UPLOAD
                // ======================
                if (obj.CvFile != null && obj.CvFile.Length > 0)
                {
                    string uploadFolder = Path.Combine(_environment.WebRootPath, "CV");

                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    string fileName = Guid.NewGuid() + "_" + obj.CvFile.FileName;
                    string filePath = Path.Combine(uploadFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        obj.CvFile.CopyTo(stream);
                    }

                    obj.CvPath = "/CV/" + fileName;
                }

                // ======================
                // INSERT DATA
                // ======================
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    string query = @"INSERT INTO JobApplications
                            (FirstName, Email, Phone, Position, Experience, CoverNote, CvPath)
                            VALUES
                            (@FirstName, @Email, @Phone, @Position, @Experience, @CoverNote, @CvPath)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", obj.FirstName ?? "");
                        cmd.Parameters.AddWithValue("@Email", obj.Email ?? "");
                        cmd.Parameters.AddWithValue("@Phone", obj.Phone ?? "");
                        cmd.Parameters.AddWithValue("@Position", obj.Position ?? "");
                        cmd.Parameters.AddWithValue("@Experience", obj.Experience ?? "");
                        cmd.Parameters.AddWithValue("@CoverNote", obj.CoverNote ?? "");
                        cmd.Parameters.AddWithValue("@CvPath", (object?)obj.CvPath ?? DBNull.Value);

                        con.Open();

                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            TempData["Message"] = "Application Submitted Successfully";
                        }
                        else
                        {
                            TempData["Message"] = "Application Submission Failed";
                        }
                    }
                }

                return RedirectToAction("Career");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error: " + ex.Message;
                return RedirectToAction("Career");
            }
        }


        public IActionResult Blog()
        {
            return View();
        }

        public IActionResult BlogDetail()
        {
            return View();
        }

        public IActionResult Admin()
        {
            return View();
        }

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
