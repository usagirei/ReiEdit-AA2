// --------------------------------------------------
// ReiEditAA2 - CharModifierBase.cs
// --------------------------------------------------

using System.Windows.Input;
using AA2Lib.Code;
using ReiEditAA2.ViewModels;

namespace ReiEditAA2.Code.Modifiers
{
    internal abstract class CharModifierBase
    {
        public ICommand ModifyCommand { get; private set; }
        public abstract string Name { get; }

        protected CharModifierBase()
        {
            ModifyCommand = new RelayCommand<CharacterViewModel>(Modify, model => model != null);
        }

        public abstract void Modify(CharacterViewModel model);
    }
}