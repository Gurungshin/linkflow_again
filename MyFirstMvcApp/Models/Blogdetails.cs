namespace MyFirstMvcApp.Models
{
    public class Blogdetails
    {
        public int ID { get; set; }

        public int BlogCardID { get; set; }

        public string? BlogHeading { get; set; }

        public string? ImagePath { get; set; }

        public IFormFile? ImageFile { get; set; }

        public string? Author { get; set; }

        public string? Keyword { get; set; }

        public string? URL { get; set; }

        public string? Paragraph { get; set; }

        public DateTime TimeSpan { get; set; }
    }
}
