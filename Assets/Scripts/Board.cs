using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Class representing a single tile on the board
public class Tile {
    public Vector2 Position; // Position of the tile on the board
    public bool IsEmpty; // Flag indicating whether the tile is empty
    public string Letter; // Letter on the tile
}

// Class representing the game board
public class Board {
    private List<Tile> _tiles; // List of tiles on the board
    
    // Public property to access the list of tiles
    public List<Tile> Tiles => _tiles;

    public int size = 0; // Size of the board (assuming a square board)

    // Constructor to initialize the board with empty tiles
    public Board(int size) {
        this.size = size;
        _tiles = new List<Tile>();
        for (var i = 0; i < size; i++) {
            for (var j = 0; j < size; j++) {
                _tiles.Add(new Tile {
                    Position = new Vector2(i, j),
                    IsEmpty = true,
                    Letter = ""
                });
            }
        }
    }

    // Method to create a deep copy (clone) of the board
    public Board Clone() {
        var clone = new Board(size);
        for (int i = 0; i < _tiles.Count(); i++) {
            clone._tiles[i] = new Tile {
                Position = _tiles[i].Position,
                IsEmpty = _tiles[i].IsEmpty,
                Letter = _tiles[i].Letter
            };
        }
        return clone;
    }

    // Method to set a tile on the board with a given letter
    public void SetTile(Vector2 position, string letter) {
        var tile = GetTile(position);
        if (tile == null) {
            return;
        }

        if (letter == "1")
            Debug.Log("ASD");

        tile.IsEmpty = false;
        tile.Letter = letter;
    }

    // Method to get the tile at a specific position on the board
    public Tile GetTile(Vector2 position) {
        foreach (var tile in _tiles) {
            if (tile.Position == position) {
                return tile;
            }
        }

        return null;
    }

    // Method to check if a position is within the bounds of the board
    public bool IsInBounds(Vector2 position) {
        return position.x >= 0 && position.x < Math.Sqrt(_tiles.Count) &&
               position.y >= 0 && position.y < Math.Sqrt(_tiles.Count);
    }

    // Method to check if a tile at a specific position is filled (contains a letter)
    public bool IsFilled(Vector2 position) {
        if (!IsInBounds(position)) return false;
        var tile = GetTile(position);
        return tile != null && !tile.IsEmpty;
    }
    
    // Method to check if a tile at a specific position is empty
    public bool IsEmpty(Vector2 position) {
        if (!IsInBounds(position)) return false;
        var tile = GetTile(position);
        return tile != null && tile.IsEmpty;
    }

    // Override ToString method to generate a string representation of the board
    public override string ToString() {
        var gridSize = (int)Mathf.Sqrt(_tiles.Count);
        var boardArray = new string[gridSize, gridSize];

        foreach (var tile in _tiles) {
            var row = gridSize - 1 - (int)tile.Position.y;
            var col = (int)tile.Position.x;

            boardArray[row, col] = tile.IsEmpty ? "-" : tile.Letter;
        }

        var rows = new List<string>();
        for (var i = 0; i < gridSize; i++) {
            rows.Add("");
            for (var j = 0; j < gridSize; j++) {
                rows[i] += " " + boardArray[i, j];
            }
        }

        var boardString = "";
        foreach (var row in rows){
            boardString = row + "\n" + boardString;
        }

        return boardString;
    }
}
