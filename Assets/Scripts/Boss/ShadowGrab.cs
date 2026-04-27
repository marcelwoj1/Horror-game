using System.Collections;
using UnityEngine;

public class ShadowGrab : MonoBehaviour
{
    [Header("Variables")]
    public float liftHeight = 6f;
    public float liftSpeed = 5f;
    public bool waitingForClick = false;

    [Header("Hand Prefab")]
    public GameObject shadowHand;
    private GameObject _shadowHand;

    [Header("References")]
    private Transform player;
    private Rigidbody2D rb;
    private PlayerManager _playerManager;

    void Start()
    {
        // Getting components from player
        rb = GameObject.Find("Player").GetComponent<Rigidbody2D>();
        _playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        player = _playerManager.transform;
    }

    public void StartLiftAttack()
    {
        // Spawning shadow hand
        waitingForClick = true;
        _shadowHand = Instantiate(shadowHand, new Vector3(player.position.x, 29f, player.position.z), Quaternion.identity);
        StartCoroutine(LiftRoutine());
    }

    IEnumerator LiftRoutine()
    {
        // Stopping player movement and gravity
        _playerManager.AllowMovement = false;
        rb.gravityScale = 0f;

        // Set Player Height Target
        Vector3 targetPos = new Vector3(
            player.position.x,
            30f,
            player.position.z
        );

        // Lifts Player Up
        while (Vector3.Distance(player.position, targetPos) > 0.05f)
        {
            player.position = Vector3.MoveTowards(
                player.position,
                targetPos,
                liftSpeed * Time.deltaTime
            );

            yield return null;
        }

        // Waiting for Mouse Click
        waitingForClick = true;

        while (waitingForClick)
        {
            if (Input.GetMouseButtonDown(0))
            {
                waitingForClick = false;
            }

            yield return null;
        }

        // Destroys hand and unlocks player controls
        rb.gravityScale = 2f;
        _playerManager.AllowMovement = true;
        Destroy(_shadowHand);
    }
}