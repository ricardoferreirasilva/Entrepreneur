using Entrepreneur.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Entrepreneur.Screens.ViewModels
{
    class VillagePropertyMenuViewModel : ViewModel
    {
		private AcreProperties _acreProperties;

		[DataSourceProperty]
		public string VillageDescription
		{
			get
			{
				return "You arrive to the village of " + this._acreProperties.Settlement.Name + ".";
			}
		}

		[DataSourceProperty]
		public string AcreDescription1
		{
			get
			{
				return $"There are {this._acreProperties.totalAcres} total acres of land. The villagers are farming {this._acreProperties.takenAcres} of them for their lord and {this._acreProperties.playerAcres} for you.";
			}
		}

		[DataSourceProperty]
		public string AcreDescription2
		{
			get
			{
				return $"There are currently {this._acreProperties.AvailableAcres} acres of land for sale.";
			}
		}

		[DataSourceProperty]
		public string PriceDescription
		{
			get
			{
				return $"One plot of land is worth {this._acreProperties.PricePerAcre}.";
			}
		}
		[DataSourceProperty]
		public string ProductionDescription
		{
			get
			{
				return $"One plot of land produces {this._acreProperties.ProductionValue} per day.";
			}
		}
		[DataSourceProperty]
		public string RelationsDescription
		{
			get
			{
				return $"Relation with village = {this._acreProperties.RelationWithPlayer}.";
			}
		}
		[DataSourceProperty]
		public string SellMarginDescription
		{
			get
			{
				return $"Buy margin = +{(int)(this._acreProperties.AcreBuyPercentage * 100)}%";
			}
		}
		[DataSourceProperty]
		public string BuyMarginDescription
		{
			get
			{
				return $"Sell margin = -{(int)(this._acreProperties.AcreSellPercentage * 100)}%";
			}
		}

		[DataSourceProperty]
		public string BuyDescription
		{
			get
			{
				return $"{this._acreProperties.AcreSellPrice}";
			}
		}

		[DataSourceProperty]
		public string SellDescription
		{
			get
			{
				return $"{this._acreProperties.AcreBuyPrice}";
			}
		}

		[DataSourceProperty]
		public string TotalRevenueDescription
		{
			get
			{
				return $"Total daily player revenue here = {this._acreProperties.VillagePlayerRevenue}";
			}
		}

		[DataSourceProperty]
		public int PlayerGold
		{
			get
			{
				return Hero.MainHero.Gold;
			}
		}

		[DataSourceProperty]
		public String AvailablePlots
		{
			get
			{
				int availableAcres = this._acreProperties.totalAcres - this._acreProperties.takenAcres + this._acreProperties.playerAcres;
				return "Available plots: " + availableAcres.ToString();
			}
		}

		[DataSourceProperty]
		public String OwnedPlots
		{
			get
			{
				int ownedPlots = this._acreProperties.playerAcres;
				return "Owned plots: " + ownedPlots.ToString();
			}
		}

		public VillagePropertyMenuViewModel(ref AcreProperties acreProperties)
		{
			this._acreProperties = acreProperties;
		}
		private void ExitVillagePropertyMenu()
		{
			ScreenManager.PopScreen();
		}
		private void BuyAcre()
		{
			int buyPrice = this._acreProperties.AcreSellPrice;
			if (this._acreProperties.AvailableAcres > 0)
			{
				if (Hero.MainHero.Gold >= buyPrice)
				{
					this._acreProperties.buyAcre();
					GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, Settlement.CurrentSettlement, buyPrice);
					this.RefreshProperties();
				}
				else InformationManager.DisplayMessage(new InformationMessage("You dont have enouph gold to buy this plot."));
			}
			else InformationManager.DisplayMessage(new InformationMessage("There are no plots acres to buy."));
		}
		private void SellAcre()
		{
			int sellPrice = this._acreProperties.AcreBuyPrice;
			if (this._acreProperties.playerAcres > 0)
			{
				this._acreProperties.sellAcre();
				GiveGoldAction.ApplyForSettlementToCharacter(Settlement.CurrentSettlement, Hero.MainHero, sellPrice);
				this.RefreshProperties();
			}
			else InformationManager.DisplayMessage(new InformationMessage("You have no plots to sell."));
		}

		private void RefreshProperties()
		{
			OnPropertyChanged("VillageDescription");
			OnPropertyChanged("AcreDescription1");
			OnPropertyChanged("AcreDescription2");
			OnPropertyChanged("SellDescription");
			OnPropertyChanged("BuyDescription");
			OnPropertyChanged("PriceDescription");
			OnPropertyChanged("ProductionDescription");
			OnPropertyChanged("RelationsDescription");
			OnPropertyChanged("SellMarginDescription");
			OnPropertyChanged("BuyMarginDescription");
			OnPropertyChanged("TotalRevenueDescription");
			OnPropertyChanged("PlayerGold");
			OnPropertyChanged("AvailablePlots");
			OnPropertyChanged("OwnedPlots");
		}
	}
}
