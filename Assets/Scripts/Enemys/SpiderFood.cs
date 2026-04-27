using UnityEngine;

public class SpiderFood : MonoBehaviour
{
    void OnEnable()
    {
        //Registers food for spider to eat
        EnemyPatrol.RegisterFood(this);
    }

    void OnDisable()
    {
        //Unregisters food for spider to eat
        EnemyPatrol.UnregisterFood(this);
    }
}