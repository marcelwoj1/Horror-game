using UnityEngine;

public class JanitorKey : MonoBehaviour
{
    [Header("Components")]
    private PlayerManager _playerManager;
    private Enemy _enemy;
    private Janitor _janitor;

    [Header("Variables")]
    public bool KeySpawned = false;

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
        if (KeySpawned == false)
        {
            Instantiate(KeyPrefab, transform.position + new Vector3(0, -1.5f, 0), Quaternion.identity);
            KeySpawned = true;
        }
        if(_playerManager.IsCrouching == false)
        {
            _enemy.isAggressive = true;
            _janitor.chasePlayer = true;
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Janitor"), false);
        }
    }
}
