using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DebtGraph.Application.Interfaces;

namespace DebtGraph.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IDebtRepository _repository;

        public UserController(IDebtRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _repository.GetAllUsersAsync();
            return Ok(users);
        }
    }
}