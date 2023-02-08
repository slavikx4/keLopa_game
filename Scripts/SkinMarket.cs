using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinMarket : MonoBehaviour
{
    public GameObject GoldBuy;
    public GameObject PinkBuy;

    public void Start()
    {
        CheckBuyPink();
        CheckBuyGold();
    }

    public void BuyGold(int cost)
    {
        if (PlayerPrefs.GetInt("coins") >= cost)
        {
            PlayerPrefs.SetInt("Gold", 1);
            PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") - cost);
            CheckBuyGold();
        }
    }
    public void BuyPink(int cost)
    {
        if (PlayerPrefs.GetInt("coins") >= cost)
        {
            PlayerPrefs.SetInt("Pink", 1);
            PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") - cost);
            CheckBuyPink();
        }
    }
    public void CheckBuyGold()
    {
        if (PlayerPrefs.GetInt("Gold")== 1)
        {
            GoldBuy.SetActive(false);        
        }
    }
    public void CheckBuyPink()
    {
        if (PlayerPrefs.GetInt("Pink") == 1)
        {
            PinkBuy.SetActive(false);
        }
    }
}
