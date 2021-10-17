using FistVR;
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


        public void AddPoint(Vector3 position)
        {
            GameObject anchorObject = new GameObject("MovablePoint");
            anchorObject.transform.position = position;

            FVRViveHand otherHand = H3VRAnimator.GetNonPointingHand();
            if (otherHand.CurrentInteractable != null && otherHand.CurrentInteractable.GetComponent<SpectatorPanel>() == null)
            {
                anchorObject.transform.SetParent(otherHand.CurrentInteractable.transform);
            }

            PathAnchor anchorComp = anchorObject.AddComponent<PathAnchor>();

            points.Add(anchorComp);



            //Set bezier curve points visible
            if (points.Count == 1)
            {
                anchorComp.forwardPoint.gameObject.SetActive(false);
                anchorComp.backPoint.gameObject.SetActive(false);
            }

            else
            {
                points[points.Count - 2].forwardPoint.gameObject.SetActive(true);
                anchorComp.backPoint.gameObject.SetActive(true);
                anchorComp.forwardPoint.gameObject.SetActive(false);

                if(points[points.Count - 2].backPoint.gameObject.activeSelf)
                {
                    points[points.Count - 2].backPoint.PositionOtherPoint();
                }
            }
        }


        public void DrawPath()
        {
            /*
            for (int i = 1; i < points.Count; i++)
            {
                Popcron.Gizmos.Line(points[i - 1].transform.position, points[i].transform.position, Color.red);
            }
            */

            for (int i = 1; i < points.Count; i++)
            {
                PathAnchor from = points[i - 1];
                PathAnchor to = points[i];

                Vector3 prevPoint = from.transform.position;

                for(int j = 1; j <= 10; j++)
                {
                    float t = j / 10f;

                    Vector3 nextPoint = CurvePosition(from, to, t);

                    Popcron.Gizmos.Line(prevPoint, nextPoint, Color.red);

                    prevPoint = nextPoint;
                }
            }
        }


        //Concepts based on sebastion lague video: https://www.youtube.com/watch?v=RF04Fi9OCPc
        private Vector3 QuatraticLerp(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            Vector3 p0 = Vector3.Lerp(a, b, t);
            Vector3 p1 = Vector3.Lerp(b, c, t);
            return Vector3.Lerp(p0, p1, t);
        }


        public Vector3 CurvePosition(PathAnchor from, PathAnchor to, float t)
        {
            Vector3 p0 = QuatraticLerp(from.transform.position, from.forwardPoint.transform.position, to.backPoint.transform.position, t);
            Vector3 p1 = QuatraticLerp(from.forwardPoint.transform.position, to.backPoint.transform.position, to.transform.position, t);
            return Vector3.Lerp(p0, p1, t);
        }

        public void DestroyPath()
        {
            foreach(PathAnchor point in points)
            {
                GameObject.Destroy(point.gameObject);
            }

            points.Clear();
        }

        public void SetGizmosEnabled(bool enabled)
        {
            foreach(PathAnchor point in points)
            {
                point.drawGizmos = enabled;
                point.rotationPoint.drawGizmos = enabled;
                point.speedPoint.drawGizmos = enabled;
                point.forwardPoint.drawGizmos = enabled;
                point.backPoint.drawGizmos = enabled;

                point.buttonPoint.gameObject.SetActive(enabled);
                point.rotationPoint.buttonPoint.gameObject.SetActive(enabled);
                point.speedPoint.buttonPoint.gameObject.SetActive(enabled);
                point.forwardPoint.buttonPoint.gameObject.SetActive(enabled);
                point.backPoint.buttonPoint.gameObject.SetActive(enabled);
            }
        }
    }

}
