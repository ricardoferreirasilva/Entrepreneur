using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

namespace Entrepreneur.Models
{
    public class EntrepreneurMapNavigationVM : ViewModel
    {
        private INavigationHandler _navigationHandler;
        private MapBarShortcuts _shortcuts;
        private string _alertText;
        private bool _skillAlert;
        private bool _questsAlert;
        private bool _partyAlert;
        private bool _kingdomAlert;
        private bool _clanAlert;
        private bool _inventoryAlert;
        private bool _isKingdomEnabled;
        private bool _isClanEnabled;
        private bool _isQuestsEnabled;
        private bool _isEscapeMenuEnabled;
        private bool _isInventoryEnabled;
        private bool _isCharacterDeveloperEnabled;
        private bool _isPartyEnabled;
        private bool _isKingdomActive;
        private bool _isClanActive;
        private bool _isEscapeMenuActive;
        private bool _isQuestsActive;
        private bool _isInventoryActive;
        private bool _isCharacterDeveloperActive;
        private bool _isPartyActive;
        private HintViewModel _encyclopediaHint;
        private HintViewModel _skillsHint;
        private HintViewModel _escapeMenuHint;
        private HintViewModel _questsHint;
        private HintViewModel _inventoryHint;
        private HintViewModel _partyHint;
        private HintViewModel _financeHint;
        private HintViewModel _centerCameraHint;
        private HintViewModel _kingdomHint;
        private HintViewModel _clanHint;
        private HintViewModel _campHint;

        public EntrepreneurMapNavigationVM(INavigationHandler navigationHandler, MapBarShortcuts shortcuts)
        {
            this._navigationHandler = navigationHandler;
            this._shortcuts = shortcuts;
            this.IsKingdomEnabled = Hero.MainHero.MapFaction.IsKingdomFaction;
            this.IsPartyEnabled = true;
            this.IsInventoryEnabled = true;
            this.IsClanEnabled = true;
            this.IsCharacterDeveloperEnabled = true;
            this.IsQuestsEnabled = true;
            this.IsKingdomActive = false;
            this.IsPartyActive = false;
            this.IsInventoryActive = false;
            this.IsClanActive = false;
            this.IsCharacterDeveloperActive = false;
            this.IsQuestsActive = false;
            this.RefreshValues();
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
            this.ClanHint = new HintViewModel();
            this.KingdomHint = new HintViewModel();
            this.EncyclopediaHint = new HintViewModel(GameTexts.FindText("str_encyclopedia", (string)null).ToString(), (string)null);
            GameTexts.SetVariable("TEXT", GameTexts.FindText("str_character", (string)null).ToString());
            GameTexts.SetVariable("HOTKEY", this._shortcuts.CharacterHotkey);
            this.SkillsHint = new HintViewModel(GameTexts.FindText("str_hotkey_with_hint", (string)null).ToString(), (string)null);
            GameTexts.SetVariable("TEXT", GameTexts.FindText("str_escape_menu", (string)null).ToString());
            GameTexts.SetVariable("HOTKEY", this._shortcuts.EscapeMenuHotkey);
            this.EscapeMenuHint = new HintViewModel(GameTexts.FindText("str_hotkey_with_hint", (string)null).ToString(), (string)null);
            GameTexts.SetVariable("TEXT", GameTexts.FindText("str_quest", (string)null).ToString());
            GameTexts.SetVariable("HOTKEY", this._shortcuts.QuestHotkey);
            this.QuestsHint = new HintViewModel(GameTexts.FindText("str_hotkey_with_hint", (string)null).ToString(), (string)null);
            GameTexts.SetVariable("TEXT", GameTexts.FindText("str_inventory", (string)null).ToString());
            GameTexts.SetVariable("HOTKEY", this._shortcuts.InventoryHotkey);
            this.InventoryHint = new HintViewModel(GameTexts.FindText("str_hotkey_with_hint", (string)null).ToString(), (string)null);
            GameTexts.SetVariable("TEXT", GameTexts.FindText("str_party", (string)null).ToString());
            GameTexts.SetVariable("HOTKEY", this._shortcuts.PartyHotkey);
            this.PartyHint = new HintViewModel(GameTexts.FindText("str_hotkey_with_hint", (string)null).ToString(), (string)null);
            this.CampHint = new HintViewModel(GameTexts.FindText("str_camp", (string)null).ToString(), (string)null);
            this.FinanceHint = new HintViewModel(GameTexts.FindText("str_finance", (string)null).ToString(), (string)null);
            this.CenterCameraHint = new HintViewModel(GameTexts.FindText("str_return_to_hero", (string)null).ToString(), (string)null);
            this.KingdomHint.HintText = Hero.MainHero.MapFaction.IsKingdomFaction ? GameTexts.FindText("str_kingdom", (string)null).ToString() : GameTexts.FindText("str_need_to_be_a_part_of_kingdom", (string)null).ToString();
            this.AlertText = GameTexts.FindText("str_map_bar_alert", (string)null).ToString();
            this.Refresh();
        }

