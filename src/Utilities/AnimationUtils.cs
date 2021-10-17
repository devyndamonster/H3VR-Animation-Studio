using FistVR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace H3VRAnimator.Utilities
{
    public static class AnimationUtils
    {

        public static FVRViveHand GetNonPointingHand()
        {
            FVRViveHand leftHand = GM.CurrentPlayerBody.LeftHand.GetComponent<FVRViveHand>();
            FVRViveHand rightHand = GM.CurrentPlayerBody.RightHand.GetComponent<FVRViveHand>();

            if (leftHand.PointingLaser.gameObject.activeSelf) return rightHand;

            return leftHand;
        }


        public static FVRViveHand GetPointingHand()
        {
            FVRViveHand leftHand = GM.CurrentPlayerBody.LeftHand.GetComponent<FVRViveHand>();
            FVRViveHand rightHand = GM.CurrentPlayerBody.RightHand.GetComponent<FVRViveHand>();

            if (leftHand.PointingLaser.gameObject.activeSelf) return leftHand;

            return rightHand;
        }

    }
}
