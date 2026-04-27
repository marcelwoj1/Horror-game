using UnityEngine;

public class Torch : MonoBehaviour
{
    [Header("Components")]
    public GameObject TorchLight;
    public Movement _movement;
    private PlayerManager _playerManager;
    private EquippedItem _equippedItem;
    
    [Header("Variables")]
    private float xpos;
    private float ypos;

    //Assigns variables in Start
    void Start()
    {
        _playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        _equippedItem = GameObject.Find("Player").GetComponent<EquippedItem>();
    }
    void Update()
    {
        //Turns off torch if player is not grounded or is hiding
        if(_movement.AirState != Movement.AirStates.Grounded || _playerManager.IsHiding == true)
        {
            TorchLight.SetActive(false);
            _equippedItem.TorchIsLit = false;
        }

        //Gets the absolute position of the torch
        ypos = Mathf.Abs(TorchLight.transform.localPosition.y);
        xpos = Mathf.Abs(TorchLight.transform.localPosition.x);

        //Sets the position of the torch based on whether the player is moving or not
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
            //Sets the position and rotation of the torch based on whether the player is facing left or right
            if (_movement._spriteRenderer.flipX) // Right
            {
                TorchLight.transform.localPosition = new Vector3(xpos, ypos, TorchLight.transform.localPosition.z);
                TorchLight.transform.localRotation = Quaternion.Euler(0, 0, -68);
            }
            else // Left
            {
                TorchLight.transform.localPosition = new Vector3(-xpos, ypos, TorchLight.transform.localPosition.z);
                TorchLight.transform.localRotation = Quaternion.Euler(0, 0, 68);
            }
        }
    }
}
