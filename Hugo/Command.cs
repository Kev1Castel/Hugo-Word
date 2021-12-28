using Microsoft.Win32.SafeHandles;
using System.Data.Common;
using System;


namespace Hugo
{
    public class Command
    {
        public Dictionary<string, bool> commandeInformation = new Dictionary<string, bool>();
        private List<string> listWrongCharactersFoundt = new List<string>();

        public Command()
        {
            SetCommandeInformation();
        }

        private void SetCommandeInformation(){
            /*
                This method is for initializing the dictionnary for command
            */
            if (commandeInformation.Count>0) commandeInformation.Clear();
            commandeInformation.Add("isBracketOpen", false);
            commandeInformation.Add("isBracketClose", false);
            commandeInformation.Add("isBracketsNumberCorrect", false);
            commandeInformation.Add("isParentheseOpen", false);
            commandeInformation.Add("isParentheseClose", false);
            commandeInformation.Add("isParenthesesNumberCorrect", false);
            commandeInformation.Add("isArgumentNotEmpty", false);
            commandeInformation.Add("doesArgumentNotContainsWrongCharacter", true);

        }

        public void ChangeCommandeInformation(string command)
        {
            /*
                This function explore the commande wich was inputed and
                    check if it's all right:
                It needs to contain:
                    cracket (opened and closed)
                    parenthese (opened and closed)
                    an string argument wich is not empty during the running time 
                
                Take args as:
                    string command wich is the input by the user
            */
            SetCommandeInformation();
            int counterOpenParenthese = 0;
            int counterCloseParenthese = 0;
            int counterBrackets = 0;
            string argument = "";
            bool doesWrongCharacterFoundt = false;
            command = command.Replace("ajouter", "");
            foreach(char character in command)
            {
                string stringChar = character.ToString();
                doesWrongCharacterFoundt = GetDoesWrongCharacterWasFoundt(stringChar);
                if (!doesWrongCharacterFoundt){
                    if (stringChar == "(")
                    {
                        counterOpenParenthese++;
                        commandeInformation["isParentheseOpen"] = true;

                    } else if (stringChar == ")")
                    {
                        counterCloseParenthese++;
                        commandeInformation["isParentheseClose"] = true;
                    } else if (stringChar=="\"")
                    {
                        counterBrackets++;
                        if (counterBrackets == 2)
                        {
                            commandeInformation["isBracketClose"] = true;
                        } else if (counterBrackets == 1)
                        {
                            commandeInformation["isBracketOpen"] = true;
                        }
                    } else if (commandeInformation["isBracketOpen"]) {
                        argument += stringChar;
                    }
                } else {
                    commandeInformation["doesArgumentNotContainsWrongCharacter"] = false;
                }
            }
            
            commandeInformation["isBracketsNumberCorrect"] = (counterBrackets == 2);
            commandeInformation["isParenthesesNumberCorrect"] = (counterCloseParenthese == 1 && counterOpenParenthese == 1);
            commandeInformation["isArgumentNotEmpty"] = (argument.Length > 0);

        }

        private bool GetDoesWrongCharacterWasFoundt(string character)
        {
            /*
                This method is used for checking if the string argument contains
                    wrong character; Wich is an test with an list that contains every
                    wrong characters
                
                Take as arg:
                    string character is the character to test
                Return:
                    bool doesWrongCharacterWasFoundt wich is the result of the test
            */
            List<string> wrongCharacter = new List<string>();
            wrongCharacter.Add("!"); wrongCharacter.Add("@");
            wrongCharacter.Add("'"); wrongCharacter.Add("`");
            wrongCharacter.Add("&"); wrongCharacter.Add("~");
            wrongCharacter.Add("<"); wrongCharacter.Add(">");
            wrongCharacter.Add("?"); wrongCharacter.Add(".");
            wrongCharacter.Add(","); wrongCharacter.Add(";");
            wrongCharacter.Add(":"); wrongCharacter.Add("/");
            wrongCharacter.Add("§"); wrongCharacter.Add("%");
            wrongCharacter.Add("µ"); wrongCharacter.Add("*");
            wrongCharacter.Add("$"); wrongCharacter.Add("¤");
            wrongCharacter.Add("£"); wrongCharacter.Add("^");
            wrongCharacter.Add("¨"); wrongCharacter.Add("=");
            wrongCharacter.Add("+"); wrongCharacter.Add("}");
            wrongCharacter.Add("{"); wrongCharacter.Add("#");
            wrongCharacter.Add("|"); wrongCharacter.Add("_");
            wrongCharacter.Add(@"\"); wrongCharacter.Add("@");
            wrongCharacter.Add("°"); wrongCharacter.Add("+");

            bool doesWrongCharacterWasFoundt = (wrongCharacter.Contains(character));
            if (doesWrongCharacterWasFoundt)
            {
                listWrongCharactersFoundt.Add(character);
            }
            return doesWrongCharacterWasFoundt;
        }

        public bool GetIsCommandRight()
        {
            /*
                This method is for checking if all value in the dictionnary (commandeInformation)
                    are all with the true statement
                If it's true so the command was right
                If it's not so the command isn't all right and message is sended
            */
            bool isCommandRight = true;
            foreach(bool isRight in commandeInformation.Values)
            {
                if (isRight is false)
                {
                    isCommandRight = false;
                }
            }
            return isCommandRight;
        }

        public string GetError()
        {
            /*
                This method return the error wich was in the command

                Return:
                    string messageError 
            */
            string messageError = "";
            if (!commandeInformation["isBracketsNumberCorrect"])
            {
                messageError = "Le message ne contient pas assez de \"\"\" ou trop de \"\"\"";
            } else if (!commandeInformation["isParenthesesNumberCorrect"]){
                messageError = "Le message ne contient pas assez de \"(\" ou \")\" ou trop de \"(\" ou \")\"";
            } else if (!commandeInformation["isBracketOpen"])
            {
                messageError = "Vous n'avez pas ouvert la parenthèse!";
            } else if (!commandeInformation["isBracketClose"])
            {
                messageError = "Vous n'avez pas fermer la parenthèse";
            } else if (!commandeInformation["isArgumentNotEmpty"])
            {
                messageError = "Vous n'avez saisit d'argument";

            } else if (!commandeInformation["doesArgumentNotContainsWrongCharacter"])
            {
                messageError = "Votre message contient un mauvais charactère, c'est pourquoi il n'est pas prit en compte... comme:";
                foreach(string error in listWrongCharactersFoundt)
                {
                    messageError += $"\r\n\t{error}";
                }
            }
            messageError += "\r\n\rVeillez à utiliser la commande comme ceci 'ajouter(\"monMotAccentuer\")'"; 
            return messageError;
        }

    }
}