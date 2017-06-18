// --------------------------------------------------
// ReiEditAA2 - HairColorRandomizer.cs
// --------------------------------------------------

using System;
using System.Linq;
using AA2Lib;
using AA2Lib.Code;
using AA2Lib.Code.DataBlocks;
using ReiEditAA2.ViewModels;
using ReiFX;

namespace ReiEditAA2.Code.Modifiers
{
    internal class HairColorRandomizer : CharModifierBase
    {
        private static double _prevHue;

        public override string Name
        {
            get { return "Randomize Hair and Eyes Color"; }
        }

        public override void Modify(CharacterViewModel model)
        {
            var charBlocks = model.Character.CharAttributes.ToDictionary(block => block.Key, block => block.Value.DataBlock);

            var bodyKeys = charBlocks.Where(block => block.Key.StartsWith("BODY", StringComparison.Ordinal));

            var bodyColorKeys = bodyKeys.Where(pair => pair.Value is ColorDataBlock);

            //HS[V-L] for Hair
            double hue = (Core.Random.Next(0, 180) + _prevHue) % 360;
            double sat = Core.Random.NextDouble();
            double val = Core.Random.NextDouble();
            double lig = 0;
            _prevHue = hue;

            //Options
            bool matchHair = Math.Abs(0.5 - Core.Random.NextDouble()) > 0.35;
            bool heterocromia = Math.Abs(0.5 - Core.Random.NextDouble()) > 0.45;

            //Match Glasses to Hair, but darker
            Color hairColor = ColorHelper.ColorFromHSV(hue, sat, val);
            Color glassesColor = ColorHelper.ColorFromHSV(hue, .25, .25);

            //HSV for Left Eye
            hue = matchHair
                ? hue
                : (Core.Random.Next(0, 180) + _prevHue) % 360;
            sat = Core.Random.NextDouble();
            val = 0.5;
            _prevHue = hue;

            //Left = Right, if heterocromia, new Color for Right
            Color eyeColorL = ColorHelper.ColorFromHSV(hue, sat, val);
            Color eyeColorR = heterocromia
                ? ColorHelper.ColorFromHSV(hue + 180.0 + (2 * Core.Random.NextDouble() - 1) * 180, sat, val)
                : eyeColorL;

            foreach (var pair in bodyColorKeys)
            {
                switch (pair.Key)
                {
                    case "BODY_PUBIC_HAIR_COLOR":
                    case "BODY_EYEBROW_COLOR":
                    case "BODY_HAIR_COLOR":
                        model.Character.CharAttributes[pair.Key].Value = hairColor;
                        break;
                    case "BODY_GLASSES_COLOR":
                        model.Character.CharAttributes[pair.Key].Value = glassesColor;
                        break;
                    case "BODY_RIGHT_EYE_COLOR":
                        model.Character.CharAttributes[pair.Key].Value = eyeColorR;
                        break;
                    case "BODY_LEFT_EYE_COLOR":
                        model.Character.CharAttributes[pair.Key].Value = eyeColorL;
                        break;
                }
            }
        }
    }
}