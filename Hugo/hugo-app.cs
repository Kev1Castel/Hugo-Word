// using Internal;
using System;
using System.Globalization;
using System.Xml.Linq;
using System.ComponentModel.Design;
using System.Data;
using System.IO;
using System.Xml;
// [DllImport("msvcrt.dll", CharSet = CharSet.Unicode)]

class Quizz: File
{
    /*
    This class is inherit from the Program class.
    This class is for work with QuizzElement like the 
    user point, question, etc! 
    */
    public bool doesUserPlay = false;
    public string word = "";
    public string answer = "";
    private int pointWon;
    private int pointLoss;
    private int tryed;
    
    
    protected Dictionary<string, string> dictW = new Dictionary<string, string>();
    public Dictionary<string, string> dictWord {get {return dictW;}}

    public Quizz()
    {
        /*
        Constructor class for Quizz.
        Take argument as pE:
            string pE is the actual environnement path (Get From Main Class (Program))
        */
        dictW = new Dictionary<string, string>();
    }

    public void ShowGameMessage()
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write("Réecriver :");
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write(this.word);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(" (avec leurs accentuations d'origine)! Alors :");
    }

    public void CheckAnswer(string input)
    {
        /*
        This method check if the input is equal to the answer
        Take argument as:
            string input that is the user input
        */
        bool doesInputIsRight = (this.answer.Count()-2 == input.Count() && input.Any(this.answer.Contains));
        if (doesInputIsRight)
        {
            this.pointWon++;
            this.SetWord();
        } else if (this.tryed == 3) {
            this.tryed = 0;
            this.SetWord();
            Console.WriteLine("Encore {0} essais sur cette question!", this.tryed);
        } else {
            this.tryed++;
            this.pointLoss++;
        }         
    }


    private void SetWord()
    {
        /*
        This method is used for set word for the game
        */

        try {
            string subStringToCheck = "\"";
            Random rnd = new Random();
            List<string> listWordNodeName = new List<string>();
            listWordNodeName = dictWord.Keys.ToList();
            int index = rnd.Next(listWordNodeName.Count);
            this.word = listWordNodeName.ElementAt(index);
            this.answer = dictWord[this.word];
            if (this.answer.Any(subStringToCheck.Contains))
            {
                this.answer = this.answer.Replace("\"","");
            }
        } catch (Exception ex) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ERROR]:{0}", ex.GetType());
            Console.Beep();
        }
    }


    public void GetDictWord(List<string> listName, List<string> listIterText)
    {
        /*
        Method for getting all word from ".\ressource\word.xml"
        Method getting data from xml thanks to XmlDocument 
        */
        string message = (listName.Count == listIterText.Count) ? "Ouais!" : "Erreur de grandeur...";
        if (listIterText.Count == listName.Count)
        {
            for(int i = 0;i<listIterText.Count; i++)
            {
                this.dictWord[listIterText[i]] = listName[i];
            }
        }
    }

    public void StopGame()
    {
        /*
        This method stops the game
        */
        if (this.doesUserPlay)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Fermeture de la partie!");
            this.doesUserPlay = false;
            this.word = "";
        } else {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Tu ne jouais pas...!");
        }
    }

    public void StartGame()
    {
        /*
        This method starts game
        */
        Console.Clear();
        this.doesUserPlay = true;
        this.pointWon = 0;
        this.pointLoss = 0;
        this.tryed = 0;
        Console.WriteLine("");
        this.SetWord();
    }
}

