using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flamenguistMission
{
    public class Enemy
    {
        // Collections
        HashSet<Model> modelCollection;
        List<Ped> pedCollection;
        List<Vehicle> vehicleCollection;
        List<TaskInvoker> taskCollection;
        List<WeaponHash> weaponCollection;

        Vector3 enemyVehicleSpawnLocation = new Vector3(-78.88372f, -1719.535f, 29.24866f);
        Vector3 enemyVehicleDestinationLocation = new Vector3(28.59011f, -1875.178f, 22.91966f);
        public Enemy(HashSet<Model> modelCollection, List<Ped> pedCollection, List<Vehicle> vehicleCollection, 
            List<TaskInvoker> taskCollection, List<WeaponHash> weaponCollection)
        {
            this.modelCollection = modelCollection;
            this.pedCollection = pedCollection;
            this.vehicleCollection = vehicleCollection;
            this.taskCollection = taskCollection;
            this.weaponCollection = weaponCollection;
        }
        public void EnemyPedConfiguration(BlipSprite blipSprite, BlipColor blipColor, int ammoCount, bool isEnemy)
        {
            foreach (var ped in pedCollection)
            {
                ped.IsEnemy = isEnemy;
                ped.AddBlip();
                ped.AttachedBlip.Sprite = blipSprite;
                ped.AttachedBlip.Color = blipColor;

                for (int i = 0; i < weaponCollection.Count; i++)
                {
                    Random rand = new Random();
                    int index = rand.Next(weaponCollection.Count);
                    ped.Weapons.Give(weaponCollection[index], ammoCount, true, true);
                }
            }
        }
        public void EnemyVehicleConfiguration(float dirtLevel, Color primaryColor, string licensePlate)
        {
            foreach (var vehicle in vehicleCollection)
            {
                vehicle.DirtLevel = dirtLevel;
                vehicle.Mods.CustomPrimaryColor = primaryColor;
                vehicle.Mods.LicensePlate = licensePlate;
            }
        }
        public void CreateMultiplesModels()
        {
            foreach (var model in modelCollection)
            {
                if (model.IsPed)
                {
                    var ped = World.CreatePed(model, enemyVehicleSpawnLocation += Vector3.RandomXY());
                    if (ped != null)
                    {
                        pedCollection.Add(ped);
                    }
                }
                else if (model.IsVehicle)
                {
                    var vehicle = World.CreateVehicle(model, enemyVehicleSpawnLocation += Vector3.RandomXY().Around(4f) * 2f); // no heading
                    if (vehicle != null)
                    {
                        vehicleCollection.Add(vehicle);
                    }
                }
            }
        }
    }
}

