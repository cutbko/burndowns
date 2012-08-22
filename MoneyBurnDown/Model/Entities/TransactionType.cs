using System.Data.Linq.Mapping;

namespace MoneyBurnDown.Model.Entities
{
    [Table(Name = "TransactionTypes")]
    public class TransactionType : NotificationObjectBase
    {
        private int _id;

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
    }
}