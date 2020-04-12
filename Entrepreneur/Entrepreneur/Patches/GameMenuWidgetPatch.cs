using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets;
namespace Entrepreneur.Patches
{
    [HarmonyPatch(typeof(GameMenuItemWidget), "SetLeaveTypeIcon")]
    class GameMenuWidgetPatch
    {
        [HarmonyPostfix]
        private static void SetLeaveTypeIcon(GameMenuItemWidget __instance, int type)
        {
            if(type == 50)
            {
                __instance.LeaveTypeIcon.SetState("VillageProperty");
                __instance.LeaveTypeIcon.IsVisible = true;
            }

        }
    }
}
