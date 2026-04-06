using System;

namespace DebtGraph.Domain.Entities
{
    public class Debt
    {
        public int Id { get; set; }
        public int DebtorId { get; set; }      // Кто должен
        public int CreditorId { get; set; }    // Кому должен
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }

        // Для отображения
        public string DebtorName { get; set; }
        public string CreditorName { get; set; }
    }
}