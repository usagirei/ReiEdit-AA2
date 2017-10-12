// --------------------------------------------------
// AA2Lib - Core.cs
// --------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using AA2Lib.Code.ConfigurationManager;
using AA2Lib.Code.DataBlocks;
using Microsoft.Win32;

namespace AA2Lib
{
    public static partial class Core
    {
        //public static readonly Random Random = new Random(DateTime.Now.Millisecond);

        public static readonly Random Random = new Random((int) DateTime.Now.Ticks);
        //public static readonly CryptoRandom Random = new CryptoRandom();

        private static readonly ResourceDictionary SharedResourcesInternal = new SharedResources();
        public static string GetGamePath(string exe, string key)
        {
            string installDir = BrowseInstallDir(exe);
            if (installDir != null) ConfigurationManager.Instance.SetItem(key, installDir);
            return installDir;
        }
        public static string GetEditPath()
        {
            return GetGamePath("AA2Edit.exe", "EDIT_INSTALL");
        }
        public static string GetPlayPath()
        {
            return GetGamePath("AA2Play.exe", "PLAY_INSTALL");
        }

        public static string EditInstallDir
        {
            get
            {
                bool hasKey = ConfigurationManager.Instance.Configs.ContainsKey("EDIT_INSTALL");
                if (hasKey)
                {
                    string path = ConfigurationManager.Instance.GetItem("EDIT_INSTALL") as string;
                    if (path != null && Directory.Exists(path))
                        return path;
                }
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Software\\illusion\\AA2Edit");
                if (regKey == null)
                {
                    return CheckInstallDir(GetEditPath());
                }
                string registryDir = (string) regKey.GetValue("INSTALLDIR");
                regKey.Close();
                ConfigurationManager.Instance.SetItem("EDIT_INSTALL", registryDir);
                return registryDir;
            }
        }

        public static string EditSaveDir
        {
            get
            {
                string str = EditInstallDir;
                return str == null
                    ? null
                    : Path.Combine(str, "data", "save");
            }
        }

        public static string EditTextureDir
        {
            get
            {
                string str = EditInstallDir;
                return str == null
                    ? null
                    : Path.Combine(str, "data", "texture");
            }
        }

        public static string PlayInstallDir
        {
            get
            {
                bool hasKey = ConfigurationManager.Instance.Configs.ContainsKey("PLAY_INSTALL");
                if (hasKey)
                {
                    string path = ConfigurationManager.Instance.GetItem("PLAY_INSTALL") as string;
                    if (path != null && Directory.Exists(path))
                        return path;
                }
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Software\\illusion\\AA2Play");
                if (regKey == null)
                {
                    return CheckInstallDir(GetPlayPath());
                }
                string registryDir = (string) regKey.GetValue("INSTALLDIR");
                regKey.Close();
                ConfigurationManager.Instance.SetItem("PLAY_INSTALL", registryDir);
                return registryDir;
            }
        }

        public static string PlaySaveDir
        {
            get
            {
                string str = PlayInstallDir;
                return str == null
                    ? null
                    : Path.Combine(str, "data", "save");
            }
        }

        public static string PlayTextureDir
        {
            get
            {
                string str = PlayInstallDir;
                return str == null
                    ? null
                    : Path.Combine(str, "data", "texture");
            }
        }

        public static ResourceDictionary SharedResources
        {
            get { return SharedResourcesInternal; }
        }

        public static string StartupPath
        {
            get { return Path.GetDirectoryName((Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).Location); }
        }

        public static XDocument LoadCharDataXDocument()
        {
            Stream streamSchema = GetDataBlockSchemaStream();

            string charDataPath = Path.Combine(StartupPath, "XML", "DataBlocks", "chardata.xml");
            if (!File.Exists(charDataPath) || streamSchema == null)
                return null;

            Stream streamXml = File.OpenRead(charDataPath);
            XDocument xDocument = ValidateXDocument(streamXml, streamSchema);
            streamXml.Dispose();

            if (xDocument == null)
                return null;

            string overridePath = Path.Combine(StartupPath, "XML", "Overrides");
            if (!Directory.Exists(overridePath))
                return xDocument;

            foreach (string file in Directory.EnumerateFiles(overridePath, "*.xml"))
            {
                streamSchema.Seek(0, SeekOrigin.Begin);
                Stream s = File.OpenRead(file);
                XDocument over = ValidateXDocument(s, streamSchema);
                s.Dispose();
                ProcessOverrides(xDocument, over);
            }

            streamSchema.Close();

            return xDocument;
        }

