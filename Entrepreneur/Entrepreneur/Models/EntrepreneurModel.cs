using Entrepreneur.Behaviours;
using Entrepreneur.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Entrepreneur.Models
{
    class EntrepreneurModel : GameModel
    {
        public static int MaximumPlots
        {
            get
            {
                int trade = Hero.MainHero.GetSkillValue(DefaultSkills.Trade);
                int maximumPlots = trade / 4;
                return maximumPlots;
            }
        }

        public static int TotalPlayerPlots
        {
            get
            {
                int _totalPlayerPlots = 0;
                EntrepreneurCampaignBehaviour entrepreneur = Campaign.Current.GetCampaignBehavior<EntrepreneurCampaignBehaviour>();
                foreach(var village in entrepreneur.VillageData.Values)
                {
                    _totalPlayerPlots += village.playerAcres;
                }
                return _totalPlayerPlots;
            }
        }

        public static int TotalPlayerRevenue
        {
            get
            {
                int _totalPlayerRevenue = 0;
                EntrepreneurCampaignBehaviour entrepreneur = Campaign.Current.GetCampaignBehavior<EntrepreneurCampaignBehaviour>();
                foreach (var village in entrepreneur.VillageData.Values)
                {
                    _totalPlayerRevenue += village.VillagePlayerRevenue;
                }
                return _totalPlayerRevenue;
            }
        }

        public static int GetVillagePlayerAcres(string villageID)
        {
            EntrepreneurCampaignBehaviour entrepreneur = Campaign.Current.GetCampaignBehavior<EntrepreneurCampaignBehaviour>();
            
            VillageData settlementAcreProperties;
            entrepreneur.VillageData.TryGetValue(villageID, out settlementAcreProperties);
            return settlementAcreProperties.playerAcres;
        }
        public static int GetVillagePlayerRevenue(string villageID)
        {
            EntrepreneurCampaignBehaviour entrepreneur = Campaign.Current.GetCampaignBehavior<EntrepreneurCampaignBehaviour>();
            VillageData settlementAcreProperties;
            entrepreneur.VillageData.TryGetValue(villageID, out settlementAcreProperties);
            return settlementAcreProperties.VillagePlayerRevenue;
        }
    }
}
