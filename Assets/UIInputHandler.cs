using UnityEngine;
using SFB;

public class UIInputHandler : MonoBehaviour
{
    [SerializeField]
    private Grid grid;

    public void SaveSimulation()
    {
        var extensionList = new[] { new ExtensionFilter("JSON Files", "json") };
        var path = StandaloneFileBrowser.SaveFilePanel("Save Simulation", "", "Simulation", extensionList);

        //if (!string.IsNullOrEmpty(path))
        //{
        //    Grid data = grid.GetSerializableData();
        //    string json = JsonUtility.ToJson(data);
        //    System.IO.File.WriteAllText(path, json);
        //}
    }

    public void LoadSimulation()
    {
        var extensions = new[] { new ExtensionFilter("JSON Files", "json") };
        var paths = StandaloneFileBrowser.OpenFilePanel("Open Simulation", "", extensions, false);

        //if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        //{
        //    string json = System.IO.File.ReadAllText(paths[0]);
        //    WaterGridData data = JsonUtility.FromJson<WaterGridData>(json);
        //    grid.SetFromSerializableData(data);
        //}
    }
}
