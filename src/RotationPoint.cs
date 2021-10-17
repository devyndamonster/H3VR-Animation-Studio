using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace H3VRAnimator
{
    public class RotationPoint : MovablePoint
    {
        public override void Awake()
        {
            lockPostion = true;
            base.Awake();
        }

        public override void Update()
        {
            base.Update();

            if (!drawGizmos) return;
            Popcron.Gizmos.Line(transform.position, transform.position + transform.forward * 0.03f, pointColor);
        }
    }
}
