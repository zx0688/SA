using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;
using UnityEngine.UI;


public class UI_SkillItem : MonoBehaviour
{
    [SerializeField] private int _type;
    [SerializeField] private Image _icon;
    [SerializeField] private Button _showTooltipBtn;
    [SerializeField] private Text _timeValue;
    [SerializeField] private GameObject _time;

    private UI_SkillTooltip _tooltip;

    private SkillMeta _meta;
    private SkillVO _vo;
    private bool _isEmpty;
    //protected Image[] stars;

    public void SetItem(SkillVO item)
    {
        if (item == null)
        {
            Clear();
            return;
        }

        _vo = item;

        if (_meta != null && _meta.Act != null && _meta.Act.Time > 0)
        {
            _timeValue.text = Services.Player.SwipeCountLeft(_vo.Activated, _meta.Act.Time).ToString();
        }

        if (_meta != null && _meta.Id == item.Id) return;

        _meta = new SkillMeta();//Services.Data.GetSkillMeta(item.Id);
        _meta.Id = 1;
        _meta.Name = "sdfsdfsdf";
        _meta.Act = new ActionMeta();
        //_meta.Act.Time = 20;
        //_meta.Act.Chance = 2;
        _meta.Act.Reward = new List<RewardData>();
        _meta.Act.Reward.Add(new RewardData());
        _meta.Act.Reward[0].Id = 4;
        _meta.Act.Reward[0].Tp = MetaData.ITEM;
        _meta.Act.Reward[0].Count = 23;
        _meta.Act.Con = new List<ConditionData>();
        _meta.Act.Tri = new List<TriggerData>();
        _meta.One = true;
        //_meta.Time = 3;


        if (_meta.Act != null && _meta.Act.Time > 0)
        {
            _timeValue.text = Services.Player.SwipeCountLeft(_vo.Activated, _meta.Act.Time).ToString();
        }
        _time.SetActive(_meta.Act != null && _meta.Act.Time > 0);


        /*for (int i = 0; i < stars.Length; i++)
        {
            stars[i].gameObject.SetActive(i < item.Count && item.Count > 1);
        }*/

        _isEmpty = false;
        _icon.gameObject.SetActive(true);

        _showTooltipBtn.interactable = true;

        Services.Assets.SetSpriteIntoImage(_icon, "Skills/" + item.Id + "/icon", true).Forget();
    }

    public bool IsEmpty()
    {
        return _isEmpty;
    }

    public void Clear()
    {
        _meta = null;
        _isEmpty = true;

        if (_showTooltipBtn)
            _showTooltipBtn.interactable = false;
    }

    void Awake()
    {
        Clear();
    }

    private void OnClick()
    {
        _tooltip.ShowTooltip(_meta, _vo);
    }

    public void SetTooltip(UI_SkillTooltip tooltip)
    {
        this._tooltip = tooltip;
        _showTooltipBtn.onClick.AddListener(OnClick);
    }
}