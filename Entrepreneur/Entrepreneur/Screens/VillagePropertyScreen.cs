using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using Entrepreneur.Screens.ViewModels;
using Entrepreneur.Classes;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu;

namespace Entrepreneur.Screens
{
	class VillagePropertyScreen : ScreenBase
    {
		private AcreProperties _acreProperties;

		private VillagePropertyMenuViewModel _datasource;

		private GauntletLayer _gauntletLayer;

		private GauntletMovie _movie;

		private bool _firstRender;

		public VillagePropertyScreen(ref AcreProperties acreProperties)
		{
			this._acreProperties = acreProperties;
		}
		protected override void OnInitialize()
		{
			base.OnInitialize();
			_datasource = new VillagePropertyMenuViewModel(ref this._acreProperties);
			_gauntletLayer = new GauntletLayer(100);
			_gauntletLayer.IsFocusLayer = true;
			AddLayer(_gauntletLayer);
			_gauntletLayer.InputRestrictions.SetInputRestrictions();
			ScreenManager.TrySetFocus(_gauntletLayer);
			_movie = _gauntletLayer.LoadMovie("VillagePropertyScreen", _datasource);
			_firstRender = true;
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
		}
	}
}
