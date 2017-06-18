// --------------------------------------------------
// ReiEditAA2 - BodyViewModel.HairSection.cs
// --------------------------------------------------

using AA2Lib.Code.DataBlocks;

namespace ReiEditAA2.ViewModels
{
    internal partial class BodyViewModel
    {
        public class HairSection
        {
            public DataBlockWrapper BackHair
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_BACK_HAIR); }
            }

            public DataBlockWrapper BackHairFlip
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_BACK_HAIR_FLIP); }
            }

            public DataBlockWrapper BackHairLength
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_BACK_HAIR_LENGTH); }
            }

            public BodyViewModel BodyViewModel { get; private set; }

            public DataBlockWrapper ExtensionHair
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_EXTENSION_HAIR); }
            }

            public DataBlockWrapper ExtensionHairFlip
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_EXTENSION_HAIR_FLIP); }
            }

            public DataBlockWrapper ExtensionHairLength
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_EXTENSION_HAIR_LENGTH); }
            }

            public DataBlockWrapper FrontHair
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_FRONT_HAIR); }
            }

            public DataBlockWrapper FrontHairFlip
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_FRONT_HAIR_FLIP); }
            }

            public DataBlockWrapper FrontHairLength
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_FRONT_HAIR_LENGTH); }
            }

            public DataBlockWrapper HairColor
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_HAIR_COLOR); }
            }

            public DataBlockWrapper SideHair
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_SIDE_HAIR); }
            }

            public DataBlockWrapper SideHairFlip
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_SIDE_HAIR_FLIP); }
            }

            public DataBlockWrapper SideHairLength
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_SIDE_HAIR_LENGTH); }
            }

            public HairSection(BodyViewModel parent)
            {
                BodyViewModel = parent;
            }
        }
    }
}