// --------------------------------------------------
// ReiEditAA2 - CardGenerator.cs
// --------------------------------------------------

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using AA2Lib;
using AA2Lib.Code;
using ReiFX;
using ReiFX.Effects;
using Color = ReiFX.Color;

namespace ReiEditAA2.Code
{
    internal static partial class CardGenerator
    {
        private static readonly BackgroundEffect BackgroundEffect = new BackgroundEffect
        {
            RainbowAngle = 45,
            RainbowPhase = 30,
            RainbowRadial = true,
        };

        private static readonly DropShadowEffect DropShadowEffect = new DropShadowEffect
        {
            ShadowColor = new Color(0, 0, 0, 200),
            ShadowSize = 4,
        };

        private static readonly Color[] LipColors =
        {
            // Card Background Colors
            Color.FromRgb(255, 100, 000), //ORANGE
            Color.FromRgb(100, 000, 000), //DARK RED
            Color.FromRgb(255, 000, 000), //RED
            Color.FromRgb(255, 000, 100), //PINK A
            Color.FromRgb(255, 000, 200), //PINK B
            Color.FromRgb(180, 000, 150), //PURPLE A
            Color.FromRgb(200, 000, 255), //PURPLE B
            Color.FromRgb(255, 200, 250) //LIGHT PINK
        };

        private static readonly MirrorEffect MirrorEffect = new MirrorEffect
        {
            MirrorX = true,
            MirrorY = false
        };

        private static readonly BlendEffect MultiplyEffect = new BlendEffect
        {
            Mode = BlendEffect.BlendMode.Multiply,
            Source = BlendEffect.OverlaySource.Color
        };

        private static readonly OpacityEffect OpacityEffect = new OpacityEffect();

        private static readonly Color[] SexualityColors =
        {
            // Card Background Colors
            Color.FromRgb(255, 000, 000), //RED
            Color.FromRgb(255, 127, 000), //ORANGE
            Color.FromRgb(127, 000, 255), //PURPLE
            Color.FromRgb(000, 255, 000), //GREEN
            Color.FromRgb(000, 255, 255) //CYAN
        };

        private static readonly StrokeEffect StrokeEffect = new StrokeEffect();

        private static string PngRootDir
        {
            get { return Path.Combine(Core.StartupPath, "PNG"); }
        }

