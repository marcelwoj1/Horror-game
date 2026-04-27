using UnityEngine;
using UnityEngine.SceneManagement;

public class Cat : MonoBehaviour
{
    //Ends the game by loading the outro scene
    public void EndGame()
    {
        SceneManager.LoadScene("Outro");
    }

}
