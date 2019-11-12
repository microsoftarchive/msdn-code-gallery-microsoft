using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Proximity;

namespace CritterStomp
{
    public class AvailablePeer : INotifyPropertyChanged
    {
        public AvailablePeer(PeerInformation peer)
        {
            Peer = peer;
        }

        private string status;

        public PeerInformation Peer { get; private set; }
        public String Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged("Status");
                OnPropertyChanged("DisplayName");
            }
        }

        public String DisplayName
        {
            get { return ToString(); }
        }

        public override string ToString()
        {
            return Peer.DisplayName + Status;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
