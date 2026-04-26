using UnityEngine;

public class BossStart : MonoBehaviour
{
    private SpriteAnimator animator;
    private QuestService questService;


    void Start()
    {
        animator = GetComponent<SpriteAnimator>();
        questService = FindAnyObjectByType<QuestService>();
    }

    public void StartBoss()
    {
        animator.Play("Start");
        SoundService.Instance?.Play("BossMusic");
        questService.SatisfyQuest("FirstCat");
    }
}
