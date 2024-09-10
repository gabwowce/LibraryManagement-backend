namespace LibraryManagement.Models
{
    public class Loan: BaseEntity
    {
        public int MemberId { get; set; }
        public int BookId { get; set; }
        public DateTime DateOfLoan { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
    }
}
