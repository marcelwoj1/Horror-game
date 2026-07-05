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
    



    public Transform _startPart;
    public Transform _endPart;
    public GameObject _allLimbs;
    private IKChain _tempChain;


    void Start()
    {
       
        _tempChain = new IKChain
        {
            root = _startPart,
            target = _endPart,
            allLimbs = _allLimbs
        };
        _ikService.InitializeChain(_tempChain);
      
    }

    void Update()
    {
      
        _ikService.Solve(_tempChain);
       
    }
}
