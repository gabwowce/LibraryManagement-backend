namespace LibraryManagement.Models
{
    public class Member: BaseEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