        public static XDocument LoadClothDataXDocument()
        {
            Stream streamSchema = GetDataBlockSchemaStream();

            string charDataPath = Path.Combine(StartupPath, "XML", "DataBlocks", "clothdata.xml");
            if (!File.Exists(charDataPath) || streamSchema == null)
                return null;

            Stream streamXml = File.OpenRead(charDataPath);
            XDocument xDocument = ValidateXDocument(streamXml, streamSchema);
            streamXml.Dispose();

            streamSchema.Close();

            return xDocument;
        }

        public static IList<IDataBlock> LoadDataBlocks(
            byte[] scanData,
            int scanOffset,
            XDocument scanDocument,
            out int dataSize)
        {
            var blocks = new List<IDataBlock>();
            XElement root = scanDocument.Element("dataset");

            XElement structRoot = root.Element("structs");
            XElement enumerableRoot = root.Element("enumerables");
            XElement datablockRoot = root.Element("datablocks");

            var structs = structRoot == null
                ? new List<XElement>()
                : structRoot.Elements("struct")
                    .ToList();

            var enumerables = enumerableRoot == null
                ? new List<XElement>()
                : enumerableRoot.Elements("enumerable")
                    .ToList();

            var datablocks = datablockRoot == null
                ? new List<XElement>()
                : datablockRoot.Elements("datablock")
                    .ToList();

            int autoAddr = 0;
            foreach (XElement datablock in datablocks)
            {
                string type = datablock.Attribute("type")
                    .Value;
                IDataBlock block = null;
                switch (type)
                {
                    case "dummy":
                        block = CreateDummyBlock(datablock, ref autoAddr);
                        break;
                    case "bool":
                    case "byte":
                    case "int16":
                    case "int32":
                    case "color":
                    case "string":
                    case "encodedstring":
                        block = CreateValueDataBlock(type, datablock, ref autoAddr);
                        break;
                    case "enum":
                    case "seat":
                        block = CreateEnumDataBlock(enumerables, datablock, ref autoAddr);
                        break;
                    case "array":
                        var arrayBlocks = CreateArrayDataBlock(scanData, scanOffset, enumerables, structs, datablock, ref autoAddr);
                        blocks.AddRange(arrayBlocks);
                        break;
                    case "struct":
                        var structBlocks = CreateStructDataBlock(scanData, scanOffset, enumerables, structs, datablock, ref autoAddr);
                        blocks.AddRange(structBlocks);
                        break;
                }
                if (block != null)
                    blocks.Add(block);
            }

            //byteDataBlock = new byte[autoAddr];
            //Buffer.BlockCopy(scanData, scanOffset, byteDataBlock, 0, byteDataBlock.Length);
            dataSize = autoAddr;
            return blocks;
        }

        public static XDocument LoadHeaderDataXDocument()
        {
            Stream streamSchema = GetDataBlockSchemaStream();

            string charDataPath = Path.Combine(StartupPath, "XML", "DataBlocks", "headerdata.xml");
            if (!File.Exists(charDataPath) || streamSchema == null)
                return null;

            Stream streamXml = File.OpenRead(charDataPath);
            XDocument xDocument = ValidateXDocument(streamXml, streamSchema);
            streamXml.Dispose();

            streamSchema.Close();

            return xDocument;
        }

