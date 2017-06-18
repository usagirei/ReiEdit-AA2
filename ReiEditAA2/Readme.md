###ReiEditAA2 - Beta 0.9.5.2

by usagirei - May 18th 2015

ReiEditAA2 is a Character/Save Editor for ILLUSION's Artificial Academy 2

####System Requeriments:
* .NET Framework 4.5[1] - Until 0.9.3.0
* .NET Framework 4.0[2] - 0.9.3.1 Onwards
* 512MB RAM
* DirectX 9 / Shader Model 2 Capable Graphics Adapter
* Ability to Read
* Artificial Academy 2 + Editor


####Features
* Load Characters from Game Save
* Load Characters from Editor Directories
* Export Any Loaded Characters
* Edit Any Character Property Avaiable in ILLUSION's Editor
* Unlocked Character Properties (Body Sizes, Hair Lenghts, etc.)
* Customizable Random Items, with Gender/Personality locks (see XML\Random directory)
* Customizable Personality Selector (see XML\Overrides directory)
* Character Clothing Editor with Full Support to .cloth Files (import/export, loadable ingame)
* Pregnancy Risk and H-Compatibility[3] Editor
* Save Game PlayData Support [NEW IN 0.9.1.0]


####Planned Features
* More Complete Game Save Editor: (Academy Data, etc)


####Thanks to
* The People who did JG2ChrData, for the PNG and PlayData File Offsets.


####Reference Links
* [1](http://www.microsoft.com/en-us/download/details.aspx?id=8483) .Net Framework 4.5
* [2](http://www.microsoft.com/en-us/download/details.aspx?id=17718) .Net Framework 4
* [3](http://wiki.anime-sharing.com/hgames/index.php/Artificial_Academy_2/H_Guide#H_compatibility) H-Compatibility


####Changelog:
######0.9.0.0b
* First Public Release

######0.9.1.0b
* Engine Rewrite to Support Dynamic Addressing/Variable Size Data (PlayData)
* Fixed Typos in Trait Names
* Fixed Wrong Char Data Address
* Added Advanced Tab with Play Data
* Changed Seat Numbers to Zero Indexed to Reduce Confusion in PlayData Tab
* Changed Editor Load mode to Male/Female Directories only
* Added Extra Option to Load Editor Directory with Subdirectories Included
* Changed Pregnancy Risk to Normal/Safe/Danger

######0.9.2.0b
* Added Support for Append Set and Future-Proofed for Possible Future Add-ons
* Fixed a Overflow due to Off-Range values in Suit Colors
* Added Sock/Shoes/Suit Variants for Male and Female in the Suit Editor
* Fixed a Overflow when loading a Save File with Retired Students

######0.9.3.0b
* Added Save File Header Editor Dialog (Clubs, Academy Name, Year, etc)
* Changed "Always Angry" trait to "Always Hungry"

######0.9.3.1b
* Downgraded to .NET Framework 4.0 to target Windows XP (I recommend upgrading your system, WinXP is Vulnerable)
* Changed "Schemer" to "Schemer (M)" in personality names

######0.9.3.2b
* Fixed Exception on Windows XP due to a system color not avaiable on it.
* Added Hgg Personality name Override, rename XML\Overrides\zHggNames.xml.disabled to zHggNames.xml in order to use it.

######0.9.3.3b
* Should actually work on Windows XP now

######0.9.4.0b
* Added Card Generator Tool
  Uses CPU for Image Processing (No HW-Acceleration), speeds will vary on a processor basis (Multicore Enabled)
  New Tools to process Characters also Include: Body Randomizer (Standard Deviation, generates bodies similar to current loaded characters), Hair Randomizer, Eye and Hair Color Randomizer
  Moved old Item Randomizers to New Tools Format/Menu
* Save Files now create a backup on saving
* Individual Byte[]'s for Character Image/Thumbnail and Data, Enabling the Card Generator to wor
* Save Format Provider Rewrite, to allow the above two changes. Automatic Count of Male/Female Students in header upon save.
* Added HSV-RGB Color Picker 
  Sliders are a bit jumpy (specially white-ish colors) due to data-loss upon Color space conversion
  Picker Uses Windows Native API (BitBlt) instead of Managed CopyFromScreen, Takes a 1x1 Screenshot at mouse position
* Added FileSystem Characters Batch Renamer and Information Exporter 
  Select from ComboBox, Enter to Add Property, Del to Remove, Alt+Up/Down to Reorder selected
  Uses .NET String Format "{0}" for First Property, "{1}" for second, "{2:D3}" for 3 digit padded integer, "{3:X2}" for 2 Digit Hex, etc...
* Added Option to load a Specific Directory for FileSystem Characters
* Fixed Swapped Mole Buttons
* Added Seat Picker Combo for PlayData seat properties and Current Player for Saves

######0.9.4.1b
* 'Fixed' Abnormally High Memory Usage, a little residual memory is still not being disposed properly.

######0.9.4.2b
* Fixed Crash when loading characters after a previous list was already loaded
* Added Extra Suit Data into Editor (IsUnderwear/Skirt/Swimsuit)
* Added Tooltip with extra information regarding the character (FileName mainly) when hovering the portrait

######0.9.4.3b - [Unstable]
* Fixed Flippable Hair Flag being Kept on Non-Flippable Hair Selection
* Added Character Import Dialog (Under 'Tools > Replace with Imported Character')

######0.9.5.0b
* Added Plugin System - Sample Plugin Included
* Fixed FileSystem Character Reloading
* Minor Changes to Import Dialog
* Added Type Field to Advanced Tabs

######0.9.5.1b
* Added 'Reload All Characters' Command
* Included System.Xml and System.Xml.Linq as valid references to the built-in compiler
* Changed PluginBase class and included PluginAction, to enable Single Level Depth Menus
  Upgrading from Previous Version: Extend your Plugins to PluginAction instead, and create a new PluginBase
  that holds the PluginActions, see SamplePlugin.cs for more information

######0.9.5.2b
* Fixed Color Picker and Card Generator Issues
* Fixed 'Bottom Color 1' in place of 'Bottom Color 2' at Uniform Views
* Fixed Clothing Texture preview Shadow Color

######0.9.5.3b
* Implemented Multi-Threaded Loading Routine (See Below)
* Fixed Hanging on Large Directories (1000+ Characters)
* Fixed FS Provider Tools lists not adding/removing entries to export
* Fixed some Null Reference Exceptions when auto (re)loading characters