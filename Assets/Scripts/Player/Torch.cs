using UnityEngine;

public class Torch : MonoBehaviour
{
    public Movement _movement;
    public GameObject TorchLight;
    private Hiding _hiding;
    private EquippedItem _equippedItem;
    float xpos;
    float ypos;

    void Start()
    {
        _hiding = GameObject.Find("Player").GetComponent<Hiding>();
        _equippedItem = GameObject.Find("Player").GetComponent<EquippedItem>();
    }
    void Update()
    {
        if(_movement.AirState != Movement.AirStates.Grounded || _hiding.IsHiding == true)
        {
            TorchLight.SetActive(false);
            _equippedItem.TorchIsLit = false;
        }

        ypos = Mathf.Abs(TorchLight.transform.localPosition.y);
        xpos = Mathf.Abs(TorchLight.transform.localPosition.x);
        if(_movement.MoveState == Movement.MoveStates.Moving)
        {
            xpos = 1.1f;
            ypos = 0.2f;
        }
        else
        {
            xpos = 0.91f;
            ypos = -0.26f;
        }
        if (TorchLight != null)
        {
            // Assuming flipX = true means Facing Right (based on Movement.cs logic)
            // and flipX = false means Facing Left.
            

            if (_movement._spriteRenderer.flipX) // Right
            {
                TorchLight.transform.localPosition = new Vector3(xpos, ypos, TorchLight.transform.localPosition.z);
                // 0 degrees usually points Right in 2D
                TorchLight.transform.localRotation = Quaternion.Euler(0, 0, -68);
            }
            else // Left
            {
                TorchLight.transform.localPosition = new Vector3(-xpos, ypos, TorchLight.transform.localPosition.z);
                // 180 degrees points Left
                TorchLight.transform.localRotation = Quaternion.Euler(0, 0, 68);
            }
        }
    }
}
