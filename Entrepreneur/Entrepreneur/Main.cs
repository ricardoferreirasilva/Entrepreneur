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
using Entrepreneur.Behaviours;
using HarmonyLib;

namespace Entrepreneur
{
    public class Main : MBSubModuleBase
    {

        protected override void OnSubModuleLoad()
        {
            Harmony.DEBUG = true;
            Harmony harmony = new Harmony("com.goog.bannerlordmods.Entrepreneur");
            harmony.PatchAll();
            base.OnSubModuleLoad();

        }
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            Campaign campaign = game.GameType as Campaign;
            if (campaign == null) return;
            CampaignGameStarter gameInitializer = (CampaignGameStarter)gameStarterObject;
            AddBehaviors(gameInitializer);
        }

        private void AddBehaviors(CampaignGameStarter gameInitializer)
        {
            gameInitializer.AddBehavior(EntrepreneurCampaignBehaviour.Instance);
        }
    }
}
