using System.IO;
using System.Xml;

class Quizz: File
{
    /*
    This class is inherit from the Program class.
    This class is for work with QuizzElement like the 
    user point, question, etc! 
    */
    public bool doesUserPlay = false;
    public string word;
    public string answer;
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
            Console.WriteLine($"'stop' pour arrêter; Voici les données de la partie:\r\n\r\tQuestions gagnaientt:{this.pointWon} | Questions perduent:{this.pointLoss}\r\nRéecriver \"{this.word}\" avec ses accents:");
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

}


class Program
{
    /*
    Main class (Parent) generate by "dotnet command line"
    */
    static List<string> listWord = new List<string>();
    static List<string> listWordNodeName = new List<string>();
    
    // static Dictionary<string, string> dictWord = new Dictionary<string, string>();

    static void Main(string[] args)
    {
        /*
        This method is the main of this class and maybe of the program (Console).
        */

        Console.Clear();
        string command = "ouvert";

        List<string> listCommand = new List<string>();
        string environnementPath = Directory.GetCurrentDirectory();

        File file = new File();
        listCommand = file.ParseXMLFileToList(path:environnementPath+@"\ressource\commandes.xml", xPath:@"commande/*");
        listWordNodeName = file.ParseXMLFileToList(path:environnementPath+@"\ressource\word.xml", xPath:@"word/*", replaceIterTextByName:true);

        Quizz quizz = new Quizz();
        List<string> lTest = new List<string>();
        quizz.GetDictWord(file.ParseXMLFileToList(path:environnementPath+@"\ressource\word.xml", xPath:@"word/*"), file.ParseXMLFileToList(path:environnementPath+@"\ressource\word.xml", xPath:@"word/*", true));
        
        while (command != listCommand[0])
        {
            if (quizz.doesUserPlay == false)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"--Application démarrer ici:'{environnementPath}'\r\n-Taper une commande:");
                Console.ForegroundColor = ConsoleColor.White;
            } else {
                Console.ForegroundColor = ConsoleColor.Blue;
            }
            command = Console.ReadLine();
            if (listCommand.Contains(command))
            {
                //Console Commande
                Console.ForegroundColor = ConsoleColor.Blue;
                if (command == listCommand[1])
                {
                    listCommand.ForEach(Console.WriteLine);
                } else if (command == listCommand[2])
                {
                    quizz.StartGame();
                    
                } else if (command == listCommand[3])
                {
                    quizz.StopGame();
                }
            } else if (quizz.doesUserPlay){
                //Play Commande
                Console.WriteLine(quizz.answer);
                quizz.CheckAnswer(command);
                if (command == quizz.answer)
                {
                    Console.WriteLine("Bravo!");
                } else {
                    Console.WriteLine("Oops!");
                }
                Console.ReadLine();
                Console.Clear();
            } else if (command.Length > 0) {    
                //Console Commande Error               
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Désolé mais la commande semble incorrecte! \r\nSi le probléme persiste taper 'aide'\r\nAppuyer sur une touche pour continuer.");
                Console.ReadKey();
                Console.Clear();
        } else {
                Console.Clear();
            }
        }
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Fermeture de l'app.");
    }

    static void SetEncodedWord()
    {
        List<string> listC = new List<string>();
        
    }

}