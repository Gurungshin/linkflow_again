using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MyFirstMvcApp.Models;

namespace MyFirstMvcApp.Controllers
{
    public class AdminController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public AdminController(IConfiguration configuration,
                               IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
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

        [HttpGet]
        public IActionResult jobcard(int? id)
        {
            JobCard obj = new JobCard();

            if (id != null)
            {
                string conStr = _configuration.GetConnectionString("DefaultConnection");

                SqlConnection con = new SqlConnection(conStr);
                con.Open();

                SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM JobCard WHERE ID = @ID", con);

                cmd.Parameters.AddWithValue("@ID", id);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    obj.ID = Convert.ToInt32(dr["ID"]);
                    obj.JobTitle = dr["JobTitle"].ToString();
                    obj.Role = dr["Role"].ToString();
                    obj.JobType = dr["JobType"].ToString();
                    obj.Detail = dr["Detail"].ToString();
                    obj.Keyword = dr["Keyword"].ToString();
                }

                con.Close();
            }

            return View(obj);
        }

        [HttpPost]
        public IActionResult jobcard(JobCard obj)
        {
            string conStr = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            if (obj.ID > 0)
            {
                // UPDATE
                SqlCommand cmd = new SqlCommand(
                    @"UPDATE JobCard
          SET JobTitle = @JobTitle,
              Role = @Role,
              JobType = @JobType,
              Detail = @Detail,
              Keyword = @Keyword
          WHERE ID = @ID", con);

                cmd.Parameters.AddWithValue("@ID", obj.ID);
                cmd.Parameters.AddWithValue("@JobTitle", obj.JobTitle);
                cmd.Parameters.AddWithValue("@Role", obj.Role ?? "");
                cmd.Parameters.AddWithValue("@JobType", obj.JobType ?? "");
                cmd.Parameters.AddWithValue("@Detail", obj.Detail ?? "");
                cmd.Parameters.AddWithValue("@Keyword", obj.Keyword ?? "");

                cmd.ExecuteNonQuery();

                TempData["Message"] = "Job Updated Successfully";
            }
            else
            {
                // INSERT
                SqlCommand cmd = new SqlCommand(
                    @"INSERT INTO JobCard
          (JobTitle, Role, JobType, Detail, Keyword)
          VALUES
          (@JobTitle, @Role, @JobType, @Detail, @Keyword)", con);

                cmd.Parameters.AddWithValue("@JobTitle", obj.JobTitle);
                cmd.Parameters.AddWithValue("@Role", obj.Role ?? "");
                cmd.Parameters.AddWithValue("@JobType", obj.JobType ?? "");
                cmd.Parameters.AddWithValue("@Detail", obj.Detail ?? "");
                cmd.Parameters.AddWithValue("@Keyword", obj.Keyword ?? "");

                cmd.ExecuteNonQuery();

                TempData["Message"] = "Job Added Successfully";
            }

            con.Close();

            return RedirectToAction("JobDetail");
        }

        public IActionResult JobDetail()
        {
            List<JobCard> jobList = new List<JobCard>();

            string conStr = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            SqlCommand cmd = new SqlCommand(
                "SELECT * FROM JobCard ORDER BY ID DESC", con);

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                jobList.Add(new JobCard
                {
                    ID = Convert.ToInt32(dr["ID"]),
                    JobTitle = dr["JobTitle"].ToString(),
                    Role = dr["Role"].ToString(),
                    JobType = dr["JobType"].ToString(),
                    Detail = dr["Detail"].ToString(),
                    Keyword = dr["Keyword"].ToString(),
                    TimeSpan = Convert.ToDateTime(dr["TimeSpan"])
                });
            }

            con.Close();

            return View(jobList);
        }

        public IActionResult DeleteJob(int id)
        {
            string conStr = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            SqlCommand cmd = new SqlCommand(
                "DELETE FROM JobCard WHERE ID = @ID", con);

            cmd.Parameters.AddWithValue("@ID", id);

            cmd.ExecuteNonQuery();

            con.Close();

            TempData["Message"] = "Job Deleted Successfully";

            return RedirectToAction("JobDetail");
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

        [HttpGet]
        public IActionResult AddService(int? id)
        {
            ServiceDetailed obj = new ServiceDetailed();

            if (id != null)
            {
                string conStr = _configuration.GetConnectionString("DefaultConnection");

                SqlConnection con = new SqlConnection(conStr);
                con.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM ServiceDetailed WHERE ID = @ID", con);

                cmd.Parameters.AddWithValue("@ID", id);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    obj.ID = Convert.ToInt32(dr["ID"]);
                    obj.Heading = dr["Heading"].ToString();
                    obj.Paragraph = dr["Paragraph"].ToString();
                    obj.ServiceDetails = dr["ServiceDetails"].ToString();
                }

                con.Close();
            }

            return View(obj);
        }

        [HttpPost]
        public IActionResult AddService(ServiceDetailed obj)
        {
            string conStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();

                if (obj.ID > 0)
                {
                    // UPDATE
                    string query = @"UPDATE ServiceDetailed
                         SET Heading = @Heading,
                             Paragraph = @Paragraph,
                             ServiceDetails = @ServiceDetails
                         WHERE ID = @ID";

                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@ID", obj.ID);
                    cmd.Parameters.AddWithValue("@Heading", obj.Heading);
                    cmd.Parameters.AddWithValue("@Paragraph", obj.Paragraph);
                    cmd.Parameters.AddWithValue("@ServiceDetails", obj.ServiceDetails);

                    cmd.ExecuteNonQuery();

                    TempData["Message"] = "Service details updated successfully.";
                }
                else
                {
                    // INSERT
                    string query = @"INSERT INTO ServiceDetailed
                         (Heading, Paragraph, ServiceDetails)  VALUES (@Heading, @Paragraph, @ServiceDetails)";

                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@Heading", obj.Heading);
                    cmd.Parameters.AddWithValue("@Paragraph", obj.Paragraph);
                    cmd.Parameters.AddWithValue("@ServiceDetails", obj.ServiceDetails);

                    cmd.ExecuteNonQuery();

                    TempData["Message"] = "Service details added successfully.";
                }
            }

            return RedirectToAction("ServiceDetailed");
        }

