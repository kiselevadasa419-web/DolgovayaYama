using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DebtGraph.Application.Interfaces;

namespace DebtGraph.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DebtController : ControllerBase
    {
        private readonly IDebtCycleService _debtCycleService;

        public DebtController(IDebtCycleService debtCycleService)
        {
            _debtCycleService = debtCycleService;
        }

        [HttpGet("balances")]
        public async Task<IActionResult> GetBalances()
        {
            var balances = await _debtCycleService.GetBalancesAsync();
            return Ok(balances);
        }

        [HttpPost("take")]
        public async Task<IActionResult> TakeDebt([FromBody] TakeDebtRequest request)
        {
            if (request == null)
                return BadRequest(new { error = "Invalid request" });

            try
            {
                var result = await _debtCycleService.TakeDebtAsync(
                    request.DebtorId,
                    request.CreditorId,
                    request.Amount);

                var balances = await _debtCycleService.GetBalancesAsync();

                return Ok(new
                {
                    success = true,
                    balances = balances,
                    cycleCleared = result.Cleared ? new
                    {
                        cleared = true,
                        amount = result.Amount,
                        message = result.Message
                    } : null
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        private string GetUserName(dynamic balances, int userId)
        {
            foreach (var balance in balances)
            {
                if (balance.UserId == userId)
                    return balance.UserName;
            }
            return "Unknown";
        }
    }

    public class TakeDebtRequest
    {
        public int DebtorId { get; set; }
        public int CreditorId { get; set; }
        public decimal Amount { get; set; }
    }
}