using System;
using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization;
using Cards;
using UnityEngine;
using UnityEngine.UI;

public class CARD_Base : MonoBehaviour
{
    protected SwipeData Data;

    public virtual void UpdateData(SwipeData data)
    {
        this.Data = data;
        UpdateHUD();
    }

    protected Image background;
    protected Image art;
    protected Text title;
    protected Text description;
    protected Text choice;
    protected Image hero;

    protected UI_EffectPanel effectPanel;

    protected virtual void Awake()
    {
        foreach (Transform g in transform.GetComponentsInChildren<Transform>())
        {
            switch (g.name)
            {

                case "UI_Effects":
                    effectPanel = g.GetComponent<UI_EffectPanel>();
                    break;
                case "Background":
                    background = g.GetComponent<Image>();
                    break;
                case "Art":
                    art = g.GetComponent<Image>();
                    break;
                case "Hero":
                    hero = g.GetComponent<Image>();
                    break;
                case "Title":
                    title = g.GetComponent<Text>();
                    break;
                case "Description":
                    description = g.GetComponent<Text>();
                    break;
                case "ChoiceText":
                    choice = g.GetComponent<Text>();
                    break;
                default:
                    break;
            }
        }

    }

    protected string Localize(string custom, string key)
    {
        return custom != null && custom.Length > 0 ? LocalizationManager.Localize(custom) : LocalizationManager.Localize(key);
    }

    protected async virtual void UpdateHUD()
    {
        return;
        description.gameObject.SetActive(false);
        art.gameObject.SetActive(false);

        title.text = Localize(Data.Data.Name, Data.Data.Id + "cardn");

        if (Data.Data.Act.Text != null)
        {
            //  description.text = Localize(data.cardData.act.text, data.cardData.id + "cardd");
            description.gameObject.SetActive(true);
        }

        //if (data.cardData.act.image != null)
        {
            // Services.Assets.SetSpriteIntoImage(art, "Images/" + data.cardData.act.image, true, null).Forget();
            art.gameObject.SetActive(true);
        }

        //effectPanel.UpdateItems(data.skills);
    }

    public virtual void OnTakeCard()
    {

    }
    public virtual void OnStartSwipe()
    {

    }
    public virtual void OnChangeDirection(int obj)
    {

    }
    public virtual void OnChangeDeviation(float obj)
    {

    }
    public virtual void OnDrop()
    {

    }

    public void SetActive(bool enable)
    {
        gameObject.SetActive(enable);
    }

}