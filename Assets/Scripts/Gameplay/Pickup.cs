using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public float pickupSpeedMultiplier;
    public Rigidbody rb;
    public string typeid;
    public float pickupDelay;

    [SerializeField] TextMeshProUGUI idText;

    [Header("Audio")]
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip[] grabSounds;
    [SerializeField] AudioClip[] dropSounds;
    [SerializeField] float soundThreshold;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        idText.text = typeid.ToString();
    }

    public void OnPickup()
    {
        idText.gameObject.SetActive(true);
        source.PlayOneShot(grabSounds[Random.Range(0, grabSounds.Length)]);
    }

    public void OnDrop()
    {
        idText.gameObject.SetActive(false);
        source.PlayOneShot(dropSounds[Random.Range(0, dropSounds.Length)]);
    }

    private void Update()
    {
        if (pickupDelay > 0) pickupDelay -= Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(rb.velocity.sqrMagnitude >= soundThreshold)
        {
            source.PlayOneShot(dropSounds[Random.Range(0, dropSounds.Length)]);
        }
        else
        {
            source.PlayOneShot(grabSounds[Random.Range(0, grabSounds.Length)]);
        }
    }
}
