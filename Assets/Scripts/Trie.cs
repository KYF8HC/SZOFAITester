using System.Collections.Generic;
using UnityEngine;

// Class to represent a Trie (not used directly in the provided code)
public class Trie : MonoBehaviour
{
    private void Start()
    {
        // Read words from file
        var words = new List<string>();
        var file = Resources.Load<TextAsset>("words");
        var lines = file.text.Split(' ');
        foreach (var line in lines)
        {
            words.Add(line.Trim());
        }

        // Create trie
        var trie = new LetterTrie(words);
        // Test
        var board = new Board(16);

        board.SetTile(new Vector2(3, 2), "GY");
        board.SetTile(new Vector2(3, 4), "A");
        board.SetTile(new Vector2(8, 2), "R");

        var solver = new Solver(trie, new List<char> { 'Ő', 'A', 'Z' }, board);

        solver.FindAllOptions();

        foreach (var word in solver.LegalWords)
        {
            Debug.Log(word);
        }

        Debug.Log(board);
    }
}

// Class to represent a node in the Trie
public class TrieNode
{
    public bool isWord; // Flag indicating if the path to this node forms a complete word
    public Dictionary<char, TrieNode> children; // Dictionary to store child nodes for each character

    // Constructor for TrieNode
    public TrieNode(char letter)
    {
        this.isWord = false;
        this.children = new Dictionary<char, TrieNode>();
    }
}

// Class to represent a Trie for storing words and looking up their presence
public class LetterTrie
{
    public TrieNode Root; // Root node of the Trie
    public TrieNode Current; // Current node used during Trie operations

    // Constructor for LetterTrie, initializing it with a list of words
    public LetterTrie(List<string> words)
    {
        Root = new TrieNode(' '); // Initialize the root node with a dummy character
        foreach (var word in words)
        {
            AddWord(word); // Add each word to the Trie
        }
    }

    // Method to add a word to the Trie
    public void AddWord(string word)
    {
        var current = Root;
        Current = current;

        var modifiedWord = word;
        for (int i = 0; i < word.Length; i++)
        {
            LetterSubstituter.SubstituteLetterToNumber(word[i], i + 1 < word.Length ? word[i + 1] : ' ', ref modifiedWord);
        }

        if (modifiedWord != word)
        {
            foreach (var letter in modifiedWord)
            {
                current = AddLetterToTree(current, letter);
            }
            current.isWord = true;
        }
        
        current = Root;
        Current = current;
        
        foreach (var letter in word)
        {
            // Add a new node for the letter if it doesn't exist
            current = AddLetterToTree(current, letter);
        }
        current.isWord = true; // Mark the last node as representing a complete word
    }

    private TrieNode AddLetterToTree(TrieNode current, char letter)
    {
        if (!current.children.ContainsKey(letter))
        {
            current.children.Add(letter, new TrieNode(letter));
        }

        current = current.children[letter];
        Current = current;
        return current;
    }

    // Method to look up a word in the Trie and return the corresponding node
    public TrieNode LookUp(string word)
    {
        var current = Root;
        Current = current;
        foreach (var letter in word)
        {
            // If a node for the letter is not found, return null
            if (!current.children.ContainsKey(letter))
            {
                return null;
            }

            current = current.children[letter];
            Current = current;
        }

        return current;
    }

    // Method to check if a given string is a complete word in the Trie
    public bool IsWord(string word)
    {
        var node = LookUp(word);
        return node != null && node.isWord;
    }
}