        public static XDocument LoadPlayDataXDocument()
        {
            Stream streamSchema = GetDataBlockSchemaStream();

            string charDataPath = Path.Combine(StartupPath, "XML", "DataBlocks", "playdata.xml");
            if (!File.Exists(charDataPath) || streamSchema == null)
                return null;

            Stream streamXml = File.OpenRead(charDataPath);
            XDocument xDocument = ValidateXDocument(streamXml, streamSchema);
            streamXml.Dispose();

            streamSchema.Close();

            return xDocument;
        }

        internal static IDataBlock CreateEnumDataBlock(
            IEnumerable<XElement> enumerables,
            XElement datablock,
            ref int autoAddr)
        {
            XAttribute typeNode = datablock.Attribute("type");
            XAttribute addressNode = datablock.Attribute("address");
            XAttribute keyNode = datablock.Attribute("key");
            XAttribute nameNode = datablock.Attribute("displayname");

            XAttribute copyaddressNode = datablock.Attribute("copyaddress");
            XAttribute readonlyNode = datablock.Attribute("readonly");

            XAttribute metaKeyNode = datablock.Attribute("metakey");

            if (!addressNode.Value.Equals("auto", StringComparison.OrdinalIgnoreCase))
                autoAddr = addressNode.Value.ToInt32();

            bool ignore = keyNode == null;

            string type = typeNode == null
                ? "enum"
                : typeNode.Value;

            string key = keyNode == null
                ? String.Empty
                : keyNode.Value;

            string metaKey = type == "enum"
                ? metaKeyNode.Value
                : null;

            string name = nameNode == null
                ? key
                : nameNode.Value;

            var copyAddresses = copyaddressNode == null
                ? new int[0]
                : copyaddressNode.Value.Split(';')
                    .Select(Extensions.ToInt32)
                    .ToArray();
            bool readOnly = bool.Parse(readonlyNode.Value);

            XElement enumTypeNode = enumerables.FirstOrDefault
            (enumerable => enumerable.Attribute("key")
                .Value.Equals(metaKey, StringComparison.OrdinalIgnoreCase));
            string enumType;
            switch (type)
            {
                case "seat":
                    enumType = "int32";
                    break;
                default:
                    enumType = enumTypeNode.Attribute("type")
                        .Value;
                    break;
            }

            int len;
            switch (enumType)
            {
                default:
                case "byte":
                    len = 1;
                    break;
                case "int16":
                    len = 2;
                    break;
                case "int32":
                    len = 4;
                    break;
            }

            var dictionary = new Dictionary<int, string>();
            if (type == "enum")
            {
                foreach (XElement element in enumTypeNode.Elements("enumpair"))
                {
                    string pairKey = element.Attribute("key")
                        .Value;
                    string pairValue = element.Attribute("value")
                        .Value;
                    dictionary.Add(pairKey.ToInt32(), pairValue);
                }
            }

            IDataBlock block;
            switch (type)
            {
                case "seat":
                    block = new SeatDataBlock
                    {
                        Key = key,
                        Name = name,
                        Address = autoAddr,
                        CopyAddresses = copyAddresses,
                        ReadOnly = readOnly,
                    };
                    break;
                default:
                    block = new EnumDataBlock
                    {
                        Key = key,
                        Name = name,
                        Address = autoAddr,
                        EnumName = metaKey,
                        CopyAddresses = copyAddresses,
                        Size = len,
                        Enum = dictionary,
                        ReadOnly = readOnly,
                    };
                    break;
            }

            autoAddr += block.Size;

            return !ignore
                ? block
                : null;
        }

