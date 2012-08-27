using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace MoneyBurnDown.Model.Entities
{
    [Table(Name = "Transactions")]
    public class Transaction : NotificationObjectBase
    {
        private int _id;
        private decimal _amount;
        private DateTime _createdAt;

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

        [Column(CanBeNull = false)]
        public virtual decimal Amount
        {
            get { return _amount; }
            set
            {
                if (_amount != value)
                {
                    NotifyPropertyChanging("Amount");
                    _amount = value;
                    NotifyPropertyChanged("Amount");
                }
            }
        }

        [Column(CanBeNull = false)]
        public virtual DateTime CreatedAt
        {
            get { return _createdAt; }
            set
            {
                if (_createdAt != value)
                {
                    NotifyPropertyChanged("CreatedAt");
                    _createdAt = value;
                    NotifyPropertyChanging("CreatedAt");
                }
            }
        }

        [Column]
        private int? _burndownId;

        private EntityRef<Burndown> _burndown;

        [Association(Storage = "_burndown", ThisKey = "_burndownId", OtherKey = "Id", IsForeignKey = true)]
        public Burndown Burndown
        {
            get
            {
                return _burndown.Entity;
            }
            set
            {
                Burndown previousValue = _burndown.Entity;
                if (((previousValue != value) || (_burndown.HasLoadedOrAssignedValue == false)))
                {
                    NotifyPropertyChanging("Burndown");
                    if ((previousValue != null))
                    {
                        _burndown.Entity = null;
                        previousValue.Transactions.Remove(this);
                    }
                    _burndown.Entity = value;
                    if ((value != null))
                    {
                        value.Transactions.Add(this);
                        _burndownId = value.Id;
                    }
                    else
                    {
                        _burndownId = default(int?);
                    }
                    NotifyPropertyChanged("Burndown");
                }
            }
        }

        [Column]
        private int? _transactionTypeId;

        private EntityRef<TransactionType> _transactionType;

        [Association(Name = "TransactionTypeToTransaction", Storage = "_transactionType", ThisKey = "_transactionTypeId", OtherKey = "Id", IsForeignKey = true)]
        public TransactionType TransactionType
        {
            get
            {
                return _transactionType.Entity;
            }
            set
            {
                TransactionType previousValue = _transactionType.Entity;
                if (((previousValue != value) || (_transactionType.HasLoadedOrAssignedValue == false)))
                {
                    NotifyPropertyChanging("TransactionType");
                    if ((previousValue != null))
                    {
                        _transactionType.Entity = null;
                        previousValue.Transactions.Remove(this);
                    }
                    _transactionType.Entity = value;
                    if ((value != null))
                    {
                        value.Transactions.Add(this);
                        _transactionTypeId = value.Id;
                    }
                    else
                    {
                        _transactionTypeId = default(int?);
                    }
                    NotifyPropertyChanged("TransactionType");
                }
            }
        }

        [Column(IsVersion = true)]
        private Binary _version;

        private bool? _isDeleted;

        [Column]
        public virtual bool? IsDeleted
        {
            get { return _isDeleted; }
            set
            {
                if (_isDeleted != value)
                {
                    NotifyPropertyChanging("IsDeleted");
                    _isDeleted = value;
                    NotifyPropertyChanged("IsDeleted");
                }
            }
        }
    }
}