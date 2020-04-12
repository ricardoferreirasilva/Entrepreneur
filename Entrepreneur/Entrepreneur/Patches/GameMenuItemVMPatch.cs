using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu;

namespace Entrepreneur.Patches
{
    [HarmonyPatch(typeof(GameMenuItemVM),"OptionLeaveType", MethodType.Getter)]
    class GameMenuItemVMPatch
    {
        [HarmonyPostfix]
        public static int OptionLeaveType(int __result, GameMenuItemVM __instance)
        {
            if (__instance.Item.Equals("Appraise land"))
            {
                __result = 50;
                return __result;
            }
            else return __result;
        }
    }
}
