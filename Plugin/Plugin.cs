using BepInEx;
using BepInEx.Configuration;
using Bounce.Unmanaged;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using static LordAshes.HideVolumeMenuPlugin;

namespace LordAshes
{
    [BepInPlugin(Guid, Name, Version)]
    [BepInDependency(LordAshes.StatMessaging.Guid)]
    [BepInDependency(LordAshes.HideVolumeMenuPlugin.Guid)]
    public partial class GetInGetOutPlugin : BaseUnityPlugin
    {
        // Plugin info
        public const string Name = "Get In / Get Out Plug-In";         
        public const string Guid = "org.lordashes.plugins.getingetout";
        public const string Version = "2.0.0.0";

        // Configuration
        Dictionary<CreatureGuid, string> locations = new Dictionary<CreatureGuid, string>();

        void Awake()
        {
            UnityEngine.Debug.Log("Get In / Get Out Plugin: Active.");

            var harmony = new Harmony(Guid);
            harmony.PatchAll();

            Register(() => { CheckAreas(); });
        }

        void Update()
        {
            if (Utility.isBoardLoaded())
            {
            }
        }

        public void CheckAreas()
        {
            HideVolumeMenuPlugin hvm = HideVolumeMenuPlugin.Instance();
            Dictionary<NGuid,StateHideVolume> hvss = hvm.CurrentHideVolumeStates;
            foreach (CreatureBoardAsset asset in CreaturePresenter.AllCreatureAssets)
            {
                if (!locations.ContainsKey(asset.CreatureId)) { locations.Add(asset.CreatureId, ""); }
                Debug.Log("Get In / Get Out Plugin: Creature " + StatMessaging.GetCreatureName(asset) + " at " + Utility.GetRootObject(asset.CreatureId).transform.position.ToString() + " was in " + locations[asset.CreatureId]);
                bool foundLocation = false;
                foreach (StateHideVolume hvs in hvss.Values)
                {
                    Debug.Log("Get In / Get Out Plugin: Hide Volume " + hvs.Name+" Bounds "+hvs.Volume.HideVolume.Bounds.min.x+"->"+ hvs.Volume.HideVolume.Bounds.max.x+"," + hvs.Volume.HideVolume.Bounds.min.y + "->" + hvs.Volume.HideVolume.Bounds.max.y + "," + hvs.Volume.HideVolume.Bounds.min.z + "->" + hvs.Volume.HideVolume.Bounds.max.z);
                    if (isInside(Utility.GetRootObject(asset.CreatureId).transform.position, hvs.Volume.HideVolume.Bounds))
                    {
                        Debug.Log("Get In / Get Out Plugin: Creature Is In Now In Area "+ hvs.Name);
                        // Asset is inside this hide volume
                        foundLocation = true;
                        if (locations[asset.CreatureId]=="" && locations[asset.CreatureId] != hvs.Name)
                        {
                            // Asset just moved into this hide volume
                            locations[asset.CreatureId] = hvs.Name;
                            StatMessaging.SetInfo(asset.CreatureId, "AssetLocation", hvs.Name);
                            Debug.Log("Get In / Get Out Plugin: " + StatMessaging.GetCreatureName(asset) + " has entered " + locations[asset.CreatureId]);
                        }
                        else if (locations[asset.CreatureId] != "" && locations[asset.CreatureId] != hvs.Name)
                        {
                            // Asset just moved from hide volume into this hide volume
                            Debug.Log("Get In / Get Out Plugin: " + StatMessaging.GetCreatureName(asset) + " has exited " + locations[asset.CreatureId]);
                            locations[asset.CreatureId] = "";
                            StatMessaging.ClearInfo(asset.CreatureId, "AssetLocation");
                            locations[asset.CreatureId] = hvs.Name;
                            StatMessaging.SetInfo(asset.CreatureId, "AssetLocation", hvs.Name);
                            Debug.Log("Get In / Get Out Plugin: " + StatMessaging.GetCreatureName(asset) + " has entered " + locations[asset.CreatureId]);
                        }
                        else
                        {
                            Debug.Log("Get In / Get Out Plugin: " + StatMessaging.GetCreatureName(asset) + " is still in " + locations[asset.CreatureId]);
                        }

                    }
                    if (foundLocation) { break; }
                }
                if (!foundLocation)
                {
                    if (locations[asset.CreatureId] != "")
                    {
                        // Asset is no longer in a hide volume
                        Debug.Log("Get In / Get Out Plugin: " + StatMessaging.GetCreatureName(asset) + " has exited " + locations[asset.CreatureId]);
                        locations[asset.CreatureId] = "";
                        StatMessaging.ClearInfo(asset.CreatureId, "AssetLocation");
                    }
                }
            }
        }

        public bool isInside(Vector3 pos, Bounds bounds)
        {
            if (pos.x < bounds.min.x) { return false; }
            if (pos.x > bounds.max.x) { return false; }
            // if (pos.y < bounds.min.y) { return false; }
            // if (pos.y > bounds.max.y) { return false; }
            if (pos.z < bounds.min.z) { return false; }
            if (pos.z > bounds.max.z) { return false; }
            return true;
        }
    }
}
