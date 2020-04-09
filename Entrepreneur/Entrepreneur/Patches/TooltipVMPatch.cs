using HarmonyLib;
using System;
using System.Runtime.InteropServices;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Entrepreneur.Behaviours;

namespace Entrepreneur.Patches
{
    [HarmonyPatch(typeof(TooltipVM), "OpenTooltip")]
    public class TooltipVMPatch
    {
        [HarmonyPostfix]
        private static void UpdateTooltipPostfix(TooltipVM __instance, Type type, object[] args)
        {
            try
            {
                if (type.ToString().Equals("TaleWorlds.CampaignSystem.PartyBase"))
                {
                    int partyID = (int)args[0];
                    int unknown_1 = (int)args[1];
                    Boolean unkown_2 = (Boolean)args[2];
                    PartyTooltipPostfix(__instance, partyID);

                }
                else if (type.ToString().Equals("System.Collections.Generic.List`1[TaleWorlds.Core.ViewModelCollection.TooltipProperty]"))
                {
                    List<TooltipProperty> list = (List<TooltipProperty>)args[0];
                    InterfaceTooltipPostfix(__instance, list);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                Trace.WriteLine(ex.StackTrace);
                
            }

        }
        private static void PartyTooltipPostfix(TooltipVM __instance, int partyID)
        {
            int index=0;
            PartyBase party = PartyBase.FindParty(partyID);
            if (party.IsSettlement)
            {
                if (party.Settlement.IsVillage)
                {
                    foreach (var property in __instance.TooltipPropertyList)
                    {
                        if (property.DefinitionLabel.Equals("Primary Production"))
                        {
                            index = __instance.TooltipPropertyList.IndexOf(property);
                        }
                    }
                    int playerAcres = EntrepreneurCampaignBehaviour.Instance.GetVillagePlayerAcres(party.Settlement.StringId);
                    int playerRevenue = EntrepreneurCampaignBehaviour.Instance.GetVillagePlayerRevenue(party.Settlement.StringId);
                    if(playerAcres > 0)
                    {
                        __instance.TooltipPropertyList.Insert(index + 1, new TooltipProperty("Owned farm acres", playerAcres.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
                        __instance.TooltipPropertyList.Insert(index + 2, new TooltipProperty("Revenue from farms", playerRevenue.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
                    }
             
                }
            }
        }
        private static void InterfaceTooltipPostfix(TooltipVM __instance, List<TooltipProperty> tooltipPropertyList)
        {
            TooltipProperty topTooltipProperty = tooltipPropertyList[0];
            if(topTooltipProperty.DefinitionLabel.Equals("Current Denars"))
            {
                CurrentDenarsTooltipPostfix(__instance, tooltipPropertyList);
            }
        }
        private static void CurrentDenarsTooltipPostfix(TooltipVM __instance, List<TooltipProperty> tooltipPropertyList)
        {
            int totalProperties = __instance.TooltipPropertyList.Count;
            if(EntrepreneurCampaignBehaviour.Instance.TotalPlayerRevenue > 0)
            {
                string acresRevenue = "+" + EntrepreneurCampaignBehaviour.Instance.TotalPlayerRevenue;
                __instance.TooltipPropertyList.Insert(totalProperties - 2, new TooltipProperty("Revenue from acres", acresRevenue, 0, false, TooltipProperty.TooltipPropertyFlags.None));
                string currentDailyChange = __instance.TooltipPropertyList.Last().ValueLabel;
                int currentDailyChangeInt = Int32.Parse(currentDailyChange);
                int newDailyChange = currentDailyChangeInt + EntrepreneurCampaignBehaviour.Instance.TotalPlayerRevenue;
                if (newDailyChange > 0) __instance.TooltipPropertyList.Last().ValueLabel = "+" + newDailyChange;
                else __instance.TooltipPropertyList.Last().ValueLabel = newDailyChange.ToString();
            }
        }
    }
}