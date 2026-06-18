using System.ComponentModel.DataAnnotations;

namespace MyFirstMvcApp.Models
{
    public class BlogCard
    {
        public int ID { get; set; }

        [Required]
        public string Heading { get; set; }

        public string ImagePath { get; set; }

        public string URL { get; set; }

        public string Detail { get; set; }

        public DateTime TimeSpan { get; set; }

        // For image upload only (not stored in the database)
        public IFormFile ImageFile { get; set; }
    }
}
