using LibraryManagement.Enums;

namespace LibraryManagement.Models
{
    public class LoanData
    {
        public Month Month { get; set; } 
        public int Loans { get; set; }
        public Year Year { get; set; }
    }
}
