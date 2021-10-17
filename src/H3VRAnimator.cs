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

        public static List<AnimatedPoint> Animations;

        public static bool DrawGizmos = true;

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
            Animations = new List<AnimatedPoint>();

            GameObject createButton = Instantiate(__instance.List_Categories[0].gameObject, __instance.List_Categories[0].transform.parent);
            createButton.transform.localPosition = new Vector3(-800, 900, 0);
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



            GameObject clearButton = Instantiate(__instance.List_Categories[0].gameObject, __instance.List_Categories[0].transform.parent);
            clearButton.transform.localPosition = new Vector3(200, 900, 0);
            clearButton.transform.localScale = new Vector3(3, 3, 2);
            clearButton.gameObject.GetComponent<Text>().text = "Clear Points";

            Button clearComp = clearButton.gameObject.GetComponent<Button>();
            clearComp.onClick = new Button.ButtonClickedEvent();

            clearComp.onClick.AddListener(ClearPoints);



            GameObject hideButton = Instantiate(__instance.List_Categories[0].gameObject, __instance.List_Categories[0].transform.parent);
            hideButton.transform.localPosition = new Vector3(700, 900, 0);
            hideButton.transform.localScale = new Vector3(3, 3, 2);
            hideButton.gameObject.GetComponent<Text>().text = "Hide Gizmos";

            Button hideComp = hideButton.gameObject.GetComponent<Button>();
            hideComp.onClick = new Button.ButtonClickedEvent();

            hideComp.onClick.AddListener(ToggleGizmos);




            GameObject drawPoint = new GameObject();
            drawPoint.transform.SetParent(createButton.transform.parent);
            drawPoint.transform.localPosition = new Vector3(-800, 1000, 0);
            DisplayPoint = drawPoint.transform;

            return true;
        }


        private static void AddPoint(GameObject buttonObject)
        {
            Path.AddPoint(buttonObject.transform.position);
        }

        private static void AddAnimatedPoint()
        {
            FVRViveHand otherHand = GetNonPointingHand();

            FVRInteractiveObject interactable = otherHand.CurrentInteractable;

            if(interactable == null)
            {
                AnimLogger.Log("Interactable was null");
                return;
            }

            else
            {
                AnimLogger.Log("Interactable was not null");
                AnimLogger.Log("Hand: " + otherHand);

                interactable.IsHeld = false;
                otherHand.Input.IsGrabbing = false;

                interactable.EndInteraction(otherHand);
                otherHand.CurrentInteractable = null;

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
            point.drawGizmos = DrawGizmos;

            Animations.Add(point);
        }



        public static FVRViveHand GetNonPointingHand()
        {
            FVRViveHand leftHand = GM.CurrentPlayerBody.LeftHand.GetComponent<FVRViveHand>();
            FVRViveHand rightHand = GM.CurrentPlayerBody.RightHand.GetComponent<FVRViveHand>();

            if (leftHand.PointingLaser.gameObject.activeSelf) return rightHand;

            return leftHand;
        }

        public static FVRViveHand GetPointingHand()
        {
            FVRViveHand leftHand = GM.CurrentPlayerBody.LeftHand.GetComponent<FVRViveHand>();
            FVRViveHand rightHand = GM.CurrentPlayerBody.RightHand.GetComponent<FVRViveHand>();

            if (leftHand.PointingLaser.gameObject.activeSelf) return leftHand;

            return rightHand;
        }


        private static void ClearPoints()
        {
            Path.DestroyPath();

            foreach(AnimatedPoint point in Animations)
            {
                point.DestroyAnimation();
            }

            Animations.Clear();
        }


        private static void ToggleGizmos()
        {
            DrawGizmos = !DrawGizmos;
            Path.SetGizmosEnabled(DrawGizmos);

            foreach (AnimatedPoint point in Animations)
            {
                point.drawGizmos = DrawGizmos;
            }
        }


        [HarmonyPatch(typeof(SpectatorPanel), "Update")] // Specify target method with HarmonyPatch attribute
        [HarmonyPrefix]
        public static bool DrawSphere(SpectatorPanel __instance)
        {
            if (!DrawGizmos) return true;

            Popcron.Gizmos.Sphere(DisplayPoint.position, .01f, Color.blue);

            Path.DrawPath();

            return true;
        }


    }
}
