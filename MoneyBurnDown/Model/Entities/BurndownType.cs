using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace MoneyBurnDown.Model.Entities
{
    [Table(Name = "BurndownTypes")]
    public class BurndownType : NotificationObjectBase
    {
        private readonly EntitySet<Burndown> _burndowns;
        private int _id;
        private string _name;
        private DateTime _createdAt;

        public BurndownType()
        {
            _burndowns = new EntitySet<Burndown>(AttachBurndown, DetachBurndown);
        }

        private void DetachBurndown(Burndown burndown)
        {
            NotifyPropertyChanging("Burndown");
            burndown.BurndownType = this;
        }

        private void AttachBurndown(Burndown burndown)
        {
            NotifyPropertyChanging("Burndown");
            burndown.BurndownType = this;
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
                if (String.CompareOrdinal(_name, value) != 0)
                {
                    NotifyPropertyChanged("Name");
                    _name = value;
                    NotifyPropertyChanging("Name");
                }
            }
        }
        
        [Column]
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

        [Association(Storage = "_burndowns", ThisKey = "Id", OtherKey = "_burndownTypeId")]
        public EntitySet<Burndown> Burndowns
        {
            get { return _burndowns; }
            set
            {
                _burndowns.Assign(value);
            }
        }

        [Column(IsVersion = true)]
        private Binary _version;
    }
}