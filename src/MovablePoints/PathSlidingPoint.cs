using H3VRAnimator.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace H3VRAnimator
{
    public class PathSlidingPoint : MovablePoint
    {

        public AnimationPath path;
        public PathAnchor from;
        public PathAnchor to;
        public float position;
        public Vector3 offset = Vector3.zero;


        public override void Update()
        {
            CheckForRelease();
            CheckForMove();

            if (activeHand != null)
            {
                UpdatePosition();
                Popcron.Gizmos.Sphere(transform.position, .005f, Color.grey);
            }
                
            transform.position = path.GetLerpPosition(from, to, position) + offset;

            DrawGizmos();
        }


        protected void UpdatePosition()
        {
            position = path.GetClosestPoint(from, to, transform.position);
            UpdateEndpoints();
        }


        protected virtual void UpdateEndpoints()
        {
            //If far from start, switch points forward
            if (position >= 1)
            {
                PathAnchor next = path.GetNextPoint(to);

                //Only switch points if the point actually changed
                if (!next.Equals(to))
                {

                    ShiftEndpointsForwards(next);
                    //AnimLogger.Log("Move Points Forward");
                }

                else
                {
                    //AnimLogger.Log("Could Not Move Forward");
                }
            }


            //If far from end, switch points backwards
            else if (position <= 0)
            {
                PathAnchor prev = path.GetPrevPoint(from);

                //Only switch points if the point actually changed
                if (!prev.Equals(from))
                {
                    ShiftEndpointsBackwards(prev);
                }

                else
                {
                    //AnimLogger.Log("Could Not Move Backward");
                }
            }
        }


        protected virtual void ShiftEndpointsForwards(PathAnchor next)
        {
            from = to;
            to = next;
            position = 0;
        }


        protected virtual void ShiftEndpointsBackwards(PathAnchor prev)
        {
            to = from;
            from = prev;
            position = 1;
        }

    }
}
