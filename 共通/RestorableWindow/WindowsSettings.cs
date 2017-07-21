using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace 共通
{
    /// <summary>
    /// Windowsの位置・サイズの保存と復元
    /// </summary>
    /// TEXT(INI,XML)、バイナリ、レジストリに保存するのか、保存先などをどうするかを決めて実装すること。
    /// ユーザー単位で保存するのか、アプリケーション単位で保存するのかの検討も必要。
    public interface IWindowSettings
    {
        WINDOWPLACEMENT? Placement { get; set; }
        void Reload();
        void Save();
    }

    /// <summary>
    /// ApplicationSettingsBaseを使用した簡易版
    /// </summary>
    public class WindowSettings2 : ApplicationSettingsBase, IWindowSettings
    {
        public WindowSettings2(Window window) : base(window.GetType().FullName) { }

        [UserScopedSetting]
        public WINDOWPLACEMENT? Placement
        {
            get { return this["Placement"] != null ? (WINDOWPLACEMENT?)(WINDOWPLACEMENT)this["Placement"] : null; }
            set { this["Placement"] = value; }
        }
    }

    /// <summary>
    /// 保存先及び内容のカスタマイズ版
    /// </summary>
    /// mscorlib.dllで例外が発生するのは、XmlSerializerを使用しているため。
    /// 回避するためにはsgen.exeで「指定されたアセンブリの型に対して XML シリアル化アセンブリを作成」する。
    public class WindowSettings : IWindowSettings
    {
        string name = null;
        Configuration config;
        ExeConfigurationFileMap configFileMap;

        public WindowSettings(Window window)  {
            name = window.GetType().FullName;
            // Get the current configuration file.
            System.Configuration.Configuration config2 =
                    ConfigurationManager.OpenExeConfiguration(
                    ConfigurationUserLevel.None);

            configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = "d:/test2";
        }

        [ConfigurationProperty("Placement")]
        [UserScopedSetting]
        public WINDOWPLACEMENT? Placement
        {
            //get { return this["Placement"] != null ? (WINDOWPLACEMENT?)(WINDOWPLACEMENT)this["Placement"] : null; }
            //set { this["Placement"] = value; }
            get;
            set;
        }



        public void Reload()
        {
            config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
            CustomSection customSection = new CustomSection();
       

            if (config.Sections[name] == null)
            {
            }
            else
            {
                customSection = (CustomSection)config.Sections[name];
                Placement = customSection.XMLtoWINDOWPLACEMENT();
            }
        }

        public void Save()
        {
            config =  ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

            CustomSection customSection;

            if (config.Sections[name] == null)
            {
                // セクションが無い場合は作成し、追加する。
                customSection = new CustomSection();
                customSection.WINDOWPLACEMENTtoXML(Placement);
                config.Sections.Add(name, customSection);
            }
            else
            {
                // セクションが存在する場合は読み込む。(追加するとエラー)
                customSection = (CustomSection)config.Sections[name];
                customSection.WINDOWPLACEMENTtoXML(Placement);
            }
            customSection.SectionInformation.ForceSave = true;
            if (!customSection.ElementInformation.IsLocked)
            {
                config.Save(System.Configuration.ConfigurationSaveMode.Full);
            }
        }
    }

    public sealed class CustomSection : System.Configuration.ConfigurationSection

    {
        XmlSerializer serializer = new XmlSerializer(typeof(WINDOWPLACEMENT));
        [ConfigurationProperty("Name")]
        public string Name
        {
            get
            {
                return (string)this["Name"];
            }
            set
            {
                this["Name"] = value;
            }
        }

        public void WINDOWPLACEMENTtoXML(WINDOWPLACEMENT? placement)
        {
            // 書き込む書式の設定
            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = false;

            // ファイルへオブジェクトを書き込み（シリアライズ）
            StringBuilder buff = new StringBuilder();
            using (var writer = XmlWriter.Create(buff, settings))
            {
                serializer.Serialize(writer,placement);
            }

            Name = buff.ToString();
        }

        public WINDOWPLACEMENT? XMLtoWINDOWPLACEMENT()
        {
            Stream stream = new MemoryStream(Encoding.Unicode.GetBytes(Name));
            return (WINDOWPLACEMENT)serializer.Deserialize(stream);
        }
    }
}

