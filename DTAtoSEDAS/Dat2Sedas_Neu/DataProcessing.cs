using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ConvertDatToSedas;

namespace Dat2Sedas_Neu
{
    public static class DataProcessing
    {
        public static ArticleDeletionList GetDeleteArticlesList(string Path)
        {
            return new ArticleDeletionList(LoadDeleteItemsList(Path));
        }

        public static CustomerDeletionList GetDeleteCustomersList(string Path)
        {
            return new CustomerDeletionList(LoadDeleteItemsList(Path));
        }

        public static ArticleChangeList GetChangeArticlesList(string Path)
        {
            List<string> changeArticleFileContent = new List<string>();
            try
            {
                changeArticleFileContent = GetFileContent(Path);
            }
            catch (Exception ex)
            { }

            ArticleChangeList articleChangeList = new ArticleChangeList();
            foreach (string line in changeArticleFileContent)
            {
                if (line != "")
                {
                    string[] elements = line.Split(';');
                    ArticleChangePair newPair = new ArticleChangePair(elements[0].Trim(), elements[1].Trim());
                    articleChangeList.Add(newPair);
                };

            }
            return articleChangeList;
        }

        public static bool WriteToFile(string content, string filePath)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath, false))
                {
                    sw.Write(content);
                }
                return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public static List<string> GetFileContent(string sourcePathNFDatFile)
        {
            List<string> _sourceDataList = new List<string>();
            try
            {
                using (StreamReader sr = new StreamReader(sourcePathNFDatFile))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (line != "")
                            _sourceDataList.Add(line);
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO Fehlerausnahme auslösen und Fehler melden
                throw new Exception(ex.Message);
            }
            return _sourceDataList;
        }

        public static List<string> LoadInputFile(string sourcePathNFDatFile)
        {
            List<string> _sourceDataList = new List<string>();
            try
            {
                using (StreamReader sr = new StreamReader(sourcePathNFDatFile))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (line != "")
                            _sourceDataList.Add(line);
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO Fehlerausnahme auslösen und Fehler melden
                throw new Exception(ex.Message);
            }
            return _sourceDataList;
        }


        private static List<string> LoadDeleteItemsList(string Path)
        {
            List<string> itemList = new List<string>();
            try
            {
                List<string> allLines = File.ReadAllText(Path).Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
                foreach (string element in allLines)
                {
                    itemList.Add(element.Split(';')[0].Trim()); //nur erstes Element von Split nehmen.
                }
            }
            catch (Exception ex)
            { }

            return itemList;
        }


    }
}
