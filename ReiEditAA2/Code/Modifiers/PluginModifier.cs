// --------------------------------------------------
// ReiEditAA2 - PluginModifier.cs
// --------------------------------------------------

using System;
using System.Linq;
using System.Windows;
using ReiEditAA2.Plugins;
using ReiEditAA2.ViewModels;

namespace ReiEditAA2.Code.Modifiers
{
    internal class PluginModifier : CharModifierBase
    {
        private readonly PluginAction _action;

        public override string Name
        {
            get { return Action.Name; }
        }

        internal PluginAction Action
        {
            get { return _action; }
            //set { _action = value; }
        }

        public PluginModifier()
        { }

        public PluginModifier(PluginAction @base)
        {
            _action = @base;
        }

        public override void Modify(CharacterViewModel model)
        {
            PluginLoader.LogHeader(Action.Name + " - " + DateTime.Now);
            try
            {
                Action.Character = model.Proxy;
                Action.Characters = model.ParentCollection.Characters.Select(m => m.Proxy)
                    .ToArray();
                Action.Modify();
                Action.Character = null;
                Action.Characters = null;
            }
            catch (Exception ex)
            {
                PluginLoader.Log("EXCEPTION: {0}", ex.Message);
                MessageBox.Show(string.Format("Exception Thrown on '{0}', Check DynamicPlugin.txt for more information", Action.Name),
                    "Action has Crashed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            PluginLoader.LogFooter();
        }
    }
}