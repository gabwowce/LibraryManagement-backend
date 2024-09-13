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
                    Amount = 40689,
                    StatsIcon = "../../assets/StatsMini/books2.png",
                    DownOrUpIcon = "../../assets/StatsMini/up.png",
                    DownUpInfo = "8.5% Up from yesterday"
                },
                  new StatsMiniInfo
                {
                    Title = "Total Books",
                    Amount = 10293,
                    StatsIcon = "../../assets/StatsMini/group.png",
                    DownOrUpIcon = "../../assets/StatsMini/up.png",
                    DownUpInfo = "1.3% Up from past week"
                },
                   new StatsMiniInfo
                {
                    Title = "Books Loaned Out",
                    Amount = 5573,
                    StatsIcon = "../../assets/StatsMini/loan.png",
                    DownOrUpIcon = "../../assets/StatsMini/down.png",
                    DownUpInfo = "4.3% Down from past week"
                },
                   new StatsMiniInfo
                {
                    Title = "Accrued Penalties",
                    Amount = 723,
                    StatsIcon = "../../assets/StatsMini/penalties.png",
                    DownOrUpIcon = "../../assets/StatsMini/up.png",
                    DownUpInfo = "1.8% Up from past week"
                },
            };

            return Ok(statsInfoList);
        }
    }
}
