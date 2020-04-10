using Entrepreneur.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Entrepreneur.Screens.ViewModels
{
    public class EntrepreneurMapVM : ViewModel
    {
        private static readonly TextObject influenceHintStr = new TextObject("{=RVPidk5a}Influence", (Dictionary<string, TextObject>)null);
        private float _refreshTimeSpan = 2f;
        private readonly INavigationHandler _navigationHandler;
        private readonly IMapStateHandler _mapStateHandler;
        private readonly TextObject _needToBePartOfKingdomText;
        private readonly TextObject _cannotGatherWhileInEventText;
        private readonly TextObject _needToBeLeaderToManageText;
        private readonly TextObject _mercenaryCannotManageText;
        private readonly Action _openArmyManagement;
        private MapBarShortcuts _shortcuts;
        private string _latestTutorialElementID;
        private bool _isGatherArmyVisible;
        private MapInfoVM _mapInfo;
        private MapTimeControlVM _mapTimeControl;
        private EntrepreneurMapNavigationVM _mapNavigation;
        private HintViewModel _gatherArmyHint;
        private bool _isEnabled;
        private bool _isCameraCentered;
        private bool _canGatherArmy;
        private bool _isInInfoMode;
        private string _currentScreen;
        private ElementNotificationVM _tutorialNotification;

        public EntrepreneurMapVM(
          INavigationHandler navigationHandler,
          IMapStateHandler mapStateHandler,
          MapBarShortcuts shortcuts,
          Action openArmyManagement)
        {
            this._shortcuts = shortcuts;
            this._openArmyManagement = openArmyManagement;
            this._navigationHandler = navigationHandler;
            this._mapStateHandler = mapStateHandler;
            this._refreshTimeSpan = Campaign.Current.GetSimplifiedTimeControlMode() == CampaignTimeControlMode.UnstoppableFastForward ? 0.1f : 2f;
            this._needToBePartOfKingdomText = GameTexts.FindText("str_need_to_be_a_part_of_kingdom", (string)null);
            this._cannotGatherWhileInEventText = GameTexts.FindText("str_cannot_gather_army_while_in_event", (string)null);
            this._needToBeLeaderToManageText = GameTexts.FindText("str_need_to_be_leader_of_army_to_manage", (string)null);
            this._mercenaryCannotManageText = GameTexts.FindText("str_mercenary_cannot_manage_army", (string)null);
            this.TutorialNotification = new ElementNotificationVM();
            this.MapInfo = new MapInfoVM();
            this.MapTimeControl = new MapTimeControlVM(shortcuts, new Action(this.OnTimeControlChange));
            this.MapNavigation = new EntrepreneurMapNavigationVM(navigationHandler, shortcuts);
            this.GatherArmyHint = new HintViewModel();
            this.OnRefresh();
            this.IsEnabled = true;
            Game.Current.EventManager.RegisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
            this.MapInfo.RefreshValues();
            this.MapTimeControl.RefreshValues();
            this.MapNavigation.RefreshValues();
        }

        public void OnRefresh()
        {
            this.MapInfo.Refresh();
            this.MapTimeControl.Refresh();
            this.MapNavigation.Refresh();
        }

        public void Tick(float dt)
        {
            int simplifiedTimeControlMode = (int)Campaign.Current.GetSimplifiedTimeControlMode();
            this._refreshTimeSpan -= dt;
            if ((double)this._refreshTimeSpan < 0.0)
            {
                this.OnRefresh();
                this._refreshTimeSpan = simplifiedTimeControlMode == 2 ? 0.1f : 0.2f;
            }
            this.MapInfo.Tick();
            this.MapTimeControl.Tick();
            this.MapNavigation.Tick();
            if (this._mapStateHandler != null)
                this.IsCameraCentered = this._mapStateHandler.IsCameraLockedToPlayerParty();
            this.IsGatherArmyVisible = this.GetIsGatherArmyVisible();
            this.GatherArmyHint.HintText = "";
            if (!this.IsGatherArmyVisible)
                return;
            string reasonText;
            this.CanGatherArmy = this.CanGatherArmyWithReason(out reasonText);
            this.GatherArmyHint.HintText = reasonText;
        }

        private bool CanGatherArmyWithReason(out string reasonText)
        {
            if (!Hero.MainHero.MapFaction.IsKingdomFaction)
            {
                reasonText = this._needToBePartOfKingdomText.ToString();
                return false;
            }
            if (MobileParty.MainParty.MapEvent != null)
            {
                reasonText = this._cannotGatherWhileInEventText.ToString();
                return false;
            }
            if (MobileParty.MainParty.Army != null && MobileParty.MainParty.Army.LeaderParty != MobileParty.MainParty)
            {
                reasonText = this._needToBeLeaderToManageText.ToString();
                return false;
            }
            if (Clan.PlayerClan.IsUnderMercenaryService)
            {
                reasonText = this._mercenaryCannotManageText.ToString();
                return false;
            }
            reasonText = "";
            return true;
        }

        private bool GetIsGatherArmyVisible()
        {
            return this.MapTimeControl.IsInMap && (MobileParty.MainParty?.Army == null && !Hero.MainHero.IsPrisoner && MobileParty.MainParty.MapEvent == null) && this.MapTimeControl.IsCenterPanelEnabled;
        }

        private void OnTimeControlChange()
        {
            this._refreshTimeSpan = Campaign.Current.GetSimplifiedTimeControlMode() == CampaignTimeControlMode.UnstoppableFastForward ? 0.1f : 2f;
        }

        private void ExecuteResetCamera()
        {
            this._mapStateHandler?.FastMoveCameraToMainParty();
        }

        private void ExecuteArmyManagement()
        {
            this._openArmyManagement();
        }

        private void OnTutorialNotificationElementIDChange(TutorialNotificationElementChangeEvent obj)
        {
            if (!(obj.NewNotificationElementID != this._latestTutorialElementID))
                return;
            if (this._latestTutorialElementID != null)
                this.TutorialNotification.ElementID = string.Empty;
            this._latestTutorialElementID = obj.NewNotificationElementID;
            if (this._latestTutorialElementID == null)
                return;
            this.TutorialNotification.ElementID = this._latestTutorialElementID;
            if (!(this._latestTutorialElementID == "PartySpeedLabel") || this.MapInfo.IsInfoBarExtended)
                return;
            this.MapInfo.IsInfoBarExtended = true;
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            Game.Current.EventManager.UnregisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
        }

        [DataSourceProperty]
        public MapInfoVM MapInfo
        {
            get
            {
                return this._mapInfo;
            }
            set
            {
                if (value == this._mapInfo)
                    return;
                this._mapInfo = value;
                this.OnPropertyChanged(nameof(MapInfo));
            }
        }

        [DataSourceProperty]
        public MapTimeControlVM MapTimeControl
        {
            get
            {
                return this._mapTimeControl;
            }
            set
            {
                if (value == this._mapTimeControl)
                    return;
                this._mapTimeControl = value;
                this.OnPropertyChanged(nameof(MapTimeControl));
            }
        }

        [DataSourceProperty]
        public EntrepreneurMapNavigationVM MapNavigation
        {
            get
            {
                return this._mapNavigation;
            }
            set
            {
                if (value == this._mapNavigation)
                    return;
                this._mapNavigation = value;
                this.OnPropertyChanged(nameof(MapNavigation));
            }
        }

        [DataSourceProperty]
        public bool IsGatherArmyVisible
        {
            get
            {
                return this._isGatherArmyVisible;
            }
            set
            {
                if (value == this._isGatherArmyVisible)
                    return;
                this._isGatherArmyVisible = value;
                this.OnPropertyChanged(nameof(IsGatherArmyVisible));
            }
        }

        [DataSourceProperty]
        public bool IsInInfoMode
        {
            get
            {
                return this._isInInfoMode;
            }
            set
            {
                if (value == this._isInInfoMode)
                    return;
                this._isInInfoMode = value;
                this.OnPropertyChanged(nameof(IsInInfoMode));
            }
        }

        [DataSourceProperty]
        public bool IsEnabled
        {
            get
            {
                return this._isEnabled;
            }
            set
            {
                if (value == this._isEnabled)
                    return;
                this._isEnabled = value;
                this.OnPropertyChanged(nameof(IsEnabled));
            }
        }

        [DataSourceProperty]
        public bool CanGatherArmy
        {
            get
            {
                return this._canGatherArmy;
            }
            set
            {
                if (value == this._canGatherArmy)
                    return;
                this._canGatherArmy = value;
                this.OnPropertyChanged(nameof(CanGatherArmy));
            }
        }

        [DataSourceProperty]
        public HintViewModel GatherArmyHint
        {
            get
            {
                return this._gatherArmyHint;
            }
            set
            {
                if (value == this._gatherArmyHint)
                    return;
                this._gatherArmyHint = value;
                this.OnPropertyChanged(nameof(GatherArmyHint));
            }
        }

        [DataSourceProperty]
        public bool IsCameraCentered
        {
            get
            {
                return this._isCameraCentered;
            }
            set
            {
                if (value == this._isCameraCentered)
                    return;
                this._isCameraCentered = value;
                this.OnPropertyChanged(nameof(IsCameraCentered));
            }
        }

        [DataSourceProperty]
        public string CurrentScreen
        {
            get
            {
                return this._currentScreen;
            }
            set
            {
                if (!(this._currentScreen != value))
                    return;
                this._currentScreen = value;
                this.OnPropertyChanged(nameof(CurrentScreen));
            }
        }

        [DataSourceProperty]
        public ElementNotificationVM TutorialNotification
        {
            get
            {
                return this._tutorialNotification;
            }
            set
            {
                if (value == this._tutorialNotification)
                    return;
                this._tutorialNotification = value;
                this.OnPropertyChanged(nameof(TutorialNotification));
            }
        }
    }
}
