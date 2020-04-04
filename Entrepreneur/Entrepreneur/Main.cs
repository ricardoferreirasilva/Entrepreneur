using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Overlay;
using Entrepreneur.Behaviours;

namespace Entrepreneur
{
    public class Main : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
        }
        public override void OnGameLoaded(Game game, object initializerObject)
        {
            Campaign campaign = game.GameType as Campaign;
            if (campaign != null)
            {
                CampaignGameStarter gameInitializer = (CampaignGameStarter)initializerObject;
                AddBehaviors(gameInitializer);
            }
        }

        private void AddBehaviors(CampaignGameStarter gameInitializer)
        {
            gameInitializer.AddBehavior(new EntrepreneurCampaignBehaviour());
        }
    }
}
