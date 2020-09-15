using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfinityScript;

namespace Old_School_FFA
{
    public partial class OS_FFA : BaseScript
    {
        public OS_FFA()
        {
            ServerTittle(GSCFunctions.GetDvar("mapname"), "Old School FFA");

            if (GSCFunctions.GetDvar("g_gametype") != "dm")
            {
                GSCFunctions.SetDvar("g_gametype", "dm");
                Utilities.ExecuteCommand("map_restart");
                return;
            }

            GSCFunctions.MakeDvarServerInfo("didyouknow", "^2Old School FFA script by LastDemon99");
            GSCFunctions.MakeDvarServerInfo("g_motd", "^2Old School FFA script by LastDemon99");
            GSCFunctions.MakeDvarServerInfo("motd", "^2Old School FFA script by LastDemon99");

            LoadTargetZones();
            SpawnItems();
            PreCachePerksHud();

            PlayerConnected += new Action<Entity>((player) =>
            {
                ServerWelcomeTittle(player, "Old School FFA", new float[] { 0, 0, 1 });
                player.SetClientDvar("ui_mapname", "Old School FFA");
                player.SetClientDvar("ui_gametype", "Old School FFA");

                UsablesHud(player);
                UseItem(player);

                OnSpawnPlayer(player);
            });
        }

        private void LoadTargetZones()
        {
            switch (GSCFunctions.GetDvar("mapname"))
            {
                case "mp_radar":
                    TargetZones = targetZone_outpost;
                    break;
                case "mp_plaza2":
                    TargetZones = targetZone_arkaden;
                    break;
                case "mp_paris":
                    TargetZones = targetZone_resistance;
                    break;
                case "mp_carbon":
                    TargetZones = targetZone_carbon;
                    break;
                case "mp_bravo":
                    TargetZones = targetZone_mission;
                    break;
                case "mp_underground":
                    TargetZones = targetZone_underground;
                    break;
                case "mp_interchange":
                    TargetZones = targetZone_interchange;
                    break;
                case "mp_alpha":
                    TargetZones = targetZone_lockdown;
                    break;
                case "mp_village":
                    TargetZones = targetZone_village;
                    break;
                case "mp_lambeth":
                    TargetZones = targetZone_fallen;
                    break;
                case "mp_dome":
                    TargetZones = targetZone_dome;
                    break;
                case "mp_hardhat":
                    TargetZones = targetZone_hardhat;
                    break;
                case "mp_bootleg":
                    TargetZones = targetZone_bootleg;
                    break;
                case "mp_seatown":
                    TargetZones = targetZone_seatown;
                    break;
                case "mp_mobalrig":
                    TargetZones = targetZone_bakaara;
                    break;
                case "mp_mogadishu":
                    TargetZones = targetZone_bakaara;
                    break;
                case "mp_exchange":
                    TargetZones = targetZone_downturn;
                    break;
                default:
                    TargetZones = null;
                    break;
            }
        }
        private void SpawnItems()
        {
            //Spawn FX


            //Set Weapons 
            List<string> Weapons = new List<string>();
            int[] itemCount = RandomNum(TargetZones.Length, 0, OS_Weapons.Length);
            for (int i = 0; i < itemCount.Length; i++)
                Weapons.Add(OS_Weapons[itemCount[i]]);

            //Spawn Weapons & FX
            int spawnedcount = 0;
            foreach (Vector3 zone in TargetZones)
            {
                Entity fx = SpawnTriggerFX(goldcircle_fx, zone + new Vector3(0, 0, -50));
                FXList.Add(fx);

                Entity wep = SpawnModel(zone - new Vector3(0, 0, 10), FindModelbyWep(Weapons[spawnedcount]));
                ItemsList.Add(wep);
                spawnedcount++;
            }

            //Items Init
            foreach (Entity Items in ItemsList)
            {
                Items.SetField("used", 0);
                if (!OS_Models.Contains(Items.Model))
                    Items.Delete();

                if (!GSCFunctions.IsDefined(Items))
                    ItemsList.Remove(Items);
            }

            //Set Perk Zone
            List<int> PerkZone = new List<int>();
            int[] objperkszone = RandomNum(3, 0, ItemsList.Count);
            for (int i = 0; i < objperkszone.Length; i++)
                PerkZone.Add(objperkszone[i]);

            //Set Perks
            List<int> Perks = new List<int>();
            int[] _numperk = RandomNum(3, 0, os_perks.Length);
            for (int i = 0; i < _numperk.Length; i++)
                Perks.Add(_numperk[i]);

            //Spawn Perks
            int count = 0;
            foreach (int num in PerkZone)
            {
                ItemsList[num].SetModel(os_perk);

                ItemsList[num].SetField("perk", os_perks[_numperk[count]]);
                count++;
            }

            //Rotate Models
            OnInterval(5000, () =>
            {
                foreach (Entity ent in ItemsList)
                    ent.RotateYaw(360, 5);
                return true;
            });
        }
        private void HideOnUseItem()
        {
            foreach (Entity ent in ItemsList)
            {
                if (ent.GetField<int>("used") == 1)
                {
                    ent.Hide();

                    foreach (Entity fx in FXList)
                        if (fx.Origin.DistanceTo(ent.Origin + new Vector3(0, 0, -50)) <= 10)
                        {
                            FXList.Remove(fx);
                            fx.Delete();
                            break;
                        }

                    Entity newfx = SpawnTriggerFX(redcircle_fx, ent.Origin + new Vector3(0, 0, -50));
                    FXList.Add(newfx);
                }
                else
                {
                    ent.Show();

                    foreach (Entity fx in FXList)
                        if (fx.Origin.DistanceTo(ent.Origin + new Vector3(0, 0, -50)) <= 10)
                        {
                            FXList.Remove(fx);
                            fx.Delete();
                            break;
                        }

                    Entity newfx = SpawnTriggerFX(goldcircle_fx, ent.Origin + new Vector3(0, 0, -50));
                    FXList.Add(newfx);                    
                }
            }
        }

