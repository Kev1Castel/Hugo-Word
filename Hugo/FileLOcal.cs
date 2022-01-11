using System.Collections.Specialized;
using System.Xml;


namespace Hugo
{
    class LocalFile
    {
        /*
        This class is inherit from the Program class.
        This class is for working  with XmlDocument 
        */

        private Interface interfaceConsole = new Interface();
        public List<string> ParseXMLFileToList(string path, string xPath, bool replaceIterTextByName = false) //public
        {
            /*
            This method load and get data from xml file (XmlDocument).
            Take as arguments:
                string path = file environnement path (.\ressource\*.xml)
                string xPath = file XML path (.\*)
                bool replaceIterTextByName = if the data loaded should be 
                    IterName
                        OR
                    Name

            Return listFile<string> = Get XML DATA (Itername OR Name)
            */
            List<string> listFile = new List<string>();
            int maxNode;
            int countNode = 0;
            try
            {
                XmlDocument file = new XmlDocument();
                file.Load(path);
                XmlNodeList XList = file.SelectNodes(xPath);
                maxNode = XList.Count;
                foreach (XmlNode child in XList)
                {
                    countNode++;
                    interfaceConsole.LoadingLine(maxNode:maxNode, counterNode:countNode);
                    if (replaceIterTextByName){
                        listFile.Add(child.Name);
                    } else {
                        listFile.Add(child.InnerText);
                    }
                }
            } catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR]:{0}", ex.GetType());
                Console.Beep();
            }
            return listFile;
        }

        public bool SetIsThisWordInXmlFileAlready(string stringNode, string nodeInnerText, string pathRoot, string xPath)
        {
            /*
                This method is for check if the word is already in the XmlFile
                
                Arg:
                    string the stringNode to check
                    string nodeInnerText to check too
                    string pathRoot for get the environnement path
                    string xPath for the xPath in the xml file

                Return:
                    bool isAlreadyInFile
            */
            bool isAlreadyInFile = false;
           

            XmlDocument file = new XmlDocument();
            file.Load(pathRoot);

            XmlNodeList listNode = file.SelectNodes(xPath);
            foreach(XmlNode nodeChild in listNode)
            {
                if (stringNode == nodeChild.Name)
                {
                    isAlreadyInFile = true;
                } else if (nodeInnerText == nodeChild.InnerText)
                {
                    isAlreadyInFile = true;
                }
            }
            return isAlreadyInFile;
        }
        
        public void AddWord(string cmd, string word, string path)
        {
            /*
            This method used for add word in the xml file wich is loded(Parsed) for saving word 
            
            Arg:
                word (string) to add
            */
            XmlDocument file = new XmlDocument();
            file.Load(path);
            
            // if (word.Contains("\"")) word = word.Replace("\"", "");

            XmlElement newWord = file.CreateElement(cmd);
            newWord.InnerText = word;
            file.DocumentElement.AppendChild(newWord);
            file.Save(path);
        }

        public void RemoveWord(string word, string pathRoot, string wordToDelete)
        {
            /*
                This method is used for remove xml Node specify as arg in this method

                Arg:
                    word is an string wich contain the name node to del
            */
            if (word != null)
            {
                XmlDocument file = new XmlDocument();
                file.Load(@"ressource\word.xml");
                XmlNodeList listAllNodes = file.SelectNodes("word/*");
                ReWriteXml(listAllNodes, pathRoot:pathRoot, wordToDelete);
                
            }
        }

        public void ReWriteXml(XmlNodeList listNodes, string pathRoot, string nodeToDelete)
        {
            /*
                This method is used for creating file and in adding words from word.xml
                This is for deleting word

                Args:
                    listNodes is an list of Xml Data
            */
            string pathFile = pathRoot+@"\ressource\word_copy.xml";
            string nodeReformatingText = "";
            using (System.IO.File.Create(pathFile));
            XmlDocument newFile = new XmlDocument();
            newFile.LoadXml("<?xml version='1.0' ?>" +
                    "<word>" +
                    "</word>");
            newFile.Save(pathFile);
            
            nodeReformatingText = nodeToDelete;
            nodeReformatingText = nodeReformatingText.Replace("remove(", "");
            nodeReformatingText = nodeReformatingText.Replace(")", "");
            foreach(XmlNode node in listNodes)
            {
                if (node.InnerText != nodeReformatingText)
                {
                    Console.WriteLine("nodeReformat :{0} | nodeInnerText:{1}", nodeReformatingText, node.InnerText);
                    AddWord(cmd:node.InnerText, word:node.Name, pathFile);
                }
            }
            File.Delete(pathRoot+@"\ressource\word.xml");
            File.Move(pathRoot+@"\ressource\word_copy.xml", pathRoot+@"\ressource\word.xml");
            // var hi = File.Delete("ressource");

        }
    }
}