using LibraryManagement.Models;
using LibraryManagement.Enums; 
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using LibraryManagement.Helpers;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoansDataController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<LoanData>> GetLoansData()
        {
            var loansDataList = DataGenerator.GenerateRandomLoanData(2022, 2024);

            if (loansDataList == null || loansDataList.Count == 0)
            {
                return NotFound("No loan data found.");
            }

            return Ok(loansDataList);
        }
    }
}
