using CoreNet.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace BeastNet.Networking
{
    public static class NetworkUtils
    {
        public static void SendMessage(short messageCode, MessageBase message, bool serverIsSendingThis = true, bool reliable = true)
        {
            if (message is null)
            {
                Debug.LogError("Message passed to SendMessage is null!");
                return;
            }

            if (serverIsSendingThis)
            {
                if (NetworkServer.active)
                {
                    NetworkServer.SendByChannelToAll(messageCode, message, reliable ? 0 : 1);
                    return;
                }

                Debug.LogError("SendMessageToAll was meant to be used on the server but has been used on the client!");
                return;
            }

            if (NetworkClient.active) NetworkManager.singleton.client.SendByChannel(messageCode, message, reliable ? 0 : 1);
        }
    }
}
