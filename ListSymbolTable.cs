using System;
using System.Collections.Generic;
using System.Collections;


namespace SymbolTable
{
    class MarkovState
    {
        SortedDictionary<char, int> suffix = new SortedDictionary<char, int>();
        string state;
        int count;
        List<char> suffixList = new List<char>();

        public MarkovState(string state)
        {
            suffix = new SortedDictionary<char, int>();
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
            foreach (char ch in suffix.Keys)
            {
                output += $"{ch}:{suffix[ch]}";

            }
            return output;
        }
        public static ListSymbolTable<string, MarkovState> LSTTraversal(int n, string[] lines)
        {
            ListSymbolTable<string, MarkovState> db = new ListSymbolTable<string, MarkovState>();
            foreach (string line in lines)
            {
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
               
            }
            return db;
        }

    }
    public class ListSymbolTable<K, V> : IEnumerable where K : IComparable<K>
    {
        // k = key v = value
        internal class Node<K, V>
        {
            public K key;
            public V value;
            public Node<K, V> next;
            public Node(K key, V value = default)
            {
                this.key = key;
                this.value = value;
                this.next = null;
            }

            public override string ToString()
            {
                return $"{key}:{value}";
            }

        } //end of node class

        private Node<K, V> head;
        private int count;

        public int Count { get { return count; } }

        public ListSymbolTable()
        {
            head = null;
            count = 0;
        }

        private Node<K, V> WalkToNode(K key)
        {

            Node<K, V> curr = head;
            while (curr != null)
            {
                if (curr.key.Equals(key))
                {
                    return curr;
                }
                curr = curr.next;
            }
            return null;
        }

        public void Add(K key, V value)
        {
            Node<K, V> curr = WalkToNode(key);
            if (curr == null)
            {
                Node<K, V> node = new Node<K, V>(key, value);
                node.next = head;
                head = node;
                this.count++;
            }
            else
            {
                Console.WriteLine($"A node with key '{key}' already exists in the symbol table");
            }
        }
        //Add from LinkedList


        /*public void SimpleSort(bool ascending = true) //AKA SlowSort
        {

            for (Node<T> curr = head; curr != null; curr = curr.next)
            {
                Node<T> min_node = curr;
                for (Node<T> inner = curr; inner != null; inner = inner.next)
                {
                    if (inner.data.CompareTo(min_node.data) == -1)
                    {
                        min_node = inner;
                    }
                }
                T temp = curr.data;
                curr.data = min_node.data;
                min_node.data = temp;
            }
        }
        */
        public void Remove(K key)
        {
            //special case: removing the head of the list
            if (count > 0 && head.key.Equals(key))
            {
                Node<K, V> toRemove = head;
                head = head.next;
                toRemove.key = default;
                toRemove.value = default;
                toRemove.next = default;
                count--;
                return;
            }

            Node<K, V> prev = head;
            while (prev != null && prev.next != null)
            {
                // if the node in *front* of me contains the search key
                if (prev.next.key.Equals(key))
                {
                    Node<K, V> toRemove = prev.next;
                    prev.next = toRemove.next;
                    toRemove.key = default;
                    toRemove.value = default;
                    toRemove.next = default;
                    count--;
                    return;
                }
            }
        }
        public void Clear()
        {
            head = null;
            count = 0;
        }


        public override string ToString()
        {
            string info = $"MyList ({typeof(K).Name}) : (Count) items";
            Node<K, V> node = head;
            if (Count > 0)
            {
                if (Count == 1)
                {
                    info = $"MyList ({typeof(K).Name}) : {count} item ({node.value})";
                    return info;

                }
            }

            info += $"({node.value}";
            node = node.next;

            int endingCount = Math.Min(5, Count);
            for (int i = 1; i < endingCount; i++)
            {
                info += $", {node.value}";
                node = node.next;

                if (i == endingCount - 1 && Count < 6)
                {
                    info += $")";
                }
            }

            if (Count > 5)
            {
                info += $", .....";
            }
            return info;
        }
        public V this[K key]
        {
            get
            {
                Node<K, V> node = WalkToNode(key);

                if (node == null)
                {
                    string msg = $"Key{key} could not be found in the symbol table";
                    throw new System.Collections.Generic.KeyNotFoundException(msg);
                }
                return node.value;
            }
            set
            {
                Node<K, V> node = WalkToNode(key);
                if (node == null)
                {
                    Add(key, value);
                }
                else
                {
                    node.value = value;
                }
            }
        }

        public bool Contains(K key)
        {
            Node<K, V> node = WalkToNode(key);
            if (node == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public IEnumerator<K> GetEnumerator()
        {
            Node<K, V> node = head;
            while (node != null)
            {
                yield return node.key; //yield picks up where it left off
                node = node.next;
            }
            //updates current item to next item in the queue
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void FastSort()
        {
            FastSort(head);
        }

        private Node<K, V> FastSort(Node<K, V> start)
        {
            //Step 1 - Divide
            if (start == null || start.next == null)
            {
                return start;
            }
            Node<K, V> endof1st = GetHalfwayPoint(start);
            Node<K, V> begOf2nd = endof1st.next;

            endof1st.next = null;

            Node<K, V> LHS = start;
            Node<K, V> RHS = begOf2nd;


            LHS = FastSort(LHS);
            RHS = FastSort(RHS);

            //Step 2 - Conquer

            //Step 3 - Combine
            //LHS = Merge(LHS, RHS);

            return LHS;
        }

        public void InOrder()
        {
            Node<K, V> curr = head;
            while (curr != null)
            {
                curr = curr.next;
            }
        }

        private Node<K, V> GetHalfwayPoint(Node<K, V> start)
        {

            Node<K, V> sw = start;
            Node<K, V> fw = start.next;

            while (fw != null)
            {
                fw = fw.next;
                sw = sw.next;
                if (fw != null)
                {
                    fw = fw.next;
                }
            }
            return sw;
        }
    }
}