        public void Refresh()
        {
            this.RefreshAlertValues();
            PlayerUpdateTracker.Current.UpdatePartyNotification();
        }

        public void Tick()
        {
            this.RefreshPermissionValues();
            this.RefreshStates();
        }

        private void RefreshPermissionValues()
        {
            NavigationPermissionItem kingdomPermission = this._navigationHandler.KingdomPermission;
            this.IsKingdomEnabled = kingdomPermission.IsAuthorized;
            if (this.IsKingdomEnabled)
            {
                GameTexts.SetVariable("TEXT", GameTexts.FindText("str_kingdom", (string)null).ToString());
                GameTexts.SetVariable("HOTKEY", this._shortcuts.KingdomHotkey);
                this.KingdomHint.HintText = GameTexts.FindText("str_hotkey_with_hint", (string)null).ToString();
            }
            else
                this.KingdomHint.HintText = kingdomPermission.ReasonString;
            NavigationPermissionItem clanPermission = this._navigationHandler.ClanPermission;
            this.IsClanEnabled = clanPermission.IsAuthorized;
            if (this.IsClanEnabled)
            {
                GameTexts.SetVariable("TEXT", GameTexts.FindText("str_clan", (string)null).ToString());
                GameTexts.SetVariable("HOTKEY", this._shortcuts.ClanHotkey);
                this.ClanHint.HintText = GameTexts.FindText("str_hotkey_with_hint", (string)null).ToString();
            }
            else
                this.ClanHint.HintText = clanPermission.ReasonString;
        }

        private void RefreshAlertValues()
        {
            this.QuestsAlert = PlayerUpdateTracker.Current.IsQuestNotificationActive;
            this.SkillAlert = PlayerUpdateTracker.Current.IsCharacterNotificationActive;
            this.PartyAlert = PlayerUpdateTracker.Current.IsPartyNotificationActive;
            this.KingdomAlert = PlayerUpdateTracker.Current.IsKingdomNotificationActive;
            this.ClanAlert = PlayerUpdateTracker.Current.IsClanNotificationActive;
        }

        private void RefreshStates()
        {
            this.IsPartyEnabled = this._navigationHandler.PartyEnabled;
            this.IsInventoryEnabled = this._navigationHandler.InventoryEnabled;
            this.IsCharacterDeveloperEnabled = this._navigationHandler.CharacterDeveloperEnabled;
            this.IsQuestsEnabled = this._navigationHandler.QuestsEnabled;
            this.IsEscapeMenuEnabled = this._navigationHandler.EscapeMenuEnabled;
            this.IsKingdomActive = this._navigationHandler.KingdomActive;
            this.IsPartyActive = this._navigationHandler.PartyActive;
            this.IsInventoryActive = this._navigationHandler.InventoryActive;
            this.IsClanActive = this._navigationHandler.ClanActive;
            this.IsCharacterDeveloperActive = this._navigationHandler.CharacterDeveloperActive;
            this.IsQuestsActive = this._navigationHandler.QuestsActive;
            this.IsEscapeMenuActive = this._navigationHandler.EscapeMenuActive;
        }

