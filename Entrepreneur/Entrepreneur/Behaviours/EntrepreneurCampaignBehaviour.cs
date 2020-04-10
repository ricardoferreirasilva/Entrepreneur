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
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.HUDExtensions;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.Siege;

namespace Entrepreneur.Behaviours
{
    class EntrepreneurCampaignBehaviour : CampaignBehaviorBase
    {
        Dictionary<string, AcreProperties> acrePropertiesMap;
        public static readonly EntrepreneurCampaignBehaviour Instance = new EntrepreneurCampaignBehaviour();
        public override void RegisterEvents()
        {
            acrePropertiesMap  = new Dictionary<string, AcreProperties>();
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
            CampaignEvents.RaidCompletedEvent.AddNonSerializedListener(this, new Action<BattleSideEnum, MapEvent>(this.OnRaidCompleted));
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("acrePropertiesMap", ref acrePropertiesMap);
            
        }
        private void OnRaidCompleted(BattleSideEnum battle, MapEvent mapEvent){
            if (mapEvent.IsRaid)
            {
                Settlement settlement = mapEvent.MapEventSettlement;
                if (settlement.IsVillage)
                {
                    AcreProperties settlementAcreProperties;
                    this.acrePropertiesMap.TryGetValue(settlement.StringId, out settlementAcreProperties);
                    if(settlementAcreProperties.playerAcres > 0)
                    {
                        Random rand = new Random();
                        if (rand.Next(1, 101) <= 25)
                        {
                            settlementAcreProperties.playerAcres--;
                            InformationManager.DisplayMessage(new InformationMessage($"The village of {mapEvent.MapEventSettlement.Name} was raided and one of your properties was destroyed."));
                        }
                        else
                        {
                            InformationManager.DisplayMessage(new InformationMessage($"The village of {mapEvent.MapEventSettlement.Name} was raided but none of your property was destroyed."));
                        }
                    }
  
                }
            }
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
        public int TotalPlayerRevenue
        {
            get
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
                return totalIncome;
            }
        }
        private void generateRevenue()
        {
            Hero.MainHero.ChangeHeroGold(this.TotalPlayerRevenue);
            InformationManager.DisplayMessage(new InformationMessage("Your land, properties and rents have generated " + this.TotalPlayerRevenue + " worth of income today."));
        }
        public int GetVillagePlayerAcres(string stringId)
        {
            AcreProperties settlementAcreProperties;
            this.acrePropertiesMap.TryGetValue(stringId, out settlementAcreProperties);
            return settlementAcreProperties.playerAcres;
        }
        public int GetVillagePlayerRevenue(string stringId)
        {
            AcreProperties settlementAcreProperties;
            this.acrePropertiesMap.TryGetValue(stringId, out settlementAcreProperties);
            return settlementAcreProperties.VillagePlayerRevenue;
        }
        public class MySaveDefiner : SaveableTypeDefiner
        {
            public MySaveDefiner() : base(52357711)
            {
            }

            protected override void DefineClassTypes()
            {
                AddClassDefinition(typeof(AcreProperties), 52357712);
            }

            protected override void DefineContainerDefinitions()
            {
                ConstructContainerDefinition(typeof(Dictionary<string, AcreProperties>));
            }
        }

    }
}
