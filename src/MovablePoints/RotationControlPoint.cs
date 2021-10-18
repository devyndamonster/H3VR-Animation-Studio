using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace H3VRAnimator
{
    public class RotationControlPoint : MovablePoint
    {
        public RotationControlPoint other;
        public Transform referenceTransform;


        public override void Awake()
        {
            lockPostion = true;
            base.Awake();
        }

        public override void Update()
        {
            //Only perform interactions if the other point is not being controlled
            if (other.activeHand == null)
            {
                base.Update();

                if (activeHand != null && other.gameObject.activeSelf)
                {
                    RotateOtherPoint();
                }
            }
            else
            {
                DrawGizmos();
            }
        }


        public override void DrawGizmos()
        {
            if (!drawGizmos) return;

            base.DrawGizmos();
            Popcron.Gizmos.Line(transform.position, transform.position + transform.forward * 0.02f, pointColor);
        }


        public void RotateOtherPoint()
        {
            //Quaternion diff = referenceTransform.rotation * Quaternion.Inverse(transform.rotation);
            //other.transform.rotation = diff * referenceTransform.rotation;

            other.transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, referenceTransform.rotation, 2);
        }

    }
}