        private void ExecuteOpenQuests()
        {
            this._navigationHandler.OpenQuests();
        }

        private void ExecuteOpenInventory()
        {
            this._navigationHandler.OpenInventory();
        }

        private void ExecuteOpenParty()
        {
            this._navigationHandler.OpenParty();
        }

        private void ExecuteOpenCharacterDeveloper()
        {
            this._navigationHandler.OpenCharacterDeveloper();
        }

        private void ExecuteOpenKingdom()
        {
            this._navigationHandler.OpenKingdom(KingdomState.KingdomCategories.None);
        }

        private void ExecuteOpenClan()
        {
            this._navigationHandler.OpenClan();
        }

        private void ExecuteOpenEscapeMenu()
        {
            this._navigationHandler.OpenEscapeMenu();
        }

        private void ExecuteOpenMainHeroEncyclopedia()
        {
            Campaign.Current.EncyclopediaManager.GoToLink(Hero.MainHero.EncyclopediaLink);
        }

        private void ExecuteOpenMainHeroClanEncyclopedia()
        {
            Campaign.Current.EncyclopediaManager.GoToLink(Hero.MainHero.Clan.EncyclopediaLink);
        }

        private void ExecuteOpenMainHeroKingdomEncyclopedia()
        {
            if (Hero.MainHero.MapFaction == null)
                return;
            Campaign.Current.EncyclopediaManager.GoToLink(Hero.MainHero.MapFaction.EncyclopediaLink);
        }

        [DataSourceProperty]
        public string AlertText
        {
            get
            {
                return this._alertText;
            }
            set
            {
                if (!(value != this._alertText))
                    return;
                this._alertText = value;
                this.OnPropertyChanged(nameof(AlertText));
            }
        }

        [DataSourceProperty]
        public bool SkillAlert
        {
            get
            {
                return this._skillAlert;
            }
            set
            {
                if (value == this._skillAlert)
                    return;
                this._skillAlert = value;
                this.OnPropertyChanged(nameof(SkillAlert));
            }
        }

        [DataSourceProperty]
        public bool QuestsAlert
        {
            get
            {
                return this._questsAlert;
            }
            set
            {
                if (value == this._questsAlert)
                    return;
                this._questsAlert = value;
                this.OnPropertyChanged(nameof(QuestsAlert));
            }
        }

        [DataSourceProperty]
        public bool PartyAlert
        {
            get
            {
                return this._partyAlert;
            }
            set
            {
                if (value == this._partyAlert)
                    return;
                this._partyAlert = value;
                this.OnPropertyChanged(nameof(PartyAlert));
            }
        }

        [DataSourceProperty]
        public bool KingdomAlert
        {
            get
            {
                return this._kingdomAlert;
            }
            set
            {
                if (value == this._kingdomAlert)
                    return;
                this._kingdomAlert = value;
                this.OnPropertyChanged(nameof(KingdomAlert));
            }
        }

        [DataSourceProperty]
        public bool ClanAlert
        {
            get
            {
                return this._clanAlert;
            }
            set
            {
                if (value == this._clanAlert)
                    return;
                this._clanAlert = value;
                this.OnPropertyChanged(nameof(ClanAlert));
            }
        }

        [DataSourceProperty]
        public bool InventoryAlert
        {
            get
            {
                return this._inventoryAlert;
            }
            set
            {
                if (value == this._inventoryAlert)
                    return;
                this._inventoryAlert = value;
                this.OnPropertyChanged(nameof(InventoryAlert));
            }
        }

        [DataSourceProperty]
        public bool IsEscapeMenuEnabled
        {
            get
            {
                return this._isEscapeMenuEnabled;
            }
            set
            {
                if (value == this._isEscapeMenuEnabled)
                    return;
                this._isEscapeMenuEnabled = value;
                this.OnPropertyChanged(nameof(IsEscapeMenuEnabled));
            }
        }

