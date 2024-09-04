using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; protected set; }
    [SerializeField] protected GameObject[] rooms;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void LoadRoom1()
    {
        rooms[1].SetActive(false);
        rooms[2].SetActive(false);
        rooms[0].SetActive(true);
    }
    public void LoadRoom2()
    {
        rooms[0].SetActive(false);
        rooms[2].SetActive(false);
        rooms[1].SetActive(true);
    }
    public void LoadRoom3()
    {
        rooms[0].SetActive(false);
        rooms[1].SetActive(false);
        rooms[2].SetActive(true);
    }
}
