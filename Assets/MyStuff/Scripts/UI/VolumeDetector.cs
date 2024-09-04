using UnityEngine;
using UnityEngine.Events;

public class VolumeDetector : MonoBehaviour
{
    [SerializeField] protected UnityEvent OnEnter;
    private void OnTriggerEnter(Collider other)
    {

    }
}
