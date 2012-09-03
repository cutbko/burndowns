using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Microsoft.Phone.Data.Linq;
using MoneyBurnDown.Model.Entities;

namespace MoneyBurnDown.Model
{
    public class MoneyDataContext : DataContext, IMoneyDataSource
    {
        private const string ConnectionString = "Data Source=isostore:/MoneyDb.sdf";

        public MoneyDataContext()
            : base(ConnectionString)
        {
            if (!DatabaseExists())
            {
                CreateDatabase();
                GetTables();

                BurndownTypes.InsertAllOnSubmit(new List<BurndownType>
                    {
                        new BurndownType
                            {
                                Name = "USD",
                                CreatedAt = DateTime.Now,
                                IsDeleted = false
                            },
                        new BurndownType
                            {
                                Name = "EUR",
                                CreatedAt = DateTime.Now,
                                IsDeleted = false
                            }
                    });

                TransactionTypes.InsertAllOnSubmit(new List<TransactionType>
                                                       {
                                                           new TransactionType
                                                               {
                                                                   Name = "For food",
                                                                   CreatedAt = DateTime.Now,
                                                                   IsDeleted = false
                                                               },
                                                           new TransactionType
                                                               {
                                                                   Name = "For transport",
                                                                   CreatedAt = DateTime.Now,
                                                                   IsDeleted = false
                                                               }
                                                       });
                SubmitChanges();
                DatabaseSchemaUpdater databaseSchemaUpdater = this.CreateDatabaseSchemaUpdater();
                databaseSchemaUpdater.DatabaseSchemaVersion = 3;
                databaseSchemaUpdater.Execute();
            }
            else
            {
                GetTables();
            }
        }

        private void GetTables()
        {
            Burndowns = GetTable<Burndown>();
            BurndownTypes = GetTable<BurndownType>();
            Transactions = GetTable<Transaction>();
            TransactionTypes = GetTable<TransactionType>();
        }

        public Table<Burndown> Burndowns { get; set; }

        public Table<Transaction> Transactions { get; set; }

        public Table<BurndownType> BurndownTypes { get; set; }

        public Table<TransactionType> TransactionTypes { get; set; }

        public void AddBurndownType(string name)
        {
            BurndownTypes.InsertOnSubmit(new BurndownType
                                             {
                                                 Name = name,
                                                 CreatedAt = DateTime.Now,
                                                 IsDeleted = false
                                             });
        }

        public void CreateBurndown(string name, DateTime startDate, DateTime endDate, decimal moneyToSpend, BurndownType burndownType)
        {
            Burndowns.InsertOnSubmit(new Burndown
                                         {
                                             Name = name,
                                             StartDate = startDate,
                                             EndDate = endDate,
                                             MoneyOnStart = moneyToSpend,
                                             BurndownType = burndownType,
                                             IsDeleted = false
                                         });
            SubmitChanges();
        }

        public void AddTransactionType(string name)
        {
            TransactionTypes.InsertOnSubmit(new TransactionType
                                                {
                                                    Name = name,
                                                    CreatedAt = DateTime.Now,
                                                    IsDeleted = false
                                                });
            SubmitChanges();
        }

        void IMoneyDataSource.SubmitChanges()
        {
            SubmitChanges();   
        }

        public void DeleteTransactionType(TransactionType transactionType)
        {
            transactionType.IsDeleted = true;
            SubmitChanges();
        }


        IQueryable<Burndown> IMoneyDataSource.Burndowns
        {
            get { return Burndowns.Where(x=>x.IsDeleted != true); }
        }

        IQueryable<Transaction> IMoneyDataSource.Transactions
        {
            get { return Transactions.Where(x => x.IsDeleted != true); }
        }

        IQueryable<BurndownType> IMoneyDataSource.BurndownTypes
        {
            get { return BurndownTypes.Where(x => x.IsDeleted != true); }
        }

        IQueryable<TransactionType> IMoneyDataSource.TransactionTypes
        {
            get
            {
                return TransactionTypes.Where(x => x.IsDeleted != true);
            }
        }
    }
}