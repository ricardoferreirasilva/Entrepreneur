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
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using Entrepreneur.Models;
using Entrepreneur.Resources;

namespace Entrepreneur
{
    public class Main : MBSubModuleBase
    {

        protected override void OnSubModuleLoad()
        {
			ResourceLoader.Load();
            Harmony.DEBUG = false;
            Harmony harmony = new Harmony("com.goog.bannerlordmods.Entrepreneur");
            harmony.PatchAll();
            base.OnSubModuleLoad();

        }
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            Campaign campaign = game.GameType as Campaign;
            if (campaign == null) return;
            CampaignGameStarter gameInitializer = (CampaignGameStarter)gameStarterObject;
			AddModels(gameStarterObject);
			AddBehaviors(gameInitializer);
        }
		protected virtual void AddModels(IGameStarter gameStarterObject)
		{
			//ReplaceModel<DefaultClanFinanceModel, EntrepreneurClanFinanceModel>(gameStarterObject);
		}
		private void AddBehaviors(CampaignGameStarter gameInitializer)
        {
            gameInitializer.AddBehavior(EntrepreneurCampaignBehaviour.Instance);
        }

		protected void ReplaceModel<TBaseType, TChildType>(IGameStarter gameStarterObject)
			where TBaseType : GameModel
			where TChildType : TBaseType
		{
			if (!(gameStarterObject.Models is IList<GameModel> models))
			{
				Trace.WriteLine("Models was not a list");
				return;
			}

			bool found = false;
			for (int index = 0; index < models.Count; ++index)
			{
				if (models[index] is TBaseType)
				{
					found = true;
					if (models[index] is TChildType)
					{
						Trace.WriteLine($"Child model {typeof(TChildType).Name} found, skipping.");
					}
					else
					{
						Trace.WriteLine($"Base model {typeof(TBaseType).Name} found. Replacing with child model {typeof(TChildType).Name}");
						models[index] = Activator.CreateInstance<TChildType>();
					}
				}
			}

			if (!found)
			{
				Trace.WriteLine($"Base model {typeof(TBaseType).Name} was not found. Adding child model {typeof(TChildType).Name}");
				gameStarterObject.AddModel(Activator.CreateInstance<TChildType>());
			}
		}
	}
}
