using System;

namespace DebtGraph.Domain.Entities
{
    public class ClearedCycle
    {
        public int Id { get; set; }
        public string CycleInfo { get; set; }
        public decimal Amount { get; set; }
        public DateTime ClearedAt { get; set; }
    }
}