        [HttpGet]
        public ActionResult ServiceDetailed()
        {
            List<ServiceDetailed> serviceList = new List<ServiceDetailed>();

            string conStr = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            SqlCommand cmd = new SqlCommand(
                "SELECT * FROM ServiceDetailed ORDER BY ID DESC", con);

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                serviceList.Add(new ServiceDetailed
                {
                    ID = Convert.ToInt32(dr["ID"]),
                    Heading = dr["Heading"].ToString(),
                    Paragraph = dr["Paragraph"].ToString(),
                    ServiceDetails = dr["ServiceDetails"].ToString(),
                    TimeSpan = Convert.ToDateTime(dr["TimeSpan"])
                });
            }

            con.Close();

            return View(serviceList);
        }

        [HttpGet]
        public ActionResult Addgallery(int? Id)
        {
            Gallery obj = new Gallery();

            if (Id != null)
            {
                string conStr = _configuration.GetConnectionString("DefaultConnection");

                SqlConnection con = new SqlConnection(conStr);
                con.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM Gallery WHERE Id=@Id", con);
                cmd.Parameters.AddWithValue("@Id", Id);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    obj.Id = Convert.ToInt32(dr["Id"]);
                    obj.ImagePath = dr["ImagePath"].ToString();
                    obj.Caption = dr["Caption"].ToString();
                }

                con.Close();
            }

            return View(obj);
        }

        [HttpPost]
        public ActionResult Addgallery(Gallery obj)
        {
            string conStr = _configuration.GetConnectionString("DefaultConnection");

            string imagePath = obj.ImagePath;

            if (obj.ImageFile != null)
            {
                string folder = Path.Combine(_environment.WebRootPath, "serverImage");

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                string fileName = Guid.NewGuid().ToString() +
                                  Path.GetExtension(obj.ImageFile.FileName);

                string filePath = Path.Combine(folder, fileName);

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    obj.ImageFile.CopyTo(stream);
                }

                imagePath = "/serverImage/" + fileName;
            }

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            SqlCommand cmd;

            if (obj.Id > 0)
            {
                cmd = new SqlCommand(
                    "UPDATE Gallery SET ImagePath=@ImagePath, Caption=@Caption WHERE Id=@Id",
                    con);

                cmd.Parameters.AddWithValue("@Id", obj.Id);
            }
            else
            {
                cmd = new SqlCommand(
                    "INSERT INTO Gallery(ImagePath, Caption) VALUES(@ImagePath, @Caption)",
                    con);
            }

            cmd.Parameters.AddWithValue("@ImagePath", imagePath);
            cmd.Parameters.AddWithValue("@Caption", obj.Caption);

            cmd.ExecuteNonQuery();

            con.Close();

            TempData["Message"] = obj.Id > 0
                ? "Gallery Updated Successfully"
                : "Gallery Saved Successfully";

            return RedirectToAction("GalleryDetailed");
        }

        public ActionResult GalleryDetailed()
        {
            List<Gallery> list = new List<Gallery>();

            string conStr = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Gallery ORDER BY ID DESC", con);

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                Gallery obj = new Gallery();

                obj.Id = Convert.ToInt32(dr["ID"]);
                obj.ImagePath = dr["ImagePath"].ToString();
                obj.Caption = dr["Caption"].ToString();

                list.Add(obj);
            }

            con.Close();

            return View(list);
        }

        [HttpGet]
        public IActionResult DeleteGallery(int id)
        {
            string conStr = _configuration.GetConnectionString("DefaultConnection");

            string imagePath = "";

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            // Get image path first
            SqlCommand cmd = new SqlCommand(
                "SELECT ImagePath FROM Gallery WHERE Id=@Id", con);

            cmd.Parameters.AddWithValue("@Id", id);

            object result = cmd.ExecuteScalar();

            if (result != null)
            {
                imagePath = result.ToString();
            }

            // Delete database record
            cmd = new SqlCommand(
                "DELETE FROM Gallery WHERE Id=@Id", con);

            cmd.Parameters.AddWithValue("@Id", id);

            cmd.ExecuteNonQuery();

            con.Close();

            // Delete physical file
            if (!string.IsNullOrEmpty(imagePath))
            {
                string fullPath = Path.Combine(
                    _environment.WebRootPath,
                    imagePath.TrimStart('/').Replace("/", "\\"));

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }

            TempData["Message"] = "Gallery Deleted Successfully";

            return RedirectToAction("GalleryDetailed");
        }

        [HttpGet]
        public IActionResult BlogCard(int? Id)
        {
            if (Id == null)
            {
                return View(new BlogCard());
            }

            string conStr = _configuration.GetConnectionString("DefaultConnection");

            BlogCard obj = new BlogCard();

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM BlogCard WHERE ID=@ID", con);
            cmd.Parameters.AddWithValue("@ID", Id);

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                obj.ID = Convert.ToInt32(dr["ID"]);
                obj.Heading = dr["Heading"].ToString();
                obj.ImagePath = dr["ImagePath"].ToString();
                obj.URL = dr["URL"].ToString();
                obj.Detail = dr["Detail"].ToString();
            }

            con.Close();

            return View(obj);
        }

        [HttpPost]
        public IActionResult BlogCard(BlogCard obj)
        {
            string conStr = _configuration.GetConnectionString("DefaultConnection");

            if (obj.ImageFile != null && obj.ImageFile.Length > 0)
            {
                string uploadFolder = Path.Combine(_environment.WebRootPath, "serverImage");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(obj.ImageFile.FileName);
                string filePath = Path.Combine(uploadFolder, fileName);

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    obj.ImageFile.CopyTo(stream);
                }

                obj.ImagePath = "/serverImage/" + fileName;
            }

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            SqlCommand cmd;

            if (obj.ID > 0)
            {
                if (string.IsNullOrEmpty(obj.ImagePath))
                {
                    cmd = new SqlCommand(@"UPDATE BlogCard
                                   SET Heading=@Heading,
                                       URL=@URL,
                                       Detail=@Detail
                                   WHERE ID=@ID", con);
                }
                else
                {
                    cmd = new SqlCommand(@"UPDATE BlogCard
                                   SET Heading=@Heading,
                                       ImagePath=@ImagePath,
                                       URL=@URL,
                                       Detail=@Detail
                                   WHERE ID=@ID", con);

                    cmd.Parameters.AddWithValue("@ImagePath", obj.ImagePath);
                }

                cmd.Parameters.AddWithValue("@ID", obj.ID);
                cmd.Parameters.AddWithValue("@Heading", obj.Heading);
                cmd.Parameters.AddWithValue("@URL", obj.URL ?? "");
                cmd.Parameters.AddWithValue("@Detail", obj.Detail ?? "");

                cmd.ExecuteNonQuery();

                TempData["Message"] = "Blog Updated Successfully";
            }
            else
            {
                cmd = new SqlCommand(@"INSERT INTO BlogCard
                              (Heading,ImagePath,URL,Detail)
                              VALUES
                              (@Heading,@ImagePath,@URL,@Detail)", con);

                cmd.Parameters.AddWithValue("@Heading", obj.Heading);
                cmd.Parameters.AddWithValue("@ImagePath", obj.ImagePath ?? "");
                cmd.Parameters.AddWithValue("@URL", obj.URL ?? "");
                cmd.Parameters.AddWithValue("@Detail", obj.Detail ?? "");

                cmd.ExecuteNonQuery();

                TempData["Message"] = "Blog Added Successfully";
            }

            con.Close();

            return RedirectToAction("BlogCard");
        }

        public IActionResult BlogCarddetailed()
        {
            string conStr = _configuration.GetConnectionString("DefaultConnection");

            List<BlogCard> list = new List<BlogCard>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM BlogCard ORDER BY ID DESC", con);

                con.Open();

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
            }

            return View(list);
        }

        public IActionResult DeleteBlogCard(int id)
        {
            string conStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();

                // Get image path
                SqlCommand cmd = new SqlCommand("SELECT ImagePath FROM BlogCard WHERE ID=@ID", con);
                cmd.Parameters.AddWithValue("@ID", id);

                string imagePath = Convert.ToString(cmd.ExecuteScalar());

                // Delete record
                SqlCommand cmd1 = new SqlCommand("DELETE FROM BlogCard WHERE ID=@ID", con);
                cmd1.Parameters.AddWithValue("@ID", id);
                cmd1.ExecuteNonQuery();

                // Delete image from wwwroot/serverImage
                if (!string.IsNullOrEmpty(imagePath))
                {
                    string fullPath = Path.Combine(_environment.WebRootPath, imagePath.TrimStart('/').Replace("/", "\\"));

                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }

                con.Close();
            }

            return RedirectToAction("BlogCarddetailed");
        }


        [HttpGet]
        public IActionResult AddBlogDetailed(int? id)
        {
            Blogdetails obj = new Blogdetails();

            List<BlogCard> list = new List<BlogCard>();

            string conStr = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            SqlCommand cmd = new SqlCommand("SELECT ID, URL FROM BlogCard", con);

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new BlogCard
                {
                    ID = Convert.ToInt32(dr["ID"]),
                    URL = dr["URL"].ToString()
                });
            }

            dr.Close();

            if (id != null)
            {
                SqlCommand cmd1 = new SqlCommand("SELECT * FROM BlogDetails WHERE ID=@ID", con);
                cmd1.Parameters.AddWithValue("@ID", id);

                SqlDataReader dr1 = cmd1.ExecuteReader();

                if (dr1.Read())
                {
                    obj.ID = Convert.ToInt32(dr1["ID"]);
                    obj.BlogCardID = Convert.ToInt32(dr1["BlogCardID"]);
                    obj.BlogHeading = dr1["BlogHeading"].ToString();
                    obj.ImagePath = dr1["ImagePath"].ToString();
                    obj.Author = dr1["Author"].ToString();
                    obj.Keyword = dr1["Keyword"].ToString();
                    obj.URL = dr1["URL"].ToString();
                    obj.Paragraph = dr1["Paragraph"].ToString();
                }

                dr1.Close();
            }

            con.Close();

            ViewBag.BlogList = list;

            return View(obj);
        }

        [HttpPost]
        public IActionResult AddBlogDetailed(Blogdetails obj)
        {
            string conStr = _configuration.GetConnectionString("DefaultConnection");
            string imagePath = obj.ImagePath;

            // Upload Image
            if (obj.ImageFile != null && obj.ImageFile.Length > 0)
            {
                string folder = Path.Combine(_environment.WebRootPath, "serverImage");

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(obj.ImageFile.FileName);
                string filePath = Path.Combine(folder, fileName);

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    obj.ImageFile.CopyTo(stream);
                }

                imagePath = "/serverImage/" + fileName;
            }

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            // Get URL from BlogCard
            string url = "";

            SqlCommand urlCmd = new SqlCommand("SELECT URL FROM BlogCard WHERE ID=@ID", con);
            urlCmd.Parameters.AddWithValue("@ID", obj.BlogCardID);

            object result = urlCmd.ExecuteScalar();

            if (result != null)
            {
                url = result.ToString();
            }

            SqlCommand cmd;

            if (obj.ID > 0)
            {
                // UPDATE
                cmd = new SqlCommand(@"
        UPDATE BlogDetails
        SET BlogCardID=@BlogCardID,
            BlogHeading=@BlogHeading,
            ImagePath=@ImagePath,
            Author=@Author,
            Keyword=@Keyword,
            URL=@URL,
            Paragraph=@Paragraph
        WHERE ID=@ID", con);

                cmd.Parameters.AddWithValue("@ID", obj.ID);

                TempData["Message"] = "Blog Details Updated Successfully";
            }
            else
            {
                // INSERT
                cmd = new SqlCommand(@"
        INSERT INTO BlogDetails
        (BlogCardID,BlogHeading,ImagePath,Author,Keyword,URL,Paragraph)
        VALUES
        (@BlogCardID,@BlogHeading,@ImagePath,@Author,@Keyword,@URL,@Paragraph)", con);

                TempData["Message"] = "Blog Details Saved Successfully";
            }

            cmd.Parameters.AddWithValue("@BlogCardID", obj.BlogCardID);
            cmd.Parameters.AddWithValue("@BlogHeading", obj.BlogHeading ?? "");
            cmd.Parameters.AddWithValue("@ImagePath", imagePath ?? "");
            cmd.Parameters.AddWithValue("@Author", obj.Author ?? "");
            cmd.Parameters.AddWithValue("@Keyword", obj.Keyword ?? "");
            cmd.Parameters.AddWithValue("@URL", url);
            cmd.Parameters.AddWithValue("@Paragraph", obj.Paragraph ?? "");

            cmd.ExecuteNonQuery();

            con.Close();

            return RedirectToAction("BlogDetailed");
        }


        public IActionResult BlogDetailed()
        {
            List<Blogdetails> list = new List<Blogdetails>();

            string conStr = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection con = new SqlConnection(conStr);
            con.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM BlogDetails ORDER BY ID DESC", con);

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                Blogdetails obj = new Blogdetails();

                obj.ID = Convert.ToInt32(dr["ID"]);
                obj.BlogCardID = Convert.ToInt32(dr["BlogCardID"]);
                obj.BlogHeading = dr["BlogHeading"].ToString();
                obj.ImagePath = dr["ImagePath"].ToString();
                obj.Author = dr["Author"].ToString();
                obj.Keyword = dr["Keyword"].ToString();
                obj.URL = dr["URL"].ToString();
                obj.Paragraph = dr["Paragraph"].ToString();

                if (dr["TimeSpan"] != DBNull.Value)
                {
                    obj.TimeSpan = Convert.ToDateTime(dr["TimeSpan"]);
                }

                list.Add(obj);
            }

            dr.Close();
            con.Close();

            return View(list);
        }

        public IActionResult DeleteBlogDetailed(int id)
        {
            string conStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();

                // Get image path
                SqlCommand cmd = new SqlCommand(
                    "SELECT ImagePath FROM BlogDetails WHERE ID=@ID", con);

                cmd.Parameters.AddWithValue("@ID", id);

                string imagePath = Convert.ToString(cmd.ExecuteScalar());

                // Delete record
                SqlCommand cmd1 = new SqlCommand(
                    "DELETE FROM BlogDetails WHERE ID=@ID", con);

                cmd1.Parameters.AddWithValue("@ID", id);

                cmd1.ExecuteNonQuery();

                con.Close();

                // Delete image from wwwroot/serverImage
                if (!string.IsNullOrEmpty(imagePath))
                {
                    string fullPath = Path.Combine(
                        _environment.WebRootPath,
                        imagePath.TrimStart('/').Replace("/", "\\"));

                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
            }

            TempData["Message"] = "Blog Detail Deleted Successfully";

            return RedirectToAction("BlogDetailed");
        }

    }
}
