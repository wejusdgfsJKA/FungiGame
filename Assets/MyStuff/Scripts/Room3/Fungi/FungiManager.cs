using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FungiManager : MonoBehaviour
{
    public static FungiManager Instance { get; private set; }
    [SerializeField] protected Fungi baseFungiPrefab;
    [SerializeField] protected FungiData[] roster;
    [SerializeField] protected float spawnFrequency = 2;
    [SerializeField] protected AnimationCurve curve;
    protected WaitForSeconds wait;
    protected Coroutine coroutine;
    protected Queue<Fungi> pool = new();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        wait = new WaitForSeconds(spawnFrequency);
    }
    private void OnEnable()
    {
        //start spawning
        coroutine = StartCoroutine(SpawnFungi());
    }
    protected IEnumerator SpawnFungi()
    {
        while (true)
        {
            yield return wait;
            for (int i = 0; i < transform.childCount; i++)
            {
                int j = (roster.Length + 1) * (int)curve.Evaluate(Random.value);
                if (j < roster.Length)
                {
                    Spawn(i, roster[j]);
                }
            }
        }
    }
    protected void Spawn(int point, FungiData fungidata)
    {
        Fungi entity;
        if (pool.Count > 0)
        {
            entity = pool.Dequeue();
            entity.transform.position = transform.GetChild(point).position;
            entity.gameObject.SetActive(true);
        }
        else
        {
            entity = Instantiate(baseFungiPrefab, transform.GetChild(point).position,
                transform.GetChild(point).rotation);
        }
        entity.Init(fungidata);
    }
    public void HasDied(Fungi fungi)
    {
        pool.Enqueue(fungi);
    }
    private void OnDisable()
    {
        //stop spawning
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }
}
