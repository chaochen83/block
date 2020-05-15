using UnityEngine;

public enum PieceState
{
    origin,
    held,  // being held and follow the mouse moving
    lerping, // released from held and lerping to highlight fillable
    inFill, // stay still in fillable
    returning // returning to origin postion
}


public class Pieces : MonoBehaviour
{
    private Board board;
    private GameManager gameManager;

    private Vector2 initialPosition;

    private GameObject[] blocks;

    // By default they are disabled
    private GameObject[] hintBlocks;


    [Header("Setup data")]
    // This is set from outside by game level editors
    public Vector2[] blockPostions;

    // Piece's final positon, used as offset to generate hint blocks
    public Vector2 hintPosition;

    public GameObject blockPrefab;
    public GameObject particalEffectPrefab;
    public GameObject hintEffect;


    [Header("Runtime info")]
    public bool wholePieceIsMatched;

    public PieceState currentState = PieceState.origin;

    // If the hint is being shown
    public bool isShowingHint = false;



    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>(); // b/c there is only 1 board
        gameManager = FindObjectOfType<GameManager>(); // b/c there is only 1 board

        initialPosition = transform.position;
        wholePieceIsMatched = false;


        blocks = new GameObject[blockPostions.Length];
        hintBlocks = new GameObject[blockPostions.Length];

        GeneratePiece();


        GenerateHints();
    }

    void Update()
    {
        if (currentState == PieceState.lerping)
        {
            LerpToHighlightPosition();
        }

        if (currentState == PieceState.returning)
        {
            ReturnToInitPosition();
        }

        if (blocks != null && currentState == PieceState.held)
        {
            int count = 0;

            foreach (GameObject fillalbleTile in board.allFillables)
            {
                // This fillableTile already has some piece's block on it:
                if (fillalbleTile.GetComponent<Fillable>().isOccupied == true)
                {
                    continue;
                }

                foreach (GameObject block in blocks)
                {
                    if (board.IsCloseEnough(block, fillalbleTile))
                    {
                
                        fillalbleTile.GetComponent<Fillable>().setMatchedBlock(block);

                        block.GetComponent<Drag>().SetMatchedFillable(fillalbleTile);

                        count++;
      

                        continue;
                    }
                    // When just moved away from current matched fillableTile:
                    else if (fillalbleTile.GetComponent<Fillable>().matchedBlock == block)
                    {
                        fillalbleTile.GetComponent<Fillable>().removeHighlight();

                        fillalbleTile.GetComponent<Fillable>().unsetMatchedBlock();
                    }
                }


            }

            // All blocks of the piece is in fillable area
            if (count == blocks.Length)
            {
                wholePieceIsMatched = true;
  
                HighlightMatchedFillables();
            }
            // The piece is moved out of fillable area
            else
            {
                wholePieceIsMatched = false;

                board.RemoveAllFillableHighlights();

                board.RemoveAllFillableMatchedBlocks();
            }


        }
    }

    void GeneratePiece()
    {
        for (int i = 0; i < blockPostions.Length; i++)
        {
            GameObject block = Instantiate(blockPrefab, blockPostions[i], Quaternion.identity) as GameObject;

            block.transform.parent = this.transform;

            block.name = this.name + "_[" + i + "]";

            blocks[i] = block;
        }
    }

    void GenerateHints()
    {
        for (int i = 0; i < blockPostions.Length; i++)
        {
            Vector2 blockHintPosition = new Vector2(blockPostions[i].x + hintPosition.x - transform.position.x, blockPostions[i].y + hintPosition.y - transform.position.y);

            GameObject hintBlock = Instantiate(hintEffect, blockHintPosition, Quaternion.identity) as GameObject;

            // Don't set hint's parent to be piece,
            // b/c then hints will move with the piece
            hintBlock.transform.parent = board.transform;

            hintBlock.name = "Hint_" + this.name + "_[" + i + "]";

            hintBlocks[i] = hintBlock;
        }
    }

    public void showHints()
    {
        foreach (GameObject hintBlock in hintBlocks)
        {
            hintBlock.SetActive(true);
        }

        isShowingHint = true;
    }

    public void hideHints()
    {
        foreach (GameObject hintBlock in hintBlocks)
        {
            hintBlock.SetActive(false);
        }

        isShowingHint = false;
    }


    private void HighlightMatchedFillables()
    {
        foreach (GameObject block in blocks)
        {
            block.GetComponent<Drag>().matchedFillableTile.GetComponent<Fillable>().doHighlight();
        }
    }


    public void ClearAllMatchedFillable()
    {
        foreach (GameObject block in blocks)
        {
            block.GetComponent<Drag>().matchedFillableTile = null;
        }
    }

    public void SetAllFillableOccupied()
    {
        foreach (GameObject block in blocks)
        {
            if (block.GetComponent<Drag>().matchedFillableTile == null) continue;
            block.GetComponent<Drag>().matchedFillableTile.GetComponent<Fillable>().isOccupied = true;
        }
    }

    public void UnsetAllFillableOccupied()
    {
        foreach (GameObject block in blocks)
        {
            if (block.GetComponent<Drag>().matchedFillableTile == null) continue;
            block.GetComponent<Drag>().matchedFillableTile.GetComponent<Fillable>().isOccupied = false;
        }
    }

    public void ReturnToInitPosition()
    {
        float lerpSpeed = 0.5f;

        this.transform.position = Vector2.Lerp(this.transform.position, initialPosition, lerpSpeed);

        // Returning finished:
        if ((Vector2)this.transform.position == initialPosition)
        {
            currentState = PieceState.origin;
        }
    }

    public void LerpToHighlightPosition()
    {
        float lerpSpeed = 0.7f;

        float xOffset = blocks[0].GetComponent<Drag>().matchedFillableTile.transform.position.x - blocks[0].transform.position.x;

        float yOffset = blocks[0].GetComponent<Drag>().matchedFillableTile.transform.position.y - blocks[0].transform.position.y;

        Vector2 towardsPostion = new Vector2(this.transform.position.x + xOffset, this.transform.position.y + yOffset);

        this.transform.position = Vector2.Lerp(this.transform.position, towardsPostion, lerpSpeed);

        // Lerp finished, piece stays in fillable area:
        if ((Vector2)this.transform.position == towardsPostion)
        {
            currentState = PieceState.inFill;

            /*
             * Should keep the fillableTile's matched blocks,
             * so these fillableTiles won't be matched again by other pieces
             */
            board.RemoveAllFillableHighlights();

            SetAllFillableOccupied();

            if (board.IsPuzzleSovled())
            {
                gameManager.LevelComplete();
            }
        }
    }

    public void SetTopSortingOrder()
    {
        foreach (GameObject block in blocks)
        {
            block.GetComponent<SpriteRenderer>().sortingOrder = 9;
        }
    }

    public void UnsetTopSortingOrder()
    {
        int defaultSortingOrder = 1;
        foreach (GameObject block in blocks)
        {
            block.GetComponent<SpriteRenderer>().sortingOrder = defaultSortingOrder;
        }
    }

    private void DoParticleEffect()
    {
        foreach (GameObject block in blocks)
        {
            GameObject partical = Instantiate(particalEffectPrefab, block.transform.position, Quaternion.identity) as GameObject;

            partical.transform.parent = block.transform;

            partical.name = block.name + "_partical";

            //Destroy(partical);
        }
    }

    public void GameWinEffect()
    {
        DoParticleEffect();

        foreach (GameObject block in blocks)
        {
            block.GetComponent<SpriteRenderer>().enabled = false;
        }
    }


}
