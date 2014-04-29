using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.XMPP;

namespace PvPNetChatClient
{

    public class ClientStateEventArgs : EventArgs
    {
        public string State { get; set; }

        public ClientStateEventArgs(string state) : base()
        {
            this.State = state;
        }
    }

    public class ConversationItemEventArgs : EventArgs
    {
        public Guid Key;

        public ConversationItemEventArgs(Guid key) : base()
        {
            Key = key;
        }
    }

    public class ConversationItem : INotifyPropertyChanged
    {
        public RosterItem RosterItem { get; set; }
        public TextMessage Message { get; set; }

        public ConversationItem(RosterItem rosterItem, TextMessage message)
            : base()
        {
            RosterItem = rosterItem;
            Message = message;
            OnPropertyChanged("RosterItem");
            OnPropertyChanged("Message");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }

    public class Client
    {
        private const int _port = 5223;
        private const string _passPrefix = "AIR_";
        private const string _domain = "pvp.net";
        private XMPPClient XMPPClient = new XMPPClient();

        public event EventHandler<ClientStateEventArgs> StateChanged;

        public event EventHandler Connected;

        public event EventHandler Disconnected;

        public event EventHandler RosterRetrieved;

        public event EventHandler<ConversationItemEventArgs> NewConversationItem;

        public Dictionary<Guid, ConversationItem> ConversationItems = new Dictionary<Guid,ConversationItem>();

        public static Dictionary<Constants.Server.Region, string> _serverAddresses = new Dictionary<Constants.Server.Region, string>()
        {
            {Constants.Server.Region.EUNE, "chat.eun1.lol.riotgames.com"},
            {Constants.Server.Region.EUW, "chat.eu.lol.riotgames.com"},
            {Constants.Server.Region.NA, "chat.na1.lol.riotgames.com"},

        };

        public List<RosterItem> GetRoster()
        {
           return XMPPClient.RosterItems.ToList();      
        }

        public void SendMessage(string user, string message)
        {
            var jid = XMPPClient.RosterItems.First( o => o.Name == user).JID;
            XMPPClient.SendChatMessage(message, jid);
        }

        public void Connect(string username, string password, Constants.Server.Region region)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || region == Constants.Server.Region.UNK)
                throw new ArgumentException("Username, password or region is emtpy");

            var address = _serverAddresses.Single(o => o.Key == region).Value;

            XMPPClient.UserName = username;
            XMPPClient.Password = _passPrefix + password;
            XMPPClient.Server = address;
            XMPPClient.Domain = _domain;
            XMPPClient.Resource = "xiff";
            XMPPClient.Port = 5223;

            XMPPClient.UseTLS = true;

            XMPPClient.UseOldStyleTLS = true;

            XMPPClient.AutoAcceptPresenceSubscribe = false;
            XMPPClient.AutomaticallyDownloadAvatars = false;
            XMPPClient.RetrieveRoster = true;

            XMPPClient.OnStateChanged += XMPPClient_OnStateChanged;
            XMPPClient.OnRetrievedRoster += XMPPClient_OnRetrievedRoster;
            XMPPClient.OnNewConversationItem += XMPPClient_OnNewConversationItem;
            XMPPClient.Connect();
        }

        void XMPPClient_OnNewConversationItem(RosterItem item, bool bReceived, TextMessage msg)
        {
            if (bReceived)
            {
                var key = Guid.NewGuid();
                ConversationItems.Add(key, new ConversationItem(item, msg));
                if (NewConversationItem != null)
                    NewConversationItem(this, new ConversationItemEventArgs(key));
            }
        }

        void XMPPClient_OnRetrievedRoster(object sender, EventArgs e)
        {
            if (RosterRetrieved != null)
                RosterRetrieved(this, EventArgs.Empty);
        }

        void XMPPClient_OnStateChanged(object sender, EventArgs e)
        {
            var state = XMPPClient.XMPPState;
            OnStateChanged(state.ToString());
            if (state == XMPPState.Ready)
            {
                XMPPClient.PresenceStatus.PresenceType = PresenceType.available;
                XMPPClient.PresenceStatus.Status = "online";
                XMPPClient.PresenceStatus.PresenceShow = PresenceShow.chat;
                XMPPClient.UpdatePresence();
                OnConnected();
            }
            if(state == XMPPState.Unknown)
            {
                OnDisconnected();
            }
        }

        private void OnDisconnected()
        {
            if (Disconnected != null)
                Disconnected(this, EventArgs.Empty);
        }

        private void OnConnected()
        {
            if (Connected != null)
                Connected(this, EventArgs.Empty);
        }

        private void OnStateChanged(string state)
        {
            if (StateChanged != null)
                StateChanged(this, new ClientStateEventArgs(state));
        }
    }
}
