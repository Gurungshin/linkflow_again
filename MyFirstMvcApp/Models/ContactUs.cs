namespace MyFirstMvcApp.Models
{
    public class ContactUs
    {
        public int ID { get; set; }

        public string Email { get; set; }

        public string Subject { get; set; }

        public string? ServiceSubject { get; set; }

        public string Message { get; set; }

        public DateTime TimeSpan { get; set; }
    }
}