        [DataSourceProperty]
        public bool IsKingdomEnabled
        {
            get
            {
                return this._isKingdomEnabled;
            }
            set
            {
                if (value == this._isKingdomEnabled)
                    return;
                this._isKingdomEnabled = value;
                this.OnPropertyChanged(nameof(IsKingdomEnabled));
            }
        }

        [DataSourceProperty]
        public bool IsPartyEnabled
        {
            get
            {
                return this._isPartyEnabled;
            }
            set
            {
                if (value == this._isPartyEnabled)
                    return;
                this._isPartyEnabled = value;
                this.OnPropertyChanged(nameof(IsPartyEnabled));
            }
        }

        [DataSourceProperty]
        public bool IsInventoryEnabled
        {
            get
            {
                return this._isInventoryEnabled;
            }
            set
            {
                if (value == this._isInventoryEnabled)
                    return;
                this._isInventoryEnabled = value;
                this.OnPropertyChanged(nameof(IsInventoryEnabled));
            }
        }

        [DataSourceProperty]
        public bool IsQuestsEnabled
        {
            get
            {
                return this._isQuestsEnabled;
            }
            set
            {
                if (value == this._isQuestsEnabled)
                    return;
                this._isQuestsEnabled = value;
                this.OnPropertyChanged(nameof(IsQuestsEnabled));
            }
        }

        [DataSourceProperty]
        public bool IsCharacterDeveloperEnabled
        {
            get
            {
                return this._isCharacterDeveloperEnabled;
            }
            set
            {
                if (value == this._isCharacterDeveloperEnabled)
                    return;
                this._isCharacterDeveloperEnabled = value;
                this.OnPropertyChanged(nameof(IsCharacterDeveloperEnabled));
            }
        }

        [DataSourceProperty]
        public bool IsClanEnabled
        {
            get
            {
                return this._isClanEnabled;
            }
            set
            {
                if (value == this._isClanEnabled)
                    return;
                this._isClanEnabled = value;
                this.OnPropertyChanged(nameof(IsClanEnabled));
            }
        }

        [DataSourceProperty]
        public bool IsKingdomActive
        {
            get
            {
                return this._isKingdomActive;
            }
            set
            {
                if (value == this._isKingdomActive)
                    return;
                this._isKingdomActive = value;
                this.OnPropertyChanged(nameof(IsKingdomActive));
            }
        }

        [DataSourceProperty]
        public bool IsPartyActive
        {
            get
            {
                return this._isPartyActive;
            }
            set
            {
                if (value == this._isPartyActive)
                    return;
                this._isPartyActive = value;
                this.OnPropertyChanged(nameof(IsPartyActive));
            }
        }

        [DataSourceProperty]
        public bool IsInventoryActive
        {
            get
            {
                return this._isInventoryActive;
            }
            set
            {
                if (value == this._isInventoryActive)
                    return;
                this._isInventoryActive = value;
                this.OnPropertyChanged(nameof(IsInventoryActive));
            }
        }

        [DataSourceProperty]
        public bool IsQuestsActive
        {
            get
            {
                return this._isQuestsActive;
            }
            set
            {
                if (value == this._isQuestsActive)
                    return;
                this._isQuestsActive = value;
                this.OnPropertyChanged(nameof(IsQuestsActive));
            }
        }

        [DataSourceProperty]
        public bool IsCharacterDeveloperActive
        {
            get
            {
                return this._isCharacterDeveloperActive;
            }
            set
            {
                if (value == this._isCharacterDeveloperActive)
                    return;
                this._isCharacterDeveloperActive = value;
                this.OnPropertyChanged(nameof(IsCharacterDeveloperActive));
            }
        }

        [DataSourceProperty]
        public bool IsClanActive
        {
            get
            {
                return this._isClanActive;
            }
            set
            {
                if (value == this._isClanActive)
                    return;
                this._isClanActive = value;
                this.OnPropertyChanged(nameof(IsClanActive));
            }
        }

