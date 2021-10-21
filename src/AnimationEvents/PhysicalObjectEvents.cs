using FistVR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace H3VRAnimator
{
    public static class PhysicalObjectEvents
    {
        public static void DropItem(AnimatedPoint eventTarget)
        {
            FVRPhysicalObject physObj = eventTarget.interactable;
            eventTarget.interactable = null;

            physObj.RootRigidbody.useGravity = true;
        }

        public static void DropItemWithVelocity(AnimatedPoint eventTarget)
        {
            FVRPhysicalObject physObj = eventTarget.interactable;
            eventTarget.interactable = null;

            physObj.RootRigidbody.useGravity = true;
            physObj.RootRigidbody.velocity = (eventTarget.currVector - eventTarget.prevVector) / Time.deltaTime;
        }

    }
}
