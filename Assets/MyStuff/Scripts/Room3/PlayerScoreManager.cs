using TMPro;
using UnityEngine;

public class PlayerScoreManager : MonoBehaviour
{
    public static PlayerScoreManager Instance { get; protected set; }
    protected int playerScore;
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
                if (value >= victoryScore)
                {
                    //victory
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
    [SerializeField]
    protected int victoryScore;
    [SerializeField] protected TextMeshProUGUI scoreText;
    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
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
            PlayerScore += fungi.Score;
        }
        else
        {
            PlayerScore -= fungi.Score;
        }
    }
}
