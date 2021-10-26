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
        public Quaternion prevRotation = Quaternion.identity;

        //Used to track movement direction when sliding along path
        public float prevPosition = 0;
        public bool changedPathSegment;

        public List<EventPoint> activeEvents = new List<EventPoint>();

        public FVRViveHand fakeHand;

        public override void Awake()
        {
            base.Awake();

            CreateFakeHand();
        }

        public override void Update()
        {
            if (from == null || to == null) Destroy(gameObject);

            CheckForRelease();
            CheckForMove();

            //If this point is currently held, we perform the slidable point stuff
            if (activeHand != null)
            {
                UpdatePosition();
                Popcron.Gizmos.Sphere(transform.position, .005f, Color.grey);
            }

            if (IsHeldByFakeHand())
            {
                interactable.UpdateInteraction(fakeHand);
            }

            Animate();
            UpdateHandMovement();
            HandleEvents();
            DrawGizmos();

            prevVector = transform.position;
            prevRotation = transform.rotation;
            prevPosition = position;
        }

        private void UpdateHandMovement()
        {
            Vector3 velocity = (transform.position - prevVector) / Time.deltaTime;

            //This creates fluctuation when moving over max or min
            //Vector3 angularVelocity = (Quaternion.Inverse(prevRotation) * transform.rotation).eulerAngles / Time.deltaTime;

            //I found this bit of code from here: https://forum.unity.com/threads/manually-calculate-angular-velocity-of-gameobject.289462/
            var deltaRot = transform.rotation * Quaternion.Inverse(prevRotation);
            var eulerRot = new Vector3(Mathf.DeltaAngle(0, deltaRot.eulerAngles.x), Mathf.DeltaAngle(0, deltaRot.eulerAngles.y), Mathf.DeltaAngle(0, deltaRot.eulerAngles.z));

            Vector3 angularVelocity = eulerRot;

            //AnimLogger.Log("Velocity: " + velocity.ToString("F3") + ", Angular Velocity: " + angularVelocity.ToString("F3"));

            fakeHand.Input.VelLinearLocal = velocity;
            fakeHand.Input.VelLinearWorld = velocity;

            fakeHand.Input.VelAngularLocal = angularVelocity;
            fakeHand.Input.VelAngularWorld = angularVelocity;
        }



        private void CreateFakeHand()
        {
            fakeHand = gameObject.AddComponent<FVRViveHand>();
            fakeHand.m_initState = FVRViveHand.HandInitializationState.Uninitialized;
            fakeHand.IsInDemoMode = true;
            fakeHand.Input = new HandInput();
            fakeHand.Buzzer = gameObject.AddComponent<FVRHaptics>();
            fakeHand.OtherHand = fakeHand;
        }


        public void SetInteractable(FVRPhysicalObject physObj)
        {
            interactable = physObj;
            interactable.m_hand = fakeHand;
            interactable.IsHeld = true;
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
                position += Mathf.Lerp(from.speedPoint.value, to.speedPoint.value, position) * Time.deltaTime / path.GetDistanceBetweenPoints(from, to);
            }
            

            if (!IsHeldByFakeHand() || interactable.transform.parent != null)
            {
                Destroy(gameObject);
                return;
            }

            interactable.RootRigidbody.velocity = Vector3.zero;
            interactable.transform.position = transform.position - offset;
            interactable.transform.rotation = transform.rotation;
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

        public bool IsHeldByFakeHand()
        {
            return interactable != null && interactable.m_hand.Equals(fakeHand);
        }

        private void OnDestroy()
        {
            if (IsHeldByFakeHand())
            {
                interactable.IsHeld = false;
                interactable.m_hand = null;
            }

            foreach(EventPoint point in activeEvents)
            {
                point.trackedAnimations.Remove(this);
            }

            path.animations.Remove(this);
        }

    }
}
