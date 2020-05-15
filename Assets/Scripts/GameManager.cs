using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private GameObject[] pieces;

    public float restartDelay = 1f;

    public GameObject completeLevelUI;

 
    public void showRandomHint()
    {
        GameObject[] pieces = GameObject.FindGameObjectsWithTag("Piece");

        foreach (GameObject piece in pieces)
        {
            // Igore this piece if it's already showing hint
            if (piece.GetComponent<Pieces>().isShowingHint == true)
            {
                continue;
            }
            // Find the piece to show:
            else
            {
                piece.GetComponent<Pieces>().showHints();

                break;
            }
        }
    }

    public void LevelComplete()
    {
        GameObject[] pieces = GameObject.FindGameObjectsWithTag("Piece");

        foreach (GameObject piece in pieces)
        {
            piece.GetComponent<Pieces>().GameWinEffect();
        }
         
        completeLevelUI.transform.GetChild(1).GetComponent<Text>().text = "You have cleared " + SceneManager.GetActiveScene().name + "!";

        //Debug.Log();

        completeLevelUI.SetActive(true);

        //Invoke("Restart", restartDelay);

    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
