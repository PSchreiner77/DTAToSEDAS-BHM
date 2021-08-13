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
        #region PROPERTIES
        private string _INIPath = System.IO.Directory.GetCurrentDirectory() + "\\Settings.ini";
        private List<Section> _iniContent = new List<Section>();
        private List<string> _iniImportedTextLines = new List<string>();

        private string INIPath
        {
            get { return _INIPath; }
            set
            {
                _INIPath = checkIniPath(value);
            }
        }

        #endregion


        #region KONSTRUKTOR    
        public IniManager()
        { }


        public IniManager(string Path)
        {

            if (!System.IO.File.Exists(Path))
            {
                throw new FileLoadException("Ini-Datei kann nicht geladen werden");
            }
            INIPath = Path;
            if (readIniFile(_INIPath)) importIniContent(_iniImportedTextLines);

        }
        #endregion


        #region PRIVATE METHODS

        private string checkIniPath(string path)
        {

            if (!System.IO.File.Exists(path))
            {
                throw new IOException($"Ini-Datei {path} kann nicht geladen werden");
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
            _iniImportedTextLines.Clear();
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    while (!sr.EndOfStream)
                    {
                        _iniImportedTextLines.Add(sr.ReadLine());
                    }
                    sr.Close();
                }

                if (_iniImportedTextLines.Count() > 0) { return true; }
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

            for (int i = 0; i < _iniImportedTextLines.Count(); i++)
            {
                if (_iniImportedTextLines[i].Trim().Contains("["))
                {
                    //Section gefunden                    
                    string sectionName = getSectionNameFromLine(Content[i].Trim());
                    addNewSectionToSectionList(sectionName);

                    int j = i + 1;
                    while (j < _iniImportedTextLines.Count())
                    {
                        if (_iniImportedTextLines[j].Contains("[")) break;

                        if (_iniImportedTextLines[j].Contains("="))
                        {
                            string[] parameterLine = _iniImportedTextLines[j].Split('=');
                            string parameterName = parameterLine[0].Trim();
                            string ParameterValue = parameterLine[1].Trim();
                            addNewParameterToSectionList(sectionName, parameterName, ParameterValue);
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


        private void addNewSectionToSectionList(string sectionName)
        {
            _iniContent.Add(new Section(sectionName.ToUpper()));
        }


        private bool addNewParameterToSectionList(string SectionName, string ParameterName, string ParameterValue = "")
        {
            bool parameterAdded = false;
            foreach (Section section in this._iniContent)
            {
                if (section.sectionName == SectionName.ToUpper())
                {
                    if (section.ParameterDic.ContainsKey(ParameterName.ToUpper()))
                    {
                        section.ParameterDic[ParameterName] = ParameterValue;
                        parameterAdded = true;
                    }
                    else
                    {
                        section.ParameterDic.Add(ParameterName, ParameterValue);
                        parameterAdded = true;
                    }
                }
            }
            return parameterAdded;
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

        public string GetParameterValue(string SectionName, string ParameterName)
        {
            foreach (Section section in _iniContent)
            {
                if (section.sectionName == SectionName.ToUpper())
                {
                    if (section.ParameterDic.ContainsKey(ParameterName.ToUpper()))
                    {
                        return section.ParameterDic[ParameterName.ToUpper()];
                    }
                }
            }
            return "";
        }


        public void UpdateParameterValue(string SectionName, string ParameterName, string ParameterValue)
        {
            bool valueUpdated = false;
            foreach (Section section in _iniContent)
            {
                if (section.sectionName == SectionName.ToUpper())
                {
                    if (section.ParameterDic.ContainsKey(ParameterName.ToUpper()))
                    {
                        section.ParameterDic[ParameterName.ToUpper()] = ParameterValue;
                        valueUpdated = true;
                        break;
                    }
                }
            }

            if (valueUpdated)
            {
                writeToFile();
            }
        }


        public void AddNewSection(string SectionName)
        {
            bool isNewSection = true;
            foreach (Section section in this._iniContent)
            {
                if (section.sectionName == SectionName.ToUpper())
                {
                    isNewSection = false;
                }
            }

            if (isNewSection)
            {
                addNewSectionToSectionList(SectionName);
                writeToFile();
            }
        }


        public void AddNewParameter(string SectionName, string ParameterName, string ParameterValue = "")
        {
            bool parameterAdded = addNewParameterToSectionList(SectionName, ParameterName, ParameterValue);

            if (!parameterAdded)
            {
                addNewSectionToSectionList(SectionName);
                addNewParameterToSectionList(SectionName, ParameterName, ParameterValue);
            }

            writeToFile();
        }


        public void DeleteParameter(string SectionName, string ParameterName)
        {
            foreach (Section section in _iniContent)
            {
                if (section.sectionName == SectionName.ToUpper())
                {
                    if (section.ParameterDic.ContainsKey(ParameterName.ToUpper()))
                    {
                        section.ParameterDic.Remove(ParameterName.ToUpper());
                    }
                }
            }
        }


        public void DeleteSection(string SectionName)
        {
            //TODO Fehler wg. verändertem index beim löschen

            for (int i = 0; i < _iniContent.Count; i++)
            {
                if (_iniContent[i].sectionName == SectionName) _iniContent.RemoveAt(i);
                break;
            }
            //foreach (Section section in _iniContent)
            //{
            //    if (section.sectionName == SectionName.ToUpper())
            //    {
            //        deletedSections.Add(section);
            //        _iniContent.Remove(section);
            //        break;
            //    }
            //}
        }


        public void DeleteAllEntries()
        {
            _iniContent.Clear();
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
                    //TODO Sections aufsteigend sortiert (alphabetisch) ausgeben.
                    foreach (Section section in _iniContent)
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

        /// <summary>
        /// Schreibt die INI-Datei neu mit den geänderten Werten.
        /// </summary>
        /// <returns></returns>
        private bool writeToFileNew()
        {
            bool checkWrite = false;
            try
            {
                using (StreamWriter sw = new StreamWriter(INIPath, false))
                {
                    //TODO Sections aufsteigend sortiert (alphabetisch) ausgeben.
                    foreach (Section section in _iniContent)
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


    public class IniManagerNew
    {
        #region PROPERTIES
        private string _INIPath = System.IO.Directory.GetCurrentDirectory() + "\\Settings.ini";
        private List<Section> _iniContent = new List<Section>();
        private List<string> _iniTextLines = new List<string>();

        private string INIPath
        {
            get { return _INIPath; }
            set
            {
                _INIPath = checkIniPath(value);
            }
        }

        #endregion


        #region KONSTRUKTOR    
        public IniManagerNew()
        { }


        public IniManagerNew(string Path)
        {

            if (!System.IO.File.Exists(Path))
            {
                throw new FileLoadException("Ini-Datei kann nicht geladen werden");
            }
            INIPath = Path;
            if (readIniFile(_INIPath)) importIniContent(_iniTextLines);

        }
        #endregion


        #region PRIVATE METHODS

        private string checkIniPath(string path)
        {

            if (!System.IO.File.Exists(path))
            {
                throw new IOException($"Ini-Datei {path} kann nicht geladen werden");
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


        ///// <summary>
        ///// Liest den Inhalt aus der List<string> iniContent in eine List<Section> ein.
        ///// </summary>
        ///// <param name="Content">List<string> mit Zeilen der INI-Datei.</string></param>
        //private void importIniContent(List<string> Content)
        //{
        //    List<string> sectionBlock = new List<string>();

        //    for (int i = 0; i < _iniTextLines.Count(); i++)
        //    {
        //        if (_iniTextLines[i].Trim().Contains("["))
        //        {
        //            //Section gefunden                    
        //            string sectionName = getSectionNameFromLine(Content[i].Trim());
        //            addNewSectionToSectionList(sectionName);

        //            int j = i + 1;
        //            while (j < _iniTextLines.Count())
        //            {
        //                if (_iniTextLines[j].Contains("[")) break;

        //                if (_iniTextLines[j].Contains("="))
        //                {
        //                    string[] parameterLine = _iniTextLines[j].Split('=');
        //                    string parameterName = parameterLine[0].Trim();
        //                    string ParameterValue = parameterLine[1].Trim();
        //                    addNewParameterToSectionList(sectionName, parameterName, ParameterValue);
        //                }
        //                j++;
        //            }
        //            i = j - 1;
        //        }
        //    }
        //}


        //private string getSectionNameFromLine(string sectionHeader)
        //{
        //    string sectionName = sectionHeader.Trim().ToUpper();

        //    if (sectionName.Substring(0, 1) == "[") sectionName = sectionName.Remove(0, 1);
        //    if (sectionName.Substring(sectionName.Length - 1, 1) == "]") sectionName = sectionName.Remove(sectionName.Length - 1);
        //    return sectionName;
        //}


        //private void addNewSectionToSectionList(string sectionName)
        //{
        //    _iniContent.Add(new Section(sectionName.ToUpper()));
        //}


        //private bool addNewParameterToSectionList(string SectionName, string ParameterName, string ParameterValue = "")
        //{
        //    bool parameterAdded = false;
        //    foreach (Section section in this._iniContent)
        //    {
        //        if (section.sectionName == SectionName.ToUpper())
        //        {
        //            if (section.ParameterDic.ContainsKey(ParameterName.ToUpper()))
        //            {
        //                section.ParameterDic[ParameterName] = ParameterValue;
        //                parameterAdded = true;
        //            }
        //            else
        //            {
        //                section.ParameterDic.Add(ParameterName, ParameterValue);
        //                parameterAdded = true;
        //            }
        //        }
        //    }
        //    return parameterAdded;
        //}
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

        //public string GetParameterValue(string SectionName, string ParameterName)
        //{
        //    foreach (Section section in _iniContent)
        //    {
        //        if (section.sectionName == SectionName.ToUpper())
        //        {
        //            if (section.ParameterDic.ContainsKey(ParameterName.ToUpper()))
        //            {
        //                return section.ParameterDic[ParameterName.ToUpper()];
        //            }
        //        }
        //    }
        //    return "";
        //}


        private int getEntryIndex(string SectionName)
        {
            foreach (string line in _iniTextLines)
            {
                if (line.ToUpper().Contains($"[{SectionName.ToUpper()}]"))
                {
                    return _iniTextLines.IndexOf(line);
                }
            }
            return 0;
        }

        private int getEntryIndex(string SectionName, string ParameterName)
        {
            int index = getEntryIndex(SectionName);
            if (index == 0) return 0;

            index++;
            for (int i = index; i < _iniTextLines.Count; i++)
            {
                if (_iniTextLines[index].Contains("[") || _iniTextLines[index] == "")
                    return 0;

                if (_iniTextLines[index].ToUpper().Contains(ParameterName.ToUpper()))
                    return index;
            }
            return 0;
        }


        public string GetParameterValue(string SectionName, string ParameterName)
        {
            int index = getEntryIndex(SectionName, ParameterName);
            if (index == 0)
                return "";

            string[] parameter = _iniTextLines[index].Split('=');
            return parameter[1].Trim();
        }


        public void UpdateParameterValue(string SectionName, string ParameterName, string NewParameterValue)
        {
            bool valueUpdated = false;
            int index = getEntryIndex(SectionName, ParameterName);
            if (index != 0)
            {
                string[] parameter = _iniTextLines[index].Split('=');
                parameter[1] = NewParameterValue;
                _iniTextLines[index] = parameter[0] + "=" + parameter[1];

                writeToFile();
            }
        }

        //TODO Erweitern und löschen von Einträgen und Sektionen überarbeiten!!!
        //public void AddNewSection(string SectionName)
        //{
        //    bool isNewSection = false;
        //    if (getEntryIndex(SectionName) == 0)
        //        isNewSection = false;

        //    if (isNewSection)
        //    {
        //        _iniTextLines.Add($"[{SectionName.ToUpper()}");
        //        writeToFile();
        //    }
        //}


        //public void AddNewParameter(string SectionName, string ParameterName, string ParameterValue = "")
        //{
        //    bool parameterAdded = addNewParameterToSectionList(SectionName, ParameterName, ParameterValue);

        //    if (getEntryIndex(SectionName) == 0) AddNewSection(SectionName);

        //    int index = getEntryIndex(SectionName);
        //    _iniTextLines.Insert(++index, $"{ParameterName}={ParameterValue}");

        //    writeToFile();
        //}


        //public void DeleteParameter(string SectionName, string ParameterName)
        //{
        //    int index = getEntryIndex(SectionName, ParameterName);
        //    if (index != 0) _iniTextLines.RemoveAt(index);
        //}


        //public void DeleteSection(string SectionName)
        //{
        //    //TODO Fehler wg. verändertem index beim löschen

        //    for (int i = 0; i < _iniContent.Count; i++)
        //    {
        //        if (_iniContent[i].sectionName == SectionName) _iniContent.RemoveAt(i);
        //        break;
        //    }
        //    //foreach (Section section in _iniContent)
        //    //{
        //    //    if (section.sectionName == SectionName.ToUpper())
        //    //    {
        //    //        deletedSections.Add(section);
        //    //        _iniContent.Remove(section);
        //    //        break;
        //    //    }
        //    //}
        //}


        //public void DeleteAllEntries()
        //{
        //    _iniContent.Clear();
        //    writeToFile();
        //}


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
                    sw.WriteLine(_iniTextLines);

                    ////TODO Sections aufsteigend sortiert (alphabetisch) ausgeben.
                    //foreach (Section section in _iniContent)
                    //{
                    //    sw.WriteLine("[{0}]", section.sectionName.ToUpper());
                    //    foreach (string parameter in section.ParameterDic.Keys)
                    //    {
                    //        sw.WriteLine("{0} = {1}", parameter, section.ParameterDic[parameter]);
                    //    }
                    //    sw.WriteLine();
                    //}
                    sw.Close();
                    checkWrite = true;
                }
            }
            catch (IOException ex)
            { return checkWrite; }
            return checkWrite;
        }

        /// <summary>
        /// Schreibt die INI-Datei neu mit den geänderten Werten.
        /// </summary>
        /// <returns></returns>
        private bool writeToFileNew()
        {
            bool checkWrite = false;
            try
            {
                using (StreamWriter sw = new StreamWriter(INIPath, false))
                {
                    //TODO Sections aufsteigend sortiert (alphabetisch) ausgeben.
                    foreach (Section section in _iniContent)
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
