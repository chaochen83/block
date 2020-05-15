using UnityEngine;


public class Drag : MonoBehaviour
{
    //private Board board;

    // For init position of the whole piece when click on the object
    private float minPiecePosX;
    private float maxPiecePosX;
    private float startPosX;  // in between (middle) of minPiecePosX and maxPiecePosX
    private float startPosY;  // the lowest block's Y position

    private bool isBeingHeld = false;

    [Header("Match Fillable")]
    public GameObject matchedFillableTile;


    // Start is called before the first frame update
    void Start()
    {
        //board = FindObjectOfType<Board>(); // b/c there is only 1 board

    }

    // Update is called once per frame
    void Update()
    {
        if (isBeingHeld == true)
        {
            Vector3 mousePos = Input.mousePosition;

            mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            this.transform.parent.GetComponent<Transform>().localPosition = new Vector3(mousePos.x - startPosX, mousePos.y - startPosY, 0);

        }
    }


    private void OnMouseDown()
    {
        // If not set to false, the piece will always lerping
        transform.parent.GetComponent<Pieces>().currentState = PieceState.held;

        // If not, the fillables will be occupied when put down & put up
        transform.parent.GetComponent<Pieces>().UnsetAllFillableOccupied();

        transform.parent.GetComponent<Pieces>().SetTopSortingOrder();

        Vector3 mousePos = Input.mousePosition;

        mousePos = Camera.main.ScreenToWorldPoint(mousePos);


        startPosY = this.transform.localPosition.y;

        // A little more lower than "the middle point of the lowest block":
        startPosY = startPosY - this.GetComponent<Renderer>().bounds.size.y / 2;


        // Give them an init value, othewise they will be "0" as default.
        // 0 will casue issue when other block's position is -3, etc
        minPiecePosX = maxPiecePosX = this.transform.localPosition.x;

        foreach (Transform child in this.transform.parent.GetComponent<Transform>())
        {
            startPosY = startPosY < child.localPosition.y ? startPosY : child.localPosition.y;

            minPiecePosX = minPiecePosX < child.localPosition.x ? minPiecePosX : child.localPosition.x;
            maxPiecePosX = maxPiecePosX > child.localPosition.x ? maxPiecePosX : child.localPosition.x;
        }

        startPosX = minPiecePosX + (maxPiecePosX - minPiecePosX) / 2;

        isBeingHeld = true;
    }

    private void OnMouseUp()
    {
        isBeingHeld = false;

        transform.parent.GetComponent<Pieces>().UnsetTopSortingOrder();


        // If not all the blocks of the piece is in fillable area, then the piece return to init postion
        if (transform.parent.GetComponent<Pieces>().wholePieceIsMatched == false)
        {
            transform.parent.GetComponent<Pieces>().currentState = PieceState.returning;

            transform.parent.GetComponent<Pieces>().ClearAllMatchedFillable();
        }
        else
        {
            transform.parent.GetComponent<Pieces>().currentState = PieceState.lerping;
        }
    }

    public void SetMatchedFillable(GameObject fillalbleTile)
    {
        matchedFillableTile = fillalbleTile;
    }


}
