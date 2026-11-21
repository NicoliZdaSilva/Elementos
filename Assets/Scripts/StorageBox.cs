using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class StorageBox : MonoBehaviour
{
    [SerializeField] Transform retrievePoint;
    [SerializeField] TMP_Text counterText;

    public event Action<string, int> CountChanged;

    readonly Dictionary<string, Stack<GameObject>> stored = new();
    readonly HashSet<XRGrabInteractable> inside = new();

    void Start() => Refresh();

    public int GetCount(string itemId) =>
        stored.TryGetValue(itemId, out var s) ? s.Count : 0;

    void OnTriggerEnter(Collider other)
    {
        var grab = other.GetComponentInParent<XRGrabInteractable>();
        if (grab == null || grab.GetComponentInParent<CollectibleItem>() == null) return;

        inside.Add(grab);
        grab.selectExited.RemoveListener(OnReleased);
        grab.selectExited.AddListener(OnReleased);
    }

    void OnTriggerExit(Collider other)
    {
        var grab = other.GetComponentInParent<XRGrabInteractable>();
        if (grab == null) return;

        inside.Remove(grab);
        grab.selectExited.RemoveListener(OnReleased);
    }

    void OnReleased(SelectExitEventArgs args)
    {
        var grab = args.interactableObject as XRGrabInteractable;
        if (grab != null && inside.Contains(grab))
            StartCoroutine(StoreNextFrame(grab));
    }

    IEnumerator StoreNextFrame(XRGrabInteractable grab)
    {
        yield return null;
        if (grab == null || grab.isSelected) yield break;

        var item = grab.GetComponentInParent<CollectibleItem>();
        if (!stored.TryGetValue(item.itemId, out var stack))
            stored[item.itemId] = stack = new Stack<GameObject>();

        inside.Remove(grab);
        grab.selectExited.RemoveListener(OnReleased);

        // guarda o objeto raiz que contém o CollectibleItem
        var root = item.gameObject;
        stack.Push(root);
        root.SetActive(false);

        Refresh();
        CountChanged?.Invoke(item.itemId, stack.Count);
    }

    public void Retrieve(string itemId)
    {
        if (!stored.TryGetValue(itemId, out var stack) || stack.Count == 0) return;

        var go = stack.Pop();
        go.transform.position = retrievePoint.position;

        foreach (var rb in go.GetComponentsInChildren<Rigidbody>())
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        go.SetActive(true);
        Refresh();
        CountChanged?.Invoke(itemId, stack.Count);
    }

    void Refresh()
    {
        if (counterText == null) return;

        var sb = new StringBuilder();
        foreach (var kv in stored)
            if (kv.Value.Count > 0) sb.AppendLine($"{kv.Key} x{kv.Value.Count}");

        counterText.text = sb.Length > 0 ? sb.ToString() : "Vazio";
    }
}