using BepInEx;
using FistVR;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace H3VRAnimator
{
    [BepInPlugin("devyndamonster.h3vr.animator", "H3VR Animator", "0.1.0")]
    public class H3VRAnimator : BaseUnityPlugin
    {

        private static Transform DisplayPoint = null;
        public static AnimationPath Path;

        private void Awake()
        {
            AnimLogger.Init();

            Harmony.CreateAndPatchAll(typeof(H3VRAnimator));
        }

        [HarmonyPatch(typeof(SpectatorPanel), "Start")] // Specify target method with HarmonyPatch attribute
        [HarmonyPrefix]
        public static bool AddButton(SpectatorPanel __instance)
        {
            AnimLogger.Log("The Spectator Panel just started!");
            Path = new AnimationPath();

            GameObject createButton = Instantiate(__instance.List_Categories[0].gameObject, __instance.List_Categories[0].transform.parent);
            createButton.transform.localPosition = new Vector3(-850, 900, 0);
            createButton.transform.localScale = new Vector3(3, 3, 2);
            createButton.gameObject.GetComponent<Text>().text = "Create Point";

            Button buttonComp = createButton.gameObject.GetComponent<Button>();
            buttonComp.onClick = new Button.ButtonClickedEvent();

            buttonComp.onClick.AddListener(() => { AddPoint(createButton); });



            GameObject animateButton = Instantiate(__instance.List_Categories[0].gameObject, __instance.List_Categories[0].transform.parent);
            animateButton.transform.localPosition = new Vector3(-300, 900, 0);
            animateButton.transform.localScale = new Vector3(3, 3, 2);
            animateButton.gameObject.GetComponent<Text>().text = "Animate";

            Button animateComp = animateButton.gameObject.GetComponent<Button>();
            animateComp.onClick = new Button.ButtonClickedEvent();

            animateComp.onClick.AddListener(() => { AddAnimatedPoint(); });



            GameObject drawPoint = new GameObject();
            drawPoint.transform.SetParent(createButton.transform.parent);
            drawPoint.transform.localPosition = new Vector3(-850, 1000, 0);
            DisplayPoint = drawPoint.transform;

            return true;
        }


        private static void AddPoint(GameObject buttonObject)
        {
            GameObject anchorObject = new GameObject("MovablePoint");
            anchorObject.transform.position = DisplayPoint.position;
            PathAnchor anchorComp = anchorObject.AddComponent<PathAnchor>();

            Path.points.Add(anchorComp);
        }

        private static void AddAnimatedPoint()
        {

            float distLeft = Vector3.Distance(GM.CurrentPlayerBody.LeftHand.position, DisplayPoint.position);
            float distRight = Vector3.Distance(GM.CurrentPlayerBody.RightHand.position, DisplayPoint.position);
            FVRViveHand furthestHand;

            if (distLeft < distRight)
            {
                furthestHand = GM.CurrentPlayerBody.RightHand.GetComponent<FVRViveHand>();
            }
            else
            {
                furthestHand = GM.CurrentPlayerBody.LeftHand.GetComponent<FVRViveHand>();
            }

            FVRInteractiveObject interactable = furthestHand.CurrentInteractable;

            if(interactable == null)
            {
                AnimLogger.Log("Interactable was null");
                return;
            }

            else
            {
                AnimLogger.Log("Interactable was not null");
                AnimLogger.Log("Hand: " + furthestHand);
                interactable.IsHeld = false;
                furthestHand.Input.IsGrabbing = false;

                interactable.EndInteraction(furthestHand);
                furthestHand.CurrentInteractable = null;

                if(interactable is FVRPhysicalObject physObject)
                {
                    physObject.RootRigidbody.useGravity = false;
                    physObject.RootRigidbody.velocity = Vector3.zero;
                }

                interactable.transform.position = Path.points[0].transform.position;
            }

            GameObject animatedPoint = new GameObject("AnimatedPoint");
            animatedPoint.transform.position = Path.points[0].transform.position;
            AnimatedPoint point = animatedPoint.AddComponent<AnimatedPoint>();
            point.path = Path;
            point.interactable = interactable;
        }


        [HarmonyPatch(typeof(SpectatorPanel), "Update")] // Specify target method with HarmonyPatch attribute
        [HarmonyPrefix]
        public static bool DrawSphere(SpectatorPanel __instance)
        {
            Popcron.Gizmos.Sphere(DisplayPoint.position, .01f, Color.blue);

            Path.DrawPath();

            return true;
        }


    }
}
