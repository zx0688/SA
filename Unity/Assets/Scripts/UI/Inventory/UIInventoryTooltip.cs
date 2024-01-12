using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UI.Components;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryTooltip : MonoBehaviour
{
    [SerializeField] private TooltipBack background;
    [SerializeField] private Image icon;
    [SerializeField] private UIConditions howTo;
    [SerializeField] private UIConditions whereTo;

    [SerializeField] private GameObject whereText;
    [SerializeField] private GameObject howText;
    [SerializeField] private Text description;

    protected ItemMeta meta;

    public void HideTooltip()
    {
        background.Hide();
        background.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }


    public void ShowTooltip(ItemMeta meta)
    {
        background.Show("red", meta.Name.Localize(LocalizePartEnum.CardName));
        background.gameObject.SetActive(true);
        gameObject.SetActive(true);

        this.meta = meta;
        howTo.SetItem(meta.HowTo != null ? meta.HowTo.ToList() : null);
        whereTo.SetItem(meta.WhereTo != null ? meta.WhereTo.ToList() : null);

        howText.SetActive(meta.HowTo != null && meta.HowTo.Length > 0);
        whereText.SetActive(meta.WhereTo != null && meta.WhereTo.Length > 0);

        description.text = meta.Desc.Localize(LocalizePartEnum.CardDescription);
        icon.LoadItemIcon(meta.Id);
    }

    public virtual void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HideTooltip();
        }
    }

}