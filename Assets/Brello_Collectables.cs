using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Brello_Collectables : MonoBehaviour
{
    private int coins;
    private int droply;

    [SerializeField] TextMeshProUGUI droplyText;
    [SerializeField] TextMeshProUGUI coinText;

    // Start is called before the first frame update
    void Start()
    {
        coins = droply = 0;
    }


    public void AddCoin()
    {
        coins++;
        UpdateHUD();
    }

    public void AddDroply()
    {
        droply++;
        UpdateHUD();
    }

    private void UpdateHUD()
    {
        coinText.text = coins.ToString();
        droplyText.text = droply.ToString();
    }
}
