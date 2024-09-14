using LibraryManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<StatsMiniInfo>> GetStatsInfo()
        {
            var statsInfoList = new List<StatsMiniInfo>
            {
                 new StatsMiniInfo
                {
                    Title = "Total Members",
                    Amount = "40,689",
                    StatsIcon = "/StatsMini/books2.png",
                    DownOrUpIcon = "/StatsMini/up.png",
                    Procentage="8.5%",
                    DownUpInfo = "Up from yesterday"
                },
                  new StatsMiniInfo
                {
                    Title = "Total Books",
                    Amount = "10,293",
                    StatsIcon = "/StatsMini/group.png",
                    DownOrUpIcon = "/StatsMini/up.png",
                    Procentage="1.3%",
                    DownUpInfo = "Up from past week"
                },
                   new StatsMiniInfo
                {
                    Title = "Books Loaned Out",
                    Amount = "5,573",
                    StatsIcon = "/StatsMini/loan.png",
                    DownOrUpIcon = "/StatsMini/down.png",
                    Procentage="4.3%",
                    DownUpInfo = "Down from past week"
                },
                   new StatsMiniInfo
                {
                    Title = "Accrued Penalties",
                    Amount = "$723",
                    StatsIcon = "/StatsMini/penalties.png",
                    DownOrUpIcon = "/StatsMini/up.png",
                    Procentage="1.8%",
                    DownUpInfo = "Up from past week"
                },
            };

            return Ok(statsInfoList);
        }
    }
}
