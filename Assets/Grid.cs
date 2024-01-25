using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class Grid : MonoBehaviour
{

    int Width = 30;
    int Height = 30;
    int Depth = 30;

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

    Cell[,,] Cells;

    Liquid LiquidSimulator;
    Sprite[] LiquidFlowSprites;


    private Camera cam;

    public GameObject cellPrefab;


    bool Fill;

    void Awake()
    {
        cam = Camera.main; // Ensure you have a main camera tagged


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

                    cell.Set(x, y, z, new Vector3(xpos, ypos, zpos), CellSize, LiquidFlowSprites, ShowFlow, RenderDownFlowingLiquid, RenderFloatingLiquid);

                    // add a border
                    if (y == Height-1)
                    {
                        cell.SetType(CellType.Solid);
                    }

                    cell.transform.parent = cellContainer.transform;
                    Cells[x, y, z] = cell;
                }
            }
        }
        UpdateNeighbors();
    }


    // Live update the grid properties
    void RefreshGrid()
    {

        Vector2 offset = this.transform.position;


        // Cells
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                float xpos = offset.x + (x * CellSize) + (LineWidth * x) + LineWidth;
                float ypos = offset.y - (y * CellSize) - (LineWidth * y) - LineWidth;
                Cells[x, y, 0].Set(x, y, 0, new Vector2(xpos, ypos), CellSize, LiquidFlowSprites, ShowFlow, RenderDownFlowingLiquid, RenderFloatingLiquid);

            }
        }

        // Fit camera to grid
        //View.transform.position = this.transform.position + new Vector3(HorizontalLines [0].transform.localScale.x/2f, -VerticalLines [0].transform.localScale.y/2f);
        //View.transform.localScale = new Vector2 (HorizontalLines [0].transform.localScale.x, VerticalLines [0].transform.localScale.y);
        //Camera.main.GetComponent<Camera2D> ().Set ();
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

    private void ModifyCell(bool addWater)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 hitPoint = hit.point;
            Cell cell = hit.collider.GetComponent<Cell>();
            var target = Cells[cell.X, cell.Y, cell.Z];
            if (addWater)
            {
                target.AddLiquid(5);
            }
            else
            {
                if (cell.Type == CellType.Blank)
                    target.SetType(CellType.Solid);
                else
                    target.SetType(CellType.Blank);

            }
        }
    }

    void Update()
    {


        if (Input.GetMouseButtonDown(0)) // Left click
        {
            ModifyCell(true);
        }
        else if (Input.GetMouseButtonDown(1)) // Right click
        {
            ModifyCell(false);
        }

        // Run our liquid simulation 
        LiquidSimulator.Simulate(ref Cells);
    }

}
