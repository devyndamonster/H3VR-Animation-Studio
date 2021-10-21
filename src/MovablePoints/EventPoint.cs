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
        public OptionPoint optionPoint;
        public List<OptionPoint> optionList = new List<OptionPoint>();

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
        }

        protected void UpdateOptionPosition()
        {
            optionPoint.transform.position = transform.position + Vector3.down * 0.03f;

            for (int i = 0; i < optionList.Count; i++)
            {
                optionList[i].transform.position = transform.position + Vector3.down * 0.03f * (i + 2);
            }
        }


        protected void UpdateEventText()
        {
            textObject.transform.rotation = Quaternion.LookRotation(textObject.transform.position - GM.CurrentPlayerBody.Head.position);
            textObject.transform.position = transform.position + Vector3.up * 0.03f;
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
            optionPoint = option.AddComponent<OptionPoint>();
            optionPoint.optionText.text = "+";

            optionPoint.clickEvent = ToggleShowOptions;
        }


        private void ToggleShowOptions()
        {
            isOptionExpanded = !isOptionExpanded;

            if (isOptionExpanded)
            {
                ExpandOptions();
            }
            else
            {
                HideOptions();
            }
        }


        private void ExpandOptions()
        {
            optionPoint.optionText.text = "-";

            GameObject delete = new GameObject("DeletePoint");
            delete.transform.SetParent(transform);
            delete.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 2);
            OptionPoint deletePoint = delete.AddComponent<OptionPoint>();
            deletePoint.optionText.text = "Delete Event";
            deletePoint.clickEvent = () => {
                from.eventsList.Remove(this);
                Destroy(gameObject);
            };
            optionList.Add(deletePoint);

            GameObject fire = new GameObject("FireGun");
            fire.transform.SetParent(transform);
            fire.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 2);
            OptionPoint firePoint = fire.AddComponent<OptionPoint>();
            firePoint.optionText.text = "Set Fire Gun";
            firePoint.clickEvent = () => {
                eventText.text = "Fire Gun";
                events.Clear();
                events.Add(FirearmEvents.FireGun); 
            };
            optionList.Add(firePoint);

            GameObject mag = new GameObject("DropMag");
            mag.transform.SetParent(transform);
            mag.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 2);
            OptionPoint dropMag = mag.AddComponent<OptionPoint>();
            dropMag.optionText.text = "Set Drop Magazine";
            dropMag.clickEvent = () => {
                eventText.text = "Drop Magazine";
                events.Clear();
                events.Add(FirearmEvents.ReleaseMagazine);
            };
            optionList.Add(dropMag);

            GameObject rack = new GameObject("RackSlide");
            rack.transform.SetParent(transform);
            rack.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 2);
            OptionPoint rackPoint = rack.AddComponent<OptionPoint>();
            rackPoint.optionText.text = "Set Rack Slide";
            rackPoint.clickEvent = () => {
                eventText.text = "Rack Slide";
                events.Clear();
                events.Add(FirearmEvents.RackSlide);
            };
            optionList.Add(rackPoint);

            GameObject drop = new GameObject("DropItem");
            drop.transform.SetParent(transform);
            drop.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 2);
            OptionPoint dropPoint = drop.AddComponent<OptionPoint>();
            dropPoint.optionText.text = "Set Drop Item";
            dropPoint.clickEvent = () => {
                eventText.text = "Drop Item";
                events.Clear();
                events.Add(PhysicalObjectEvents.DropItem);
            };
            optionList.Add(dropPoint);

            GameObject throwItem = new GameObject("ThrowItem");
            throwItem.transform.SetParent(transform);
            throwItem.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 2);
            OptionPoint throwItemPoint = throwItem.AddComponent<OptionPoint>();
            throwItemPoint.optionText.text = "Set Throw Item";
            throwItemPoint.clickEvent = () => {
                eventText.text = "Throw Item";
                events.Clear();
                events.Add(PhysicalObjectEvents.DropItemWithVelocity);
            };
            optionList.Add(throwItemPoint);

        }


        private void HideOptions()
        {
            optionPoint.optionText.text = "+";

            for (int i = 0; i < optionList.Count; i++)
            {
                Destroy(optionList[i].gameObject);
            }

            optionList.Clear();
        }


        public void SetGizmosEnabled(bool enabled)
        {
            drawGizmos = enabled;

            buttonPoint.SetActive(drawGizmos);

            optionPoint.drawGizmos = drawGizmos;
            optionPoint.buttonPoint.SetActive(drawGizmos);

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


        public virtual void HandleEvents(AnimatedPoint eventTarget)
        {
            foreach(UnityAction<AnimatedPoint> action in events) {
                action.Invoke(eventTarget);
            }
        }

        public void OnDestroy()
        {
            Destroy(textObject);
            Destroy(optionPoint.gameObject);

            foreach(OptionPoint point in optionList)
            {
                Destroy(point.gameObject);
            }
        }
    }



}
