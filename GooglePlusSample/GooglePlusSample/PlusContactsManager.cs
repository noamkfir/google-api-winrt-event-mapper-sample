using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Google.Apis.Plus.v1.Data;

namespace GooglePlusSample
{
    public class PlusContactsManager : INotifyPropertyChanged
    {
        private IEnumerable<PersonViewModel> _friends;
        private bool _areFriendsLoaded;

        public bool AreFriendsLoaded
        {
            get { return _areFriendsLoaded; }
            set
            {
                _areFriendsLoaded = value;
                NotifyOnPropertyChanged(() => AreFriendsLoaded);
                NotifyOnPropertyChanged(() => AreFriendsLoadedInverted);
            }
        }

        public bool AreFriendsLoadedInverted
        {
            get { return !_areFriendsLoaded; }
        }

        public IEnumerable<PersonViewModel> Friends
        {
            get { return _friends; }
        }

        public IEnumerable<PersonViewModel> SelectedFriends
        {
            get { return _friends.Where(pvm => pvm.IsSelected); }
        }

        public void SetPersonsList(IEnumerable<Person> persons)
        {
            if (persons == null)
                throw new ArgumentNullException("persons");

            IList<PersonViewModel> items = new List<PersonViewModel>();
            foreach (Person person in persons)
            {
                items.Add(new PersonViewModel(person));
            }
            _friends = items;
            AreFriendsLoaded = true;
            NotifyOnPropertyChanged(() => Friends);
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
