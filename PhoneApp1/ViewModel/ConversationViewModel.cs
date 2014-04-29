using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PvPNetChatClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.XMPP;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PhoneApp1.ViewModel
{
    public class ConversationViewModel : ViewModelBase
    {
        private string _partnerName;

        public string PartnerName 
        { 
            get
            {
                return _partnerName;
            }
            set
            {
                _partnerName = value;
                RaisePropertyChanged("PartnerName");
            }
        }

        private ObservableCollection<ConversationItem> _messages = new ObservableCollection<ConversationItem>();

        public ObservableCollection<ConversationItem> Messages
        {
            get
            {
                return _messages;
            }
            set
            {
                _messages = value;
                RaisePropertyChanged("Messages");
            }
        }

        private string _message;

        public string Message 
        { 
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                RaisePropertyChanged(() => Message);
            }
        }

        public ICommand SendMessageCommand { get; set; }

        public ConversationViewModel(string key)
        {
            PartnerName = key;
            SendMessageCommand = new RelayCommand(() => SendMessage());
            Messages = new ObservableCollection<ConversationItem>( App.Client.ConversationItems.Where(o => o.Value.RosterItem.Name == key).Select(s => s.Value));

            App.Client.NewConversationItem += Client_NewConversationItem;
        }

        void SendMessage()
        {
            App.Client.SendMessage(PartnerName,Message);
            Messages.Add(new ConversationItem(new RosterItem() { Name = "Me" }, new TextMessage() { Message = this.Message }));
            Message = string.Empty;
        }

        void Client_NewConversationItem(object sender, ConversationItemEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
                   {
                       var item = App.Client.ConversationItems[e.Key];
                       if (item.RosterItem.Name == PartnerName)
                           Messages.Add(item);
                   });
        }
    }
}
