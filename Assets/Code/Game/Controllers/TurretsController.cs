using Code.Game.Data.Vehicles;
using Code.Game.Models;
using Code.Game.Views;
using Photon.Pun;
using UnityEngine;

namespace Code.Game.Controllers
{
    public class TurretsController
    {
        public TurretModel Install(VehicleModel vehicleModel, VehicleTurretRack vehicleTurretRack, TurretData turretData)
        {
            var turret = PhotonNetwork.Instantiate(
                turretData.TurretPrefabPath, 
                vehicleTurretRack.TurretInstallPoint.position, vehicleTurretRack.TurretInstallPoint.rotation
            );
            
            turret.transform.parent = vehicleTurretRack.TurretInstallPoint;

            var turretView = turret.GetComponent<TurretView>();

            var turretModel = new TurretModel(turretData, turretView)
            {
                AmmoCount = turretData.MaxAmmoCount,
            };
            
            vehicleModel.Turrets[vehicleTurretRack] = turretModel;

            return turretModel;
        }
        
        public void Remove(VehicleModel vehicleModel, VehicleTurretRack vehicleTurretRack)
        {
            if (!vehicleModel.Turrets.TryGetValue(vehicleTurretRack, out var turretModel))
                return;
            
            PhotonNetwork.Destroy(turretModel.TurretView.gameObject);
            vehicleModel.Turrets[vehicleTurretRack] = null;
        }
        
        public void UpdateVehicleTurretRotationInput(VehicleModel vehicleModel, Transform camera)
        {
            foreach (var (vehicleTurretRack, turretModel) in vehicleModel.Turrets)
            {
                if (turretModel == null)
                    continue;
                
                var stand = turretModel.TurretView.Stand;
                var gun = turretModel.TurretView.Gun;

                var oldGunRotation = gun.rotation;
                var newGunRotation = Quaternion.Lerp(oldGunRotation, camera.rotation, turretModel.TurretData.GunRotationSpeed);
                oldGunRotation.Set(newGunRotation.x, newGunRotation.y, newGunRotation.z, newGunRotation.w);
                gun.rotation = oldGunRotation;

                var oldStandRotation = stand.rotation;
                oldStandRotation.Set(0, newGunRotation.y, 0, oldStandRotation.w);
                stand.rotation = oldStandRotation;
            }
        }
        
        public void UpdateShootDelay(VehicleModel vehicleModel)
        {
            foreach (var (vehicleTurretRack, turretModel) in vehicleModel.Turrets)
            {
                if (turretModel == null)
                    continue;
                
                if (turretModel.ShootDelayTime >= -1)
                {
                    turretModel.ShootDelayTime -= Time.deltaTime;
                }
            }
        }

        public void Shoot(VehicleModel vehicleModel)
        {
            foreach (var (vehicleTurretRack, turretModel) in vehicleModel.Turrets)
            {
                if (turretModel == null)
                    continue;
                
                if (turretModel.ShootDelayTime <= 0)
                {
                    turretModel.TurretView.AudioSource.PlayOneShot(turretModel.TurretData.ShootingSound);
                    
                    var barrelPosition = turretModel.TurretView.ShootPoint.position;
                    var barrelForward = turretModel.TurretView.ShootPoint.forward;

                    turretModel.TurretView.ShootEffect.Play();

#if UNITY_EDITOR
                    Debug.DrawRay(barrelPosition, barrelForward, Color.red);
#endif
                    
                    if (Physics.Raycast(barrelPosition, barrelForward, out var raycastHit, turretModel.TurretData.ShootMaxDistance))
                    {
                        if (vehicleModel.VehicleView.gameObject.GetInstanceID() != raycastHit.collider.gameObject.GetInstanceID())
                        {
                            if (raycastHit.collider.gameObject.TryGetComponent<VehicleView>(out var enemyVehicleView))
                            {
                                enemyVehicleView.AddDamage(vehicleModel.VehicleView, turretModel.TurretData.Damage);
                            }
                        }
                    }

                    turretModel.ShootDelayTime = turretModel.TurretData.ShootingDelaySeconds;
                }
            }
        }
    }
}