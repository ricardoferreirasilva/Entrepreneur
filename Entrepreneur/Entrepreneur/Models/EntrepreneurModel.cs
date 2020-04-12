﻿using Entrepreneur.Behaviours;
using Entrepreneur.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
                int stewarding = Hero.MainHero.GetSkillValue(DefaultSkills.Steward);
                int maximumPlots = stewarding / 4;
                return maximumPlots;
            }
        }

        public static int TotalPlayerPlots
        {
            get
            {
                int _totalPlayerPlots = 0;
                EntrepreneurCampaignBehaviour entrepreneur = Campaign.Current.GetCampaignBehavior<EntrepreneurCampaignBehaviour>();
                foreach (var village in entrepreneur.VillageData.Values)
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

        public static void BuyPlot(VillageData villageData)
        {
            EntrepreneurCampaignBehaviour entrepreneur = Campaign.Current.GetCampaignBehavior<EntrepreneurCampaignBehaviour>();

            Dictionary<string, int> itemRequirements = new Dictionary<string, int>();
            itemRequirements.Add("Tools", 5);
            itemRequirements.Add("Hardwood", 5);

            Dictionary<string, int> missingRequirements = new Dictionary<string, int>();
            missingRequirements.Add("Tools", 5);
            missingRequirements.Add("Hardwood", 5);

            Dictionary<ItemRosterElement, int> itemsToRemove = new Dictionary<ItemRosterElement, int>();
            foreach (KeyValuePair<string, int> requirement in itemRequirements)
            {
                IEnumerable<ItemRosterElement> items = Hero.MainHero.PartyBelongedTo.ItemRoster.AsQueryable().Where(item => item.Amount >= requirement.Value && item.EquipmentElement.Item.Name.ToString().Equals(requirement.Key));
                if (items.Count() != 0)
                {
                    int currentAmount = items.First().Amount;
                    itemsToRemove.Add(items.First(), currentAmount - requirement.Value);
                    missingRequirements.Remove(requirement.Key);
                }
            }
            if (missingRequirements.Count == 0)
            {
                Trace.WriteLine("You have everything.");
                foreach (var item in itemsToRemove)
                {
                    // Remove whole stack.
                    Hero.MainHero.PartyBelongedTo.ItemRoster.Remove(item.Key);

                    // Add the difference.
                    Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(item.Key.EquipmentElement.Item, item.Value);
                }
            }
            else
            {
                foreach (KeyValuePair<string, int> requirement in missingRequirements)
                {
                    Trace.WriteLine($"You are missing {requirement.Value} items of {requirement.Key}.");
                }
            }
        }
        public static void SellPlot(VillageData villageData)
        {
            // Getting back our sweet materials.
            var scrapItems = new List<(string, int)> {("Tools", 3)};
            foreach (var scrap in scrapItems)
            {
                ItemObject scrapItem = Items.FindFirst(item => item.Name.ToString().Equals(scrap.Item1));
                Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(scrapItem, scrap.Item2);
            }
        }
    }
}
