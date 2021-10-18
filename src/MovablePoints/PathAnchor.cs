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

        public PositionControlPoint forwardPoint;
        public PositionControlPoint backPoint;

        public RotationControlPoint forwardRotationPoint;
        public RotationControlPoint backRotationPoint;

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
            forwardPoint = forward.AddComponent<PositionControlPoint>();
            forwardPoint.pointColor = Color.grey;
            forwardPoint.radius = .005f;

            GameObject back = new GameObject("BackwardPoint");
            back.transform.SetParent(transform);
            back.transform.position = transform.position - transform.forward * 0.06f;
            backPoint = back.AddComponent<PositionControlPoint>();
            backPoint.pointColor = Color.grey;
            backPoint.radius = .005f;

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

            backPoint.other = forwardPoint;
            forwardPoint.other = backPoint;

            forwardRotationPoint.other = backRotationPoint;
            backRotationPoint.other = forwardRotationPoint;

            forwardRotationPoint.referenceTransform = rotationPoint.transform;
            backRotationPoint.referenceTransform = rotationPoint.transform;
        }

        public override void Update()
        {
            base.Update();

            rotationPoint.transform.position = transform.position + Vector3.up * 0.03f;
            speedPoint.transform.position = transform.position + Vector3.up * 0.06f;

            forwardRotationPoint.transform.position = forwardPoint.transform.position + Vector3.up * 0.03f;
            backRotationPoint.transform.position = backPoint.transform.position + Vector3.up * 0.03f;
        }

        public void OnDestroy()
        {
            Destroy(rotationPoint.gameObject);
            Destroy(speedPoint.gameObject);
            Destroy(forwardPoint.gameObject);
            Destroy(backPoint.gameObject);
            Destroy(forwardRotationPoint.gameObject);
            Destroy(backRotationPoint.gameObject);
        }

    }
}
