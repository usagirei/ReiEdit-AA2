// --------------------------------------------------
// ReiEditAA2 - CardGeneratorModifier.cs
// --------------------------------------------------

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using ReiEditAA2.ViewModels;
using ReiFX;

namespace ReiEditAA2.Code.Modifiers
{
    internal class CardGeneratorModifier : CharModifierBase
    {
        public override string Name
        {
            get { return "Generate Card/ThumbImage"; }
        }

        public override void Modify(CharacterViewModel model)
        {
            Layer thumbLayer = CardGenerator.GenerateThumbnail(model.Character);
            Bitmap thumbBitmap = CardGenerator.Resample(thumbLayer, 74, 90, InterpolationMode.HighQualityBicubic);

            Layer cardLayer = CardGenerator.GenerateCard(model.Character);

            Bitmap cardBitmap = CardGenerator.Resample(cardLayer, 200, 300, InterpolationMode.Bilinear);

            MemoryStream streamThumb = new MemoryStream();

            thumbBitmap.Save(streamThumb, ImageFormat.Png);
            model.Character.SetThumbnail(streamThumb.ToArray());

            MemoryStream streamCard = new MemoryStream();

            cardBitmap.Save(streamCard, ImageFormat.Png);
            model.Character.SetCard(streamCard.ToArray());
        }
    }
}