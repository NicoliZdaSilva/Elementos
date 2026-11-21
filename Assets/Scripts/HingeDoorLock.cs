using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HingeDoorLock : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] StorageBox storageBox;
    [SerializeField] HingeJoint hinge;
    [SerializeField] Rigidbody doorRb;
    [SerializeField] XRGrabInteractable grabInteractable;

    [Header("Requisito para Destrancar")]
    [SerializeField] string requiredItemId = "Pedra";
    [SerializeField] int requiredAmount = 1;

    [Header("Fechamento Suave após Passar")]
    [SerializeField] float closeSpringForce = 8f;
    [SerializeField] float closeDamper = 6f;

    bool isUnlocked;
    bool autoClosing;

    void Awake()
    {
        if (hinge == null) hinge = GetComponent<HingeJoint>();
        if (doorRb == null) doorRb = GetComponent<Rigidbody>();
        if (grabInteractable == null) grabInteractable = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        if (storageBox != null)
            storageBox.CountChanged += OnInventoryChanged;
    }

    void OnDisable()
    {
        if (storageBox != null)
            storageBox.CountChanged -= OnInventoryChanged;
    }

    void Start()
    {
        LockDoor();

        if (storageBox != null)
            OnInventoryChanged(requiredItemId, storageBox.GetCount(requiredItemId));
    }

    void OnInventoryChanged(string itemId, int newCount)
    {
        if (itemId == requiredItemId && newCount >= requiredAmount && !isUnlocked && !autoClosing)
        {
            UnlockDoor();
        }
    }

    void LockDoor()
    {
        isUnlocked = false;
        if (grabInteractable != null) grabInteractable.enabled = false;
        if (hinge != null) hinge.useSpring = false;
        if (doorRb != null) doorRb.isKinematic = true;
    }

    void UnlockDoor()
    {
        isUnlocked = true;
        autoClosing = false;
        if (hinge != null) hinge.useSpring = false;
        if (doorRb != null) doorRb.isKinematic = false;
        if (grabInteractable != null) grabInteractable.enabled = true;
    }

    public void CloseAndLockAfterPass()
    {
        if (!isUnlocked || autoClosing) return;

        autoClosing = true;
        isUnlocked = false;

        if (grabInteractable != null)
            grabInteractable.enabled = false;

        if (doorRb != null)
            doorRb.isKinematic = false;

        if (hinge != null)
        {
            JointSpring spring = hinge.spring;
            spring.spring = closeSpringForce;
            spring.damper = closeDamper;
            spring.targetPosition = 0f;
            hinge.spring = spring;
            hinge.useSpring = true;
        }
    }

    void Update()
    {
        if (autoClosing && hinge != null && Mathf.Abs(hinge.angle) < 1.5f)
        {
            autoClosing = false;
            LockDoor();
        }
    }
}