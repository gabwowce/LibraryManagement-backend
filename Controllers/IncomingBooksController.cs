using LibraryManagement.Enums;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IncomingBooksController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<IncomingBook>> GetIncomingBooks()
        {
            var IncomingBookList = new List<IncomingBook>
            {
                 new IncomingBook
                {
                    Title = "Shatter Me",
                    Author = "Tahereh Mafi",
                    ReleaseYear = 2018,
                    Price = "12,89 €",
                    Amount=5,
                    Status = "Delivered"
                },
                  new IncomingBook
                {
                     Title = "Can't Hurt Me",
                    Author = "David Goggins",
                    ReleaseYear = 2024,
                    Price = "52,89 €",
                    Amount=4,
                    Status = "Delivered"
                },
                   new IncomingBook
                {
                    Title = "Twisted Hate",
                    Author = "Ana Huang",
                    ReleaseYear = 2022,
                    Price = "13,99 €",
                    Amount=4,
                    Status = "Pending"
                },
                   new IncomingBook
                {
                     Title = "The 48 Laws Of Power",
                    Author = "28,59 €",
                    ReleaseYear = 2000,
                    Price = "28,59 €",
                    Amount=3,
                    Status = "Pending"
                },
            };

            return Ok(IncomingBookList);
        }
    }
}

