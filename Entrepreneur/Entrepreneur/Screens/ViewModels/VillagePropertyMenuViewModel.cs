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
				return "You arrive to the village of " + this._acreProperties.Settlement.Name + " and ask the local elders for investment opportunities.";
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
				return $"One acre of land costs {this._acreProperties.PricePerAcre}.";
			}
		}
		[DataSourceProperty]
		public string ProductionDescription
		{
			get
			{
				return $"One acre of land produces {this._acreProperties.ProductionValue}.";
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
				return $"Sell margin = +{(int)(this._acreProperties.AcreBuyPercentage * 100)}%";
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
				return $"Buy for {this._acreProperties.AcreSellPrice}";
			}
		}

		[DataSourceProperty]
		public string SellDescription
		{
			get
			{
				return $"Sell for {this._acreProperties.AcreBuyPrice}";
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
				else InformationManager.DisplayMessage(new InformationMessage("You dont have enouph denars to buy this acre."));
			}
			else InformationManager.DisplayMessage(new InformationMessage("There are no available acres to buy."));
		}
		private void SellAcre()
		{
			int buyPrice = this._acreProperties.AcreSellPrice;
			if (this._acreProperties.playerAcres > 0)
			{
				this._acreProperties.sellAcre();
				GiveGoldAction.ApplyForSettlementToCharacter(Settlement.CurrentSettlement, Hero.MainHero, buyPrice);
				this.RefreshProperties();
			}
			else InformationManager.DisplayMessage(new InformationMessage("You have no acres to sell."));
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
		}
	}
}
