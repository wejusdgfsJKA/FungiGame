using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance { get; private set; }
    [SerializeField] protected Fungi baseFungi;
    [SerializeField] protected FungiData[] roster;
    [SerializeField] protected Transform[] spawnPoints;
    protected Queue<Fungi> pool = new();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void OnEnable()
    {
        //start spawning
    }
    private void OnDisable()
    {
        //stop spawning
    }
    protected void Spawn(int point, FungiData fungi)
    {
        if (pool.Count > 0)
        {
            var entity = pool.Dequeue();
            entity.Init(fungi);
            entity.transform.position = spawnPoints[point].position;
            entity.gameObject.SetActive(true);
            return;
        }
        Instantiate(baseFungi, spawnPoints[point].position, spawnPoints[point].rotation);
    }
    public void HasDied(Fungi fungi)
    {
        pool.Enqueue(fungi);
    }
}
