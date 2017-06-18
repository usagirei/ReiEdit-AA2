// --------------------------------------------------
// ReiEditAA2 - BodyViewModel.cs
// --------------------------------------------------

namespace ReiEditAA2.ViewModels
{
    internal partial class BodyViewModel
    {
        public BodySection BodyPart { get; private set; }
        public CharacterViewModel CharacterViewModel { get; private set; }

        public FaceSection FacePart { get; private set; }
        public HairSection HairPart { get; private set; }

        public BodyViewModel(CharacterViewModel parent)
        {
            CharacterViewModel = parent;
            BodyPart = new BodySection(this);
            FacePart = new FaceSection(this);
            HairPart = new HairSection(this);
        }

        private static class Keys
        {
            public const string KEY_BACK_HAIR = "BODY_BACK_HAIR";
            public const string KEY_BACK_HAIR_FLIP = "BODY_BACK_HAIR_FLIP";
            public const string KEY_BACK_HAIR_LENGTH = "BODY_BACK_HAIR_LENGTH";
            public const string KEY_BREAST_DEPTH = "BODY_BREAST_DEPTH";
            public const string KEY_BREAST_DIRECTION = "BODY_BREAST_DIRECTION";
            public const string KEY_BREAST_FIRMNESS = "BODY_BREAST_FIRMNESS";
            public const string KEY_BREAST_HEIGHT = "BODY_BREAST_HEIGHT";
            public const string KEY_BREAST_RANK = "BODY_BREAST_RANK";
            public const string KEY_BREAST_ROUNDNESS = "BODY_BREAST_ROUNDNESS";
            public const string KEY_BREAST_SHAPE = "BODY_BREAST_SHAPE";
            public const string KEY_BREAST_SIZE = "BODY_BREAST_SIZE";
            public const string KEY_BREAST_SPACING = "BODY_BREAST_SPACING";
            public const string KEY_CHEEKBONE_MOLE_LEFT = "BODY_CHEEKBONE_MOLE_LEFT";
            public const string KEY_CHEEKBONE_MOLE_RIGHT = "BODY_CHEEKBONE_MOLE_RIGHT";
            public const string KEY_CHIN_MOLE_LEFT = "BODY_CHIN_MOLE_LEFT";
            public const string KEY_CHIN_MOLE_RIGHT = "BODY_CHIN_MOLE_RIGHT";
            public const string KEY_EXTENSION_HAIR = "BODY_EXTENSION_HAIR";
            public const string KEY_EXTENSION_HAIR_FLIP = "BODY_EXTENSION_HAIR_FLIP";
            public const string KEY_EXTENSION_HAIR_LENGTH = "BODY_EXTENSION_HAIR_LENGTH";
            public const string KEY_EXTERNAL_HIGHLIGHT_TEXTURE = "BODY_EXTERNAL_HIGHLIGHT_TEXTURE";
            public const string KEY_EXTERNAL_HIGHLIGHT_TEXTURE_NAME = "BODY_EXTERNAL_EYE_HIGHLIGHT_TEXTURE_NAME";
            public const string KEY_EXTERNAL_IRIS_TEXTURE = "BODY_EXTERNAL_IRIS_TEXTURE";
            public const string KEY_EXTERNAL_IRIS_TEXTURE_NAME = "BODY_EXTERNAL_IRIS_TEXTURE_NAME";
            public const string KEY_EYEBROW_ANGLE = "BODY_EYEBROW_ANGLE";
            public const string KEY_EYEBROW_COLOR = "BODY_EYEBROW_COLOR";
            public const string KEY_EYEBROW_SHAPE = "BODY_EYEBROW_SHAPE";
            public const string KEY_EYELID = "BODY_EYELID";
            public const string KEY_EYE_ANGLE = "BODY_EYE_ANGLE";
            public const string KEY_EYE_HEIGHT = "BODY_EYE_HEIGHT";
            public const string KEY_EYE_HIGHLIGHT_TYPE = "BODY_EYE_HIGHLIGHT_TYPE";
            public const string KEY_EYE_POSITION = "BODY_EYE_POSITION";
            public const string KEY_EYE_SPACING = "BODY_EYE_SPACING";
            public const string KEY_EYE_WIDTH = "BODY_EYE_WIDTH";
            public const string KEY_FACE_TYPE_F = "BODY_FACE_TYPE_F";
            public const string KEY_FACE_TYPE_M = "BODY_FACE_TYPE_M";
            public const string KEY_FIGURE_F = "BODY_FIGURE_FEMALE";
            public const string KEY_FIGURE_M = "BODY_FIGURE_MALE";
            public const string KEY_FRONT_HAIR = "BODY_FRONT_HAIR";
            public const string KEY_FRONT_HAIR_FLIP = "BODY_FRONT_HAIR_FLIP";

