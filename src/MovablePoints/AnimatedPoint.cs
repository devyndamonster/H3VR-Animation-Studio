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
        public float prevPosition = 0;
        public bool changedPathSegment;

        public FVRViveHand fakeHand;


        public override void Awake()
        {
            base.Awake();

            CreateFakeHand();
        }

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


        private void CreateFakeHand()
        {
            fakeHand = gameObject.AddComponent<FVRViveHand>();
            fakeHand.m_initState = FVRViveHand.HandInitializationState.Uninitialized;
            fakeHand.Input = new HandInput();
            fakeHand.Buzzer = gameObject.AddComponent<FVRHaptics>();
            fakeHand.OtherHand = fakeHand;
        }


        protected void Animate()
        {
            changedPathSegment = false;

            //Check for next curve segment if this is not held and the position is far enough
            if (activeHand == null && position >= 1)
            {
                PathAnchor next = path.GetNextPoint(to);
                position = 0;

                if (!to.Equals(next))
                {
                    ShiftEndpointsForwards(next);
                }

                else
                {
                    from = path.points[0];
                    to = path.points[1];
                    changedPathSegment = true;
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
            if (changedPathSegment)
            {
                AnimLogger.Log("Skipping because from changed");
                return;
            }

            foreach(EventPoint point in from.eventsList)
            {
                //AnimLogger.Log("Looping through event, count: " + from.eventsList.Count);

                if (prevPosition < point.position && position > point.position)
                {
                    point.HandleEventForward(this);
                }

                else if(prevPosition > point.position && position < point.position)
                {
                    point.HandleEventBackward(this);
                }
            }

            foreach(EventEndPoint endpoint in from.eventEndList)
            {
                //AnimLogger.Log("Looping through endpoints, count: " + from.eventEndList.Count);

                if (prevPosition < endpoint.position && position > endpoint.position)
                {
                    endpoint.other.HandleEndpointEventForward(this);
                }

                else if (prevPosition > endpoint.position && position < endpoint.position)
                {
                    endpoint.other.HandleEndpointEventBackward(this);
                }
            }
        }

        protected override void ShiftEndpointsForwards(PathAnchor next)
        {
            base.ShiftEndpointsForwards(next);
            changedPathSegment = true;
        }

        protected override void ShiftEndpointsBackwards(PathAnchor prev)
        {
            base.ShiftEndpointsBackwards(prev);
            changedPathSegment = true;
        }

        private void OnDestroy()
        {
            if (interactable != null)
            {
                interactable.gameObject.name = interactable.gameObject.name.Replace("Animated_", "");
            }

            path.animations.Remove(this);
        }

    }
}
