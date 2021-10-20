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

        public override void Update()
        {
            CheckForRelease();
            CheckForMove();

            //If this point is currently held, we perform the slidable point stuff
            if (activeHand != null)
            {
                UpdatePosition();
                Popcron.Gizmos.Sphere(transform.position, .005f, Color.grey);
            }

            Animate();
            
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
            

            if(interactable != null)
            {
                if (interactable.IsHeld || interactable.transform.parent != null)
                {
                    AnimLogger.Log("Object Is Held!");
                    interactable = null;
                    path.animations.Remove(this);
                    Destroy(gameObject);
                    return;
                }

                if (interactable is FVRPhysicalObject physInteractable)
                {
                    physInteractable.RootRigidbody.velocity = Vector3.zero;
                    physInteractable.transform.position = transform.position - offset;
                    physInteractable.transform.rotation = transform.rotation;
                }
                else
                {
                    interactable.transform.position = transform.position - offset;
                    interactable.transform.rotation = transform.rotation;
                }
            }
        }
    }
}
