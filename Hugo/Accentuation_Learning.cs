using System.Collections.Concurrent;
using System.Runtime.Serialization;
using System.Reflection.Emit;
// using Internal;
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
        
        static List<string> listCommand = new List<string>();
        static Dictionary<string, int> dictUserGameData = new Dictionary<string, int>();

        static void Main(string[] args)
        {
            /*
            This method is the main of this class and maybe of the program (Console).
            */

            Console.Clear();
            Console.Title = "Accentuation Training.exe";
            string command = "ouvert";

            string environnementPath = Directory.GetCurrentDirectory();

            Set_DictGameUserData();
            
            LocalFile file = new LocalFile();
            listCommand = file.ParseXMLFileToList(path:environnementPath+@"\ressource\commandes.xml", xPath:@"commande/*");
            listWordNodeName = file.ParseXMLFileToList(path:environnementPath+@"\ressource\word.xml", xPath:@"word/*", replaceIterTextByName:true);

            List<string> lTest = new List<string>();
            quizz.GetDictWord(file.ParseXMLFileToList(path:environnementPath+@"\ressource\word.xml", xPath:@"word/*"), file.ParseXMLFileToList(path:environnementPath+@"\ressource\word.xml", xPath:@"word/*", true));
            if (listCommand.Count != 0)
            {
                while (command != listCommand[0] || quizz.doesUserPlay)
                {
                    string commandAdd = listCommand[4];
                    string commandHelp = listCommand[1];

                    ShowIntroductionMessage(environnementPath);

                    command = Console.ReadLine();

                    //Better lines

                    if (quizz.doesUserPlay) {// Game stuff
                        if (command == listCommand[3]) {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("-Félicitation, fin de la partie.");
                            Console.ReadKey();
                            quizz.StopGame();
                        } else if (command == listCommand[0]) {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("-Pour rappel, vous êtes en jeu alors utiliser la commande"); 
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write(" 'stop'");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("d'abord.");
                            Console.ReadKey();
                        } else {
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
                                Console.WriteLine("Mauvaise réponse....!");
                            }
                            Console.ReadLine();
                        }

                    } else { //Main commands stuff here
                        if (command == listCommand[2]) //Play
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            dictUserGameData["index_question"] = dictUserGameData["maximum_question"];
                            Console.WriteLine("-Une partie à était démarrer.");
                            Console.ReadKey();
                            quizz.StartGame();
                        } else if (command.Contains(listCommand[1])){ //help command
                            if (command.Length == 4)
                            {
                                ShowCommandGlobalHelp(listCommand);
                            } else {
                                List<string> listSubCommand = new List<string>();
                                listSubCommand.Add(listCommand[4]);
                                listSubCommand.Add(listCommand[5]);
                                listSubCommand.Add(listCommand[6]);
                                bool isHelpCommandValid = commandClass.CheckHelpCommand(command ,listSubCommand);
                                string subCommand = commandClass.GetSubHelpCommand(command);
                                //Show Help Command
                                ShowCommandTargetHelp(subCommand);
                                Console.ReadKey();
                            }
                        } else if (command.Contains(listCommand[4])){ //Add word in the xml file
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
                        } else if (command.Contains(listCommand[5])) { //Delete word in the xml File
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
                        } else if (command.Contains(listCommand[6])){ //option (change game data)
                            string stringArgument = commandClass.GetArgument(command, "option");
                            bool isArgumentValid = commandClass.SetArgIsAnNumericCharacter(stringArgument);
                            if (isArgumentValid)
                            {
                                int intArgument = commandClass.ConvertStringToInt(stringArgument);
                                if (intArgument > -1 && intArgument < 100)
                                {
                                    if (intArgument != 0)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Blue;
                                        Console.WriteLine("-Superbe, le nombre de maximum de question à était réactualiser à {0}", stringArgument);
                                    } else {
                                        Console.ForegroundColor = ConsoleColor.Blue;
                                        Console.WriteLine("-Superbe, le nombre à maximum est assigner à l'infinit.");
                                    }
                                    dictUserGameData["maximum_question"] = intArgument;
                                } else {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("-Erreur, veillez à choissir un nombre entre 0 et 99");
                                }
                                Console.ReadKey();
                            }
                        } else if(command != listCommand[0]) {  //Error stuff here
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("-Commande incorrecte:");
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write("\r\tVous pouvez utilisé la commande ");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("'aide'.");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\r\n--Appuyer sur 'entrer' pour continuer.");
                            Console.ReadKey();
                        }
                        
                    }
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("--À bientot!");
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.ForegroundColor = ConsoleColor.White;
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

        static void ShowCommandTargetHelp(string commandTarget)
        {
            /*
                This method is get targetting help, it target an command for returning her help.
                Show that help in Console App.

                Take Args:
                    commandTarget is an string that contains the command targeted
            */
            Dictionary<string, string> dictHelpCommandTargeted = new Dictionary<string, string>();
            dictHelpCommandTargeted.Add(listCommand[5], $"sert à supprimer un mot cible.\r\n\r\tPar exemple:\r\n\r\t\tVous souhaiteriez supprimer le mot \"Abc\" alors \r\n\r\t\r\t {listCommand[5]} (\"Abc\")");
            dictHelpCommandTargeted.Add(listCommand[4], $"sert à ajouter un mot cible.\r\n\r\tPar exemple:\r\n\r\t\tVous souhaiteriez ajouter le mot \"Abc\" alors \r\n\r\t\r\t {listCommand[4]} (\"Abc\")");
            dictHelpCommandTargeted.Add(listCommand[6], $"sert à modifier les paramètre d'une session de quizz.\r\n\r\tPar exemple:\r\n\r\t\tVous souhaiteriez modifier le maximum de question d'une session de quizz alors \r\n\r\t\r\t {listCommand[6]} 5");
            if (dictHelpCommandTargeted.Keys.Contains(commandTarget))
            {
                Console.WriteLine(dictHelpCommandTargeted[commandTarget]);
            } else {
                Console.WriteLine("Désolé mais la commande n'existe donc aucunne aide n'a était trouvée.");
            }
        }

        static void ShowIntroductionMessage(string ep)
        {
            Console.Clear();
            if (quizz.doesUserPlay == false)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"--Application démarrer ici:'{ep}'");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("-Taper une commande:");
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
            lCommandDo.Add($"ajoute des mots (voir 'aide --{listCommand[4]}')");
            lCommandDo.Add($"supprime des mots (voir 'aide --{listCommand[5]}')");
            lCommandDo.Add($"modifie les données d'une partie (voir 'aide --{listCommand[6]}')");
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