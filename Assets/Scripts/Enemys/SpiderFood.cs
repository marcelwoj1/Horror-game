using UnityEngine;

public class SpiderFood : MonoBehaviour
{
    void OnEnable()
    {
        EnemyPatrol.RegisterFood(this);
    }

    void OnDisable()
    {
        EnemyPatrol.UnregisterFood(this);
    }
}