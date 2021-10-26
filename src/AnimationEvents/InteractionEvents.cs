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
        }

        public static void TouchpadHold(AnimatedPoint eventTarget, Vector2 pressDir)
        {
            eventTarget.fakeHand.Input.TouchpadDown = true;
            eventTarget.fakeHand.Input.TouchpadPressed = true;
            eventTarget.fakeHand.Input.TouchpadAxes = pressDir;

            eventTarget.interactable.UpdateInteraction(eventTarget.fakeHand);

            eventTarget.fakeHand.Input.TouchpadDown = false;
        }

        public static void TouchpadRelease(AnimatedPoint eventTarget)
        {
            eventTarget.fakeHand.Input.TouchpadDown = false;
            eventTarget.fakeHand.Input.TouchpadUp = true;
            eventTarget.fakeHand.Input.TouchpadPressed = false;
            eventTarget.fakeHand.Input.TouchpadAxes = Vector2.zero;

            eventTarget.interactable.UpdateInteraction(eventTarget.fakeHand);

            eventTarget.fakeHand.Input.TouchpadUp = false;
        }


        public static void PullTrigger(AnimatedPoint eventTarget)
        {
            eventTarget.fakeHand.Input.TriggerDown = true;
            eventTarget.fakeHand.Input.TriggerPressed = true;
            eventTarget.fakeHand.Input.TriggerFloat = 1;

            eventTarget.interactable.UpdateInteraction(eventTarget.fakeHand);

            eventTarget.fakeHand.Input.TriggerDown = false;
            eventTarget.fakeHand.Input.TriggerPressed = false;
            eventTarget.fakeHand.Input.TriggerFloat = 0;

            eventTarget.interactable.UpdateInteraction(eventTarget.fakeHand);
        }

        public static void HoldTrigger(AnimatedPoint eventTarget)
        {
            eventTarget.fakeHand.Input.TriggerDown = true;
            eventTarget.fakeHand.Input.TriggerPressed = true;
            eventTarget.fakeHand.Input.TriggerFloat = 1;

            eventTarget.interactable.UpdateInteraction(eventTarget.fakeHand);

            eventTarget.fakeHand.Input.TriggerDown = false;
        }

        public static void ReleaseTrigger(AnimatedPoint eventTarget)
        {
            eventTarget.fakeHand.Input.TriggerDown = false;
            eventTarget.fakeHand.Input.TriggerUp = true;
            eventTarget.fakeHand.Input.TriggerPressed = false;
            eventTarget.fakeHand.Input.TriggerFloat = 0;

            eventTarget.interactable.UpdateInteraction(eventTarget.fakeHand);

            eventTarget.fakeHand.Input.TriggerUp = false;
        }


    }
}