        [DataSourceProperty]
        public bool IsEscapeMenuActive
        {
            get
            {
                return this._isEscapeMenuActive;
            }
            set
            {
                if (value == this._isEscapeMenuActive)
                    return;
                this._isEscapeMenuActive = value;
                this.OnPropertyChanged(nameof(IsEscapeMenuActive));
            }
        }

        [DataSourceProperty]
        public HintViewModel FinanceHint
        {
            get
            {
                return this._financeHint;
            }
            set
            {
                if (value == this._financeHint)
                    return;
                this._financeHint = value;
                this.OnPropertyChanged(nameof(FinanceHint));
            }
        }

        [DataSourceProperty]
        public HintViewModel EncyclopediaHint
        {
            get
            {
                return this._encyclopediaHint;
            }
            set
            {
                if (value == this._encyclopediaHint)
                    return;
                this._encyclopediaHint = value;
                this.OnPropertyChanged(nameof(EncyclopediaHint));
            }
        }

        [DataSourceProperty]
        public HintViewModel EscapeMenuHint
        {
            get
            {
                return this._escapeMenuHint;
            }
            set
            {
                if (value == this._escapeMenuHint)
                    return;
                this._escapeMenuHint = value;
                this.OnPropertyChanged(nameof(EscapeMenuHint));
            }
        }

        [DataSourceProperty]
        public HintViewModel SkillsHint
        {
            get
            {
                return this._skillsHint;
            }
            set
            {
                if (value == this._skillsHint)
                    return;
                this._skillsHint = value;
                this.OnPropertyChanged(nameof(SkillsHint));
            }
        }

        [DataSourceProperty]
        public HintViewModel QuestsHint
        {
            get
            {
                return this._questsHint;
            }
            set
            {
                if (value == this._questsHint)
                    return;
                this._questsHint = value;
                this.OnPropertyChanged(nameof(QuestsHint));
            }
        }

        [DataSourceProperty]
        public HintViewModel InventoryHint
        {
            get
            {
                return this._inventoryHint;
            }
            set
            {
                if (value == this._inventoryHint)
                    return;
                this._inventoryHint = value;
                this.OnPropertyChanged(nameof(InventoryHint));
            }
        }

        [DataSourceProperty]
        public HintViewModel PartyHint
        {
            get
            {
                return this._partyHint;
            }
            set
            {
                if (value == this._partyHint)
                    return;
                this._partyHint = value;
                this.OnPropertyChanged(nameof(PartyHint));
            }
        }

        [DataSourceProperty]
        public HintViewModel KingdomHint
        {
            get
            {
                return this._kingdomHint;
            }
            set
            {
                if (value == this._kingdomHint)
                    return;
                this._kingdomHint = value;
                this.OnPropertyChanged(nameof(KingdomHint));
            }
        }

        [DataSourceProperty]
        public HintViewModel ClanHint
        {
            get
            {
                return this._clanHint;
            }
            set
            {
                if (value == this._clanHint)
                    return;
                this._clanHint = value;
                this.OnPropertyChanged(nameof(ClanHint));
            }
        }

        [DataSourceProperty]
        public HintViewModel FinancesHint
        {
            get
            {
                return new HintViewModel("Work in Progress","finances_hint");
            }
        }

        [DataSourceProperty]
        public HintViewModel CenterCameraHint
        {
            get
            {
                return this._centerCameraHint;
            }
            set
            {
                if (value == this._centerCameraHint)
                    return;
                this._centerCameraHint = value;
                this.OnPropertyChanged(nameof(CenterCameraHint));
            }
        }

        [DataSourceProperty]
        public HintViewModel CampHint
        {
            get
            {
                return this._campHint;
            }
            set
            {
                if (value == this._campHint)
                    return;
                this._campHint = value;
                this.OnPropertyChanged(nameof(CampHint));
            }
        }
    }
}
