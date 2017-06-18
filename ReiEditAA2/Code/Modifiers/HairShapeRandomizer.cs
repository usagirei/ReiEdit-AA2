// --------------------------------------------------
// ReiEditAA2 - HairShapeRandomizer.cs
// --------------------------------------------------

using System;
using System.Linq;
using AA2Lib;
using ReiEditAA2.ViewModels;

namespace ReiEditAA2.Code.Modifiers
{
    internal class HairShapeRandomizer : CharModifierBase
    {
        public override string Name
        {
            get { return "Randomize Hair Shape"; }
        }

        public override void Modify(CharacterViewModel model)
        {
            //Pubic Hair
            var enumValues = model.Body.BodyPart.PubicHairShape.EnumData.Keys.ToArray();
            int pubicHairIndex = (int) Math.Round((enumValues.Length - 1) * Core.Random.NextDouble());
            double pubicShape = enumValues[pubicHairIndex];
            pubicShape = pubicShape.Clamp(0, 255);
            model.Body.BodyPart.PubicHairShape.Value = (byte) pubicShape;
            model.Body.BodyPart.PubicHairOpacity.Value = (byte) Core.Random.Next(0, 100);

            string[] slots = {"FRONT", "SIDE", "BACK", "EXTENSION"};
            const string num = "BODY_{0}_HAIR";
            const string len = "BODY_{0}_HAIR_LENGTH";
            const string flp = "BODY_{0}_HAIR_FLIP";

            //Hairs
            for (int n = 0; n < 4; n++)
            {
                int slotId = n;
                var hairs = CharacterHairProvider.Items.Where(item => (int) item.Slot == slotId)
                    .ToArray();
                int numHairs = hairs.Length - 1;
                int hairIndex = (int) (numHairs * Core.Random.NextDouble());
                CharacterHairProvider.CharacterHair selected = hairs[hairIndex];

                if (selected == null)
                    continue;

                string hairNum = string.Format(num, slots[n]);
                string hairLen = string.Format(len, slots[n]);
                string hairFlip = string.Format(flp, slots[n]);

                model.Character.CharAttributes[hairNum].Value = selected.Id;
                model.Character.CharAttributes[hairLen].Value = (byte) Core.Random.Next(0, 100);
                model.Character.CharAttributes[hairFlip].Value = selected.Flippable
                                                                 && Math.Abs(0.5 - Core.Random.NextDouble()) > 0.25;
            }
        }
    }
}