namespace MyFirstMvcApp.Models
{
    public class Gallery
    {
        public int Id { get; set; }

        public string ImagePath { get; set; }

        public string Caption { get; set; }

        public DateTime TimeSpan { get; set; }

        public IFormFile ImageFile { get; set; }
    }
}
