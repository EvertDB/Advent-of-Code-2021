using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GiantSquid
{
    class Program
    {
        public static void Main(string[] args)
        {
            string filename = "";
            Console.Write("Wat is de filename? ");
            filename = Console.ReadLine();

            Console.Write(score(filename));

            Console.ReadLine();
        }

        public static int score(string filename)
        {
            int result = 0;

            List<string> fileContents = File.ReadLines(filename).ToList();

            //List<int> getallen = fileContents[0].Split(',').Select(int.Parse).ToList();
            Queue<int> getallen = new Queue<int>( fileContents[0].Split(',').Select(int.Parse));

            fileContents.RemoveAt(0);

            // Nu zouden enkel de borden moeten overblijven: lege regel, gevolgd door de 5 regels van een bord
            List<Bord> borden = new List<Bord>();
            Queue<string> bordInhoud = new Queue<string>();
            foreach (var item in fileContents)
            {
                bordInhoud.Enqueue(item);
            }
            while (bordInhoud.Count > 0)
            {
                Bord bord = new Bord(5, 5);
                int[,] inhoud = new int[5, 5];
                for (int teller = 0; teller < 6; teller++)
                {
                    if (teller == 0)
                    {
                        // verwijder de blanco regel
                        bordInhoud.Dequeue();
                        //fileContents.RemoveAt(teller);
                    }
                    else
                    {
                        int kIndex = 0;
                        //                        foreach (int getal in fileContents[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse))
                        foreach (int getal in bordInhoud.Dequeue().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse))
                            inhoud[teller - 1, kIndex++] = getal;
                        //fileContents.RemoveAt(0);
                    }
                }
                bord.Fill(inhoud);
                borden.Add(bord);
            }

            // Trek nu de getallen 1 voor 1 tot we een winnaar hebben
            Bord winnendBord = null;
            int getrokkenGetal=0;
            while(getallen.Count>0)
            {
                getrokkenGetal = getallen.Dequeue();
                foreach (var bord in borden)
                {
                    bord.Mark(getrokkenGetal);
                    //winnendBord = GetWinnendBord(borden);
                    if(bord.HasWinner())
                    {
                        winnendBord = bord;
                        break;
                    }
                    //if (winnendBord != null)
                    //    break;
                }

                // Stop met getallen trekken als er een winnaar is.
                if (winnendBord != null)
                    break;
            }

            // Bereken de totaalscore
            if (winnendBord != null)
                result = winnendBord.GetScore() * getrokkenGetal;
            else
                result = 0;

            return result;
        }

        private static  Bord GetWinnendBord(IEnumerable<Bord> borden)
        {
            Bord result = null;

            foreach (var bord in borden)
            {
                if(bord.HasWinner())
                {
                    result = bord;
                    break;
                }
            }

            return result;
        }

    }

    /*
     /// <summary>
     /// BasicBord wordt geïmplementeerd als een dictionary waarbij de key het getal is en de value true/false (gemarkeerd of niet).
     /// </summary>
     public class BasicBoard
     {
         public Dictionary<int,bool> Elements { get; set; }
         public BasicBoard()
         {
             Elements = new Dictionary<int, bool>();
         }

         public void Fill(int[] numbers)
         {
             foreach (var item in numbers)
             {
                 Elements.Add(item, false);
             }
         }

         public void Mark(int number)
         {
             if (Elements.ContainsKey(number))
                 Elements[number] = true;
         }

         /// <summary>
         /// Winner indien volledige rij of kolom gemarkeerd is.
         /// </summary>
         /// <returns>True indien winner; false indien niet.</returns>
         public bool HasWinner()
         {

         }
     }
    */

    public class Coord
    {
        public Coord(int x, int y)
        {
            Rij = x;
            Kolom = y;
        }
        public int Rij { get; set; }
        public int Kolom { get; set; }
        public override string ToString()
        {
            return $"({Rij},{Kolom})";
        }
    }

    public class BordElement
    {
        public int Waarde { get; set; }
        public bool IsGetrokken { get; set; }
        public Coord Positie { get; set; }

        public BordElement(Coord pos, int waarde)
        {
            Positie = pos;
            Waarde = waarde;
            IsGetrokken = false;
        }

        public override string ToString()
        {
            string result;
            if (IsGetrokken)
                result = Waarde.ToString(" *#0");
            else
                result=Waarde.ToString("  #0");
            return result;
        }
    }

    public class Bord
    {
        int maxRijen, maxKolommen,aantalMarks;
        public BordElement[,] Elementen { get; set; }
        public Bord(int rijen, int kolommen)
        {
            maxRijen = rijen;
            maxKolommen = kolommen;
            Elementen = new BordElement[maxRijen, maxKolommen];
            aantalMarks = 0;
        }

        public void Fill(int[,] inhoud)
        {
            if (inhoud.GetLength(0) != maxRijen || inhoud.GetLength(1) != maxKolommen || inhoud.Length != maxRijen * maxKolommen)
                throw new ArgumentException($"Er wordt een {maxRijen}x{maxKolommen} array verwacht.");

            for (int i = 0; i < maxRijen; i++)
                for (int j = 0; j < maxKolommen; j++)
                    Elementen[i, j] = new BordElement(new Coord(i, j), inhoud[i, j]);
        }

        public void Mark(int waarde)
        {
            for (int i = 0; i < maxRijen; i++)
                for (int j = 0; j < maxKolommen; j++)
                {
                    if (Elementen[i, j].Waarde == waarde)
                    {
                        Elementen[i, j].IsGetrokken = true;
                        aantalMarks++;
                    }
                }
        }

        /// <summary>
        /// Winnaar indien een volledige rij of kolom getrokken is.
        /// </summary>
        /// <returns></returns>
        public bool HasWinner()
        {
            bool hasWinner = false;
            int aantalHits;

            // optimalisatie:
            if (aantalMarks < maxRijen)
                return false;

            // check rijen
            for (int rij = 0; rij < maxRijen; rij++)
            {
                aantalHits = 0;

                for (int kolom = 0; kolom < maxKolommen; kolom++)
                    if (Elementen[rij, kolom].IsGetrokken)
                        aantalHits++;
                    else // optimalisatie: stop met zoeken in deze rij indien niet getrokken
                        break;

                // alle kolommen op de rij doorlopen... indien 5 hits, dan hebben we een winnaar.
                if (aantalHits == 5)
                {
                    hasWinner = true;
                    break;
                }
            }

            if (!hasWinner)
                // check kolommen
                for (int kolom = 0; kolom < maxKolommen; kolom++)
                {
                    aantalHits = 0;

                    for (int rij = 0; rij < maxRijen; rij++)
                        if (Elementen[rij, kolom].IsGetrokken)
                            aantalHits++;
                        else // optimalisatie: stop met zoeken in deze rij indien niet getrokken
                            break;

                    // alle rijen in de kolom doorlopen... indien 5 hits, dan hebben we een winnaar.
                    if (aantalHits == 5)
                    {
                        hasWinner = true;
                        break;
                    }
                }

            // Return result
            return hasWinner;
        }

        /// <summary>
        /// Geef de score van dit bord: de som van alle ongemarkeerde getallen.
        /// </summary>
        /// <returns>De score.</returns>
        public int GetScore()
        {
            int score = 0;

            foreach (var item in Elementen)
            {
                if (!item.IsGetrokken)
                    score += item.Waarde;
            }
            return score;
        }

        public override string ToString()
        {
            string result = "";
            for (int i = 0; i < maxRijen; i++)
            {
                for (int j = 0; j < maxKolommen; j++)
                {
                    result += $"{Elementen[i, j]} ";
                }
                result = result.TrimEnd() + Environment.NewLine;
            }

            //verwijder de laatste Enter
            result = result.Substring(0, result.Length - Environment.NewLine.Length);

            return result;
        }
    }
}
