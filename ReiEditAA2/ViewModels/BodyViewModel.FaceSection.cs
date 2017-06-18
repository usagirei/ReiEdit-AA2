// --------------------------------------------------
// ReiEditAA2 - BodyViewModel.FaceSection.cs
// --------------------------------------------------

using AA2Lib.Code;
using AA2Lib.Code.DataBlocks;

namespace ReiEditAA2.ViewModels
{
    internal partial class BodyViewModel
    {
        public class FaceSection
        {
            public BodyViewModel BodyViewModel { get; private set; }

            public DataBlockWrapper CheekboneMoleLeft
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_CHEEKBONE_MOLE_LEFT); }
            }

            public DataBlockWrapper CheekboneMoleRight
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_CHEEKBONE_MOLE_RIGHT); }
            }

            public DataBlockWrapper ChinMoleLeft
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_CHIN_MOLE_LEFT); }
            }

            public DataBlockWrapper ChinMoleRight
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_CHIN_MOLE_RIGHT); }
            }

            public DataBlockWrapper ExternalHighlightTexture
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_EXTERNAL_HIGHLIGHT_TEXTURE); }
            }

            public DataBlockWrapper ExternalHighlightTextureName
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_EXTERNAL_HIGHLIGHT_TEXTURE_NAME); }
            }

            public DataBlockWrapper ExternalIrisTexture
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_EXTERNAL_IRIS_TEXTURE); }
            }

            public DataBlockWrapper ExternalIrisTextureName
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_EXTERNAL_IRIS_TEXTURE_NAME); }
            }

            public DataBlockWrapper EyeAngle
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_EYE_ANGLE); }
            }

            public DataBlockWrapper EyeHeight
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_EYE_HEIGHT); }
            }

            public DataBlockWrapper EyeHighlightType
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_EYE_HIGHLIGHT_TYPE); }
            }

            public DataBlockWrapper EyePosition
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_EYE_POSITION); }
            }

            public DataBlockWrapper EyeSpacing
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_EYE_SPACING); }
            }

            public DataBlockWrapper EyeWidth
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_EYE_WIDTH); }
            }

            public DataBlockWrapper EyebrowAngle
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_EYEBROW_ANGLE); }
            }

            public DataBlockWrapper EyebrowColor
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_EYEBROW_COLOR); }
            }

            public DataBlockWrapper EyebrowShape
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_EYEBROW_SHAPE); }
            }

            public DataBlockWrapper Eyelid
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_EYELID); }
            }

            public DataBlockWrapper FaceType
            {
                get
                {
                    if (BodyViewModel.CharacterViewModel.IsDisposed)
                        return null;
                    bool ismale = BodyViewModel.CharacterViewModel.Profile.Gender.Value.Equals((byte) 0);
                    return ismale
                        ? FaceTypeM
                        : FaceTypeF;
                }
            }

            public DataBlockWrapper FaceTypeF
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_FACE_TYPE_F); }
            }

            public DataBlockWrapper FaceTypeM
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_FACE_TYPE_M); }
            }

            public DataBlockWrapper Glasses
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_GLASSES); }
            }

            public DataBlockWrapper GlassesColor
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_GLASSES_COLOR); }
            }

            public DataBlockWrapper GlassesType
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_GLASSES_TYPE); }
            }

            public DataBlockWrapper IrisHeight
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_IRIS_HEIGHT); }
            }

            public DataBlockWrapper IrisPosition
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_IRIS_POSITION); }
            }

            public DataBlockWrapper IrisShape
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_IRIS_SHAPE); }
            }

            public DataBlockWrapper IrisType
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_IRIS_TYPE); }
            }

            public DataBlockWrapper IrisWidth
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_IRIS_WIDTH); }
            }

            public DataBlockWrapper LeftEyeColor
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_LEFT_EYE_COLOR); }
            }

            public DataBlockWrapper LipstickColor
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_LIPSTICK_COLOR); }
            }

            public DataBlockWrapper LipstickOpacity
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_LIPSTICK_OPACITY); }
            }

            public DataBlockWrapper LowerEyelid
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_LOWER_EYELID); }
            }

            public DataBlockWrapper RightEyeColor
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_RIGHT_EYE_COLOR); }
            }

            public TextureWatcher TextureWatcher
            {
                get { return TextureWatcher.Instance; }
            }

            public DataBlockWrapper UpperEyelid
            {
                get { return BodyViewModel.CharacterViewModel.GetAttribute(Keys.KEY_UPPER_EYELID); }
            }

            public FaceSection(BodyViewModel parent)
            {
                BodyViewModel = parent;
            }
        }
    }
}