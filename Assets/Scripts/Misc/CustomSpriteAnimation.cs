using UnityEngine;

[CreateAssetMenu(fileName = "New Custom Sprite Animation", menuName = "Custom/Sprite Animation")]
public class CustomSpriteAnimation : ScriptableObject
{
    public string AnimationName;
    public Sprite[] Frames;
    public float FrameRate = 10f;
    public bool Loop = true;
    public bool PingPong = false; // If true, animation will reverse when it reaches the end
    public bool Uninterruptible = false; // If true, animation must complete fully before a new animation can play
}
