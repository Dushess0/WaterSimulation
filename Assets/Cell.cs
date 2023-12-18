using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Cell : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 1f)]
    private float waterVolume;
    [SerializeField]
    [Range(0f, 1f)]
    private float stoneVolume;

    private GameObject stone;
    private GameObject water;

    public GameObject stonePrefab;
    public GameObject waterPrefab;

    private const float MinimumVisibleVolume = 0.05f;



    public float WaterVolume
    {
        get { return waterVolume; }
        set { waterVolume = Mathf.Clamp(value, 0, 1 - stoneVolume); UpdateWaterPrefab(); }

    }

    public float StoneVolume
    {
        get { return stoneVolume; }
        set { stoneVolume = Mathf.Clamp(value, 0, 1 - waterVolume); UpdateStonePrefab(); }
    }

    private void Start()
    {
        stone = Instantiate(stonePrefab, transform.position, Quaternion.identity, transform);
        water = Instantiate(waterPrefab, transform.position, Quaternion.identity, transform);

        UpdateStonePrefab();
        UpdateWaterPrefab();
    }


    private void UpdateStonePrefab()
    {
        if (stone == null) return;
        if (stoneVolume < MinimumVisibleVolume)
        {
            stone.SetActive(false);
        }
        else
        {
            stone.SetActive(true);
            stone.transform.localScale = new Vector3(1, stoneVolume, 1);
            stone.transform.localPosition = new Vector3(0, stoneVolume / 2, 0);
        }
    }

    private void UpdateWaterPrefab()
    {
        if (water == null) return;

        if (waterVolume < MinimumVisibleVolume)
        {
            water.SetActive(false);
        }
        else
        {
            water.SetActive(true);
            water.transform.localScale = new Vector3(1, waterVolume, 1);
            water.transform.localPosition = new Vector3(0, waterVolume / 2 + stoneVolume, 0);
        }
    }
    public void TransferWaterTo(Cell otherCell, float amount)
    {
        amount = Mathf.Min(Mathf.Floor(amount * 100) / 100.0f, waterVolume);

        waterVolume -= amount;
        otherCell.WaterVolume += amount;

        UpdateWaterPrefab();
        otherCell.UpdateWaterPrefab();
    }

    private void Update()
    {
        UpdateStonePrefab();
        UpdateWaterPrefab();
    }

}

[System.Serializable]
public class CellData
{
    public float WaterVolume;
    public float StoneVolume;
}

[System.Serializable]
public class WaterGridData
{
    public CellData[] Cells; // 1D array to store cell data
    public int Width;
    public int Height;
    public int Depth;
}
