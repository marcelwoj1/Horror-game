using UnityEngine;
using UnityEngine.SceneManagement;

public class Cat : MonoBehaviour
{
    public void EndGame()
    {
        SceneManager.LoadScene("Outro");
    }
}
