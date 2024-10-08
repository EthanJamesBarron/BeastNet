﻿using BepInEx;
using BepInEx.Unity.IL2CPP;
using Coatsink.UnityServices;
using CoreNet.Messaging.Messages;
using GB.Data;
using HarmonyLib;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace BeastNet
{
    [BepInPlugin("com.ethanbarron.il2cppmodding", "GangBeastsIL2CPPDevelopment", "0.0.1")]
    public class Main : BasePlugin
    {
        bool menuHasLoadedPreviously;

        public override void Load()
        {
            AddComponent<Core>();

            new Harmony("Cheese").PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
            SceneManager.add_sceneLoaded(new Action<Scene, LoadSceneMode>(Networking.MessageManager.OnSceneLoaded));
            SceneManager.add_sceneLoaded(new Action<Scene, LoadSceneMode>(WrapperFix));
        }

        public void WrapperFix(Scene scene, LoadSceneMode mode)
        {
            if (menuHasLoadedPreviously) return;
            if (scene.name == "Menu")
            {
                menuHasLoadedPreviously = true;

                AddComponent<DevelopmentTestServer>().ui = GameObject.Find("Global(Clone)/UI/PlatformUI/Development Server Menu").GetComponent<DevelopmentTestServerUI>();

                // Launching from my server terminal gives the server arg which just mutes the game, hides the load level UI (it bugs out) and attempts to
                // initialize integral coatsink wrappers
                if (Environment.GetCommandLineArgs().Contains("-SERVER"))
                {
                    UnityServicesManager.Instance.Initialise(UnityServicesManager.InitialiseFlags.DedicatedServer, null, "", "DGS");
                    AudioListener.pause = true;
                    GameObject.Find("Global(Clone)/LevelLoadSystem").SetActive(false);
                }
            }
        }
    }

    public class Core : MonoBehaviour
    {
        public void OnGUI()
        {
            if (GUILayout.Button("Send message"))
            {
                Networking.NetworkUtils.SendMessage(6942, NetNullMessage.CachedEmptyMessage, NetworkServer.active, true);
            }
        }
    }

    // DevelopmentTestServer looks for impossible command line arguments so I just fixed it
    [HarmonyPatch(typeof(DevelopmentTestServer), nameof(DevelopmentTestServer.Awake))]
    public class CommandLineFix
    {
        public static void Postfix()
        {
            string valueForKey = CommandLineParser.Instance.GetValueForKey("-DDC_IP", true);
            string valueForKey2 = CommandLineParser.Instance.GetValueForKey("-DDC_PORT", true);
            if (!string.IsNullOrEmpty(valueForKey2))
            {
                DevelopmentTestServer.DirectConnectPort = int.Parse(valueForKey2);
            }
            if (!string.IsNullOrEmpty(valueForKey))
            {
                DevelopmentTestServer.DirectConnectIP = valueForKey;
            }
        }
    }

    // Temporary
    [HarmonyPatch(typeof(StringLoader), nameof(StringLoader.LoadString))]
    public static class StringTester
    {
        public static void Postfix(ref string key)
        {
            Debug.LogError("LoadString called with a key of " + key);
        }
    }
}