        internal static IDataBlock CreateValueDataBlock(string type, XElement datablock, ref int autoAddr)
        {
            XAttribute addressNode = datablock.Attribute("address");
            XAttribute keyNode = datablock.Attribute("key");
            XAttribute nameNode = datablock.Attribute("displayname");

            XAttribute copyaddressNode = datablock.Attribute("copyaddress");
            XAttribute dataLenghtNode = datablock.Attribute("datasize");
            XAttribute readonlyNode = datablock.Attribute("readonly");

            XAttribute minvalNode = datablock.Attribute("minvalue");
            XAttribute maxvalNode = datablock.Attribute("maxvalue");
            XAttribute warnvalNode = datablock.Attribute("warnvalue");

            if (!addressNode.Value.Equals("auto", StringComparison.OrdinalIgnoreCase))
                autoAddr = addressNode.Value.ToInt32();

            bool ignore = keyNode == null;

            string key = keyNode == null
                ? String.Empty
                : keyNode.Value;

            string name = nameNode == null
                ? key
                : nameNode.Value;

            var copyAddresses = copyaddressNode == null
                ? new int[0]
                : copyaddressNode.Value.Split(';')
                    .Select(Extensions.ToInt32)
                    .ToArray();

            bool readOnly = bool.Parse(readonlyNode.Value);

            bool isRanged = minvalNode != null || maxvalNode != null;

            IDataBlock block = null;
            switch (type)
            {
                case "bool":

                    block = new BoolDataBlock
                    {
                        Key = key,
                        Address = autoAddr,
                        Name = name,
                        CopyAddresses = copyAddresses,
                        ReadOnly = readOnly
                    };
                    break;
                case "byte":
                    block = new ByteDataBlock
                    {
                        Key = key,
                        Address = autoAddr,
                        Name = name,
                        CopyAddresses = copyAddresses,
                        ReadOnly = readOnly,
                        IsRanged = isRanged,
                        MinValue = minvalNode == null
                            ? Byte.MinValue
                            : minvalNode.Value.ToByte(),
                        MaxValue = maxvalNode == null
                            ? Byte.MaxValue
                            : maxvalNode.Value.ToByte(),
                        WarnValue = warnvalNode == null
                            ? Byte.MaxValue
                            : warnvalNode.Value.ToByte(),
                    };
                    break;
                case "int16":
                    block = new Int16DataBlock
                    {
                        Key = key,
                        Address = autoAddr,
                        Name = name,
                        CopyAddresses = copyAddresses,
                        ReadOnly = readOnly,
                        IsRanged = isRanged,
                        MinValue = minvalNode == null
                            ? Int16.MinValue
                            : minvalNode.Value.ToInt16(),
                        MaxValue = maxvalNode == null
                            ? Int16.MaxValue
                            : maxvalNode.Value.ToInt16(),
                        WarnValue = warnvalNode == null
                            ? Int16.MaxValue
                            : warnvalNode.Value.ToInt16(),
                    };
                    break;
                case "int32":
                    block = new Int32DataBlock
                    {
                        Key = key,
                        Address = autoAddr,
                        Name = name,
                        CopyAddresses = copyAddresses,
                        ReadOnly = readOnly,
                        IsRanged = isRanged,
                        MinValue = minvalNode == null
                            ? Int32.MinValue
                            : minvalNode.Value.ToInt32(),
                        MaxValue = maxvalNode == null
                            ? Int32.MaxValue
                            : maxvalNode.Value.ToInt32(),
                        WarnValue = warnvalNode == null
                            ? Int32.MaxValue
                            : warnvalNode.Value.ToInt32(),
                    };
                    break;
                case "color":
                    block = new ColorDataBlock
                    {
                        Key = key,
                        Address = autoAddr,
                        Name = name,
                        CopyAddresses = copyAddresses,
                        ReadOnly = readOnly,
                    };
                    break;
                case "string":
                    block = new StringDataBlock
                    {
                        Key = key,
                        Address = autoAddr,
                        Name = name,
                        CopyAddresses = copyAddresses,
                        ReadOnly = readOnly,
                        Size = dataLenghtNode.Value.ToInt32()
                    };
                    break;
                case "encodedstring":
                    block = new EncodedStringDataBlock
                    {
                        Key = key,
                        Address = autoAddr,
                        Name = name,
                        CopyAddresses = copyAddresses,
                        ReadOnly = readOnly,
                        Size = dataLenghtNode.Value.ToInt32()
                    };
                    break;
            }
            if (block != null)
                autoAddr += block.Size;
            return !ignore
                ? block
                : null;
        }

