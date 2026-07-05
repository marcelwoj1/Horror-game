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
    private GameObject LeftArmLimbs;
    

    // Connector nodes
    private GameObject IK_ATTACH_L;

  


    // Temporary
    public Transform _startPart;
    public Transform _endPart;
    //public GameObject _allLimbs;
    private IKChain _leftArmChain;


    void Start()
    {
        if (Player_IK_Rig != null)
        {
            Head = FindChildByName("Head");
            UpperTorso = FindChildByName("UpperTorso");
            LowerTorso = FindChildByName("LowerTorso");

            LeftArmLimbs = FindChildByName("LeftArm");

            IK_ATTACH_L = FindChildByName("IK_ATTACH_L");
     
        }

        _leftArmChain = new IKChain
        {
            root = IK_ATTACH_L.transform,
            target = _endPart,
            allLimbs = LeftArmLimbs
        };
        _ikService.InitializeChain(_leftArmChain);
      
    }

    void Update()
    {
      
        _ikService.Solve(_leftArmChain);
       
    }

    private GameObject FindChildByName(string name)
    {
        Transform[] children = Player_IK_Rig.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            if (child.name == name)
            {
                return child.gameObject;
            }
        }
        return null;
    }
}

