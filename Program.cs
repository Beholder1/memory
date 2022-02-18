using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class MemoryGame
    {
        int counter = 0;
        int prevX = 0;
        int prevY = 0;
        int guessed = 0;
        int uniqueWords;
        int chances;
        int maxChances;
        string difficulty;
        string[,] blanks;
        string[,] memory;
        public MemoryGame(string[] allWords, string difficulty)
        {
            Random randomizer = new Random();
            int rows;
            if (difficulty == "1" || difficulty == "Easy" || difficulty == "easy")
            {
                this.difficulty = "easy";
                rows = 2;
                this.uniqueWords = 4;
                this.chances = 10;
                this.maxChances = 10;
            }
            else
            {
                this.difficulty = "hard";
                rows = 4;
                uniqueWords = 8;
                this.chances = 15;
                this.maxChances = 15;
            }
            this.blanks = new string[rows, 4];
            this.memory = new string[rows, 4];
            List<string> words = new List<string>();
            string word;
            for (int i = 0; i < uniqueWords; i++)
            {
                word = allWords[randomizer.Next(0, allWords.Length)];
                words.Add(word);
                words.Add(word);
            }
            int index;
            for (int i = 0; i < blanks.GetLength(0); i++)
            {
                for (int j = 0; j < blanks.GetLength(1); j++)
                {
                    index = randomizer.Next(0, words.Count);
                    this.blanks[i, j] = "X";
                    this.memory[i, j] = words[index];
                    words.RemoveAt(index);
                }
            }

        }
        /*
        Method that runs the whole game. It asks user for coordinates, prints the board using method PrintBoard after every guess and allows user to guess using method Guess
        @return Information if user  won the game
        */
        public bool StartGame()
        {
            PrintBoard();
            bool isGameWon = false;
            do
            {
                string coordinates = Console.ReadLine();
                if (coordinates.Length>1 && !coordinates.Substring(1, coordinates.Length - 1).Any(x => char.IsLetter(x)) && coordinates.Substring(1, coordinates.Length - 1).Any(x => char.IsLetterOrDigit(x)))
                {
                    int coorX = coordinates[0] % 65;
                    int coorY = Convert.ToInt32(coordinates.Substring(1, coordinates.Length - 1)) - 1;
                    if (coorX < blanks.GetLength(0) && coorX >= 0 && coorY < blanks.GetLength(1) && coorY >= 0)
                    {
                        Guess(memory, coorX, coorY);
                    }
                }
                PrintBoard();
                if (this.guessed == uniqueWords)
                {
                    isGameWon = true;
                    break;
                }
            } while (this.chances > 0);
            return isGameWon;
        }
        /*
        Method calculates how times user guessed in total
        @return Total number of guesses
        */
        public int GetUsedChances()
        {
            return guessed + maxChances - chances;
        }
        /*
        Method that prints the board
        */
        public void PrintBoard()
        {
            Console.Clear();
            Console.WriteLine("Level: " + this.difficulty);
            Console.WriteLine("Guess chances: " + this.chances);
            Console.Write("   ");
            for (int i = 1; i <= this.blanks.GetLength(1); i++)
            {
                Console.Write(String.Format("|{0,-13}", i));
            }
            for (int i = 0; i < this.blanks.GetLength(0); i++)
            {
                Console.Write("\n");
                Console.Write((char)(65 + i) + "  ");
                for (int j = 0; j < this.blanks.GetLength(1); j++)
                {
                    Console.Write(String.Format("|{0,-13}", this.blanks[i, j]));
                }
            }
            Console.Write("\n");
        }
        /*
        Method that checks if user uncovers first or second word in his turn. If first, updates the board with this word.
        If second, it checks if it matches the first word. If yes, it updates board with the second eord. If no, it reverses the two last uncovwrings
        @param words 2-dimensional table that represents words placement in under the board
        @param coorX x coordinate of the board
        @param Y y coordinate of the board
        */
        public void Guess(string[,] words, int coorX, int coorY)
        {
            this.counter++;
            for (int i = 0; i < this.blanks.GetLength(0); i++)
            {
                Console.Write("\n");
                Console.Write((char)(65 + i));
                Console.Write(' ');
                for (int j = 0; j < this.blanks.GetLength(1); j++)
                {
                    if (i == coorX && j == coorY)
                    {
                        if (this.counter == 1)
                        {
                            this.prevX = coorX;
                            this.prevY = coorY;
                            if (this.blanks[i, j] == "X")
                            {
                                this.blanks[i, j] = words[i, j];
                            }
                            else
                            {
                                this.counter = 0;
                            }
                            break;
                        }
                        else
                        {
                            if (words[i, j] == this.blanks[this.prevX, this.prevY] && (this.prevX != coorX || this.prevY != coorY))
                            {
                                this.blanks[i, j] = words[i, j];
                                this.guessed++;
                            }
                            else
                            {
                                this.blanks[i, j] = words[i, j];
                                PrintBoard();
                                System.Threading.Thread.Sleep(3000);
                                this.blanks[i, j] = "X";
                                this.blanks[this.prevX, this.prevY] = "X";
                                this.chances--;
                            }
                            this.counter = 0;
                            break;
                        }
                    }

                }
            }
        }
        /*
        Method that prints the best scores for given difficulty and allows user to add his score, if it is better than any score in leaderboard.
        @param time user's time
        @param score user's score
        */
        public void BestScores(int time, int score)
        {
            string dir;
            if (this.difficulty == "easy")
            {
                dir = System.IO.Directory.GetParent(System.IO.Directory.GetParent(Environment.CurrentDirectory).ToString()).ToString() + @"\BestScoresEasy.txt";
            }
            else
            {
                dir = System.IO.Directory.GetParent(System.IO.Directory.GetParent(Environment.CurrentDirectory).ToString()).ToString() + @"\BestScoresHard.txt";
            }
            int[] times;
            int[] scores;
            List<string> bestScores = new List<string>();
            string name;
            string a;
            if (System.IO.File.Exists(dir))
            {
                bestScores = System.IO.File.ReadAllLines(dir).ToList();
                times = new int[bestScores.Count];
                scores = new int[bestScores.Count];
                Console.WriteLine("\nBest scores at " + this.difficulty + " difficulty:");
                for (int i = 0; i < bestScores.Count; i++)
                {
                    Console.WriteLine(bestScores[i]);
                    a = bestScores[i].Split(',')[2];
                    a = a.Remove(0, 7);
                    times[i] = Convert.ToInt32(a);
                    a = bestScores[i].Split(',')[1];
                    a = a.Remove(0, 15);
                    scores[i] = Convert.ToInt32(a);
                }
                if (scores[scores.Length - 1] > score || (scores[scores.Length - 1] == score && times[times.Length - 1] < time) || scores.Length - 1 < 10)
                {
                    Console.WriteLine("Insert your name:");
                    name = Console.ReadLine();
                    System.IO.FileStream fs = System.IO.File.Create(dir);
                    bool didChange = false;
                    int index;
                    for (int i = 0; i < scores.Length; i++)
                    {
                        index = i + 1;
                        if (!didChange && (scores[i] > score  || (scores[i] == score && times[i] >= time)))
                        {
                            bestScores.Insert(i, index + ". " + name + ", chances used: " + score + ", time: " + time);
                            didChange = true;
                            continue;
                        }
                        if (didChange)
                        {
                            bestScores[i] = index + bestScores[i].Remove(0, 1);
                        }
                    }
                    if (!didChange && bestScores.Count<10)
                    {
                        index = bestScores.Count+1;
                        string tmp = index + ". " + name + ", chances used: " + score + ", time: " + time;
                        bestScores.Add(tmp);
                    }
                    if (bestScores[bestScores.Count-1][0] == '1' && bestScores[bestScores.Count - 1][0] == '0')
                    {
                        bestScores[bestScores.Count - 1] = bestScores.Count + bestScores[bestScores.Count - 1].Remove(0, 2);
                    }
                    else
                    {
                        bestScores[bestScores.Count - 1] = bestScores.Count + bestScores[bestScores.Count - 1].Remove(0, 1);
                    }
                    int numberOfRows = bestScores.Count < 10 ? bestScores.Count : 10;
                    for (int i=0; i<numberOfRows; i++)
                    {
                        Byte[] row = new UTF8Encoding(true).GetBytes(bestScores[i] + "\n");
                        fs.Write(row, 0, row.Length);
                    }
                    fs.Close();
                }
            }
            else
            {
                System.IO.FileStream fs = System.IO.File.Create(dir);
                Console.WriteLine("\nInsert your name:");
                name = Console.ReadLine();
                Byte[] row = new UTF8Encoding(true).GetBytes("1. " + name + ", chances used: " + score + ", time: " + time + "\n");
                fs.Write(row, 0, row.Length);
                fs.Close();
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            string restartInput;
            bool restart;
            string difficulty;
            string dir;
            string[] words;
            bool isGameWon;
            int usedChances;
            System.Diagnostics.Stopwatch stopwatch;
            do
            {
                do
                {
                    Console.WriteLine("Welcome to the game. Choose difficulty:");
                    Console.WriteLine("1 - Easy");
                    Console.WriteLine("2 - Hard");
                    difficulty = Console.ReadLine();
                } while (difficulty != "1" && difficulty != "2" && difficulty != "Easy" && difficulty != "Hard" && difficulty != "easy" && difficulty != "hard");

                restart = false;
                dir = System.IO.Directory.GetParent(System.IO.Directory.GetParent(Environment.CurrentDirectory).ToString()).ToString();
                words = System.IO.File.ReadAllLines(dir + @"\Words.txt");
                MemoryGame m = new MemoryGame(words, difficulty);
                stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();
                isGameWon = m.StartGame();
                stopwatch.Stop();
                usedChances = m.GetUsedChances();
                if (isGameWon)
                {
                    Console.WriteLine("\nYou win!");
                    Console.WriteLine("\nYou solved the memory game after " + usedChances + " chances. It took you " + stopwatch.Elapsed.Seconds + " seconds");
                }
                else
                {
                    Console.WriteLine("\nYou lose!");
                    Console.WriteLine("\nYou solved the memory game after " + usedChances + " chances. It took you " + stopwatch.Elapsed.Seconds + " seconds");
                }
                m.BestScores(stopwatch.Elapsed.Seconds, usedChances);
                Console.WriteLine("Do you want to restart? (y/n)");
                do
                {
                    restartInput = Console.ReadLine();
                    if (restartInput == "Y" || restartInput == "y" || restartInput == "yes" || restartInput == "Yes")
                    {
                        restart = true;
                    }
                } while (restartInput != "Y" && restartInput != "y" && restartInput != "yes" && restartInput != "Yes" && restartInput != "N" && restartInput != "n" && restartInput != "no" && restartInput != "No");
                Console.Clear();
            } while (restart);
        }
    }
}
