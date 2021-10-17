using FistVR;
using H3VRAnimator.Utilities;
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
        public bool isBezier = true;
        public int bezierMidPoints = 10;


        public void AddPoint(Vector3 position)
        {
            GameObject anchorObject = new GameObject("MovablePoint");
            anchorObject.transform.position = position;

            FVRViveHand otherHand = AnimationUtils.GetNonPointingHand();
            if (otherHand.CurrentInteractable != null && otherHand.CurrentInteractable.GetComponent<SpectatorPanel>() == null)
            {
                anchorObject.transform.SetParent(otherHand.CurrentInteractable.transform);
            }

            PathAnchor anchorComp = anchorObject.AddComponent<PathAnchor>();
            points.Add(anchorComp);



            //Set bezier curve points visibility
            if (points.Count == 1 || !isBezier)
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
            if (isBezier)
            {
                for (int i = 1; i < points.Count; i++)
                {
                    PathAnchor from = points[i - 1];
                    PathAnchor to = points[i];

                    Vector3 prevPoint = from.transform.position;

                    for (int j = 1; j <= bezierMidPoints; j++)
                    {
                        float t = ((float)j) / bezierMidPoints;

                        Vector3 nextPoint = GetLerp(from, to, t);

                        Popcron.Gizmos.Line(prevPoint, nextPoint, Color.red);

                        prevPoint = nextPoint;
                    }
                }
            }

            else
            {
                for (int i = 1; i < points.Count; i++)
                {
                    Popcron.Gizmos.Line(points[i - 1].transform.position, points[i].transform.position, Color.red);
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


        public Vector3 GetLerp(PathAnchor from, PathAnchor to, float t)
        {
            if (isBezier)
            {
                Vector3 p0 = QuatraticLerp(from.transform.position, from.forwardPoint.transform.position, to.backPoint.transform.position, t);
                Vector3 p1 = QuatraticLerp(from.forwardPoint.transform.position, to.backPoint.transform.position, to.transform.position, t);
                return Vector3.Lerp(p0, p1, t);
            }
            else
            {
                return Vector3.Lerp(from.transform.position, to.transform.position, t);
            }
        }


        public void DestroyPath()
        {
            foreach(PathAnchor point in points)
            {
                GameObject.Destroy(point.gameObject);
            }

            points.Clear();
        }


        public void ToggleBezier()
        {
            isBezier = !isBezier;

            for (int i = 0; i < points.Count; i++)
            {
                points[i].forwardPoint.buttonPoint.gameObject.SetActive(isBezier && i < points.Count - 1);
                points[i].backPoint.buttonPoint.gameObject.SetActive(isBezier && i > 0);
            }
        }


        public void SetGizmosEnabled(bool enabled)
        {
            for(int i = 0; i < points.Count; i++)
            {
                points[i].drawGizmos = enabled;
                points[i].rotationPoint.drawGizmos = enabled;
                points[i].speedPoint.drawGizmos = enabled;
                points[i].forwardPoint.drawGizmos = enabled;
                points[i].backPoint.drawGizmos = enabled;

                points[i].buttonPoint.gameObject.SetActive(enabled);
                points[i].rotationPoint.buttonPoint.gameObject.SetActive(enabled);
                points[i].speedPoint.buttonPoint.gameObject.SetActive(enabled);

                //Control points have special cases for if they're on the end or not
                points[i].forwardPoint.buttonPoint.gameObject.SetActive(enabled && i < points.Count - 1);
                points[i].backPoint.buttonPoint.gameObject.SetActive(enabled && i > 0);
            }
        }
    }

}
