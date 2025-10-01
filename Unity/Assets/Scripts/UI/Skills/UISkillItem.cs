using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;


public class UISkillItem : MonoBehaviour
{
    [SerializeField] private int slot;
    [SerializeField] private Image icon;
    [SerializeField] private GameObject[] stars;
    [SerializeField] private Button showTooltipBtn;

    private UI_SkillTooltip tooltip;

    public int Slot => slot;

    private SkillMeta meta;
    private SkillItem skill;

    public void UpdateItem(int index)
    {
        skill = Services.Player.GetSkillItemBySlot(index);

        if (skill == null)
        {
            Empty();
            return;
        }
        meta = Services.Meta.Game.Skills[skill.Id];

        stars.ToList().ForEach(s => s.SetActive(false));
        if (skill.Level > 2)
            throw new Exception("skill must be no more then 2");

        for (var l = 0; l <= skill.Level; l++)
            stars[l].SetActive(true);

        icon.gameObject.SetActive(true);
        icon.LoadSkillImage(skill.Level == 2 ? meta.Image : meta.Icon);
        showTooltipBtn.interactable = true;
    }

    public void Empty()
    {
        icon.gameObject.SetActive(false);
        stars.ToList().ForEach(s => s.SetActive(false));
        if (showTooltipBtn)
            showTooltipBtn.interactable = false;
    }

    private void OnClick()
    {
        this.tooltip.ShowTooltip(meta, skill);
    }

    public void SetTooltip(UI_SkillTooltip tooltip)
    {
        this.tooltip = tooltip;
        showTooltipBtn.onClick.AddListener(OnClick);
    }
}