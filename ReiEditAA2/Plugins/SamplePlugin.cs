// --------------------------------------------------
// ReiEditAA2 - SampleActionA.cs
// --------------------------------------------------


using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using ReiFX;
using AA2Lib;
using ReiEditAA2.Plugins;



// You may change the Namespace for something else to avoid Conflicting Names
namespace ReiEditAA2.Plugins.Samples
{

    /*
     * Plugins Must Extend the ReiEditAA2.PluginBase class
     * and override the Name Property and provide a collection of PluginActions
     * PluginActions shall override the Name property and the Modify method
     * 
     * Dynamically Compiled Plugins are compiled against the Following References
     *
     * ReiEditAA2
     * AA2Lib
     * ReiFX
     * 
     * System
     * System.Xml
     * System.Xml.Linq (XDocument)
     * System.Core (LINQ)
     * 
     * If you need any other GAC References, you may compile your own Plugins and drop them in the bin directory
     * Use of Additional Third Party Libraries untested as of now
     * 
     * PLUGINS ARE NOT SANDBOXED, EXECUTE THIRD PARTY CODE AT YOUR OWN RISK
     * MALICIOUS CODE COULD POTENTIALLY DAMAGE OR STEAL INFORMATION FROM YOUR SYSTEM
     * AVOID PRECOMPILED SCRIPTS PROVIDED BY UNKNOWN PARTIES, AND ALWAYS CHECK THE SOURCE FOR POTENTIALLY DANGEROUS CODE
     * (System.IO for example, can acess your file system, providing the script the ability to read/write data)
     */

    public class SamplePlugins : PluginBase
    {

        private readonly PluginAction[] _acts;

        /*
         * Plugin Actions are the Second Level Items
         * That actually Perform the Modifications
         */

        public override PluginAction[] Actions
        {
            get { return _acts; }
        }

        /*
         * Constructor Sets the Actions into an Array
         */
        public SamplePlugins()
        {
            _acts = new PluginAction[] {
                new SampleActionA(), // Create Action A
                new SampleActionB(), // Create Action B
            };
        }

        /*
         * Top Level Menu Item Name
         */
        public override string Name
        {
            get { return "Samples"; }
        }

    }

    /* Action Sample A */
    internal class SampleActionA : PluginAction
    {

        /*
         * This is the Name that will Appear on the Plugins Menu
         */

        public override string Name
        {
            get { return "Sample Plugin A"; }
        }

        /*
         * Action Entry Point
        */

        public override void Modify()
        {
            /* 
             * You have Acces to the Character Data via the following methods
             * 
             * object Character.GetAttribute(string attr)
             * T Character.GetAttribute<T>(string attr)
             * object Character.GetPlayData(string attr)
             * T Character.GetPlayData<T>(string attr)
             * 
             * and
             * 
             * void Character.SetAttribute(string attr, object value)
             * void Character.SetPlayData(string attr, object value)
             * 
             * Play Data Still Has Limitations to adittion, its only possible to edit the present values
             * 
             * All values are Retrieved as objects, or cast to T in the Generic Overloads
             * Since Values are Retrieved as Objects, you must cast them to the proper types before using
             * You can See the Value Types on the Advanced Tab of the Main Window
             */
            string namea = Character.GetAttribute<string>("PROFILE_FAMILY_NAME");
            string nameb = Character.GetAttribute<string>("PROFILE_FIRST_NAME");
            Character.SetAttribute("PROFILE_FAMILY_NAME", ReverseStr(nameb));
            Character.SetAttribute("PROFILE_FIRST_NAME", ReverseStr(namea));

            /*
             * The Colors are NOT the System Color Types, they are part of the ReiFX Library
             * The ReiFX.Color Type has support for HSL, HSV and RGB Color Spaces
             * 
             * Color Color.FromRgb(byte r, byte g, byte b, [byte a = 255])
             * 
             * Hue = 0~360; Sat,Value/Lightness and Alpha = 0~1
             * 
             * Color Color.FromHsl(float h, float s, float l, [float a = 1.0]) 
             * Color Color.FromHsv(float h, float s, float v, [float a = 1.0]) 
             * Color Color.FromRgb(byte r, byte g, byte b, [byte a = 255])
             * 
             * as well as HSL/HSV retrieval via
             * 
             * void Color.ToHsv(out float hue, out float saturation, out float value)
             * void Color.ToHsl(out float hue, out float saturation, out float lightness)
             */
            Character.SetAttribute("BODY_HAIR_COLOR", Color.FromHsv(180, 0.5, 1));
            Character.SetAttribute("BODY_PUBIC_HAIR_COLOR", Color.FromHsl(60, 0.5, 0.5));
            Character.SetAttribute("BODY_EYEBROW_COLOR", Color.FromRgb(127, 64, 255));

            /*
             * A Log Method is also available, messages will be output to DynamicPlugin.txt
             */
            Log("Logging some Message");
            Log("Logging some Message, with {0}", "Arguments");
        }

        /*
         * Sample LINQ Method to reverse a string
         */

        private string ReverseStr(string @in)
        {
            return new String
                (@in.Reverse()
                    .ToArray());
        }

    }

    /*
     * Besides the Character Property
     * The PluginBase Class has access to the Characters Property
     * That holds all of the currently loaded Characters
     * 
     * Action Sample B 
     */
    internal class SampleActionB : PluginAction
    {

        public override string Name
        {
            get { return "Sample Plugin B (Invert ALL names)"; }
        }

        public override void Modify()
        {
            foreach (var c in Characters)
            {
                string namea = c.GetAttribute<string>("PROFILE_FAMILY_NAME");
                string nameb = c.GetAttribute<string>("PROFILE_FIRST_NAME");
                Log("{0} {1} Name has been Inverted", namea, namea);
                c.SetAttribute("PROFILE_FAMILY_NAME", ReverseStr(nameb));
                c.SetAttribute("PROFILE_FIRST_NAME", ReverseStr(namea));
            }
        }

        private string ReverseStr(string @in)
        {
            return new String
                (@in.Reverse()
                    .ToArray());
        }

    }

}