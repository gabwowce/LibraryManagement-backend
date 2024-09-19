using LibraryManagement.Models;
using System.Collections.Generic;

namespace LibraryManagement.Interfaces
{
    public interface ILoanRepository
    {
        Loan GetLoanById(int id);
        IEnumerable<Loan> GetLoansByMemberId(int memberId);
        IEnumerable<Loan> GetAllLoans();
        void AddLoan(Loan loan, DateTime? customStartDate = null);
        void UpdateLoanStatus(int loanId, string newStatus);
    }
}
