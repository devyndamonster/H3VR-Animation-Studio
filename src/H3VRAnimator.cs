﻿using BepInEx;
using FistVR;
using H3VRAnimator.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Stratum;
using System.Collections;
using Sodalite;

namespace H3VRAnimator
{
    [BepInPlugin("devyndamonster.h3vr.animator", "H3VR Animator", "0.3.0")]
    [BepInDependency(StratumRoot.GUID, StratumRoot.Version)]
    [BepInDependency(SodaliteConstants.Guid, SodaliteConstants.Version)]
    public class H3VRAnimator : StratumPlugin
    {
        public static SpectatorPanelAnimator SpectatorPanel;

        public static Material NightVisionMaterial;

        private void Awake()
        {
            AnimLogger.Init();

            Harmony.CreateAndPatchAll(typeof(H3VRAnimator));
        }


        public override void OnSetup(IStageContext<Empty> ctx)
        {
            ctx.Loaders.Add("anim_assets", LoadShaders);
        }

        public override IEnumerator OnRuntime(IStageContext<IEnumerator> ctx)
        {
            yield break;
        }

        


        [HarmonyPatch(typeof(SpectatorPanel), "Start")] // Specify target method with HarmonyPatch attribute
        [HarmonyPrefix]
        public static bool AddButton(SpectatorPanel __instance)
        {
            SpectatorPanel = __instance.gameObject.AddComponent<SpectatorPanelAnimator>();

            return true;
        }


        [HarmonyPatch(typeof(FVRViveHand), "Awake")] // Specify target method with HarmonyPatch attribute
        [HarmonyPrefix]
        public static bool PreventAwake(FVRViveHand __instance)
        {
            if (__instance.gameObject.name.Contains("FakeHand"))
            {
                AnimLogger.Log("Preventing fake hand from awaking");
                return false;
            }

            return true;
        }


        [HarmonyPatch(typeof(FVRViveHand), "Start")] // Specify target method with HarmonyPatch attribute
        [HarmonyPrefix]
        public static bool PreventStart(FVRViveHand __instance)
        {
            if(__instance.gameObject.name.Contains("FakeHand"))
            {
                AnimLogger.Log("Preventing fake hand from starting");
                return false;
            }

            return true;
        }


        [HarmonyPatch(typeof(FVRPhysicalObject), "FU")] // Specify target method with HarmonyPatch attribute
        [HarmonyPrefix]
        public static bool PreventUpdate(FVRPhysicalObject __instance)
        {
            if (__instance.IsHeld && __instance.gameObject.name.Contains("Animated_"))
            {
                return false;
            }

            return true;
        }

        public Empty LoadShaders(FileSystemInfo info)
        {
            string path = info.FullName;
            AnimLogger.Log("Loading shaders from path: " + path);

            AssetBundle bundle = AssetBundle.LoadFromFile(path);
            Material[] mats = bundle.LoadAllAssets<Material>();

            AnimLogger.Log("Loaded Materials: " + mats.Length);
            NightVisionMaterial = mats[0];

            return new Empty();
        }

        
    }
}
