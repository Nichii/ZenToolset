using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZenToolset
{
    public class BillboardToCamera : MonoBehaviour
    {
        [SerializeField] private Camera targetCamera;
        
        private Transform lookAtTransform = null;

        public void ChangeTargetCamera(Camera targetCamera)
        {
            this.targetCamera = targetCamera;

            if (!targetCamera)
            {
                lookAtTransform = Camera.main.transform;
            }
            else
            {
                lookAtTransform = targetCamera.transform;
            }
        }

        private void Start()
        {
            ChangeTargetCamera(targetCamera);
        }

        private void LateUpdate()
        {
            transform.LookAt(transform.position + lookAtTransform.rotation * Vector3.forward, lookAtTransform.rotation * Vector3.up);
        }
    }
}
