using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmSound : MonoBehaviour
{
    public static AlarmSound main;
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip stopSound;
    [SerializeField] float rampUp;
    bool isStopped;

    private void Awake()
    {
        if (main != null) Destroy(main.gameObject);
        DontDestroyOnLoad(gameObject);
        main = this;
    }

    private void Update()
    {
        source.volume += Time.deltaTime * rampUp;
    }

    public void Stop()
    {
        source.Stop();
        source.PlayOneShot(stopSound);
        isStopped = true;
    }
}
