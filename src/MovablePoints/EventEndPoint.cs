using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace H3VRAnimator
{
    public class EventEndPoint : PathSlidingPoint
    {

        public EventPoint other;

        protected override void ShiftEndpointsForwards(PathAnchor next)
        {
            from.eventEndList.Remove(this);
            base.ShiftEndpointsForwards(next);
            from.eventEndList.Add(this);
        }


        protected override void ShiftEndpointsBackwards(PathAnchor prev)
        {
            from.eventEndList.Remove(this);
            base.ShiftEndpointsBackwards(prev);
            from.eventEndList.Add(this);
        }

    }
}
