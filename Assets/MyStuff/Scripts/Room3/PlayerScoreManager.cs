using TMPro;
using UnityEngine;

public class PlayerScoreManager : MonoBehaviour
{
    public static PlayerScoreManager Instance { get; protected set; }
    protected int playerScore;
    [SerializeField] protected GameObject victoryScreen;
    public int PlayerScore
    {
        get
        {
            return playerScore;
        }
        protected set
        {
            if (playerScore != value)
            {
                if (value >= VictoryScore)
                {
                    //victory
                    victoryScreen.SetActive(true);
                    RoomManager.Instance.LoadRoom1();
                    return;
                }
                if (value <= 0)
                {
                    //game over
                    RoomManager.Instance.LoadRoom1();
                    return;
                }
                scoreText.text = "Score: " + value.ToString();
                playerScore = value;
            }
        }
    }
    [SerializeField]
    protected int initialScore;
    [field: SerializeField]
    public int VictoryScore { get; protected set; }
    [SerializeField] protected TextMeshProUGUI scoreText;
    [SerializeField] protected AudioClip goodClip, badClip;
    protected AudioSource audioSource;
    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        audioSource = GetComponent<AudioSource>();
    }
    private void OnEnable()
    {
        ResetScore();
    }
    public void ResetScore()
    {
        PlayerScore = initialScore;
    }
    public void Contact(FungiData fungi)
    {
        //a fungi has made contact with the player
        if (fungi.Beneficial)
        {
            audioSource.PlayOneShot(goodClip);
            PlayerScore += fungi.Score;
        }
        else
        {
            audioSource.PlayOneShot(badClip);
            PlayerScore -= fungi.Score;
        }
    }
}
