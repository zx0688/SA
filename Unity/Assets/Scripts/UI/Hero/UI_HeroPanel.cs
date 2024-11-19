using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_HeroPanel : MonoBehaviour, ISetData<string>
{
    [SerializeField] private Image icon;
    [SerializeField] private Text description;
    [SerializeField] private Text name;
    [SerializeField] private UI_BuffItem buffPanel;

    private string data;

    public string Data => data;

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetItem(string data)
    {
        this.data = data;

        var hero = Services.Meta.Game.Heroes[data];

        Services.Assets.SetSpriteIntoImage(icon, "Heroes/" + data, true).Forget();
        description.text = hero.Desc.Localize(LocalizePartEnum.CardDescription);
        name.text = hero.Name.Localize(LocalizePartEnum.CardName);
        icon.LoadHeroImage(hero.Id);

        if (hero.Cards.HasTriggers())
            buffPanel.SetItem(Services.Meta.GetCard(hero.Cards[0]));
        else
            buffPanel.Hide();
    }
}