        private static string BrowseInstallDir(string fileName)
        {
            OpenFileDialog opfl = new OpenFileDialog
            {
                Title = string.Format("Select File Path: {0}", fileName),
                Filter = string.Format("{0}|{0}", fileName)
            };
            if (opfl.ShowDialog()
                .Value)
            {
                string dir = Path.GetDirectoryName(opfl.FileName);
                return dir;
            }
            return null;
        }

        private static string CheckInstallDir(string path)
        {
            if (path != null) return path;
            MessageBox.Show("ReiEditAA2 Requires Artificial Academy 2 and Editor Installed in order to function.",
                "ReiEditAA2 - Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error,
                MessageBoxResult.OK);
            Environment.Exit(0);
            return null;
        }

        private static IEnumerable<IDataBlock> CreateArrayDataBlock(
            byte[] scanData,
            int scanOffset,
            List<XElement> enumerables,
            List<XElement> structs,
            XElement datablock,
            ref int autoAddr)
        {
            XAttribute keyNode = datablock.Attribute("key");
            XAttribute addressNode = datablock.Attribute("address");
            XAttribute nameNode = datablock.Attribute("displayname");

            XAttribute copyaddressNode = datablock.Attribute("copyaddress");
            XAttribute readOnlyNode = datablock.Attribute("readonly");
            XAttribute dataLenghtNode = datablock.Attribute("datasize");

            XAttribute minvalNode = datablock.Attribute("minvalue");
            XAttribute maxvalNode = datablock.Attribute("maxvalue");
            XAttribute warnvalNode = datablock.Attribute("warnvalue");

            XAttribute innerTypeNode = datablock.Attribute("innertype");
            XAttribute headTypeNode = datablock.Attribute("headtype");
            XAttribute arraySizeNode = datablock.Attribute("arraysize");

            XAttribute metaKeyNode = datablock.Attribute("metakey");

            if (!addressNode.Value.Equals("auto", StringComparison.OrdinalIgnoreCase))
                autoAddr = addressNode.Value.ToInt32();

            bool ignore = keyNode == null;

            string key = keyNode == null
                ? String.Empty
                : keyNode.Value;

            if (innerTypeNode == null)
                throw new Exception(string.Format("Inner Type not set for Array: {0}", key));

            string innerType = innerTypeNode.Value;

            string name = nameNode == null
                ? key
                : nameNode.Value;

            var copyAddresses = copyaddressNode == null
                ? new int[0]
                : copyaddressNode.Value.Split(';')
                    .Select(Extensions.ToInt32)
                    .ToArray();

            IDataBlock baseBlock;
            string arraySizeStr = arraySizeNode.Value;
            string headKey = key;
            if (arraySizeStr.Equals("auto", StringComparison.OrdinalIgnoreCase))
            {
                string headType = headTypeNode.Value;
                int dataLenght;
                switch (headType)
                {
                    default:
                    case "byte":
                        dataLenght = 1;
                        break;
                    case "int16":
                        dataLenght = 2;
                        break;
                    case "int32":
                        dataLenght = 4;
                        break;
                }

                switch (dataLenght)
                {
                    default:
                    case 1:
                        baseBlock = new ByteDataBlock
                        {
                            Address = autoAddr,
                            CopyAddresses = copyAddresses,
                            Key = headKey,
                            Name = name,
                            ReadOnly = true
                        };
                        break;
                    case 2:
                        baseBlock = new Int16DataBlock
                        {
                            Address = autoAddr,
                            CopyAddresses = copyAddresses,
                            Key = headKey,
                            Name = name,
                            ReadOnly = true
                        };
                        break;
                    case 4:
                        baseBlock = new Int32DataBlock
                        {
                            Address = autoAddr,
                            CopyAddresses = copyAddresses,
                            Key = headKey,
                            Name = name,
                            ReadOnly = true
                        };
                        break;
                }
            }
            else
            {
                baseBlock = new DummyDataBlock
                {
                    Address = autoAddr,
                    CopyAddresses = new int[0],
                    Key = headKey,
                    Name = name,
                    ReadOnly = true,
                    DummyValue = arraySizeStr.ToInt32()
                };
            }

            var outBlocks = new List<IDataBlock>();
            autoAddr += baseBlock.Size;
            outBlocks.Add(baseBlock);

            int elCount = Convert.ToInt32(baseBlock.Read(scanData, scanOffset));

            for (int i = 0; i < elCount; i++)
            {
                IDataBlock block = null;
                XElement innerBlock = new XElement("datablock",
                    new XAttribute("type", innerType),
                    //
                    new XAttribute("key", string.Format("{0}[{1}]", key, i)),
                    new XAttribute("address", "auto"),
                    new XAttribute("displayname",
                        string.Format("{0} Element {1}", name, i)
                            .Trim()),
                    //
                    readOnlyNode,
                    metaKeyNode,
                    dataLenghtNode,
                    //
                    minvalNode,
                    maxvalNode,
                    warnvalNode);

                switch (innerType)
                {
                    case "dummy":
                        block = CreateDummyBlock(innerBlock, ref autoAddr);
                        break;
                    case "bool":
                    case "byte":
                    case "int16":
                    case "int32":
                    case "color":
                    case "string":
                    case "encodedstring":
                        block = CreateValueDataBlock(innerType, innerBlock, ref autoAddr);
                        break;
                    case "enum":
                        block = CreateEnumDataBlock(enumerables, innerBlock, ref autoAddr);
                        break;
                    case "array":
                        throw new Exception("Arrays of Arrays are not supported, use a struct datatype");
                        break;
                    case "struct":
                        var structBlocks = CreateStructDataBlock(scanData, scanOffset, enumerables, structs, innerBlock, ref autoAddr);
                        outBlocks.AddRange(structBlocks);
                        break;
                }
                if (block != null)
                    outBlocks.Add(block);
            }

            return !ignore
                ? outBlocks
                : Enumerable.Empty<IDataBlock>();
        }

