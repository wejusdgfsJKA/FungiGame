using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FungiManager : MonoBehaviour
{
    public static FungiManager Instance { get; private set; }
    [SerializeField] protected Fungi baseFungiPrefab;
    [SerializeField] protected FungiData[] roster;
    [SerializeField] protected float spawnPosVariation = 1;
    [SerializeField] protected float minSpawnFrequency = 1, maxSpawnFrequency = 5;
    [SerializeField] TextMeshProUGUI text;
    protected Coroutine coroutine;
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
        coroutine = StartCoroutine(SpawnFungi());
    }
    protected IEnumerator SpawnFungi()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnFrequency,
                maxSpawnFrequency));
            for (int i = 0; i < transform.childCount; i++)
            {
                int j = Random.Range(0, roster.Length);
                text.text = j.ToString();
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
        text.text = "A";
        if (pool.Count > 0)
        {
            entity = pool.Dequeue();
            entity.transform.position = transform.GetChild(point).position +
                spawnPosVariation * new Vector3(Random.value, Random.value, 0);
            entity.gameObject.SetActive(true);
            text.text = "B";
        }
        else
        {
            entity = Instantiate(baseFungiPrefab, transform.GetChild(point).
                position + spawnPosVariation * new Vector3(Random.value,
                Random.value, 0), transform.GetChild(point).rotation);
            text.text = "C";
        }
        text.text = "D";
        entity.Init(fungidata);
        text.text = "E";
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
