using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.XMPP;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PhoneApp1.ViewModel
{
    public class ChatViewModel : ViewModelBase
    {
        public ICommand OnViewLoaded { get; set; }
        public static ICommand OpenConversationCommand { get; set; }

        private RosterItem _selectedRosterItem;

        public RosterItem SelectedRosterItem 
        { 
            get
            {
                return _selectedRosterItem;
            }
            set
            {
                _selectedRosterItem = value;
                RaisePropertyChanged(() => SelectedRosterItem);
            }
        }

        private ObservableCollection<RosterItem> _roster;

        public ObservableCollection<RosterItem> Roster 
        { 
            get
            {
                return _roster;
            }
            set
            {
                _roster = value;
                Thread.Sleep(3000);
                RaisePropertyChanged(() => Roster);
            }
        }

        public ChatViewModel()
        {
            OnViewLoaded = new RelayCommand(() => ViewLoaded());
            OpenConversationCommand = new RelayCommand(() => OpenConversation());
        }

        void ViewLoaded()
        {
            App.Client.GetRoster();
            App.Client.RosterRetrieved += _client_RosterRetrieved;
        }

        public void OpenConversation()
        {
            ViewModelLocator.SelectConversation(SelectedRosterItem.Name);
        }

        void _client_RosterRetrieved(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    var newRoster = App.Client.GetRoster();
                    if(Roster == null)
                    Roster = new ObservableCollection<RosterItem>(newRoster);
                    else
                    {
                        foreach(var item in Roster.Intersect(newRoster))
                        {
                            Roster.Remove(item);
                        }
                        foreach (var item in newRoster.Intersect(Roster))
                        {
                            Roster.Add(item);
                        }
                    }
                });
        }
    }
}
