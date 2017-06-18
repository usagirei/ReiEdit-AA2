// --------------------------------------------------
// ReiEditAA2 - BodyDeviationCharRandomizer.cs
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
    internal class BodyDeviationCharRandomizer : CharModifierBase
    {
        public override string Name
        {
            get { return "Randomize Body (Standard Deviation)"; }
        }

        public override void Modify(CharacterViewModel model)
        {
            var charBlocks = model.Character.CharAttributes.ToDictionary(block => block.Key, block => block.Value.DataBlock);

            CharacterCollection collection = model.ParentCollection;

            byte gender = (byte) model.Profile.Gender.Value;

            var bodyKeys = charBlocks.Where(block => block.Key.StartsWith("BODY", StringComparison.Ordinal))
                .ToArray();

            var bodyEnumKeys = bodyKeys.Where(pair => pair.Value is EnumDataBlock && !pair.Key.Contains("HAIR"));

            var bodyValueKeys = bodyKeys.Where(pair => pair.Value is ByteDataBlock && !pair.Key.Contains("HAIR"));

            var bodyBoolKeys = bodyKeys.Where(pair => pair.Value is BoolDataBlock);

            //Body Values
            var bodyDeviation = bodyValueKeys.Select
            (pair => new StandardDeviation
            {
                Samples = StandardDeviation.GetSamples(collection, gender, pair.Key),
                Key = pair.Key
            });

            foreach (StandardDeviation dev in bodyDeviation)
            {
                if (dev.Key == "BODY_FACE_TYPE_F" && gender == 0)
                    continue;
                if (dev.Key == "BODY_FACE_TYPE_M" && gender == 1)
                    continue;
                double factor = 2 * Core.Random.NextDouble() - 1;
                double newVal = dev.Average + dev.Deviation * factor;
                newVal = Math.Round(newVal);
                newVal = newVal.Clamp(0, 255);
                model.Character.CharAttributes[dev.Key].Value = (byte) newVal;
            }

            //Body Enums
            var enumDeviation = bodyEnumKeys.Select
            (pair => new StandardDeviation
            {
                Samples = StandardDeviation.GetSamples(collection, gender, pair.Key),
                Key = pair.Key
            });

            foreach (StandardDeviation dev in enumDeviation)
            {
                if (dev.Key == "BODY_FIGURE_FEMALE" && gender == 0)
                    continue;
                if (dev.Key == "BODY_FIGURE_MALE" && gender == 1)
                    continue;
                if (dev.Key == "BODY_BREAST_RANK")
                    continue;
                double factor = Core.Random.NextDouble();
                double newVal;
                switch (dev.Key)
                {
                    case "BODY_TAN_MARK":
                    case "BODY_IRIS_SHAPE":
                    case "BODY_IRIS_TYPE":
                    case "BODY_IRIS_HIGHLIGHT_TYPE":
                    case "BODY_GLASSES_TYPE":
                    case "BODY_LIPSTICK_COLOR":
                        var enumValues = (charBlocks[dev.Key] as EnumDataBlock).Enum.Keys.ToArray();
                        newVal = enumValues[(int) Math.Round((enumValues.Length - 1) * factor)];
                        newVal = newVal.Clamp(0, 255);
                        model.Character.CharAttributes[dev.Key].Value = (byte) newVal;
                        break;
                    default:
                        newVal = dev.Average + dev.Deviation * (2 * factor - 1);
                        newVal = newVal.Clamp(0, 255);
                        newVal = Math.Round(newVal);
                        model.Character.CharAttributes[dev.Key].Value = (byte) newVal;
                        break;
                }
            }

            //Body Bools
            foreach (var pair in bodyBoolKeys)
            {
                double factor = Core.Random.NextDouble();
                double delta = Math.Abs(0.5 - factor);

                switch (pair.Key)
                {
                    case "BODY_CHEEKBONE_MOLE_LEFT":
                    case "BODY_CHEEKBONE_MOLE_RIGHT":
                    case "BODY_CHIN_MOLE_LEFT":
                    case "BODY_CHIN_MOLE_RIGHT":
                        model.Character.CharAttributes[pair.Key].Value = delta > 0.4;
                        break;
                    case "BODY_GLASSES":
                        model.Character.CharAttributes[pair.Key].Value = delta > 0.25;
                        break;
                }
            }

            var skinSamples = collection.Characters.Where(chr => (byte) chr.Profile.Gender.Value == gender)
                .Select
                (chr =>
                {
                    Color color = (Color) chr.Character.CharAttributes["BODY_SKIN_COLOR"].Value;
                    double h, s, l;
                    ColorHelper.ColorToHSL(color, out h, out s, out l);
                    return new
                    {
                        H = h,
                        S = s,
                        L = l
                    };
                });

            StandardDeviation skinHueDev = new StandardDeviation
            {
                Samples = skinSamples.Select(arg => arg.H)
            };

            StandardDeviation skinSatDev = new StandardDeviation
            {
                Samples = skinSamples.Select(arg => arg.S)
            };

            StandardDeviation skinLigDev = new StandardDeviation
            {
                Samples = skinSamples.Select(arg => arg.L)
            };

            double hue = skinHueDev.Average + skinHueDev.Deviation * (2 * Core.Random.NextDouble() - 1);
            double sat = skinSatDev.Average + skinSatDev.Deviation * (2 * Core.Random.NextDouble() - 1);
            double lig = skinLigDev.Average + skinLigDev.Deviation * (2 * Core.Random.NextDouble() - 1);

            Color skinColor = ColorHelper.ColorFromHSL(hue, sat, lig);

            model.Body.BodyPart.SkinColor.Value = skinColor;
        }
    }
}