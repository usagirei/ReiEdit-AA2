// --------------------------------------------------
// ReiEditAA2 - PluginCharacterProxy.cs
// --------------------------------------------------

using System;
using AA2Lib.Code.DataBlocks;
using ReiEditAA2.Code.CharacterViewModelProviders;
using ReiEditAA2.ViewModels;

namespace ReiEditAA2.Plugins
{
    public class PluginCharacterProxy : MarshalByRefObject
    {
        internal readonly CharacterViewModel _viewModel;

        internal CharacterViewModel ViewModel
        {
            get { return _viewModel; }
        }

        internal PluginCharacterProxy(CharacterViewModel vm)
        {
            _viewModel = vm;
        }

        public object Metadata
        {
            get { return ViewModel.Metadata; }
        }

        public object GetAttribute(string attr)
        {
            DataBlockWrapper attribute = ViewModel.GetAttribute(attr);
            if (attribute != null)
                return attribute.Value;
            return null;
        }

        public T GetAttribute<T>(string attr)
        {
            /*
            DataBlockWrapper attribute = ViewModel.GetAttribute(attr);
            if (attribute != null && attribute.Value is T)
                return (T) attribute.Value;
            return default(T);
            */
            object val = GetAttribute(attr);
            if (val is T)
                return (T) val;
            return default(T);
        }

        public object GetPlayData(string attr)
        {
            if (!ViewModel.ExtraData.ContainsKey("PLAY_DATA"))
                return null;

            DataBlockWrapperBuffer wb = ViewModel.ExtraData["PLAY_DATA"] as DataBlockWrapperBuffer;
            DataBlockWrapper playData = wb.GetAttribute(attr);

            if (playData != null)
                return playData.Value;

            return null;
        }

        public T GetPlayData<T>(string attr)
        {
            /*
            if (!ViewModel.ExtraData.ContainsKey("PLAY_DATA"))
                return default(T);

            DataBlockWrapperBuffer wb = ViewModel.ExtraData["PLAY_DATA"] as DataBlockWrapperBuffer;
            DataBlockWrapper playData = wb.GetAttribute(attr);

            if (playData != null && playData.Value is T)
                return (T) playData.Value;

            return default(T);
            */
            object val = GetPlayData(attr);
            if (val is T)
                return (T) val;
            return default(T);
        }

        public void SetAttribute(string attr, object value)
        {
            DataBlockWrapper attribute = ViewModel.GetAttribute(attr);
            if (attribute != null)
                attribute.Value = value;
        }

        public void SetPlayData(string attr, object value)
        {
            if (!ViewModel.ExtraData.ContainsKey("PLAY_DATA"))
                return;

            DataBlockWrapperBuffer wb = ViewModel.ExtraData["PLAY_DATA"] as DataBlockWrapperBuffer;
            DataBlockWrapper playData = wb.GetAttribute(attr);

            if (playData != null)
                playData.Value = value;
        }
    }
}