using Code.Game.Data.Vehicles;
using Code.Game.Models;
using Code.Game.Views;
using Photon.Pun;
using UnityEngine;

namespace Code.Game.Controllers
{
    public class VehicleController
    {
        public VehicleModel Spawn(Vector3 spawnPosition, VehicleData vehicleData)
        {
            var vehicle = PhotonNetwork.Instantiate(vehicleData.VehiclePrefabPath, spawnPosition, Quaternion.identity);

            var vehicleView = vehicle.GetComponent<VehicleView>();

            var vehicleModel = new VehicleModel();
            vehicleModel.SetVehicle(vehicleView, vehicleData);
            vehicleModel.VehicleView.AudioSource.clip = vehicleData.EngineSound;
            vehicleModel.VehicleView.SetDisplayHealth(vehicleModel.Health);

            return vehicleModel;
        }

        public void UpdateVehicleMovementInput(VehicleModel vehicleModel, float torqueAxis, float steerAxis, bool backwardsWheelsInvert)
        {
            var speed = vehicleModel.Speed;
            if (torqueAxis > 0) {
                // Accelerating
                speed = torqueAxis * vehicleModel.VehicleData.Acceleration;
            }
            
            if (torqueAxis < 0) {
                if (speed > 0) {
                    // Braking
                    speed = torqueAxis * vehicleModel.VehicleData.BrakeSpeed;
                } else {
                    // Reversing
                    speed = torqueAxis * vehicleModel.VehicleData.ReverseSpeed;
                }
            }

            if (torqueAxis == 0) {
                // Not accelerating or braking
                if (speed > 0 || speed < 0) {
                    speed = 0;
                }
            }
            
            vehicleModel.Speed = Mathf.Clamp(speed, vehicleModel.VehicleData.SpeedMin, vehicleModel.VehicleData.SpeedMax);
            vehicleModel.VehicleView.AudioSource.pitch = vehicleModel.Speed / (vehicleModel.VehicleData.SpeedMax / 1) + 0.1f;

            if (!vehicleModel.VehicleView.AudioSource.isPlaying)
            {
                vehicleModel.VehicleView.AudioSource.Play();
            }

            if (backwardsWheelsInvert && vehicleModel.Speed < 0) {
                // Going backwards, invert wheels
                steerAxis *= -1f;
            }
            
            foreach (var wheelsAxis in vehicleModel.VehicleView.WheelsAxis)
            {
                if (!wheelsAxis.IsMotor || !wheelsAxis.IsSteering)
                    continue;

                foreach (var wheel in wheelsAxis.Wheels)
                {
                    if (wheelsAxis.IsMotor)
                    {
                        wheel.motorTorque = vehicleModel.Speed * vehicleModel.VehicleData.MotorTorque;
                    }

                    if (wheelsAxis.IsSteering)
                    {
                        wheel.steerAngle = steerAxis * vehicleModel.VehicleData.SteerAngle;
                    }
                }
            }
        }
    }
}