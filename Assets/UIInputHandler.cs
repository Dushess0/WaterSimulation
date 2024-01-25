using UnityEngine;
using SFB;
using System.IO;
using System.Linq;
using System.Text;

public class UIInputHandler : MonoBehaviour
{
    [SerializeField]
    private WaterSimulation.Grid grid;

    private float[,,] ParseCSV(string[] lines)
    {
        // Assuming the depth is equal to the number of groups of lines divided by the height
        int height = lines[0].Split(',').Length; // Assuming height is consistent across all z-levels
        int depth = lines.Length / height; // Calculate depth based on total lines and height
        int width = lines[0].Split(',').Length; // Assuming width is consistent across all z-levels

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
                    float value = cell.Type == CellType.Solid ? 99999 : cell.Liquid;
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
}
