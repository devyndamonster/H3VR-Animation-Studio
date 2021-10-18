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
        public List<AnimatedPoint> animations = new List<AnimatedPoint>();
        public bool isBezier = true;
        public bool isContinuous = false;
        public bool onlyDrawPoints = false;
        public bool drawRotation = false;
        public int bezierMidPoints = 10;
        public bool drawGizmos = true;



        public void Animate()
        {
            foreach (AnimatedPoint point in animations)
            {
                point.Animate();
            }
        }


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
            UpdateControlPoints();
        }



        public void DrawPath()
        {
            if (!drawGizmos) return;

            if (isBezier)
            {
                //Draw the points from beginning to end
                for (int i = 1; i < points.Count; i++)
                {
                    PathAnchor from = points[i - 1];
                    PathAnchor to = points[i];

                    DrawBezierCurve(from, to);
                    if(drawRotation)DrawRotation(from, to);
                }

                //If this is continuous, also draw from end to beginning
                if (isContinuous)
                {
                    PathAnchor from = points[points.Count - 1];
                    PathAnchor to = points[0];

                    DrawBezierCurve(from, to);
                    if (drawRotation) DrawRotation(from, to);
                }
            }

            else
            {
                for (int i = 1; i < points.Count; i++)
                {
                    Popcron.Gizmos.Line(points[i - 1].transform.position, points[i].transform.position, Color.red);
                }

                if (isContinuous)
                {
                    Popcron.Gizmos.Line(points[points.Count - 1].transform.position, points[0].transform.position, Color.red);
                }
            }
        }



        private void DrawBezierCurve(PathAnchor from, PathAnchor to)
        {
            Vector3 prevPoint = from.transform.position;

            for (int j = 1; j <= bezierMidPoints; j++)
            {
                float t = ((float)j) / bezierMidPoints;

                Vector3 nextPoint = GetLerpPosition(from, to, t);

                if (onlyDrawPoints)
                {
                    Popcron.Gizmos.Sphere(nextPoint, .005f,  Color.red);
                }
                else
                {
                    Popcron.Gizmos.Line(prevPoint, nextPoint, Color.red);
                }

                prevPoint = nextPoint;
            }
        }


        private void DrawRotation(PathAnchor from, PathAnchor to)
        {
            for (int j = 1; j <= bezierMidPoints; j++)
            {
                float t = ((float)j) / bezierMidPoints;

                Vector3 pos = GetLerpPosition(from, to, t);
                Quaternion rot = GetLerpRotation(from, to, t);

                Popcron.Gizmos.Line(pos, pos + rot * Vector3.forward * 0.2f, Color.red);
            }
        }


        //Concepts based on sebastion lague video: https://www.youtube.com/watch?v=RF04Fi9OCPc
        private Vector3 QuadraticLerpPosition(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            Vector3 p0 = Vector3.Lerp(a, b, t);
            Vector3 p1 = Vector3.Lerp(b, c, t);
            return Vector3.Lerp(p0, p1, t);
        }

        private Quaternion QuadraticLerpRotation(Quaternion a, Quaternion b, Quaternion c, float t)
        {
            Quaternion r0 = Quaternion.Lerp(a, b, t);
            Quaternion r1 = Quaternion.Lerp(b, c, t);
            return Quaternion.Slerp(r0, r1, t);
        }


        public Vector3 GetLerpPosition(PathAnchor from, PathAnchor to, float t)
        {
            if (isBezier)
            {
                Vector3 p0 = QuadraticLerpPosition(from.transform.position, from.forwardPoint.transform.position, to.backPoint.transform.position, t);
                Vector3 p1 = QuadraticLerpPosition(from.forwardPoint.transform.position, to.backPoint.transform.position, to.transform.position, t);
                return Vector3.Lerp(p0, p1, t);
            }
            else
            {
                return Vector3.Lerp(from.transform.position, to.transform.position, t);
            }
        }


        public Quaternion GetLerpRotation(PathAnchor from, PathAnchor to, float t)
        {
            if (isBezier)
            {
                Quaternion r0 = QuadraticLerpRotation(from.rotationPoint.transform.rotation, from.forwardRotationPoint.transform.rotation, to.backRotationPoint.transform.rotation, t);
                Quaternion r1 = QuadraticLerpRotation(from.forwardRotationPoint.transform.rotation, to.backRotationPoint.transform.rotation, to.rotationPoint.transform.rotation, t);
                return Quaternion.Slerp(r0, r1, t);
            }
            else
            {
                return Quaternion.Slerp(from.rotationPoint.transform.rotation, to.rotationPoint.transform.rotation, t);
            }
        }

        public float GetDistanceBetweenPoints(PathAnchor from, PathAnchor to)
        {
            if (isBezier)
            {
                //Approximate curve length
                float controlPointDist = Vector3.Distance(from.transform.position, from.forwardPoint.transform.position) + Vector3.Distance(from.forwardPoint.transform.position, to.backPoint.transform.position) + Vector3.Distance(to.backPoint.transform.position, to.transform.position);
                return Vector3.Distance(from.transform.position, to.transform.position) + controlPointDist / 2;
            }
            else
            {
                return Vector3.Distance(from.transform.position, to.transform.position);
            }
        }


        public void DestroyPath()
        {
            foreach(PathAnchor point in points)
            {
                GameObject.Destroy(point.gameObject);
            }

            foreach(AnimatedPoint animation in animations)
            {
                GameObject.Destroy(animation.gameObject);
            }

            points.Clear();
            animations.Clear();
        }



        public void ToggleBezier()
        {
            isBezier = !isBezier;

            UpdateControlPoints();
        }

        public void ToggleContinuous()
        {
            isContinuous = !isContinuous;

            UpdateControlPoints();
        }

        public void ToggleLineMode()
        {
            onlyDrawPoints = !onlyDrawPoints;
        }

        public void ToggleDrawRotation()
        {
            drawRotation = !drawRotation;
        }


        private void UpdateControlPoints()
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i].forwardPoint.drawGizmos = drawGizmos && isBezier && (i < points.Count - 1 || isContinuous);
                points[i].backPoint.drawGizmos = drawGizmos && isBezier && (i > 0 || isContinuous);
                points[i].forwardPoint.buttonPoint.gameObject.SetActive(drawGizmos && isBezier && (i < points.Count - 1 || isContinuous));
                points[i].backPoint.buttonPoint.gameObject.SetActive(drawGizmos && isBezier && (i > 0 || isContinuous));

                points[i].forwardRotationPoint.drawGizmos = drawGizmos && isBezier && (i < points.Count - 1 || isContinuous);
                points[i].backRotationPoint.drawGizmos = drawGizmos && isBezier && (i > 0 || isContinuous);
                points[i].forwardRotationPoint.buttonPoint.gameObject.SetActive(drawGizmos && isBezier && (i < points.Count - 1 || isContinuous));
                points[i].backRotationPoint.buttonPoint.gameObject.SetActive(drawGizmos && isBezier && (i > 0 || isContinuous));
            }
        }


        public void SetGizmosEnabled(bool enabled)
        {
            drawGizmos = enabled;

            for(int i = 0; i < points.Count; i++)
            {
                points[i].drawGizmos = enabled;
                points[i].rotationPoint.drawGizmos = enabled;
                points[i].speedPoint.drawGizmos = enabled;

                points[i].buttonPoint.gameObject.SetActive(enabled);
                points[i].rotationPoint.buttonPoint.gameObject.SetActive(enabled);
                points[i].speedPoint.buttonPoint.gameObject.SetActive(enabled);

                //Control points have special cases for if they're on the end or not
                UpdateControlPoints();
            }


            foreach (AnimatedPoint point in animations)
            {
                point.drawGizmos = enabled;
            }
        }
    }

}