        public static Layer GenerateCard(CharacterFile characterFile)
        {
            byte gender = characterFile.GetAttribute<byte>(KEY_GENDER);
            bool isMale = gender == 0;

            byte height = characterFile.GetAttribute<byte>(KEY_HEIGHT);
            string figureKey = isMale
                ? KEY_FIGURE_M
                : KEY_FIGURE_F;

            byte figure = characterFile.GetAttribute<byte>(figureKey);

            byte sexuality = characterFile.GetAttribute<byte>(KEY_ORIENTATION);
            bool isRainbow = characterFile.GetAttribute<bool>(KEY_RAINBOW);

            byte breastSize = characterFile.GetAttribute<byte>(KEY_BREASTS);

            Layer layerBackground = LoadLayer(Path.Combine(PngRootDir, "BACKGROUND"));
            Layer layerFrame = LoadLayer(Path.Combine(PngRootDir, "FRAME"));

            MultiplyEffect.OverlayColor = isRainbow
                ? new Color(200, 200, 200)
                : (SexualityColors[sexuality] / 1.7f) + 50;
            MultiplyEffect.ApplyEffect(layerFrame);

            StrokeEffect.StrokeWidth = 3;
            StrokeEffect.SourceOver = true;
            StrokeEffect.StrokeColor = new Color(255, 255, 255);
            StrokeEffect.ApplyEffect(layerFrame);

            Layer layerPortrait = GeneratePortraitLayer(characterFile);

            Layer layerName = GenerateTextLayer(characterFile);
            StrokeEffect.StrokeWidth = 4;
            StrokeEffect.SourceOver = true;
            StrokeEffect.StrokeColor = new Color(50, 50, 50);
            StrokeEffect.ApplyEffect(layerName);

            Layer layerPortraitName = CompositeUnsorted(layerPortrait, layerName);

            Layer layerCutout = CompositeUnsorted(layerPortraitName, layerFrame); //(Layer) layerPortraitName.Clone();

            StrokeEffect.StrokeWidth = 8;
            StrokeEffect.SourceOver = false;
            StrokeEffect.StrokeColor = new Color(0, 0, 0);
            StrokeEffect.ApplyEffect(layerCutout);

            Rectangle shadowArea = layerPortraitName.GetArea();
            shadowArea.Inflate(DropShadowEffect.ShadowSize, DropShadowEffect.ShadowSize);
            DropShadowEffect.ApplyEffect(layerPortraitName, shadowArea);

            DropShadowEffect.ApplyEffect(layerFrame);

            Layer layerBackgroundCutout = CompositeUnsorted(layerBackground, layerCutout);
            BackgroundEffect.RainbowCenter = new Point(CARD_WIDTH / 2, CARD_HEIGHT / 2);
            BackgroundEffect.IsRainbow = isRainbow;
            BackgroundEffect.Color = SexualityColors[sexuality];
            BackgroundEffect.ApplyEffect(layerBackgroundCutout);

            Layer genderTag = LoadLayer(Path.Combine(PngRootDir, "TAGS", "GENDER", String.Format("{0:D3}", gender)));

            byte bodyId = (byte) (isMale
                ? figure
                : (height << 2) | figure);

            string bodyDir = isMale
                ? "BODY_M"
                : "BODY_F";

            Layer bodyTag = LoadLayer(Path.Combine(PngRootDir, "TAGS", bodyDir, String.Format("{0:D3}", bodyId)));

            byte breastId = (byte) (isMale
                ? 0
                : Math.Max(1, breastSize / 17));
            Layer breastTag = LoadLayer(Path.Combine(PngRootDir, "TAGS", "BREASTS", String.Format("{0:D3}", breastId)));

            Layer layerTags = Composite(genderTag, bodyTag, breastTag);

            //StrokeEffect.StrokeWidth = 4;
            //StrokeEffect.AlphaBlend = true;
            //StrokeEffect.StrokeColor = new Color(50, 50, 50);

            //StrokeEffect.ApplyEffect(layerTags);
            //DropShadowEffect.ApplyEffect(layerTags);

            Layer final = CompositeUnsorted(layerBackgroundCutout, layerPortraitName, layerFrame, layerTags);

            return final;
        }

        public static Layer GenerateThumbnail(CharacterFile characterFile)
        {
            byte sexuality = characterFile.GetAttribute<byte>(KEY_ORIENTATION);
            bool isRainbow = characterFile.GetAttribute<bool>(KEY_RAINBOW);

            Layer portrait = GeneratePortraitLayer(characterFile);

            Rectangle thumbRect = new Rectangle(THUMB_LEFT, THUMB_TOP, THUMB_WIDTH, THUMB_HEIGHT);

            DropShadowEffect.Region = thumbRect;
            DropShadowEffect.ApplyEffect(portrait);
            DropShadowEffect.Region = Rectangle.Empty;

            Layer thumb = Layer.Create(THUMB_WIDTH, THUMB_HEIGHT);

            for (int i = 0; i < thumb.BitmapData.Length; i += 4)
            {
                //int px = (i / 4) % THUMB_WIDTH;
                int py = (i / 4) / THUMB_WIDTH;

                byte y = (byte) (py * 150 / THUMB_HEIGHT);
                thumb.BitmapData[i + 0] = y;
                thumb.BitmapData[i + 1] = y;
                thumb.BitmapData[i + 2] = y;
                thumb.BitmapData[i + 3] = 255;
            }

            BackgroundEffect.RainbowCenter = new Point(THUMB_WIDTH / 2, THUMB_HEIGHT / 2);
            BackgroundEffect.IsRainbow = isRainbow;
            BackgroundEffect.Color = SexualityColors[sexuality];
            BackgroundEffect.ApplyEffect(thumb);

            BlendEffect blend = new BlendEffect
            {
                OverlayLayer = portrait,
                Offset = new Point(-THUMB_LEFT, -THUMB_TOP),
            };
            blend.ApplyEffect(thumb);

            return thumb;
        }

