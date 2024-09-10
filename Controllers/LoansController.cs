using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Interfaces;
using LibraryManagement.Models;
using System.Collections.Generic;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly ILoanRepository _loanRepository;

    public LoansController(ILoanRepository loanRepository)
    {
        _loanRepository = loanRepository;
    }

    [HttpGet("{id}")]
    public ActionResult<Loan> GetLoanById(int id)
    {
        var loan = _loanRepository.GetLoanById(id);
        if (loan == null)
        {
            return NotFound();
        }
        return Ok(loan);
    }

    [HttpGet]
    public ActionResult<IEnumerable<Loan>> GetAllLoans()
    {
        var loans = _loanRepository.GetAllLoans();
        return Ok(loans);
    }

    [HttpPost]
    public IActionResult Post([FromBody] Loan loan, [FromQuery] DateTime? customStartDate)
    {
        if (loan == null)
        {
            return BadRequest("Invalid loan data.");
        }

        _loanRepository.AddLoan(loan, customStartDate);
        return Ok("Loan added successfully.");
    }

    [HttpPut("{loanId}")]
    public IActionResult Put(int loanId, [FromQuery] string newStatus)
    {
        if (string.IsNullOrWhiteSpace(newStatus))
        {
            return BadRequest("Invalid status.");
        }

        _loanRepository.UpdateLoanStatus(loanId, newStatus);
        return Ok("Loan status updated successfully.");
    }

}
