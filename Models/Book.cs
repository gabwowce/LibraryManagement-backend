namespace LibraryManagement.Models
{
    public class Book: BaseEntity
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public int YearOfRelease { get; set; }
        public string Category { get; set; }
        public string ImagePath { get; set; }
        public int Amount { get; set; }
        public int RemainingFree { get; set; }
    }
}
