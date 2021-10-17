using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace H3VRAnimator
{
    public class ControlPoint : MovablePoint
    {
        public ControlPoint other;


        public override void Update()
        {
            //Only perform interactions if the other point is not being controlled
            if(other.activeHand == null)
            {
                base.Update();

                if (activeHand != null && other.gameObject.activeSelf)
                {
                    PositionOtherPoint();
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
            Popcron.Gizmos.Line(transform.position, transform.parent.position, pointColor);
        }


        public void PositionOtherPoint()
        {
            other.transform.localPosition = -transform.localPosition.normalized * other.transform.localPosition.magnitude;
        }
    }
}