class File
{
    /*
    This class is inherit from the Program class.
    This class is for working  with XmlDocument 
    */
    public List<string> ParseXMLFileToList(string path=null, string xPath=null, bool replaceIterTextByName = false) //public
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
        try
        {
            XmlDocument file = new XmlDocument();
            file.Load(path);
            XmlNodeList XList = file.SelectNodes(xPath);
            foreach (XmlNode child in XList)
            {
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
	
	public void AddWord(string cmd, string word, string path)
	{
		/*
		This method used for add word in the xml file wich is loded(Parsed) for saving word 
		
		Arg:
			word (string) to add
		*/
        XmlDocument file = new XmlDocument();
        file.Load(path);
        XmlElement new_word = file.CreateElement(cmd);
        new_word.InnerText = word;
        file.DocumentElement.AppendChild(new_word);
        file.Save(path);
    }

    public void RemoveWord(string word)
    {
        /*
            This method is used for remove xml Node specify as arg in this method

            Arg:
                word is an string wich contain the name node to del
        */
        if (word != null)
        {
            // XmlDocument file = new XmlDocument();
            // file.Load(@"ressource\word.xml");
            // // var something = file.
            // XmlNodeList listChild = file.SelectNodes(@"word/*");
            // bool isWordExisting = false;
            // int index = -1;
            // foreach(XmlNode c in listChild)
            // {
            //     string nodeName = "\""+c.Name+"\"";
            //     if (nodeName == word)
            //     {
            //         Console.WriteLine("Mot trouvait! Effacage comlpéter");
            //         isWordExisting = true;
            //         // var tests = file.RemoveChild(c);
            //         var tests = file.Node

            //     }
            //     if (!isWordExisting)
            //     {
            //         index++;

            //     }
            // }
            // if (!isWordExisting)
            // {
            //     Console.WriteLine("Mot non-trouvait");
            // }
            XmlDocument file = new XmlDocument();
            file.Load(@"ressource\word.xml");
            XmlNodeList listAllNodes = file.SelectNodes("word/*");
            ReWriteXml(listAllNodes);
        }
    }

    private void ReWriteXml(XmlNodeList listNodes)
    {
        /*
            This method is used for creating file and in adding words from word.xml
            This is for deleting word

            Args:
                listNodes is an list of Xml Data
        */
        string pathFile = @"ressource\word_copy.xml";
        using (System.IO.File.Create(pathFile));
        XmlDocument newFile = new XmlDocument();
        newFile.LoadXml("<?xml version='1.0' ?>" +
                "<word>" +
                "</word>");
        newFile.Save(pathFile);
        foreach(XmlNode node in listNodes)
        {
            AddWord(cmd:node.InnerText, word:node.Name, @"\ressource\word_copy.xml");
        }



    }
}


class Program
{
    /*
    Main class (Parent) generate by "dotnet command line"
    */
    static List<string> listWord = new List<string>();
    static List<string> listWordNodeName = new List<string>();
    static Quizz quizz = new Quizz();
    
    // static Dictionary<string, string> dictWord = new Dictionary<string, string>();

    static void Main(string[] args)
    {
        /*
        This method is the main of this class and maybe of the program (Console).
        */

        Console.Clear();
        Console.Title = "Adrien.exe **";
        string command = "ouvert";

        List<string> listCommand = new List<string>();
        string environnementPath = Directory.GetCurrentDirectory();

        File file = new File();
        listCommand = file.ParseXMLFileToList(path:environnementPath+@"\ressource\commandes.xml", xPath:@"commande/*");
        listWordNodeName = file.ParseXMLFileToList(path:environnementPath+@"\ressource\word.xml", xPath:@"word/*", replaceIterTextByName:true);

        List<string> lTest = new List<string>();
        quizz.GetDictWord(file.ParseXMLFileToList(path:environnementPath+@"\ressource\word.xml", xPath:@"word/*"), file.ParseXMLFileToList(path:environnementPath+@"\ressource\word.xml", xPath:@"word/*", true));
        if (listCommand.Count != 0)
        {
            while (command != listCommand[0])
            {
                string commandAdd = listCommand[4];
                string commandHelp = listCommand[1];
                bool isGamingCommand = (command == listCommand[2] || command == listCommand[3]);
                ShowIntroductionMessage(environnementPath);

                command = Console.ReadLine();

                if (isGamingCommand) //listCommand.Contains(command)
                {
                    //Console Commande
                    Console.ForegroundColor = ConsoleColor.Blue;
                    if (command == listCommand[2])
                    {
                        quizz.StartGame();
                        
                    } 
                } else if (quizz.doesUserPlay){
                    //Play Commande
                    quizz.CheckAnswer(command);
                    if (command == listCommand[3]) {
                        quizz.StopGame();
                    } else if (command == quizz.answer) {
                        Console.WriteLine("Bravo!");
                    } else {
                        Console.WriteLine("Oops!");
                    }
                    Console.ReadLine();
                    // Console.Clear();
                } else if (command == listCommand[2]) {
                    quizz.StartGame();
                }   else if (command.Contains(commandHelp)) {
                    string prefix_command = "--";
                    if (command.Any(prefix_command.Contains))
                    {
                        string childCommand = GetArgument(command);
                        if (childCommand != null)
                        {
                            if (childCommand == listCommand[4])
                            {
                                ShowCommandTargetHelp(childCommand, listCommand);
                            }
                        }
                    } else {
                        ShowCommandGlobalHelp(listCommand);
                    }

                    // if (command == listCommand[1])
                    // {
                    //     // listCommand.ForEach(Console.WriteLine);
                    //     ShowCommandHelp(listCommand);

                
                } else if (command.Contains(listCommand[5])){
                    List<string> word = GetWord(command);
                    quizz.RemoveWord(word[1]);
                    Console.ReadKey();
                } else if (command.Contains(commandAdd)) {
                    List<string> word = GetWord(command);
                    word.ForEach(Console.WriteLine);
                    Console.WriteLine(word.Count);
                    if (word.Count >= 2)
                    {
                        file.AddWord(word[0], word[1],@"ressource\word.xml");
                        Console.WriteLine(word);
                    } else {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Tentative d'ajout d'élément impossible... élément vide!");
                        Console.ReadKey();
                    }
                } else if (command.Length > 0 & command != listCommand[0]) {    
                    //Console Commande Error               
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Désolé mais la commande semble incorrecte! \r\nSi le probléme persiste taper 'aide'\r\nAppuyer sur une touche pour continuer.");
                    Console.ReadKey();
            } else {
                    Console.Clear();
                }
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Fermeture de l'app.");
        } else {
            Console.WriteLine("Désolé, mais les commandes n'ont pas chargaient...");
        }
    }

    static string GetArgument(string cmd)
    {
        /*
            This method is used for when 'help' was inputed for getting
                an help target on some commands
        */
        string argument = null;
        string cString;
        int countPrefix = 0;
        foreach(char c in cmd)
        {
            cString = c.ToString();
            if (cString == "-")
            {
                countPrefix++;
            }
            if (countPrefix == 2 && cString != "=")
            {
                argument += cString;
            }
        }
        if (countPrefix != 2)
        {
            argument = null;
        } else {
            if (argument.Contains("-"))
            {
                argument = argument.Replace("-","");
            }
        }
        return argument;
    }

    static void ShowCommandTargetHelp(string commandTarget, List<string> lCommand)
    {
        /*
            This method is get targetting help, it target an command for returning her help.
            Show that help in Console App.

            Take Args:
                commandTarget is an string that contains the command targeted
        */
        List<string> listSubCommand = new List<string>();
        if (commandTarget == lCommand[4])
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"Commande d'aide pour '{commandTarget}':");
            Console.WriteLine("\r\t");
        } else {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"\r\t'{commandTarget}' n'est pas reconnus en tant-que commande interne donc voici 'aide'");
            Console.ReadKey();
        }
    }

