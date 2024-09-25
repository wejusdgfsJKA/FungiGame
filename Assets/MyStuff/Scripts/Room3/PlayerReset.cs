using UnityEngine;

public class PlayerReset : MonoBehaviour
{
    [SerializeField] protected Transform playerPos, player;
    private void OnEnable()
    {
        player.position = playerPos.position;
        player.rotation = playerPos.rotation;
    }
}
