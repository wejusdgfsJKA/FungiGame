using TMPro;
using UnityEngine;
using System.Text;

public class PlayerScoreManager : MonoBehaviour
{
    public static PlayerScoreManager Instance { get; protected set; }
    protected int playerScore;
    [SerializeField] protected GameObject victoryScreen;

    // StringBuilder for efficient string operations
    private static StringBuilder stringBuilder = new StringBuilder(32);
    private const string SCORE_PREFIX = "Score: ";
    private const string SCORE_SUFFIX = "/30";
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
                // Use StringBuilder to avoid string allocation garbage
                stringBuilder.Clear();
                stringBuilder.Append(SCORE_PREFIX);
                stringBuilder.Append(value);
                stringBuilder.Append(SCORE_SUFFIX);
                scoreText.text = stringBuilder.ToString();
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