    static void ShowIntroductionMessage(string ep)
    {
        Console.Clear();
        if (quizz.doesUserPlay == false)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"--Application démarrer ici:'{ep}'\r\n-Taper une commande:");
            Console.ForegroundColor = ConsoleColor.White;
        } else {
            quizz.ShowGameMessage();
        }
        Console.ForegroundColor = ConsoleColor.DarkGray;
    }
	
    static void ShowCommandGlobalHelp(List<string> lCommand)
    {
        Dictionary<string, string> dictHelpCommand = new Dictionary<string, string>();
        List<string> lCommandDo = new List<string>();
        lCommandDo.Add("ferme l'application");
        lCommandDo.Add("affiche cette commande");
        lCommandDo.Add("démarre une session de jeu non sauvegardée");
        lCommandDo.Add("détruit une session de jeu non sauvegardée");
        lCommandDo.Add("ajoute des mots (voir 'aide --ajouter')");
        lCommandDo.Add("supprime des mots (voir 'aide --supprimer')");
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine("Commande d'aide appellait:");

        foreach(string element in lCommand)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            // Console.WriteLine();
            Console.Write($"\r\t{element} :");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(lCommandDo[lCommand.IndexOf(element)]);
            // Console.Write();
        }
        Console.WriteLine("");
        Console.ReadKey();
    }
	static List<string> GetWord(string cmd)
	{
		/*
		This method is used for get the word when the user add it
		
		Arg:
			cmd = string command from input
		Return:
			//word wich is an string containing the word added
            list that contains word encrypted and not encrypted
		*/
        List<string> listNewWord = new List<string>();
		bool doesParenthesisOpen = false;
		string word = "";
		string word_encrypted = "";
		string string_c;
		foreach (char c in cmd)
		{
			string_c = c.ToString();
            string_c = string_c.ToLower();
			if (string_c == "(")
			{
				doesParenthesisOpen = true;
			} else if (doesParenthesisOpen && string_c == ")") {
				doesParenthesisOpen = false;
			}
			if (doesParenthesisOpen && string_c != "(")
			{
                word_encrypted += string_c;
                if (string_c != "\"") { 
                    if (string_c == "é" || string_c == "è" || string_c == "ê" || string_c == "ë")
                    {
                        word += "e";
                    } else if (string_c == "ù" || string_c == "û" || string_c == "ü") {
                        word += "u";
                    } else if (string_c == "ô" || string_c == "ö"){
                        word += "o";
                    } else {
                        word += string_c;				
                    }
                }
			}
		}
        listNewWord.Add(word);
        listNewWord.Add(word_encrypted);
		return listNewWord;
		
	}
}