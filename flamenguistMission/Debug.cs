///
/// RelationshipGroups HASH KEYS: https://pastebin.com/xRAUSEVW
/// 
/// 
/// 
/// 
/// 
///
using GTA.UI;
using GTA;
using LemonUI.Elements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using GTA.Native;

namespace flamenguistMission
{
    public class Debug
    {
        public bool debugToggle;
        public bool trafficToggle;
        public Vehicle[] nearbyVehicles; // Mudar para privado posteriormente
        public Ped[] nearbyPeds;

        // Construtor
        public Debug(bool debugToggle, bool trafficToggle)
        {
            this.debugToggle = debugToggle;
            this.trafficToggle = trafficToggle; 
        }
        public void Activate()
        {
            if (debugToggle)
            {
                Point point0 = new Point(50, 50);
                Size size0 = new Size(600, 400);
                ScaledRectangle rec = new ScaledRectangle(point0, size0);
                ScaledText txt1 = new ScaledText(new Point(300, 60), "Debug Mode");
                ScaledText txtFPS = new ScaledText(new Point(300, 120), "FPS: " + Game.FPS);
                ScaledText txtTrafficStatus = new ScaledText(new Point(320, 180), "Traffic Toggle: " + trafficToggle); // lemon ui: interessante um sistema de layers para os scaled objects

                if (nearbyVehicles != null)
                {
                    ScaledText txtNearbyVehicles = new ScaledText(new Point(320, 230), "Nearby vehicles: " + nearbyVehicles.Length);
                    txtNearbyVehicles.Color = Color.White;
                    txtNearbyVehicles.Scale = 0.5f;
                    txtNearbyVehicles.Alignment = Alignment.Center;
                    txtNearbyVehicles.Draw();
                }

                rec.Color = Color.Black;
                txt1.Color = Color.Yellow;
                txtFPS.Color = Color.White;
                txtTrafficStatus.Color = Color.White;

                txt1.Scale = 0.5f;
                txtFPS.Scale = 0.5f;
                txtTrafficStatus.Scale = 0.5f;

                txt1.Alignment = Alignment.Center;
                txtFPS.Alignment = Alignment.Center;
                txtTrafficStatus.Alignment = Alignment.Center;

                rec.Draw();
                txt1.Draw();
                txtFPS.Draw();
                txtTrafficStatus.Draw();

                RemoveTraffic();
                Notification.Show("Position and Rotation of Player Saved in Text.");
                saveInfoInFile(@"D:\Epic\GTAV\Scripts\coord.txt", Game.Player.Character.Position, Game.Player.Character.Rotation);
            }
        }

        private void RemoveTraffic()
        {
            nearbyVehicles = World.GetNearbyVehicles(Game.Player.Character.Position, 500.0f);
            Model enemyCarModel = new Model("Cavalcade");
            Model enemyCarModel2 = new Model("Cavalcade2");
            if (trafficToggle)
            {
                for (int i = 0; i < nearbyVehicles.Length; i++)
                {
                    if (nearbyVehicles[i].Model != enemyCarModel && nearbyVehicles[i].Model != enemyCarModel2)
                    {
                        nearbyVehicles[i].Delete();
                    }
                }
            }
        }

        public void GetEnemiesRelationShip(List<Ped> createdPeds, List<Vehicle> createdVehicles)
        {
            int count = 0;

            for (int i = 0; i < createdPeds.Count; i++)
            {
                GTA.UI.Notification.Show(count + " - " + createdPeds[i].GetRelationshipWithPed(createdPeds[i]));
                GTA.UI.Notification.Show(count + " - " + createdPeds[i].RelationshipGroup);
                GTA.UI.Notification.Show(count + " - " + Function.Call<int>(Hash.GET_RELATIONSHIP_BETWEEN_PEDS, createdPeds[0], createdPeds[1]));
                count++;
            }
        }

        public void deleteEntitys(float area, string entityType)
        {
            switch (entityType)
            {
                case "ped":
                    {
                        nearbyPeds = World.GetNearbyPeds(Game.Player.Character, area);
                        for (int i = 0; i < nearbyPeds.Length; i++)
                        {
                            nearbyPeds[i].Delete();
                        }
                    }
                    break;

                case "vehicle":
                    {
                        nearbyVehicles = World.GetNearbyVehicles(Game.Player.Character, area);
                        for (int i = 0; i < nearbyVehicles.Length; i++)
                        {
                            nearbyVehicles[i].Delete();
                        }
                    }
                    break;
            }
        }
        private void saveInfoInFile(string filename, object obj1, object obj2)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                writer.WriteLine(obj1 + " : " + obj2);
            }
        }
    }
}
