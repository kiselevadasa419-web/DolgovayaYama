using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DebtGraph.Domain.Entities;
using DebtGraph.Domain.ValueObjects;
using DebtGraph.Application.Interfaces;

namespace DebtGraph.Application.Services
{
    public class DebtCycleService : IDebtCycleService
    {
        private readonly IDebtRepository _repository;

        public DebtCycleService(IDebtRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<UserBalance>> GetBalancesAsync()
        {
            var users = await _repository.GetAllUsersAsync();
            var debts = await _repository.GetAllDebtsAsync();
            var balances = new List<UserBalance>();

            foreach (var user in users)
            {
                // Долги, где пользователь является должником (должен другим)
                var debtsOwed = debts.Where(d => d.DebtorId == user.Id).ToList();
                // Долги, где пользователь является кредитором (должны ему)
                var debtsToMe = debts.Where(d => d.CreditorId == user.Id).ToList();

                foreach (var debt in debtsOwed)
                {
                    debt.CreditorName = users.First(u => u.Id == debt.CreditorId).Name;
                }
                foreach (var debt in debtsToMe)
                {
                    debt.DebtorName = users.First(u => u.Id == debt.DebtorId).Name;
                }

                balances.Add(new UserBalance
                {
                    UserId = user.Id,
                    UserName = user.Name,
                    NetBalance = debtsToMe.Sum(d => d.Amount) - debtsOwed.Sum(d => d.Amount),
                    DebtsOwed = debtsOwed,    // Кому должен (конкретные ребра)
                    DebtsToMe = debtsToMe     // Кто должен ему (конкретные ребра)
                });
            }

            return balances;
        }

        public async Task<CycleResult> TakeDebtAsync(int debtorId, int creditorId, decimal amount)
        {
            if (debtorId == creditorId)
                throw new ArgumentException("Cannot borrow from yourself");

            if (amount <= 0)
                throw new ArgumentException("Amount must be positive");

            // Добавляем новое ребро в граф
            var debt = new Debt
            {
                DebtorId = debtorId,
                CreditorId = creditorId,
                Amount = amount,
                CreatedAt = DateTime.UtcNow
            };
            await _repository.AddDebtAsync(debt);

            // Ищем и погашаем циклы
            return await FindAndClearCyclesAsync();
        }

        private async Task<CycleResult> FindAndClearCyclesAsync()
        {
            var result = new CycleResult { Cleared = false };
            bool foundCycle = true;

            while (foundCycle)
            {
                foundCycle = false;
                var users = await _repository.GetAllUsersAsync();
                var debts = await _repository.GetAllDebtsAsync();

                // Строим граф из конкретных ребер
                var graph = new Dictionary<int, List<Debt>>();
                foreach (var user in users)
                {
                    graph[user.Id] = new List<Debt>();
                }
                foreach (var debt in debts)
                {
                    graph[debt.DebtorId].Add(debt);
                }

                // Ищем цикл
                var cycle = FindCycle(graph, users.Select(u => u.Id).ToList());

                if (cycle != null && cycle.Count > 0)
                {
                    decimal minAmount = cycle.Min(d => d.Amount);

                    if (minAmount > 0)
                    {
                        await ClearCycle(cycle, minAmount, users);
                        result.Cleared = true;
                        result.Amount += minAmount;
                        result.Message += FormatCycle(cycle, users) + " cleared: " + minAmount + " coins\n";
                        foundCycle = true;
                    }
                }
            }

            return result;
        }

        private List<Debt> FindCycle(Dictionary<int, List<Debt>> graph, List<int> nodes)
        {
            foreach (var start in nodes)
            {
                var visited = new HashSet<int>();
                var path = new List<Debt>();

                if (DFS(start, start, visited, path, graph))
                {
                    return path;
                }
            }
            return null;
        }

        private bool DFS(int current, int start, HashSet<int> visited, List<Debt> path,
            Dictionary<int, List<Debt>> graph)
        {
            if (current == start && path.Count > 1)
            {
                return true;
            }

            if (visited.Contains(current))
                return false;

            visited.Add(current);

            if (graph.ContainsKey(current))
            {
                foreach (var debt in graph[current])
                {
                    path.Add(debt);
                    if (DFS(debt.CreditorId, start, visited, path, graph))
                        return true;
                    path.RemoveAt(path.Count - 1);
                }
            }

            visited.Remove(current);
            return false;
        }

        private async Task ClearCycle(List<Debt> cycle, decimal amount, List<User> users)
        {
            foreach (var debt in cycle)
            {
                // Уменьшаем конкретный долг
                debt.Amount -= amount;

                if (debt.Amount == 0)
                {
                    // Удаляем ребро, если оно обнулилось
                    await _repository.DeleteDebtAsync(debt.Id);
                }
                else
                {
                    // Обновляем сумму ребра
                    await _repository.UpdateDebtAsync(debt);
                }
            }

            // Записываем историю
            var cycleInfo = FormatCycle(cycle, users);
            var clearedCycle = new ClearedCycle
            {
                CycleInfo = cycleInfo,
                Amount = amount,
                ClearedAt = DateTime.UtcNow
            };
            await _repository.AddClearedCycleAsync(clearedCycle);
        }

        private string FormatCycle(List<Debt> cycle, List<User> users)
        {
            var parts = new List<string>();
            foreach (var debt in cycle)
            {
                var debtor = users.First(u => u.Id == debt.DebtorId).Name.Split(' ')[0];
                var creditor = users.First(u => u.Id == debt.CreditorId).Name.Split(' ')[0];
                parts.Add($"{debtor}->{creditor}({debt.Amount})");
            }
            return string.Join(" -> ", parts);
        }
    }
}