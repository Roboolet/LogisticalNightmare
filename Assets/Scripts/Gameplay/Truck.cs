using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Truck : MonoBehaviour
{
    public Dictionary<string, int> truckRequiredItems = new Dictionary<string, int>();
    [SerializeField] Vector3 boxCenter, boxBounds;
    [SerializeField] LayerMask layerMask;
    [SerializeField] Animator anim;

    public UnityEvent OnDeliver, OnArrival;

    public Pickup[] GetContainedPickups()
    {
        Pickup[] output = new Pickup[0];
        Collider[] colls = Physics.OverlapBox(boxCenter + transform.position, boxBounds*0.5f, Quaternion.identity, layerMask);
        if(colls.Length > 0)
        {
            output = colls.Select(f => f.GetComponent<Pickup>()).ToArray();
        }

        return output;
    }

    public void StartDelivery() => StartCoroutine("Delivery");
    IEnumerator Delivery()
    {
        Pickup[] pups = GetContainedPickups();
        anim.SetTrigger("depart");

        // parent pickups to truck so they dont phase out
        foreach(Pickup p in pups)
        {
            p.transform.SetParent(transform);
        }

        yield return new WaitForSeconds(4);
        anim.SetTrigger("arrive");

        // score
        GameManager.main.ParseDeliveryResults(DeliverItems(pups));
        OnDeliver?.Invoke();

        // destroy pickups
        for (int i = pups.Length - 1; i >= 0; i--)
        {
            Destroy(pups[i].gameObject);
        }

        yield return new WaitForSeconds(4.6f);
        GameManager.main.NextWave();
        OnArrival?.Invoke();
    }

    List<DeliveryResult> DeliverItems(Pickup[] pups)
    {
        List<DeliveryResult> results = new List<DeliveryResult>();
        for (int i = 0; i < pups.Length; i++)
        {
            Pickup p = pups[i];
            if (truckRequiredItems.ContainsKey(p.typeid))
            {
                if (truckRequiredItems[p.typeid] > 0)
                {
                    results.Add(DeliveryResult.Success);
                    truckRequiredItems[p.typeid]--;
                }
                else results.Add(DeliveryResult.ExtraItem);
            }
            else
            {
                results.Add(DeliveryResult.WrongItem);
            }
        }

        // get missing item count
        foreach(KeyValuePair<string, int> pair in truckRequiredItems)
        {
            for(int i = 0; i < pair.Value; i++)
            {
                results.Add(DeliveryResult.MissingItem);
            }
        }
        return results;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(boxCenter+transform.position, boxBounds);
    }

}
public enum DeliveryResult { Unknown, Success, MissingItem, ExtraItem, WrongItem }
