using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace BeastNet.Networking
{
    public static class MessageManager
    {
        static Dictionary<short, NetworkMessageDelegate> clientMessageRelationships = new Dictionary<short, NetworkMessageDelegate>();

        public static void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            if (scene.name != "Menu" && scene.name != "_bootScene")
            {
                if (NetworkManager.singleton.client != null)
                {
                    for (int i = 0; i < clientMessageRelationships.Count; i++)
                    {
                        KeyValuePair<short, NetworkMessageDelegate> element = clientMessageRelationships.ElementAt(i);
                        NetworkManager.singleton.client.RegisterHandler(element.Key, element.Value);
                    }
                }
            }
        }

        public static void AddMessageToQueue(short messageCode, Action<NetworkMessage> methodCallback, bool server = true)
        {
            if (server)
            {
                NetworkServer.RegisterHandler(messageCode, (NetworkMessageDelegate)methodCallback);
            }
        }
    }
}
