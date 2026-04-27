using UnityEngine;
using UnityEngine.UI;

//Handles the individual heart visuals
public class HealthHeart : MonoBehaviour
{
    public Sprite FullHeart, EmptyHeart;

    Image HeartImage;

    //Gets the heart image
    private void Awake()
    {
        HeartImage = GetComponent<Image>();
    }

    //Sets the heart state to full or empty
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

    //Enum for heart states
    public enum HeartState
    {
        Full = 1,
        Empty = 0
    }
}
