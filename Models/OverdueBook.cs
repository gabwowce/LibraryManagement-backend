namespace LibraryManagement.Models
{
    public class OverdueBook : BaseEntity
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string BorrowerName { get; set; }
        public DateTime LoanStartDate { get; set; }
        public int DaysOverdue { get; set; }
    }
}
