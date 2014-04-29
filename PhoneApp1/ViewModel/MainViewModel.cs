using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PvPNetChatClient;
using System.Collections.Generic;
using System.Windows.Input;
using System.Linq;
using Microsoft.Phone.Controls;
using System.IO.IsolatedStorage;
using System.Windows.Navigation;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows;
using System;

namespace PhoneApp1.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {

        private string _username;
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                RaisePropertyChanged(() => Username);
            }
        }

        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                RaisePropertyChanged(() => Password);
            }
        }

        public List<string> _regions;
        public List<string> Regions
        {
            get
            {
                return _regions;
            }
            set
            {
                _regions = value;
                RaisePropertyChanged(() => Regions);
            }
        }

        public NavigationService NavigationService { get; set; }

        private PvPNetChatClient.Constants.Server.Region _selectedRegion;
        public string SelectedRegionName
        {
            get
            {
                string value;
                if (_selectedRegion != null && Constants.Server.RegionNames.TryGetValue(_selectedRegion, out value))
                    return value;
                else
                {
                    var first = Constants.Server.RegionNames.First();
                    _selectedRegion = first.Key;
                    return first.Value;
                }
            }
            set
            {
                _selectedRegion = Constants.Server.RegionNames.SingleOrDefault(o => o.Value == value).Key;
                RaisePropertyChanged(() => SelectedRegionName);
            }
        }       
        public ICommand ConnectCommand { get; set; }
        public ICommand OnViewLoaded { get; set; }

        private bool _connecting;

        public bool Connecting
        {
            get
            {
                return _connecting;
            }
            set
            {
                _connecting = value;
                RaisePropertyChanged(() => Connecting);
            }
        }

        private string _clientState;

        public string ClientState
        {
            get
            {
                return _clientState;
            }
            set
            {
                _clientState = value;
                RaisePropertyChanged(() => ClientState);
            }
        }

        private bool _autoLogin = true;

        public MainViewModel()
        {

            ConnectCommand = new RelayCommand(() => Connect());
            OnViewLoaded = new RelayCommand(() => AfterViewLoaded());

            Regions = Constants.Server.RegionNames.Values.ToList();
            SelectedRegionName = Regions.First();
            App.Client.StateChanged += _client_StateChanged;
            App.Client.Disconnected += (o, p) => this.Cleanup();

            LoadSettings();
        }

        void AfterViewLoaded()
        {

            if (_autoLogin)
                Connect();
        }
        void _client_StateChanged(object sender, ClientStateEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                ClientState = e.State;
            });
        }

        public void Connect()
        {
            SaveSettings();
            App.Client.Connected += (o, p) =>
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    Connecting = false; NavigationService.Navigate(new System.Uri("/Chatpage.xaml", UriKind.Relative)); 
                });
            };
            Connecting = true;

            App.Client.Connect(Username, Password, _selectedRegion);          
        }

        public void LoadSettings()
        {
            string username;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue("Username", out username);
            Username = username;

            string password;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue("Password", out password);
            Password = password;

            string regionName;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue("SelectedRegionName", out regionName);
            SelectedRegionName = regionName;
        }

        public void SaveSettings()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("Username"))
                IsolatedStorageSettings.ApplicationSettings["Username"] = Username;
            else
                IsolatedStorageSettings.ApplicationSettings.Add("Username", Username);

            if (IsolatedStorageSettings.ApplicationSettings.Contains("Password"))
                IsolatedStorageSettings.ApplicationSettings["Password"] = Password;
            else
                IsolatedStorageSettings.ApplicationSettings.Add("Password", Password);

            if (IsolatedStorageSettings.ApplicationSettings.Contains("SelectedRegionName"))
                IsolatedStorageSettings.ApplicationSettings["SelectedRegionName"] = SelectedRegionName;
            else
                IsolatedStorageSettings.ApplicationSettings.Add("SelectedRegionName", SelectedRegionName);

            IsolatedStorageSettings.ApplicationSettings.Save();
        }
    }
}