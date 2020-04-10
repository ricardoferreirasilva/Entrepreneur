using SandBox.GauntletUI.Map;
using SandBox.View.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Engine.Screens;
using TaleWorlds.MountAndBlade.View.Missions;

namespace Entrepreneur.Screens
{
    [OverrideView(typeof(MapBar))]
    public class GauntletMapBar : MapView
    {
        private EntrepreneurMapBarGlobalLayer _gauntlerMapBarGlobalLayer;

        protected override void CreateLayout()
        {
            base.CreateLayout();
            this._gauntlerMapBarGlobalLayer = new EntrepreneurMapBarGlobalLayer();
            this._gauntlerMapBarGlobalLayer.Initialize(this.MapScreen);
            ScreenManager.AddGlobalLayer((GlobalLayer)this._gauntlerMapBarGlobalLayer, true);
        }

        protected override void OnFinalize()
        {
            this._gauntlerMapBarGlobalLayer.OnFinalize();
            ScreenManager.RemoveGlobalLayer((GlobalLayer)this._gauntlerMapBarGlobalLayer);
            base.OnFinalize();
        }

        protected override void OnResume()
        {
            base.OnResume();
            this._gauntlerMapBarGlobalLayer.Refresh();
        }

        protected override bool IsEscaped()
        {
            return this._gauntlerMapBarGlobalLayer.IsEscaped();
        }
    }
}