        private static IDataBlock CreateDummyBlock(XElement datablock, ref int autoAddr)
        {
            XAttribute addressNode = datablock.Attribute("address");
            XAttribute keyNode = datablock.Attribute("key");
            XAttribute nameNode = datablock.Attribute("displayname");

            XAttribute dataLenghtNode = datablock.Attribute("datasize");

            if (!addressNode.Value.Equals("auto", StringComparison.OrdinalIgnoreCase))
                autoAddr = addressNode.Value.ToInt32();

            bool ignore = keyNode == null;

            string key = keyNode == null
                ? String.Empty
                : keyNode.Value;

            string name = nameNode == null
                ? key
                : nameNode.Value;

            IDataBlock block = new DummyDataBlock
            {
                Address = autoAddr,
                Key = key,
                Name = name,
                Size = dataLenghtNode.Value.ToInt32(),
            };
            autoAddr += block.Size;
            return !ignore
                ? block
                : null;
        }

        private static IEnumerable<IDataBlock> CreateStructDataBlock(
            byte[] scanData,
            int scanOffset,
            List<XElement> enumerables,
            List<XElement> structs,
            XElement datablock,
            ref int autoAddr)
        {
            XAttribute keyNode = datablock.Attribute("key");
            XAttribute addressNode = datablock.Attribute("address");

            XAttribute metaKeyNode = datablock.Attribute("metakey");

            if (!addressNode.Value.Equals("auto", StringComparison.OrdinalIgnoreCase))
                autoAddr = addressNode.Value.ToInt32();

            if (metaKeyNode == null || String.IsNullOrEmpty(metaKeyNode.Value))
                return Enumerable.Empty<IDataBlock>();

            bool ignore = keyNode == null;

            string key = keyNode == null
                ? String.Empty
                : keyNode.Value;

            string metaKey = metaKeyNode.Value;

            XElement targetStruct = structs.FirstOrDefault
            (element => element.Attribute("key")
                .Value.Equals(metaKey, StringComparison.OrdinalIgnoreCase));

            var blocks = new List<IDataBlock>();

            blocks.Add
            (new DummyDataBlock
            {
                Address = autoAddr,
                Key = key,
                DummyValue = String.Empty
            });

            foreach (XElement innerBlock in targetStruct.Elements())
            {
                string type = innerBlock.Attribute("type")
                    .Value;
                IDataBlock block = null;
                switch (type)
                {
                    case "dummy":
                        block = CreateDummyBlock(innerBlock, ref autoAddr);
                        break;
                    case "bool":
                    case "byte":
                    case "int16":
                    case "int32":
                    case "color":
                    case "string":
                    case "encodedstring":
                        block = CreateValueDataBlock(type, innerBlock, ref autoAddr);
                        break;
                    case "enum":
                    case "seat":
                        block = CreateEnumDataBlock(enumerables, innerBlock, ref autoAddr);
                        break;
                    case "array":
                        var arrayBlocks = CreateArrayDataBlock(scanData, scanOffset, enumerables, structs, innerBlock, ref autoAddr);
                        foreach (IDataBlock arrayBlock in arrayBlocks)
                        {
                            arrayBlock.Key = key + "." + arrayBlock.Key;
                            blocks.Add(arrayBlock);
                        }
                        break;
                    case "struct":
                        var structBlocks = CreateStructDataBlock(scanData, scanOffset, enumerables, structs, innerBlock, ref autoAddr);
                        foreach (IDataBlock structBlock in structBlocks)
                        {
                            structBlock.Key = key + "." + structBlock.Key;
                            blocks.Add(structBlock);
                        }
                        break;
                }
                if (block != null)
                {
                    block.Key = key + "." + block.Key;
                    blocks.Add(block);
                }
            }

            return !ignore
                ? blocks
                : Enumerable.Empty<IDataBlock>();
        }

