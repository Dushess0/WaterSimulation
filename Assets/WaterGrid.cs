using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WaterGrid : MonoBehaviour
{
    private Cell[,,] cells;
    public int Width = 10;
    public int Height = 10;
    public int Depth = 10;
    public GameObject cellPrefab;

    private void InitCells()
    {
        cells = new Cell[Width, Height, Depth];
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int z = 0; z < Depth; z++)
                {
                    Vector3 position = new Vector3(x, y, z);
                    GameObject cellObject = Instantiate(cellPrefab, position, Quaternion.identity);
                    Cell cell = cellObject.GetComponent<Cell>();

                    if (y == 0)
                    {
                        cell.StoneVolume = 1.0f;
                    }
                    cells[x, y, z] = cell;
                }
            }
        }

        cells[4, 1, 4].WaterVolume = 1.0f;

    }

    private void Start()
    {

        InitCells();

    }
    private void SpreadWaterHorizontally()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int z = 0; z < Depth; z++)
                {
                    Cell currentCell = cells[x, y, z];

                    // Skip cells with no water or cells that are full
                    if (currentCell.WaterVolume <= 0 || currentCell.WaterVolume + currentCell.StoneVolume >= 1)
                        continue;

                    // Spread to left and right neighbors
                    if (x > 0) // Check left neighbor
                        SpreadWaterBetweenCells(currentCell, cells[x - 1, y, z]);
                    if (x < Width - 1) // Check right neighbor
                        SpreadWaterBetweenCells(currentCell, cells[x + 1, y, z]);

                    // Spread to forward and backward neighbors
                    if (z > 0) // Check backward neighbor
                        SpreadWaterBetweenCells(currentCell, cells[x, y, z - 1]);
                    if (z < Depth - 1) // Check forward neighbor
                        SpreadWaterBetweenCells(currentCell, cells[x, y, z + 1]);
                }
            }
        }
    }
    public Cell GetCell(int x, int y, int z)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height && z >= 0 && z < Depth)
        {
            return cells[x, y, z];
        }
        return null;
    }

    private void SpreadWaterBetweenCells(Cell source, Cell target)
    {
        float totalWater = source.WaterVolume + target.WaterVolume;
        float evenDistribution = totalWater / 2;

        // Calculate how much water can actually be moved considering the stone volume
        float waterToMove = Mathf.Min(evenDistribution, 1 - target.StoneVolume) - target.WaterVolume;

        if (waterToMove > 0)
        {
            source.TransferWaterTo(target, waterToMove);
        }
    }

    private void Update()
    {
        SpreadWaterVertically();
        SpreadWaterHorizontally();
    }
    private void SpreadWaterVertically()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = Height - 1; y >= 0; y--) // Start from top to bottom
            {
                for (int z = 0; z < Depth; z++)
                {
                    Cell currentCell = cells[x, y, z];

                    // Skip cells with no water or bottom layer cells
                    if (currentCell.WaterVolume <= 0 || y == 0)
                        continue;

                    Cell cellBelow = cells[x, y - 1, z];

                    // Calculate how much water can move down
                    float waterToMove = Mathf.Min(currentCell.WaterVolume, 1 - cellBelow.WaterVolume - cellBelow.StoneVolume);

                    if (waterToMove > 0)
                    {
                        currentCell.TransferWaterTo(cellBelow, waterToMove);
                    }
                }
            }
        }
    }

}