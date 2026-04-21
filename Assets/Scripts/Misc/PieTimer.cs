using UnityEngine;
using UnityEngine.UI;

public class PieTimer : MonoBehaviour
{
    public Image timerImage;
    public float duration = 15f;

    private float timeRemaining;
    private PlayerManager _playerManager;

    void Start()
    {
        timeRemaining = duration;
        timerImage.fillAmount = 1f;
        _playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
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
        if(gameObject.name == "BugSprayTimer")
        {
            _playerManager.BugSprayEnded();
        }
    }
}