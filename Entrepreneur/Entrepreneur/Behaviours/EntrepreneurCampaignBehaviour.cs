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
using TaleWorlds.SaveSystem;
using TaleWorlds.Engine.Screens;
using Entrepreneur.Screens;

namespace Entrepreneur.Behaviours
{
    class EntrepreneurCampaignBehaviour : CampaignBehaviorBase
    {
        Dictionary<string, AcreProperties> acrePropertiesMap = new Dictionary<string, AcreProperties>();
        Dictionary<string, string> testDictionary = new Dictionary<string, string>();
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, this.generateRevenue);
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("acrePropertiesMap", ref acrePropertiesMap);
            
        }
        private void OnSessionLaunched(CampaignGameStarter obj)
        {
            populateSettlementsWithProperty();
            createEntrepeneurVillageMenus(obj);
        }
        private void createEntrepeneurVillageMenus(CampaignGameStarter obj)
        {
            // Enter appraising menu
            obj.AddGameMenuOption("village", "village_enter_entr_option", "Appraise land", (MenuCallbackArgs args) => {
                args.optionLeaveType = GameMenuOption.LeaveType.Trade;
                return true;
            }, (MenuCallbackArgs args) => {
                AcreProperties settlementAcreProperties;
                this.acrePropertiesMap.TryGetValue(Settlement.CurrentSettlement.StringId, out settlementAcreProperties);
                // Edit the View Model here? or pass data into VillagePropertyScreen and do it there?
                ScreenManager.PushScreen(new VillagePropertyScreen(ref settlementAcreProperties));
            }, false, 4);
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
        public class MySaveDefiner : SaveableTypeDefiner
        {
            public MySaveDefiner() : base(10000001)
            {
            }

            protected override void DefineClassTypes()
            {
                AddClassDefinition(typeof(AcreProperties), 1);
            }

            protected override void DefineContainerDefinitions()
            {
                ConstructContainerDefinition(typeof(Dictionary<string, AcreProperties>));
            }
        }

    }
}
