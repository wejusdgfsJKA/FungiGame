using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] protected GameObject[] rooms;
    public RoomManager Instance { get; protected set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void LoadRoom1()
    {
        rooms[0].SetActive(true);
        rooms[1].SetActive(false);
        rooms[2].SetActive(false);
    }
    public void LoadRoom2()
    {
        rooms[0].SetActive(false);
        rooms[1].SetActive(true);
        rooms[2].SetActive(false);
    }
    public void LoadRoom3()
    {
        rooms[0].SetActive(false);
        rooms[1].SetActive(false);
        rooms[2].SetActive(true);
    }
}
