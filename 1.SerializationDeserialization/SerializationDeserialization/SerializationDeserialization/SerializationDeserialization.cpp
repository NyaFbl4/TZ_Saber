#include <iostream>
#include <fstream>
#include <string>
#include <unordered_map>

using namespace std;

class ListNode
{
public:
	ListNode* Prev;
	ListNode* Next;
	ListNode* Rand;
	string Data;
};

class ListRand
{
public:
	ListNode* Head;
	ListNode* Tail;
	int Count;

    void Clear();
    void AddNode(const string& data, ListNode* randNode = nullptr);
    void PrintList();

    ~ListRand() { Clear(); }

	void Serialize(ofstream& file)
	{
        file.write(reinterpret_cast<const char*>(&Count), sizeof(Count));

        if (Count == 0) return;

        unordered_map<ListNode*, int> nodeToIndex;
		ListNode* currentNode = Head;

		for (int i = 0; i < Count; i++)
		{
			nodeToIndex[currentNode] = i;
			currentNode = currentNode->Next;
		}

		currentNode = Head;

		while (currentNode != nullptr)
		{
			size_t dataSize = currentNode->Data.size();
			file.write(reinterpret_cast<const char*>(&dataSize), sizeof(dataSize));
			file.write(currentNode->Data.c_str(), dataSize);
		
			int randIndex = (currentNode->Rand != nullptr) ? nodeToIndex[currentNode->Rand] : -1;
			file.write(reinterpret_cast<const char*>(&randIndex), sizeof(randIndex));

			currentNode = currentNode->Next;
		}
	};

    void Deserialize(ifstream& file) 
    {
        Clear();

        file.read(reinterpret_cast<char*>(&Count), sizeof(Count));

        if (Count == 0) return;

        vector<ListNode*> nodes(Count); 
        vector<int> randIndices(Count); 

        for (int i = 0; i < Count; ++i) 
        {
            nodes[i] = new ListNode();

            size_t dataSize;
            file.read(reinterpret_cast<char*>(&dataSize), sizeof(dataSize));
            char* buffer = new char[dataSize + 1];
            file.read(buffer, dataSize);
            buffer[dataSize] = '\0';
            nodes[i]->Data = string(buffer);
            delete[] buffer;

            file.read(reinterpret_cast<char*>(&randIndices[i]), sizeof(int));
        }

        for (int i = 0; i < Count; ++i) 
        {
            if (i > 0) 
            {
                nodes[i]->Prev = nodes[i - 1];
                nodes[i - 1]->Next = nodes[i];
            }

            int randIndex = randIndices[i];
            if (randIndex != -1) 
            {
                nodes[i]->Rand = nodes[randIndex];
            }
        }

        Head = nodes.front();
        Tail = nodes.back();
    }
};

int main()
{

}

