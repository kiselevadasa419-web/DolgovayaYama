# DebtGraph - Оптимизационная задача на графах

## О проекте
Система управления долгами с автоматическим погашением циклов в ориентированном графе.
5 пользователей (Джон, Дейнерис, Тирион, Серсея, Санса). Долги = направленные ребра графа.
При замыкании цикла долги погашаются минимальной суммой.

## Требования
- .NET 8.0 SDK
- SQL Server (или использовать JSON вариант без SQL)


## Запуск с SQL Server
 1. Создать базу данных
Выполнить в SQL Server Management Studio SQL_Debt.sql

2. Настроить подключение
В файле appsettings.json изменить строку:

json
"ConnectionStrings": {
    "DefaultConnection": "Data Source=localhost;Initial Catalog=DebtGraphDB;Integrated Security=True;TrustServerCertificate=True"
}
3. Запустить
bash
dotnet run --project DebtGraph.API
Проверка работы
Открыть в браузере:

https://localhost:7210/api/User/all - список пользователей

https://localhost:7210/api/Debt/balances - балансы