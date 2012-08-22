using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
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
                                CreatedAt = DateTime.Now
                            },
                        new BurndownType
                            {
                                Name = "EUR",
                                CreatedAt = DateTime.Now
                            }
                    });
                SubmitChanges();
                DatabaseSchemaUpdater databaseSchemaUpdater = this.CreateDatabaseSchemaUpdater();
                databaseSchemaUpdater.DatabaseSchemaVersion = 1;
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
        }

        public Table<Burndown> Burndowns { get; set; }

        public Table<Transaction> Transactions { get; set; }

        public Table<BurndownType> BurndownTypes { get; set; }

        public void AddBurndownType(string name)
        {
            BurndownTypes.InsertOnSubmit(new BurndownType
                                             {
                                                 Name = name,
                                                 CreatedAt = DateTime.Now
                                             });
            SubmitChanges();
        }

        public void CreateBurndown(string name, DateTime startDate, DateTime endDate, decimal moneyToSpend, BurndownType burndownType)
        {
            Burndowns.InsertOnSubmit(new Burndown
                                         {
                                             Name = name,
                                             StartDate = startDate,
                                             EndDate = endDate,
                                             MoneyOnStart = moneyToSpend,
                                             BurndownType = burndownType
                                         });
            SubmitChanges();
        }


        IQueryable<Burndown> IMoneyDataSource.Burndowns
        {
            get { return Burndowns; }
        }

        IQueryable<Transaction> IMoneyDataSource.Transactions
        {
            get { return Transactions; }
        }

        IQueryable<BurndownType> IMoneyDataSource.BurndownTypes
        {
            get { return BurndownTypes; }
        }
    }
}