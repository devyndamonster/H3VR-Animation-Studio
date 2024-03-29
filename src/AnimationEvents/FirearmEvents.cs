﻿using FistVR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace H3VRAnimator
{
    public static class FirearmEvents
    {

        public static void FireGun(AnimatedPoint eventTarget)
        {
            if (eventTarget.interactable is Handgun handgun)
            {
                if(handgun.Slide.CurPos == HandgunSlide.SlidePos.Forward)
                {
                    handgun.ReleaseSeer();
                    handgun.HasTriggerReset = true;
                    handgun.m_isSeerReady = true;
                }
            }

            else if(eventTarget.interactable is ClosedBoltWeapon closedBolt)
            {
                if(closedBolt.Bolt.CurPos == ClosedBolt.BoltPos.Forward)
                {
                    closedBolt.DropHammer();
                }
            }

            else if(eventTarget.interactable is OpenBoltReceiver openBolt)
            {
                openBolt.ReleaseSeer();
            }

            else if (eventTarget.interactable is BoltActionRifle boltAction)
            {
                boltAction.DropHammer();
            }

            else if (eventTarget.interactable is BreakActionWeapon breakAction)
            {
                breakAction.DropHammer();
            }

            else if (eventTarget.interactable is Derringer derringer)
            {
                derringer.DropHammer();
            }

            else if (eventTarget.interactable is LeverActionFirearm lever)
            {
                lever.Fire();
            }

            else if (eventTarget.interactable is TubeFedShotgun shotgun)
            {
                shotgun.ReleaseHammer();
            }
        }


        public static void ReleaseMagazine(AnimatedPoint eventTarget)
        {
            if(eventTarget.interactable is FVRFireArm firearm)
            {
                firearm.EjectMag();
            }
        }

        public static void RackSlide(AnimatedPoint eventTarget)
        {
            if (eventTarget.interactable is Handgun handgun)
            {
                handgun.Slide.ImpartFiringImpulse();
            }

            else if (eventTarget.interactable is ClosedBoltWeapon closedBolt)
            {
                closedBolt.Bolt.ImpartFiringImpulse();
            }

            else if (eventTarget.interactable is OpenBoltReceiver openBolt)
            {
                openBolt.Bolt.ImpartFiringImpulse();
            }
        }

    }
}
