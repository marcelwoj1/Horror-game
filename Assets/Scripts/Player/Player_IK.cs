using UnityEngine;

public class Player_IK : MonoBehaviour
{
    public IKService _ikService;

    public GameObject Player_IK_Rig;


    // Main body
    private GameObject Head;
    private GameObject UpperTorso;
    private GameObject LowerTorso;


    // Arms
    private GameObject LeftUpperArm;
    private GameObject LeftLowerArm;

    private GameObject RightUpperArm;
    private GameObject RightLowerArm;


    // Legs
    private GameObject LeftUpperLeg;
    private GameObject LeftLowerLeg;
    private GameObject LeftFoot;

    private GameObject RightUpperLeg;
    private GameObject RightLowerLeg;
    private GameObject RightFoot;
    


    void Start()
    {

    }

    void Update()
    {
   
    }
}
