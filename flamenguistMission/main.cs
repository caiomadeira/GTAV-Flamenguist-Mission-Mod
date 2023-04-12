// System classes
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;

// Scripthook and LemonUI
using GTA;
using GTA.Native;
using GTA.Math;
using GTA.UI;
using LemonUI.Scaleform;
using LemonUI;
using GTA.NaturalMotion;
using LemonUI.Elements;
using System.Drawing.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;

// Begin
namespace flamenguistMission
{
    public class FlaMission : Script
    {
        // Debug
        Debug debug = new Debug(false, false);
        Camera camera;
        ObjectPool pool = new ObjectPool();
        BigMessage msg;

        bool unlockNpcTrader = false;
        bool isMission = false;

        int MissionIndex = -1;
        int TaskIndex = 0;
        int PackageIndex = -1;
        public Camera cutsceneCamera;

        float defaultTimeScale = 1.0f;
        float timeScaleFail = 0.2f;

        bool failMission = false;
        bool isCutscene = false;
        bool hasAlreadyRun = false;
        bool spawnSecondWave = false;
        bool secondWaveIsActive = false;
        bool hasCompletedMission = false;

        Blip missionDestinationBlip;

        Vector3 michaelHousePosition = new Vector3(-836.3887f, 165.2782f, 68.49527f); // X:-836,3887 Y:165,2782 Z:68,49527 : X:0 Y:0 Z:-59,18466
        Vector3 trevorHousePosition = new Vector3(131.6756f, -1309.062f, 29.02058f); // X:131,6756 Y:-1309,062 Z:29,02058 : X:0 Y:0 Z:-44,52343
        Vector3 franklinHousePosition = new Vector3(9.055607f, 546.9609f, 175.6681f); // X:9,055607 Y:546,9609 Z:175,6681 : X:0 Y:0 Z:-121,9812

        Blip npcTraderBlip;
        Prop MissionPackage;
        Ped npcTraderPed;
        Vehicle vehicleEnemy;

        // Collection 1
        HashSet<Model> modelsCollection = new HashSet<Model>() { PedHash.BallaEast01GMY, PedHash.BallaOrig01GMY, PedHash.BallaSout01GMY, PedHash.Ballas01GFY, VehicleHash.Cavalcade };
        List<Ped> createdPeds = new List<Ped>();
        List<Vehicle> createdVehicles = new List<Vehicle>();
        List<WeaponHash> weaponCollection = new List<WeaponHash> { WeaponHash.AssaultRifle, WeaponHash.AssaultShotgun, WeaponHash.AssaultRifle, WeaponHash.BullpupRifle };
        //

        // Collection 2
        HashSet<Model> modelSecondCollection = new HashSet<Model>() { PedHash.Ballasog, PedHash.BallasLeaderCutscene, PedHash.BallaSout01GMY, PedHash.BallasLeader, VehicleHash.Cavalcade2 };
        List<Ped> pedSecondCollection = new List<Ped>();
        List<Vehicle> vehicleSecondCollection = new List<Vehicle>();
        List<WeaponHash> weaponSecondCollection = new List<WeaponHash> { WeaponHash.Pistol50, WeaponHash.HeavyPistol, WeaponHash.APPistol, WeaponHash.MicroSMG, WeaponHash.Pistol };
        //


        // Relationship Group
        RelationshipGroup enemiesRelationShipGroup = World.AddRelationshipGroup("ENEMYVASCO");
        RelationshipGroup traderRelationShipGroup = World.AddRelationshipGroup("TRADERFLAMENGO");
        int playerRelationShipGroup = Function.Call<int>(GTA.Native.Hash.GET_HASH_KEY, "PLAYER");
        int copRelationShipGroup = Function.Call<int>(GTA.Native.Hash.GET_HASH_KEY, "COP");
        int ambientBallasRelationGroup = Function.Call<int>(GTA.Native.Hash.GET_HASH_KEY, "AMBIENT_GANG_BALLAS");

        Vector3 playerPos = Game.Player.Character.Position;
        Vector3 MissionRespawn = new Vector3(61.94363f, -1910.36f, 21.48537f);

