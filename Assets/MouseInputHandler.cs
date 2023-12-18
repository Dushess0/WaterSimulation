using UnityEngine;

public class MouseInputHandler : MonoBehaviour
{
    public WaterGrid waterGrid; // Reference to your WaterGrid
    public float waterToAdd = 0.5f; // Amount of water to add
    public float stoneToAdd = 1.0f; // Amount of stone to add
    private FreeCamera FreeCamera;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main; // Ensure you have a main camera tagged
        FreeCamera = cam.GetComponent<FreeCamera>();
    }

    private void Update()
    {
        if (FreeCamera == null) throw new System.Exception("Attach freecamera component to camera object");
        if (FreeCamera.isCursorLocked) return;
      
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            ModifyCell(true);
        }
        else if (Input.GetMouseButtonDown(1)) // Right click
        {
            ModifyCell(false);
        }
    }

    private void ModifyCell(bool addWater)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 hitPoint = hit.point;
            int x = Mathf.FloorToInt(hitPoint.x);
            int y = Mathf.FloorToInt(hitPoint.y);
            int z = Mathf.FloorToInt(hitPoint.z);

            if (x >= 0 && x < waterGrid.Width && y >= 0 && y < waterGrid.Height && z >= 0 && z < waterGrid.Depth)
            {
                Cell clickedCell = waterGrid.GetCell(x, y, z);
                if (clickedCell == null) return;

                if (addWater)
                {
                    float maxWaterToAdd = 1 - clickedCell.WaterVolume - clickedCell.StoneVolume; // Maximum water that can be added
                    float waterToAddLeft = Mathf.Min(maxWaterToAdd, waterToAdd); // Water to add is the lesser of available space and the desired amount

                    // Check if there's room for more water in the clicked cell
                    if (waterToAddLeft > 0)
                    {
                        clickedCell.WaterVolume += waterToAddLeft;
                    }
                    else if (y + 1 < waterGrid.Height) // Check the cell above if there's no room
                    {
                        Cell aboveCell = waterGrid.GetCell(x, y + 1, z);
                        if (aboveCell != null && aboveCell.WaterVolume + waterToAdd <= 1 - aboveCell.StoneVolume)
                        {
                            aboveCell.WaterVolume += waterToAdd;
                        }
                    }
                }
                else // Add stone
                {
                    float amountToAdd = Mathf.Min(stoneToAdd, 1 - clickedCell.StoneVolume - clickedCell.WaterVolume);
                    clickedCell.StoneVolume += amountToAdd;
                    // Optionally handle adding stone to the cell above similar to water
                }
            }
        }
    }

}
