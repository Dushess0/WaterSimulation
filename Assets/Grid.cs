using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace WaterSimulation
{
    public class Grid : MonoBehaviour
    {

        public int Width = 30;
        public int Height = 30;
        public int Depth = 30;

        [SerializeField]
        [Range(0.1f, 1f)]
        float CellSize = 1;
        float PreviousCellSize = 1;

        [SerializeField]
        [Range(0f, 0.1f)]
        float LineWidth = 0;
        float PreviousLineWidth = 0;

        [SerializeField]
        Color LineColor = Color.black;
        Color PreviousLineColor = Color.black;

        [SerializeField]
        bool ShowFlow = true;

        [SerializeField]
        bool RenderDownFlowingLiquid = false;

        [SerializeField]
        bool RenderFloatingLiquid = false;

        public Cell[,,] Cells;

        Liquid LiquidSimulator;
        Sprite[] LiquidFlowSprites;




        public GameObject cellPrefab;


        bool Fill;

        void Awake()
        {
            // Load some sprites to show the liquid flow directions

            // Generate our viewable grid GameObjects
            CreateGrid();

            // Initialize the liquid simulator
            LiquidSimulator = new Liquid();
            LiquidSimulator.Initialize(Cells);
        }
        //GetSerializableData()
        void CreateGrid()
        {

            Cells = new Cell[Width, Height, Depth];

            Vector3 offset = this.transform.position;

            // Organize the grid objects
            GameObject cellContainer = new GameObject("Cells");
            cellContainer.transform.parent = this.transform;

            // Cells
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int z = 0; z < Height; z++)
                    {


                        GameObject cellObject = Instantiate(cellPrefab, cellContainer.transform.position, Quaternion.identity);
                        Cell cell = cellObject.GetComponent<Cell>();
                        float xpos = offset.x + (x * CellSize) + (LineWidth * x) + LineWidth;
                        float ypos = offset.y - (y * CellSize) - (LineWidth * y) - LineWidth;
                        float zpos = offset.z + (z * CellSize) - (LineWidth * z) - LineWidth;

                        cell.Set(x, y, z, new Vector3(xpos, ypos, zpos), CellSize);

                        // add a border
                        if (y == Height - 1)
                        {
                            cell.SetType(CellType.Solid,CellStyle.Grass);
                        }

                        cell.transform.parent = cellContainer.transform;
                        Cells[x, y, z] = cell;
                    }
                }
            }
            UpdateNeighbors();
        }

        public void CreateGrid(float[,,] cellValues)
        {
            // Clear existing cells if any
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            Cells = new Cell[Width, Height, Depth];
            Vector3 offset = this.transform.position;

            // Organize the grid objects
            GameObject cellContainer = new GameObject("Cells");
            cellContainer.transform.parent = this.transform;

            // Cells
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int z = 0; z < Depth; z++)
                    {
                        GameObject cellObject = Instantiate(cellPrefab, cellContainer.transform.position, Quaternion.identity);
                        Cell cell = cellObject.GetComponent<Cell>();
                        float xpos = offset.x + (x * CellSize) + (LineWidth * x) + LineWidth;
                        float ypos = offset.y - (y * CellSize) - (LineWidth * y) - LineWidth;
                        float zpos = offset.z + (z * CellSize) - (LineWidth * z) - LineWidth;

                        cell.Set(x, y, z, new Vector3(xpos, ypos, zpos), CellSize);

                        // Set cell type and liquid based on input array
                        if (cellValues[x, y, z] > 1000)
                        {
                            cell.SetType(CellType.Solid, (CellStyle)cellValues[x, y, z]);
                        }
                        else
                        {
                            cell.SetType(CellType.Blank);
                            cell.AddLiquid(cellValues[x, y, z]);
                        }

                        cell.transform.parent = cellContainer.transform;
                        Cells[x, y, z] = cell;
                    }
                }
            }
            UpdateNeighbors();
        }

        // Sets neighboring cell references
        void UpdateNeighbors()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int z = 0; z < Height; z++) // Updated to loop through z dimension as well
                    {
                        // Set left and right neighbors
                        if (x > 0)
                        {
                            Cells[x, y, z].Left = Cells[x - 1, y, z];
                        }
                        if (x < Width - 1)
                        {
                            Cells[x, y, z].Right = Cells[x + 1, y, z];
                        }

                        // Set top and bottom neighbors
                        if (y > 0)
                        {
                            Cells[x, y, z].Top = Cells[x, y - 1, z];
                        }
                        if (y < Height - 1)
                        {
                            Cells[x, y, z].Bottom = Cells[x, y + 1, z];
                        }

                        // Set forward and backward neighbors
                        if (z > 0)
                        {
                            Cells[x, y, z].Backward = Cells[x, y, z - 1];
                        }
                        if (z < Height - 1) // Assuming the depth is also 'Height', adjust if needed
                        {
                            Cells[x, y, z].Forward = Cells[x, y, z + 1];
                        }
                    }
                }
            }
        }



        void Update()
        {
            LiquidSimulator.Simulate(ref Cells);
        }

    }

}