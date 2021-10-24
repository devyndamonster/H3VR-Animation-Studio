using H3VRAnimator.Logging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace H3VRAnimator
{
    //[ExecuteInEditMode]
    public class CameraEffectScript : MonoBehaviour
    {
        public Material mat;

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (mat != null)
            {
                Graphics.Blit(src, dest, mat);
            }
            else
            {
                Graphics.Blit(src, dest);
            }
        }
    }
}


