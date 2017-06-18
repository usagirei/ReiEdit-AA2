// --------------------------------------------------
// ReiEditAA2 - ICharacterViewModelProvider.cs
// --------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using ReiEditAA2.ViewModels;

namespace ReiEditAA2.Code.CharacterViewModelProviders
{
    internal interface ICharacterViewModelProvider : IDisposable
    {
        event CharacterProviderDelegate CharacterAdded;
        event CharacterLoadedDelegate CharacterLoaded;
        event CharacterProviderDelegate CharacterUpdated;

        ListSortDirection DefaultSortDirection { get; }
        string DefaultSortProperty { get; }

        bool Finish();

        IEnumerable<CharacterViewModel> GetCharacters();

        Dictionary<string, string> GetSortableProperties();

        void Initialize(Window window);

        //event CharacterProviderDelegate CharacterDeleted;
    }
}