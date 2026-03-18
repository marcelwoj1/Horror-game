using UnityEngine;

public class RatSpawner : MonoBehaviour
{
    public GameObject TheRat;

    public void SpawnRatFromDrawer()
    {
        Instantiate(TheRat, transform.position, Quaternion.identity);
    }


}
