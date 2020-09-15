using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfinityScript;

namespace Old_School_FFA
{
    public partial class OS_FFA 
    {
        private static int[] RandomNum(int size, int Min, int Max)
        {
            int[] UniqueArray = new int[size];
            Random rnd = new Random();
            int Random;

            for (int i = 0; i < size; i++)
            {
                Random = rnd.Next(Min, Max);

                for (int j = i; j >= 0; j--)
                {

                    if (UniqueArray[j] == Random)
                    { Random = rnd.Next(Min, Max); j = i; }

                }
                UniqueArray[i] = Random;
            }
            return UniqueArray;
        }
        private static Entity SpawnModel(Vector3 origin, string model)
        {
            Entity entity = GSCFunctions.Spawn("script_model", origin);
            entity.SetModel(model);

            return entity;
        }

        private static string FindWepbyModel(string model)
        {
            string nam = null;
            foreach (string info in _ListWeapons)
            {
                string wepname = info.Split(';')[0];
                string modelname = info.Split(';')[1];

                if (0 <= modelname.ToLower().IndexOf(model.ToLower(), StringComparison.InvariantCultureIgnoreCase))
                    nam = wepname;
            }

            return nam;
        }
        private static string FindModelbyWep(string wep)
        {
            string nam = null;
            foreach (string info in _ListWeapons)
            {
                string wepname = info.Split(';')[0];
                string modelname = info.Split(';')[1];

                if (0 <= wepname.ToLower().IndexOf(wep.ToLower(), StringComparison.InvariantCultureIgnoreCase))
                    nam = modelname;
            }

            return nam;
        }
        private static bool CheckSlotperk(Entity player, Entity perkfield)
        {
            string getperk = "";
            string getfield = perkfield.GetField<string>("perk");
            bool hasperk = false;

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

            Vector3 perkslot = player.GetField<Vector3>("perkslot");

            string slot1 = null;
            string slot2 = null;
            string slot3 = null;

            if (perkslot.X != -1f)
                slot1 = os_perks[(int)perkslot.X];
            if (perkslot.Y != -1f)
                slot2 = os_perks[(int)perkslot.Y];
            if (perkslot.Z != -1f)
                slot3 = os_perks[(int)perkslot.Z];

            if (slot1 != null && slot1 == getperk)
                hasperk = true;
            else if (slot2 != null && slot2 == getperk)
                hasperk = true;
            else if (slot3 != null && slot3 == getperk)
                hasperk = true;
            else
                hasperk = false;

            return hasperk;
        }

        private static void DisableSelectClass(Entity player)
        {
            GSCFunctions.ClosePopUpMenu(player, "");
            GSCFunctions.CloseInGameMenu(player);
            player.Notify("menuresponse", "team_marinesopfor", "allies");
            player.OnNotify("joined_team", ent =>
            {
                AfterDelay(500, () => { ent.Notify("menuresponse", "changeclass", "class1"); });
            });
            player.OnNotify("menuresponse", (player2, menu, response) =>
            {
                if (menu.ToString().Equals("class") && response.ToString().Equals("changeclass_marines"))
                {
                    AfterDelay(100, () => { player.Notify("menuresponse", "changeclass", "back"); });
                }
            });
        }
        private static void ServerWelcomeTittle(Entity player, string tittle, float[] rgb)
        {
            player.SetField("welcome", 0);
            player.SpawnedPlayer += new Action(() =>
            {
                if (player.GetField<int>("welcome") == 0)
                {
                    HudElem serverWelcome = HudElem.CreateFontString(player, HudElem.Fonts.HudBig, 1f);
                    serverWelcome.SetPoint("TOPCENTER", "TOPCENTER", 0, 165);
                    serverWelcome.SetText(tittle);
                    serverWelcome.GlowColor = (new Vector3(rgb[0], rgb[1], rgb[2]));
                    serverWelcome.GlowAlpha = 1f;
                    serverWelcome.SetPulseFX(150, 4700, 700);
                    player.SetField("welcome", 1);

                    AfterDelay(5000, () => { serverWelcome.Destroy(); });
                }
            });
        }

        private static Entity SpawnTriggerFX(int fxid, Vector3 pos)
        {
            Vector3 upangles = GSCFunctions.VectorToAngles(new Vector3(0, 0, 1000));
            Vector3 forward = GSCFunctions.AnglesToForward(upangles);
            Vector3 right = GSCFunctions.AnglesToRight(upangles);

            Entity effect = GSCFunctions.SpawnFX(fxid, pos, forward, right);
            GSCFunctions.TriggerFX(effect);

            return effect;
        }

        public static void PrintLog(string message)
        {
            InfinityScript.Log.Write(LogLevel.All, message);
        }
    }
}
