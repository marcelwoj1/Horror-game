using UnityEngine;

[System.Serializable]
public class IKChain
{
    public Transform root;
    public Transform target;
    public GameObject allLimbs;
    public int MAX_ITER = 10;
    public float tolerance = 0.001f;

    public Transform[] tails;
    public Transform[] heads;
    public float[] lengths;
    public int count;
}

public class IKService : MonoBehaviour
{
    public void InitializeChain(IKChain chain)
    {
        chain.count = chain.allLimbs.transform.childCount;
        chain.tails = new Transform[chain.count];
        chain.heads = new Transform[chain.count];
        chain.lengths = new float[chain.count];

        for (int i = 0; i < chain.count; i++)
        {
            Transform limb = chain.allLimbs.transform.GetChild(i);
            int idx = int.Parse(limb.name) - 1;
            chain.tails[idx] = limb.Find("Tail");
            chain.heads[idx] = limb.Find("Head");
            chain.lengths[idx] = Vector3.Distance(chain.tails[idx].position, chain.heads[idx].position);
        }
    }

    public void Solve(IKChain chain)
    {
        int jointCount = chain.count + 1;
        Vector3[] joints = new Vector3[jointCount];

        joints[0] = chain.tails[0].position;
        for (int i = 0; i < chain.count; i++)
        {
            joints[i + 1] = chain.heads[i].position;
        }

        Vector3 root = chain.root.position;
        Vector3 target = chain.target.position;

        for (int iteration = 0; iteration < chain.MAX_ITER; iteration++)
        {
            joints[jointCount - 1] = target;
            for (int i = chain.count - 1; i >= 0; i--)
            {
                Vector3 dir = (joints[i] - joints[i + 1]).normalized;
                joints[i] = joints[i + 1] + dir * chain.lengths[i];
            }

            joints[0] = root;
            for (int i = 0; i < chain.count; i++)
            {
                Vector3 dir = (joints[i + 1] - joints[i]).normalized;
                joints[i + 1] = joints[i] + dir * chain.lengths[i];
            }

            if (Vector3.Distance(joints[jointCount - 1], target) < chain.tolerance)
                break;
        }

        for (int i = 0; i < chain.count; i++)
        {
            Transform limb = chain.allLimbs.transform.GetChild(i);
            int idx = int.Parse(limb.name) - 1;

            Vector3 tailOffset = chain.tails[idx].position - limb.position;
            limb.position = joints[idx] - tailOffset;

            Vector3 currentDir = (chain.heads[idx].position - chain.tails[idx].position).normalized;
            Vector3 desiredDir = (joints[idx + 1] - joints[idx]).normalized;

            Quaternion rotation = Quaternion.FromToRotation(currentDir, desiredDir);
            limb.rotation = rotation * limb.rotation;
        }
    }
}
