using UnityEngine;
public class PropManager : MonoBehaviour
{
    [SerializeField] protected PropData microscope1;
    [SerializeField] protected PropData microscope1Lens;
    [SerializeField] protected PropData microscope2;
    [SerializeField] protected PropData microscope2Lens;
    [SerializeField] protected PropData[] fungiHolders;
    protected bool spawnedMic1, spawnedMic2;
    protected bool spawnedFungi;
    private void OnEnable()
    {
        if (!spawnedFungi)
        {
            spawnedFungi = true;
            for (int i = 0; i < fungiHolders.Length; i++)
            {
                SpawnProp(fungiHolders[i]);
            }
        }
        GenerateMicroscope2();
        GenerateMicroscope1();
    }
    public void GenerateFungiHolders()
    {
        for (int i = 0; i < fungiHolders.Length; i++)
        {
            SpawnProp(fungiHolders[i]);
        }
    }
    public void GenerateMicroscope1()
    {
        if (!spawnedMic1)
        {
            spawnedMic1 = true;
            SpawnProp(microscope1);
            SpawnProp(microscope1Lens);
        }
    }
    public void GenerateMicroscope2()
    {
        if (!spawnedMic2 && spawnedMic1)
        {
            spawnedMic2 = true;
            //SpawnProp(microscope2);
            //SpawnProp(microscope2Lens);
        }
    }
    protected void SpawnProp(PropData prop)
    {
        int i = Random.Range(0, prop.SpawnPoints.Length - 1);
        Instantiate(prop.Prefab, prop.SpawnPoints[i].position,
            Quaternion.identity, prop.SpawnPoints[i]);
    }
}
