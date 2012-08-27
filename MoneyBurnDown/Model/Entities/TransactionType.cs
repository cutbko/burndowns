using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace MoneyBurnDown.Model.Entities
{
    [Table(Name = "TransactionTypes")]
    public class TransactionType : NotificationObjectBase
    {
        private int _id;
        private string _name;
        private DateTime _createdAt;
        private EntitySet<Transaction> _transactions;

        public TransactionType()
        {
            _transactions = new EntitySet<Transaction>(AttachTransaction, DetachTransaction);
        }

        private void DetachTransaction(Transaction transaction)
        {
            NotifyPropertyChanging("Transaction");
        }

        private void AttachTransaction(Transaction transaction)
        {
            NotifyPropertyChanging("Transaction");
            transaction.TransactionType = this;
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

        [Column(CanBeNull = false)]
        public virtual string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    NotifyPropertyChanging("Name");
                    _name = value;
                    NotifyPropertyChanged("Name");
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

        [Association(Storage = "_transactions", ThisKey = "Id", OtherKey = "_burndownId")]
        public EntitySet<Transaction> Transactions
        {
            get { return _transactions; }
            set
            {
                _transactions.Assign(value);
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
                if(_isDeleted != value)
                {
                    NotifyPropertyChanging("IsDeleted");
                    _isDeleted = value;
                    NotifyPropertyChanged("IsDeleted");
                }
            }
        }
    }
}