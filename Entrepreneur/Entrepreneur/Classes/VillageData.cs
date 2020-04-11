using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.Character;
using TaleWorlds.SaveSystem;
using static TaleWorlds.CampaignSystem.SettlementComponent;

namespace Entrepreneur.Classes
{
    public class VillageData
    {
        [SaveableField(1)]
        public int totalAcres;
        [SaveableField(2)]
        public int takenAcres;
        [SaveableField(3)]
        public int playerAcres = 0;
        [SaveableField(4)]
        public string settlementID;
        public VillageData(string settlementID, int totalAcres, int takenAcres)
        {
            this.settlementID = settlementID;
            this.totalAcres = totalAcres;
            this.takenAcres = takenAcres;
        }

        private Settlement getSelf()
        {
            return Settlement.Find(this.settlementID);
        }

        public Settlement Settlement
        {
            get
            {
                return Settlement.Find(this.settlementID);
            }
        }

        public int PricePerAcre
        {
            get {
                double availability = ((double)(this.playerAcres + this.takenAcres) / (double)this.totalAcres);


                int pricePerAcre = Convert.ToInt32(this.ProductionValue * 50 + availability * this.ProductionValue * 30);

                return pricePerAcre;
            }
        }
        public int ProductionValue
        {
            get {
                double valueReducer = 3;
                Settlement settlement = this.getSelf();
                var products = settlement.Village.VillageType.Productions;
                int totalProductionValue = 0;
                float prosperity = settlement.Village.Bound.Prosperity;
                foreach (var (item, amount) in products)
                {
                    totalProductionValue += (int) amount * item.Value;
                }
                totalProductionValue = (int)((totalProductionValue / 7) / valueReducer);

                //If village is deserted, production is 30%.
                if (settlement.IsRebelling || settlement.IsStarving)
                {
                    totalProductionValue = (int)(totalProductionValue * 0.5d);
                }

                //If village is deserted, production is 10%.
                if (settlement.Village.IsDeserted)
                {
                    totalProductionValue = (int) (totalProductionValue * 0.1d);
                }

                // If settlement is raided then its not producing.
                if (settlement.IsRaided || settlement.IsUnderRaid || settlement.IsUnderSiege)
                {
                    totalProductionValue = 0;
                }
                return totalProductionValue;
            }
        }
        public float RelationWithPlayer{
            get{
                Settlement settlement = this.getSelf();
                var heroes = settlement.HeroesWithoutParty;
                var relations = new List<float>();
                foreach (var hero in heroes)
                {
                    float relationWithPlayer = hero.GetRelationWithPlayer();
                    relations.Add(relationWithPlayer);
                }
                relations.Average();
                if (relations.Count > 0) return relations.Average();
                else return 0f;
            }
        }

        public int AvailableAcres
        {
            get
            {
                return (this.totalAcres - (this.playerAcres + this.takenAcres));
            }
        }
        public int VillagePlayerRevenue
        {
            get
            {
                return (this.playerAcres * this.ProductionValue);
            }
        }

        // Percentage used for buying from the player. Higher percentage, player receives less.
        public double AcreBuyPercentage
        {
            get
            {
                double points = 25;
                float relation = this.RelationWithPlayer;
                Settlement settlement = this.getSelf();
                if (relation > 0 && relation <= 40)
                {
                    points -= (int) Math.Round(relation / (float) 2);
                }
                if (relation < 0)
                {
                    points += (int) Math.Round(relation*-1);
                }

                
                //If village is rebelling or starving, buy percentage increases by 10.
                if (settlement.IsRebelling || settlement.IsStarving)
                {
                    points += 10;
                }

                //If village is deserted, buy percentage increases by 10.
                if (settlement.Village.IsDeserted)
                {
                    points += 10;
                }

                //If village is raided, buy percentage increases by 50.
                if (settlement.IsRaided || settlement.IsUnderRaid || settlement.IsUnderSiege)
                {
                    points += 50;
                }
                
                return (points / (double) 100);
            }
        }
        // Percentage used for selling to the player. Higher percentage, player pays more.
        public double AcreSellPercentage
        {
            get
            {
                double points = 25;
                float relation = this.RelationWithPlayer;
                Settlement settlement = this.getSelf();
                if(relation > 0 && relation <= 40)
                {
                    points -= (int)Math.Round(relation / (float)2); ;
                }
                if (relation < 0)
                {
                    points += (int)Math.Round(relation*-1);
                }
                
                //If village is rebelling or starving, buy percentage increases by 10.
                if (settlement.IsRebelling || settlement.IsStarving)
                {
                    points -= 5;
                }

                //If village is deserted, buy percentage increases by 10.
                if (settlement.Village.IsDeserted)
                {
                    points -= 10;
                }
                //If village is raided, buy percentage increases by 50.
                if (settlement.IsRaided || settlement.IsUnderRaid || settlement.IsUnderSiege)
                {
                    points -= 20;
                }
                
                return (points / (double)100);
            }
        }
        // Price that the village uses to buy acres from the player.
        public int AcreBuyPrice{
            get
            {
                return Convert.ToInt32(this.PricePerAcre - ( (double) this.PricePerAcre * this.AcreBuyPercentage));
            }
        }

        // Price that the village uses to sell acres to the player.
        public int AcreSellPrice
        {
            get
            {
                return Convert.ToInt32(this.PricePerAcre + ( (double) this.PricePerAcre * this.AcreSellPercentage));
            }
        }
        public void buyAcre()
        {
            this.playerAcres++;
        }
        public void sellAcre()
        {
            this.playerAcres--;
        }
    }
}
