using System.Collections;
using System.Collections.Generic;
using Meta;
using UnityEngine;
using UnityEngine.UI;

public class UI_HeroPanel : MonoBehaviour
{
    private Image icon;
    private Text name;
    private int id;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void SetHero(int id)
    {

        if (this.id == id)
            return;

        this.id = id;
        ItemData heroData = Services.Data.ItemInfo(id);
        //name.text = heroData.name.ToString();

        Services.Assets.SetSpriteIntoImage(icon, "Heroes/" + id + "/icon", true).Forget();
    }

    void Awake()
    {
        name = transform.Find("NameHero").GetComponent<Text>();
        icon = transform.Find("ImageHero").GetComponent<Image>();
    }
}