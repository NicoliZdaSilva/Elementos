using UnityEngine;
using Unity.XR.CoreUtils;

public class DoorPassTrigger : MonoBehaviour
{
    [SerializeField] HingeDoorLock doorLock;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponentInParent<XROrigin>() != null)
        {
            if (doorLock != null)
                doorLock.CloseAndLockAfterPass();
        }
    }
}