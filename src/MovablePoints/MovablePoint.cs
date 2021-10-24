using FistVR;
using H3VRAnimator.Logging;
using H3VRAnimator.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace H3VRAnimator
{
    public class MovablePoint : ClickablePoint
    {
        public bool lockPosition = false;
        public bool lockRotation = false;


        public override void Update()
        {
            CheckForMove();

            base.Update();
        }


        public virtual void CheckForMove()
        {
            if (activeHand != null)
            {
                if (!lockPosition)
                {
                    transform.position = activeHand.transform.position + activeHand.PointingTransform.forward * savedDist;
                }

                if (!lockRotation)
                {
                    transform.rotation = activeHand.PointingTransform.rotation;
                }
            }
        }


    }
}
