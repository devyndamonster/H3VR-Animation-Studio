﻿using FistVR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace H3VRAnimator
{
    public class MovablePoint : MonoBehaviour
    {
        public bool lockPostion;
        public Color pointColor = Color.red;
        public float radius = .01f;

        public GameObject buttonPoint;

        protected FVRViveHand activeHand = null;
        protected float savedDist;
        
        public virtual void Awake()
        {
            gameObject.layer = LayerMask.NameToLayer("Interactable");

            Canvas canvasComp = gameObject.AddComponent<Canvas>();
            RectTransform rect = canvasComp.GetComponent<RectTransform>();
            canvasComp.renderMode = RenderMode.WorldSpace;
            rect.sizeDelta = new Vector2(1, 1);

            buttonPoint = new GameObject();
            buttonPoint.transform.SetParent(transform);
            buttonPoint.transform.localPosition = Vector3.zero;
            buttonPoint.transform.rotation = transform.rotation;
            buttonPoint.transform.localScale = new Vector3(0.0005f, 0.0005f, 0.0005f);

            BoxCollider collider = buttonPoint.AddComponent<BoxCollider>();
            collider.size = new Vector3(50, 50, 50);
            collider.isTrigger = true;

            Button button = buttonPoint.AddComponent<Button>();
            button.onClick.AddListener(ButtonPressed);

            FVRPointableButton fvrButton = buttonPoint.AddComponent<FVRPointableButton>();
            fvrButton.useGUILayout = true;
            fvrButton.ColorSelected = Color.blue;
            fvrButton.ColorUnselected = Color.cyan;
            fvrButton.MaxPointingRange = 2;
            fvrButton.Button = button;
        }

        public virtual void ButtonPressed()
        {
            AnimLogger.Log("Button pressed!");


            //If the hand is already set, then we unfollow the hand
            if(activeHand != null)
            {
                AnimLogger.Log("Hand was active");
                return;
            }


            //If the hand is not set, we start following it
            AnimLogger.Log("Hand was not active");
            float distLeft = Vector3.Distance(GM.CurrentPlayerBody.LeftHand.position, transform.position);
            float distRight = Vector3.Distance(GM.CurrentPlayerBody.RightHand.position, transform.position);

            if(distLeft < distRight)
            {
                activeHand = GM.CurrentPlayerBody.LeftHand.GetComponent<FVRViveHand>();
            }
            else
            {
                activeHand = GM.CurrentPlayerBody.RightHand.GetComponent<FVRViveHand>();
            }

            savedDist = Vector3.Distance(activeHand.transform.position, transform.position);
        }


        public virtual void ButtonReleased()
        {
            AnimLogger.Log("Trigger Released!");
            activeHand = null;
        }


        public virtual void Update()
        {
            if(activeHand != null)
            {
                if (activeHand.Input.TriggerFloat < 0.2f)
                {
                    ButtonReleased();
                    return;
                }

                if (!lockPostion)
                {
                    transform.position = activeHand.transform.position + activeHand.PointingTransform.forward * savedDist;
                }

                transform.rotation = activeHand.PointingTransform.rotation;
            }

            DrawPoint();
        }

        public void DrawPoint()
        {
            Popcron.Gizmos.Sphere(transform.position, radius, pointColor);
        }

    }
}
