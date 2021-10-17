using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace H3VRAnimator
{
    public class CurvePoint : MovablePoint
    {
        public CurvePoint other;


        public override void Update()
        {
            base.Update();

            if(activeHand != null && other.gameObject.activeSelf)
            {
                PositionOtherPoint();
            }

            if (!drawGizmos) return;
            Popcron.Gizmos.Line(transform.position, transform.parent.position, pointColor);
        }


        public void PositionOtherPoint()
        {
            other.transform.localPosition = -transform.localPosition.normalized * other.transform.localPosition.magnitude;
        }
    }
}
