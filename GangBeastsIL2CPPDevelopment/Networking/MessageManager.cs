using GB.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace BeastNet.Networking
{
    public class MessageManager
    {
        public enum MessageSide
        {
            Server,
            Client,
            Both
        }

        public delegate void ManagedNetworkDelegate(NetworkMessage netMsg);
        Dictionary<short, ManagedNetworkDelegate> clientMessageRelationships = new Dictionary<short, ManagedNetworkDelegate>();
        Dictionary<short, ManagedNetworkDelegate> serverMessageRelationships = new Dictionary<short, ManagedNetworkDelegate>();

        public void AddMessageToQueue(short messageCode, ManagedNetworkDelegate methodCallback, MessageSide side)
        {
            switch (side)
            {
                case MessageSide.Server:
                    serverMessageRelationships.Add(messageCode, methodCallback);
                    break;

                case MessageSide.Client:
                    clientMessageRelationships.Add(messageCode, methodCallback);
                    break;

                default:
                    clientMessageRelationships.Add(messageCode, methodCallback);
                    serverMessageRelationships.Add(messageCode, methodCallback);
                    break;
            }
        }


    }
}
