using UnityEngine;

public class PlayerScoreManager : MonoBehaviour
{
    public static PlayerScoreManager Instance { get; protected set; }
    public int PlayerScore { get; protected set; }
    [SerializeField]
    protected int initialScore;
    [SerializeField]
    protected int victoryScore;
    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
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
            if (PlayerScore >= victoryScore)
            {
                //victory
                RoomManager.Instance.LoadRoom1();
                return;
            }
        }
        else
        {
            PlayerScore -= fungi.Score;
            if (PlayerScore <= 0)
            {
                //game over
                RoomManager.Instance.LoadRoom1();
                return;
            }
        }
    }
}
