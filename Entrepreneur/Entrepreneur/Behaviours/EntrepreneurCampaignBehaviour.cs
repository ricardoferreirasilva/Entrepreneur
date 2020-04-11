using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using Entrepreneur.Classes;
using TaleWorlds.SaveSystem;
using TaleWorlds.Engine.Screens;
using Entrepreneur.Screens;

namespace Entrepreneur.Behaviours
{
    class EntrepreneurCampaignBehaviour : CampaignBehaviorBase
    {
        Dictionary<string, VillageData> _villageData;
        public static EntrepreneurCampaignBehaviour MyInstance {
            get {
                return Campaign.Current.GetCampaignBehavior<EntrepreneurCampaignBehaviour>();
            }
        }
        public static readonly EntrepreneurCampaignBehaviour Instance = new EntrepreneurCampaignBehaviour();
        public override void RegisterEvents()
        {
            _villageData = new Dictionary<string, VillageData>();
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
            CampaignEvents.RaidCompletedEvent.AddNonSerializedListener(this, new Action<BattleSideEnum, MapEvent>(this.OnRaidCompleted));
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("acrePropertiesMap", ref _villageData);

        }
        private void OnRaidCompleted(BattleSideEnum battle, MapEvent mapEvent) {
            if (mapEvent.IsRaid)
            {
                Settlement settlement = mapEvent.MapEventSettlement;
                if (settlement.IsVillage)
                {
                    VillageData settlementAcreProperties;
                    this._villageData.TryGetValue(settlement.StringId, out settlementAcreProperties);
                    if (settlementAcreProperties.playerAcres > 0)
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
                VillageData settlementAcreProperties;
                this._villageData.TryGetValue(Settlement.CurrentSettlement.StringId, out settlementAcreProperties);
                // Edit the View Model here? or pass data into VillagePropertyScreen and do it there?
                ScreenManager.PushScreen(new VillagePropertyScreen(ref settlementAcreProperties));
            }, false, 4);
        }
        private void populateSettlementsWithProperty()
        {
            if (_villageData.Count == 0)
            {
                Random random = new Random();
                foreach (Settlement settlement in Settlement.All)
                {
                    if (settlement.IsVillage)
                    {
                        int availableAcres = random.Next(10, 100);
                        int takenAcres = random.Next(5, availableAcres - (availableAcres / 2));
                        string settlementID = settlement.StringId;
                        _villageData.Add(settlementID, new VillageData(settlementID, availableAcres, takenAcres));
                    }
                }
            }
        }
        public Dictionary<string, VillageData> VillageData
        {
            get
            {
                return this._villageData;
            }
        }
        public class MySaveDefiner : SaveableTypeDefiner
        {
            public MySaveDefiner() : base(52357711)
            {
            }

            protected override void DefineClassTypes()
            {
                AddClassDefinition(typeof(VillageData), 52357712);
            }

            protected override void DefineContainerDefinitions()
            {
                ConstructContainerDefinition(typeof(Dictionary<string, VillageData>));
            }
        }

    }
}
