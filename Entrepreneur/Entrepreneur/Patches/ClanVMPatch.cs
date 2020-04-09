using Entrepreneur.Behaviours;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;

namespace Entrepreneur.Patches
{
    [HarmonyPatch(typeof(ClanVM), "get_TotalIncome")]
    public class ClanVMPatch
    {
        [HarmonyPostfix]
        static int Postfix(int value)
        {
            try
            {
                return value + EntrepreneurCampaignBehaviour.Instance.TotalPlayerRevenue;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                Trace.WriteLine(ex.StackTrace);
                return value;

            }
        }

    }
}
