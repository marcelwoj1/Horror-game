using UnityEngine;

public class PickFlashlightup : MonoBehaviour
{
    private Movement _movement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _movement = GameObject.Find("Player").GetComponent<Movement>();
    }

    public void PickUp()
    {
        _movement.PickFlashlightUp();
    }
}
