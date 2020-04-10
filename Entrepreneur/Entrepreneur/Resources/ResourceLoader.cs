using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.TwoDimension;

namespace Entrepreneur.Resources
{
    static class ResourceLoader
    {
        public static void Load()
        {
            var rd = UIResourceManager.UIResourceDepot;
            var rc = UIResourceManager.ResourceContext;
            var sd = UIResourceManager.SpriteData;

            var spriteData = new SpriteData("EntrepreneurSpriteData");
            spriteData.Load(rd);
            var texture = new TaleWorlds.TwoDimension.Texture((TaleWorlds.TwoDimension.ITexture)
                new EngineTexture(
                    TaleWorlds.Engine.Texture.CreateTextureFromPath(
                         @"../../Modules/Entrepreneur/GUI/SpriteSheets/", "entrepreneur-ui-1.png")
                    )
                );

            sd.SpriteCategories.Add("entrepreneur_icons", spriteData.SpriteCategories["entrepreneur_icons"]);

            
            sd.SpritePartNames.Add("FinancesIcon", spriteData.SpritePartNames["FinancesIcon"]);
            sd.SpriteNames.Add("FinancesIcon", new SpriteGeneric("FinancesIcon", spriteData.SpritePartNames["FinancesIcon"]));

            sd.SpritePartNames.Add("MapbarLeftFrame", spriteData.SpritePartNames["MapbarLeftFrame"]);
            sd.SpriteNames.Add("MapbarLeftFrame", new SpriteGeneric("MapbarLeftFrame", spriteData.SpritePartNames["MapbarLeftFrame"]));

            var bettertimeicons = sd.SpriteCategories["entrepreneur_icons"];
            bettertimeicons.SpriteSheets.Add(texture);
            bettertimeicons.Load((ITwoDimensionResourceContext)rc, rd);

            UIResourceManager.BrushFactory.Initialize();
        }
    }
}
