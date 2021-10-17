using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace H3VRAnimator
{
    
    public class AnimationPath
    {
        public List<PathAnchor> points = new List<PathAnchor>();

        public void DrawPath()
        {
            for (int i = 1; i < points.Count; i++)
            {
                Popcron.Gizmos.Line(points[i - 1].transform.position, points[i].transform.position, Color.red);
            }
        }
    }

}
