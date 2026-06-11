using System.ComponentModel.DataAnnotations;

namespace MyFirstMvcApp.Models
{
    public class JobApplications
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        // This MUST match the name="" attribute in your HTML file input!
        public IFormFile CvFile { get; set; }
        public string CvPath { get; set; }

        public string Position { get; set; }
        public string Experience { get; set; }
        public string CoverNote { get; set; }
        public DateTime TimeSpan { get; set; }
    }
}
