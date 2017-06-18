// --------------------------------------------------
// ReiEditAA2 - PluginMenu.cs
// --------------------------------------------------

using System.Linq;
using ReiEditAA2.Code.Modifiers;

namespace ReiEditAA2.Plugins
{
    internal class PluginMenu
    {
        private PluginModifier[] _pluginModifiers;

        public PluginBase Base { get; private set; }

        public PluginModifier[] Modifiers
        {
            get
            {
                return _pluginModifiers ?? (_pluginModifiers = Base.Actions.Select(action => new PluginModifier(action))
                           .ToArray());
            }
        }

        public string Name
        {
            get { return Base.Name; }
        }

        public PluginMenu(PluginBase @base)
        {
            Base = @base;
        }
    }
}