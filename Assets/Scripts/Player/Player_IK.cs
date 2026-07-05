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

    
    public float runAnimationSpeed = 10f;
    public float strideLength = 0.5f;
    public float stepHeight = 0.25f;

    private Vector3 _leftLegBaseLocalPos;
    private Vector3 _rightLegBaseLocalPos;
    private Vector3 _leftLegOffsetFromTorso;
    private Vector3 _rightLegOffsetFromTorso;
    private Movement _movement;
    private Rigidbody2D _rb;

    // Chains
    private IKChain _leftArmChain;
    private IKChain _rightArmChain;
    private IKChain _leftLegChain;
    private IKChain _rightLegChain;


    void Start()
    {
        _movement = GetComponent<Movement>();
        if (_movement == null) _movement = GetComponentInParent<Movement>();
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null) _rb = GetComponentInParent<Rigidbody2D>();

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

            if (LeftLegTarget != null) _leftLegBaseLocalPos = LeftLegTarget.localPosition;
            if (RightLegTarget != null) _rightLegBaseLocalPos = RightLegTarget.localPosition;

            if (LeftLegTarget != null && LowerTorso != null)
            {
                _leftLegOffsetFromTorso = LeftLegTarget.position - LowerTorso.transform.position;
            }
            if (RightLegTarget != null && LowerTorso != null)
            {
                _rightLegOffsetFromTorso = RightLegTarget.position - LowerTorso.transform.position;
            }
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
        bool isMoving = false;
        if (_movement != null)
        {
            isMoving = _movement.MoveState == Movement.MoveStates.Moving && _movement.AirState == Movement.AirStates.Grounded;
        }
        else if (_rb != null)
        {
            isMoving = Mathf.Abs(_rb.linearVelocityX) > 0.1f;
        }

        if (isMoving)
        {
            float time = Time.time * runAnimationSpeed;
            if (LeftLegTarget != null)
            {
                if (LowerTorso != null)
                {
                    Vector3 leftOffset = new Vector3(Mathf.Sin(time) * strideLength, Mathf.Max(0, Mathf.Cos(time)) * stepHeight, 0);
                    LeftLegTarget.position = LowerTorso.transform.position + _leftLegOffsetFromTorso + leftOffset;
                }
                else
                {
                    Vector3 leftOffset = new Vector3(Mathf.Sin(time) * strideLength, Mathf.Max(0, Mathf.Cos(time)) * stepHeight, 0);
                    LeftLegTarget.localPosition = _leftLegBaseLocalPos + leftOffset;
                }
            }
            if (RightLegTarget != null)
            {
                float rightTime = time + Mathf.PI;
                if (LowerTorso != null)
                {
                    Vector3 rightOffset = new Vector3(Mathf.Sin(rightTime) * strideLength, Mathf.Max(0, Mathf.Cos(rightTime)) * stepHeight, 0);
                    RightLegTarget.position = LowerTorso.transform.position + _rightLegOffsetFromTorso + rightOffset;
                }
                else
                {
                    Vector3 rightOffset = new Vector3(Mathf.Sin(rightTime) * strideLength, Mathf.Max(0, Mathf.Cos(rightTime)) * stepHeight, 0);
                    RightLegTarget.localPosition = _rightLegBaseLocalPos + rightOffset;
                }
            }
        }
        else
        {
            if (LeftLegTarget != null)
            {
                if (LowerTorso != null)
                {
                    Vector3 targetPos = LowerTorso.transform.position + _leftLegOffsetFromTorso;
                    LeftLegTarget.position = Vector3.Lerp(LeftLegTarget.position, targetPos, Time.deltaTime * 5f);
                }
                else
                {
                    LeftLegTarget.localPosition = Vector3.Lerp(LeftLegTarget.localPosition, _leftLegBaseLocalPos, Time.deltaTime * 5f);
                }
            }
            if (RightLegTarget != null)
            {
                if (LowerTorso != null)
                {
                    Vector3 targetPos = LowerTorso.transform.position + _rightLegOffsetFromTorso;
                    RightLegTarget.position = Vector3.Lerp(RightLegTarget.position, targetPos, Time.deltaTime * 5f);
                }
                else
                {
                    RightLegTarget.localPosition = Vector3.Lerp(RightLegTarget.localPosition, _rightLegBaseLocalPos, Time.deltaTime * 5f);
                }
            }
        }

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

