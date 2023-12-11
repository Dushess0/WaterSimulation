using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterGrid: MonoBehaviour
{
    private Cell[,,] cells;
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int Depth { get; private set; }

    public WaterGrid(int width = 10, int height = 10, int depth = 10)
    {
        Width = width;
        Height = height;
        Depth = depth;
        cells = new Cell[width, height, depth];

        // Initialize the cells
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    cells[x, y, z] = new Cell();
                }
            }
        }
    }

    public void Update()
    {
        // Update logic for each cell
        // Apply rules for liquid movement

        // Example of debug drawing
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int z = 0; z < Depth; z++)
                {
                    Vector3 position = new Vector3(x, y, z);
                    Debug.DrawLine(position, position + Vector3.up * cells[x, y, z].LiquidAmount, Color.blue);
                }
            }
        }
    }
}