            public const string KEY_FRONT_HAIR_LENGTH = "BODY_FRONT_HAIR_LENGTH";

            //Glasses
            public const string KEY_GLASSES = "BODY_GLASSES";

            public const string KEY_GLASSES_COLOR = "BODY_GLASSES_COLOR";
            public const string KEY_GLASSES_TYPE = "BODY_GLASSES_TYPE";
            public const string KEY_HAIR_COLOR = "BODY_HAIR_COLOR";
            public const string KEY_HEAD_HEIGHT = "BODY_HEAD_HEIGHT";
            public const string KEY_HEAD_SIZE = "BODY_HEAD_SIZE";
            public const string KEY_HEIGHT = "BODY_HEIGHT";
            public const string KEY_IRIS_HEIGHT = "BODY_IRIS_HEIGHT";
            public const string KEY_IRIS_POSITION = "BODY_IRIS_POSITION";
            public const string KEY_IRIS_SHAPE = "BODY_IRIS_SHAPE";
            public const string KEY_IRIS_TYPE = "BODY_IRIS_TYPE";
            public const string KEY_IRIS_WIDTH = "BODY_IRIS_WIDTH";

            public const string KEY_LEFT_EYE_COLOR = "BODY_LEFT_EYE_COLOR";

            //Face Details
            public const string KEY_LIPSTICK_COLOR = "BODY_LIPSTICK_COLOR";

            public const string KEY_LIPSTICK_OPACITY = "BODY_LIPSTICK_OPACITY";
            public const string KEY_LOWER_EYELID = "BODY_LOWER_EYELID";
            public const string KEY_NIPPLE_COLOR = "BODY_NIPPLE_COLOR";
            public const string KEY_NIPPLE_OPACITY = "BODY_NIPPLE_OPACITY";
            public const string KEY_NIPPLE_SIZE = "BODY_NIPPLE_SIZE";
            public const string KEY_NIPPLE_TYPE = "BODY_NIPPLE_TYPE";
            public const string KEY_PUBIC_HAIR_COLOR = "BODY_PUBIC_HAIR_COLOR";
            public const string KEY_PUBIC_HAIR_OPACITY = "BODY_PUBIC_HAIR_OPACITY";
            public const string KEY_PUBIC_HAIR_SHAPE = "BODY_PUBIC_HAIR_SHAPE";
            public const string KEY_PUSSY_MOSAIC_COLOR = "BODY_PUSSY_MOSAIC_COLOR";

            public const string KEY_RIGHT_EYE_COLOR = "BODY_RIGHT_EYE_COLOR";

            //Hair
            public const string KEY_SIDE_HAIR = "BODY_SIDE_HAIR";

            public const string KEY_SIDE_HAIR_FLIP = "BODY_SIDE_HAIR_FLIP";
            public const string KEY_SIDE_HAIR_LENGTH = "BODY_SIDE_HAIR_LENGTH";
            public const string KEY_SKIN_COLOR = "BODY_SKIN_COLOR";
            public const string KEY_TAN_MARK = "BODY_TAN_MARK";
            public const string KEY_TAN_OPACITY = "BODY_TAN_OPACITY";
            public const string KEY_UPPER_EYELID = "BODY_UPPER_EYELID";
            public const string KEY_WAIST_SIZE = "BODY_WAIST_SIZE";
        }
    }
}