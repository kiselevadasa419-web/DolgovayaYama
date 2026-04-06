using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using DebtGraph.Domain.Entities;
using DebtGraph.Application.Interfaces;

namespace DebtGraph.Infrastructure.Repositories
{
   
    public class DebtRepository : IDebtRepository
    {
        private readonly string _connectionString;

        public DebtRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Получить всех пользователей
        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = new List<User>();

            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT Id, Name, CreatedAt FROM Users ORDER BY Id", conn);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        users.Add(new User
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            CreatedAt = reader.GetDateTime(2)
                        });
                    }
                }
            }
            return users;
        }

        // Получить пользователя по Id
        public async Task<User> GetUserByIdAsync(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT Id, Name, CreatedAt FROM Users WHERE Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new User
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            CreatedAt = reader.GetDateTime(2)
                        };
                    }
                }
            }
            return null;
        }

        // Получить все долги
        public async Task<List<Debt>> GetAllDebtsAsync()
        {
            var debts = new List<Debt>();

            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT Id, DebtorId, CreditorId, Amount, CreatedAt FROM Debts", conn);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        debts.Add(new Debt
                        {
                            Id = reader.GetInt32(0),
                            DebtorId = reader.GetInt32(1),
                            CreditorId = reader.GetInt32(2),
                            Amount = reader.GetDecimal(3),
                            CreatedAt = reader.GetDateTime(4)
                        });
                    }
                }
            }
            return debts;
        }

        // Добавить новый долг
        public async Task AddDebtAsync(Debt debt)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(@"
                    INSERT INTO Debts (DebtorId, CreditorId, Amount, CreatedAt) 
                    VALUES (@debtorId, @creditorId, @amount, @createdAt);
                    SELECT SCOPE_IDENTITY();", conn);

                cmd.Parameters.AddWithValue("@debtorId", debt.DebtorId);
                cmd.Parameters.AddWithValue("@creditorId", debt.CreditorId);
                cmd.Parameters.AddWithValue("@amount", debt.Amount);
                cmd.Parameters.AddWithValue("@createdAt", debt.CreatedAt);

                debt.Id = Convert.ToInt32(await cmd.ExecuteScalarAsync()); // Получаем новый Id
            }
        }

        // Обновить сумму долга
        public async Task UpdateDebtAsync(Debt debt)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("UPDATE Debts SET Amount = @amount WHERE Id = @id", conn);
                cmd.Parameters.AddWithValue("@amount", debt.Amount);
                cmd.Parameters.AddWithValue("@id", debt.Id);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        // Удалить долг
        public async Task DeleteDebtAsync(int debtId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("DELETE FROM Debts WHERE Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", debtId);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        // Добавить запись о погашенном цикле
        public async Task AddClearedCycleAsync(ClearedCycle cycle)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(@"
                    INSERT INTO ClearedCycles (CycleInfo, Amount, ClearedAt) 
                    VALUES (@info, @amount, @clearedAt)", conn);

                cmd.Parameters.AddWithValue("@info", cycle.CycleInfo);
                cmd.Parameters.AddWithValue("@amount", cycle.Amount);
                cmd.Parameters.AddWithValue("@clearedAt", cycle.ClearedAt);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        // Получить историю погашений
        public async Task<List<ClearedCycle>> GetClearedCyclesAsync()
        {
            var cycles = new List<ClearedCycle>();

            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT Id, CycleInfo, Amount, ClearedAt FROM ClearedCycles ORDER BY ClearedAt DESC", conn);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        cycles.Add(new ClearedCycle
                        {
                            Id = reader.GetInt32(0),
                            CycleInfo = reader.GetString(1),
                            Amount = reader.GetDecimal(2),
                            ClearedAt = reader.GetDateTime(3)
                        });
                    }
                }
            }
            return cycles;
        }
    }
}