using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WarmupElvishCheatCodes
{
    class Program
    {
        public struct Coords
        {
            public Coords(int x, int y)
            {
                X = x;
                Y = y;
            }
            public int X { get; }
            public int Y { get; }

            public override string ToString() => $"({X}, {Y})";
        }
        enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }
        //class Coord
        //{
        //    public int X { get; set; }
        //    public int Y { get; set; }
        //    public Coord() { X = 0; Y = 0; }
        //    public Coord(int x, int y) { X = x; Y = y; }
        //    public Coord Move(Direction direction)
        //    {
        //        switch (direction)
        //        {
        //            case Direction.Up:
        //                MoveXY(0, 1);
        //                break;
        //            case Direction.Down:
        //                MoveXY(0, -1);
        //                break;
        //            case Direction.Left:
        //                MoveXY(-1, 0);
        //                break;
        //            case Direction.Right:
        //                MoveXY(1, 0);
        //                break;
        //            default:
        //                break;
        //        }
        //        return new Coord(X,Y);
        //    }
        //    public Coord MoveUp() { return MoveXY(0, 1); }
        //    public Coord MoveDown() { return MoveXY(0, -1); }
        //    public Coord MoveLeft() { return MoveXY(-1, 0); }
        //    public Coord MoveRight() { return MoveXY(1, 0); }
        //    private Coord MoveXY(int deltaX, int deltaY)
        //    {
        //        X += deltaX;
        //        Y += deltaY;
        //        return this;
        //    }
        //    public override string ToString()
        //    {
        //        return $"({X},{Y})";
        //    }
        //    public override bool Equals(object obj)
        //    {
        //        if (GetType() != obj.GetType())
        //            return false;
        //        else
        //            return ((Coord)obj).X == X && ((Coord)obj).Y == Y;
        //    }
        //}
        class Grid
        {
            public Grid()
            {
                content = new Dictionary<Coords, char>();
                ActivePosition = new Coords(0, 0);
                IsMarked = false;
            }
            public bool IsMarked;
            private Dictionary<Coords, char> content;
            public Coords ActivePosition;
            public Dictionary<Coords, char> Content
            {
                get { return content; }
                set { content = value; }
            }
            public void Move(Direction direction)
            {
                switch (direction)
                {
                    case Direction.Up:
                        MoveXY(0, 1);
                        break;
                    case Direction.Down:
                        MoveXY(0, -1);
                        break;
                    case Direction.Left:
                        MoveXY(-1, 0);
                        break;
                    case Direction.Right:
                        MoveXY(1, 0);
                        break;
                    default:
                        break;
                }
                IsMarked = false;
                //return new Coords(X, Y);
            }
            private void MoveXY(int deltaX, int deltaY)
            {
                ActivePosition = new Coords(ActivePosition.X + deltaX, ActivePosition.Y + deltaY);
            }

            public void Mark(char marker)
            {
                //if (!content.ContainsKey(Position))
                //    content.Add(Position, marker);
                //else
                content[ActivePosition] = marker;
                IsMarked = true;
            }
            public void Act(string action)
            {
                Direction direction;
                char marker;
                if (Enum.TryParse(action, out direction))
                    Move(direction);
                else if (char.TryParse(action, out marker))
                {
                    Mark(marker);
                }
                else
                {
                    throw new Exception("Invalid input");
                }
            }
            public string ShowCurrent()
            {
                char value;
                if (!content.TryGetValue(ActivePosition, out value))
                    value = ' ';
                return $"{ActivePosition} {value}";
            }
            public string ShowAll()
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var item in content)
                {
                    stringBuilder.AppendLine($"{item.Key} {item.Value}");
                }
                return stringBuilder.ToString();
            }
        }
        static void Main(string[] args)
        {
            string input = File.ReadAllText("Elvish Cheat Codes.txt");
            if (!input.TrimEnd().EndsWith("Start"))
            {
                Console.WriteLine("No start command given.");
            }
            else
            {
                // strip Start from input
                input = input.Remove(input.LastIndexOf("Start")).TrimEnd(new char[] { ' ', ',' });
                Grid grid = new Grid();
                List<Coords> markersA = new List<Coords>(), markersB = new List<Coords>();
                // Compose the grid and show it
                ComposeGrid(ref grid, input);
                Console.ReadLine();
                Console.Clear();
                string title = $"Grid containing {grid.Content.Count} elements:";
                Console.WriteLine(title);
                Console.WriteLine(new string('=', title.Length));
                Console.WriteLine(grid.ShowAll());
                Console.WriteLine();
                // Calculate alle taxicab distances from origin
                Coords origin = new Coords(0, 0), furthestPoint = origin;
                int furthest = -1, currentDistance;
                foreach (var key in grid.Content.Keys)
                {
                    currentDistance = CalculateTaxicabDistance(origin, key);
                    if (currentDistance > furthest)
                    {
                        furthest = currentDistance;
                        furthestPoint = key;
                    }
                    if (grid.Content[key] == 'A')
                        markersA.Add(key);
                    else if (grid.Content[key] == 'B')
                        markersB.Add(key);

                }
                Console.WriteLine($"Point furthest from origin: {furthestPoint} (distance = {furthest}).");
                Console.WriteLine();
                //Console.WriteLine("Press Enter to continue to couple different markers most far apart...");
                //Console.ReadLine();
                // Maximally far apart *different* markers
                furthest = -1;
                Coords furthestA = origin, furthestB = origin;
                foreach (var keyA in markersA)
                {
                    foreach (var keyB in markersB)
                    {
                        currentDistance = CalculateTaxicabDistance(keyA, keyB);
                        if (currentDistance > furthest)
                        {
                            furthest = currentDistance;
                            furthestA = keyA;
                            furthestB = keyB;
                        }
                    }
                }
                Console.WriteLine($"Different markers most far apart: A{furthestA} -> B{furthestB} = {furthest}");
            }

            //Wachten...
            Console.WriteLine();
            Console.Write("Druk op Enter om af te sluiten...");
            Console.ReadLine();
        }

        private static void ComposeGrid(ref Grid grid, string input)
        {
            string[] actions = input.Replace(", ", ",").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var action in actions)
            {
                grid.Act(action);
                if (grid.IsMarked)
                    Console.WriteLine(grid.ShowCurrent());
            }
        }

        private static int CalculateTaxicabDistance(Coords point1, Coords point2)
        {
            return Math.Abs(point1.X - point2.X) + Math.Abs(point1.Y - point2.Y);
        }
    }
}