        public static Bitmap Resample(Layer layer, int width, int height, InterpolationMode mode)
        {
            Bitmap bmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bmp);
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.InterpolationMode = mode;
            g.DrawImage((Image) layer, 0, 0, width, height);
            g.Flush();

            return bmp;
        }

        private static Layer Composite(params Layer[] layers)
        {
            if (layers.Length == 0)
                return null;
            if (layers.Length == 1)
                return (Layer) layers[0].Clone();

            var ordered = layers.Where(layer => layer != null)
                .OrderBy(layer => layer.ZIndex)
                .ToArray();

            return CompositeUnsorted(ordered);
        }

        private static Layer CompositeUnsorted(params Layer[] layers)
        {
            if (layers.Length == 0)
                return null;
            if (layers.Length == 1)
                return (Layer) layers[0].Clone();

            Layer @base = layers[0];
            bool valid = layers.Aggregate(true, (b, layer) => b && layer.Width == @base.Width && layer.Height == @base.Height);
            if (!valid)
                throw new Exception("Layer Sizes Mismatch");
            Layer tgtLyr = Layer.Create(@base.Width, @base.Height);

            BlendEffect effect = new BlendEffect();
            foreach (Layer t in layers)
            {
                Rectangle area = t.GetArea();
                area.Inflate(1, 1);

                effect.OverlayLayer = t;
                effect.Region = area;
                effect.ApplyEffect(tgtLyr);
            }
            return tgtLyr;
        }

        private static Color FixHairColor(Color input)
        {
            float hairHue, hairSat, hairBright;
            Color.ToHsl(input, out hairHue, out hairSat, out hairBright);
            return Color.FromHsl(hairHue, hairSat * 1, .2 + (hairBright) * .8);
        }

