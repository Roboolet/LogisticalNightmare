using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScene : MonoBehaviour
{
    [SerializeField] GameObject text, button;
    [SerializeField] TextMeshProUGUI score;
    void Start()
    {
        score.text = GameManager.score.ToString();
        StartCoroutine("Animation");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    IEnumerator Animation()
    {
        yield return new WaitForSeconds(2f);
        text.SetActive(true);
        yield return new WaitForSeconds(2f);
        score.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        button.SetActive(true);
        AlarmSound.main?.Stop();
    }

    public void RetryButton() => SceneManager.LoadScene("GameplayScene");
}
