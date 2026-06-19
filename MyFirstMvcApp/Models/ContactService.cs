using System.ComponentModel.DataAnnotations;

namespace MyFirstMvcApp.Models
{
    public class ContactService
    {
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        public string CompanyName { get; set; }

        [Required]
        public string Service { get; set; }

        public string Message { get; set; }

        public DateTime TimeSpan { get; set; }
    }
}
