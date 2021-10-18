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
        public List<AnimationPath> paths = new List<AnimationPath>();
        public List<GameObject> pathButtons = new List<GameObject>();
        public int selectedPath = 0;
        public SpectatorPanel original;

        private Vector3 pathButtonPos = new Vector3(-1500, 700, 0);

        public void Awake()
        {
            AnimLogger.Log("The Spectator Panel just started!");
            original = gameObject.GetComponent<SpectatorPanel>();

            CreateDisplayPoint();

            AddTopButton("Create Point", new Vector3(-800, 900, 0), () => { AddPoint(); });

            AddTopButton("Animate", new Vector3(-300, 900, 0), () => { AddAnimatedPoint(false); });

            AddTopButton("Animate Copy", new Vector3(-300, 1000, 0), () => { AddAnimatedPoint(true); });

            AddTopButton("Toggle Pause", new Vector3(-300, 1100, 0), TogglePause);

            AddTopButton("Clear Points", new Vector3(200, 900, 0), ClearPoints);

            AddTopButton("Toggle Gizmos", new Vector3(700, 900, 0), ToggleGizmos);

            AddTopButton("Toggle Bezier", new Vector3(700, 1000, 0), ToggleBezier);

            AddTopButton("Toggle Continuous", new Vector3(700, 1100, 0), ToggleContinuous);

            AddTopButton("Toggle Line Mode", new Vector3(700, 1200, 0), ToggleLineMode);

            AddTopButton("Toggle Rotation", new Vector3(700, 1300, 0), ToggleDrawRotation);

            AddTopButton("Add Path", new Vector3(-1500, 850, 0), AddPathButton);

            AddPathButton();
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


        private void AddPathButton()
        {
            //Instantiate the button object
            GameObject buttonObject = Instantiate(original.List_Categories[0].gameObject, original.List_Categories[0].transform.parent);
            buttonObject.transform.localPosition = pathButtonPos;
            pathButtonPos += Vector3.down * 100;
            buttonObject.transform.localScale = new Vector3(3, 3, 2);

            //Setup the button event
            Button buttonComp = buttonObject.gameObject.GetComponent<Button>();
            buttonComp.onClick = new Button.ButtonClickedEvent();
            int pathIndex = paths.Count;
            buttonComp.onClick.AddListener(() => { SelectPath(pathIndex); });

            //Track the new path
            pathButtons.Add(buttonObject);
            paths.Add(new AnimationPath());

            Text textComp = buttonObject.gameObject.GetComponent<Text>();
            textComp.text = "Path " + paths.Count;
            if (paths.Count == 1) textComp.color = Color.white;
            else textComp.color = Color.grey;
        }



        private void SelectPath(int i)
        {
            pathButtons[selectedPath].GetComponent<Text>().color = Color.grey;
            pathButtons[i].GetComponent<Text>().color = Color.white;

            selectedPath = i;
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
            paths[selectedPath].AddPoint(displayPoint.transform.position);
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
                    FVRPhysicalObject copy = Instantiate(physObj, paths[selectedPath].points[0].transform.position, paths[selectedPath].points[0].rotationPoint.transform.rotation);

                    copy.IsHeld = false;
                    copy.RootRigidbody.useGravity = false;
                    copy.RootRigidbody.velocity = Vector3.zero;

                    physObj = copy;
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
                }
            }

            else
            {
                AnimLogger.Log("Interactable was null or not physical");
                return;
            }


            AnimLogger.Log("Physical object selected!");

            paths[selectedPath].AddAnimatedPoint(physObj);
        }


        private void ClearPoints()
        {
            paths[selectedPath].DestroyPath();
        }


        private void ToggleGizmos()
        {
            paths[selectedPath].ToggleGizmos();
        }

        private void ToggleBezier()
        {
            paths[selectedPath].ToggleBezier();
        }

        private void ToggleContinuous()
        {
            paths[selectedPath].ToggleContinuous();
        }

        private void ToggleLineMode()
        {
            paths[selectedPath].ToggleLineMode();
        }

        private void ToggleDrawRotation()
        {
            paths[selectedPath].ToggleDrawRotation();
        }

        private void TogglePause()
        {
            paths[selectedPath].TogglePause();
        }


        public void Update()
        {
            Popcron.Gizmos.Sphere(displayPoint.position, .01f, Color.blue);

            foreach(AnimationPath path in paths)
            {
                path.Animate();
                path.DrawPath();
            }
        }


    }
}
