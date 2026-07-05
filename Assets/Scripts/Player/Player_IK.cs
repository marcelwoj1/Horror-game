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
    private GameObject RightArmLimbs;

    // Legs
    private GameObject LeftLegLimbs;
    private GameObject RightLegLimbs;

    // Connector nodes
    private GameObject IK_ATTACH_L; // For left arm
    private GameObject IK_ATTACH_R; // For right arm
    private GameObject IK_LEG_ATTACH_L; // For left leg
    private GameObject IK_LEG_ATTACH_R; // For right leg


    // Target
    public GameObject TargetPositions;
    private Transform LeftArmTarget;
    private Transform RightArmTarget;
    private Transform LeftLegTarget;
    private Transform RightLegTarget;

    // Chains
    private IKChain _leftArmChain;
    private IKChain _rightArmChain;
    private IKChain _leftLegChain;
    private IKChain _rightLegChain;


    void Start()
    {
        if (Player_IK_Rig != null)
        {
            Head = FindChildByName("Head");
            UpperTorso = FindChildByName("UpperTorso");
            LowerTorso = FindChildByName("LowerTorso");

            LeftArmLimbs = FindChildByName("LeftArm");
            RightArmLimbs = FindChildByName("RightArm");
            LeftLegLimbs = FindChildByName("LeftLeg");
            RightLegLimbs = FindChildByName("RightLeg");

            IK_ATTACH_L = FindChildByName("IK_ATTACH_L");
            IK_ATTACH_R = FindChildByName("IK_ATTACH_R");
            IK_LEG_ATTACH_L = FindChildByName("IK_LEG_ATTACH_L");
            IK_LEG_ATTACH_R = FindChildByName("IK_LEG_ATTACH_R");
        }

        if (TargetPositions == null)
        {
            TargetPositions = GameObject.Find("TargetPositions");
        }

        if (TargetPositions != null)
        {
            LeftArmTarget = FindChildInRoot(TargetPositions, "LeftArm");
            RightArmTarget = FindChildInRoot(TargetPositions, "RightArm");
            LeftLegTarget = FindChildInRoot(TargetPositions, "LeftLeg");
            RightLegTarget = FindChildInRoot(TargetPositions, "RightLeg");
        }

        _leftArmChain = new IKChain
        {
            root = IK_ATTACH_L.transform,
            target = LeftArmTarget,
            allLimbs = LeftArmLimbs
        };
        _ikService.InitializeChain(_leftArmChain);

        _rightArmChain = new IKChain
        {
            root = IK_ATTACH_R.transform,
            target = RightArmTarget,
            allLimbs = RightArmLimbs
        };
        _ikService.InitializeChain(_rightArmChain);

        _leftLegChain = new IKChain
        {
            root = IK_LEG_ATTACH_L.transform,
            target = LeftLegTarget,
            allLimbs = LeftLegLimbs
        };
        _ikService.InitializeChain(_leftLegChain);

        _rightLegChain = new IKChain
        {
            root = IK_LEG_ATTACH_R.transform,
            target = RightLegTarget,
            allLimbs = RightLegLimbs
        };
        _ikService.InitializeChain(_rightLegChain);
    }

    void Update()
    {
        _ikService.Solve(_leftArmChain);
        _ikService.Solve(_rightArmChain);
        _ikService.Solve(_leftLegChain);
        _ikService.Solve(_rightLegChain);
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

    private Transform FindChildInRoot(GameObject root, string name)
    {
        if (root == null) return null;
        Transform[] children = root.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            if (child.name == name)
            {
                return child;
            }
        }
        return null;
    }
}

