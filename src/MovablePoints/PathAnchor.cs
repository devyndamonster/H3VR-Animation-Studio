using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace H3VRAnimator
{
    public class PathAnchor : MovablePoint
    {

        public RotationPoint rotationPoint;
        public SpeedPoint speedPoint;
        public ControlPoint forwardPoint;
        public ControlPoint backPoint;

        public override void Awake()
        {
            base.Awake();

            lockRotation = true;

            GameObject rotation = new GameObject("RotationPoint");
            rotation.transform.SetParent(transform);
            rotation.transform.position = transform.position + Vector3.up * 0.03f;
            rotationPoint = rotation.AddComponent<RotationPoint>();
            rotationPoint.pointColor = Color.cyan;

            GameObject speed = new GameObject("SpeedPoint");
            speed.transform.SetParent(transform);
            speed.transform.position = transform.position + Vector3.up * 0.06f;
            speedPoint = speed.AddComponent<SpeedPoint>();
            speedPoint.pointColor = Color.yellow;

            GameObject forward = new GameObject("ForwardPoint");
            forward.transform.SetParent(transform);
            forward.transform.position = transform.position + transform.forward * 0.06f;
            forwardPoint = forward.AddComponent<ControlPoint>();
            forwardPoint.pointColor = Color.grey;
            forwardPoint.radius = .005f;

            GameObject back = new GameObject("BackwardPoint");
            back.transform.SetParent(transform);
            back.transform.position = transform.position - transform.forward * 0.06f;
            backPoint = back.AddComponent<ControlPoint>();
            backPoint.pointColor = Color.grey;
            backPoint.radius = .005f;

            backPoint.other = forwardPoint;
            forwardPoint.other = backPoint;
        }

        public override void Update()
        {
            base.Update();

            rotationPoint.transform.position = transform.position + Vector3.up * 0.03f;
            speedPoint.transform.position = transform.position + Vector3.up * 0.06f;
        }

        public void OnDestroy()
        {
            Destroy(rotationPoint.gameObject);
            Destroy(speedPoint.gameObject);
            Destroy(forwardPoint.gameObject);
            Destroy(backPoint.gameObject);
        }

    }
}