        private void OnSpawnPlayer(Entity player)
        {
            DisableSelectClass(player);
            GSCFunctions.DisableWeaponPickup(player);
            player.SpawnedPlayer += new Action(() =>
            {
                GSCFunctions.DisableWeaponPickup(player);
                player.ClearPerks();
                player.TakeAllWeapons();
                AfterDelay(500, () =>
                {
                    player.GiveWeapon(os_primarywep);
                    player.SwitchToWeaponImmediate(os_primarywep);
                    player.GiveWeapon(os_secondarywep);
                });

                player.SetField("perkslot", new Vector3(-1, -1, -1));
            });
        }
        private void UsablesHud(Entity player)
        {
            player.SetField("msgstate", 0);

            HudElem message = HudElem.CreateFontString(player, HudElem.Fonts.HudBig, 0.6f);
            message.SetPoint("CENTER", "CENTER", 0, -50);
            message.Alpha = 0f;

            OnInterval(100, () =>
            {
                foreach (Entity ent in ItemsList)
                    if (player.Origin.DistanceTo2D(ent.Origin) <= 70)
                        player.SetField("obj", ent);

                if (player.HasField("obj"))
                {
                    Entity obj = player.GetField<Entity>("obj");
                    if (obj.HasField("perk") && CheckSlotperk(player, obj) == true)
                        message.SetText("");
                    else if (obj.HasField("perk") && CheckSlotperk(player, obj) == false)
                        message.SetText("Press ^3[{+activate}] ^7to get perk.");
                    else if (!player.HasWeapon(FindWepbyModel(obj.Model)))
                        message.SetText("Press ^3[{+activate}] ^7to get weapon.");
                    else if (player.GetAmmoCount(FindWepbyModel(obj.Model)) != GSCFunctions.WeaponStartAmmo(FindWepbyModel(obj.Model)))
                        message.SetText("Press ^3[{+activate}] ^7to get ammo.");
                    else if (player.GetAmmoCount(FindWepbyModel(obj.Model)) == GSCFunctions.WeaponStartAmmo(FindWepbyModel(obj.Model)))
                        message.SetText("");

                    if (obj.GetField<int>("used") == 0)
                        if (player.Origin.DistanceTo2D(obj.Origin) <= 70)
                            message.Alpha = 1f;
                        else
                            message.Alpha = 0f;
                    else
                        message.Alpha = 0f;
                }
                return true;
            });
        }
        private void UseItem(Entity player)
        {
            player.NotifyOnPlayerCommand("use", "+activate");
            player.OnNotify("use", entity =>
            {
                foreach (Entity ent in ItemsList)
                    if (player.Origin.DistanceTo2D(ent.Origin) <= 70)
                        player.SetField("obj_use", ent);

                if (player.HasField("obj_use"))
                {
                    Entity obj = player.GetField<Entity>("obj_use");
                    if (player.Origin.DistanceTo2D(obj.Origin) <= 70 && obj.GetField<int>("used") == 0)
                    {
                        if (obj.HasField("perk"))
                        {
                            if (CheckSlotperk(player, obj) == false)
                            {
                                SlotState(player, obj);
                                ShowPerksHud(player, obj);
                                player.PlayLocalSound("scavenger_pack_pickup");
                                player.SetPerk(obj.GetField<string>("perk"), true, false);
                                obj.SetField("used", 1);
                                HideOnUseItem();
                                CheckSlotperk(player, obj);
                            }
                        }
                        else if (!player.HasWeapon(FindWepbyModel(obj.Model)))
                        {
                            player.PlayLocalSound("mp_suitcase_pickup");
                            GetItem(player, obj.Model);
                            obj.SetField("used", 1);
                            HideOnUseItem();
                        }
                        else if (player.GetAmmoCount(FindWepbyModel(obj.Model)) != GSCFunctions.WeaponStartAmmo(FindWepbyModel(obj.Model)))
                        {
                            player.PlayLocalSound("scavenger_pack_pickup");
                            player.GiveStartAmmo(FindWepbyModel(obj.Model));
                            obj.SetField("used", 1);
                            HideOnUseItem();
                        }
                    }

                    AfterDelay(30000, () =>
                    {
                        if (obj.GetField<int>("used") == 1)
                            obj.SetField("used", 0);

                        HideOnUseItem();
                    });
                }
            });
        }

