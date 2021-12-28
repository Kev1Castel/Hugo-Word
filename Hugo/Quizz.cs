namespace Hugo
{
    class Quizz: LocalFile
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
}