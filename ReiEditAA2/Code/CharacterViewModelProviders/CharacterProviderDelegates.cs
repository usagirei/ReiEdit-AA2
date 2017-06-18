// --------------------------------------------------
// ReiEditAA2 - CharacterProviderDelegates.cs
// --------------------------------------------------

using ReiEditAA2.ViewModels;

namespace ReiEditAA2.Code.CharacterViewModelProviders
{
    internal delegate void CharacterProviderDelegate(object sender, CharacterViewModelProviderEventArgs args);

    internal delegate void CharacterLoadedDelegate(object sender, CharacterLoadedDelegateArgs args);

    internal class CharacterLoadedDelegateArgs
    {
        public int Current { get; set; }
        public string Message { get; set; }
        public int NumChars { get; set; }

        public CharacterLoadedDelegateArgs(int current, int max, string message = null)
        {
            NumChars = max;
            Current = current;
            Message = message;
        }
    }

    internal class CharacterViewModelProviderEventArgs
    {
        public CharacterViewModel Model { get; private set; }

        public object NewMetadata { get; private set; }
        public object OldMetadata { get; private set; }

        public CharacterViewModelProviderEventArgs(
            CharacterViewModel model,
            object oldMetadata = null,
            object newMetadata = null)
        {
            Model = model;
            OldMetadata = oldMetadata;
            NewMetadata = newMetadata;
        }
    }
}