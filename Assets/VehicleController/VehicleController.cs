using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using vc.VehicleComponent;
using vc.VehicleComponentsSO;

namespace vc
{
    public class VehicleController : MonoBehaviour
    {
        [Title("Configuration")]
        [SerializeField]
        VehicleConfiguration.WheelConfigData[] wheelConfig;
        [SerializeField,Space]
        VehicleConfiguration.SuspensionConfigData[] suspensionConfig;

        Dictionary<ComponentTypes, List<IVehicleComponent>> components;

        Rigidbody rb;
        public void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("Missing rigid body!");
            }

            SetupVehicle();
        }

        public void Update()
        {            
        }
        private void FixedUpdate()
        {
            foreach(var susp in components[ComponentTypes.Suspension])
            {
                susp.Update(Time.fixedDeltaTime);
            }
        }
        #region Vehicle
        private void SetupVehicle()
        {
            components = new();

            // wheels 
            Dictionary<WheelID, WheelComponent> wheels = new();
            List<IVehicleComponent> wheelComponents = new();
            foreach (var wc in wheelConfig) 
            {                
                var w = new WheelComponent(wc.Config, wc.ID);
                wheels.Add(w.id, w);
                wheelComponents.Add(w);
            }

            components.Add(ComponentTypes.Wheel, wheelComponents);

            // Setup Suspensions
            List<IVehicleComponent> suspensionComponents = new();
            foreach (var susp in suspensionConfig)
            {                
                suspensionComponents.Add(new SuspensionComponent(susp.Config, wheels[susp.ID],susp.mountPoint,rb));
            }
            components.Add(ComponentTypes.Suspension, suspensionComponents);
        }
        #endregion Vehicle


        #region DebugInformation

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            foreach (IDebugInformation c in components[ComponentTypes.Suspension])
            {
                c.DrawGizmos();
            }
            foreach (IDebugInformation c in components[ComponentTypes.Wheel])
            {
                c.DrawGizmos();
            }
        }

        private void OnGUI()
        {
            float yOffset = 10f;
            float yStep = 20f;
            float xPos = 10f;
            foreach (IDebugInformation c in components[ComponentTypes.Suspension]) 
            {
                yOffset = c.OnGUI(xPos, yOffset, yStep);
            }
        }
        #endregion
    }

    namespace VehicleConfiguration
    {
        [System.Serializable]
        public class WheelConfigData
        {            
            [HorizontalGroup("Config")]
            public WheelID ID;
            [HorizontalGroup("Config")]
            public WheelSO Config;

            [HorizontalGroup("Physical")]
            public Transform mesh; 
        }

        [System.Serializable]
        public class SuspensionConfigData
        {
            [HorizontalGroup("Config")]
            public WheelID ID;
            [HorizontalGroup("Config")]
            public SuspensionSO Config;

            [HorizontalGroup("Physical")]
            public Transform mountPoint;
        }
    }
}