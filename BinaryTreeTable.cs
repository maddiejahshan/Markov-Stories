using System;
using System.Collections.Generic;
using System.Collections;


namespace MyTreeTable
{

    public class MyTreeTable<K , V> : IEnumerable<K> where K : IComparable<K>
    {
        public class Node<K, V>
        {
            public K key;
            public V value;
            public Node<K, V> R;
            public Node<K, V> L;
            public int count;

            public Node(K key, V value = default)
            {
                this.key = key;
                this.value = value;
                this.L = null;
                this.R = null;
                this.count = 1;
            }

            public override string ToString()
            {
                string Lchild = "NULL";
                string Rchild = "NULL";
                if (L != null)
                {
                    Lchild = $"{L.key}";
                }
                return $"{Lchild} <- {key} : {value} ->{Rchild}";

                if (L != null)
                {
                    Rchild = $"{R.key}";
                }
                return $"{Rchild} <- {key} : {value} ->{Lchild}";
            }
        }


        public Node<K, V> root;

        public MyTreeTable()
        {
            root = null;
        }
        public int Count
        {
            get
            {
                if(root == null)
                {
                    return 0;
                }
                else
                {
                    return root.count;
                }
            }
        }

        public void Add(K key, V value)
        {
            root = Add(key, value, root);
        }

        private Node<K ,V> Add(K key, V value, Node<K , V> subroot)
        {
            //first node in our tree
            if(subroot == null)
            {
                return new Node<K, V>(key, value);
            }
            int compare = key.CompareTo(subroot.key);

            if(compare == -1)  //key was less than the current subroot
            {
                subroot.L = Add(key, value, subroot.L);
            }
            else if(compare == +1) //key was greater than the current subroot
            {
                subroot.R = Add(key, value, subroot.R);
            }
            else
            {
                throw new ArgumentException($"A node with key '{key}' already exists in the symbol table");
            }
            subroot.count++;
            return subroot;
        }
        public K Max()
        {
            return Max(root).key;
        }

        private Node<K ,V> Max(Node<K, V> subroot)
        {
            //check if we can move right
            // YES -> move right(recursive)
            //NO -> return subroot
            /*if (subroot.R != null)
            {
                return Max(subroot.R);
            }
            else
            {
                return subroot.key;
            }*/
            while (subroot.R != null)
            {
                subroot = subroot.R;
            }
            return subroot; 
        }

        private Node<K, V> WalkToNode(K nodeKey, Node<K, V> subroot)
        {
            if (subroot == null)
            {
                return null;
            }
            int compare = nodeKey.CompareTo(subroot.key);
            if (compare == -1)
            {
                return WalkToNode(nodeKey, subroot.L);
            }
            else if (compare == +1)
            {
                return WalkToNode(nodeKey, subroot.R);
            }
            else
            {
                return subroot;
            }
        }
        public K Predecessor(K fromKey)
        {
            Node<K, V> curr = WalkToNode(fromKey, root);
            if (curr == null)
            {
                throw new ArgumentException($"Error: '{fromKey}' does not exist");
            }
            else if (curr.L == null)
            {
                throw new InvalidOperationException($"Error: '{fromKey}' does not have a predecessor");
            }
            else
            {
                Node<K, V> replacement = Max(curr.L);
                return replacement.key;
            }
        }
        // do min^
        private K Min(Node<K, V> subroot)
        {
            //check if we can move right
            // YES -> move right(recursive)
            //NO -> return subroot
            if (subroot.L != null)
            {
                return Min(subroot.L);
            }
            else
            {
                return subroot.key;
            }
            while (subroot.L != null)
            {
                subroot = subroot.L;
            }
            return subroot.key;
        }

        public K Successor(K fromKey)
        {
            Node<K, V> curr = WalkToNode(fromKey, root);
            if (curr == null)
            {
                throw new ArgumentException($"Error: '{fromKey}' does not exist");
            }
            else if (curr.R == null)
            {
                throw new InvalidOperationException($"Error: '{fromKey}' does not have a predecessor");
            }
            else
            {
                return Min(curr.R);
            }
        }

        public void PrintKeysInOrder()
        {
            PrintKeysInOrder(root);
        }

        private void PrintKeysInOrder(Node<K, V> subroot)
        {
            if (subroot != null)
            {
                PrintKeysInOrder(subroot.L);
                Console.WriteLine(subroot.key);
                PrintKeysInOrder(subroot.R);
            }
        }
        public void Remove(K key)
        {
            //taken from ListSymbolTable
            Node<K, V> prev = root;
            while (prev.L.R != null)
            {
                if (prev.L.R.key.Equals(key))
                {
                    Node<K, V> toRemove = prev.L.R;
                    prev.L.R = toRemove.L.R;
                    toRemove.key = default;
                    toRemove.value = default;
                    toRemove.L.R = default;
                    return;
                }
            }

        }
        private IEnumerable<K> GetEnumerator(Node<K , V> subroot)
        {
            if(subroot != null)
            {
                foreach (K key in GetEnumerator(subroot.L)) yield return key;
                yield return subroot.key;
                foreach (K key in GetEnumerator (subroot.R)) yield return key;
            }
        }
        public IEnumerator<K> GetEnumerator()
        {
            foreach(K key in GetEnumerator(root))
            {
                yield return key;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public V this[K key]
        {
            get
            {
                Node<K, V> node = WalkToNode(key, root);

                if (node == null)
                {
                    string msg = $"Key '{key}' could not be found in the symbol table";
                    throw new System.Collections.Generic.KeyNotFoundException(msg);
                }
                return node.value;
            }
            set
            {
                Node<K, V> node = WalkToNode(key, root);
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
            Node<K, V> node = WalkToNode(key, root);
            if (node == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
