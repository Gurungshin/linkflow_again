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
            LoadServiceMenu();
            return View();
        }

        private void LoadServiceMenu()
        {
            string conStr = _configuration.GetConnectionString("DefaultConnection");

            List<ServiceDetailed> list = new List<ServiceDetailed>();

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            SqlCommand cmd = new SqlCommand(
                "SELECT Heading, URL FROM ServiceDetailed ORDER BY ID", con);

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new ServiceDetailed
                {
                    Heading = dr["Heading"].ToString(),
                    URL = dr["URL"].ToString()
                });
            }

            dr.Close();
            con.Close();

            ViewBag.ServiceList = list;
        }

        public IActionResult Privacy()
        {
            LoadServiceMenu();
            return View();
        }

        public IActionResult Tearm()
        {
            LoadServiceMenu();
            return View();
        }

       
        public IActionResult About()
        {
            LoadServiceMenu();
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

        [HttpGet]
        [Route("service/{url}")]
        public IActionResult serviceDetail(string url)
        {
            LoadServiceMenu();
            ServiceDetailed obj = new ServiceDetailed();

            string conStr = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            SqlCommand cmd = new SqlCommand(
                "SELECT * FROM ServiceDetailed WHERE URL=@URL", con);

            cmd.Parameters.AddWithValue("@URL", url);

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                obj.ID = Convert.ToInt32(dr["ID"]);
                obj.Heading = dr["Heading"].ToString();
                obj.Paragraph = dr["Paragraph"].ToString();
                obj.ServiceDetails = dr["ServiceDetails"].ToString();
                obj.URL = dr["URL"].ToString();
                obj.TimeSpan = Convert.ToDateTime(dr["TimeSpan"]);
            }

            dr.Close();
            con.Close();

            return View(obj);
        }

        [HttpPost]
        [Route("service/{url}")]
        public IActionResult serviceDetail(string url, ContactService obj)
        {
            string conStr = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            SqlCommand cmd = new SqlCommand(@"
        INSERT INTO ContactService
        (Name,Email,CompanyName,Service,Message)
        VALUES
        (@Name,@Email,@CompanyName,@Service,@Message)", con);

            cmd.Parameters.AddWithValue("@Name", obj.Name ?? "");
            cmd.Parameters.AddWithValue("@Email", obj.Email ?? "");
            cmd.Parameters.AddWithValue("@CompanyName", obj.CompanyName ?? "");
            cmd.Parameters.AddWithValue("@Service", obj.Service ?? "");
            cmd.Parameters.AddWithValue("@Message", obj.Message ?? "");

            cmd.ExecuteNonQuery();

            con.Close();

            return RedirectToAction("Thankyou");
        }
        public IActionResult gallery()
        {
            LoadServiceMenu();
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
            LoadServiceMenu();
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
            LoadServiceMenu();
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
            LoadServiceMenu();
            List<BlogCard> list = new List<BlogCard>();

            string conStr = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM BlogCard ORDER BY ID DESC", con);

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                BlogCard obj = new BlogCard();

                obj.ID = Convert.ToInt32(dr["ID"]);
                obj.Heading = dr["Heading"].ToString();
                obj.ImagePath = dr["ImagePath"].ToString();
                obj.URL = dr["URL"].ToString();
                obj.Detail = dr["Detail"].ToString();
                obj.TimeSpan = Convert.ToDateTime(dr["TimeSpan"]);

                list.Add(obj);
            }

            con.Close();

            return View(list);
        }

        [HttpGet]
        [Route("blog/{url}")]
        public IActionResult BlogDetail(string url)
        {
            //LoadServiceMenu();
            //string conStr = _configuration.GetConnectionString("DefaultConnection");

            //Blogdetails obj = new Blogdetails();

            //SqlConnection con = new SqlConnection(conStr);
            //con.Open();

            //SqlCommand cmd = new SqlCommand(
            //    "SELECT * FROM BlogDetails WHERE URL=@URL", con);

            //cmd.Parameters.AddWithValue("@URL", url);   // <-- this line is required

            //SqlDataReader dr = cmd.ExecuteReader();

            //if (dr.Read())
            //{
            //    obj.ID = Convert.ToInt32(dr["ID"]);
            //    obj.BlogHeading = dr["BlogHeading"].ToString();
            //    obj.ImagePath = dr["ImagePath"].ToString();
            //    obj.Author = dr["Author"].ToString();
            //    obj.Keyword = dr["Keyword"].ToString();
            //    obj.URL = dr["URL"].ToString();
            //    obj.Paragraph = dr["Paragraph"].ToString();
            //}

            //con.Close();

            //return View(obj);
            LoadServiceMenu();

            string conStr = _configuration.GetConnectionString("DefaultConnection");

            Blogdetails obj = new Blogdetails();

            List<BlogCard> relatedBlogs = new List<BlogCard>();

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            // Current Blog
            SqlCommand cmd = new SqlCommand(
                "SELECT * FROM BlogDetails WHERE URL=@URL", con);

            cmd.Parameters.AddWithValue("@URL", url);

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                obj.ID = Convert.ToInt32(dr["ID"]);
                obj.BlogHeading = dr["BlogHeading"].ToString();
                obj.ImagePath = dr["ImagePath"].ToString();
                obj.Author = dr["Author"].ToString();
                obj.Keyword = dr["Keyword"].ToString();
                obj.URL = dr["URL"].ToString();
                obj.Paragraph = dr["Paragraph"].ToString();
                obj.TimeSpan = Convert.ToDateTime(dr["TimeSpan"]);
            }

            dr.Close();

            // Related Blogs
            SqlCommand cmd2 = new SqlCommand(
                @"SELECT TOP 3 *
          FROM BlogCard
          WHERE URL <> @URL
          ORDER BY ID DESC", con);

            cmd2.Parameters.AddWithValue("@URL", url);

            SqlDataReader dr2 = cmd2.ExecuteReader();

            while (dr2.Read())
            {
                relatedBlogs.Add(new BlogCard
                {
                    ID = Convert.ToInt32(dr2["ID"]),
                    Heading = dr2["Heading"].ToString(),
                    ImagePath = dr2["ImagePath"].ToString(),
                    Detail = dr2["Detail"].ToString(),
                    URL = dr2["URL"].ToString(),
                    TimeSpan = Convert.ToDateTime(dr2["TimeSpan"])
                });
            }

            dr2.Close();
            con.Close();

            ViewBag.RelatedBlogs = relatedBlogs;

            return View(obj);
        }

        public IActionResult Admin()
        {
            LoadServiceMenu();
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
            LoadServiceMenu();
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

        public IActionResult Thankyou()
        {
            LoadServiceMenu();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
