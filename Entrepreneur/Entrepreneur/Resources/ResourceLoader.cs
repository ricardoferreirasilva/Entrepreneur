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

            var spriteData = new SpriteData("entrepreneur-ui-1");
            spriteData.Load(rd);
            var texture = new TaleWorlds.TwoDimension.Texture((TaleWorlds.TwoDimension.ITexture)
                new EngineTexture(
                    TaleWorlds.Engine.Texture.CreateTextureFromPath(
                         @"../../Modules/Entrepreneur/GUI/SpriteSheets/", "entrepreneur-ui-1.png")
                    )
                );

            sd.SpriteCategories.Add("entrepreneur-ui-1", spriteData.SpriteCategories["entrepreneur-ui-1"]);

            
            sd.SpritePartNames.Add("FinancesIcon", spriteData.SpritePartNames["FinancesIcon"]);
            sd.SpriteNames.Add("FinancesIcon", new SpriteGeneric("FinancesIcon", spriteData.SpritePartNames["FinancesIcon"]));

            sd.SpritePartNames.Add("MapbarLeftFrame", spriteData.SpritePartNames["MapbarLeftFrame"]);
            sd.SpriteNames.Add("MapbarLeftFrame", new SpriteGeneric("MapbarLeftFrame", spriteData.SpritePartNames["MapbarLeftFrame"]));

            sd.SpritePartNames.Add("Entrepreneur.EmptyField", spriteData.SpritePartNames["Entrepreneur.EmptyField"]);
            sd.SpriteNames.Add("Entrepreneur.EmptyField", new SpriteGeneric("Entrepreneur.EmptyField", spriteData.SpritePartNames["Entrepreneur.EmptyField"]));

            sd.SpritePartNames.Add("Entrepreneur.WorkingField", spriteData.SpritePartNames["Entrepreneur.WorkingField"]);
            sd.SpriteNames.Add("Entrepreneur.WorkingField", new SpriteGeneric("Entrepreneur.WorkingField", spriteData.SpritePartNames["Entrepreneur.WorkingField"]));

            sd.SpritePartNames.Add("Entrepreneur.VillagePropertyIcon", spriteData.SpritePartNames["Entrepreneur.VillagePropertyIcon"]);
            sd.SpriteNames.Add("Entrepreneur.VillagePropertyIcon", new SpriteGeneric("Entrepreneur.VillagePropertyIcon", spriteData.SpritePartNames["Entrepreneur.VillagePropertyIcon"]));
            var bettertimeicons = sd.SpriteCategories["entrepreneur-ui-1"];
            bettertimeicons.SpriteSheets.Add(texture);
            bettertimeicons.Load((ITwoDimensionResourceContext)rc, rd);

            UIResourceManager.BrushFactory.Initialize();
        }
    }
}
