using UnityEngine;

public class IKService : MonoBehaviour
{
    public Transform _startPart;
    public Transform _endPart;
    public GameObject _allLimbs;
    public int MAX_ITER = 10;
    public float TOLERANCE = 0.001f;

    private Transform[] _tails;
    private Transform[] _heads;
    private float[] _lengths;
    private int _count;

    void Start()
    {
        _count = _allLimbs.transform.childCount;
        _tails = new Transform[_count];
        _heads = new Transform[_count];
        _lengths = new float[_count];

        for (int i = 0; i < _count; i++)
        {
            Transform limb = _allLimbs.transform.GetChild(i);
            _tails[i] = limb.Find("Tail");
            _heads[i] = limb.Find("Head");
            _lengths[i] = Vector3.Distance(_tails[i].position, _heads[i].position);
        }
    }

    void Update()
    {
        Run();
    }

    private void Run()
    {
        int jointCount = _count + 1;
        Vector3[] joints = new Vector3[jointCount];

        joints[0] = _tails[0].position;
        for (int i = 0; i < _count; i++)
        {
            joints[i + 1] = _heads[i].position;
        }

        Vector3 root = _startPart.position;
        Vector3 target = _endPart.position;

        for (int iteration = 0; iteration < MAX_ITER; iteration++)
        {
            joints[jointCount - 1] = target;
            for (int i = _count - 1; i >= 0; i--)
            {
                Vector3 dir = (joints[i] - joints[i + 1]).normalized;
                joints[i] = joints[i + 1] + dir * _lengths[i];
            }

            joints[0] = root;
            for (int i = 0; i < _count; i++)
            {
                Vector3 dir = (joints[i + 1] - joints[i]).normalized;
                joints[i + 1] = joints[i] + dir * _lengths[i];
            }

            if (Vector3.Distance(joints[jointCount - 1], target) < TOLERANCE)
                break;
        }

        for (int i = 0; i < _count; i++)
        {
            Transform limb = _allLimbs.transform.GetChild(i);

            Vector3 tailOffset = _tails[i].position - limb.position;
            limb.position = joints[i] - tailOffset;

            Vector3 currentDir = (_heads[i].position - _tails[i].position).normalized;
            Vector3 desiredDir = (joints[i + 1] - joints[i]).normalized;

            Quaternion rotation = Quaternion.FromToRotation(currentDir, desiredDir);
            limb.rotation = rotation * limb.rotation;
        }
    }
}
