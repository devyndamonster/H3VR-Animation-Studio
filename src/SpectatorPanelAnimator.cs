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

        private NightVisionPostEffect postEffectPrev;
        private NightVisionPostEffect postEffectSpec;

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
            
            postEffectPrev = prevCam.GetComponent<NightVisionPostEffect>();
            postEffectSpec = specCam.GetComponent<NightVisionPostEffect>();

            if (postEffectPrev == null)
            {
                AnimLogger.Log("Cam did not have post effect scropt. Adding it!");
                postEffectPrev = prevCam.gameObject.AddComponent<NightVisionPostEffect>();
                postEffectPrev.nightVisionMat = H3VRAnimator.NightVisionMaterial;
                postEffectPrev.bloom = H3VRAnimator.BloomMaterial;
            }

            if(postEffectSpec == null)
            {
                AnimLogger.Log("Cam did not have post effect scropt. Adding it!");
                postEffectSpec = specCam.gameObject.AddComponent<NightVisionPostEffect>();
                postEffectSpec.nightVisionMat = H3VRAnimator.NightVisionMaterial;
                postEffectSpec.bloom = H3VRAnimator.BloomMaterial;
            }


            if(postEffectPrev.renderNightVision)
            {
                AnimLogger.Log("Cam had night vision enabled, disabling!");
                postEffectPrev.renderNightVision = false;
                postEffectPrev.renderBloom = false;
                postEffectSpec.renderNightVision = false;
                postEffectSpec.renderBloom = false;
                prevCam.depthTextureMode = DepthTextureMode.None;
                specCam.depthTextureMode = DepthTextureMode.None;
                RemoveShaderControls();
            }

            else
            {
                AnimLogger.Log("Cam had night vision disabled, enabling!");
                postEffectPrev.renderNightVision = true;
                postEffectPrev.renderBloom = true;
                postEffectSpec.renderNightVision = true;
                postEffectSpec.renderBloom = true;
                prevCam.depthTextureMode = DepthTextureMode.Depth;
                specCam.depthTextureMode = DepthTextureMode.Depth;
                AddShaderControls();
            }
            
        }


        private void AddShaderControls()
        {
            CreateShaderControlPoint("Light Sensetivity", 0, 2000, 5, H3VRAnimator.NightVisionMaterial.GetFloat("_LightSensetivity"), (o) =>
            {
                H3VRAnimator.NightVisionMaterial.SetFloat("_LightSensetivity", o);
            });

            CreateShaderControlPoint("Noise Scale", 0, 0.5f, 0.01f, H3VRAnimator.NightVisionMaterial.GetFloat("_NoiseScale"), (o) =>
            {
                H3VRAnimator.NightVisionMaterial.SetFloat("_NoiseScale", o);
            });

            CreateShaderControlPoint("Distance Scale", 0, 1, 0.01f, H3VRAnimator.NightVisionMaterial.GetFloat("_DistanceScale"), (o) =>
            {
                H3VRAnimator.NightVisionMaterial.SetFloat("_DistanceScale", o);
            });

            CreateShaderControlPoint("Distance Offset", 0, 1, 0.01f, H3VRAnimator.NightVisionMaterial.GetFloat("_DistanceOffset"), (o) =>
            {
                H3VRAnimator.NightVisionMaterial.SetFloat("_DistanceOffset", o);
            });

            CreateShaderControlPoint("Red Tint", 0, 1, 0.01f, H3VRAnimator.NightVisionMaterial.GetColor("_ColorTint").r, (o) =>
            {
                Color col = H3VRAnimator.NightVisionMaterial.GetColor("_ColorTint");
                col.r = o;
                H3VRAnimator.NightVisionMaterial.SetColor("_ColorTint", col);
            });

            CreateShaderControlPoint("Green Tint", 0, 1, 0.01f, H3VRAnimator.NightVisionMaterial.GetColor("_ColorTint").g, (o) =>
            {
                Color col = H3VRAnimator.NightVisionMaterial.GetColor("_ColorTint");
                col.g = o;
                H3VRAnimator.NightVisionMaterial.SetColor("_ColorTint", col);
            });

            CreateShaderControlPoint("Blue Tint", 0, 1, 0.01f, H3VRAnimator.NightVisionMaterial.GetColor("_ColorTint").b, (o) =>
            {
                Color col = H3VRAnimator.NightVisionMaterial.GetColor("_ColorTint");
                col.b = o;
                H3VRAnimator.NightVisionMaterial.SetColor("_ColorTint", col);
            });

            CreateShaderControlPoint("Bloom Iterations", 1, 5, 1, postEffectSpec.iterations, (o) =>
            {
                postEffectSpec.iterations = (int)o;
            });

            CreateShaderControlPoint("Bloom Threshold", 0, 2, 0.02f, H3VRAnimator.BloomMaterial.GetFloat("_Threshold"), (o) =>
            {
                H3VRAnimator.BloomMaterial.SetFloat("_Threshold", o);
            });
        }

        private void RemoveShaderControls()
        {
            foreach(RangedSlidingPoint point in shaderControls)
            {
                Destroy(point.gameObject);
            }
            shaderControls.Clear();
        }



        private ValueSlidingPoint CreateShaderControlPoint(string shaderField, float minVal, float maxVal, float multiplier, float startingVal, UnityAction<float> valueChangeEvent)
        {
            GameObject valObj = new GameObject(shaderField);
            valObj.transform.SetParent(transform);
            valObj.transform.localPosition = Vector3.right * 0.375f + Vector3.up * (0.2f - shaderControls.Count * 0.04f);
            valObj.transform.rotation = transform.rotation;
            valObj.transform.up = transform.right;

            ValueSlidingPoint valPoint = valObj.AddComponent<ValueSlidingPoint>();
            valPoint.buttonPoint.transform.rotation = transform.rotation;
            valPoint.pointColor = Color.yellow;
            valPoint.maxUp = 0.05f;
            valPoint.maxDown = 0.05f;
            valPoint.maxValue = maxVal;
            valPoint.minValue = minVal;
            valPoint.multiplier = multiplier;
            valPoint.SetValue(startingVal);
            valPoint.valueChangeEvent = valueChangeEvent;

            GameObject fieldCanvas = new GameObject("TextCanvas");
            fieldCanvas.transform.SetParent(valObj.transform);
            fieldCanvas.transform.position = valObj.transform.position + transform.up * 0.02f;
            fieldCanvas.transform.rotation = transform.rotation;
            Canvas canvasComp = fieldCanvas.AddComponent<Canvas>();
            RectTransform rect = canvasComp.GetComponent<RectTransform>();
            canvasComp.renderMode = RenderMode.WorldSpace;
            rect.sizeDelta = new Vector2(1, 1);

            GameObject fieldObj = new GameObject("Text");
            fieldObj.transform.SetParent(fieldCanvas.transform);
            fieldObj.transform.localPosition = Vector3.zero;
            fieldObj.transform.rotation = transform.rotation;
            fieldObj.transform.localScale = new Vector3(0.0005f, 0.0005f, 0.0005f);

            Text fieldText = fieldObj.AddComponent<Text>();
            fieldText.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
            fieldText.alignment = TextAnchor.MiddleCenter;
            fieldText.text = shaderField;
            fieldText.horizontalOverflow = HorizontalWrapMode.Overflow;

            shaderControls.Add(valPoint);

            return valPoint;
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
