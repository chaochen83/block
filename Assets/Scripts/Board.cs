using UnityEngine;

public class Board : MonoBehaviour
{
    private int width = 12;
    private int height = 21;

    [Header("Setup data")]
    public int fillableWidth;
    public int fillableHeight;

    [Header("Runtime info")]
    public GameObject tilePrefab;
    public GameObject fillablePrefab;
    public GameObject[] allFillables;


    // Start is called before the first frame update
    void Start()
    {
        //width = 12;
        //height = 21;

        allFillables = new GameObject[fillableWidth * fillableHeight];

        SetUp();
    }


    // Set up the board
    private void SetUp()
    {
        int minFillableX = (int) (width - fillableWidth) / 2;
        int minFillableY = (int) (height - fillableHeight) / 2;


        int allFillablesIndex = 0;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 blockPosition = new Vector2(i, j);

                if (j > minFillableY && j <= minFillableY + fillableHeight && i >= minFillableX && i < minFillableX + fillableWidth)
                {
                    GameObject fillableBackground = Instantiate(fillablePrefab, blockPosition, Quaternion.identity) as GameObject;

                    fillableBackground.transform.parent = this.transform;

                    fillableBackground.name = "Fillable_(" + i + "," + j + ")";

                    allFillables[allFillablesIndex] = fillableBackground;
                    allFillablesIndex++;
                }
                else
                {
                    GameObject backgroundTile = Instantiate(tilePrefab, blockPosition, Quaternion.identity) as GameObject;

                    backgroundTile.transform.parent = this.transform;

                    backgroundTile.name = "Tile_(" + i + "," + j + ")";
                }
   
            }
        }

        //Debug.Log(allFillables[27].transform.position.x);

    }


    /*
     * Helper function.
     */
    public bool IsCloseEnough(GameObject object1, GameObject object2)
    {
        // Within this distance means block can fill into fillableTile
        float HalfCellDistance = 0.5f;

        float xDistance = Mathf.Abs(object1.transform.position.x - object2.transform.position.x);
        float yDistance = Mathf.Abs(object1.transform.position.y - object2.transform.position.y);

        if (xDistance < HalfCellDistance && yDistance < HalfCellDistance) return true;

        return false;
    }


    public void RemoveAllFillableHighlights()
    {
        foreach (GameObject fillalbleTile in allFillables)
        {
            fillalbleTile.GetComponent<Fillable>().removeHighlight();

            //if (withMatchedBlocks == true) fillalbleTile.GetComponent<Fillable>().unsetMatchedBlock();
        }
    }

    public void RemoveAllFillableMatchedBlocks()
    {
        foreach (GameObject fillalbleTile in allFillables)
        {
            fillalbleTile.GetComponent<Fillable>().unsetMatchedBlock();

        }
    }

    public bool IsPuzzleSovled()
    {
        int count = 0;

        foreach (GameObject fillalbleTile in allFillables)
        {
            if (fillalbleTile.GetComponent<Fillable>().isOccupied == true)
            {
                count++;
            }
        }

        if (count == allFillables.Length) {
            //Debug.Log("Win!");
            //destroyAllBlocks();

            return true;
        };

        return false;
    }

 
}
