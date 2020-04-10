// Decompiled with JetBrains decompiler
// Type: SandBox.GauntletUI.Map.GauntlerMapBarGlobalLayer
// Assembly: SandBox.GauntletUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 72CAF7B8-15CC-4D97-B625-9F07AB3F0458
// Assembly location: A:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\SandBox\bin\Win64_Shipping_Client\SandBox.GauntletUI.dll

using SandBox.View.Map;
using SandBox.GauntletUI;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using System.Diagnostics;
using TaleWorlds.MountAndBlade.View.Missions;
using Entrepreneur.Screens.ViewModels;

namespace SandBox.GauntletUI.Map
{
    public class EntrepreneurMapBarGlobalLayer : GlobalLayer
    {
        private EntrepreneurMapVM _mapDataSource;
        private GauntletLayer _gauntletLayer;
        private GauntletMovie _movie;
        private MapScreen _mapScreen;
        private MapNavigationHandler _mapNavigationHandler;
        private EncyclopediaScreenManager _encyclopediaManager;
        private ArmyManagementVM _armyManagementVM;
        private GauntletMovie _gauntletArmyManagementMovie;
        private CampaignTimeControlMode _timeControlModeBeforeArmyManagementOpened;

        public void Initialize(MapScreen mapScreen)
        {
            this._mapScreen = mapScreen;
            this._mapNavigationHandler = new MapNavigationHandler();
            this._mapDataSource = new EntrepreneurMapVM((INavigationHandler)this._mapNavigationHandler, (IMapStateHandler)this._mapScreen, new MapBarShortcuts()
            {
                EscapeMenuHotkey = Game.Current.GameTextManager.GetHotKeyGameText("GenericCampaignPanelsGameKeyCategory", "Exit").ToString(),
                CharacterHotkey = Game.Current.GameTextManager.GetHotKeyGameText("GenericCampaignPanelsGameKeyCategory", 30).ToString(),
                QuestHotkey = Game.Current.GameTextManager.GetHotKeyGameText("GenericCampaignPanelsGameKeyCategory", 35).ToString(),
                PartyHotkey = Game.Current.GameTextManager.GetHotKeyGameText("GenericCampaignPanelsGameKeyCategory", 36).ToString(),
                KingdomHotkey = Game.Current.GameTextManager.GetHotKeyGameText("GenericCampaignPanelsGameKeyCategory", 33).ToString(),
                ClanHotkey = Game.Current.GameTextManager.GetHotKeyGameText("GenericCampaignPanelsGameKeyCategory", 34).ToString(),
                InventoryHotkey = Game.Current.GameTextManager.GetHotKeyGameText("GenericCampaignPanelsGameKeyCategory", 31).ToString(),
                FastForwardHotkey = Game.Current.GameTextManager.GetHotKeyGameText("MapHotKeyCategory", 48).ToString(),
                PauseHotkey = Game.Current.GameTextManager.GetHotKeyGameText("MapHotKeyCategory", 46).ToString(),
                PlayHotkey = Game.Current.GameTextManager.GetHotKeyGameText("MapHotKeyCategory", 47).ToString()
            }, new Action(this.OpenArmyManagement));
            this._gauntletLayer = new GauntletLayer(200, "GauntletLayer");
            this.Layer = (ScreenLayer)this._gauntletLayer;
            this._movie = this._gauntletLayer.LoadMovie("MapBar", (ViewModel)this._mapDataSource);
            this._encyclopediaManager = mapScreen.EncyclopediaScreenManager;
        }

        public void OnFinalize()
        {
            this._armyManagementVM?.OnFinalize();
            this._mapDataSource.OnFinalize();
            this._gauntletArmyManagementMovie?.Release();
            this._movie.Release();
            this._armyManagementVM = (ArmyManagementVM)null;
            this._gauntletLayer = (GauntletLayer)null;
            this._mapDataSource = (EntrepreneurMapVM)null;
            this._encyclopediaManager = (EncyclopediaScreenManager)null;
            this._mapScreen = (MapScreen)null;
        }

