using UnityEngine;

public class Torch : MonoBehaviour
{
    public Movement _movement;
    public GameObject TorchLight;
    private SpriteRenderer _spriteRenderer;


    void Update()
    {
        if (TorchLight != null)
        {
            // Assuming flipX = true means Facing Right (based on Movement.cs logic)
            // and flipX = false means Facing Left.
            
            float xPos = Mathf.Abs(TorchLight.transform.localPosition.x);

            if (_movement._spriteRenderer.flipX) // Right
            {
                TorchLight.transform.localPosition = new Vector3(xPos, TorchLight.transform.localPosition.y, TorchLight.transform.localPosition.z);
                // 0 degrees usually points Right in 2D
                TorchLight.transform.localRotation = Quaternion.Euler(0, 0, -90);
            }
            else // Left
            {
                TorchLight.transform.localPosition = new Vector3(-xPos, TorchLight.transform.localPosition.y, TorchLight.transform.localPosition.z);
                // 180 degrees points Left
                TorchLight.transform.localRotation = Quaternion.Euler(0, 0, 90);
            }
        }
    }
}
