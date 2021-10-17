using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace H3VRAnimator
{
    public class PathAnchor : MovablePoint
    {

        public RotationPoint rotation;
        public SpeedPoint speed;

        public override void Awake()
        {
            base.Awake();

            GameObject rotation = new GameObject("RotationPoint");
            rotation.transform.position = transform.position + Vector3.up * 0.03f;
            this.rotation = rotation.AddComponent<RotationPoint>();
            this.rotation.pointColor = Color.cyan;

            GameObject speed = new GameObject("SpeedPoint");
            speed.transform.position = transform.position + Vector3.up * 0.06f;
            this.speed = speed.AddComponent<SpeedPoint>();
            this.speed.pointColor = Color.yellow;
        }

        public override void Update()
        {
            base.Update();

            rotation.transform.position = transform.position + Vector3.up * 0.03f;
            speed.transform.position = transform.position + Vector3.up * 0.06f;
        }

    }
}
