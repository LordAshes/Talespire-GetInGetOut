using System;
using System.Collections.Generic;
using System.IO;

using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace LordAshes
{
    public partial class GetInGetOutPlugin : BaseUnityPlugin
    {
        public static Action _callback = null;

        public static void Register(Action callback)
        {
            _callback = callback;
        }

        [HarmonyPatch(typeof(MovableBoardAsset), "Drop")]
        public static class Patch01
        {
            public static bool Prefix(Vector3 dropDestination, float height)
            {
                Debug.Log("Get In / Get Out Plugin: Patch: Object Dropped");
                _callback();
                return true;
            }
        }
    }
}