using FistVR;
using H3VRAnimator.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace H3VRAnimator
{
    public class AnimatedPoint : MonoBehaviour
    {
        public AnimationPath path;
        public FVRPhysicalObject interactable;
        public bool drawGizmos = true;
        public bool isPaused = false;

        private int moveToIndex = 1;
        private float progress = 0;

        public void Animate()
        {
            
            //If we have reached the end of the curve segment, decide where to move next
            if (progress >= 1)
            {
                progress = 0;
                moveToIndex += 1;
                if (moveToIndex >= path.points.Count)
                {
                    if (path.isContinuous)
                    {
                        moveToIndex = 0;
                    }

                    else
                    {
                        moveToIndex = 1;
                        transform.position = path.points[0].transform.position;
                    }
                }
            }

            //Set to and from based on our destination
            PathAnchor from;
            PathAnchor to;

            if (moveToIndex == 0)
            {
                from = path.points[path.points.Count - 1];
                to = path.points[moveToIndex];
            }
            else
            {
                from = path.points[moveToIndex - 1];
                to = path.points[moveToIndex];
            }

            //If we just went through a jump point, immediately move to next point
            if (from.isJumpPoint) progress = 1;
            

            transform.position = path.GetLerpPosition(from, to, progress);
            transform.rotation = path.GetLerpRotation(from, to, progress);

            //Only progress this animation if not paused
            if (!isPaused)
            {
                progress += Mathf.Lerp(from.speedPoint.speed, to.speedPoint.speed, progress) * Time.deltaTime / path.GetDistanceBetweenPoints(from, to);
            }
            
            DrawPoint();

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
                    //physInteractable.SetFakeHand(Vector3.zero, transform.position);
                    physInteractable.RootRigidbody.velocity = Vector3.zero;
                    physInteractable.transform.position = transform.position;
                    physInteractable.transform.rotation = transform.rotation;
                }
                else
                {
                    interactable.transform.position = transform.position;
                    interactable.transform.rotation = transform.rotation;
                }
            }
        }

        public void DrawPoint()
        {
            if (!drawGizmos) return;
            Popcron.Gizmos.Sphere(transform.position, .01f, Color.green);
        }


    }
}