        private static Stream GetDataBlockSchemaStream()
        {
            return Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(@"AA2Lib.XML.datablocks.xsd");
        }

        private static void ProcessOverrides(XDocument target, XDocument overrides)
        {
            XElement targetDataSet = target.Element("dataset");
            XElement overrideDataSet = overrides.Element("dataset");

            XElement targetEnumerables = targetDataSet.Element("enumerables");
            XElement overrideEnumerables = overrideDataSet.Element("enumerables");

            //TODO: Add Overrides for other stuff
            if (overrideEnumerables == null)
                return;

            foreach (XElement enumerable in overrideEnumerables.Elements("enumerable"))
            {
                string enumKey = enumerable.Attribute("key")
                    .Value;
                XElement targetNode = targetEnumerables.Elements("enumerable")
                    .FirstOrDefault
                    (element => element.Attribute("key")
                        .Value.Equals(enumKey));
                if (targetNode == null)
                    continue;

                foreach (XElement pair in enumerable.Elements("enumpair"))
                {
                    string pairKey = pair.Attribute("key")
                        .Value;
                    XElement targetPair = targetNode.Elements("enumpair")
                        .FirstOrDefault
                        (element => element.Attribute("key")
                            .Value.Equals(pairKey));
                    if (targetPair == null)
                        targetNode.Add(pair);
                    else
                        targetPair.ReplaceWith(pair);
                }

                var ordered = targetNode.Elements("enumpair")
                    .OrderBy
                    (element => int.Parse
                    (element.Attribute("key")
                        .Value))
                    .ToArray();

                targetNode.RemoveNodes();
                targetNode.Add(ordered.Cast<object>());
            }
        }

        private static XDocument ValidateXDocument(Stream xmlStream, Stream schemaStream)
        {
            StreamReader srXml = new StreamReader(xmlStream, Encoding.GetEncoding("utf-8"));
            StreamReader srSchema = new StreamReader(schemaStream, Encoding.GetEncoding("utf-8"));
            XDocument xDocument = XDocument.Load(srXml);
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add("", XmlReader.Create(srSchema));
            bool errors = false;
            xDocument.Validate(schemaSet,
                (o, e) =>
                {
                    Debug.Print("{0}\r\n{1}", e.Message, e.Exception.ToString());
                    errors = true;
                },
                true);
            return errors
                ? null
                : xDocument;
        }
    }
}