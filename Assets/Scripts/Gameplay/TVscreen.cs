using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

public class TVscreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI reqs;
    [SerializeField] TextMeshProUGUI res_score;
    [SerializeField] TextMeshProUGUI res_success, res_missing, res_extra, res_wrong;

    public void RefreshScore(ScoreInfo info)
    {
        res_score.text = info.newScore.ToString();
        res_success.text = info.successes.ToString();
        res_missing.text = info.misses.ToString();
        res_extra.text = info.extras.ToString();
        res_wrong.text = info.wrongs.ToString();

    }

    public void RefreshReqs(WaveInfo info)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < info.boxTypes.Length; i++)
        {
            BoxInfo bi = info.boxTypes[i];
            sb.Append($"{bi.typeid} (x{bi.amount})\n");

        }
        reqs.text = sb.ToString();
    }
}
