using FistVR;
using H3VRAnimator.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace H3VRAnimator
{
    public static class InteractionEvents
    {

        public static void TouchpadPressed(AnimatedPoint eventTarget, Vector2 pressDir)
        {
            eventTarget.fakeHand.Input.TouchpadDown = true;
            eventTarget.fakeHand.Input.TouchpadPressed = true;
            eventTarget.fakeHand.Input.TouchpadAxes = pressDir;

            eventTarget.interactable.UpdateInteraction(eventTarget.fakeHand);

            eventTarget.fakeHand.Input.TouchpadDown = false;
            eventTarget.fakeHand.Input.TouchpadPressed = false;
            eventTarget.fakeHand.Input.TouchpadAxes = Vector2.zero;

            eventTarget.interactable.UpdateInteraction(eventTarget.fakeHand);

            eventTarget.interactable.IsHeld = false;
            eventTarget.interactable.m_hand = null;
        }

        public static void PullTrigger(AnimatedPoint eventTarget)
        {
            eventTarget.fakeHand.Input.TriggerDown = true;
            eventTarget.fakeHand.Input.TriggerPressed = true;
            eventTarget.fakeHand.Input.TriggerFloat = 1;

            eventTarget.interactable.UpdateInteraction(eventTarget.fakeHand);

            if(eventTarget.interactable is Handgun handgun){
                AnimLogger.Log($"Handgun, Trigger float ({handgun.m_triggerFloat}), Break Threshold ({handgun.TriggerBreakThreshold})");
            }

            eventTarget.fakeHand.Input.TriggerDown = false;
            eventTarget.fakeHand.Input.TriggerPressed = false;
            eventTarget.fakeHand.Input.TriggerFloat = 0;

            eventTarget.interactable.UpdateInteraction(eventTarget.fakeHand);

            eventTarget.interactable.IsHeld = false;
            eventTarget.interactable.m_hand = null;
        }


    }
}
