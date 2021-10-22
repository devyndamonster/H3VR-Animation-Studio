using FistVR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace H3VRAnimator
{
    public static class AnimationEvents
    {

        public static void MoveToPath(AnimatedPoint eventTarget, AnimationPath path)
        {
            FVRPhysicalObject physObj = eventTarget.interactable;
            eventTarget.interactable = null;
            path.AddAnimatedPoint(physObj);
        }


        public static void DuplicateToPath(AnimatedPoint eventTarget, AnimationPath path)
        {
            FVRPhysicalObject physObj = GameObject.Instantiate(eventTarget.interactable, path.points[0].transform.position, path.points[0].rotationPoint.transform.rotation);
            path.AddAnimatedPoint(physObj);
        }

    }
}
