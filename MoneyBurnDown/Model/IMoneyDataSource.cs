using System;
using System.Data.Linq;
using System.Linq;
using MoneyBurnDown.Model.Entities;

namespace MoneyBurnDown.Model
{
    public interface IMoneyDataSource : IDisposable
    {
        IQueryable<Burndown> Burndowns { get; }

        IQueryable<Transaction> Transactions { get; }

        IQueryable<BurndownType> BurndownTypes { get; }

        IQueryable<TransactionType> TransactionTypes { get; }

        void AddBurndownType(string name);

        void SubmitChanges();

        void CreateBurndown(string name, DateTime startDate, DateTime endDate, decimal moneyToSpend, BurndownType burndownType);
        
        void AddTransactionType(string name);

        void DeleteTransactionType(TransactionType transactionType);
    }
}