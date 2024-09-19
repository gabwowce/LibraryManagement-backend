namespace LibraryManagement.Models
{
    public class Member: BaseEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public List<Loan> Loans { get; set; } = new List<Loan>();

    }
}
