namespace LibraryManagement.Models
{
    public class Book: BaseEntity
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public int YearOfRelease { get; set; }
        public int CategoryID { get; set; }
        public string ImagePath { get; set; }
        public bool IsLoaned { get; set; }
    }
}
