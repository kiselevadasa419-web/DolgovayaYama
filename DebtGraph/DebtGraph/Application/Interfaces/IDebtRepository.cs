using System.Collections.Generic;
using System.Threading.Tasks;
using DebtGraph.Domain.Entities;

namespace DebtGraph.Application.Interfaces
{
    public interface IDebtRepository
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task<List<Debt>> GetAllDebtsAsync();
        Task AddDebtAsync(Debt debt);
        Task UpdateDebtAsync(Debt debt);
        Task DeleteDebtAsync(int debtId);
        Task AddClearedCycleAsync(ClearedCycle cycle);
        Task<List<ClearedCycle>> GetClearedCyclesAsync();
    }
}