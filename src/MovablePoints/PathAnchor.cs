using H3VRAnimator.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace H3VRAnimator
{
    public class PathAnchor : MovablePoint
    {
        public AnimationPath path;

        public RotationPoint rotationPoint;
        public SpeedPoint speedPoint;

        public PositionControlPoint forwardPoint;
        public PositionControlPoint backPoint;

        public RotationControlPoint forwardRotationPoint;
        public RotationControlPoint backRotationPoint;

        public OptionPoint optionPoint;
        public List<OptionPoint> optionList = new List<OptionPoint>();

        public bool isJumpPoint = false;

        private bool isOptionExpanded = false;

        public override void Awake()
        {
            base.Awake();

            lockRotation = true;

            CreateRotationPoint();
            CreateSpeedPoint();
            CreatePositionControlPoints();
            CreateRotationControlPoints();
            CreateOptions();
        }


        public override void Update()
        {
            rotationPoint.transform.position = transform.position + Vector3.up * 0.03f;
            speedPoint.transform.position = transform.position + Vector3.up * 0.06f;

            forwardRotationPoint.transform.position = forwardPoint.transform.position + Vector3.up * 0.03f;
            backRotationPoint.transform.position = backPoint.transform.position + Vector3.up * 0.03f;

            optionPoint.transform.position = transform.position + Vector3.down * 0.03f;

            for(int i = 0; i < optionList.Count; i++)
            {
                optionList[i].transform.position = transform.position + Vector3.down * 0.03f * (i + 2);
            }

            base.Update();
        }


        private void CreateRotationPoint()
        {
            GameObject rotation = new GameObject("RotationPoint");
            rotation.transform.SetParent(transform);
            rotation.transform.position = transform.position + Vector3.up * 0.03f;
            rotationPoint = rotation.AddComponent<RotationPoint>();
            rotationPoint.pointColor = Color.cyan;
        }


        private void CreateSpeedPoint()
        {
            GameObject speed = new GameObject("SpeedPoint");
            speed.transform.SetParent(transform);
            speed.transform.position = transform.position + Vector3.up * 0.06f;
            speedPoint = speed.AddComponent<SpeedPoint>();
            speedPoint.pointColor = Color.yellow;
        }


        private void CreatePositionControlPoints()
        {
            GameObject forward = new GameObject("ForwardPoint");
            forward.transform.SetParent(transform);
            forward.transform.position = transform.position + transform.forward * 0.06f;
            forwardPoint = forward.AddComponent<PositionControlPoint>();
            forwardPoint.pointColor = Color.grey;
            forwardPoint.radius = .005f;

            GameObject back = new GameObject("BackwardPoint");
            back.transform.SetParent(transform);
            back.transform.position = transform.position - transform.forward * 0.06f;
            backPoint = back.AddComponent<PositionControlPoint>();
            backPoint.pointColor = Color.grey;
            backPoint.radius = .005f;

            backPoint.other = forwardPoint;
            forwardPoint.other = backPoint;
        }


        private void CreateRotationControlPoints()
        {
            GameObject forwardRot = new GameObject("ForwardRotationPoint");
            forwardRot.transform.SetParent(transform);
            forwardRot.transform.position = transform.position - transform.forward * 0.06f;
            forwardRotationPoint = forwardRot.AddComponent<RotationControlPoint>();
            forwardRotationPoint.pointColor = Color.cyan;
            forwardRotationPoint.radius = .005f;

            GameObject backRot = new GameObject("BackRotationPoint");
            backRot.transform.SetParent(transform);
            backRot.transform.position = transform.position - transform.forward * 0.06f;
            backRotationPoint = backRot.AddComponent<RotationControlPoint>();
            backRotationPoint.pointColor = Color.cyan;
            backRotationPoint.radius = .005f;

            forwardRotationPoint.other = backRotationPoint;
            backRotationPoint.other = forwardRotationPoint;

            forwardRotationPoint.referenceTransform = rotationPoint.transform;
            backRotationPoint.referenceTransform = rotationPoint.transform;
        }


        private void CreateOptions()
        {
            GameObject option = new GameObject("OptionPoint");
            option.transform.SetParent(transform);
            option.transform.position = transform.position + Vector3.up * 0.03f;
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

            GameObject add = new GameObject("Add");
            add.transform.SetParent(transform);
            add.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 2);
            OptionPoint addPoint = add.AddComponent<OptionPoint>();
            addPoint.optionText.text = "Add";
            addPoint.clickEvent = () => { path.InsertPointAfter(this); };
            optionList.Add(addPoint);

            GameObject delete = new GameObject("Delete");
            delete.transform.SetParent(transform);
            delete.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 2);
            OptionPoint deletePoint = delete.AddComponent<OptionPoint>();
            deletePoint.optionText.text = "Delete";
            deletePoint.clickEvent = () => { path.DeletePoint(this); };
            optionList.Add(deletePoint);

            GameObject jump = new GameObject("ToggleJump");
            jump.transform.SetParent(transform);
            jump.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 2);
            OptionPoint jumpPoint = jump.AddComponent<OptionPoint>();
            jumpPoint.optionText.text = "Toggle Jump";
            jumpPoint.clickEvent = ToggleJump;
            optionList.Add(jumpPoint);

            /*
            GameObject addEvent = new GameObject("AddEvent");
            addEvent.transform.SetParent(transform);
            addEvent.transform.position = transform.position + Vector3.down * 0.03f * (optionList.Count + 2);
            OptionPoint eventPoint = addEvent.AddComponent<OptionPoint>();
            eventPoint.optionText.text = "Add Event";
            eventPoint.clickEvent = () => { AddPathSlidePointAfter(eventPoint); };
            optionList.Add(eventPoint);
            */
        }


        private void AddPathSlidePointAfter(OptionPoint pressed)
        {
            GameObject slide = new GameObject("SlidingPoint");
            slide.transform.SetParent(transform);
            slide.transform.position = transform.position;
            PathSlidingPoint slidePoint = slide.AddComponent<PathSlidingPoint>();
            slidePoint.pointColor = new Color(((float)217) / 255, ((float)0) / 255, ((float)255) / 255, 1);
            slidePoint.radius = .005f;
            slidePoint.path = path;
            slidePoint.from = this;
            slidePoint.to = path.GetNextPoint(this);
            slidePoint.position = 0.5f;
            slidePoint.offset = new Vector3(0, -0.2f, 0);
        }


        private void HideOptions()
        {
            optionPoint.optionText.text = "+";

            for(int i = 0; i < optionList.Count; i++)
            {
                Destroy(optionList[i].gameObject);
            }

            optionList.Clear();
        }


        private void ToggleJump()
        {
            isJumpPoint = !isJumpPoint;
        }


        public void SetGizmosEnabled(bool enabled)
        {
            drawGizmos = enabled;

            rotationPoint.drawGizmos = drawGizmos;
            speedPoint.drawGizmos = drawGizmos;
            optionPoint.drawGizmos = drawGizmos;
            forwardPoint.drawGizmos = drawGizmos;
            backPoint.drawGizmos = drawGizmos;
            forwardRotationPoint.drawGizmos = drawGizmos;
            backRotationPoint.drawGizmos = drawGizmos;

            buttonPoint.gameObject.SetActive(drawGizmos);
            rotationPoint.buttonPoint.gameObject.SetActive(drawGizmos);
            speedPoint.buttonPoint.gameObject.SetActive(drawGizmos);
            optionPoint.gameObject.SetActive(drawGizmos);
            forwardPoint.buttonPoint.gameObject.SetActive(drawGizmos);
            backPoint.buttonPoint.gameObject.SetActive(drawGizmos);
            forwardRotationPoint.buttonPoint.gameObject.SetActive(drawGizmos);
            backRotationPoint.buttonPoint.gameObject.SetActive(drawGizmos);

            foreach(OptionPoint point in optionList)
            {
                point.drawGizmos = drawGizmos;
                point.buttonPoint.gameObject.SetActive(drawGizmos);
            }
        }



        public void OnDestroy()
        {
            Destroy(rotationPoint.gameObject);
            Destroy(speedPoint.gameObject);
            Destroy(forwardPoint.gameObject);
            Destroy(backPoint.gameObject);
            Destroy(forwardRotationPoint.gameObject);
            Destroy(backRotationPoint.gameObject);
            Destroy(optionPoint.gameObject);
        }



    }
}
