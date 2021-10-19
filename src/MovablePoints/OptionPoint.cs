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
    public class OptionPoint : ClickablePoint
    {
        public UnityAction clickEvent;

        public Text optionText;

        public override void Awake()
        {
            drawInteractionSphere = false;
            base.Awake();

            optionText = buttonPoint.AddComponent<Text>();
            optionText.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
            optionText.alignment = TextAnchor.MiddleCenter;
            optionText.horizontalOverflow = HorizontalWrapMode.Overflow;
            optionText.verticalOverflow = VerticalWrapMode.Overflow;
            optionText.fontSize = 20;
            optionText.text = "Example Text";
        }


        public override void Update()
        {
            base.Update();

            if (drawGizmos)
            {
                transform.rotation = Quaternion.LookRotation(transform.position - GM.CurrentPlayerBody.Head.position);
            }
        }


        public override void ButtonPressed()
        {
            base.ButtonPressed();

            clickEvent.Invoke();
        }

    }
}
