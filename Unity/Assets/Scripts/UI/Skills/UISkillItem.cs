using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;


public class UISkillItem : MonoBehaviour
{
    [SerializeField] private int slot;
    [SerializeField] private Image icon;
    //[SerializeField] private Button _showTooltipBtn;

    private UI_SkillTooltip _tooltip;

    public int Slot => slot;

    public void SetItem(SkillMeta skill)
    {
        if (skill == null)
        {
            Empty();
            return;
        }

        icon.gameObject.SetActive(true);
        icon.LoadSkillImage(skill.Image);
    }

    public void Empty()
    {
        icon.gameObject.SetActive(false);
        //if (_showTooltipBtn)
        //    _showTooltipBtn.interactable = false;
    }

    private void OnClick()
    {
        //_tooltip.ShowTooltip(meta, _vo);
    }

    public void SetTooltip(UI_SkillTooltip tooltip)
    {
        this._tooltip = tooltip;
        //_showTooltipBtn.onClick.AddListener(OnClick);
    }
}