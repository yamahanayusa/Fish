using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using static SellableItem;

public class mouth : MonoBehaviour
{

    private GameObject amuamu;
    private float amuamuCounter = 1;
    private Vector3 amuPos = Vector3.zero;

    void OnTriggerEnter(Collider collision)
    {
        if (amuamu) return;

        if (collision.gameObject.tag.Contains("Item_"))
        {
            SellableItem sellableItem = collision.gameObject.GetComponent<SellableItem>();

            float value = sellableItem.GetSellPrice();

            if(sellableItem.GetPriceType() == PriceType.enFish)
            {
                value = value * sellableItem.GetDoneness();
            }
            else
            {
                value /= 3;
            }

            Wallet wallet = GameObject.Find("Player").GetComponent<Wallet>();

            wallet.AddGold((int)value);
            SetAmuAmu(sellableItem.gameObject);
        }
        else
        {
            Debug.Log("‚±‚êˆá‚¤");
        }
    }


    private void Update()
    {
        if (amuamu)
        {
            amuamu.transform.position = Vector3.Lerp(transform.position, amuPos, amuamuCounter);
            amuamu.transform.localScale *= amuamuCounter;
            amuamuCounter -= Time.deltaTime;
            if(amuamuCounter <= 0)
            {
                Destroy(amuamu);
            }
        }
    }

    private void SetAmuAmu(GameObject amu)
    {
        amuamu = amu;
        amuPos = amu.transform.position;

        amuamuCounter = 1;
    }
}
