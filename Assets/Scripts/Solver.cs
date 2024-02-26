using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Solver {
    private LetterTrie _trie;
    private List<char> _letters;
    private Board _board;

    public List<string> _legalWords = new List<string>();

    public Solver(LetterTrie trie, List<char> letters, Board board) {
        _trie = trie;
        _letters = letters;
        _board = board;
    }

    public Vector2 MoveLeft(Vector2 position) {
        return new Vector2(position.x + 1, position.y);
    }

    public Vector2 MoveRight(Vector2 position) {
        return new Vector2(position.x - 1, position.y);
    }

    public Vector2 MoveUp(Vector2 position) {
        return new Vector2(position.x, position.y + 1);
    }

    public Vector2 MoveDown(Vector2 position) {
        return new Vector2(position.x, position.y - 1);
    }

    public Tile[] FindAnchors() {
        var anchors = new List<Tile>();
        foreach (var tile in _board.Tiles) {
            var position = tile.Position;
            var empty = _board.IsEmpty(position);
            var neighbors_empty = _board.IsFilled(MoveLeft(position)) || _board.IsFilled(MoveRight(position)) ||
                                  _board.IsFilled(MoveUp(position)) || _board.IsFilled(MoveDown(position));
            if (neighbors_empty && empty) anchors.Add(tile);
        }

        return anchors.ToArray();
    }

    public void LegalMove(string word, Vector2 lastPosition) {
        _legalWords.Add(word);
        
        var boardIfWePlaceWord = _board.Clone();
        var playPosition = lastPosition;
        var letterIndex = word.Length - 1;
        while (letterIndex >= 0) {
            var letter = word[letterIndex];
            boardIfWePlaceWord.SetTile(playPosition, letter);
            playPosition = MoveLeft(playPosition);
            letterIndex--;
        }
        
        Debug.Log(boardIfWePlaceWord);
        
    }

   //public void AllWords(TrieNode currentNode, string partialWord) {
   //    if (currentNode.isWord) {
   //        LegalMove(partialWord, );
   //    }

   //    foreach (var nextLetter in currentNode.children.Keys) {
   //        if (_letters.All(x => x != nextLetter)) continue; // if the letter is not in the player rack, skip it
   //        var nextNode = currentNode.children[nextLetter];
   //        _letters.Remove(nextLetter);
   //        var nextPartialWord = partialWord + nextLetter;
   //        AllWords(nextNode, nextPartialWord);
   //        _letters.Add(nextLetter);
   //    }
   //}
    
    public void ExtendRight(TrieNode currentNode, string partialWord, Vector2 nextPosition) {
        if (currentNode.isWord) {
            LegalMove(partialWord, MoveLeft(nextPosition));
        }

        if(!_board.IsInBounds(nextPosition))
            return;
        
        foreach (var nextLetter in currentNode.children.Keys) {
            if (_letters.All(x => x != nextLetter)) continue; // if the letter is not in the player rack, skip it
            var nextNode = currentNode.children[nextLetter];
            _letters.Remove(nextLetter);
            var nextPartialWord = partialWord + nextLetter;
            ExtendRight(nextNode, nextPartialWord, MoveRight(nextPosition));
            _letters.Add(nextLetter);
        }
    }

    public void FindAllOptions() {
        var anchors = FindAnchors();
        foreach (var anchor in anchors) {
            if (_board.IsFilled(MoveLeft(anchor.Position))) {
                var scanPosition = MoveLeft(anchor.Position);
                var partialWord = _board.GetTile(scanPosition).Letter.ToString();
                while (_board.IsFilled(MoveLeft(scanPosition))) {
                    scanPosition = MoveLeft(scanPosition);
                    partialWord += _board.GetTile(scanPosition).Letter;
                }
                
                var partialWordNode = _trie.LookUp(partialWord);
                if (partialWordNode != null) ExtendRight(partialWordNode, partialWord, anchor.Position);
            }
        }
    }
}