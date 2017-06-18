// --------------------------------------------------
// ReiEditAA2 - PluginLoader.cs
// --------------------------------------------------

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AA2Lib;
using Microsoft.CSharp;

namespace ReiEditAA2.Plugins
{
    internal class PluginLoader : MarshalByRefObject
    {
        private const string HEADER = "----------";

        private static readonly string LogPath = Path.Combine(Core.StartupPath, "DynamicPlugin.txt");

        public bool CompilePlugins(string sourceName)
        {
            CSharpCodeProvider cscp = new CSharpCodeProvider
            (new Dictionary<string, string>
            {
                {"CompilerVersion", "v4.0"}
            });

            string source = Path.GetFileName(sourceName);
            string sdir = Path.GetDirectoryName(sourceName);
            string sass = Path.GetFileNameWithoutExtension(sourceName);
            string outputAssembly = Path.Combine(sdir, "bin", sass + ".dll");

            Directory.CreateDirectory(Path.GetDirectoryName(outputAssembly));

            if (File.Exists(outputAssembly))
                File.Delete(outputAssembly);

            CompilerParameters cp = new CompilerParameters
            {
                OutputAssembly = outputAssembly,
                GenerateInMemory = false,
                GenerateExecutable = false,
                IncludeDebugInformation = false,
                TreatWarningsAsErrors = false,
            };

            cp.ReferencedAssemblies.Add("ReiEditAA2.exe");
            cp.ReferencedAssemblies.Add("AA2Lib.dll");
            cp.ReferencedAssemblies.Add("ReiFX.dll");
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("System.Xml.dll");
            cp.ReferencedAssemblies.Add("System.Xml.Linq.dll");
            cp.ReferencedAssemblies.Add("System.Core.dll");

            CompilerResults cr = cscp.CompileAssemblyFromFile(cp, sourceName);
            if (cr.Errors.HasErrors)
            {
                LogHeader(source + " - " + DateTime.Now);
                foreach (object error in cr.Errors)
                {
                    Log("COMPILE: {0}", error);
                }
                LogFooter();
                return false;
            }
            return true;
        }

        public PluginBase[] LoadPlugins(string assemblyName)
        {
            var assData = File.ReadAllBytes(assemblyName);
            Assembly pluginAssembly = AppDomain.CurrentDomain.Load(assData);
            //Assembly pluginAssembly = Assembly.LoadFrom(assemblyName);

            var tools = (from t in pluginAssembly.GetTypes()
                where !t.IsAbstract && t.IsClass
                where typeof(PluginBase).IsAssignableFrom(t)
                select Activator.CreateInstance(t)).ToArray();

            var arr = tools.Cast<PluginBase>()
                .ToArray();

            return arr;
        }

        public static void Log(string format, params object[] args)
        {
            string tgt = string.Format(format + Environment.NewLine, args);
            File.AppendAllText(LogPath, tgt);
        }

        public static void LogFooter()
        {
            File.AppendAllText(LogPath, string.Format("{0}{0}{0}{0}" + Environment.NewLine, HEADER));
        }

        public static void LogHeader(string text)
        {
            File.AppendAllText(LogPath, string.Format("{0} {1} {0}" + Environment.NewLine, HEADER, text));
        }
    }
}