// --------------------------------------------------
// ReiEditAA2 - SexualItemsCharRandomizer.cs
// --------------------------------------------------

using System;
using ReiEditAA2.ViewModels;

namespace ReiEditAA2.Code.Modifiers
{
    internal class SexualItemsCharRandomizer : CharModifierBase
    {
        private const string RANDOM_ITEM_FORMAT = "{0} {1}'s {2}";

        public override string Name
        {
            get { return "Randomize Item: Sexual"; }
        }

        public override void Modify(CharacterViewModel model)
        {
            byte personality = (byte) model.Profile.Personality.Value;
            byte gender = (byte) model.Profile.Gender.Value;
            string familyName = (string) model.Profile.FamilyName.Value;
            string firstName = (string) model.Profile.FirstName.Value;
            string generated = ProfileItemProvider.GenerateLoversItem(personality, gender);

            model.Profile.ItemSexual.Value = String.Format(RANDOM_ITEM_FORMAT, familyName, firstName, generated);
        }
    }
}