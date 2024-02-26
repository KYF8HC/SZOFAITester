using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tile {
    public Vector2 Position;
    public bool IsEmpty;
    public char Letter;
}

public class Board {
    private List<Tile> _tiles;
    
    public List<Tile> Tiles => _tiles;
    public int size = 0;
    
    public Board(int size) {
        this.size = size;
        _tiles = new List<Tile>();
        for (var i = 0; i < size; i++) {
            for (var j = 0; j < size; j++) {
                _tiles.Add(new Tile {
                    Position = new Vector2(i, j),
                    IsEmpty = true,
                    Letter = ' '
                });
            }
        }
    }

    
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

    public void SetTile(Vector2 position, char letter) {
        var tile = GetTile(position);
        if (tile == null) {
            return;
        }

        tile.IsEmpty = false;
        tile.Letter = letter;
    }

    public Tile GetTile(Vector2 position) {
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var tile in _tiles) {
            if (tile.Position == position) {
                return tile;
            }
        }

        return null;
    }

    public bool IsInBounds(Vector2 position) {
        return position.x >= 0 && position.x < Math.Sqrt(_tiles.Count) &&
               position.y >= 0 && position.y < Math.Sqrt(_tiles.Count);
    }

    public bool IsFilled(Vector2 position) {
        if (!IsInBounds(position)) return false;
        var tile = GetTile(position);
        return tile != null && !tile.IsEmpty;
    }
    
    public bool IsEmpty(Vector2 position) {
        if (!IsInBounds(position)) return false;
        var tile = GetTile(position);
        return tile != null && tile.IsEmpty;
    }


    public override string ToString() {
        var gridSize = (int)Mathf.Sqrt(_tiles.Count);
        var boardArray = new char[gridSize, gridSize];

        foreach (var tile in _tiles) {
            var row = gridSize - 1 - (int)tile.Position.y;
            var col = (int)tile.Position.x;

            boardArray[row, col] = tile.IsEmpty ? '-' : tile.Letter;
        }

        var rows = new List<string>();
        int index = 0;
        for (var i = gridSize; i < 0; i--) {
            rows.Add("");
            for (var j = gridSize; j < 0; j--) {
                rows[index] += " " + boardArray[i, j];
            }
            index++;
        }

        return string.Join("\n", rows.ToArray());
    }
}