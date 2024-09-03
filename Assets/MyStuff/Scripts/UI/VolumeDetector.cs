using UnityEngine;

public class VolumeDetector : MonoBehaviour
{
    public AudioSource audioSource;
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "FungiHolder")
        {
            audioSource.Play();
        }
    }
}
