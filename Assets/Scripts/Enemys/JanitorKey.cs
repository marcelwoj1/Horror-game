using UnityEngine;

public class JanitorKey : MonoBehaviour
{
    [Header("Components")]
    private PlayerManager _playerManager;
    private Enemy _enemy;
    private Janitor _janitor;

    [Header("Prefabs")]
    public GameObject KeyPrefab;

    void Start()
    {
        _playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        _enemy = GetComponent<Enemy>();
        _janitor = GetComponent<Janitor>();
    }

    public void Spawn()
    {
        //Won't spawn a second key
        if (_enemy.KeyDropped == false)
        {
            //Spawn Key
            Instantiate(KeyPrefab, transform.position + new Vector3(0, -1.5f, 0), Quaternion.identity);
            _enemy.KeyDropped = true;
        }
        //Janitor becomes aggressive if not Crouching
        if(_playerManager.IsCrouching == false)
        {
            //Make Janitor Aggressive
            _enemy.isAggressive = true;
            _janitor.chasePlayer = true;
        }
    }
}
