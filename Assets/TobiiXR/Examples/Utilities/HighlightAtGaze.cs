// Copyright © 2018 – Property of Tobii AB (publ) - All Rights Reserved

using Tobii.G2OM;
using UnityEngine;
using System.Collections;
namespace Tobii.XR.Examples
{
//Monobehaviour which implements the "IGazeFocusable" interface, meaning it will be called on when the object receives focus
    public class HighlightAtGaze : MonoBehaviour, IGazeFocusable
    {
        public Color HighlightColor = Color.red;
        public float AnimationTime = 0.1f;

        public Vector3 currPos, currPosReal, prevPos, prevPosReal;
        public Vector3 currDir, currDirReal, prevDir, prevDirReal;

        public Transform headTransform;
        private float simulatedTime = 0;

        private Renderer _renderer;
        private Color _originalColor;
        private Color _targetColor;

        //The method of the "IGazeFocusable" interface, which will be called when this object receives or loses focus
        public void GazeFocusChanged(bool hasFocus)
        {
            //If this object received focus, fade the object's color to highlight color
            if (hasFocus)
            {
                _targetColor = HighlightColor;
            }
            //If this object lost focus, fade the object's color to it's original color
            else
            {
                _targetColor = _originalColor;
            }
        }

        public static Vector3 FlattenedPos3D(Vector3 vec, float height = 0)
        {
            return new Vector3(vec.x, height, vec.z);
        }

        public static Vector2 FlattenedPos2D(Vector3 vec)
        {
            return new Vector2(vec.x, vec.z);
        }

        public static Vector3 FlattenedDir3D(Vector3 vec)
        {
            return (new Vector3(vec.x, 0, vec.z)).normalized;
        }

        public static Vector2 FlattenedDir2D(Vector3 vec)
        {
            return new Vector2(vec.x, vec.z).normalized;
        }

        public static Vector3 UnFlatten(Vector2 vec, float height = 0)
        {
            return new Vector3(vec.x, height, vec.y);
        }
        public static Vector3 GetRelativePosition(Vector3 pos, Transform origin)
        {
            return Quaternion.Inverse(origin.rotation) * (pos - origin.position);
        }

        public static Vector3 GetRelativeDirection(Vector3 dir, Transform origin)
        {
            return Quaternion.Inverse(origin.rotation) * dir;
        }

        /*
        void UpdateCurrentUserState()
        {
            currPos = FlattenedPos3D(headTransform.position);
            currPosReal = GetRelativePosition(currPos, this.transform);
            currDir = FlattenedDir3D(headTransform.forward);
            currDirReal = FlattenedDir3D(GetRelativeDirection(currDir, this.transform));
        }

        void UpdatePreviousUserState()
        {
            prevPos = FlattenedPos3D(headTransform.position);
            prevPosReal = GetRelativePosition(prevPos, this.transform);
            prevDir = FlattenedDir3D(headTransform.forward);
            prevDirReal = FlattenedDir3D(GetRelativeDirection(prevDir, this.transform));
        }
        */
        /*void CalculateStateChanges()
        {
            deltaPos = currPos - prevPos;
            deltaDir = Utilities.GetSignedAngle(prevDir, currDir);
        }*/


        private void Start()
        {
            _renderer = GetComponent<Renderer>();
            _originalColor = _renderer.material.color;
            _targetColor = _originalColor;

            simulatedTime = 0;
            //UpdatePreviousUserState();


        }

        private void Update()
        {

            simulatedTime += 1.0f / 60f;
            //UpdateCurrentUserState();
            //UpdatePreviousUserState();
            //CalculateStateChanges();

            //This lerp will fade the color of the object
            if (_renderer.material.HasProperty(Shader.PropertyToID("_BaseColor"))) // new rendering pipeline (lightweight, hd, universal...)
            {
                _renderer.material.SetColor("_BaseColor", Color.Lerp(_renderer.material.GetColor("_BaseColor"), _targetColor, Time.deltaTime * (1 / AnimationTime)));




            }
            else // old standard rendering pipline
            {
                _renderer.material.color = Color.Lerp(_renderer.material.color, _targetColor, Time.deltaTime * (1 / AnimationTime));

                var eyeTrackingData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);
                // Check if gaze ray is valid
                if (eyeTrackingData.GazeRay.IsValid)
                {
                    // The origin of the gaze ray is a 3D point
                    var rayOrigin = eyeTrackingData.GazeRay.Origin;

                    // The direction of the gaze ray is a normalized direction vector
                    var rayDirection = eyeTrackingData.GazeRay.Direction;

                    // For social use cases, data in local space may be easier to work with
                    var eyeTrackingDataLocal = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.Local);

                    // The EyeBlinking bool is true when the eye is closed
                    var isLeftEyeBlinking = eyeTrackingDataLocal.IsLeftEyeBlinking;
                    var isRightEyeBlinking = eyeTrackingDataLocal.IsRightEyeBlinking;

                    // Using gaze direction in local space makes it easier to apply a local rotation
                    // to your virtual eye balls.
                    var eyesDirection = eyeTrackingDataLocal.GazeRay.Direction;


                    //Debug.Log("rayOrigin : " + rayOrigin);
                    //Debug.Log("raydirectOrigin : " + rayDirection);
                    //Debug.Log("left : " + isLeftEyeBlinking);
                }



            }
        }
    }
}
