using Microsoft.Win32.SafeHandles;
using System.Threading.Tasks.Dataflow;
using System.Runtime.InteropServices;
using System;
using System.Globalization;
using System.Xml.Linq;
using System.ComponentModel.Design;
using System.Data;
using System.IO;


namespace Hugo
{
    class Program
    {
        /*
        Main class (Parent) generate by "dotnet command line"
        */
        static List<string> listWord = new List<string>();
        static List<string> listWordNodeName = new List<string>();
        static Quizz quizz = new Quizz();
        static Command commandClass = new Command();
        
        static Dictionary<string, int> dictUserGameData = new Dictionary<string, int>();

        static void Main(string[] args)
        {
            /*
            This method is the main of this class and maybe of the program (Console).
            */

            Console.Clear();
            Console.Title = "Hugo.exe **";
            string command = "ouvert";

            List<string> listCommand = new List<string>();
            string environnementPath = Directory.GetCurrentDirectory();

            Set_DictGameUserData();
            
            LocalFile file = new LocalFile();
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
                            dictUserGameData["index_question"] = dictUserGameData["maximum_question"];
                            
                        } else if (command == listCommand[3] && quizz.doesUserPlay) {
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.WriteLine("Arrêt de la partie... Bravo!");
                            Console.ReadKey();
                            quizz.StopGame();
                        }
                    } else if (command.Contains(listCommand[6])) {
                        string arg_string = commandClass.Get_Argument(command);
                        bool is_num;
                        if (command.Length > 6)
                        {
                            is_num = commandClass.Set_Arg_Is_An_Numeric_Character(arg_string);
                            if (is_num)
                            {
                                int arg_int = commandClass.Convert_Argument_To_Int(arg_string);
                                dictUserGameData["maximum_question"] = arg_int;
                                Console.ForegroundColor = ConsoleColor.Green;
                                if (arg_int != 0)
                                {
                                    Console.WriteLine("Nombre de question changait à {0}", arg_int);
                                } else {
                                    Console.WriteLine("Nombre de maximum de question mit à l'infit");
                                }
                                Console.ReadKey();
                            } else {
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.WriteLine("Votre argument n'est pas valide.");
                                Console.ReadKey();
                            }
                        } else {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine("Veuillez préciser un argument.");
                            Console.ReadKey();
                        }

                    } else if (quizz.doesUserPlay){
                        //Play Commande
                        Console.WriteLine("cmd :{0} | cmd == stop : | ", command, (command == listCommand[3]), listCommand[3]);
                        Console.ReadKey();
                        quizz.CheckAnswer(command);
                        if (command == quizz.answer) {
                            Console.WriteLine("Bravo!");
                            if (dictUserGameData["maximum_question"] == 0 || dictUserGameData["index_question"] > 0)
                            {
                                quizz.SetWord();
                                if (dictUserGameData["index_question"] > 0)
                                {
                                    dictUserGameData["index_question"] -= 1;
                                } else if (dictUserGameData["maximum_question"] != 0) {
                                    Console.ForegroundColor = ConsoleColor.DarkRed;
                                    Console.WriteLine("Partie finit!");
                                    Console.ReadKey();
                                }
                            }
                        } else {
                            Console.WriteLine("Oops!");
                        }
                        Console.ReadLine();
                    } else if (command == listCommand[2]) {
                        quizz.StartGame();
                    } else if (command.Contains(commandHelp)) {
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
                    } else if (command.Contains(listCommand[5])){
                        

                        commandClass.ChangeCommandeInformation(command);
                        bool isCommandCorrect = commandClass.GetIsCommandRight();
                
                        if (isCommandCorrect)
                        { 
                            List<string> word = GetWord(command);
                            bool isThisWordExistAlready = file.SetIsThisWordInXmlFileAlready(stringNode:word[0], nodeInnerText:word[1], pathRoot:environnementPath+@"\ressource\word.xml", xPath:"word/*");
                            if (isThisWordExistAlready)
                            {
                                quizz.RemoveWord(word[1], pathRoot:environnementPath, wordToDelete:command);
                                Console.ReadKey();
                            } else {
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.WriteLine("Attention... vous essayez de supprimer un mot qui n'existe pas.");
                                Console.ReadKey();
                            }
                        } else {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine(commandClass.GetError());
                            Console.ReadKey();
                        }
                    } else if (command.Contains(commandAdd)) {
                        commandClass.ChangeCommandeInformation(command);
                        bool isCommandCorrect = commandClass.GetIsCommandRight();
                        if (isCommandCorrect)
                        {
                            List<string> word = GetWord(command);
                            bool isThisWordExistAlready = file.SetIsThisWordInXmlFileAlready(stringNode:word[0], nodeInnerText:word[1], pathRoot:environnementPath+@"\ressource\word.xml", xPath:"word/*");
                            if (!isThisWordExistAlready)
                            {
                                if (word.Count >= 2)
                                {
                                    file.AddWord(word[0], word[1],@"ressource\word.xml");
                                    Console.WriteLine(word);
                                } else {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Tentative d'ajout d'élément impossible... élément vide!");
                                    Console.ReadKey();
                                }                            
                            } else {
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.WriteLine("Hélas! Le mot qui vous essayez de rajouter est déjà existant.");
                                Console.ReadKey();
                            }
                        } else {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine(commandClass.GetError());
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

        static void Set_DictGameUserData()
        {
            /* 
                Defines dictUserGameData
            */
            dictUserGameData.Add("maximum_question", 0); //0 means inifnite
            dictUserGameData.Add("index_question", 0);

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
}