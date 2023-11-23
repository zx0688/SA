using System.Net.Http.Headers;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UI_Recipes : MonoBehaviour, IPage
{
    [SerializeField] private UI_RecipesTooltip _tooltip;
    [SerializeField] private UI_RecipeItem[] _items;
    [SerializeField] private Text _empty;

    public UI_TickChildren tick;
    private PageSwiper _swiper;

    void Awake()
    {
        _items = GetComponentsInChildren<UI_RecipeItem>();

        foreach (UI_RecipeItem item in _items)
        {
            item.SetTooltip(_tooltip);
        }

        _swiper = GetComponentInChildren<PageSwiper>();
        gameObject.SetActive(false);
        _tooltip.HideTooltip();
    }

    void UpdateList()
    {
        if (!Services.isInited)
            return;

        /*List<ItemVO> items = new List<ItemVO>();

        QuestVO q = new QuestVO(2, 2);
        q.Activated = 1652134383;
        q.Choice = 1;
        q.Executed = 0;
        q.Id = 2;

        for (int i = 0; i < 1; i++)
            items.Add(q);


        _empty.gameObject.SetActive(items.Count == 0);
        _swiper.gameObject.SetActive(items.Count != 0);
        _swiper.UpdateData(items);
*/
        /*
        List<QuestVO> quests = Services.Player.playerVO.quests;
        int time = GameTime.Current;

        foreach (QuestVO q in quests)
        {

            UI_QuestsItem slot = Array.Find(items, i => i.GetId() == q.id);
            // QuestData questData = Services.Data.QuestInfo (q.id);

            if (q.state == QuestVO.STATE_ACTIVE)
            {
                if (slot != null)
                    continue;
                slot = Array.Find(items, i => i.IsEmpty());
                if (slot == null)
                    continue;
                slot.SetItem(q);
            }
            else if (slot != null)
            {
                slot?.Clear();
            }
        }


        tick.UpdateTickList();
        */
    }

    public void Show()
    {
        _tooltip?.HideTooltip();
        UpdateList();
    }

    public string GetName() => "Рецепты";
    public GameObject GetGameObject() => gameObject;

    public void Hide()
    {

    }
}