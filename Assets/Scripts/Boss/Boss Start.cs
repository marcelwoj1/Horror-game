using UnityEngine;

public class BossStart : MonoBehaviour
{
    private SpriteAnimator animator;
    private Boss_manager boss_manager;

    void Start()
    {
        animator = GetComponent<SpriteAnimator>();
        boss_manager = GetComponent<Boss_manager>();
    }

    public void StartBoss()
    {
        animator.Play("Start");
        SoundService.Instance?.Play("BossMusic");
    }
}
