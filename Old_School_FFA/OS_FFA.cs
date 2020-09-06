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
            GSCFunctions.MakeDvarServerInfo("didyouknow", "^2Old School FFA script by LastDemon99");
            GSCFunctions.MakeDvarServerInfo("g_motd", "^2Old School FFA script by LastDemon99");
            GSCFunctions.MakeDvarServerInfo("motd", "^2Old School FFA script by LastDemon99");

            LoadTargetZones();
            SpawnItems();
            PreCachePerksHud();

            HideOnUseItem();
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

        public static void LoadTargetZones()
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
        public static void SpawnItems()
        {
            List<string> Items = new List<string>();
            int[] itemCount = RandomNum(TargetZones.Length, 0, OS_Weapons.Length);
            for (int i = 0; i < itemCount.Length; i++)
                Items.Add(OS_Weapons[itemCount[i]]);

            int spawnedcount = 0;
            foreach (Vector3 zone in TargetZones)
            {
                SpawnModel(zone - new Vector3(0, 0, 10), FindModelbyWep(Items[spawnedcount]));
                spawnedcount++;
            }

            foreach (Entity ent in ItemsList)
            {
                ent.SetField("used", 0);

                if (!OS_Models.Contains(ent.Model))
                    ent.Delete();

                switch (ent.Model)
                {
                    case "weapon_dragunov_iw5":
                        SetScope("weapon_dragunov_scope_iw5", ent, -6);
                        break;
                    case "weapon_rsass_iw5":
                        SetScope("weapon_rsass_scope_iw5", ent, -6);
                        break;
                    case "weapon_as50_iw5":
                        SetScope("weapon_as50_scope_iw5", ent, 5);
                        break;
                    case "weapon_l96a1_iw5":
                        SetScope("weapon_l96a1_scope_iw5", ent, -6);
                        break;
                    case "weapon_remington_msr_iw5":
                        SetScope("weapon_remington_msr_scope_iw5", ent, -6);
                        break;

                }
            }

            List<int> pnum = new List<int>();
            int[] objperkszone = RandomNum(3, 0, ItemsList.Count);
            for (int i = 0; i < objperkszone.Length; i++)
                pnum.Add(objperkszone[i]);

            List<int> numperk = new List<int>();
            int[] _numperk = RandomNum(3, 0, os_perks.Length);
            for (int i = 0; i < _numperk.Length; i++)
                numperk.Add(_numperk[i]);

            int count = 0;
            foreach (int num in pnum)
            {
                if (ItemsList[num].HasField("scope"))
                {
                    Entity scope = ItemsList[num].GetField<Entity>("scope");
                    scope.Delete();
                }
                ItemsList[num].SetModel(os_perk);

                ItemsList[num].SetField("perk", os_perks[_numperk[count]]);
                count++;
            }

            OnInterval(1000, () =>
            {
                foreach (Entity ent in ItemsList)
                    ent.RotateYaw(360, 4);
                return true;
            });
        }
        public static void PreCachePerksHud()
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
        public static void HideOnUseItem()
        {
            OnInterval(100, () =>
            {
                foreach (Entity ent in ItemsList)
                {
                    if (ent.GetField<int>("used") == 1)
                    {
                        if (ent.HasField("scope"))
                        {
                            Entity scope = ent.GetField<Entity>("scope");
                            scope.Hide();
                            ent.Hide();
                        }
                        else
                            ent.Hide();
                    }
                    else
                    {
                        if (ent.HasField("scope"))
                        {
                            Entity scope = ent.GetField<Entity>("scope");
                            scope.Show();
                            ent.Show();
                        }
                        else
                            ent.Show();
                    }
                }
                return true;
            });
        }

        public static void OnSpawnPlayer(Entity player)
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
        public static void UsablesHud(Entity player)
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
        public static void UseItem(Entity player)
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
                                CheckSlotperk(player, obj);
                            }
                        }
                        else if (!player.HasWeapon(FindWepbyModel(obj.Model)))
                        {
                            player.PlayLocalSound("mp_suitcase_pickup");
                            GetItem(player, obj.Model);
                            obj.SetField("used", 1);
                        }
                        else if (player.GetAmmoCount(FindWepbyModel(obj.Model)) != GSCFunctions.WeaponStartAmmo(FindWepbyModel(obj.Model)))
                        {
                            player.PlayLocalSound("scavenger_pack_pickup");
                            player.GiveStartAmmo(FindWepbyModel(obj.Model));
                            obj.SetField("used", 1);
                        }
                    }

                    AfterDelay(30000, () =>
                    {
                        if (obj.GetField<int>("used") == 1)
                            obj.SetField("used", 0);
                    });
                }
            });
        }        

        public static void GetItem(Entity player, string modelname)
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
        public static void GivePerkHud(Entity player, string name, string perk, string color)
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
        public static void ShowPerksHud(Entity player, Entity perkfield)
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
        public static void SlotState(Entity player, Entity perkfield)
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
    }
}