        private void GetItem(Entity player, string modelname)
        {
            try
            {
                string getwep = FindWepbyModel(modelname);

                player.TakeWeapon(player.CurrentWeapon);
                player.GiveWeapon(getwep);
                player.SwitchToWeaponImmediate(getwep);
            }
            catch { };
        }
        private void GivePerkHud(Entity player, string name, string perk, string color)
        {
            HudElem perkicon = HudElem.CreateIcon(player, perk, 90, 90);
            perkicon.Parent = HudElem.UIParent;
            perkicon.SetPoint("TOPCENTER", "TOPCENTER", 0, 60);
            perkicon.SetShader(perk, 90, 90);
            perkicon.Foreground = true;
            perkicon.HideWhenInMenu = true;
            AfterDelay(1400, () => { perkicon.Destroy(); });

            HudElem perkname = HudElem.CreateFontString(player, HudElem.Fonts.HudBig, 1f);
            perkname.SetPoint("TOPCENTER", "TOPCENTER", 0, 165);
            perkname.SetText(name);
            perkname.GlowAlpha = 1f;
            perkname.SetPulseFX(100, 1000, 600);

            switch (color)
            {
                case "green":
                    perkname.GlowColor = (new Vector3(0f, 1f, 0f));
                    break;
                case "blue":
                    perkname.GlowColor = (new Vector3(0f, 0f, 1f));
                    break;
                case "red":
                    perkname.GlowColor = (new Vector3(1f, 0f, 0f));
                    break;
                default:
                    perkname.GlowColor = (new Vector3(245f, 208f, 051f));
                    break;
            }
        }
        private void ShowPerksHud(Entity player, Entity perkfield)
        {
            if (perkfield.GetField<string>("perk") == "specialty_longersprint")
                GivePerkHud(player, "Extreme Conditioning", "specialty_longersprint_upgrade", "blue");
            else if (perkfield.GetField<string>("perk") == "specialty_fastreload")
                GivePerkHud(player, "Sleight of Hand", "specialty_fastreload_upgrade", "blue");
            else if (perkfield.GetField<string>("perk") == "specialty_quickdraw")
                GivePerkHud(player, "Quickdraw", "specialty_quickdraw_upgrade", "red");
            else if (perkfield.GetField<string>("perk") == "specialty_stalker")
                GivePerkHud(player, "Stalker", "specialty_stalker_upgrade", "green");
            else if (perkfield.GetField<string>("perk") == "specialty_paint")
                GivePerkHud(player, "Recon", "specialty_paint_upgrade", "blue");
            else if (perkfield.GetField<string>("perk") == "specialty_bulletaccuracy")
                GivePerkHud(player, "Steady Aim", "specialty_steadyaim_upgrade", "green");
            else if (perkfield.GetField<string>("perk") == "specialty_quieter")
                GivePerkHud(player, "Dead Silent", "specialty_quieter_upgrade", "green");
        }
        private void SlotState(Entity player, Entity perkfield)
        {
            string getperk = "";
            int perkIndex = 0;

            string getfield = perkfield.GetField<string>("perk");

            switch (getfield)
            {
                case "specialty_longersprint":
                    getperk = "specialty_longersprint";
                    break;
                case "specialty_fastreload":
                    getperk = "specialty_fastreload";
                    break;
                case "specialty_quickdraw":
                    getperk = "specialty_quickdraw";
                    break;
                case "specialty_stalker":
                    getperk = "specialty_stalker";
                    break;
                case "specialty_autospot":
                    getperk = "specialty_autospot";
                    break;
                case "specialty_paint":
                    getperk = "specialty_paint";
                    break;
                case "specialty_bulletaccuracy":
                    getperk = "specialty_bulletaccuracy";
                    break;
                case "specialty_quieter":
                    getperk = "specialty_quieter";
                    break;
            }

            perkIndex = Array.IndexOf(os_perks, getperk);

            Vector3 perkslot = player.GetField<Vector3>("perkslot");

            if (perkslot.X == -1f)
                player.SetField("perkslot", new Vector3(perkIndex, perkslot.Y, perkslot.Z));
            else if (perkslot.Y == -1f && perkIndex != perkslot.X)
                player.SetField("perkslot", new Vector3(perkslot.X, perkIndex, perkslot.Z));
            else if (perkslot.Z == -1f && perkIndex != perkslot.Y && perkIndex != perkslot.X)
                player.SetField("perkslot", new Vector3(perkslot.X, perkslot.Y, perkIndex));
        }

        private void PreCachePerksHud()
        {
            GSCFunctions.PreCacheShader("specialty_longersprint_upgrade");
            GSCFunctions.PreCacheShader("specialty_fastreload_upgrade");
            GSCFunctions.PreCacheShader("specialty_quickdraw_upgrade");
            GSCFunctions.PreCacheShader("specialty_stalker_upgrade");
            GSCFunctions.PreCacheShader("specialty_coldblooded_upgrade");
            GSCFunctions.PreCacheShader("specialty_paint_upgrade");
            GSCFunctions.PreCacheShader("specialty_steadyaim_upgrade");
            GSCFunctions.PreCacheShader("specialty_quieter_upgrade");
        }
    }
}
