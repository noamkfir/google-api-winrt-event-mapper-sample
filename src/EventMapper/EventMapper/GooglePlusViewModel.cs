using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace GooglePlusSample
{
    public class GooglePlusViewModel : INotifyPropertyChanged
    {
        PlusContactsManager _plusContactsManager = new PlusContactsManager();
        EventCreationManager _eventCreationManager = new EventCreationManager();

        public PlusContactsManager PlusContactsManager { get { return _plusContactsManager; } }
        public EventCreationManager EventCreationManager { get { return _eventCreationManager; } }

        public GooglePlusViewModel()
        {
            _eventCreationManager.OnAddSelected += EventCreationManagerOnOnAddSelected;
            _plusContactsManager.AreFriendsLoaded = false;
            InitAuthentication();
        }

        private void EventCreationManagerOnOnAddSelected()
        {
            _eventCreationManager.SetSelectedPersons(_plusContactsManager.SelectedFriends);
        }

        private void InitAuthentication()
        {
            //Task.Factory.StartNew(() =>
            //{
            //    var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description)
            //    {
            //        ClientIdentifier = "954413084569.apps.googleusercontent.com",
            //        ClientSecret = "eIVrjsMOOF6NKzvvmcFTntYq"
            //    };
            //    var auth = new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthorization);


            //    PlusService service = new PlusService(new BaseClientService.Initializer()
            //    {
            //        Authenticator = auth,
            //        ApplicationName = "Plus API Sample",
            //    });

            //    // Create and execute the request.
            //    var request = service.People.List("me", PeopleResource.CollectionEnum.Visible);
            //    var peopleFeed = request.Execute();

            //    // List all items on this page.
            //    if (peopleFeed.Items != null && peopleFeed.Items.Any())
            //    {
            //        _plusContactsManager.SetPersonsList(peopleFeed.Items);
            //    }
            //});
        }

//        private IAuthorizationState GetAuthorization(NativeApplicationClient client)
//        {
//            // You should use a more secure way of storing the key here as
//            // .NET applications can be disassembled using a reflection tool.
//            const string STORAGE = "GooglePlusTesting";
//            const string KEY = "AIzaSyAEcnXQ1gkfpRo5WWCD_6CBdwsVhotCdt0";

//            // Check if there is a cached refresh token available.
//            IAuthorizationState state = AuthorizationMgr.GetCachedRefreshToken(STORAGE, KEY);
//            if (state != null)
//            {
//                try
//                {
//                    client.RefreshToken(state);
//                    return state; // Yes - we are done.
//                }
//                catch (DotNetOpenAuth.Messaging.ProtocolException ex)
//                {
//                    //CommandLine.WriteError("Using existing refresh token failed: " + ex.Message);
//                }
//            }

//            // Retrieve the authorization from the user.
//            state = AuthorizationMgr.RequestNativeAuthorization(client, PlusService.Scopes.PlusLogin.GetStringValue()
//);//_scope.ToArray());
//            AuthorizationMgr.SetCachedRefreshToken(STORAGE, KEY, state);
//            return state;
//        }

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
}
