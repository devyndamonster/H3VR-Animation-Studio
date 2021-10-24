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

        public List<ValueSlidingPoint> shaderControls = new List<ValueSlidingPoint>();

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

            AddTopButton("Toggle Night Vision", new Vector3(700, 1400, 0), ToggleNightVision);

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
            string pathName = "Path " + paths.Count;
            AnimationPath path = new AnimationPath();
            path.pathName = pathName;
            paths.Add(path);

            Text textComp = buttonObject.gameObject.GetComponent<Text>();
            textComp.text = pathName;
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

        private void ToggleNightVision()
        {
            
            AnimLogger.Log("Adding night vision to cam!");

            Camera prevCam = GM.CurrentSceneSettings.m_previewCam;
            Camera specCam = GM.CurrentSceneSettings.m_specCam;
            
            CameraEffectScript postEffectPrev = prevCam.GetComponent<CameraEffectScript>();
            CameraEffectScript postEffectSpec = specCam.GetComponent<CameraEffectScript>();

            if (postEffectPrev == null)
            {
                AnimLogger.Log("Cam did not have post effect scropt. Adding it!");
                postEffectPrev = prevCam.gameObject.AddComponent<CameraEffectScript>();
            }

            if(postEffectSpec == null)
            {
                AnimLogger.Log("Cam did not have post effect scropt. Adding it!");
                postEffectSpec = specCam.gameObject.AddComponent<CameraEffectScript>();
            }


            if(postEffectPrev.mat != null)
            {
                AnimLogger.Log("Cam had night vision enabled, disabling!");
                postEffectPrev.mat = null;
                postEffectSpec.mat = null;
                RemoveShaderControls();
                
            }

            else
            {
                AnimLogger.Log("Cam had night vision disabled, enabling!");
                postEffectPrev.mat = H3VRAnimator.NightVisionMaterial;
                postEffectSpec.mat = H3VRAnimator.NightVisionMaterial;
                AddShaderControls();
            }
            
        }


        private void AddShaderControls()
        {
            GameObject intensity = new GameObject("IntensityPoint");
            intensity.transform.SetParent(transform);
            intensity.transform.localPosition = Vector3.right * 0.33f;
            intensity.transform.rotation = transform.rotation;
            ValueSlidingPoint intensityPoint = intensity.AddComponent<ValueSlidingPoint>();
            intensityPoint.pointColor = Color.yellow;
            intensityPoint.maxUp = 0.05f;
            intensityPoint.maxDown = 0.05f;
            intensityPoint.maxValue = 2000;
            intensityPoint.minValue = 0;
            intensityPoint.multiplier = 5;
            intensityPoint.value = H3VRAnimator.NightVisionMaterial.GetFloat("_LightSensetivity");
            intensityPoint.valueChangeEvent = (o) =>
            {
                H3VRAnimator.NightVisionMaterial.SetFloat("_LightSensetivity", o);
            };
            shaderControls.Add(intensityPoint);

            GameObject noise = new GameObject("NoisePoint");
            noise.transform.SetParent(transform);
            noise.transform.localPosition = Vector3.right * 0.35f;
            noise.transform.rotation = transform.rotation;
            ValueSlidingPoint noisePoint = noise.AddComponent<ValueSlidingPoint>();
            noisePoint.pointColor = Color.yellow;
            noisePoint.maxUp = 0.05f;
            noisePoint.maxDown = 0.05f;
            noisePoint.maxValue = 0.01f;
            noisePoint.minValue = 0;
            noisePoint.multiplier = 0.0001f;
            noisePoint.value = H3VRAnimator.NightVisionMaterial.GetFloat("_NoiseScale");
            noisePoint.valueChangeEvent = (o) =>
            {
                H3VRAnimator.NightVisionMaterial.SetFloat("_NoiseScale", o);
            };
            shaderControls.Add(noisePoint);

            GameObject r = new GameObject("RedPoint");
            r.transform.SetParent(transform);
            r.transform.localPosition = Vector3.right * 0.37f;
            r.transform.rotation = transform.rotation;
            ValueSlidingPoint rPoint = r.AddComponent<ValueSlidingPoint>();
            rPoint.pointColor = Color.yellow;
            rPoint.maxUp = 0.05f;
            rPoint.maxDown = 0.05f;
            rPoint.maxValue = 1;
            rPoint.minValue = 0;
            rPoint.multiplier = 0.01f;
            rPoint.value = H3VRAnimator.NightVisionMaterial.GetColor("_ColorTint").r;
            rPoint.valueChangeEvent = (o) =>
            {
                Color col = H3VRAnimator.NightVisionMaterial.GetColor("_ColorTint");
                col.r = o;
                H3VRAnimator.NightVisionMaterial.SetColor("_ColorTint", col);
            };
            shaderControls.Add(rPoint);

            GameObject g = new GameObject("GreenPoint");
            g.transform.SetParent(transform);
            g.transform.localPosition = Vector3.right * 0.39f;
            g.transform.rotation = transform.rotation;
            ValueSlidingPoint gPoint = g.AddComponent<ValueSlidingPoint>();
            gPoint.pointColor = Color.yellow;
            gPoint.maxUp = 0.05f;
            gPoint.maxDown = 0.05f;
            gPoint.maxValue = 1;
            gPoint.minValue = 0;
            gPoint.multiplier = 0.01f;
            gPoint.value = H3VRAnimator.NightVisionMaterial.GetColor("_ColorTint").g;
            gPoint.valueChangeEvent = (o) =>
            {
                Color col = H3VRAnimator.NightVisionMaterial.GetColor("_ColorTint");
                col.g = o;
                H3VRAnimator.NightVisionMaterial.SetColor("_ColorTint", col);
            };
            shaderControls.Add(gPoint);

            GameObject b = new GameObject("BluePoint");
            b.transform.SetParent(transform);
            b.transform.localPosition = Vector3.right * 0.41f;
            b.transform.rotation = transform.rotation;
            ValueSlidingPoint bPoint = b.AddComponent<ValueSlidingPoint>();
            bPoint.pointColor = Color.yellow;
            bPoint.maxUp = 0.05f;
            bPoint.maxDown = 0.05f;
            bPoint.maxValue = 1;
            bPoint.minValue = 0;
            bPoint.multiplier = 0.01f;
            bPoint.value = H3VRAnimator.NightVisionMaterial.GetColor("_ColorTint").b;
            bPoint.valueChangeEvent = (o) =>
            {
                Color col = H3VRAnimator.NightVisionMaterial.GetColor("_ColorTint");
                col.b = o;
                H3VRAnimator.NightVisionMaterial.SetColor("_ColorTint", col);
            };
            shaderControls.Add(bPoint);

        }

        private void RemoveShaderControls()
        {
            foreach(RangedSlidingPoint point in shaderControls)
            {
                Destroy(point.gameObject);
            }
            shaderControls.Clear();
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
                path.DrawPath();
            }
        }


    }
}
