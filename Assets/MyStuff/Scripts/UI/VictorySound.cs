using UnityEngine;

public class VictorySound : MonoBehaviour
{
    protected AudioSource audioSource;
    [SerializeField] protected GameObject victoryText;
    protected bool playedSound = false;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void OnEnable()
    {
        if (victoryText.activeSelf && !playedSound)
        {
            audioSource.Play();
            playedSound = true;
        }
    }
}
