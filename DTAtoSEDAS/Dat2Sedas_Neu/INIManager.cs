using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INIManager
{
    /// <summary>
    /// Klasse zum Schreiben und Lesen von Daten aus INI-Dateien
    /// Inkl. Sektion, Parameter und Wert.    ///
    /// Die Ini-Datei wird in ein Objekt eingelesen. Über dieses Objekt kann auf die Parameter und Werte zugegriffen werden. 
    /// Wird ein Parameterwert geändert (update), oder eine Section oder Parameter hinzugefügt oder gelöscht, 
    /// wird die Ini-Datei aktualisiert.
    /// </summary>
    public class IniManager
    {
        //TODO ExceptionHandler-Ereignis einbauen (Delegate)

        #region PROPERTIES
        private string _INIPath = System.IO.Directory.GetCurrentDirectory() + "\\Config.ini";
        private List<Section> _sectionList = new List<Section>();
        private List<string> _iniTextLines = new List<string>();
        private string iniHeaderText = "";

        private string INIPath
        {
            get { return _INIPath; }
            set
            {
                _INIPath = checkIniPath(value);
            }
        }

        private string GetIniHeaderText
        {
            get
            {
                return "-----------------------\n" +
                       "DATtoSEDAS Config-Datei\n" +
                       "-----------------------\n" +
                       "Quell- und Zielpfad müssen mit Laufwerksbuchstabe angegeben werden(vollständig), jedoch ohne Dateiname.\n" +
                       "Der Dateiname der Quell- und Zieldatei wird separat eingetragen.\n" +
                       "Werden Quell- und Zieldateiname beim Programmstart per Schalter übergeben(/Q=, /Z=), werden die Einträge\n" +
                       "in der Config.ini übergangen.\n" +
                       "Dies gilt auch für alle weiteren Schalter (z.B.QuelleLöschen, /D)     ";
            }
        }
        #endregion


        #region KONSTRUKTOR    
        public IniManager()
        { }


        public IniManager(string Path) : base()
        {

            if (checkIniPath(Path) != "")
            {
                INIPath = Path;
                if (readIniFile(_INIPath)) importIniContent(_iniTextLines);
            }
        }
        #endregion


        #region PRIVATE METHODS

        private string checkIniPath(string path)
        {
            if (!File.Exists(path))
            {
                //TODO LogMessage/Ereignis erzeugen: throw new IOException($"Ini-Datei {path} kann nicht geladen werden");
                return "";
            }
            return path;
        }


        /// <summary>
        /// Liest den Inhalt der INI-Datei in ein List<string> Objekt ein.
        /// </summary>
        /// <param name="path">Pfad zur INI-Datei.</param>
        /// <returns></returns>
        private bool readIniFile(string path)
        {
            _iniTextLines.Clear();
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    while (!sr.EndOfStream)
                    {
                        _iniTextLines.Add(sr.ReadLine());
                    }
                    sr.Close();
                }

                if (_iniTextLines.Count() > 0) { return true; }
            }
            catch (Exception ex)
            {
                throw new IOException($"Fehler beim Lesen der ini-Datei {path}.");
            }
            return false;
        }


        /// <summary>
        /// Liest den Inhalt aus der List<string> iniContent in eine List<Section> ein.
        /// </summary>
        /// <param name="Content">List<string> mit Zeilen der INI-Datei.</string></param>
        private void importIniContent(List<string> Content)
        {
            List<string> sectionBlock = new List<string>();

            for (int i = 0; i < _iniTextLines.Count(); i++)
            {
                if (_iniTextLines[i].Trim().Contains("["))
                {
                    //Section gefunden                    
                    string sectionName = getSectionNameFromLine(Content[i].Trim());
                    AddNewSection(sectionName);

                    int j = i + 1;
                    while (j < _iniTextLines.Count())
                    {
                        if (_iniTextLines[j].Contains("[")) break;

                        if (_iniTextLines[j].Contains("="))
                        {
                            string[] parameterLine = _iniTextLines[j].Split('=');
                            string parameterName = parameterLine[0].Trim();
                            string ParameterValue = parameterLine[1].Trim();
                            AddNewParameter(sectionName, parameterName, ParameterValue);
                        }
                        j++;
                    }
                    i = j - 1;
                }
            }
        }


        private string getSectionNameFromLine(string sectionHeader)
        {
            string sectionName = sectionHeader.Trim().ToUpper();

            if (sectionName.Substring(0, 1) == "[") sectionName = sectionName.Remove(0, 1);
            if (sectionName.Substring(sectionName.Length - 1, 1) == "]") sectionName = sectionName.Remove(sectionName.Length - 1);
            return sectionName;
        }

        #endregion


        #region PUBLIC METHODS
        public void CreateNewIniFile(string path)
        {
            try
            {
                File.CreateText(path);
                INIPath = path;
            }
            catch
            {
                throw new IOException($"Erstellen der neuen Ini-Datei {path} fehlgeschlagen.");
            }
        }


        //TODO Erweitern und löschen von Einträgen und Sektionen überarbeiten!!!
        public void AddNewSection(string SectionName)
        {
            if (!_sectionList.Exists(sec => sec.sectionName.Contains(SectionName.ToUpper())))
            {
                _sectionList.Add(new Section(SectionName));
                writeToFile();
            }
        }


        public void AddNewParameter(string SectionName, string ParameterName)
        {
            AddNewParameter(SectionName, ParameterName, "");
        }

        public void AddNewParameter(string SectionName, string ParameterName, string ParameterValue)
        {
            if (_sectionList.Exists(sec => sec.sectionName.Contains(SectionName.ToUpper())))
            {
                Section section = _sectionList.Find(sec => sec.sectionName == SectionName.ToUpper());

                if (!section.ParameterDic.ContainsKey("ParameterName"))
                {
                    section.ParameterDic.Add(ParameterName, ParameterValue);
                    writeToFile();
                }
            }
        }

        public string GetParameterValue(string SectionName, string ParameterName)
        {
            if (_sectionList.Exists(sec => sec.sectionName.Contains(SectionName.ToUpper())))
            {
                Section section = _sectionList.Find(sec => sec.sectionName == SectionName.ToUpper());

                if (section.ParameterDic.ContainsKey(ParameterName))
                {
                    return section.ParameterDic[ParameterName].Trim();
                }
            }
            return "";
        }

        public void UpdateParameterValue(string SectionName, string ParameterName, string NewParameterValue)
        {
            if (_sectionList.Exists(sec => sec.sectionName.Contains(SectionName.ToUpper())))
            {
                Section section = _sectionList.Find(sec => sec.sectionName == SectionName.ToUpper());

                if (section.ParameterDic.ContainsKey(ParameterName))
                {
                    section.ParameterDic[ParameterName] = NewParameterValue;
                    writeToFile();
                }
            }
        }

        public void WriteIniFile()
        {
            writeToFile();
        }
  
        /// <summary>
        /// Schreibt die INI-Datei neu mit den geänderten Werten.
        /// </summary>
        /// <returns></returns>
        private bool writeToFile()
        {
            bool checkWrite = false;
            try
            {
                using (StreamWriter sw = new StreamWriter(INIPath, false))
                {
                    sw.WriteLine(GetIniHeaderText);

                    //TODO Sections aufsteigend sortiert (alphabetisch) ausgeben.
                    foreach (Section section in _sectionList)
                    {
                        sw.WriteLine("[{0}]", section.sectionName.ToUpper());
                        foreach (string parameter in section.ParameterDic.Keys)
                        {
                            sw.WriteLine("{0} = {1}", parameter, section.ParameterDic[parameter]);
                        }
                        sw.WriteLine();
                    }
                    sw.Close();
                    checkWrite = true;
                }
            }
            catch (IOException ex)
            { return checkWrite; }
            return checkWrite;
        }
        #endregion


        #region SUBCLASSES

        public class Section
        {
            //# EIGENSCHAFTEN
            public string sectionName { get; set; } = "";
            public Dictionary<string, string> ParameterDic = new Dictionary<string, string>();

            //# KONSTRUKTOR
            public Section() { }
            public Section(string SectionName)
            {
                sectionName = SectionName;
            }

        }
        #endregion
    }
}
