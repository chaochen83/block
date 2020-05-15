using UnityEngine;

//public enum FillableState
//{
//    origin,
//    highlight,
//    occupy
//}


public class Fillable : MonoBehaviour
{
    //private Board board;

    public GameObject matchedBlock;

    public bool isOccupied = false;

    //public FillableState currentState = FillableState.origin;


    // Start is called before the first frame update
    void Start()
    {
        //board = FindObjectOfType<Board>(); // b/c there is only 1 board

    }

    /*
     * 
     */
    public void setMatchedBlock(GameObject block)
    {
        matchedBlock = block;

    }

    public void unsetMatchedBlock()
    {
        matchedBlock = null;
    }

    public void doHighlight()
    {
        this.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0);
    }

    public void removeHighlight()
    {
        this.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 255);

    }

}
