using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GooglePlusSample
{
    public class EventCreationManager : INotifyPropertyChanged
    {
        private ICommand _removeSelectedCommand;
        private ICommand _addSelectedCommand;
        private ObservableCollection<PersonViewModel> _selectedFriends = new ObservableCollection<PersonViewModel>();
        private readonly Dictionary<TimeFrame, string> _timeFrames = new Dictionary<TimeFrame, string>
        {
            {TimeFrame.Minutes, TimeFrame.Minutes.ToString()},
            {TimeFrame.Hours, TimeFrame.Hours.ToString()},
            {TimeFrame.Days, TimeFrame.Days.ToString()}
        };
        private readonly Dictionary<NotificationType, string> _notificationTypes = new Dictionary<NotificationType, string>
        {
            {NotificationType.Email, NotificationType.Email.ToString()},
            {NotificationType.SMS, NotificationType.SMS.ToString()}
        };

        public event AddSelectedHandler OnAddSelected;

        public ICommand RemoveSelectedCommand
        {
            get
            {
                return _removeSelectedCommand ??
                       (_removeSelectedCommand = new ActionCommand<PersonViewModel>(RemoveSelectedCommandAction));
            }
        }

        public ICommand AddSelectedCommand
        {
            get
            {
                return _addSelectedCommand ??
                    (_addSelectedCommand = new ActionCommand(AddSelectedCommandAction));
            }
        }

        public Dictionary<NotificationType, string> NotificationTypes
        {
            get { return _notificationTypes; }
        }

        public Dictionary<TimeFrame, string> TimeFrames
        {
            get { return _timeFrames; }
        }

        public ObservableCollection<PersonViewModel> SelectedFriends
        {
            get { return _selectedFriends; }
        }

        public void SetSelectedPersons(IEnumerable<PersonViewModel> personViewModels)
        {
            foreach (PersonViewModel viewModel in personViewModels)
            {
                if (!_selectedFriends.Contains(viewModel))
                    _selectedFriends.Add(viewModel);
                viewModel.IsSelected = false;
            }
        }

        private void RemoveSelectedCommandAction(PersonViewModel personViewModel)
        {
            if (personViewModel != null)
            {
                if (_selectedFriends.Contains(personViewModel))
                    _selectedFriends.Remove(personViewModel);
            }
        }

        private void AddSelectedCommandAction()
        {
            if (OnAddSelected != null)
                OnAddSelected();
        }

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
    }

    public delegate void AddSelectedHandler();
}
