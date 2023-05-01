using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    [SerializeField] int junkMax;

    public void SpawnItems(WaveInfo info) => StartCoroutine(SpawnItemCoroutine(info));

    IEnumerator SpawnItemCoroutine(WaveInfo info)
    {
        for(int i =0; i < info.boxTypes.Length; i++)
        {
            BoxInfo boxInfo = info.boxTypes[i];
            int amt = boxInfo.amount + Random.Range(1, junkMax);
            for(int b = 0; b < amt; b++)
            {
                Instantiate(boxInfo.prefab, 
                    transform.position + new Vector3(Random.Range(-1,1),0,Random.Range(-1,1)),
                    Quaternion.Euler(Random.Range(-90,90), Random.Range(-90, 90), Random.Range(-90, 90)));
                yield return new WaitForSeconds(0.18f);
            }
        }

        yield return new WaitForSeconds(0.2f);
    }
}
