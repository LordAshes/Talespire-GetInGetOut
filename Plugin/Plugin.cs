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
        public const string Version = "1.0.0.0";

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
                if (!locations.ContainsKey(asset.Creature.CreatureId)) { locations.Add(asset.Creature.CreatureId, ""); }
                Debug.Log("Creature " + StatMessaging.GetCreatureName(asset) + " at " + asset.CreatureRoot.transform.position.ToString() + " in " + locations[asset.Creature.CreatureId]);
                bool foundLocation = false;
                foreach (StateHideVolume hvs in hvss.Values)
                {
                    Debug.Log("Hide Volume "+hvs.Name+" Bounds "+hvs.Volume.HideVolume.Bounds.min.x+"->"+ hvs.Volume.HideVolume.Bounds.max.x+"," + hvs.Volume.HideVolume.Bounds.min.y + "->" + hvs.Volume.HideVolume.Bounds.max.y + "," + hvs.Volume.HideVolume.Bounds.min.z + "->" + hvs.Volume.HideVolume.Bounds.max.z);
                    if (isInside(asset.CreatureRoot.position, hvs.Volume.HideVolume.Bounds))
                    {
                        // Asset is inside this hide volume
                        foundLocation = true;
                        if (locations[asset.Creature.CreatureId] != hvs.Name)
                        {
                            // Asset just moved into this hide volume
                            locations[asset.Creature.CreatureId] = hvs.Name;
                            StatMessaging.SetInfo(asset.Creature.CreatureId, "AssetLocation", hvs.Name);
                            Debug.Log("Get In / Get Out Plugin: " + StatMessaging.GetCreatureName(asset) + " has entered " + locations[asset.Creature.CreatureId]);
                        }
                        else
                        {
                            Debug.Log("Get In / Get Out Plugin: " + StatMessaging.GetCreatureName(asset) + " is still in " + locations[asset.Creature.CreatureId]);
                        }

                    }
                    if (foundLocation) { break; }
                }
                if (!foundLocation)
                {
                    if (locations[asset.Creature.CreatureId] != "")
                    {
                        // Asset is no longer in a hide volume
                        Debug.Log("Get In / Get Out Plugin: " + StatMessaging.GetCreatureName(asset) + " has exited " + locations[asset.Creature.CreatureId]);
                        locations[asset.Creature.CreatureId] = "";
                        StatMessaging.ClearInfo(asset.Creature.CreatureId, "AssetLocation");
                    }
                }
            }
        }

        public bool isInside(Vector3 pos, Bounds bounds)
        {
            if (pos.x < bounds.min.x) { return false; }
            if (pos.x > bounds.max.x) { return false; }
            if (pos.y < bounds.min.y) { return false; }
            if (pos.y > bounds.max.y) { return false; }
            if (pos.z < bounds.min.z) { return false; }
            if (pos.z > bounds.max.z) { return false; }
            return true;
        }
    }
}
