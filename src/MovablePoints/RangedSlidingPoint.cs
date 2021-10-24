using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace H3VRAnimator
{
    public class RangedSlidingPoint : MovablePoint
    {
        public float maxUp = 1;
        public float maxDown = 1;
        public float position = 0;

        public override void Awake()
        {
            lockRotation = true;
            base.Awake();
        }

        public override void DrawGizmos()
        {
            if (!drawGizmos) return;

            Popcron.Gizmos.Sphere(buttonPoint.transform.position, radius, pointColor);
            Popcron.Gizmos.Line(transform.position + transform.up * -maxDown, transform.position + transform.up * maxUp, Color.white);
        }

        public override void CheckForMove()
        {
            if (activeHand != null)
            {
                if (!lockPosition)
                {
                    buttonPoint.transform.position = Vector3.Project(activeHand.transform.position + activeHand.PointingTransform.forward * savedDist, transform.up);
                    
                    //Can't edit a transforms vector directly, so this is what you gotta do
                    Vector3 localPos = buttonPoint.transform.localPosition;
                    localPos.x = 0;
                    localPos.z = 0;
                    localPos.y = Mathf.Clamp(localPos.y, -maxDown, maxUp);

                    buttonPoint.transform.localPosition = localPos;
                }

                if (!lockRotation)
                {
                    buttonPoint.transform.rotation = activeHand.PointingTransform.rotation;
                }
            }
        }

    }
}
