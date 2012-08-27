using System;
using System.Collections.Generic;
using System.Linq;
using MoneyBurnDown.Model;
using MoneyBurnDown.Model.Entities;

namespace MoneyBurnDown.Design
{
    public class DesignMoneyDataSource : IMoneyDataSource 
    {
        public DesignMoneyDataSource()
        {
            List<TransactionType> transactionTypes = new List<TransactionType>
                                                         {
                                                             new TransactionType
                                                                 {
                                                                     Name = "For transport",
                                                                     CreatedAt = DateTime.Now
                                                                 },
                                                              new TransactionType
                                                                  {
                                                                      Name = "For food",
                                                                      CreatedAt = DateTime.Now
                                                                  }
                                                         };

            List<BurndownType> burndownTypes = new List<BurndownType>
                {
                    new BurndownType
                        {
                            Id = 1,
                            Name = "USD",
                            CreatedAt = DateTime.Now
                        },
                    new BurndownType
                        {
                            Id = 2,
                            Name = "EUR",
                            CreatedAt = DateTime.Now
                        }
                };

            List<Burndown> burndowns = new List<Burndown>
                {
                    new Burndown
                        {
                            Id = 1,
                            StartDate = DateTime.Today,
                            EndDate = DateTime.Today.AddMonths(1),
                            MoneyOnStart = 1000,
                            Name = "My money",
                            BurndownType = burndownTypes.First()
                        },
                    new Burndown
                        {
                            Id = 2,
                            StartDate = DateTime.Today,
                            EndDate = DateTime.Today.AddMonths(1),
                            MoneyOnStart = 1000,
                            Name = "My money 2",
                            BurndownType = burndownTypes.Last()
                        }
                };

            List<Transaction> transactions = new List<Transaction>
                {
                    new Transaction
                        {
                            Id = 1,
                            Amount = 100,
                            Burndown = burndowns.First(),
                            CreatedAt = DateTime.Now.AddDays(1),
                            TransactionType =  transactionTypes.First()
                        },
                    new Transaction
                        {
                            Id = 2,
                            Amount = 100,
                            Burndown = burndowns.First(),
                            CreatedAt = DateTime.Now.AddDays(15),
                            TransactionType = transactionTypes.Last()
                        },
                };

            Burndowns = new EnumerableQuery<Burndown>(burndowns);
            BurndownTypes = new EnumerableQuery<BurndownType>(burndownTypes);
            Transactions = new EnumerableQuery<Transaction>(transactions);
            TransactionTypes = new EnumerableQuery<TransactionType>(transactionTypes);
        }

        public IQueryable<Burndown> Burndowns { get; private set; }
        public IQueryable<Transaction> Transactions { get; private set; }
        public IQueryable<BurndownType> BurndownTypes { get; private set; }
        public IQueryable<TransactionType> TransactionTypes { get; private set; }

        public void AddBurndownType(string name)
        {
            throw new NotImplementedException();
        }

        public void SubmitChanges()
        {
        }

        public void CreateBurndown(string name, DateTime startDate, DateTime endDate, decimal moneyToSpend, BurndownType burndownType)
        {
            throw new NotImplementedException();
        }

        public void AddTransactionType(string name)
        {
            throw new NotImplementedException();
        }

        public void DeleteTransactionType(TransactionType transactionType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }
    }
}