using UnityEngine;
using UnityEngine.UI;

public class PieTimer : MonoBehaviour
{
    public Image timerImage;
    public float duration = 15f;

    private float timeRemaining;
    private Hiding _hiding;

    void Start()
    {
        timeRemaining = duration;
        timerImage.fillAmount = 1f;
        _hiding = GameObject.Find("Player").GetComponent<Hiding>();
    }

    void OnEnable()
    {
        timeRemaining = duration;
        timerImage.fillAmount = 1f;
    }

    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            timerImage.fillAmount = timeRemaining / duration;
        }
        else
        {
            timerImage.fillAmount = 0;
            TimerFinished();
        }
    }

    void TimerFinished()
    {
        _hiding.BugSprayEnded();
    }
}