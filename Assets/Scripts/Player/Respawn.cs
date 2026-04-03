using UnityEngine;
using UnityEngine.SceneManagement;

public class Respawn : MonoBehaviour
{
    private PlayerHealth playerHealth;
    public GameObject DeathPanel;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        
        // Fallback in case Respawn is on a different GameObject
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth>();
        }
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDeath += ReloadScene;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDeath -= ReloadScene;
        }
    }

    private void ReloadScene()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject obj in objects)
        {
            Destroy(obj);
        }
        
        DeathPanel.SetActive(true);
    }
}