        // Cutscene Fixed Cameras
        List<Vector3> cam1Config = new List<Vector3>();
        List<Vector3> cam2Config = new List<Vector3>();

        // Mission Index 0
        Vector3 MissionTelefoneStartLocation = new Vector3(214.2005f, -853.4378f, 29.18798f);
        Vector3 MissionGroveLocation = new Vector3(64.74966f, -1913.361f, 21.45174f);
        Vector3 MissionGroveMarkLocation =  new Vector3(64.74966f, -1913.361f, 20.2243f);

        // Mission Index 1
        Vector3 npcTraderSpawnLocation = new Vector3(82.43924f, -1944.79f, 20.86181f);

        // Mission Index 2
        Vector3 talkToNpcLocation = new Vector3(82.34441f, -1943.666f, 20.74761f);
        Vector3 talkToNpcMarkLocation = new Vector3(82.34441f, -1943.666f, 19.77162f);

        // Mission Index 4
        Vector3 enemyVehicleSpawnLocation = new Vector3(-119.8248f, -1709.926f, 29.58983f);
        Vector3 enemyVehicleDestinationLocation = new Vector3(42.36935f, -1887.316f, 22.16708f);

        Vector3 enemySecondWaveSpawnLocation = new Vector3(78.44891f, -1917.414f, 21.12497f);

        public FlaMission() 
        {
            this.Tick += OnTick;
            this.KeyUp += OnKeyUp;
            this.KeyDown += OnKeyDown;
        }

