using System.Collections.Generic;
using System.Linq;

public class Solver
{
    private LetterTrie _trie;
    private List<char> _letters;

    public List<string> _legalWords = new List<string>();

    public Solver(LetterTrie trie, List<char> letters)
    {
        _trie = trie;
        _letters = letters;
    }

    public void LegalMove(string word)
    {
        _legalWords.Add(word);
    }

    public void AllWords(TrieNode currentNode, string partialWord)
    {
        if (currentNode.isWord)
        {
            LegalMove(partialWord);
        }

        foreach (var nextLetter in currentNode.children.Keys)
        {
            if (_letters.All(x => x != nextLetter)) continue; // if the letter is not in the player rack, skip it
            var nextNode = currentNode.children[nextLetter];
            _letters.Remove(nextLetter);
            var nextPartialWord = partialWord + nextLetter;
            AllWords(nextNode, nextPartialWord);
            _letters.Add(nextLetter);
        }
    }

    public void FindAllOptions()
    {
        AllWords(_trie.Root, "");
    }
}