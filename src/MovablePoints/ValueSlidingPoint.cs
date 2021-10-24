using FistVR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace H3VRAnimator
{
    public class ValueSlidingPoint : RangedSlidingPoint
    {

        public float value = 0;
        public float maxValue = 1;
        public float minValue = -1;
        public float multiplier = 1;
        public Text valueText;
        public UnityAction<float> valueChangeEvent;

        public override void Awake()
        {
            base.Awake();

            valueText = buttonPoint.AddComponent<Text>();
            valueText.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
            valueText.alignment = TextAnchor.MiddleCenter;
            valueText.text = value.ToString("0.00");
        }

        public override void Update()
        {
            base.Update();

            if (activeHand != null)
            {
                UpdateValue();
                HandleEvents();
            }
        }

        public void RotateToPlayer()
        {
            if (drawGizmos)
            {
                buttonPoint.transform.rotation = Quaternion.LookRotation(buttonPoint.transform.position - GM.CurrentPlayerBody.Head.position);
            }
        }


        public void UpdateValue()
        {
            value = Mathf.Clamp(value + buttonPoint.transform.localPosition.y * multiplier, minValue, maxValue);
            valueText.text = value.ToString("0.00");
        }

        public void HandleEvents()
        {
            if(valueChangeEvent != null)
            {
                valueChangeEvent.Invoke(value);
            }
        }

        public void SetValue(float val)
        {
            value = val;
            UpdateValue();
        }

        public override void ButtonReleased()
        {
            base.ButtonReleased();

            buttonPoint.transform.position = transform.position;
        }

    }
}