        private static Layer GeneratePortraitLayer(CharacterFile characterFile)
        {
            Color zSkinBase = characterFile.GetAttribute<Color>(KEY_SKIN_COLOR);
            byte zTanOpacity = characterFile.GetAttribute<byte>(KEY_TAN_OPACITY);
            Color zSkinTan = Color.FromRgb(0x4D, 0xD1, 0x13, zTanOpacity); //0x4D1D13

            //Color colorSkin = Color.Combine(zSkinBase, zSkinTan);
            Color colorSkin = zSkinBase & zSkinTan;
            Color colorLEye = characterFile.GetAttribute<Color>(KEY_EYE_LEFT_COLOR);
            Color colorREye = characterFile.GetAttribute<Color>(KEY_EYE_RIGHT_COLOR);
            Color colorHair = FixHairColor(characterFile.GetAttribute<Color>(KEY_HAIR_COLOR));
            Color colorGlass = characterFile.GetAttribute<Color>(KEY_GLASSES_COLOR);
            Color colorEyebrow = characterFile.GetAttribute<Color>(KEY_EYEBROW_COLOR);

            bool bIsMale = characterFile.GetAttribute<byte>(KEY_GENDER) == 0;

            bool flipBackHair = characterFile.GetAttribute<bool>(KEY_HAIR_BACK_FLIP);
            bool flipSideHair = characterFile.GetAttribute<bool>(KEY_HAIR_SIDE_FLIP);
            bool flipFrontHair = characterFile.GetAttribute<bool>(KEY_HAIR_FRONT_FLIP);
            bool flipExtHair = characterFile.GetAttribute<bool>(KEY_HAIR_EXT_FLIP);

            bool moleCheekLeft = characterFile.GetAttribute<bool>(KEY_MOLE_CHEEK_LEFT);
            bool moleCheekRight = characterFile.GetAttribute<bool>(KEY_MOLE_CHEEK_RIGHT);
            bool moleChinLeft = characterFile.GetAttribute<bool>(KEY_MOLE_CHIN_LEFT);
            bool moleChinRight = characterFile.GetAttribute<bool>(KEY_MOLE_CHIN_RIGHT);

            byte idBackHair = characterFile.GetAttribute<byte>(KEY_HAIR_BACK);
            byte idSideHair = characterFile.GetAttribute<byte>(KEY_HAIR_SIDE);
            byte idFrontHair = characterFile.GetAttribute<byte>(KEY_HAIR_FRONT);
            byte idExtHair = characterFile.GetAttribute<byte>(KEY_HAIR_EXT);

            byte idEyebrow = characterFile.GetAttribute<byte>(KEY_EYEBROW_SHAPE);

            byte idLipstick = characterFile.GetAttribute<byte>(KEY_LIPSTICK_COLOR);
            byte lipOpacity = bIsMale
                ? (byte) 0
                : characterFile.GetAttribute<byte>(KEY_LIPSTICK_OPACITY);

            byte idGlasses = characterFile.GetAttribute<byte>(KEY_GLASSES_TYPE);
            string zFaceKey = bIsMale
                ? KEY_FACETYPE_M
                : KEY_FACETYPE_F;

            byte idFace = characterFile.GetAttribute<byte>(zFaceKey);

            var layersSkin = LoadLayers(PngRootDir, "SKIN");
            MultiplyEffect.OverlayColor = colorSkin;
            layersSkin.Where(layer => !layer.XFlag)
                .ForEach(layer => MultiplyEffect.ApplyEffect(layer, layer.GetArea()));

            var layersHair = new List<Layer>();
            layersHair.AddRange(LoadAndMirror(Path.Combine(PngRootDir, "HAIR_BACK"), idBackHair, flipBackHair));
            layersHair.AddRange(LoadAndMirror(Path.Combine(PngRootDir, "HAIR_SIDE"), idSideHair, flipSideHair));
            layersHair.AddRange(LoadAndMirror(Path.Combine(PngRootDir, "HAIR_FRONT"), idFrontHair, flipFrontHair));
            layersHair.AddRange(LoadAndMirror(Path.Combine(PngRootDir, "HAIR_EXT"), idExtHair, flipExtHair));
            MultiplyEffect.OverlayColor = colorHair;
            layersHair.Where(layer => !layer.XFlag)
                .ForEach(layer => MultiplyEffect.ApplyEffect(layer, layer.GetArea()));

            string zEyesDir = bIsMale
                ? "EYES_M"
                : "EYES_F";
            var layersFace = LoadLayers(Path.Combine(PngRootDir, zEyesDir), idFace);
            MultiplyEffect.OverlayColor = colorLEye;
            MultiplyEffect.Region = new Rectangle(CARD_WIDTH / 2, 0, CARD_WIDTH / 2, CARD_HEIGHT);
            layersFace.Where(layer => !layer.XFlag)
                .ForEach(layer => MultiplyEffect.ApplyEffect(layer, layer.GetArea()));

            MultiplyEffect.OverlayColor = colorREye;
            MultiplyEffect.Region = new Rectangle(0, 0, CARD_WIDTH / 2, CARD_HEIGHT);
            layersFace.Where(layer => !layer.XFlag)
                .ForEach(layer => MultiplyEffect.ApplyEffect(layer, layer.GetArea()));

            MultiplyEffect.Region = Rectangle.Empty;

            var layersGlass = LoadLayers(Path.Combine(PngRootDir, "GLASSES"), idGlasses);
            MultiplyEffect.OverlayColor = colorGlass;
            layersGlass.Where(layer => !layer.XFlag)
                .ForEach(layer => MultiplyEffect.ApplyEffect(layer, layer.GetArea()));

            var layersMole = new List<Layer>();
            if (moleCheekLeft)
                layersMole.AddRange(LoadLayers(Path.Combine(PngRootDir, "MOLES"), "CHEKL"));
            if (moleCheekRight)
                layersMole.AddRange(LoadLayers(Path.Combine(PngRootDir, "MOLES"), "CHEKR"));
            if (moleChinLeft)
                layersMole.AddRange(LoadLayers(Path.Combine(PngRootDir, "MOLES"), "CHINL"));
            if (moleChinRight)
                layersMole.AddRange(LoadLayers(Path.Combine(PngRootDir, "MOLES"), "CHINR"));

            var layersEyebrows = LoadLayers(Path.Combine(PngRootDir, "EYEBROWS"), idEyebrow);
            MultiplyEffect.OverlayColor = colorEyebrow;
            layersEyebrows.Where(layer => !layer.XFlag)
                .ForEach(layer => MultiplyEffect.ApplyEffect(layer, layer.GetArea()));

            var layersLips = LoadLayers(PngRootDir, "LIPS");
            MultiplyEffect.OverlayColor = LipColors[idLipstick];
            OpacityEffect.Opacity = lipOpacity / 255.0f;
            layersLips.Where(layer => !layer.XFlag)
                .ForEach
                (layer =>
                {
                    Rectangle subRegion = layer.GetArea();
                    MultiplyEffect.ApplyEffect(layer, subRegion);
                    OpacityEffect.ApplyEffect(layer, subRegion);
                });

            var allLayers = new List<Layer>();
            allLayers.AddRange(layersFace);
            allLayers.AddRange(layersGlass);
            allLayers.AddRange(layersHair);
            allLayers.AddRange(layersMole);
            allLayers.AddRange(layersSkin);
            allLayers.AddRange(layersEyebrows);
            allLayers.AddRange(layersLips);

            return Composite(allLayers.ToArray());
        }

