using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerializationDeserialization
{

    public class ListNode
    {
        public ListNode Prev;
        public ListNode Next;
        public ListNode Rand;
        public string Data;
    }

    public class ListRand
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;

        public void Serialize(FileStream file)
        {
            BinaryWriter writer = new BinaryWriter(file);
            writer.Write(Count);
            
            if (Count == 0)
            {
                return;
            }

            Dictionary<ListNode, int> nodeToIndex = new Dictionary<ListNode, int>();
            ListNode currentNode = Head;

            for (int i = 0; i < Count; i++)
            {
                nodeToIndex.Add(currentNode, i);
                currentNode = currentNode.Next;
            }

            currentNode = Head;
            while (currentNode != null)
            {
                writer.Write(currentNode.Data);
                writer.Write(currentNode.Rand != null ? nodeToIndex[currentNode.Rand] : -1);
                currentNode = currentNode.Next;
            }
        }

        public void Deserialize(FileStream file)
        {
            BinaryReader reader = new BinaryReader(file);
            Count = reader.ReadInt32();
            
            if (Count == 0)
            {
                Head = null;
                Tail = null;
                return;
            }

            List<ListNode> nodes = new List<ListNode>(Count);
            List<int> randIndexes = new List<int>(Count);

            for (int i = 0; i < Count; i++)
            {
                nodes.Add( new ListNode
                {
                    Data = reader.ReadString()
                });
                
                randIndexes.Add(reader.ReadInt32());
            }

            for (int i = 0; i < Count; i++)
            {
                if (i > 0)
                {
                    nodes[i].Prev = nodes[i - 1];
                    nodes[i - 1].Next = nodes[i];
                }

                int randIndex = randIndexes[i];
                if (randIndex != -1)
                {
                    nodes[i].Rand = nodes[randIndex];
                }
            }
            
            Head = nodes[0];
            Tail = nodes[Count - 1];
        }
        
        public void AddNode(string data, ListNode randNode = null)
        {
            ListNode newNode = new ListNode
            {
                Data = data,
                Rand = randNode
            };

            if (Head == null)
            {
                Head = Tail = newNode;
            }
            else
            {
                Tail.Next = newNode;
                newNode.Prev = Tail;
                Tail = newNode;
            }
            Count++;
        }

        public void PrintList()
        {
            Console.WriteLine("List (Count: " + Count);
            ListNode current = Head;
            while (current != null)
            {
                Console.Write("Node: " + current.Data);
                if (current.Rand != null)
                {
                    Console.Write(" -> Rand: "+ current.Rand.Data);
                }
                Console.WriteLine();
                current = current.Next;
            }
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            // Создаем тестовый список
            ListRand list = new ListRand();
            list.AddNode("First");
            list.AddNode("Second");
            list.AddNode("Third");
            list.AddNode("Fourth");

            // Устанавливаем произвольные ссылки
            list.Head.Rand = list.Tail;            // First -> Fourth
            list.Head.Next.Rand = list.Head;       // Second -> First
            list.Tail.Prev.Rand = list.Tail;       // Third -> Fourth

            Console.WriteLine("Original list:");
            list.PrintList();

            // Сериализуем список в файл
            string filePath = "list.dat";
            var serializeFile = new FileStream(filePath, FileMode.Create);
            list.Serialize(serializeFile);
            serializeFile.Close();
            Console.WriteLine("List serialized to file.");

            // Десериализуем список из файла
            ListRand deserializedList = new ListRand();
            var deserializeFile = new FileStream(filePath, FileMode.Open);
            deserializedList.Deserialize(deserializeFile);
            deserializeFile.Close();

            Console.WriteLine("Deserialized list:");
            deserializedList.PrintList();
        }
    }
}
