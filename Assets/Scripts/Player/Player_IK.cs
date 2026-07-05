using System.Collections;
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

    private Vector3 _leftArmBaseLocalPos;
    private Vector3 _rightArmBaseLocalPos;
    private Vector3 _leftLegBaseLocalPos;
    private Vector3 _rightLegBaseLocalPos;
    
    private Vector3 _leftArmOffsetFromTorso;
    private Vector3 _rightArmOffsetFromTorso;
    private Vector3 _leftLegOffsetFromTorso;
    private Vector3 _rightLegOffsetFromTorso;
    
    private Movement _movement;
    private Rigidbody2D _rb;
    private SpriteRenderer _leftAxeSpriteRenderer;
    private SpriteRenderer _rightAxeSpriteRenderer;
    private bool _facingRight;

    // Chains
    private IKChain _leftArmChain;
    private IKChain _rightArmChain;
    private IKChain _leftLegChain;
    private IKChain _rightLegChain;

    // Swing Animation variables
    private Coroutine _swingCoroutine;
    private Vector3 _leftArmSwingOffset = Vector3.zero;
    private Vector3 _rightArmSwingOffset = Vector3.zero;

    
    public float swaySpeed = 1f;
    public float swayMagnitude = 0.05f;

    private float _leftArmSwaySeedX;
    private float _leftArmSwaySeedY;
    private float _rightArmSwaySeedX;
    private float _rightArmSwaySeedY;

    
    public float walkBobSpeed = 10f;
    public float walkBobMagnitude = 0.03f;
    private float _currentWalkBobY = 0f;

    
    public float cursorFollowMagnitude = 0.45f;
    public float cursorFollowScaling = 0.7f;


    void Start()
    {
        _leftArmSwaySeedX = Random.Range(0f, 1000f);
        _leftArmSwaySeedY = Random.Range(0f, 1000f);
        _rightArmSwaySeedX = Random.Range(0f, 1000f);
        _rightArmSwaySeedY = Random.Range(0f, 1000f);

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

            if (LeftArmTarget != null) _leftArmBaseLocalPos = LeftArmTarget.localPosition;
            if (RightArmTarget != null) _rightArmBaseLocalPos = RightArmTarget.localPosition;
            if (LeftLegTarget != null) _leftLegBaseLocalPos = LeftLegTarget.localPosition;
            if (RightLegTarget != null) _rightLegBaseLocalPos = RightLegTarget.localPosition;

            if (LeftArmTarget != null && UpperTorso != null)
            {
                _leftArmOffsetFromTorso = LeftArmTarget.position - UpperTorso.transform.position;
            }
            if (RightArmTarget != null && UpperTorso != null)
            {
                _rightArmOffsetFromTorso = RightArmTarget.position - UpperTorso.transform.position;
            }
            if (LeftLegTarget != null && LowerTorso != null)
            {
                _leftLegOffsetFromTorso = LeftLegTarget.position - LowerTorso.transform.position;
            }
            if (RightLegTarget != null && LowerTorso != null)
            {
                _rightLegOffsetFromTorso = RightLegTarget.position - LowerTorso.transform.position;
            }
        }

        if (LeftArmTarget != null)
        {
            Transform axeTransform = FindChildInRoot(LeftArmTarget.gameObject, "Axe");
            if (axeTransform != null)
            {
                _leftAxeSpriteRenderer = axeTransform.GetComponent<SpriteRenderer>();
            }
        }

        if (RightArmTarget != null)
        {
            Transform axeTransform = FindChildInRoot(RightArmTarget.gameObject, "Axe");
            if (axeTransform != null)
            {
                _rightAxeSpriteRenderer = axeTransform.GetComponent<SpriteRenderer>();
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
        // Check movement state
        bool isMoving = false;
        if (_movement != null)
        {
            isMoving = _movement.MoveState == Movement.MoveStates.Moving && _movement.AirState == Movement.AirStates.Grounded;
        }
        else if (_rb != null)
        {
            isMoving = Mathf.Abs(_rb.linearVelocityX) > 0.1f;
        }

        // Calculate Walk Bob
        float targetBobY = 0f;
        if (isMoving)
        {
            targetBobY = Mathf.Sin(Time.time * walkBobSpeed) * walkBobMagnitude;
        }
        _currentWalkBobY = Mathf.Lerp(_currentWalkBobY, targetBobY, Time.deltaTime * 10f);
        Vector3 walkBob = new Vector3(0f, _currentWalkBobY, 0f);

        // Calculate Perlin Noise sway offsets for the arms
        Vector3 leftSway = GetPerlinSway(_leftArmSwaySeedX, _leftArmSwaySeedY);
        Vector3 rightSway = GetPerlinSway(_rightArmSwaySeedX, _rightArmSwaySeedY);

        // Calculate Cursor Follow offsets for the arms
        Vector3 leftMouseOffset = Vector3.zero;
        Vector3 rightMouseOffset = Vector3.zero;
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;

            if (UpperTorso != null)
            {
                Vector3 leftShoulder = UpperTorso.transform.position + _leftArmOffsetFromTorso;
                Vector3 leftDir = mouseWorldPos - leftShoulder;
                if (leftDir.sqrMagnitude > 0.0001f)
                {
                    leftMouseOffset = leftDir.normalized * Mathf.Min(leftDir.magnitude * cursorFollowScaling, cursorFollowMagnitude);
                }

                Vector3 rightShoulder = UpperTorso.transform.position + _rightArmOffsetFromTorso;
                Vector3 rightDir = mouseWorldPos - rightShoulder;
                if (rightDir.sqrMagnitude > 0.0001f)
                {
                    rightMouseOffset = rightDir.normalized * Mathf.Min(rightDir.magnitude * cursorFollowScaling, cursorFollowMagnitude);
                }
            }
            else
            {
                if (LeftArmTarget != null)
                {
                    Vector3 leftDir = mouseWorldPos - LeftArmTarget.position;
                    if (leftDir.sqrMagnitude > 0.0001f)
                    {
                        leftMouseOffset = leftDir.normalized * Mathf.Min(leftDir.magnitude * cursorFollowScaling, cursorFollowMagnitude);
                    }
                }
                if (RightArmTarget != null)
                {
                    Vector3 rightDir = mouseWorldPos - RightArmTarget.position;
                    if (rightDir.sqrMagnitude > 0.0001f)
                    {
                        rightMouseOffset = rightDir.normalized * Mathf.Min(rightDir.magnitude * cursorFollowScaling, cursorFollowMagnitude);
                    }
                }
            }
        }

        // Update Arm Target world positions relative to UpperTorso (including swing, sway, walk bob & cursor follow offsets)
        if (LeftArmTarget != null)
        {
            if (UpperTorso != null)
            {
                LeftArmTarget.position = UpperTorso.transform.position + _leftArmOffsetFromTorso + _leftArmSwingOffset + leftSway + walkBob + leftMouseOffset;
            }
            else
            {
                LeftArmTarget.localPosition = _leftArmBaseLocalPos + _leftArmSwingOffset + leftSway + walkBob + leftMouseOffset;
            }
        }

        if (RightArmTarget != null)
        {
            if (UpperTorso != null)
            {
                RightArmTarget.position = UpperTorso.transform.position + _rightArmOffsetFromTorso + _rightArmSwingOffset + rightSway + walkBob + rightMouseOffset;
            }
            else
            {
                RightArmTarget.localPosition = _rightArmBaseLocalPos + _rightArmSwingOffset + rightSway + walkBob + rightMouseOffset;
            }
        }


        float dirSign = 1f;
        if (_rb != null)
        {
            if (_rb.linearVelocityX < -0.05f) dirSign = -1f;
            else if (_rb.linearVelocityX > 0.05f) dirSign = 1f;
        }

        if (isMoving)
        {
            float time = Time.time * runAnimationSpeed;
            if (LeftLegTarget != null)
            {
                if (LowerTorso != null)
                {
                    Vector3 leftOffset = new Vector3(Mathf.Sin(time) * strideLength * dirSign, Mathf.Max(0, Mathf.Cos(time)) * stepHeight, 0);
                    LeftLegTarget.position = LowerTorso.transform.position + _leftLegOffsetFromTorso + leftOffset;
                }
                else
                {
                    Vector3 leftOffset = new Vector3(Mathf.Sin(time) * strideLength * dirSign, Mathf.Max(0, Mathf.Cos(time)) * stepHeight, 0);
                    LeftLegTarget.localPosition = _leftLegBaseLocalPos + leftOffset;
                }
            }
            if (RightLegTarget != null)
            {
                float rightTime = time + Mathf.PI;
                if (LowerTorso != null)
                {
                    Vector3 rightOffset = new Vector3(Mathf.Sin(rightTime) * strideLength * dirSign, Mathf.Max(0, Mathf.Cos(rightTime)) * stepHeight, 0);
                    RightLegTarget.position = LowerTorso.transform.position + _rightLegOffsetFromTorso + rightOffset;
                }
                else
                {
                    Vector3 rightOffset = new Vector3(Mathf.Sin(rightTime) * strideLength * dirSign, Mathf.Max(0, Mathf.Cos(rightTime)) * stepHeight, 0);
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

        bool hasAxe = _movement != null && _movement.AxeEquipped;

        bool faceRight = false;
        if (_movement != null && _movement.HeadSpriteRenderer != null)
        {
            faceRight = _movement.HeadSpriteRenderer.flipX;
        }
        else if (_rb != null)
        {
            if (_rb.linearVelocityX < -0.05f) _facingRight = false;
            else if (_rb.linearVelocityX > 0.05f) _facingRight = true;
            faceRight = _facingRight;
        }

        if (_leftAxeSpriteRenderer != null)
        {
            bool leftActive = hasAxe && !faceRight;
            if (_leftAxeSpriteRenderer.enabled != leftActive)
            {
                _leftAxeSpriteRenderer.enabled = leftActive;
            }
        }

        if (_rightAxeSpriteRenderer != null)
        {
            bool rightActive = hasAxe && faceRight;
            if (_rightAxeSpriteRenderer.enabled != rightActive)
            {
                _rightAxeSpriteRenderer.enabled = rightActive;
            }
        }

        _ikService.Solve(_leftArmChain);
        _ikService.Solve(_rightArmChain);
        _ikService.Solve(_leftLegChain);
        _ikService.Solve(_rightLegChain);
    }

    public void PlayAttackSwing()
    {
        bool faceRight = false;
        if (_movement != null && _movement.HeadSpriteRenderer != null)
        {
            faceRight = _movement.HeadSpriteRenderer.flipX;
        }
        else if (_rb != null)
        {
            faceRight = _facingRight;
        }

        if (_swingCoroutine != null)
        {
            StopCoroutine(_swingCoroutine);
            _leftArmSwingOffset = Vector3.zero;
            _rightArmSwingOffset = Vector3.zero;
        }

        _swingCoroutine = StartCoroutine(SwingAnimation(faceRight));
    }

    private IEnumerator SwingAnimation(bool isRightArm)
    {
        float duration = 0.25f;
        float elapsed = 0f;

        bool faceRight = false;
        if (_movement != null && _movement.HeadSpriteRenderer != null)
        {
            faceRight = _movement.HeadSpriteRenderer.flipX;
        }
        else if (_rb != null)
        {
            faceRight = _facingRight;
        }
        float finalDir = faceRight ? 1f : -1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            Vector3 offset = Vector3.zero;
            if (t < 0.5f)
            {
                float subT = t / 0.25f;
                float x = Mathf.Lerp(0f, -0.4f * finalDir, subT);
                float y = Mathf.Lerp(0f, 0.4f, subT);
                offset = new Vector3(x, y, 0f);
            }
            else if (t < 0.7f)
            {
                float subT = (t - 0.25f) / 0.35f;
                float x = Mathf.Lerp(-0.4f * finalDir, 0.8f * finalDir, subT);
                float y = Mathf.Lerp(0.4f, -0.4f, subT);
                offset = new Vector3(x, y, 0f);
            }
            else
            {

                float subT = (t - 0.6f) / 0.4f;
                float x = Mathf.Lerp(0.8f * finalDir, 0f, subT);
                float y = Mathf.Lerp(-0.4f, 0f, subT);
                offset = new Vector3(x, y, 0f);
            }

            if (isRightArm)
            {
                _rightArmSwingOffset = offset;
            }
            else
            {
                _leftArmSwingOffset = offset;
            }

            yield return null;
        }

        if (isRightArm)
        {
            _rightArmSwingOffset = Vector3.zero;
        }
        else
        {
            _leftArmSwingOffset = Vector3.zero;
        }
    }

    private Vector3 GetPerlinSway(float seedX, float seedY)
    {
        float time = Time.time * swaySpeed;
        float x = (Mathf.PerlinNoise(seedX + time, 0f) - 0.5f) * 2f * swayMagnitude;
        float y = (Mathf.PerlinNoise(0f, seedY + time) - 0.5f) * 2f * swayMagnitude;
        return new Vector3(x, y, 0f);
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

