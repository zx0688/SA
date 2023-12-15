using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

public class UI_RecipesTooltip : MonoBehaviour
{
    [SerializeField] private TooltipBack _background;
    [SerializeField] private Image _icon;
    [SerializeField] private UITarget _target;
    [SerializeField] private UIReward _reward;
    [SerializeField] private Text _timer;
    [SerializeField] private GameObject _timerBox;
    [SerializeField] private Text _description;

    protected Text description;
    protected Text header;

    private CardMeta _meta;
    private CardData data;

    public void HideTooltip()
    {
        _background.Hide();
        _background.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }


    public void ShowTooltip(CardMeta meta, CardData vo)
    {
        _background.Show("yellow", meta.Name);
        _background.gameObject.SetActive(true);
        gameObject.SetActive(true);

        //_timer.text = TimeFormat.ONE_CELL_FULLNAME(i);

        if (this._meta != null && this._meta.Id == meta.Id)
            return;

        this._meta = meta;
        this.data = vo;

        //_target.SetItems(meta.Act.Con);
        //_reward.SetItems(meta.Act.Reward);

        //_description.text = _meta.Des;
        //header.text = LocalizationManager.Localize(this.itemData.Nam);
        //description.text = LocalizationManager.Localize(this._meta.descr);

        Services.Assets.SetSpriteIntoImage(_icon, "Items/" + meta.Id + "/icon", true).Forget();
        //LoadSprite ().Forget ();


        _timerBox.SetActive(false);

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HideTooltip();
        }
    }

}