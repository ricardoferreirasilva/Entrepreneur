using Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment.Managers;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Entrepreneur.Models
{
    class EntrepreneurClanFinanceModel : DefaultClanFinanceModel
    {
        public override void CalculateClanIncome(
          Clan clan,
          ref ExplainedNumber goldChange,
          bool applyWithdrawals = false)
        {
            goldChange.Add(666,new TextObject("hardcoded 666"));
        }
    }
}
