using System.Collections.Generic;
using UnityEngine;

public class Trie : MonoBehaviour
{
    private void Start()
    {
        //Read words from file
        var words = new List<string>();
        var file = Resources.Load<TextAsset>("words");
        var lines = file.text.Split(' ');
        foreach (var line in lines)
        {
            words.Add(line.Trim());
        }
        //Create trie
        var trie = new LetterTrie(words);
        //Test
        var board = new Board(10);  
        
        board.SetTile(new Vector2(3,4), 'L');
        board.SetTile(new Vector2(3,3), 'O');
        board.SetTile(new Vector2(3,2), 'P');
        
        var solver = new Solver(trie, new List<char>{'Á', 'R', 'M'}, board);
        
        solver.FindAllOptions();
        
        foreach (var word in solver._legalWords)
        {
            Debug.Log(word);
        }
        
        
        Debug.Log(board);   
    }
}


public class TrieNode
{
    public bool isWord;
    public Dictionary<char, TrieNode> children;

    public TrieNode(char letter)
    {
        this.isWord = false;
        this.children = new Dictionary<char, TrieNode>();
    }
}

public class LetterTrie
{
    public TrieNode Root;
    public LetterTrie(List<string> words)
    {
        Root = new TrieNode(' ');
        foreach (var word in words)
        {
            AddWord(word);
        }
    }
    public void AddWord(string word)
    {
        var current = Root;
        foreach (var letter in word)
        {
            if (!current.children.ContainsKey(letter))
            {
                current.children.Add(letter, new TrieNode(letter));
            }
            current = current.children[letter];
        }
        current.isWord = true;
    }
    
    public TrieNode LookUp(string word)
    {
        var current = Root;
        foreach (var letter in word)
        {
            if (!current.children.ContainsKey(letter))
            {
                return null;
            }
            current = current.children[letter];
        }
        return current;
    }
    
    public bool IsWord(string word)
    {
        var node = LookUp(word);
        return node != null && node.isWord;
    }
}