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

            transform.position = Vector3.Lerp(path.points[moveToIndex - 1].transform.position, path.points[moveToIndex].transform.position, progress);
            transform.rotation = Quaternion.Slerp(path.points[moveToIndex - 1].rotation.transform.rotation, path.points[moveToIndex].rotation.transform.rotation, progress);

            progress += Mathf.Lerp(path.points[moveToIndex - 1].speed.speed, path.points[moveToIndex].speed.speed, progress) * Time.deltaTime;

            Popcron.Gizmos.Sphere(transform.position, .01f, Color.green);


            if(interactable != null)
            {
                if (interactable.IsHeld)
                {
                    AnimLogger.Log("Object Was Held!");
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

    }
}
