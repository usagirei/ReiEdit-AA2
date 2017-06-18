// --------------------------------------------------
// ReiEditAA2 - PluginBase.cs
// --------------------------------------------------

using System;

namespace ReiEditAA2.Plugins
{
    public abstract class PluginBase : MarshalByRefObject
    {
        public abstract PluginAction[] Actions { get; }
        public abstract string Name { get; }
    }

    public abstract class PluginAction : MarshalByRefObject
    {
        public PluginCharacterProxy Character { get; set; }
        public PluginCharacterProxy[] Characters { get; set; }
        public abstract string Name { get; }

        public void Log(string format, params object[] args)
        {
            PluginLoader.Log(format, args);
        }

        public abstract void Modify();
    }
}