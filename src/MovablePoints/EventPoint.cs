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
        public List<UnityAction<AnimatedPoint>> enterEvents = new List<UnityAction<AnimatedPoint>>();
        public List<UnityAction<AnimatedPoint>> stayEvents = new List<UnityAction<AnimatedPoint>>();
        public List<UnityAction<AnimatedPoint>> exitEvents = new List<UnityAction<AnimatedPoint>>();

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
                    if (!point.IsHeldByFakeHand()) continue;

                    foreach (UnityAction<AnimatedPoint> action in stayEvents)
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
                endPoint.position = position + ((1 - position) / 2);
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

        public void ClearEvents()
        {
            enterEvents.Clear();
            stayEvents.Clear();
            exitEvents.Clear();
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

            AddEventOption("-", HideOptions);
            AddEventOption("Firearm Events >", ShowFirearmEventOptions);
            AddEventOption("Physics Events >", ShowPhysicsEventOptions);
            AddEventOption("Animation Events >", ShowAnimationEventOptions);
            AddEventOption("Interaction Events >", ShowInteractionEventOptions);
            AddEventOption("Delete Event", () => {
                from.eventsList.Remove(this);
                Destroy(gameObject);
            });
        }


        private void HideOptions()
        {
            ClearOptions();

            AddEventOption("+", ExpandOptions);
        }


        private void ShowFirearmEventOptions()
        {
            ClearOptions();

            AddEventOption("<", ExpandOptions);

            AddEventOption("Set Fire Gun", () => {
                eventText.text = "Fire Gun";
                ClearEvents();
                enterEvents.Add(FirearmEvents.FireGun);
                SetPoint();
            });

            AddEventOption("Set Drop Magazine", () => {
                eventText.text = "Drop Magazine";
                ClearEvents();
                enterEvents.Add(FirearmEvents.ReleaseMagazine);
                SetPoint();
            });

            AddEventOption("Set Rack Slide", () => {
                eventText.text = "Rack Slide";
                ClearEvents();
                enterEvents.Add(FirearmEvents.RackSlide);
                SetPoint();
            });
        }


        private void ShowPhysicsEventOptions()
        {
            ClearOptions();

            AddEventOption("<", ExpandOptions);

            AddEventOption("Set Drop Item", () => {
                eventText.text = "Drop Item";
                ClearEvents();
                enterEvents.Add(PhysicalObjectEvents.DropItem);
                SetPoint();
            });

            AddEventOption("Set Throw Item", () => {
                eventText.text = "Throw Item";
                ClearEvents();
                enterEvents.Add(PhysicalObjectEvents.DropItemWithVelocity);
                SetPoint();
            });
        }


        private void ShowInteractionEventOptions()
        {
            ClearOptions();

            AddEventOption("<", ExpandOptions);

            AddEventOption("Set Touchpad Up", () => {
                eventText.text = "Touchpad Up";
                ClearEvents();
                enterEvents.Add((o) =>
                {
                    InteractionEvents.TouchpadPressed(o, Vector2.up);
                });
                SetPoint();
            });

            AddEventOption("Set Touchpad Down", () => {
                eventText.text = "Touchpad Down";
                ClearEvents();
                enterEvents.Add((o) =>
                {
                    InteractionEvents.TouchpadPressed(o, Vector2.down);
                });
                SetPoint();
            });

            AddEventOption("Set Touchpad Left", () => {
                eventText.text = "Touchpad Left";
                ClearEvents();
                enterEvents.Add((o) =>
                {
                    InteractionEvents.TouchpadPressed(o, Vector2.left);
                });
                SetPoint();
            });

            AddEventOption("Set Touchpad Right", () => {
                eventText.text = "Touchpad Right";
                ClearEvents();
                enterEvents.Add((o) =>
                {
                    InteractionEvents.TouchpadPressed(o, Vector2.right);
                });
                SetPoint();
            });

            AddEventOption("Set Trigger Pull", () => {
                eventText.text = "Trigger Pull";
                ClearEvents();
                enterEvents.Add(InteractionEvents.PullTrigger);
                SetPoint();
            });


            AddEventOption("Set Hold Touchpad Up", () => {
                eventText.text = "Hold Touchpad Up";
                ClearEvents();
                enterEvents.Add((o) =>
                {
                    InteractionEvents.TouchpadHold(o, Vector2.up);
                });
                exitEvents.Add(InteractionEvents.TouchpadRelease);
                SetRanged();
            });

            AddEventOption("Set Hold Touchpad Down", () => {
                eventText.text = "Hold Touchpad Down";
                ClearEvents();
                enterEvents.Add((o) =>
                {
                    InteractionEvents.TouchpadHold(o, Vector2.down);
                });
                exitEvents.Add(InteractionEvents.TouchpadRelease);
                SetRanged();
            });

            AddEventOption("Set Hold Touchpad Left", () => {
                eventText.text = "Hold Touchpad Left";
                ClearEvents();
                enterEvents.Add((o) =>
                {
                    InteractionEvents.TouchpadHold(o, Vector2.left);
                });
                exitEvents.Add(InteractionEvents.TouchpadRelease);
                SetRanged();
            });

            AddEventOption("Set Hold Touchpad Right", () => {
                eventText.text = "Hold Touchpad Right";
                ClearEvents();
                enterEvents.Add((o) =>
                {
                    InteractionEvents.TouchpadHold(o, Vector2.right);
                });
                exitEvents.Add(InteractionEvents.TouchpadRelease);
                SetRanged();
            });


            AddEventOption("Set Trigger Hold", () => {
                eventText.text = "Trigger Hold";
                ClearEvents();
                enterEvents.Add(InteractionEvents.HoldTrigger);
                exitEvents.Add(InteractionEvents.ReleaseTrigger);
                SetRanged();
            });
        }


        private void ShowAnimationEventOptions()
        {
            ClearOptions();

            AddEventOption("<", ExpandOptions);
            AddEventOption("Duplicate Animation Events >", ShowDuplicateEventOptions);
            AddEventOption("Move Animation Events >", ShowMoveEventOptions);
        }


        private void ShowDuplicateEventOptions()
        {
            ClearOptions();

            AddEventOption("<", ShowAnimationEventOptions);

            foreach (AnimationPath path in H3VRAnimator.SpectatorPanel.paths)
            {
                AddEventOption("Duplicate To Path : " + path.pathName, () => {
                    eventText.text = "Duplicate To Path : " + path.pathName;
                    enterEvents.Clear();
                    enterEvents.Add((o) =>
                    {
                        AnimationEvents.DuplicateToPath(o, path);
                    });
                    SetPoint();
                });
            }
        }


        private void ShowMoveEventOptions()
        {
            ClearOptions();

            AddEventOption("<", ShowAnimationEventOptions);

            foreach (AnimationPath path in H3VRAnimator.SpectatorPanel.paths)
            {
                AddEventOption("Move To Path : " + path.pathName, () => {
                    eventText.text = "Move To Path : " + path.pathName;
                    enterEvents.Clear();
                    enterEvents.Add((o) =>
                    {
                        AnimationEvents.MoveToPath(o, path);
                    });
                    SetPoint();
                });
            }
        }


        private OptionPoint AddEventOption(string optionText, UnityAction onClick)
        {
            GameObject pointObj = new GameObject(optionText);
            pointObj.transform.SetParent(transform);
            pointObj.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 1);
            OptionPoint point = pointObj.AddComponent<OptionPoint>();
            point.optionText.text = optionText;
            point.clickEvent = onClick;
            optionList.Add(point);

            return point;
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
                eventTarget.activeEvents.Add(this);
            }

            //Now perform the entering event for this animation
            foreach (UnityAction<AnimatedPoint> action in enterEvents)
            {
                action.Invoke(eventTarget);
            }
            
        }

        public virtual void HandleEventBackward(AnimatedPoint eventTarget)
        {
            if(endPoint != null)
            {
                AnimLogger.Log("Handling event backwards, removing event, event count: " + trackedAnimations.Count + ", Prev pos: " + eventTarget.prevPosition + ", Pos: " + eventTarget.position);
                trackedAnimations.Remove(eventTarget);
                eventTarget.activeEvents.Remove(this);

                //Perform exit for ranged event
                foreach (UnityAction<AnimatedPoint> action in exitEvents)
                {
                    action.Invoke(eventTarget);
                }
            }

            
        }

        public virtual void HandleEndpointEventForward(AnimatedPoint eventTarget)
        {
            if (endPoint != null)
            {
                AnimLogger.Log("Handling endpoint forwards, Removing from event, event count: " + trackedAnimations.Count + ", Prev pos: " + eventTarget.prevPosition + ", Pos: " + eventTarget.position);
                trackedAnimations.Remove(eventTarget);
                eventTarget.activeEvents.Remove(this);

                //Perform exit for ranged event
                foreach (UnityAction<AnimatedPoint> action in exitEvents)
                {
                    action.Invoke(eventTarget);
                }
            }

        }

        public virtual void HandleEndpointEventBackward(AnimatedPoint eventTarget)
        {
            if (endPoint != null)
            {
                AnimLogger.Log("Handling endpoint backwards, Adding to event, event count: " + trackedAnimations.Count + ", Prev pos: " + eventTarget.prevPosition + ", Pos: " + eventTarget.position);
                trackedAnimations.Add(eventTarget);
                eventTarget.activeEvents.Add(this);

                //Perform enter for ranged event
                foreach (UnityAction<AnimatedPoint> action in enterEvents)
                {
                    action.Invoke(eventTarget);
                }
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
