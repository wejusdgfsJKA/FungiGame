using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(Collider))]
public class VolumeDetector : MonoBehaviour
{
    [SerializeField] protected UnityEvent<Collider> OnEnter;
    private void OnTriggerEnter(Collider other)
    {
        OnEnter?.Invoke(other);
    }
}
