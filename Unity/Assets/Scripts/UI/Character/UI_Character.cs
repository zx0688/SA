using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UI_Character : MonoBehaviour, IPage
{
    //public UI_TickChildren tick;
    [SerializeField] private UI_SkillItem[] _items;
    [SerializeField] private PagePanel _pagePanel;
    [SerializeField] private UI_SkillTooltip _tooltip;
    [SerializeField] private Text _swipeCount;

    void Awake()
    {
        foreach (UI_SkillItem item in _items)
        {
            item.SetTooltip(_tooltip);
        }

        gameObject.SetActive(false);
        _tooltip.HideTooltip();
    }

    public void UpdateList()
    {

        if (!Services.isInited)
            return;

        //List<SkillVO> skills = Services.Player.playerVO.skills;
        //int time = GameTime.Get ();
        _swipeCount.text = Services.Player.GetPlayerVO.SwipeCount.ToString();

        for (int i = 0; i < _items.Length; i++)
        {
            UI_SkillItem item = _items[i];
            SkillVO skillVO = new SkillVO(1, 2);//Services.Player.skillHandler.GetVO(skillItem.id, skillItem.type);

            item.SetItem(skillVO);
        }

        //tick.UpdateTickList ();

    }

    public void Show()
    {
        _tooltip?.HideTooltip();
        UpdateList();
        _pagePanel.SetActivePageCounter(false);
        _pagePanel.HideArrow();
    }

    public string GetName() => "Персонаж";
    public GameObject GetGameObject() => gameObject;

    public void Hide()
    {

    }
}