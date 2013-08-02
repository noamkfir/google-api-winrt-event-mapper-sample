using Google.Apis.Plus.v1.Data;
using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace GooglePlusSample
{
    public class PersonViewModel : INotifyPropertyChanged
    {
        private Person _person;
        private bool _isSelected;

        public Person Person { get { return _person; } }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                NotifyOnPropertyChanged(() => IsSelected);
            }
        }

        public PersonViewModel(Person person)
        {
            _person = person;
        }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyOnPropertyChanged<T>(Expression<Func<T>> expression)
        {
            MemberExpression body = expression.Body as MemberExpression;
            if (body != null)
                NotifyOnPropertyChanged(body.Member.Name);
        }

        private void NotifyOnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
