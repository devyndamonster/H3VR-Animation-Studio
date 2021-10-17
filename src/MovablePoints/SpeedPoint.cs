using FistVR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace H3VRAnimator
{
    public class SpeedPoint : MovablePoint
    {
        public float speed = 0.3f;

        private Text speedText;
        private float speedMultiplier = 1f;



        public override void Awake()
        {
            lockPostion = true;
            base.Awake();

            speedText = buttonPoint.AddComponent<Text>();
            speedText.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
            speedText.alignment = TextAnchor.MiddleCenter;
            speedText.text = speed.ToString("0.00");
        }


        public override void ButtonReleased()
        {
            speed = Mathf.Max(speed + GetSpeedChange(), 0);
            speedText.text = speed.ToString("0.00");

            base.ButtonReleased();
        }



        public override void Update()
        {
            base.Update();

            if(activeHand != null)
            {
                speedText.text = Mathf.Max(speed + GetSpeedChange(), 0).ToString("0.00");
            }

            transform.rotation = Quaternion.LookRotation(transform.position - GM.CurrentPlayerBody.Head.position);
        }

        public float GetSpeedChange()
        {
            if (activeHand == null) return 0;

            return (savedDist - Vector3.Distance(transform.position, activeHand.transform.position)) * speedMultiplier;
        }
    }
}
