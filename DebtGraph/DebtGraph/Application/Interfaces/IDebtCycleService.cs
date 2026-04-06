using System.Collections.Generic;
using System.Threading.Tasks;
using DebtGraph.Domain.ValueObjects;

namespace DebtGraph.Application.Interfaces
{
    public interface IDebtCycleService
    {
        Task<List<UserBalance>> GetBalancesAsync();
        Task<CycleResult> TakeDebtAsync(int debtorId, int creditorId, decimal amount);
    }

    public class CycleResult
    {
        public bool Cleared { get; set; }
        public decimal Amount { get; set; }
        public string Message { get; set; }
    }
}