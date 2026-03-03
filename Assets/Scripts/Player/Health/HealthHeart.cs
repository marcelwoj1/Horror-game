using UnityEngine;
using UnityEngine.UI;

public class HealthHeart : MonoBehaviour
{
    public Sprite FullHeart, EmptyHeart;

    Image HeartImage;

    private void Awake()
    {
        HeartImage = GetComponent<Image>();
    }

    public void SetHeartState(HeartState state)
    {
        switch (state)
        {
            case HeartState.Full:
                HeartImage.sprite = FullHeart;
                break;
            case HeartState.Empty:
                HeartImage.sprite = EmptyHeart;
                break;
        }
    }
    public enum HeartState
    {
        Full = 1,
        Empty = 0
    }
}
