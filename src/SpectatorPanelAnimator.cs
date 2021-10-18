using FistVR;
using H3VRAnimator.Logging;
using H3VRAnimator.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace H3VRAnimator
{
    public class SpectatorPanelAnimator : MonoBehaviour
    {

        public Transform displayPoint = null;
        public AnimationPath path;
        public bool drawGizmos = true;

        public SpectatorPanel original;

        public void Awake()
        {
            AnimLogger.Log("The Spectator Panel just started!");
            original = gameObject.GetComponent<SpectatorPanel>();
            path = new AnimationPath();

            CreateDisplayPoint();

            AddTopButton("Create Point", new Vector3(-800, 900, 0), () => { AddPoint(); });

            AddTopButton("Animate", new Vector3(-300, 900, 0), () => { AddAnimatedPoint(false); });

            AddTopButton("Animate Copy", new Vector3(-300, 1000, 0), () => { AddAnimatedPoint(true); });

            AddTopButton("Clear Points", new Vector3(200, 900, 0), ClearPoints);

            AddTopButton("Toggle Gizmos", new Vector3(700, 900, 0), ToggleGizmos);

            AddTopButton("Toggle Bezier", new Vector3(700, 1000, 0), path.ToggleBezier);

            AddTopButton("Toggle Continuous", new Vector3(700, 1100, 0), path.ToggleContinuous);

            AddTopButton("Toggle Line Mode", new Vector3(700, 1200, 0), path.ToggleLineMode);

            AddTopButton("Toggle Rotation", new Vector3(700, 1300, 0), path.ToggleDrawRotation);
        }


        private void AddTopButton(string text, Vector3 localPos, UnityAction buttonEvent)
        {
            //Instantiate the button object
            GameObject buttonObject = Instantiate(original.List_Categories[0].gameObject, original.List_Categories[0].transform.parent);
            buttonObject.transform.localPosition = localPos;
            buttonObject.transform.localScale = new Vector3(3, 3, 2);
            buttonObject.gameObject.GetComponent<Text>().text = text;

            //Setup the button event
            Button buttonComp = buttonObject.gameObject.GetComponent<Button>();
            buttonComp.onClick = new Button.ButtonClickedEvent();
            buttonComp.onClick.AddListener(buttonEvent);
        }


        private void CreateDisplayPoint()
        {
            GameObject drawPoint = new GameObject();
            drawPoint.transform.SetParent(original.List_Categories[0].transform.parent);
            drawPoint.transform.localPosition = new Vector3(0, 1300, 0);
            displayPoint = drawPoint.transform;
        }


        private void AddPoint()
        {
            path.AddPoint(displayPoint.transform.position);
        }


        private void AddAnimatedPoint(bool copyObject)
        {
            FVRViveHand otherHand = AnimationUtils.GetNonPointingHand();

            if(otherHand.CurrentInteractable is FVRPhysicalObject physObj)
            {
                AnimLogger.Log("Interactable was a physical object");

                //If we copy the object, we try to instantiate a new instance of the object
                if (copyObject)
                {
                    //Only copy object if it is spawnable from the object dictionary
                    //Note: we only do this to avoid copying unwanted things, like the camera. Maybe there is a more elegant solution?
                    if (physObj.ObjectWrapper != null && IM.OD.ContainsKey(physObj.ObjectWrapper.ItemID))
                    {
                        FVRPhysicalObject copy = Instantiate(physObj, path.points[0].transform.position, path.points[0].rotationPoint.transform.rotation);

                        copy.IsHeld = false;
                        copy.RootRigidbody.useGravity = false;
                        copy.RootRigidbody.velocity = Vector3.zero;

                        physObj = copy;
                    }

                    else
                    {
                        AnimLogger.Log("Interactable was not duplicatable");
                        return;
                    }
                }

                //If we are not meant to copy the object, then we just take the object from the players hand
                else
                {
                    physObj.IsHeld = false;
                    otherHand.Input.IsGrabbing = false;

                    physObj.EndInteraction(otherHand);
                    otherHand.CurrentInteractable = null;

                    physObj.RootRigidbody.useGravity = false;
                    physObj.RootRigidbody.velocity = Vector3.zero;

                    physObj.transform.position = path.points[0].transform.position;
                    physObj.transform.rotation = path.points[0].rotationPoint.transform.rotation;
                }
            }

            else
            {
                AnimLogger.Log("Interactable was null or not physical");
                return;
            }


            GameObject animatedPoint = new GameObject("AnimatedPoint");
            animatedPoint.transform.position = path.points[0].transform.position;
            AnimatedPoint point = animatedPoint.AddComponent<AnimatedPoint>();
            point.path = path;
            point.interactable = physObj;
            point.drawGizmos = drawGizmos;

            path.animations.Add(point);
        }


        private void ClearPoints()
        {
            path.DestroyPath();
        }


        private void ToggleGizmos()
        {
            drawGizmos = !drawGizmos;
            path.SetGizmosEnabled(drawGizmos);
        }


        public void Update()
        {
            Popcron.Gizmos.Sphere(displayPoint.position, .01f, Color.blue);

            path.Animate();
            path.DrawPath();
        }


    }
}
