using FistVR;
using H3VRAnimator.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace H3VRAnimator
{
    public class EventPoint : PathSlidingPoint
    {
        public List<UnityAction<AnimatedPoint>> events = new List<UnityAction<AnimatedPoint>>();

        public GameObject textObject;
        public Text eventText;

        public bool isOptionExpanded;
        public List<OptionPoint> optionList = new List<OptionPoint>();

        public EventEndPoint endPoint;
        public List<AnimatedPoint> trackedAnimations = new List<AnimatedPoint>();

        public override void Awake()
        {
            base.Awake();

            CreateOptions();
            AddEventText();
        }


        public override void Update()
        {
            base.Update();

            if (drawGizmos)
            {
                UpdateOptionPosition();
                UpdateEventText();
            }

            HandleRangedEvents();
        }

        public override void DrawGizmos()
        {
            if (!drawGizmos) return;
            base.DrawGizmos();

            if(endPoint != null)
            {
                Popcron.Gizmos.Line(transform.position, endPoint.transform.position, pointColor);
            }
        }


        protected void UpdateOptionPosition()
        {
            for (int i = 0; i < optionList.Count; i++)
            {
                optionList[i].transform.position = transform.position + Vector3.down * 0.03f * (i + 1);
            }
        }


        protected void UpdateEventText()
        {
            textObject.transform.rotation = Quaternion.LookRotation(textObject.transform.position - GM.CurrentPlayerBody.Head.position);
            textObject.transform.position = transform.position + Vector3.up * 0.03f;
        }


        protected void HandleRangedEvents()
        {
            if (endPoint != null)
            {
                foreach(AnimatedPoint point in trackedAnimations)
                {
                    foreach (UnityAction<AnimatedPoint> action in events)
                    {
                        action.Invoke(point);
                    }
                }
            }
        }


        public void SetRanged()
        {
            if(endPoint == null)
            {
                GameObject slide = new GameObject("OtherPoint");
                slide.transform.position = transform.position;
                endPoint = slide.AddComponent<EventEndPoint>();
                endPoint.pointColor = new Color(((float)101) / 255, ((float)48) / 255, ((float)110) / 255, 1);
                endPoint.radius = .004f;
                endPoint.path = path;
                endPoint.from = from;
                endPoint.to = path.GetNextPoint(to);
                endPoint.position = 0.5f;
                endPoint.other = this;

                from.eventEndList.Add(endPoint);
            }
        }


        public void SetPoint()
        {
            if(endPoint != null)
            {
                endPoint.from.eventEndList.Remove(endPoint);

                Destroy(endPoint.gameObject);
                endPoint = null;
            }
        }


        private void AddEventText()
        {
            textObject = new GameObject();
            textObject.transform.SetParent(transform);
            textObject.transform.position = transform.position + Vector3.up * 0.03f;
            textObject.transform.rotation = transform.rotation;
            textObject.transform.localScale = new Vector3(0.0005f, 0.0005f, 0.0005f);

            eventText = textObject.AddComponent<Text>();
            eventText.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
            eventText.alignment = TextAnchor.MiddleCenter;
            eventText.horizontalOverflow = HorizontalWrapMode.Overflow;
            eventText.verticalOverflow = VerticalWrapMode.Overflow;
            eventText.fontSize = 16;
            eventText.text = "Select An Event";
        }


        private void CreateOptions()
        {
            GameObject option = new GameObject("OptionPoint");
            option.transform.SetParent(transform);
            option.transform.position = transform.position + Vector3.down * 0.03f;

            optionList.Add(option.AddComponent<OptionPoint>());
            optionList[0].optionText.text = "+";
            optionList[0].clickEvent = ExpandOptions;
        }


        private void ClearOptions()
        {
            foreach(OptionPoint point in optionList)
            {
                Destroy(point.gameObject);
            }

            optionList.Clear();
        }


        private void ExpandOptions()
        {
            ClearOptions();

            GameObject hide = new GameObject("HidePoint");
            hide.transform.SetParent(transform);
            hide.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 1);
            OptionPoint hidePoint = hide.AddComponent<OptionPoint>();
            hidePoint.optionText.text = "-";
            hidePoint.clickEvent = HideOptions;
            optionList.Add(hidePoint);

            GameObject firearm = new GameObject("FirearmEvents");
            firearm.transform.SetParent(transform);
            firearm.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 1);
            OptionPoint firearmPoint = firearm.AddComponent<OptionPoint>();
            firearmPoint.optionText.text = "Firearm Events >";
            firearmPoint.clickEvent = ShowFirearmEventOptions;
            optionList.Add(firearmPoint);

            GameObject physics = new GameObject("PhysicsEvents");
            physics.transform.SetParent(transform);
            physics.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 1);
            OptionPoint physicsPoint = physics.AddComponent<OptionPoint>();
            physicsPoint.optionText.text = "Physics Events >";
            physicsPoint.clickEvent = ShowPhysicsEventOptions;
            optionList.Add(physicsPoint);

            GameObject animation = new GameObject("AnimationEvents");
            animation.transform.SetParent(transform);
            animation.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 1);
            OptionPoint animationPoint = animation.AddComponent<OptionPoint>();
            animationPoint.optionText.text = "Animation Events >";
            animationPoint.clickEvent = ShowAnimationEventOptions;
            optionList.Add(animationPoint);

            GameObject interaction = new GameObject("InteractionEvents");
            interaction.transform.SetParent(transform);
            interaction.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 1);
            OptionPoint interactionPoint = interaction.AddComponent<OptionPoint>();
            interactionPoint.optionText.text = "Interaction Events >";
            interactionPoint.clickEvent = ShowInteractionEventOptions;
            optionList.Add(interactionPoint);

            GameObject delete = new GameObject("DeletePoint");
            delete.transform.SetParent(transform);
            delete.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 1);
            OptionPoint deletePoint = delete.AddComponent<OptionPoint>();
            deletePoint.optionText.text = "Delete Event";
            deletePoint.clickEvent = () => {
                from.eventsList.Remove(this);
                Destroy(gameObject);
            };
            optionList.Add(deletePoint);
        }


        private void HideOptions()
        {
            ClearOptions();

            GameObject expand = new GameObject("ExpandPoint");
            expand.transform.SetParent(transform);
            expand.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 1);
            OptionPoint expandPoint = expand.AddComponent<OptionPoint>();
            expandPoint.optionText.text = "+";
            expandPoint.clickEvent = ExpandOptions;
            optionList.Add(expandPoint);
        }


        private void ShowFirearmEventOptions()
        {
            ClearOptions();

            GameObject back = new GameObject("Back");
            back.transform.SetParent(transform);
            back.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 1);
            OptionPoint backPoint = back.AddComponent<OptionPoint>();
            backPoint.optionText.text = "<";
            backPoint.clickEvent = ExpandOptions;
            optionList.Add(backPoint);

            GameObject fire = new GameObject("FireGun");
            fire.transform.SetParent(transform);
            fire.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 1);
            OptionPoint firePoint = fire.AddComponent<OptionPoint>();
            firePoint.optionText.text = "Set Fire Gun";
            firePoint.clickEvent = () => {
                eventText.text = "Fire Gun";
                events.Clear();
                events.Add(FirearmEvents.FireGun);
                SetPoint();
            };
            optionList.Add(firePoint);

            GameObject hold = new GameObject("HoldFire");
            hold.transform.SetParent(transform);
            hold.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 1);
            OptionPoint holdPoint = hold.AddComponent<OptionPoint>();
            holdPoint.optionText.text = "Set Hold Fire Gun";
            holdPoint.clickEvent = () => {
                eventText.text = "Hold Fire Gun";
                events.Clear();
                events.Add(FirearmEvents.FireGun);
                SetRanged();
            };
            optionList.Add(holdPoint);

            GameObject mag = new GameObject("DropMag");
            mag.transform.SetParent(transform);
            mag.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 1);
            OptionPoint dropMag = mag.AddComponent<OptionPoint>();
            dropMag.optionText.text = "Set Drop Magazine";
            dropMag.clickEvent = () => {
                eventText.text = "Drop Magazine";
                events.Clear();
                events.Add(FirearmEvents.ReleaseMagazine);
                SetPoint();
            };
            optionList.Add(dropMag);

            GameObject rack = new GameObject("RackSlide");
            rack.transform.SetParent(transform);
            rack.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 1);
            OptionPoint rackPoint = rack.AddComponent<OptionPoint>();
            rackPoint.optionText.text = "Set Rack Slide";
            rackPoint.clickEvent = () => {
                eventText.text = "Rack Slide";
                events.Clear();
                events.Add(FirearmEvents.RackSlide);
                SetPoint();
            };
            optionList.Add(rackPoint);
        }


        private void ShowPhysicsEventOptions()
        {
            ClearOptions();

            GameObject back = new GameObject("Back");
            back.transform.SetParent(transform);
            back.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 1);
            OptionPoint backPoint = back.AddComponent<OptionPoint>();
            backPoint.optionText.text = "<";
            backPoint.clickEvent = ExpandOptions;
            optionList.Add(backPoint);

            GameObject drop = new GameObject("DropItem");
            drop.transform.SetParent(transform);
            drop.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 1);
            OptionPoint dropPoint = drop.AddComponent<OptionPoint>();
            dropPoint.optionText.text = "Set Drop Item";
            dropPoint.clickEvent = () => {
                eventText.text = "Drop Item";
                events.Clear();
                events.Add(PhysicalObjectEvents.DropItem);
                SetPoint();
            };
            optionList.Add(dropPoint);

            GameObject throwItem = new GameObject("ThrowItem");
            throwItem.transform.SetParent(transform);
            throwItem.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 1);
            OptionPoint throwItemPoint = throwItem.AddComponent<OptionPoint>();
            throwItemPoint.optionText.text = "Set Throw Item";
            throwItemPoint.clickEvent = () => {
                eventText.text = "Throw Item";
                events.Clear();
                events.Add(PhysicalObjectEvents.DropItemWithVelocity);
                SetPoint();
            };
            optionList.Add(throwItemPoint);
        }


        private void ShowInteractionEventOptions()
        {
            ClearOptions();

            GameObject back = new GameObject("Back");
            back.transform.SetParent(transform);
            back.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 1);
            OptionPoint backPoint = back.AddComponent<OptionPoint>();
            backPoint.optionText.text = "<";
            backPoint.clickEvent = ExpandOptions;
            optionList.Add(backPoint);

            GameObject up = new GameObject("InteractionUp");
            up.transform.SetParent(transform);
            up.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 1);
            OptionPoint upPoint = up.AddComponent<OptionPoint>();
            upPoint.optionText.text = "Set Touchpad Up";
            upPoint.clickEvent = () => {
                eventText.text = "Touchpad Up";
                events.Clear();
                events.Add((o) =>
                {
                    InteractionEvents.TouchpadPressed(o, Vector2.up);
                });
                SetPoint();
            };
            optionList.Add(upPoint);

            GameObject down = new GameObject("InteractionDown");
            down.transform.SetParent(transform);
            down.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 1);
            OptionPoint downPoint = down.AddComponent<OptionPoint>();
            downPoint.optionText.text = "Set Touchpad Down";
            downPoint.clickEvent = () => {
                eventText.text = "Touchpad Down";
                events.Clear();
                events.Add((o) =>
                {
                    InteractionEvents.TouchpadPressed(o, Vector2.down);
                });
                SetPoint();
            };
            optionList.Add(downPoint);

            GameObject left = new GameObject("InteractionLeft");
            left.transform.SetParent(transform);
            left.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 1);
            OptionPoint leftPoint = left.AddComponent<OptionPoint>();
            leftPoint.optionText.text = "Set Touchpad Left";
            leftPoint.clickEvent = () => {
                eventText.text = "Touchpad Left";
                events.Clear();
                events.Add((o) =>
                {
                    InteractionEvents.TouchpadPressed(o, Vector2.left);
                });
                SetPoint();
            };
            optionList.Add(leftPoint);

            GameObject right = new GameObject("InteractionRight");
            right.transform.SetParent(transform);
            right.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 1);
            OptionPoint rightPoint = right.AddComponent<OptionPoint>();
            rightPoint.optionText.text = "Set Touchpad Right";
            rightPoint.clickEvent = () => {
                eventText.text = "Touchpad Right";
                events.Clear();
                events.Add((o) =>
                {
                    InteractionEvents.TouchpadPressed(o, Vector2.right);
                });
                SetPoint();
            };
            optionList.Add(rightPoint);

            GameObject trigger = new GameObject("InteractionTrigger");
            trigger.transform.SetParent(transform);
            trigger.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 1);
            OptionPoint triggerPoint = trigger.AddComponent<OptionPoint>();
            triggerPoint.optionText.text = "Set Trigger Pull";
            triggerPoint.clickEvent = () => {
                eventText.text = "Trigger Pull";
                events.Clear();
                events.Add(InteractionEvents.PullTrigger);
                SetPoint();
            };
            optionList.Add(triggerPoint);
        }


        private void ShowAnimationEventOptions()
        {
            ClearOptions();

            GameObject back = new GameObject("Back");
            back.transform.SetParent(transform);
            back.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 1);
            OptionPoint backPoint = back.AddComponent<OptionPoint>();
            backPoint.optionText.text = "<";
            backPoint.clickEvent = ExpandOptions;
            optionList.Add(backPoint);

            GameObject duplicate = new GameObject("Duplicate");
            duplicate.transform.SetParent(transform);
            duplicate.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 1);
            OptionPoint duplicatePoint = duplicate.AddComponent<OptionPoint>();
            duplicatePoint.optionText.text = "Duplicate Animation Events >";
            duplicatePoint.clickEvent = ShowDuplicateEventOptions;
            optionList.Add(duplicatePoint);

            GameObject move = new GameObject("Move");
            move.transform.SetParent(transform);
            move.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 1);
            OptionPoint movePoint = move.AddComponent<OptionPoint>();
            movePoint.optionText.text = "Move Animation Events >";
            movePoint.clickEvent = ShowMoveEventOptions;
            optionList.Add(movePoint);
        }


        private void ShowDuplicateEventOptions()
        {
            ClearOptions();

            GameObject back = new GameObject("Back");
            back.transform.SetParent(transform);
            back.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 1);
            OptionPoint backPoint = back.AddComponent<OptionPoint>();
            backPoint.optionText.text = "<";
            backPoint.clickEvent = ShowAnimationEventOptions;
            optionList.Add(backPoint);

            foreach(AnimationPath path in H3VRAnimator.SpectatorPanel.paths)
            {
                GameObject duplicate = new GameObject("Duplicate");
                duplicate.transform.SetParent(transform);
                duplicate.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 1);
                OptionPoint duplicatePoint = duplicate.AddComponent<OptionPoint>();
                duplicatePoint.optionText.text = "Duplicate To Path : " + path.pathName;
                duplicatePoint.clickEvent = () => {
                    eventText.text = "Duplicate To Path : " + path.pathName;
                    events.Clear();
                    events.Add((o) =>
                    {
                        AnimationEvents.DuplicateToPath(o, path);
                    });
                    SetPoint();
                };
                optionList.Add(duplicatePoint);
            }
        }


        private void ShowMoveEventOptions()
        {
            ClearOptions();

            GameObject back = new GameObject("Back");
            back.transform.SetParent(transform);
            back.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 1);
            OptionPoint backPoint = back.AddComponent<OptionPoint>();
            backPoint.optionText.text = "<";
            backPoint.clickEvent = ShowAnimationEventOptions;
            optionList.Add(backPoint);

            foreach(AnimationPath path in H3VRAnimator.SpectatorPanel.paths)
            {
                GameObject move = new GameObject("Move");
                move.transform.SetParent(transform);
                move.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 1);
                OptionPoint movePoint = move.AddComponent<OptionPoint>();
                movePoint.optionText.text = "Move To Path : " + path.pathName;
                movePoint.clickEvent = () => {
                    eventText.text = "Move To Path : " + path.pathName;
                    events.Clear();
                    events.Add((o) =>
                    {
                        AnimationEvents.MoveToPath(o, path);
                    });
                    SetPoint();
                };
                optionList.Add(movePoint);
            }
        }




        public void SetGizmosEnabled(bool enabled)
        {
            drawGizmos = enabled;

            buttonPoint.SetActive(drawGizmos);

            textObject.SetActive(drawGizmos);

            foreach(OptionPoint point in optionList)
            {
                point.drawGizmos = drawGizmos;
                point.buttonPoint.SetActive(drawGizmos);
            }
        }


        protected override void ShiftEndpointsForwards(PathAnchor next)
        {
            from.eventsList.Remove(this);
            base.ShiftEndpointsForwards(next);
            from.eventsList.Add(this);
        }


        protected override void ShiftEndpointsBackwards(PathAnchor prev)
        {
            from.eventsList.Remove(this);
            base.ShiftEndpointsBackwards(prev);
            from.eventsList.Add(this);
        }


        public virtual void HandleEventForward(AnimatedPoint eventTarget)
        {
            //If the animated point just entered an event region, we track the animation
            if (endPoint != null)
            {
                AnimLogger.Log("Handling event forwards, Adding to event, event count: " + trackedAnimations.Count + ", Prev pos: " + eventTarget.prevPosition + ", Pos: " + eventTarget.position);
                trackedAnimations.Add(eventTarget);
            }

            //If this event is just a point event, perform the event
            else
            {
                foreach (UnityAction<AnimatedPoint> action in events)
                {
                    action.Invoke(eventTarget);
                }
            }
        }

        public virtual void HandleEventBackward(AnimatedPoint eventTarget)
        {
            if(endPoint != null)
            {
                AnimLogger.Log("Handling event backwards, removing event, event count: " + trackedAnimations.Count + ", Prev pos: " + eventTarget.prevPosition + ", Pos: " + eventTarget.position);
                trackedAnimations.Remove(eventTarget);
            }
        }

        public virtual void HandleEndpointEventForward(AnimatedPoint eventTarget)
        {
            if (endPoint != null)
            {
                AnimLogger.Log("Handling endpoint forwards, Removing from event, event count: " + trackedAnimations.Count + ", Prev pos: " + eventTarget.prevPosition + ", Pos: " + eventTarget.position);
                trackedAnimations.Remove(eventTarget);
            }
        }

        public virtual void HandleEndpointEventBackward(AnimatedPoint eventTarget)
        {
            if (endPoint != null)
            {
                AnimLogger.Log("Handling endpoint backwards, Adding to event, event count: " + trackedAnimations.Count + ", Prev pos: " + eventTarget.prevPosition + ", Pos: " + eventTarget.position);
                trackedAnimations.Add(eventTarget);
            }
        }


        public void OnDestroy()
        {
            Destroy(textObject);

            foreach(OptionPoint point in optionList)
            {
                Destroy(point.gameObject);
            }

            if(endPoint != null)
            {
                Destroy(endPoint.gameObject);
            }
        }
    }



}
