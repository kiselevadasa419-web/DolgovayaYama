using System.Collections.Generic;
using DebtGraph.Domain.Entities;

namespace DebtGraph.Domain.ValueObjects
{
    public class UserBalance
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public decimal NetBalance { get; set; }
        public List<Debt> DebtsOwed { get; set; } = new List<Debt>();
        public List<Debt> DebtsToMe { get; set; } = new List<Debt>();
    }
}