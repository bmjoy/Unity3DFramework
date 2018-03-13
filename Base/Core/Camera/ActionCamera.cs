﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Framework
{
    [RequireComponent(typeof(Camera))]
    public class ActionCamera : CameraBase
    {
        [Header("Debug")]
        public bool InitOnStart;
        public bool DrawDebug;

        [Header("Target")]
        public string TargetTag = "Player";
        public Transform Target;
        public Transform LockOn;

        [Header("Input")]
        public string LookX = "Look X";
        public string LookY = "Look Y";

        [Header("Collision")]
        public LayerMask Mask;

        [Header("Offset")]
        [FormerlySerializedAs("Offset")]
        public Vector3 RotatedOffset = new Vector3(0, 0, -4);
        public Vector3 FlatOffset = Vector3.zero;
        
        [Header("Speed")]
        public float SpeedX = 2;
        public float SpeedY = 2;

        [Header("Angles")]
        public float MaxAngleY =  45;
        public float MinAngleY = -45;

        [HideInInspector]
        public float AngleX;

        [HideInInspector]
        public float AngleY;

        void Start()
        {
            if (!InitOnStart)
                return;

            Init();
        }

        public void Init()
        {
            if (Target != null)
                return;

            var go = GameObject.FindGameObjectWithTag(TargetTag);
            Target = go ? go.transform : null;

            AngleX = 0;
            AngleY = 0;
        }

        void LateUpdate()
        {
            OnUpdate();
        }

        public void SetLockOn(Transform lockOn)
        {
            LockOn = lockOn;
        }

        public void OnUpdate()
        {
            var rot = Quaternion.identity;

            if (LockOn)
            {
                var distance = transform.position - LockOn.position;
                var rawRot = Quaternion.LookRotation(-distance.normalized).eulerAngles;
                rot = Quaternion.Euler(rawRot.x, rawRot.y, rawRot.z);

                AngleY = rawRot.x;
                AngleX = rawRot.y;
            }
            else
            {
                AngleX += Input.GetAxis(LookX) * SpeedX;
                AngleY -= Input.GetAxis(LookY) * SpeedY;

                AngleY = Mathf.Min(Mathf.Max(MinAngleY, AngleY), MaxAngleY);

                rot = Quaternion.Euler(AngleY, AngleX, 0);
            }
            
            var offset = new Vector3(RotatedOffset.x, RotatedOffset.y, CalcCameraDistance());
            var pos = rot * offset + (
                Target ? Target.position : Vector3.zero
            ) + FlatOffset;

            transform.position = pos;
            transform.rotation = rot; //Quaternion.RotateTowards(transform.rotation, rot, 360 * Time.deltaTime);
        }

        public float CalcCameraDistance()
        {
            if (!Target)
                return RotatedOffset.z;

            RaycastHit hit;
            Vector3    offset = new Vector3(0, RotatedOffset.y, 0);

            if (!Physics.Raycast(Target.position + offset, -transform.forward, out hit, Mathf.Abs(RotatedOffset.z), Mask))
                return RotatedOffset.z;

            if (DrawDebug)
                Debug.DrawRay(Target.position + offset, -transform.forward * hit.distance, Color.red, 5.0f);

            return -hit.distance;
        }
    }
}