using System.Collections;
using UnityEngine;

public class ShadowGrab : MonoBehaviour
{
    private Transform player;
    public float liftHeight = 6f;
    public float liftSpeed = 5f;
    public GameObject shadowHand;
    private GameObject _shadowHand;
    private Rigidbody2D rb;

    [Header("References")]
    private PlayerManager _playerManager;

    [Header("Variables")]
    public bool waitingForClick = false;

    void Start()
    {
        rb = GameObject.Find("Player").GetComponent<Rigidbody2D>();
        _playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        player = _playerManager.transform;
    }

    public void StartLiftAttack()
    {
        waitingForClick = true;
        _shadowHand = Instantiate(shadowHand, new Vector3(player.position.x, 29f, player.position.z), Quaternion.identity);
        StartCoroutine(LiftRoutine());
    }

    IEnumerator LiftRoutine()
    {
        // LOCK PLAYER CONTROL
        _playerManager.AllowMovement = false;
        rb.gravityScale = 0f;

        Vector3 targetPos = new Vector3(
            player.position.x,
            30f,
            player.position.z
        );

        // --- LIFT PLAYER UP ---
        while (Vector3.Distance(player.position, targetPos) > 0.05f)
        {
            player.position = Vector3.MoveTowards(
                player.position,
                targetPos,
                liftSpeed * Time.deltaTime
            );

            yield return null;
        }

        // WAIT FOR INPUT
        waitingForClick = true;

        while (waitingForClick)
        {
            if (Input.GetMouseButtonDown(0))
            {
                waitingForClick = false;
            }

            yield return null;
        }

        // UNLOCK PLAYER CONTROL
        rb.gravityScale = 2f;
        _playerManager.AllowMovement = true;
        Destroy(_shadowHand);
    }
}