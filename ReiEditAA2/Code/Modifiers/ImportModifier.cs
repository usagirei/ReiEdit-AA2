// --------------------------------------------------
// ReiEditAA2 - ImportModifier.cs
// --------------------------------------------------

using System.IO;
using AA2Lib;
using AA2Lib.Code;
using Microsoft.Win32;
using ReiEditAA2.ViewModels;
using ReiEditAA2.Views;

namespace ReiEditAA2.Code.Modifiers
{
    internal class ImportModifier : CharModifierBase
    {
        public override string Name
        {
            get { return "Replace with Imported Character"; }
        }

        public override void Modify(CharacterViewModel model)
        {
            OpenFileDialog opfd = new OpenFileDialog
            {
                Filter = "PNG Files (*.png)|*.png",
                CheckFileExists = true,
                InitialDirectory = Core.EditSaveDir,
                Multiselect = false,
            };
            opfd.CustomPlaces.Add(new FileDialogCustomPlace(Core.EditSaveDir));
            if (!opfd.ShowDialog()
                .Value)
                return;
            CharacterFile inC;
            using (Stream s = File.OpenRead(opfd.FileName))
                inC = CharacterFile.Load(s);
            if (inC == null)
                return;

            ImportWindow iWnd = new ImportWindow
            {
                Imported = inC,
                Target = model.Character
            };
            iWnd.ShowDialog();
        }
    }
}