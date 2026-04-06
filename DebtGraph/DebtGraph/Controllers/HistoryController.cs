using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DebtGraph.Application.Interfaces;

namespace DebtGraph.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly IDebtRepository _repository;

        public HistoryController(IDebtRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var history = await _repository.GetClearedCyclesAsync();
            return Ok(history);
        }
    }
}