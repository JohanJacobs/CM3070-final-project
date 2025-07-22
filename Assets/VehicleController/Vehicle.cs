using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using vc.VehicleComponent;
using vc.VehicleComponentsSO;
using vc.VehicleConfiguration;
using Sirenix.Utilities;

namespace vc
{
    public class Vehicle
    {
        public Dictionary<WheelID, SuspensionComponent> suspension;
        public Dictionary<WheelID, WheelComponent> wheels;
        public DifferentialComponent differential;
        public TransmissionComponent transmission;
        public EngineComponent engine;
        public ClutchComponent clutch;
        public BodyComponent body;
        public RollbarComponent rollbarFront;
        public RollbarComponent rollbarRear;
        public BrakeComponent brake;
        public AeroComponent aero;
        public TractionControlEngineComponent TractionControlEngine;
        public ElectronicStabilityControlComponent ElectronicSpeedController;

        public static Vehicle Setup(Rigidbody carRigidbody,
            VehicleConfiguration.WheelConfiguration[] wheelConfig,
            BodySO bodyConfig,
            DifferentialSO differentialConfig,
            TransmissionSO transmissionConfig,
            ClutchSO clutchConfig,
            EngineSO engineConfig,
            BrakeSO brakeConfig,
            AeroSO aeroConfig,
            AntiRollbarSO antiRollbarFront,
            AntiRollbarSO antiRollbarRear,
            VehicleVariablesSO vehicleVariables,
            TractionControlSO tcEngine)
        {
            Vehicle newVehicle = new();

            var wheelHitData = WheelHitData.SetupDefault(carRigidbody);

            // wheels 
            newVehicle.wheels = new();
            newVehicle.suspension = new();

            wheelConfig.ForEach(wc =>
            {

                newVehicle.wheels.Add(wc.id, new(wc.id, wc.WheelConfig, wheelHitData[wc.id], wc.wheelMesh, vehicleVariables));
                newVehicle.suspension.Add(wc.id, new SuspensionComponent(wc.SuspensionConfig, wheelHitData[wc.id], wc.suspMount, vehicleVariables));
            });

            // car body
            newVehicle.body = new(bodyConfig, carRigidbody, newVehicle.suspension[WheelID.LeftFront].mountPoint, newVehicle.suspension[WheelID.RightFront].mountPoint, vehicleVariables);
            newVehicle.body.Start();

            wheelHitData.ForEach(whd => whd.Value.body = newVehicle.body);


            newVehicle.suspension.ForEach(s => s.Value.Start());
            newVehicle.wheels.ForEach(w => w.Value.Start());

            newVehicle.rollbarFront = new(carRigidbody, antiRollbarFront, wheelHitData[WheelID.LeftFront], wheelHitData[WheelID.RightFront], vehicleVariables);
            newVehicle.rollbarFront.Start();

            newVehicle.rollbarRear = new(carRigidbody, antiRollbarRear, wheelHitData[WheelID.LeftRear], wheelHitData[WheelID.RightRear], vehicleVariables);
            newVehicle.rollbarRear.Start();

            newVehicle.differential = new(differentialConfig, 2, vehicleVariables);
            newVehicle.differential.Start();

            newVehicle.transmission = new(transmissionConfig, vehicleVariables);
            newVehicle.transmission.Start();

            newVehicle.clutch = new(clutchConfig, vehicleVariables);
            newVehicle.clutch.Start();

            newVehicle.engine = new(engineConfig, vehicleVariables);
            newVehicle.engine.Start();

            newVehicle.brake = new(brakeConfig, vehicleVariables);
            newVehicle.brake.Start();

            newVehicle.aero = new(aeroConfig, carRigidbody, vehicleVariables);
            newVehicle.aero.Start();

            newVehicle.TractionControlEngine = new(tcEngine, vehicleVariables, newVehicle.wheels);
            newVehicle.TractionControlEngine.Start();


            newVehicle.ElectronicSpeedController = new(vehicleVariables);
            newVehicle.ElectronicSpeedController.Start();
            return newVehicle;
        }


        public void SetDriveWheels(bool leftFront, bool rightFront, bool leftRear, bool rightRear)
        {
            wheels[WheelID.LeftFront].SetDriveWheel(leftFront);
            wheels[WheelID.RightFront].SetDriveWheel(rightFront);
            wheels[WheelID.LeftRear].SetDriveWheel(leftRear);
            wheels[WheelID.RightRear].SetDriveWheel(rightRear);

        }

        public static void Shutdown(Vehicle vehicle)
        {
            vehicle.body.Shutdown();
            vehicle.aero.Shutdown();
            vehicle.TractionControlEngine.Shutdown();
            vehicle.engine.Shutdown();
            vehicle.clutch.Shutdown();
            vehicle.transmission.Shutdown();
            vehicle.differential.Shutdown();
            vehicle.rollbarFront.Shutdown();
            vehicle.rollbarRear.Shutdown();
            vehicle.brake.Shutdown();
            vehicle.wheels.ForEach(w => w.Value.Shutdown());
            vehicle.suspension.ForEach(s => s.Value.Shutdown());
            vehicle.TractionControlEngine.Shutdown();
            vehicle.ElectronicSpeedController.Shutdown();
            vehicle = null;
        }
    }
}