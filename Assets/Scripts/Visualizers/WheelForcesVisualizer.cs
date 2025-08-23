using Sirenix.Utilities;
using System.Collections.Generic;
using UnityEngine;
using vc;
using vc.VehicleComponent;

namespace VC
{
    public class WheelForcesVisualizer : MonoBehaviour, IToggle
    {
        [Header("?Settings")]
        [SerializeField] bool ShowSpringCompression=false;

        [Header("Configuration")]
        [SerializeField] Transform[] hideBodyParts;
        [SerializeField] MeshRenderer frontRightMeshRenderer;
        [SerializeField] MeshRenderer frontLeftMeshRenderer;
        [SerializeField] MeshRenderer rearRightMeshRenderer;
        [SerializeField] MeshRenderer rearLeftMeshRenderer;
        [SerializeField] MeshRenderer leftFrontNormalForceCylinder;
        [SerializeField] MeshRenderer rightFrontNormalForceCylinder;
        [SerializeField] MeshRenderer leftBackNormalForceCylinder;
        [SerializeField] MeshRenderer rightBackNormalForceCylinder;


        Dictionary<WheelID, WheelVisualData> wheelData;


        [SerializeField] float maximumNormalForce = 0f;
        private class WheelVisualData
        {
            public WheelHitData wheelData;
            public Vector3 previousLocalScale = default;
            public Material material;
            public MeshRenderer meshRenderer;

            public MeshRenderer normalForceMeshRenderer;
            public Material normalForceMaterial;
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

            leftFrontNormalForceCylinder.enabled = false;
            rightFrontNormalForceCylinder.enabled = false;
            leftBackNormalForceCylinder.enabled = false;
            rightBackNormalForceCylinder.enabled = false;


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


            // Update Lateral and Longitudinal Forces 
            var visualData = wheelData[id];
            var data = visualData.wheelData;
            // normalize the forces
            var localScale = new Vector3(
                data.combinedSlip.y * data.normalforce, // X - lateral  forces - left right
                data.normalforce,                       // Y - normal forces - up down
                data.combinedSlip.x * data.normalforce  // z - longitudinal force - forward back
                ).normalized;

            visualData.previousLocalScale = Vector3.Lerp(visualData.previousLocalScale, localScale, Time.deltaTime);

            // Position the visual for longitudinal and lateral forces accounting for the movement of the wheel
            visualData.meshRenderer.transform.position = data.hitInfo.point + data.suspensionMountPoint.up * 0.05f;

            // Set the shader parameters
            visualData.material.SetVector("_ForcePosition", new Vector4(localScale.x, localScale.z));

            // Update shader for Normal Forces visual
            visualData.normalForceMaterial.SetFloat("_ForcePercentage",
                ShowSpringCompression?data.suspension.springCompression: data.normalforce / maximumNormalForce);
        }
        MeshRenderer GetMeshRenderer(WheelID id)
        {
            switch (id)
            {
                case WheelID.LeftRear: return rearLeftMeshRenderer;
                case WheelID.RightRear: return rearRightMeshRenderer;
                case WheelID.RightFront: return frontRightMeshRenderer;
                case WheelID.LeftFront: return frontLeftMeshRenderer;
                default:return null;
            }
            //if (id == WheelID.LeftFront) return frontLeftMeshRenderer;
            //if (id == WheelID.RightFront) return frontRightMeshRenderer;
            //if (id == WheelID.LeftRear) return rearLeftMeshRenderer;
            //if (id == WheelID.RightRear) return rearRightMeshRenderer;
            //return null;
        }

        MeshRenderer GetCylinderMeshRenderer(WheelID id)
        {
            switch(id)
            {
                case WheelID.LeftRear: return leftBackNormalForceCylinder;
                case WheelID.RightRear: return rightBackNormalForceCylinder;
                case WheelID.LeftFront: return leftFrontNormalForceCylinder;
                case WheelID.RightFront: return rightFrontNormalForceCylinder;
                default: return null;
            }
        }

        void WheelComponent_onVisualWheelUpdate(WheelID id, WheelHitData data)
        {
            if (!wheelData.ContainsKey(id))
            {
                var wheelMeshRenderer = GetMeshRenderer(id);
                var wheelNormalForceMeshRenderer = GetCylinderMeshRenderer(id);

                wheelData.Add(id, new WheelVisualData
                {
                    wheelData = data,
                    previousLocalScale = new Vector3(),
                    meshRenderer = wheelMeshRenderer,
                    material = wheelMeshRenderer.material,
                    normalForceMeshRenderer = wheelNormalForceMeshRenderer,
                    normalForceMaterial = wheelNormalForceMeshRenderer.material,
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

            hideBodyParts.ForEach( p => p.gameObject.SetActive(!p.gameObject.activeSelf));

            wheelData.ForEach(wheel =>
            {
                wheel.Value.meshRenderer.enabled = !wheel.Value.meshRenderer.enabled;
                wheel.Value.normalForceMeshRenderer.enabled = !wheel.Value.normalForceMeshRenderer.enabled;
            });
        }
    }
}