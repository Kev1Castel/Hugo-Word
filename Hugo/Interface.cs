using System.Drawing;
using System.Threading;

namespace Hugo
{
    public class Interface
    {
        
        public void LoadingLine(int maxNode, int counterNode)
        {
            /*
                Return the line to show for the loading effect

                Take arg as:
                    int maxNode wich is the maximum of node in the xml file
                    int counterNode wich is the actual index of node loaded.
            */
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Blue;
            string line = "Chargement:\r\n\r\t|";
            string loadingCharacter = "X";
            int firstIndex;
            for(firstIndex = 1; firstIndex <= counterNode; firstIndex++)
            {
                ChangeColorLoading(actualNodeLoaded:counterNode, maxNode:maxNode);
                line += loadingCharacter;
            }
            for(int index=firstIndex; index <= maxNode; index++)
            {
                line += " ";
            }
            // Console.ForegroundColor = ConsoleColor.Blue;
            line += "|";
            Console.WriteLine(line);
            Thread.Sleep(15);
        }
        
        private void ChangeColorLoading(float actualNodeLoaded, float maxNode)
        {
            /*
                Set an color for the loading, it's using pourcentage
                    if the actualNodeLoaded is under than 30% so the color is red
                    if the actualNodeLoaded is between 33% & 63% so the color is orange
                    if the actualNodeLoaded is higher than 63% so the color is green
            */
            float pourcentage = 0.0f;
            pourcentage = (actualNodeLoaded/maxNode) * 100;
            if (pourcentage < 33)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
            } else if (pourcentage > 33 && pourcentage < 66)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            } else {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            

        }
    }
}