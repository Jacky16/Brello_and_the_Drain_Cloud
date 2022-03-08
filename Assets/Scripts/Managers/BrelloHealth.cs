using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrelloHealth : Health
{
    [SerializeField] Sprite[] healthImages;
    [SerializeField] Image currentImage;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void onDamage()
    {
        currentImage.sprite = healthImages[currLife];
    }

    protected override void onHeal()
    {
        currentImage.sprite = healthImages[currLife];
    }
}