        public void Refresh()
        {
            this._mapDataSource?.OnRefresh();
        }

        protected override void OnTick(float dt)
        {
            base.OnTick(dt);
            GameState activeState = Game.Current.GameStateManager.ActiveState;
            ScreenBase topScreen1 = ScreenManager.TopScreen;
            switch (topScreen1)
            {
                case MapScreen _:
                case InventoryGauntletScreen _:
                case GauntletPartyScreen _:
                case GauntletCharacterDeveloperScreen _:
                case GauntletClanScreen _:
                case GauntletQuestsScreen _:
                case GauntletKingdomScreen _:
                    this._mapDataSource.IsEnabled = true;
                    this._mapDataSource.CurrentScreen = topScreen1.GetType().Name;
                    bool flag = ScreenManager.TopScreen is MapScreen;
                    this._mapDataSource.MapTimeControl.IsInMap = flag;
                    this.Layer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
                    if (!(activeState is MapState))
                        this._mapDataSource.MapTimeControl.IsCenterPanelEnabled = false;
                    else if (flag)
                    {
                        MapScreen topScreen2 = ScreenManager.TopScreen as MapScreen;
                        topScreen2.IsBarExtended = this._mapDataSource.MapInfo.IsInfoBarExtended;
                        this._mapDataSource.MapTimeControl.IsInRecruitment = topScreen2.IsInRecruitment;
                        this._mapDataSource.MapTimeControl.IsInBattleSimulation = topScreen2.IsInBattleSimulation;
                        this._mapDataSource.MapTimeControl.IsEncyclopediaOpen = this._encyclopediaManager.IsEncyclopediaOpen;
                        this._mapDataSource.MapTimeControl.IsInArmyManagement = topScreen2.IsInArmyManagement;
                        this._mapDataSource.MapTimeControl.IsInTownManagement = topScreen2.IsInTownManagement;
                        this._mapDataSource.MapTimeControl.IsInCampaignOptions = topScreen2.IsInCampaignOptions;
                    }
                    else
                        this._mapDataSource.MapTimeControl.IsCenterPanelEnabled = false;
                    this._mapDataSource.Tick(dt);
                    break;
                default:
                    this._mapDataSource.IsEnabled = false;
                    this.Layer.InputRestrictions.ResetInputRestrictions();
                    break;
            }
        }

        private void OpenArmyManagement()
        {
            if (this._gauntletLayer == null)
                return;
            this._armyManagementVM = new ArmyManagementVM(new Action(this.CloseArmyManagement));
            this._gauntletArmyManagementMovie = this._gauntletLayer.LoadMovie("ArmyManagement", (ViewModel)this._armyManagementVM);
            this._timeControlModeBeforeArmyManagementOpened = Campaign.Current.TimeControlMode;
            Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
            Campaign.Current.SetTimeControlModeLock(true);
            if (!(ScreenManager.TopScreen is MapScreen topScreen))
                return;
            topScreen.IsInArmyManagement = true;
        }

        private void CloseArmyManagement()
        {
            this._gauntletLayer.ReleaseMovie(this._gauntletArmyManagementMovie);
            this._armyManagementVM.OnFinalize();
            Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(TutorialContexts.MapWindow));
            this._gauntletArmyManagementMovie = (GauntletMovie)null;
            this._armyManagementVM = (ArmyManagementVM)null;
            Campaign.Current.SetTimeControlModeLock(false);
            Campaign.Current.TimeControlMode = this._timeControlModeBeforeArmyManagementOpened;
            if (!(ScreenManager.TopScreen is MapScreen topScreen))
                return;
            topScreen.IsInArmyManagement = false;
        }

        internal bool IsEscaped()
        {
            if (this._armyManagementVM == null)
                return false;
            this._armyManagementVM.ExecuteCancel();
            return true;
        }
    }
}
