using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EventMapper.Common;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace EventMapper
{
    public class CalendarViewModel : BindableBase
    {
        private ObservableCollection<CalendarListEntry> _calendars;
        private ObservableCollection<Event> _events;
        private CalendarService _calendarService;

        public CalendarViewModel()
        {
            _calendars = new ObservableCollection<CalendarListEntry>();
            //TODO: Add Authentication
            //var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description)
            //{
            //    ClientIdentifier = "919726503037.apps.googleusercontent.com",
            //    ClientSecret = "Lo67H2uMWH_TgvF9Cv4lnan-"
            //};

            //var auth = new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthorization);

            _calendarService = new CalendarService(/*new BaseClientService.Initializer
            {
                Authenticator = auth
            }*/);

            LoadCalendars();
        }

        public ObservableCollection<CalendarListEntry> Calendars
        {
            get
            {
                return _calendars;
            }
            set
            {
                if (_calendars == value)
                    return;

                _calendars = value;

                OnPropertyChanged();
            }
        }

        public ObservableCollection<Event> Events
        {
            get
            {
                return _events;
            }
            set
            {
                if (_events == value)
                    return;

                _events = value;

                OnPropertyChanged();
            }
        }

        public async Task LoadCalendars()
        {
            //var calendars = await Task.Run(() => _calendarService.CalendarList.List().ExecuteAsync().Result.Items;

            var calendars = new List<CalendarListEntry>
            {
                new CalendarListEntry{Summary = "Calendar 1", BackgroundColor = "#7bd148", Id = "Calendar1.someone@gmail.com", Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."},
                new CalendarListEntry{Summary = "Calendar 2", BackgroundColor = "#9fc6e7", Id = "Calendar2.someone@gmail.com", Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."},
                new CalendarListEntry{Summary = "Calendar 3", BackgroundColor = "#fad165", Id = "Calendar3.someone@gmail.com", Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."},
            };

            App.UIDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    _calendars.Clear();

                    foreach (var calendar in calendars)
                    {
                        _calendars.Add(calendar);
                    }
                });
        }

        public async Task LoadEvents(string calendarId)
        {
            //var events = await _calendarService.Events.List(calendarId).ExecuteAsync().Result.Items;

            var events = new List<Event>
            {
                new Event
                {
                    Summary = "Event 1",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
                },
                new Event
                {
                    Summary = "Event 2",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
                },
                new Event
                {
                    Summary = "Event 3",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
                },
                new Event
                {
                    Summary = "Event 4",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
                },
                new Event
                {
                    Summary = "Event 5",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
                },
            };

            App.UIDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                _events.Clear();

                foreach (var eve in events)
                {
                    _events.Add(eve);
                }
            });
        }
    }
}
