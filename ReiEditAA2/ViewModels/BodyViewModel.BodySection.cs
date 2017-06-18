// --------------------------------------------------
// ReiEditAA2 - BodyViewModel.BodySection.cs
// --------------------------------------------------

using AA2Lib.Code.DataBlocks;

namespace ReiEditAA2.ViewModels
{
    internal partial class BodyViewModel
    {
        public class BodySection
        {
            public BodyViewModel BodyViewModel { get; private set; }

            public DataBlockWrapper BreastDepth
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_BREAST_DEPTH); }
            }

            public DataBlockWrapper BreastDirection
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_BREAST_DIRECTION); }
            }

            public DataBlockWrapper BreastFirmness
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_BREAST_FIRMNESS); }
            }

            public DataBlockWrapper BreastHeight
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_BREAST_HEIGHT); }
            }

            public DataBlockWrapper BreastRank
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_BREAST_RANK); }
            }

            public DataBlockWrapper BreastRoundness
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_BREAST_ROUNDNESS); }
            }

            public DataBlockWrapper BreastShape
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_BREAST_SHAPE); }
            }

            public DataBlockWrapper BreastSize
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_BREAST_SIZE); }
            }

            public DataBlockWrapper BreastSpacing
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_BREAST_SPACING); }
            }

            public DataBlockWrapper Figure
            {
                get
                {
                    if (BodyViewModel.CharacterViewModel.IsDisposed)
                        return null;
                    bool ismale = BodyViewModel.CharacterViewModel.Profile.Gender.Value.Equals((byte) 0);
                    return ismale
                        ? FigureMale
                        : FigureFemale;
                }
            }

            public DataBlockWrapper FigureFemale
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_FIGURE_F); }
            }

            public DataBlockWrapper FigureMale
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_FIGURE_M); }
            }

            public DataBlockWrapper HeadHeight
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_HEAD_HEIGHT); }
            }

            public DataBlockWrapper HeadSize
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_HEAD_SIZE); }
            }

            public DataBlockWrapper Height
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_HEIGHT); }
            }

            public DataBlockWrapper NippleColor
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_NIPPLE_COLOR); }
            }

            public DataBlockWrapper NippleOpacity
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_NIPPLE_OPACITY); }
            }

            public DataBlockWrapper NippleSize
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_NIPPLE_SIZE); }
            }

            public DataBlockWrapper NippleType
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_NIPPLE_TYPE); }
            }

            public DataBlockWrapper PubicHairColor
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_PUBIC_HAIR_COLOR); }
            }

            public DataBlockWrapper PubicHairOpacity
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_PUBIC_HAIR_OPACITY); }
            }

            public DataBlockWrapper PubicHairShape
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_PUBIC_HAIR_SHAPE); }
            }

            public DataBlockWrapper PussyMosaicColor
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_PUSSY_MOSAIC_COLOR); }
            }

            public DataBlockWrapper SkinColor
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_SKIN_COLOR); }
            }

            public DataBlockWrapper TanMark
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_TAN_MARK); }
            }

            public DataBlockWrapper TanOpacity
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_TAN_OPACITY); }
            }

            public DataBlockWrapper Waist
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_WAIST_SIZE); }
            }

            public BodySection(BodyViewModel parent)
            {
                BodyViewModel = parent;
            }
        }
    }
}