namespace LibraryManagement.Models
{
    public class BookDto
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public int YearOfRelease { get; set; }
        public int CategoryId { get; set; }
        public string ImagePath { get; set; }
        public int Amount { get; set; }
    }
}