        private static Layer GenerateTextLayer(CharacterFile characterFile)
        {
            string personality = characterFile.GetEnumValue(KEY_PERSONALITY);
            int indexOfPar = personality.IndexOf('(');
            if (indexOfPar != -1)
                personality = personality.Remove(indexOfPar - 1);
            string familyName = characterFile.GetAttribute<string>(KEY_FAMILY_NAME);
            string firstName = characterFile.GetAttribute<string>(KEY_FIRST_NAME);

            Bitmap bmp = new Bitmap(CARD_WIDTH, CARD_HEIGHT);
            Graphics g = Graphics.FromImage(bmp);

            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            StringFormat sf = new StringFormat
            {
                Trimming = StringTrimming.Character,
                FormatFlags = StringFormatFlags.NoWrap
            };

            Font nameFont = new Font("Tahoma", 50, FontStyle.Bold, GraphicsUnit.Pixel);
            Font persFont = new Font("Tahoma", 32, FontStyle.Bold, GraphicsUnit.Pixel);

            sf.Alignment = StringAlignment.Near;
            RectangleF textBounds = new RectangleF(30, 30, 340, 540);

            g.DrawString(personality, persFont, Brushes.White, textBounds, sf);
            g.TranslateTransform(0, 450);
            g.RotateTransform(-10);
            sf.Alignment = StringAlignment.Near;
            g.DrawString(familyName, nameFont, Brushes.White, textBounds, sf);
            g.RotateTransform(10);
            g.TranslateTransform(0, 50);
            g.RotateTransform(-10);
            sf.Alignment = StringAlignment.Far;
            g.DrawString(firstName, nameFont, Brushes.White, textBounds, sf);
            g.Flush();

            Layer textLayer = (Layer) bmp;
            g.Dispose();
            bmp.Dispose();
            return textLayer;
        }
    }
}