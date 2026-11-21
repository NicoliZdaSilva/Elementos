using UnityEngine;

public class FollowPlayerYaw : MonoBehaviour
{
    [SerializeField] Transform head;   // arraste a Main Camera do XR Origin
    [SerializeField] Vector3 offset = new Vector3(0.3f, -0.6f, 0.2f);

    void LateUpdate()
    {
        var yaw = Quaternion.Euler(0, head.eulerAngles.y, 0);
        transform.SetPositionAndRotation(head.position + yaw * offset, yaw);
    }
}