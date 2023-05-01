using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager main;
    [SerializeField] WaveInfo[] definedWaves;
    [SerializeField] GameObject[] pickupPrefabs;

    [Header("References")]
    [SerializeField] Truck truck;
    [SerializeField] PickupSpawner spawner;
    [SerializeField] TVscreen screen;
    int currentWave;
    public static float score;
    [SerializeField] float timeleft;

    bool alarmSoundHasStarted = false;
    [SerializeField] GameObject alarmSoundPrefab;

    private void Awake()
    {
        main = this;
        score = 0;
        NextWave();
    }

    private void Update()
    {
        timeleft -= Time.deltaTime;
        // start alarm sound
        if(timeleft <= 10 && !alarmSoundHasStarted)
        {
            alarmSoundHasStarted = true;
            Instantiate(alarmSoundPrefab);
        }
        else if (timeleft <= 0) SceneManager.LoadScene("GameOver");
    }

    public void NextWave()
    {
        WaveInfo info;
        if (currentWave < definedWaves.Length) info = definedWaves[currentWave];
        else info = GenerateRandomWave(Mathf.CeilToInt(currentWave*1.8f));

        truck.truckRequiredItems = info.GetDict();
        spawner.SpawnItems(info);
        screen.RefreshReqs(info);

        currentWave++;
    }

    WaveInfo GenerateRandomWave(int pickupAmount)
    {
        WaveInfo info = new WaveInfo();
        List<BoxInfo> boxInfos = new List<BoxInfo>();

        int toReq = pickupAmount;

        while (toReq > 0)
        {
            // random amount of items
            int rnd = Random.Range(1, Mathf.CeilToInt(toReq*0.75f));
            GameObject chosenPrefab = pickupPrefabs[Random.Range(0, pickupPrefabs.Length)];
            bool alreadyExists = false;

            for(int i=0; i < boxInfos.Count; i++)
            {
                if (boxInfos[i].prefab.name == chosenPrefab.name)
                {
                    alreadyExists = true;
                    int previousAmount = boxInfos[i].amount;
                    boxInfos[i] = new BoxInfo(chosenPrefab, rnd + previousAmount);
                }
            }

            if (!alreadyExists)
            {
                boxInfos.Add(new BoxInfo(chosenPrefab, rnd));
            }
            toReq -= rnd;
        }

        info.boxTypes = boxInfos.ToArray();
        return info;
    }

    public void ParseDeliveryResults(List<DeliveryResult> results)
    {
        short succ = 0, miss = 0, extr = 0, wrng = 0;
        for (int i = 0; i < results.Count; i++)
        {
            switch (results[i])
            {
                case DeliveryResult.Success: succ++; break;
                case DeliveryResult.MissingItem: miss++; break;
                case DeliveryResult.ExtraItem: extr++; break;
                case DeliveryResult.WrongItem: wrng++; break;
            }
        }

        ScoreInfo newScoreInfo = new ScoreInfo();
        newScoreInfo.successes = succ * 100;
        newScoreInfo.misses = miss * -125;
        newScoreInfo.extras = extr * -75;
        newScoreInfo.wrongs = wrng * -175;

        newScoreInfo.oldScore = score;
        score += newScoreInfo.successes + newScoreInfo.misses + newScoreInfo.extras + newScoreInfo.wrongs;
        newScoreInfo.newScore = score;

        screen.RefreshScore(newScoreInfo);
    }
}

public struct ScoreInfo
{
    public float oldScore, newScore;
    public float successes, misses, extras, wrongs;
}

[System.Serializable]
public struct WaveInfo
{
    public BoxInfo[] boxTypes;

    public Dictionary<string, int> GetDict()
    {
        Dictionary<string, int> dict = new Dictionary<string, int>();
        for(int i =0; i < boxTypes.Length; i++)
        {
            BoxInfo bi = boxTypes[i];
            if (dict.TryGetValue(bi.typeid, out int val))
            {
                dict[bi.typeid] += bi.amount;
            }
            else
            {
                dict.Add(bi.typeid, bi.amount);
            }
        }

        return dict;
    }
}

[System.Serializable]
public struct BoxInfo
{
    public GameObject prefab;
    public int amount;
    public string typeid => prefab.GetComponent<Pickup>().typeid;

    public BoxInfo(GameObject _prefab, int _amount)
    {
        prefab = _prefab;
        amount = _amount;
    }
}
