using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Meta;
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

    //private SkillMeta meta;
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

        //if (_meta != null && _meta.Act != null && _meta.Act.Time > 0)
        {
            _timeValue.text = "3434";//Services.Player.SwipeCountLeft(_vo.Activated, _meta.Act.Time).ToString();
        }

        /*   if (meta != null && meta.Id == item.Id) return;

           meta = new SkillMeta();//Services.Data.GetSkillMeta(item.Id);
           meta.Id = 1;
           meta.Name = "Темная сила";
           meta.Type = 3;
           meta.Act = new ActionMeta();
           //_meta.Act.Time = 20;
           //_meta.Act.Chance = 2;
           meta.Act.Reward = new List<RewardMeta>();
           meta.Act.Reward.Add(new RewardMeta());
           meta.Act.Reward[0].Id = 4;
           meta.Act.Reward[0].Tp = GameMeta.ITEM;
           meta.Act.Reward[0].Count = 23;
           meta.Act.Con = new List<ConditionMeta>();
           meta.Act.Tri = new List<TriggerMeta>();
           meta.One = true;
           //_meta.Time = 3;


           //if (_meta.Act != null && _meta.Act.Time > 0)
           {
               _timeValue.text = "4545";// Services.Player.SwipeCountLeft(_vo.Activated, _meta.Act.Time).ToString();
           }
           //_time.SetActive(_meta.Act != null && _meta.Act.Time > 0);


           /*for (int i = 0; i < stars.Length; i++)
           {
               stars[i].gameObject.SetActive(i < item.Count && item.Count > 1);
           }

           _isEmpty = false;
           _icon.gameObject.SetActive(true);

           _showTooltipBtn.interactable = true;

           Services.Assets.SetSpriteIntoImage(_icon, "Skills/" + item.Id + "/icon", true).Forget();
           */
    }

    public bool IsEmpty()
    {
        return _isEmpty;
    }

    public void Clear()
    {
        //meta = null;
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
        //_tooltip.ShowTooltip(meta, _vo);
    }

    public void SetTooltip(UI_SkillTooltip tooltip)
    {
        this._tooltip = tooltip;
        _showTooltipBtn.onClick.AddListener(OnClick);
    }
}