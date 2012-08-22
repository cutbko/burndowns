using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;

namespace MoneyBurnDown.Model.Entities
{
    [Table(Name = "Burndowns")]
    public class Burndown : NotificationObjectBase
    {
        private int _id;
        private string _name;
        private DateTime _startDate;
        private DateTime _endDate;
        private readonly EntitySet<Transaction> _transactions;

        public Burndown()
        {
            _transactions = new EntitySet<Transaction>(AttachTransaction, DetachTransaction);
        }

        private void DetachTransaction(Transaction transaction)
        {
            NotifyPropertyChanging("Transaction");
            transaction.Burndown = this;
        }

        private void AttachTransaction(Transaction transaction)
        {
            NotifyPropertyChanging("Transaction");
            transaction.Burndown = this;
        }

        [Column(IsDbGenerated = true, AutoSync = AutoSync.OnInsert, IsPrimaryKey = true, DbType = "Int NOT NULL IDENTITY")]
        public virtual int Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    NotifyPropertyChanging("Id");
                    _id = value;
                    NotifyPropertyChanged("Id");
                }
                
            }
        }

        [Column(CanBeNull = false, DbType = "NVarChar(100) NOT NULL")]
        public string Name
        {
            get { return _name; }
            set
            {
                if (String.CompareOrdinal(_name, value)!=0)
                {
                    NotifyPropertyChanged("Name");
                    _name = value;
                    NotifyPropertyChanging("Name");
                }
            }
        }

        [Column(CanBeNull = false)]
        public virtual DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                if (_startDate != value)
                {
                    NotifyPropertyChanged("StartDate");
                    _startDate = value;
                    NotifyPropertyChanging("StartDate");
                }
            }
        }

        [Column(CanBeNull = false)]
        public virtual DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                if (_endDate != value)
                {
                    NotifyPropertyChanged("EndDate");
                    _endDate = value;
                    NotifyPropertyChanging("EndDate");
                }
            }
        }

        public decimal MoneyLeft
        {
            get { return MoneyOnStart - Transactions.Sum(x => x.Amount); }
        }

        [Column(CanBeNull = false)]
        public virtual decimal MoneyOnStart { get; set; }

        [Association(Storage = "_transactions", ThisKey = "Id", OtherKey = "_burndownId")]
        public EntitySet<Transaction> Transactions
        {
            get { return _transactions; }
            set
            {
                _transactions.Assign(value);
            }
        }

        [Column]
        private int? _burndownTypeId;

        private EntityRef<BurndownType> _burndownType;

        [Association(Storage = "_burndownType", ThisKey = "_burndownTypeId", OtherKey = "Id", IsForeignKey = true)]
        public BurndownType BurndownType
        {
            get
            {
                return _burndownType.Entity;
            }
            set
            {
                BurndownType previousValue = _burndownType.Entity;
                if (((previousValue != value) || (_burndownType.HasLoadedOrAssignedValue == false)))
                {
                    NotifyPropertyChanging("BurndownType");
                    if ((previousValue != null))
                    {
                        _burndownType.Entity = null;
                        previousValue.Burndowns.Remove(this);
                    }
                    _burndownType.Entity = value;
                    if ((value != null))
                    {
                        value.Burndowns.Add(this);
                        _burndownTypeId = value.Id;
                    }
                    else
                    {
                        _burndownTypeId = default(int?);
                    }
                    NotifyPropertyChanged("BurndownType");
                }
            }
        }

        [Column(IsVersion = true)]
        private Binary _version;
    }
}