using System;
using System.IO;
using System.Collections.Generic;
using SymbolTable;
using MyTreeTable;
using System.Diagnostics;
using System.Threading;


public class Program
{
    class MarkovState
    {
        string state;
        int count;
        List<char> suffixList = new List<char>();

        public MarkovState(string state)
        {
            this.state = state;
            this.count = 0;
        }

        public void AddSuffix(char ch)
        {
            count++;
            suffixList.Add(ch);
            //use suffix symbol table to keep track of the counts of each different "ch"
        }

        public char RandomLetter()
        {
            Random rng = new Random();
            int num = rng.Next(suffixList.Count);
            return suffixList[num];
        }
        public override string ToString()
        {
            string output = $"MarkovEntry '{state}' : ";
            foreach (char ch in suffixList)
            {
                output += $"{ch}:{suffixList[ch]}";

            }
            return output;
        }
        public static void Main(string[] args)
        {

            string filename = args[0]; //filename
            if(!int.TryParse(args[1], out int N))// Markov model number
            {
                Console.WriteLine("Error: invalid Markov Model number. Please enter an integer. ");
                return;
            }

            if(!int.TryParse(args[2], out int L))//story length
            {
                Console.WriteLine("Error: invalid story length. Please enter integer.");
                return;
            }

            //Validate Range
            /*if(N < 5 || N < 10)
            {
                Console.WriteLine("Error: story length is out of range");
            }*/

            // Creation of stopwatchs for timing
            Stopwatch symbollisttime = new Stopwatch();
            Stopwatch BSTtime = new Stopwatch();
            Stopwatch dotnettime = new Stopwatch();

            //string filename = "twilight.txt";

            // Reads every line in the file
            StreamReader sr = new StreamReader(filename);
            sr.ReadLine();
            string line = File.ReadAllText(filename);
            sr.Close();

            //get length of source text file
            int textLength = 0;
            foreach (char ch in line)
            {
                textLength++;
            }

            string story1 = line.Substring(0, N);
            string story2 = line.Substring(0, N);
            string story3 = line.Substring(0, N);//initialize to first N letters of source text file

            //SYMBOL TABLE//

            Console.WriteLine(" ");
            Console.WriteLine("Custom Linked List Symbol Table");

            symbollisttime.Start();
            // Call Symbol List Traversal here 
            ListSymbolTable<string, MarkovState> db1 = LSTTraversal(N, line);

            for (int i = 0; i < L; i++)
            {
                string state = story1.Substring(story1.Length - N, N);

                if (!db1.Contains(state))
                {
                    break;
                }
                story1 += db1[state].RandomLetter();
            }
            symbollisttime.Stop();
            Console.WriteLine($"Text Length: {textLength} chars");
            Console.WriteLine($"Elapsed Time: {symbollisttime.ElapsedMilliseconds} milliseconds");
            Console.WriteLine(story1);

            //BINARY SEARCH TREE//

            Console.WriteLine(" ");
            Console.WriteLine("Custom Binary Search Tree Table");

            BSTtime.Start();

            MyTreeTable<string, MarkovState> db2 = BSTTraversal(N, line);
           
            /*foreach(string line in db2)
            {
                db2.Add(, v);
            }*/

            for (int i = 0; i < L; i++)
            {
                string state = story2.Substring(story2.Length - N, N);
                if (!db2.Contains(state))
                {
                    break;
                }
                story2 += db2[state].RandomLetter();
            }

            BSTtime.Stop();
            Console.WriteLine($"Text length: {textLength} chars");
            Console.WriteLine($"Elapsed Time: {BSTtime.ElapsedMilliseconds} milliseconds");
            Console.WriteLine(story2);

            //.NET SORTED DICTIONARY//
            Console.WriteLine(" ");
            Console.WriteLine("Dot Net Sorted Dictionary");

            dotnettime.Start();

            SortedDictionary<string, MarkovState> db3 = STTraversal(N, line);
      
            //foreach (KeyValuePair<string , MarkovState> key in db3)
            //{
            //    Console.WriteLine($"{key.Key}, {key.Value}");
            //}

            for (int i = 0; i < L; i++)
            {
                string state = story3.Substring(story3.Length - N, N);
                if (!db3.ContainsKey(state))
                {
                    break;
                }
                story3 += db3[state].RandomLetter();
            }
            dotnettime.Stop();
            Console.WriteLine($"Text length: {textLength} chars");
            Console.WriteLine($"Elapsed Time: {dotnettime.ElapsedMilliseconds} milliseconds");
            Console.WriteLine(story3);
        }

        public static MyTreeTable<string, MarkovState> BSTTraversal(int n, string line)
        {
            MyTreeTable<string, MarkovState> db2 = new MyTreeTable<string, MarkovState>();
            for (int i = 0; i < line.Length - n; i++)
            {
                string state = line.Substring(i, n);
                char next = line[i + n];
                if (!db2.Contains(state))
                {
                    db2.Add(state, new MarkovState(state));
                }
                db2[state].AddSuffix(next);
            }
            return db2;
        }

        public static ListSymbolTable<string, MarkovState> LSTTraversal(int n, string line)
        {
            // Make a new 'DB' here
            ListSymbolTable<string, MarkovState> db = new ListSymbolTable<string, MarkovState>();
            for (int i = 0; i < line.Length - n; i++)
            {
                string state = line.Substring(i, n);
                char next = line[i + n];
                if (!db.Contains(state))
                {
                    db.Add(state, new MarkovState(state));
                }
                db[state].AddSuffix(next);
            }
  
            return db;
        }

        public static SortedDictionary<string, MarkovState> STTraversal(int n, string line)
        {
            // Make a new 'DB' here
            SortedDictionary<string, MarkovState> db = new SortedDictionary<string, MarkovState>();
            for (int i = 0; i < line.Length - n; i++)
            {
                string state = line.Substring(i, n);
                char next = line[i + n];
                if (!db.ContainsKey(state))
                {
                    db.Add(state, new MarkovState(state));
                }
                db[state].AddSuffix(next);
            }
            return db;
        }
    }
}

