using FistVR;
using H3VRAnimator.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace H3VRAnimator
{
    public class AnimatedPoint : PathSlidingPoint
    {
        public FVRPhysicalObject interactable;
        public bool isPaused = false;

        public Vector3 prevVector = Vector3.zero;
        public Vector3 currVector = Vector3.zero;

        //Used to track movement direction when sliding along path
        protected float prevPosition = 0;


        public override void Update()
        {
            prevPosition = position;
            
            CheckForRelease();
            CheckForMove();

            //If this point is currently held, we perform the slidable point stuff
            if (activeHand != null)
            {
                UpdatePosition();
                Popcron.Gizmos.Sphere(transform.position, .005f, Color.grey);
            }

            Animate();
            HandleEvents();
            DrawGizmos();
        }


        protected void Animate()
        {
            
            //Check for next curve segment if this is not held and the position is far enough
            if (activeHand == null && position >= 1)
            {
                PathAnchor next = path.GetNextPoint(to);
                position = 0;

                if (!to.Equals(next))
                {
                    from = to;
                    to = next;
                }

                else
                {
                    from = path.points[0];
                    to = path.points[1];
                }
            }


            //If we just went through a jump point, immediately move to next point
            if (from.isJumpPoint) position = 1;
            

            transform.position = path.GetLerpPosition(from, to, position) + offset;
            transform.rotation = path.GetLerpRotation(from, to, position);


            //Only progress this animation if not paused and not held
            if (!isPaused && activeHand == null)
            {
                position += Mathf.Lerp(from.speedPoint.speed, to.speedPoint.speed, position) * Time.deltaTime / path.GetDistanceBetweenPoints(from, to);
            }
            

            if (interactable == null || interactable.IsHeld || interactable.transform.parent != null)
            {
                //AnimLogger.Log("Object Is Held!");
                interactable = null;
                path.animations.Remove(this);
                Destroy(gameObject);
                return;
            }

            interactable.RootRigidbody.velocity = Vector3.zero;
            interactable.transform.position = transform.position - offset;
            interactable.transform.rotation = transform.rotation;

            prevVector = currVector;
            currVector = transform.position;
        }


        protected void HandleEvents()
        {
            foreach(EventPoint point in from.eventsList)
            {
                if(prevPosition < point.position && position > point.position)
                {
                    point.HandleEvents(this);
                }
            }
        }
    }
}
