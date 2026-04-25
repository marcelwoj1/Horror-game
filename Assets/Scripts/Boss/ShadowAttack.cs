using UnityEngine;

public class ShadowAttack : MonoBehaviour
{
    private Transform player;

    [Header("Variables")]
    public GameObject shadow;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    public void SpawnShadows()
    {
        Instantiate(shadow, player.position, Quaternion.identity);
    }
}
