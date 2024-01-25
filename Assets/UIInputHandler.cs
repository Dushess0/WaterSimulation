using UnityEngine;
using SFB;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine.EventSystems;

public class UIInputHandler : MonoBehaviour
{
    private Camera cam;
    [SerializeField]
    private WaterSimulation.Grid grid;

    private float actionDelay = 0.25f;
    private float lastActionTime;

    private float[,,] ParseCSV(string[] lines)
    {
        int height = lines[0].Split(',').Length;
        int depth = lines.Length / height;
        int width = lines[0].Split(',').Length;

        float[,,] cellValues = new float[width, height, depth];

        for (int z = 0; z < depth; z++)
        {
            for (int y = 0; y < height; y++)
            {
                string[] cellValuesInRow = lines[z * height + y].Split(',');
                for (int x = 0; x < width; x++)
                {
                    if (x < cellValuesInRow.Length)
                    {
                        float value = float.Parse(cellValuesInRow[x]);
                        cellValues[x, y, z] = value;
                    }
                    else
                    {
                        cellValues[x, y, z] = 0; // Default to 0 if data is missing
                    }
                }
            }
        }

        return cellValues;
    }

    public int selectedBlock = 0;
    public void SaveSimulation()
    {
        var extensionList = new[] { new ExtensionFilter("CSV Files", "csv") };
        var path = StandaloneFileBrowser.SaveFilePanel("Save Simulation", "", "Simulation", extensionList);

        if (!string.IsNullOrEmpty(path))
        {
            string csvContent = ConvertGridToCSV(grid.Cells);
            File.WriteAllText(path, csvContent);
        }
    }

    private string ConvertGridToCSV(Cell[,,] cells)
    {
        StringBuilder csvBuilder = new StringBuilder();

        int width = cells.GetLength(0);
        int height = cells.GetLength(1);
        int depth = cells.GetLength(2);

        for (int z = 0; z < depth; z++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Cell cell = cells[x, y, z];
                    float value = 0;
                    if (cell.Type == CellType.Solid)
                    {
                        value = (float)cell.Style;
                    }
                    else
                    {
                        value = cell.Liquid;
                    }
                    csvBuilder.Append(value);

                    if (x < width - 1)
                    {
                        csvBuilder.Append(",");
                    }
                }
                csvBuilder.AppendLine();
            }

        }
        return csvBuilder.ToString();
    }


    public void LoadSimulation()
    {
        var extensions = new[] { new ExtensionFilter("CSV Files", "csv") };
        var paths = StandaloneFileBrowser.OpenFilePanel("Open Simulation", "", extensions, false);

        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            string[] lines = File.ReadAllLines(paths[0]);

            float[,,] cellValues = ParseCSV(lines);
            grid.CreateGrid(cellValues);
        }
    }
    private void DeleteCell()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

        foreach (var hit in hits)
        {
            Cell cell = hit.collider.GetComponent<Cell>();
            if (cell != null && (cell.Type == CellType.Solid || cell.Liquid > 0))
            {
                cell.SetType(CellType.Blank); // Set the cell type to blank
                cell.Liquid = 0;              // Remove any water in the cell
                break; // Stop after modifying the first relevant cell
            }
        }
    }
    private void PressurizeCell()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

        foreach (var hit in hits)
        {
            Cell cell = hit.collider.GetComponent<Cell>();

            if (cell == null) continue;
            if (cell.Style == CellStyle.Glass)
            {
                continue;
            }


            if (cell != null && cell.Type == CellType.Blank && cell.Liquid > 0)
            {
                cell.Liquid += 30;
                break;
            }
        }
    }

    private void ModifyCell()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

        foreach (var hit in hits)
        {
            Cell hitCell = hit.collider.GetComponent<Cell>();
            if (hitCell == null || hitCell.Type != CellType.Solid)
            {
                continue;
            }
            Vector3 hitPoint = hit.point;
            Collider cellCollider = hit.collider;
            Vector3 cellCenter = cellCollider.bounds.center;

            int x = hitCell.X;
            int y = hitCell.Y;
            int z = hitCell.Z;

            // Determine the face of the cell that was clicked
            if (Mathf.Abs(hitPoint.x - cellCenter.x) > Mathf.Abs(hitPoint.y - cellCenter.y) &&
                Mathf.Abs(hitPoint.x - cellCenter.x) > Mathf.Abs(hitPoint.z - cellCenter.z))
            {
                // Clicked on the left or right face
                x += hitPoint.x > cellCenter.x ? 1 : -1;
            }
            else if (Mathf.Abs(hitPoint.y - cellCenter.y) > Mathf.Abs(hitPoint.x - cellCenter.x) &&
                     Mathf.Abs(hitPoint.y - cellCenter.y) > Mathf.Abs(hitPoint.z - cellCenter.z))
            {
                // Clicked on the top or bottom face
                y += hitPoint.y > cellCenter.y ? -1 : 1;
            }
            else
            {
                // Clicked on the forward or backward face
                z += hitPoint.z > cellCenter.z ? 1 : -1;
            }

            // Check if the new position is within the grid bounds
            if (x >= 0 && x < grid.Width && y >= 0 && y < grid.Height && z >= 0 && z < grid.Depth)
            {

                Cell targetCell = grid.Cells[x, y, z];
                if (targetCell.Type == CellType.Blank)
                {
                    if (selectedBlock == 0)
                    {
                        targetCell.AddLiquid(1f);
                    }
                    else
                    {
                        targetCell.SetType(CellType.Solid, (CellStyle)selectedBlock);
                    }
                }
            }
            break;
        }


    }

    void Awake()
    {
        cam = Camera.main;
    }
    private void Update()
    {
        if (!(Time.time - lastActionTime > actionDelay))
        {
            return;
        }
        if(EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.GetMouseButton(0))
        {
            ModifyCell();
            lastActionTime = Time.time;
        }
        else if (Input.GetKey(KeyCode.Delete))
        {
            DeleteCell();
            lastActionTime = Time.time;
        }
        else if (Input.GetMouseButton(2))
        {
            PressurizeCell();
            lastActionTime = Time.time;
        }
    }
    public void SetBlock(int block)
    {
        selectedBlock = block;
    }

}
