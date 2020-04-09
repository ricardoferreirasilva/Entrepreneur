using Entrepreneur.Behaviours;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Localization;

namespace Entrepreneur.Patches
{
    [HarmonyPatch(typeof(DefaultClanFinanceModel), "CalculateClanIncome")]
    class ClanFinancialModelCalculateIncomePatch
    {
        [HarmonyPostfix]
        public static void Postfix(
         Clan clan,
         ref ExplainedNumber goldChange,
         bool applyWithdrawals = false)
        {
            goldChange.Add(EntrepreneurCampaignBehaviour.Instance.TotalPlayerRevenue, new TextObject("Revenue from acres"));
        }
    }
}
