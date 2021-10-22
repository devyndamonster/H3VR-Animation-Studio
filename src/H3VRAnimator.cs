using BepInEx;
using FistVR;
using H3VRAnimator.Logging;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace H3VRAnimator
{
    [BepInPlugin("devyndamonster.h3vr.animator", "H3VR Animator", "0.1.0")]
    public class H3VRAnimator : BaseUnityPlugin
    {
        public static SpectatorPanelAnimator SpectatorPanel;

        private void Awake()
        {
            AnimLogger.Init();

            Harmony.CreateAndPatchAll(typeof(H3VRAnimator));
        }

        [HarmonyPatch(typeof(SpectatorPanel), "Start")] // Specify target method with HarmonyPatch attribute
        [HarmonyPrefix]
        public static bool AddButton(SpectatorPanel __instance)
        {
            SpectatorPanel = __instance.gameObject.AddComponent<SpectatorPanelAnimator>();

            return true;
        }

    }
}
