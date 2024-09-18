namespace LibraryManagement.Models
{
    public class OverdueBook : BaseEntity
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string BorrowerName { get; set; }
        public string LoanStartDate { get; set; }
        public string LoanEndDate { get; set; }
        public int DaysOverdue { get; set; }
    }
}
