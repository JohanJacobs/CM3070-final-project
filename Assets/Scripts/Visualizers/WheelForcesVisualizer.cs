using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using vc;
using vc.VehicleComponent;

namespace VC
{
    public class WheelForcesVisualizer : MonoBehaviour, IToggle
    {
        [SerializeField] Transform carBody;
        [SerializeField] MeshRenderer frontRightMeshRenderer;
        [SerializeField] MeshRenderer frontLeftMeshRenderer;
        [SerializeField] MeshRenderer rearRightMeshRenderer;
        [SerializeField] MeshRenderer rearLeftMeshRenderer;

        Dictionary<WheelID, WheelVisualData> wheelData;

        private class WheelVisualData
        {
            public WheelHitData wheelData;
            public Vector3 previousLocalScale = default;
            public Material material;
            public MeshRenderer meshRenderer;
        }

        private void Awake()
        {
            wheelData = new();
        }

        private void Start()
        {
            frontRightMeshRenderer.enabled = false;
            frontLeftMeshRenderer.enabled = false;
            rearRightMeshRenderer.enabled = false;
            rearLeftMeshRenderer.enabled = false;                       
        }

        private void OnEnable()
        {
            WheelComponent.onVisualWheelUpdate += WheelComponent_onVisualWheelUpdate;
        }

        private void OnDisable()
        {
            WheelComponent.onVisualWheelUpdate -= WheelComponent_onVisualWheelUpdate;
        }

        private void LateUpdate()
        {
            UpdateVisual(WheelID.RightFront);
            UpdateVisual(WheelID.LeftFront);
            UpdateVisual(WheelID.RightRear);
            UpdateVisual(WheelID.LeftRear);
        }

        void UpdateVisual(WheelID id)
        {
            if (!wheelData.ContainsKey(id)) return;

            var visualData = wheelData[id];
            var data = visualData.wheelData;

            var localScale = new Vector3(
                data.combinedSlip.y * data.normalforce, // X - lateral  forces - left right
                data.normalforce,                       // Y - normal forces - up down
                data.combinedSlip.x * data.normalforce  // z - longitudinal force - forward back
                ).normalized;

            visualData.previousLocalScale = Vector3.Lerp(visualData.previousLocalScale, localScale, Time.deltaTime);

            visualData.meshRenderer.transform.position = data.hitInfo.point + data.suspensionMountPoint.up * 0.05f;
            visualData.material.SetVector("_ForcePosition", new Vector4(localScale.x, localScale.z));
        }
        MeshRenderer GetMeshRenderer(WheelID id)
        {
            if (id == WheelID.LeftFront) return frontLeftMeshRenderer;
            if (id == WheelID.RightFront) return frontRightMeshRenderer;
            if (id == WheelID.LeftRear) return rearLeftMeshRenderer;
            if (id == WheelID.RightRear) return rearRightMeshRenderer;
            return null;
        }

        void WheelComponent_onVisualWheelUpdate(WheelID id, WheelHitData data)
        {
            if (!wheelData.ContainsKey(id))
            {
                var wheelMeshRenderer = GetMeshRenderer(id);
                wheelData.Add(id, new WheelVisualData
                {
                    wheelData = data,
                    previousLocalScale = new Vector3(),
                    meshRenderer = wheelMeshRenderer,
                    material = wheelMeshRenderer.material
                });
                return;
            }
        }

        public void Toggle()
        {            
            ToggleVisuals();
        }
        void ToggleVisuals()
        {
            carBody.gameObject.SetActive(!carBody.gameObject.activeSelf);

            wheelData.ForEach(wheel =>
            {
                wheel.Value.meshRenderer.enabled = !wheel.Value.meshRenderer.enabled;
            });
        }
    }
}