        void SetupMission()
        {
            if (isMission)
            {
                SetupWorldForMission();
                switch (MissionIndex)
                {
                    case 0:
                        {
                            float CharacterDistance = Game.Player.Character.Position.DistanceTo(MissionGroveMarkLocation);

                            World.DrawMarker(MarkerType.VerticalCylinder, MissionGroveMarkLocation, new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0.7f, 0.7f, 0.5f), Color.FromArgb(255, 0, 0));
                            if (CharacterDistance < 1.5f)
                            {
                                GTA.UI.Screen.ShowSubtitle("Fale com ~r~Flamenguist.", 20000);
                                MissionIndex = 1;
                            }
                            else if (CharacterDistance > 35.0f && CharacterDistance < 55.0f)
                            {
                                GTA.UI.Screen.ShowSubtitle("Volte para falar com o ~r~Flamenguist.", 20000);
                            }
                            // fail for distance
                            else if (CharacterDistance > 55.0f)
                            {
                                fail();
                                MissionIndex = 0;
                            }
                        }
                        break;

                    case 1:
                        {
                            float DistanceToNpc = Game.Player.Character.Position.DistanceTo(npcTraderSpawnLocation);

                            for (int i = 0; i < 10; i++)
                            {
                                if(!unlockNpcTrader)
                                {
                                    npcTraderPed = spawnSpecialPed("trader", PedHash.BallaEast01GMY, npcTraderSpawnLocation);
                                    if (npcTraderPed != null)
                                    {
                                        npcTraderPed.AddBlip();
                                        npcTraderPed.AttachedBlip.Sprite = BlipSprite.Friend;
                                        npcTraderPed.AttachedBlip.Color = BlipColor.Red;
                                        npcTraderPed.Armor = 100;
                                        npcTraderPed.Weapons.Give(WeaponHash.AssaultRifle, 330, false, true);
                                        npcTraderPed.IsHeadtracking(Game.Player.Character);
                                    }
                                }
                            }

                            if (DistanceToNpc < 22.0f)
                            {
                                MissionIndex = 2;

                            } else if (DistanceToNpc > 40.0f && DistanceToNpc < 70.0f)
                            {
                                GTA.UI.Screen.ShowSubtitle("Volte e fale com ~r~Flamenguist.");

                            } else if (DistanceToNpc > 70.0f)
                            {
                                fail();
                                unlockNpcTrader = false;
                                npcTraderPed.AttachedBlip.Delete();
                                npcTraderPed.Delete();
                                MissionIndex = 1;
                                
                            } if ((npcTraderPed != null) && npcTraderPed.IsDead)
                            {
                                fail();
                                unlockNpcTrader = false;
                                npcTraderPed.AttachedBlip.Delete();
                                npcTraderPed.Delete();
                                MissionIndex = 1;
                            }
                        }
                        break;

                    case 2:
                        {
                            World.DrawMarker(MarkerType.VerticalCylinder, talkToNpcMarkLocation, new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0.7f, 0.7f, 0.5f), Color.FromArgb(255, 0, 0));
                            if (Game.Player.Character.Position.DistanceTo(talkToNpcLocation) < 1.5f)
                            {
                                GTA.UI.Screen.ShowHelpTextThisFrame("Aperte ~INPUT_CONTEXT~ para falar com ~r~Flamenguist.");
                                if(Game.IsControlJustPressed(GTA.Control.Context))
                                {
                                    MissionIndex = 3;
                                    Game.Player.CanControlCharacter = false;
                                }
                            }

                            if (!CheckSpecialEnemy())
                            {
                                fail();
                                unlockNpcTrader = false;
                                npcTraderPed.AttachedBlip.Delete();
                                npcTraderPed.Delete();
                                MissionIndex = 1;
                            }
                        } break;

                    case 3:
                        {
                            GTA.UI.Screen.ShowSubtitle("camisa ~r~Flamengo ~W~coletada.");
                            Function.Call(GTA.Native.Hash.TRIGGER_MUSIC_EVENT, "PROLOGUE_TEST_MISSION_START"); // Precisa do START para tocar as musicas
                            Wait(1000);
                            GTA.UI.Screen.FadeOut(1000);
                            Wait(1000);
                            MissionIndex = 4;

                        } break;

                    case 4:
                        {
                            // cutscene
                            Wait(1000);
                            GTA.UI.Screen.FadeIn(1000);
                            setupEnemiesAndCutscene(enemyVehicleSpawnLocation);
                            MissionIndex = 5;
                        } break;

                    case 5:
                        {
                            Function.Call(GTA.Native.Hash.TRIGGER_MUSIC_EVENT, "PROLOGUE_TEST_COLLECT_CASH");
                            GTA.UI.Screen.ShowSubtitle("É uma armadilha! Mate os ladrões de ~r~camisas.");
                            // checar posteriormente o porque quando é posto em uma função ele nao executa direito e nao remove o blip do npc trader
                            int deathCounter = 0;

                            for (int i = 0; i < (createdPeds.Count); i++)
                            {
                                if (createdPeds[i].IsDead && createdPeds[i].AttachedBlip.Exists())
                                {
                                    // é melhor que remover o blip é deixar ele full transparente pois o scripthook falha as vezes
                                    createdPeds[i].AttachedBlip.Alpha = 0;
                                    deathCounter++;

                                    if (deathCounter == 4)
                                    {
                                        MissionIndex = 6;
                                    }
                                }
                                CheckSpecialEnemy();
                            }
                        } break;

                      case 6:
                          {
                            CheckSpecialEnemy();
                            // tentar executar essa funcao de spawn uma vez apenas com a tatica do variavel de controle e depois setar o index para esse mesmo lugar assim evitando repetir o codigo
                            Function.Call(GTA.Native.Hash.TRIGGER_MUSIC_EVENT, "PROLOGUE_TEST_COP_GUNFIGHT");
                            switch ((PedHash)Game.Player.Character.Model)
                            {
                                case PedHash.Michael:
                                    Wait(1000);
                                    MissionSecondAct("b", michaelHousePosition, "Casa do Michael", BlipSprite.Michael, BlipColor.Blue2, Color.FromArgb(0, 0, 255));
                                    break;
                                case PedHash.Trevor:
                                    MissionSecondAct("o", michaelHousePosition, "Casa do Trevor", BlipSprite.Trevor, BlipColor.Orange, Color.FromArgb(255, 0, 0));
                                    break;
                                case PedHash.Franklin:
                                    MissionSecondAct("g", michaelHousePosition, "Casa do Franklin", BlipSprite.Franklin, BlipColor.Green, Color.FromArgb(0, 255, 0));
                                    break;
                            }
                          } break;

                    case 7:
                        {
                            switch((PedHash)Game.Player.Character.Model)
                            {
                                case PedHash.Michael:
                                    MissionPassed("A camisa do ~r~flamengo ~w~agora está disponível para o ~b~Michael.");
                                    break;
                                case PedHash.Trevor:
                                    MissionPassed("A camisa do ~r~flamengo ~w~agora está disponível para o ~o~Trevor.");
                                    break;
                                case PedHash.Franklin:
                                    MissionPassed("A camisa do ~r~flamengo ~w~agora está disponível para o ~g~Franklin.");
                                    break;
                            }
                        } break;
                }
                onDead();
            }
        }

        private void MissionSecondAct(string subtitleColor, Vector3 blipPosition, string blipName, BlipSprite blipSprite, BlipColor blipColor, Color blipSecondaryColor)
        {
            if (Game.Player.Character.IsInVehicle() == true)
            {
                GTA.UI.Screen.ShowSubtitle("Dirija até a sua ~" + subtitleColor + "~casa.");
                missionDestinationBlip = World.CreateBlip(blipPosition);
                missionDestinationBlip.Sprite = blipSprite;
                missionDestinationBlip.Color = blipColor;
                missionDestinationBlip.Name = blipName;
                missionDestinationBlip.SecondaryColor = blipSecondaryColor;
                missionDestinationBlip.ShowRoute = true;

                if (Game.Player.Character.Position.DistanceTo(MissionGroveLocation) > 200.0f)
                {
                    if (!spawnSecondWave)
                    {
                        SetupEnemiesChasePlayer();
                    }
                    else
                    {
                        GTA.UI.Screen.ShowSubtitle("Dirija para a sua ~" + subtitleColor +"~casa ~w~e fuja dos ~r~inimigos.");
                    }
                }
            }
            else
            {
                if (!secondWaveIsActive)
                {
                    GTA.UI.Screen.ShowSubtitle("Entre em um veículo e vá para a sua ~" + subtitleColor + "~casa.");
                }
            }

            if (secondWaveIsActive)
            {
                for (int i = 0; i < pedSecondCollection.Count; i++)
                {
                    if (pedSecondCollection[i].IsDead)
                    {
                        pedSecondCollection[i].AttachedBlip.Alpha = 0; // é melhor que remover o blip é deixar ele full transparente pois o scripthook falha as vezes
                    }
                }
            }

            World.DrawMarker(MarkerType.VerticalCylinder, blipPosition, new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0.7f, 0.7f, 0.5f), blipSecondaryColor);
            if (Game.Player.Character.Position.DistanceTo(blipPosition) < 2.0f)
            {
                MissionIndex = 7;
            }
        }
        private void SetupWorldForMission()
        {
            SetupRelationshipGroup();
            Game.IsMissionActive = true;
        }

        private void ClearItemsAfterMission()
        {
            isMission = false;
            if (!isMission)
            {
                if (npcTraderPed.AttachedBlip.Exists())
                {
                    npcTraderPed.AttachedBlip.Alpha = 0;
                }

                if (missionDestinationBlip.Exists())
                {
                    missionDestinationBlip.Alpha = 0;
                }

                foreach (var ped in pedSecondCollection)
                {
                    ped.Delete();
                }

                Game.IsMissionActive = false;
                GTA.UI.Screen.ShowSubtitle("", 100);
            }
        }
        private void MissionPassed(string awardMessage)
        {
            ClearItemsAfterMission();
            Function.Call(GTA.Native.Hash.TRIGGER_MUSIC_EVENT, "PROLOGUE_TEST_MISSION_END");
            Function.Call(GTA.Native.Hash.PLAY_MISSION_COMPLETE_AUDIO, "FRANKLIN_BIG_01");
            msg = new BigMessage($"~y~Mission Passed", "FLAMENGO!", MessageType.MissionPassedOldGen);
            msg.Visible = true;
            msg.Draw();
            GTA.UI.Screen.ShowHelpTextThisFrame(awardMessage);
            PlayExternalAudio(@"C:\Users\Madeira\Videos\Flamengo_audio.wav");
        }
        private void onDead()
        {
            if (Game.Player.IsDead)
            {
                if (isMission)
                {
                   isMission = false;
                }
            }
        }
        /// <summary>
        /// Retorna FALSE para caso o NPC TRADER esteja morto ou nulo e TRUE para caso ele esteja vivo
        /// </summary>
        private bool CheckSpecialEnemy()
        {
            if (npcTraderPed != null)
            {
                if (npcTraderPed.IsDead)
                {
                    npcTraderPed.AttachedBlip.Alpha = 0;
                    return false;
                }
                return true;
            }
          return false;
        }
        private void SetupRelationshipGroup()
        {
            enemiesRelationShipGroup.SetRelationshipBetweenGroups(copRelationShipGroup, Relationship.Neutral, true);
            enemiesRelationShipGroup.SetRelationshipBetweenGroups(playerRelationShipGroup, Relationship.Hate, true);
            enemiesRelationShipGroup.SetRelationshipBetweenGroups(enemiesRelationShipGroup, Relationship.Companion, true);
            enemiesRelationShipGroup.SetRelationshipBetweenGroups(ambientBallasRelationGroup, Relationship.Companion, true);
            traderRelationShipGroup.SetRelationshipBetweenGroups(playerRelationShipGroup, Relationship.Neutral, true);
            traderRelationShipGroup.SetRelationshipBetweenGroups(enemiesRelationShipGroup, Relationship.Companion, true);
        }
        
        private Ped spawnSpecialPed(string pedType, PedHash ped, Vector3 location)
        {
            unlockNpcTrader = true;
            switch(pedType)
            {
                case "trader":
                    {
                        var npc = World.CreatePed(ped, location);
                        npc.Weapons.Give(WeaponHash.Pistol50, 100, false, true);
                        return npc;
                    }

                default: 
                    break;
            }
            return null;
        }

        private void fail()
        {
            GTA.UI.Screen.ShowSubtitle("", 100);
            Function.Call(GTA.Native.Hash.PLAY_MISSION_COMPLETE_AUDIO, "GENERIC_FAILED");
            GTA.Script.Wait(1500);
            Game.TimeScale = timeScaleFail;
            Game.Player.Character.IsInvincible = true;
            GTA.UI.Screen.StartEffect(ScreenEffect.DeathFailOut);
            GameplayCamera.Shake(CameraShake.DeathFail, 1.5f);
            Function.Call(GTA.Native.Hash.DISPLAY_HUD, false);
            Function.Call(GTA.Native.Hash.DISPLAY_RADAR, false);
            GTA.Script.Wait(5000);
            respawnCheckpoint(ScreenEffect.SwitchShortMichaelIn, ScreenEffect.SwitchHudMichaelOut);
        }

        private void respawnCheckpoint(ScreenEffect switchCharEffectIn, ScreenEffect switchCharEffectOut)
        {
            GTA.UI.Screen.FadeOut(1000);
            GTA.Script.Wait(3500);
            Game.Player.Character.IsInvincible = false;
            if (GTA.UI.Screen.IsEffectActive(ScreenEffect.DeathFailOut))
            {
                Game.TimeScale = defaultTimeScale;
                GTA.UI.Screen.StopEffect(ScreenEffect.DeathFailOut);
                Game.Player.Character.Position = MissionRespawn;
                GameplayCamera.StopShaking();
                GTA.Script.Wait(2000);
            }

            GTA.UI.Screen.FadeIn(3000);
            if (GTA.UI.Screen.IsFadingIn)
            {
                GTA.UI.Screen.StartEffect(switchCharEffectIn, 1000);
                GTA.UI.Screen.StartEffect(switchCharEffectOut, 1000);
            }
            Function.Call(GTA.Native.Hash.DISPLAY_HUD, true);
            Function.Call(GTA.Native.Hash.DISPLAY_RADAR, true);
        }
        //Function.Call(GTA.Native.Hash.TRIGGER_MUSIC_EVENT, "PROLOGUE_TEST_COLLECT_CASH");
        private void cameraConfiguration(string camType, List<Vector3> camConfig)
        {
            Vector3 cam1Position = new Vector3(-21.95464f, -1828.79f, 25.63683f);
            Vector3 cam1Rotation = new Vector3(0f, 0f, 48.54497f);

            Vector3 cam2Position = new Vector3(59.35807f, -1910.563f, 21.62996f);
            Vector3 cam2Rotation = new Vector3(0f, 0f, 37.12193f);

            switch (camType)
            {
                case "cam1":
                    camConfig.Add(cam1Position);
                    camConfig.Add(cam1Rotation);
                    break;

                case "cam2":
                    camConfig.Add(cam2Position);
                    camConfig.Add(cam2Rotation);
                    break;
            }
        }
        private void setupEnemiesAndCutscene(Vector3 spawnLocation)
        {
            cameraConfiguration("cam1", cam1Config);
            cameraConfiguration("cam2", cam2Config);
            isCutscene = true;
            if (isCutscene)
            {
                Function.Call(GTA.Native.Hash.TRIGGER_MUSIC_EVENT, "PROLOGUE_TEST_COLLECT_MONEY");
                LoadModels(modelsCollection);
                ConfigureModels(modelsCollection, createdPeds, createdVehicles, weaponCollection, spawnLocation);
                SpawnEnemiesAndSetTask(createdPeds, createdVehicles);
                Function.Call(GTA.Native.Hash.DISPLAY_HUD, false);
                Function.Call(GTA.Native.Hash.DISPLAY_RADAR, false);
                cutsceneCamera = World.CreateCamera(cam1Config[0], cam1Config[1], 15f);
                cutsceneCamera.IsActive = true;
                World.RenderingCamera = cutsceneCamera;
                Wait(9000);

                cutsceneCamera = World.CreateCamera(cam2Config[0], cam2Config[1], 15f);
                cutsceneCamera.PointAt(createdVehicles[0]);
                Wait(6000);
                //setupEnemies(enemySecondWaveSpawnLocation);
                //Function.Call(GTA.Native.Hash.TRIGGER_MUSIC_EVENT, "PROLOGUE_TEST_COP_GUNFIGHT");
                isCutscene = false;
            }
            Game.Player.CanControlCharacter = true;
            cutsceneCamera.IsActive = false;
            World.RenderingCamera = null;
            Function.Call(GTA.Native.Hash.DISPLAY_HUD, true);
            Function.Call(GTA.Native.Hash.DISPLAY_RADAR, true);
        }

        private void LoadModels(HashSet<Model> modelsCollection)
        {
            foreach (var model in modelsCollection)
            {
                model.Request();
            }
            while (!modelsCollection.All(x => x.IsLoaded))
            {
                Wait(0);
            }
        }

        private void ConfigureModels(HashSet<Model> modelsCollection, List<Ped> pedCollection, List<Vehicle> vehicleCollection, List<WeaponHash> weaponCollection, Vector3 spawnLocation)
        {
            try
            {
                foreach (var model in modelsCollection)
                {
                    if (model.IsPed)
                    {
                        var createdPed = World.CreatePed(model, spawnLocation += Vector3.RandomXY());
                        if (createdPeds != null)
                        {
                            createdPed.IsEnemy = true;
                            createdPed.AddBlip();
                            createdPed.AttachedBlip.Name = "Inimigo";
                            createdPed.AttachedBlip.Sprite = BlipSprite.Standard;
                            createdPed.AttachedBlip.Color = BlipColor.Red;
                            createdPed.RelationshipGroup = enemiesRelationShipGroup;
                            createdPed.CanSwitchWeapons = true;
                            for (int i = 0; i < weaponCollection.Count; i++)
                            {
                                Random rand = new Random();
                                int index = rand.Next(weaponCollection.Count);
                                createdPed.Weapons.Give(weaponCollection[index], 600, true, true);
                            }
                            createdPeds.Add(createdPed);
                        }
                    }

                    else if (model.IsVehicle)
                    {
                        var createdVehicle = World.CreateVehicle(model, spawnLocation += Vector3.RandomXY().Around(3f) * 2f); // no heading
                        if (createdVehicle != null)
                        {
                            createdVehicle.DirtLevel = 40f;
                            createdVehicle.Mods.CustomPrimaryColor = Color.FromArgb(1, 1, 1);
                            createdVehicle.Mods.LicensePlate = "V4SC0";
                            createdVehicles.Add(createdVehicle);
                        }
                    }
                }
            } catch (System.NullReferenceException e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        // BEGIN SECOND WAVE
        private void SetupEnemiesChasePlayer()
        {
            try
            {
                foreach (var model in modelSecondCollection)
                {
                    model.Request();
                }
                while (!modelSecondCollection.All(x => x.IsLoaded))
                {
                    Wait(0);
                }
            } catch ( System.Exception e  ) { MessageBox.Show("1: LOAD MODELS ERROR: " + e.ToString()); }

            try
            {
                foreach (var model in modelSecondCollection)
                {
                    if (model.IsPed)
                    {
                        var createdPed = World.CreatePed(model, enemyVehicleSpawnLocation + Vector3.RandomXY());
                        if (pedSecondCollection != null)
                        {
                            createdPed.IsEnemy = true;
                            createdPed.AddBlip();
                            createdPed.AttachedBlip.Name = "Inimigo";
                            createdPed.AttachedBlip.Sprite = BlipSprite.Standard;
                            createdPed.AttachedBlip.Color = BlipColor.Red;
                            createdPed.RelationshipGroup = enemiesRelationShipGroup;
                            createdPed.CanSwitchWeapons = true;
                            for (int i = 0; i < weaponSecondCollection.Count; i++)
                            {
                                Random rand = new Random();
                                int index = rand.Next(weaponSecondCollection.Count);
                                createdPed.Weapons.Give(weaponSecondCollection[index], 600, true, true);
                            }
                            pedSecondCollection.Add(createdPed);
                        }
                    }

                    else if (model.IsVehicle)
                    {
                        //var createdVehicle = World.CreateVehicle(model, enemySecondWaveSpawnLocation += Vector3.RandomXY().Around(3f) * 2f); // no heading
                        var createdVehicle = World.CreateVehicle(model, enemyVehicleSpawnLocation + Vector3.RandomXY().Around(3f) * 2f);
                        if (createdVehicle != null)
                        {
                            createdVehicle.DirtLevel = 40f;
                            createdVehicle.Mods.CustomPrimaryColor = Color.FromArgb(1, 1, 1);
                            createdVehicle.Mods.LicensePlate = "V4SC0";
                            createdVehicle.Speed = 90;
                            createdVehicle.ForwardSpeed = 70;
                            vehicleSecondCollection.Add(createdVehicle);
                        }
                    }
                }
            }
            catch (System.Exception e) { MessageBox.Show("2: CONFIGURE MODELS ERROR: " + e.ToString()); }
            try
            {
                Wait(1000);
                Game.Player.Character.IsPriorityTargetForEnemies = true;
                vehicleSecondCollection[0].Rotation = new Vector3(0f, 0f, -132.1571f);

                for (int i = 0; i < pedSecondCollection.Count; i++)
                {
                    if (vehicleSecondCollection[0].IsSeatFree(VehicleSeat.Driver))
                    {
                        pedSecondCollection[i].SetIntoVehicle(vehicleSecondCollection[0], VehicleSeat.Driver);
                        if (pedSecondCollection[i].SeatIndex == VehicleSeat.Driver)
                        {
                            pedSecondCollection[i].Task.VehicleChase(Game.Player.Character);
                        }
                    }
                    else if (!vehicleSecondCollection[0].IsSeatFree(VehicleSeat.Driver))
                    {
                        pedSecondCollection[i].SetIntoVehicle(vehicleSecondCollection[0], VehicleSeat.Any);
                    }
                }

                secondWaveIsActive = true;
                spawnSecondWave = true;

            } catch (System.Exception e) { MessageBox.Show("3: DEFINE TASKS: " + e.ToString()); }
        }

        // END SECOND WAVE
        private void SpawnEnemiesAndSetTask(List<Ped> createdPeds, List<Vehicle> createdVehicles)
        {
            Game.Player.Character.IsPriorityTargetForEnemies = true;
            createdVehicles[0].Rotation = new Vector3(0f, 0f, 128.1007f);

            for (int i = 0; i < createdPeds.Count; i++)
            {    
                if (createdVehicles[0].IsSeatFree(VehicleSeat.Driver))
                {
                    createdPeds[i].SetIntoVehicle(createdVehicles[0], VehicleSeat.Driver);

                } else if (!createdVehicles[0].IsSeatFree(VehicleSeat.Driver))
                {
                    createdPeds[i].SetIntoVehicle(createdVehicles[0], VehicleSeat.Any);
                }
            }

            foreach (var ped in createdPeds)
            {
                if (ped.SeatIndex == VehicleSeat.Driver)
                {
                    ped.Task.DriveTo(createdVehicles[0], enemyVehicleDestinationLocation, 30, 70, DrivingStyle.AvoidTrafficExtremely);
                    if (ped.Position.DistanceTo(enemyVehicleDestinationLocation) > 0.5f)
                    {
                        ped.Task.ParkVehicle(createdVehicles[0], enemyVehicleDestinationLocation, Game.Player.Character.Heading - 90, 20, true);
                    }
                }
            }

            for (int i = 0; i < createdPeds.Count; i++)
            {
                createdPeds[i].Task.FightAgainst(Game.Player.Character);
                createdPeds[i].AlwaysKeepTask = true;
            }
        }

        /// =========================================
        /// DEFAULT METHODS
        ///
        /// 
        /// =========================================
        private void OnTick(object sender, EventArgs e)
        {
            debug.Activate();
            pool.Process();
            SetupMission();
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {

        }
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.NumPad0)
            {
                debug.debugToggle = !debug.debugToggle;
            }

            if (e.KeyCode == Keys.NumPad2)
            {
                // 1 - PROLOGUE_TEST_COLLECT_MONEY
                // 2 - PROLOGUE_TEST_COLLECT_CASH
                // 3 - PROLOGUE_TEST_COP_GUNFIGHT
                // 4 - PROLOGUE_TEST_MISSION_END
                //Function.Call(GTA.Native.Hash.TRIGGER_MUSIC_EVENT, "PROLOGUE_TEST_COLLECT_MONEY");
                //Function.Call(GTA.Native.Hash.TRIGGER_MUSIC_EVENT, "PROLOGUE_TEST_COLLECT_CASH");
                //Game.Player.Character.Position = enemySecondWaveSpawnLocation;
            }

            if (e.KeyCode == Keys.NumPad4)
            {
                GTA.UI.Notification.Show("Peds and Vehicles Removed.");
                debug.deleteEntitys(500.0f, "ped");
                debug.deleteEntitys(500.0f, "vehicle");
            }

            if (e.KeyCode == Keys.NumPad9)
            {
                isMission = !isMission;
                if (isMission)
                {
                    MissionIndex = 0;
                }
            }

            if (e.KeyCode == Keys.NumPad1)
            {
                GTA.UI.Screen.StopEffects();
                Function.Call(GTA.Native.Hash.DISPLAY_HUD, true);
                Function.Call(GTA.Native.Hash.DISPLAY_RADAR, true);
                Game.TimeScale = 1.0f;
                if (!unlockNpcTrader && npcTraderPed != null)
                {
                    npcTraderPed.AttachedBlip.Delete();
                    npcTraderPed.Delete();
                }
                World.RenderingCamera = null;
            }

            if (e.KeyCode == Keys.NumPad7)
            {
                Function.Call(GTA.Native.Hash.TRIGGER_MUSIC_EVENT, "PROLOGUE_TEST_MISSION_END");
            }

            if (e.KeyCode == Keys.NumPad3)
            {
                if (debug.debugToggle)
                {
                    debug.trafficToggle = !debug.trafficToggle;
                }
            }
            if (e.KeyCode == Keys.NumPad8)
            {
                PlayExternalAudio(@"C:\Users\Madeira\Videos\Flamengo_audio.wav");
            }
        }

        private void PlayExternalAudio(string path)
        {
            try
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer();
                player.SoundLocation = path;
                player.Play();
            } catch (System.Exception e) { MessageBox.Show(e.ToString()); }
        }
    }
}
