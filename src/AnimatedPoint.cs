using FistVR;
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
        public FVRInteractiveObject interactable;
        public bool drawGizmos = true;

        private int moveToIndex = 1;
        private float progress = 0;

        public void Update()
        {
            if (progress >= 1)
            {
                progress = 0;
                moveToIndex += 1;
                if (moveToIndex >= path.points.Count)
                {
                    moveToIndex = 1;
                    transform.position = path.points[0].transform.position;
                }
            }

            PathAnchor from = path.points[moveToIndex - 1];
            PathAnchor to = path.points[moveToIndex];

            transform.position = path.CurvePosition(from, to, progress);
            transform.rotation = Quaternion.Slerp(from.rotationPoint.transform.rotation, to.rotationPoint.transform.rotation, progress);

            progress += Mathf.Lerp(from.speedPoint.speed, to.speedPoint.speed, progress) * Time.deltaTime / Vector3.Distance(from.transform.position, to.transform.position);

            DrawPoint();

            if(interactable != null)
            {
                if (interactable.IsHeld || interactable.transform.parent != null)
                {
                    AnimLogger.Log("Object Is Held!");
                    interactable = null;
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


        public void DestroyAnimation()
        {
            Destroy(gameObject);
        }

    }
}
