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
using TaleWorlds.SaveSystem;
namespace Entrepreneur.Classes
{
    public class AcreProperties
    {
        [SaveableField(1)]
        public int totalAcres;
        [SaveableField(2)]
        public int takenAcres;
        [SaveableField(3)]
        public int playerAcres = 0;
        [SaveableField(4)]
        public string settlementID;
        public AcreProperties(string settlementID, int totalAcres, int takenAcres)
        {
            this.settlementID = settlementID;
            this.totalAcres = totalAcres;
            this.takenAcres = takenAcres;
        }

        private Settlement getSelf()
        {
            return Settlement.Find(this.settlementID);
        }

        public int PricePerAcre
        {
            get {
                double availability = ((double)(this.playerAcres + this.takenAcres) / (double)this.totalAcres);
                double availabilityYield = availability * 1500 + 500;
                int pricePerAcre = Convert.ToInt32(availabilityYield + this.ProductionValue * 3);
                return pricePerAcre;
            }
        }
        public int ProductionValue
        {
            get {
                Settlement settlement = this.getSelf();
                var products = settlement.Village.VillageType.Productions;
                int totalProductionValue = 0;
                foreach (var (item, amount) in products)
                {
                    totalProductionValue += (int)amount * item.Value;
                }
                return totalProductionValue;
            }
        }
        public int AvailableAcres
        {
            get
            {
                return (this.totalAcres - (this.playerAcres + this.takenAcres));
            }
        }
        private double AcreBuyPercentage
        {
            get
            {
                double points = 10;
                return (points / (double) 100);
            }
        }
        private double AcreSellPercentage
        {
            get
            {
                double points = 10;
                return (points / (double) 100);
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
