using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Solver
{
    public enum SolverType
    {
        Horizontal,
        Vertical
    }

    private char[] hungarianAlphabet =
    {
        'A', 'Á', 'B', 'C', 'C', 'D', 'E', 'É',
        'F', 'G', 'H', 'I', 'Í', 'J', 'K', 'L',
        'M', 'N', 'O', 'Ó', 'Ö', 'Ő', 'P', 'Q',
        'R', 'S', 'T', 'U', 'Ú', 'Ü', 'Ű', 'V',
        'W', 'X', 'Y', 'Z', '0', '1', '2', '3',
        '4', '5', '6'
    };

    private LetterTrie _trie; // Trie data structure for storing valid words
    private List<char> _letters; // List of available letters for the solver
    private Board _board; // Game board

    private Dictionary<Vector2, List<char>>
        _crossChecks = new Dictionary<Vector2, List<char>>(); // Cross-checks for each board position

    private SolverType _type; // Current solver type (Horizontal or Vertical)

    // List to store legal words formed during the solving process
    public List<string> LegalWords = new List<string>();

    // Constructor for the Solver class
    public Solver(LetterTrie trie, List<char> letters, Board board)
    {
        _trie = trie;
        _letters = letters;
        _board = board;
    }

    // Helper method to get the position before the specified position based on solver type
    public Vector2 Before(Vector2 position)
    {
        return _type == SolverType.Horizontal
            ? new Vector2(position.x - 1, position.y)
            : new Vector2(position.x, position.y - 1);
    }

    // Helper method to get the position after the specified position based on solver type
    public Vector2 After(Vector2 position)
    {
        return _type == SolverType.Horizontal
            ? new Vector2(position.x + 1, position.y)
            : new Vector2(position.x, position.y + 1);
    }

    // Helper method to get the cross-check position before the specified position based on solver type
    public Vector2 BeforeCross(Vector2 position)
    {
        return _type == SolverType.Horizontal
            ? new Vector2(position.x, position.y - 1)
            : new Vector2(position.x - 1, position.y);
    }

    // Helper method to get the cross-check position after the specified position based on solver type
    public Vector2 AfterCross(Vector2 position)
    {
        return _type == SolverType.Horizontal
            ? new Vector2(position.x, position.y + 1)
            : new Vector2(position.x + 1, position.y);
    }

    // Method to handle legal moves and update the board accordingly
    public void LegalMove(string word, Vector2 lastPosition)
    {
        LegalWords.Add(word);

        Debug.Log(word);

        // Clone the board to simulate the placement of the word
        var boardIfWePlaceWord = _board.Clone();
        var playPosition = lastPosition;
        var letterIndex = word.Length - 1;

        // Place the word on the cloned board
        while (letterIndex >= 0)
        {
            var letter = word[letterIndex];
            string displayLetter = letter.ToString();
            LetterSubstituter.SubstituteNumberToLetter(letter, ref displayLetter);
            boardIfWePlaceWord.SetTile(playPosition, displayLetter);
            letterIndex--;
            playPosition = Before(playPosition);
        }

        // Log the resulting board after placing the word
        Debug.Log(boardIfWePlaceWord);
    }

    // Method to perform cross-checks on the current board
    public Dictionary<Vector2, List<char>> CrossCheck()
    {
        var crossChecks = new Dictionary<Vector2, List<char>>();
        foreach (var tile in _board.Tiles)
        {
            // Skip filled positions
            if (_board.IsFilled(tile.Position)) continue;

            // Scan letters before the current position
            var lettersBefore = "";
            var scanPosition = tile.Position;
            while (_board.IsFilled(BeforeCross(scanPosition)))
            {
                scanPosition = BeforeCross(scanPosition);
                lettersBefore = _board.GetTile(scanPosition).Letter + lettersBefore;
            }

            // Scan letters after the current position
            var lettersAfter = "";
            scanPosition = tile.Position;
            while (_board.IsFilled(AfterCross(scanPosition)))
            {
                scanPosition = AfterCross(scanPosition);
                lettersAfter += _board.GetTile(scanPosition).Letter;
            }

            // Determine cross-checks based on the presence of letters before and after
            if (lettersBefore.Length == 0 && lettersAfter.Length == 0)
            {
                crossChecks.Add(tile.Position, hungarianAlphabet.ToList());
            }
            else
            {
                var crossCheck = new List<char>();
                foreach (var letter in hungarianAlphabet)
                {
                    var word = lettersBefore + letter + lettersAfter;

                    // Check if the formed word is a valid word in the trie
                    if (_trie.IsWord(word)) crossCheck.Add(letter);
                }

                crossChecks.Add(tile.Position, crossCheck);
            }
        }

        return crossChecks;
    }

    // Method to find anchor tiles on the board
    public Tile[] FindAnchors()
    {
        var anchors = new List<Tile>();
        foreach (var tile in _board.Tiles)
        {
            var position = tile.Position;
            var empty = _board.IsEmpty(position);
            var neighborsEmpty = _board.IsFilled(Before(position)) || _board.IsFilled(After(position)) ||
                                 _board.IsFilled(BeforeCross(position)) || _board.IsFilled(AfterCross(position));

            if (neighborsEmpty && empty)
                anchors.Add(tile);
        }

        return anchors.ToArray();
    }

    // Method to handle partial words before an anchor position
    public void BeforePart(TrieNode currentNode, string partialWord, Vector2 anchorPosition, int limit = 0)
    {
        // Extend after the partial word
        ExtendAfter(currentNode, partialWord, anchorPosition, false);

        // Recursive exploration of possible next letters with a limit
        if (limit > 0)
        {
            foreach (var nextLetter in currentNode.children.Keys)
            {
                if (_letters.Any(x => x == nextLetter))
                {
                    _letters.Remove(nextLetter);

                    var nextNode = currentNode.children[nextLetter];
                    var nextPartialWord = partialWord + nextLetter;


                    BeforePart(nextNode, nextPartialWord, anchorPosition, limit - 1);

                    _letters.Add(nextLetter);
                }
            }
        }
    }

    // Method to extend words after an anchor position
    public void ExtendAfter(TrieNode currentNode, string partialWord, Vector2 nextPosition, bool isAnchorFilled)
    {
        if (partialWord.Length > 1)
        {
            for (int i = 0; i < partialWord.Length; i++)
            {
                LetterSubstituter.SubstituteLetterToNumber(partialWord[i], i + 1 < partialWord.Length? partialWord[i + 1] : ' ', ref partialWord);
            }
        }

        // Check if a word can be formed by extending after the anchor position
        if (!_board.IsFilled(nextPosition) && currentNode.isWord && isAnchorFilled)
        {
            LegalMove(partialWord, Before(nextPosition));
        }

        // Continue extending if within the bounds of the board
        if (_board.IsInBounds(nextPosition))
        {
            if (_board.IsEmpty(nextPosition))
            {
                // Extend for each possible next letter if it is within cross-check limits
                foreach (var nextLetter in currentNode.children.Keys)
                {
                    if (_letters.Any(x => x == nextLetter) && _crossChecks[nextPosition].Contains(nextLetter))
                    {
                        _letters.Remove(nextLetter);

                        var nextNode = currentNode.children[nextLetter];
                        var nextPartialWord = partialWord + nextLetter;

                        ExtendAfter(nextNode, nextPartialWord, After(nextPosition), true);

                        _letters.Add(nextLetter);
                    }
                }
            }
            else
            {
                // Continue with the existing letter on the board
                var existingTile = _board.GetTile(nextPosition);
                var existingLetter = existingTile.Letter;
                if (existingTile.Letter.Length > 1)
                {
                    LetterSubstituter.SubstituteLetterToNumber(existingLetter[0], existingLetter[1],
                        ref existingLetter);
                }

                if (currentNode.children.ContainsKey(existingLetter[0]))
                {
                    ExtendAfter(currentNode.children[existingLetter[0]], partialWord + existingLetter,
                        After(nextPosition), true);
                }
            }
        }
    }

    // Method to find all possible word options on the board
    public void FindAllOptions()
    {
        // Iterate over both solver types (Horizontal and Vertical)
        foreach (var type in Enum.GetValues(typeof(SolverType)))
        {
            _type = (SolverType)type;

            // Find anchor tiles for the current solver type
            var anchors = FindAnchors();
            _crossChecks = CrossCheck();

            // Explore options for each anchor
            foreach (var anchor in anchors)
            {
                if (_board.IsFilled(Before(anchor.Position)))
                {
                    // Handle partial words if there is a filled position before the anchor
                    var scanPosition = Before(anchor.Position);
                    var partialWord = _board.GetTile(scanPosition).Letter;

                    while (_board.IsFilled(Before(scanPosition)))
                    {
                        scanPosition = Before(scanPosition);
                        partialWord = _board.GetTile(scanPosition).Letter + partialWord;
                    }

                    // Look up the trie for the partial word
                    var partialWordNode = _trie.LookUp(partialWord);
                    if (partialWordNode != null) ExtendAfter(partialWordNode, partialWord, anchor.Position, false);
                }
                else
                {
                    // Handle partial words before the anchor with a limit
                    var limit = 0;
                    var scanPosition = anchor.Position;

                    while (_board.IsEmpty(Before(scanPosition)) &&
                           !anchors.Contains(_board.GetTile(Before(scanPosition))))
                    {
                        scanPosition = Before(scanPosition);
                        limit++;
                    }

                    BeforePart(_trie.Root, "", anchor.Position, limit);
                }
            }
        }
    }
}