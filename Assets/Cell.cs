using UnityEngine;
using static UnityEngine.Rendering.DebugUI.MessageBox;

public enum CellType
{
    Blank,
    Solid
}
public enum CellStyle
{
    Metal = 99998,
    Glass = 99997,
    Stone = 99999
}

public enum FlowDirection
{
    Top = 0,
    Right = 1,
    Bottom = 2,
    Left = 3,
    Forward = 4,
    Backward = 5,

}

public class Cell : MonoBehaviour
{

    // Grid index reference
    public int X;
    public int Y;
    public int Z;

    private CellStyle _style= CellStyle.Stone;

    public CellStyle Style
    {
        get
        {
            return _style;
        }
        set
        {
            Destroy(solid);

            if (value == CellStyle.Stone)
            {
                solid = Instantiate(stonePrefab, transform.position, Quaternion.identity, transform);
            }
            if (value == CellStyle.Metal)
            {
                solid = Instantiate(metalPrefab, transform.position, Quaternion.identity, transform);
            }
            if (value == CellStyle.Glass)
            {
                solid = Instantiate(glassPrefab, transform.position, Quaternion.identity, transform);
            }
            _style = value;
        }
    }


    // Amount of liquid in this cell
    public float Liquid { get; set; }

    // Determines if Cell liquid is settled
    private bool _settled;
    public bool Settled
    {
        get { return _settled; }
        set
        {
            _settled = value;
            if (!_settled)
            {
                SettleCount = 0;
            }
        }
    }
    public int SettleCount { get; set; }

    [SerializeField]
    public CellType Type { get; private set; }

    // Neighboring cells
    public Cell Top;
    public Cell Bottom;
    public Cell Left;
    public Cell Right;
    public Cell Forward;
    public Cell Backward;

    // Shows flow direction of cell
    public int Bitmask { get; set; }
    public bool[] FlowDirections = new bool[6];


    private GameObject solid;
    private GameObject water;
    public GameObject stonePrefab;
    public GameObject glassPrefab;
    public GameObject metalPrefab;

    public GameObject waterPrefab;


    void Awake()
    {
        solid = Instantiate(stonePrefab, transform.position, Quaternion.identity, transform);
        water = Instantiate(waterPrefab, transform.position, Quaternion.identity, transform);
        UpdatePrefab();

    }
    private void UpdatePrefab()
    {
        if (Type == CellType.Solid)
        {
            solid.SetActive(true);
            water.SetActive(false);
            solid.transform.localPosition = new Vector3(0, 0, 0);

        }
        else
        {
            solid.SetActive(false);
            if (Liquid < 0.01f)
            {
                water.SetActive(false);
            }
            else
            {
                water.SetActive(true);
                float waterHeight = Mathf.Min(1, Liquid); // Ensure the water isn't taller than the cell
                water.transform.localScale = new Vector3(1, waterHeight, 1);
                if (waterHeight == 1)
                    water.transform.localScale = new Vector3(1, waterHeight * 1.1f, 1);


                // The y-position should be half the height of the water volume above the bottom of the cell
                float waterYPosition = (waterHeight / 2) - 0.5f; // Subtract 0.5 because the cell's pivot is in the middle
                water.transform.localPosition = new Vector3(0, waterYPosition, 0);
            }

        }

    }

    public void Set(int x, int y, int z, Vector3 position, float size)
    {
        X = x;
        Y = y;
        Z = z;
        transform.position = position;
        transform.localScale = new Vector3(size, size, size);
        if (FlowDirections.Length <= 4)
        {
            FlowDirections = new bool[6];
        }
    }

    public void SetType(CellType type)
    {
        Type = type;
        if (Type == CellType.Solid)
        {
            Liquid = 0;
        }
        UnsettleNeighbors();
    }
    public void SetType(CellType type, CellStyle style)
    {
        Type = type;
        if (Type == CellType.Solid)
        {
            Liquid = 0;
            Style = style;
        }
        UnsettleNeighbors();
    }

    public void AddLiquid(float amount)
    {
        Liquid += amount;
        Settled = false;
    }

    public void ResetFlowDirections()
    {
        FlowDirections[0] = false;
        FlowDirections[1] = false;
        FlowDirections[2] = false;
        FlowDirections[3] = false;
        FlowDirections[4] = false;
        FlowDirections[5] = false;

    }

    // Force neighbors to simulate on next iteration
    public void UnsettleNeighbors()
    {
        if (Top != null)
            Top.Settled = false;
        if (Bottom != null)
            Bottom.Settled = false;
        if (Left != null)
            Left.Settled = false;
        if (Right != null)
            Right.Settled = false;
        if (Forward != null)
            Forward.Settled = false;
        if (Backward != null)
            Backward.Settled = false;
    }

    public void Update()
    {
        UpdatePrefab();

    }



}
