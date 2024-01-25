using UnityEngine;

public enum CellType
{
    Blank,
    Solid
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
    public bool[] FlowDirections = new bool[4];

    // Liquid colors
    Color Color;
    Color DarkColor = new Color(0, 0.1f, 0.2f, 1);


    bool ShowFlow;
    bool RenderDownFlowingLiquid;
    bool RenderFloatingLiquid;

    private GameObject stone;
    private GameObject water;
    public GameObject stonePrefab;
    public GameObject waterPrefab;


    void Awake()
    {
        // todo
        stone = Instantiate(stonePrefab, transform.position, Quaternion.identity, transform);
        water = Instantiate(waterPrefab, transform.position, Quaternion.identity, transform);
        UpdatePrefab();

    }
    private void UpdatePrefab()
    {
        if (Type == CellType.Solid)
        {
            stone.SetActive(true);
            water.SetActive(false);
            stone.transform.localPosition = new Vector3(0, 0, 0);

        }
        else
        {
            stone.SetActive(false);
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

    public void Set(int x, int y, int z, Vector3 position, float size, Sprite[] flowSprites, bool showflow, bool renderDownFlowingLiquid, bool renderFloatingLiquid)
    {
        X = x;
        Y = y;
        Z = z;
        transform.position = position;
        transform.localScale = new Vector3(size, size, size);
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

        // Set background color based on cell type
        if (Type == CellType.Solid)
        {
            //BackgroundSprite.color = Color.black;
        }
        else
        {
            //BackgroundSprite.color = Color.white;
        }

        // Update bitmask based on flow directions


        if (ShowFlow)
        {
            // Show flow direction of this cell
            //FlowSprite.sprite = FlowSprites [Bitmask];
        }
        else
        {
            //FlowSprite.sprite = FlowSprites [0];
        }

        // Set size of Liquid sprite based on liquid value
        UpdatePrefab();
        //LiquidSprite.transform.localScale = new Vector2 (1, Mathf.Min (1, Liquid));	

        //// Optional rendering flags
        //if (!RenderFloatingLiquid) {
        //	// Remove "Floating" liquids
        //	if (Bottom != null && Bottom.Type != CellType.Solid && Bottom.Liquid <= 0.99f) {
        //		LiquidSprite.transform.localScale = new Vector2 (0, 0);	
        //	}
        //}
        //if (RenderDownFlowingLiquid) {
        //	// Fill out cell if cell above it has liquid
        //	if (Type == CellType.Blank && Top != null && (Top.Liquid > 0.05f || Top.Bitmask == 4)) {
        //		LiquidSprite.transform.localScale = new Vector2 (1, 1);	
        //	}
        //}

        //// Set color based on pressure in cell
        //LiquidSprite.color = Color.Lerp (Color, DarkColor, Liquid / 4f);
    }

}
