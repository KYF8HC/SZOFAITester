using System;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public Vector2 Position;
    public bool IsEmpty;
}

public class Board
{
    private List<Tile> _tiles;

    public Board(int size)
    {
        _tiles = new List<Tile>();
        for (var i = 0; i < size; i++)
        {
            for (var j = 0; j < size; j++)
            {
                _tiles.Add(new Tile
                {
                    Position = new Vector2(i, j),
                    IsEmpty = true
                });
            }
        }

        Debug.Log(_tiles.Count);
    }

    public override string ToString()
    {
        var gridSize = (int)Mathf.Sqrt(_tiles.Count);
        var boardArray = new char[gridSize, gridSize];

        foreach (var tile in _tiles)
        {
            var row = gridSize - 1 - (int)tile.Position.y;
            var col = (int)tile.Position.x;

            boardArray[row, col] = tile.IsEmpty ? 'X' : '-';
        }

        var rows = new List<string>();
        for (var i = 0; i < gridSize; i++)
        {
            rows.Add(new string(boardArray[i, 0], gridSize));
        }

        return string.Join(Environment.NewLine, rows);
    }
}