using UnityEngine;

public class BossStart : MonoBehaviour
{
    [Header("Components")]
    private SpriteAnimator animator;
    private QuestService questService;

    void Start()
    {
        animator = GetComponent<SpriteAnimator>();
        questService = FindAnyObjectByType<QuestService>();
    }

    public void StartBoss()
    {
        //Boss Fight Starts
        animator.Play("Start");
        SoundService.Instance?.Play("BossMusic");
        questService.SatisfyQuest("FirstCat");
    }
}
