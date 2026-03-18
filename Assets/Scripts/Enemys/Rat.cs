using UnityEngine;

public class Rat : MonoBehaviour
{
    private float startY;
    private SpriteAnimator _animator;
    public float speed = 6f;
    private Transform player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startY = transform.position.y;
        _animator = GetComponent<SpriteAnimator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);

        Vector3 target = new Vector3(player.position.x, startY, transform.position.z);

        Vector3 move = new Vector3(direction * speed * Time.deltaTime, 0, 0);
        _animator.Play("Walk");
        float distance = Vector2.Distance(transform.position, player.position);
        transform.position += move;

        if (player.position.x > transform.position.x)
            transform.localScale = new Vector3(1,1,1);
        else
            transform.localScale = new Vector3(-1,1,1);
    }
    
}
