using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Overlay;
using Entrepreneur.Classes;
using System.IO;
using System.Xml.Serialization;

namespace Entrepreneur.Behaviours
{
    class EntrepreneurCampaignBehaviour : CampaignBehaviorBase
    {
        Dictionary<string, AcreProperties> acrePropertiesMap = new Dictionary<string, AcreProperties>();
        string serializedAcrePropertiesMap = "NOT_SERIALIZED";
        Dictionary<string, string> testDictionary = new Dictionary<string, string>();
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
            CampaignEvents.WeeklyTickEvent.AddNonSerializedListener(this,this.generateRevenue);
            
        }

        public override void SyncData(IDataStore dataStore)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            if (dataStore.IsLoading) {
                dataStore.SyncData<string>("serializedAcrePropertiesMap", ref serializedAcrePropertiesMap);
                byte[] bytes = Convert.FromBase64String(serializedAcrePropertiesMap);
                Stream stream = new MemoryStream(bytes);
                acrePropertiesMap = (Dictionary<string, AcreProperties>) binaryFormatter.Deserialize(stream);
            }
            if (dataStore.IsSaving)
            {
                MemoryStream memoryStream = new MemoryStream();
                binaryFormatter.Serialize(memoryStream, acrePropertiesMap);
                string value = Convert.ToBase64String(memoryStream.ToArray());
                serializedAcrePropertiesMap = value;
                dataStore.SyncData<string>("serializedAcrePropertiesMap", ref serializedAcrePropertiesMap);
            }
        }
        private void OnSessionLaunched(CampaignGameStarter obj)
        {
            populateSettlementsWithProperty();
            createEntrepeneurVillageMenus(obj);
        }
        private void createEntrepeneurVillageMenus(CampaignGameStarter obj)
        {
            //Appraising menu.
            string menuDescription = "The village of {CURRENT_SETTLEMENT} has {TOTAL_ACRES} acres of land, from which {TAKEN_ACRES} belong to the villagers and {PLAYER_ACRES} belong to you. The local village elder tells you that one acre is currently worth {ACRE_PRICE} denars. The current production of this village indicates that each acre will yield about {ACRE_YIELD} denars per week.";
            obj.AddGameMenu("village_etr_menu", menuDescription, (MenuCallbackArgs args) =>
            {
                args.optionLeaveType = GameMenuOption.LeaveType.Trade;
                AcreProperties settlementAcreProperties;
                this.acrePropertiesMap.TryGetValue(Settlement.CurrentSettlement.StringId, out settlementAcreProperties);
                MBTextManager.SetTextVariable("ACRE_PRICE", settlementAcreProperties.PricePerAcre, false);
                MBTextManager.SetTextVariable("ACRE_YIELD", settlementAcreProperties.ProductionValue, false);
                MBTextManager.SetTextVariable("ACRE_BUY_PRICE", settlementAcreProperties.AcreBuyPrice, false);
                MBTextManager.SetTextVariable("ACRE_SELL_PRICE", settlementAcreProperties.AcreSellPrice, false);
                MBTextManager.SetTextVariable("PLAYER_ACRES", settlementAcreProperties.playerAcres, false);
                MBTextManager.SetTextVariable("TOTAL_ACRES", settlementAcreProperties.totalAcres, false);
                MBTextManager.SetTextVariable("TAKEN_ACRES", settlementAcreProperties.takenAcres, false);
                MBTextManager.SetTextVariable("CURRENT_SETTLEMENT", Settlement.CurrentSettlement.EncyclopediaLinkWithName, false);
            });

            // Enter appraising menu
            obj.AddGameMenuOption("village", "village_enter_entr_option", "Appraise properties",(MenuCallbackArgs args) => {
                args.optionLeaveType = GameMenuOption.LeaveType.Trade;
                return true;
            }, (MenuCallbackArgs args) => {
                InformationManager.DisplayMessage(new InformationMessage("You are appraising this village for profitable farm plots."));
                GameMenu.SwitchToMenu("village_etr_menu");
            }, false, 4);

            // Buy one acre
            obj.AddGameMenuOption("village_etr_menu", "village_buy_entr_option", "Buy acre for {ACRE_SELL_PRICE}", (MenuCallbackArgs args) => {
                args.optionLeaveType = GameMenuOption.LeaveType.Trade;
                return true;
            }, (MenuCallbackArgs args) => {
                AcreProperties settlementAcreProperties;
                this.acrePropertiesMap.TryGetValue(Settlement.CurrentSettlement.StringId, out settlementAcreProperties);
                int buyPrice = settlementAcreProperties.AcreSellPrice;
                if(settlementAcreProperties.AvailableAcres > 0)
                {
                    if(Hero.MainHero.Gold >= buyPrice)
                    {
                        settlementAcreProperties.buyAcre();
                        GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, Settlement.CurrentSettlement, buyPrice);
                        MBTextManager.SetTextVariable("ACRE_PRICE", settlementAcreProperties.PricePerAcre, false);
                        MBTextManager.SetTextVariable("PLAYER_ACRES", settlementAcreProperties.playerAcres, false);
                        MBTextManager.SetTextVariable("ACRE_BUY_PRICE", settlementAcreProperties.AcreBuyPrice, false);
                        MBTextManager.SetTextVariable("ACRE_SELL_PRICE", settlementAcreProperties.AcreSellPrice, false);
                    }
                    else InformationManager.DisplayMessage(new InformationMessage("You dont have enouph denars to buy this acre."));
                }
                else InformationManager.DisplayMessage(new InformationMessage("There are no available acres to buy."));
            }, true);

            // Sell one acre
            obj.AddGameMenuOption("village_etr_menu", "village_sell_entr_option", "Sell acre for {ACRE_BUY_PRICE}", (MenuCallbackArgs args) => {
                args.optionLeaveType = GameMenuOption.LeaveType.Trade;
                return true;
            }, (MenuCallbackArgs args) => {
                AcreProperties settlementAcreProperties;
                this.acrePropertiesMap.TryGetValue(Settlement.CurrentSettlement.StringId, out settlementAcreProperties);
                int buyPrice = settlementAcreProperties.AcreBuyPrice;
                if (settlementAcreProperties.playerAcres > 0)
                {
                    settlementAcreProperties.sellAcre();
                    GiveGoldAction.ApplyForSettlementToCharacter(Settlement.CurrentSettlement, Hero.MainHero, buyPrice);
                    MBTextManager.SetTextVariable("ACRE_PRICE", settlementAcreProperties.PricePerAcre, false);
                    MBTextManager.SetTextVariable("PLAYER_ACRES", settlementAcreProperties.playerAcres, false);
                    MBTextManager.SetTextVariable("ACRE_BUY_PRICE", settlementAcreProperties.AcreBuyPrice, false);
                    MBTextManager.SetTextVariable("ACRE_SELL_PRICE", settlementAcreProperties.AcreSellPrice, false);
                }
                else InformationManager.DisplayMessage(new InformationMessage("You have no acres to sell."));
            }, true);

            // Leave appraising menu
            obj.AddGameMenuOption("village_etr_menu", "village_leave_entr_option", "Forget it", (MenuCallbackArgs args) => {
                args.optionLeaveType = GameMenuOption.LeaveType.Leave;
                return true;
            }, (MenuCallbackArgs args) => {
                GameMenu.SwitchToMenu("village");
            }, true);
        }
        private void populateSettlementsWithProperty()
        {
            if(acrePropertiesMap.Count == 0)
            {
                Random random = new Random();
                foreach (Settlement settlement in Settlement.All)
                {
                    if (settlement.IsVillage)
                    {
                        int availableAcres = random.Next(10, 100);
                        int takenAcres = random.Next(5, availableAcres - (availableAcres / 2));
                        string settlementID = settlement.StringId;
                        acrePropertiesMap.Add(settlementID, new AcreProperties(settlementID, availableAcres, takenAcres));
                    }
                }
            }
        }

        private void generateRevenue()
        {
            int totalIncome = 0;
            foreach (Settlement settlement in Settlement.All)
            {
                if (settlement.IsVillage)
                {
                    AcreProperties settlementAcreProperties;
                    this.acrePropertiesMap.TryGetValue(settlement.StringId, out settlementAcreProperties);
                    totalIncome += settlementAcreProperties.playerAcres * settlementAcreProperties.ProductionValue;
                }
            }
            Hero.MainHero.ChangeHeroGold(totalIncome);
            InformationManager.DisplayMessage(new InformationMessage("Your land, properties and rents have generated " + totalIncome + " worth of income this week."));

        }
    }
}
