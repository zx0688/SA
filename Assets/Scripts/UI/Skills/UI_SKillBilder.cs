using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.UI;

public class UI_SKillBilder : MonoBehaviour
{
    [Header("REWARD")]
    [SerializeField] private GameObject _reward;
    [SerializeField] private Image _rewardIcon;
    [SerializeField] private Text _rewardText;
    [SerializeField] private Text _rewardValue;
    [Header("TIME")]
    [SerializeField] private GameObject _time;
    [SerializeField] private Text _timeText;
    [SerializeField] private Text _timeValue;
    [Header("TRIGGER")]
    [SerializeField] private GameObject _trigger;
    [SerializeField] private Text _triggerText;
    [SerializeField] private Text _triggerName;
    [SerializeField] private Image _triggerIcon;
    [Header("CONDITION")]
    [SerializeField] private GameObject _condition;
    [SerializeField] private Text _conditionText;
    [SerializeField] private Text _conditionSign;
    [SerializeField] private Text _conditionValue;
    [SerializeField] private Image _conditionIcon;
    [Header("CHANCE")]
    [SerializeField] private GameObject _chance;
    [SerializeField] private Text _chanceValue;
    [Header("ONCE")]
    [SerializeField] private GameObject _once;
    [Header("TIME_LEFT")]
    [SerializeField] private GameObject _timeLeft;
    [SerializeField] private Text _timeLeftValue;


    public void Build(SkillMeta skill, SkillVO vo)
    {
        _reward.SetActive(false);
        if (skill.Act != null && skill.Act.Reward.Count > 0)
        {
            _reward.SetActive(true);
            _rewardText.text = "Получите";
            _rewardValue.text = skill.Act.Reward[0].Count.ToString();
            Services.Assets.SetSpriteIntoImageData(_rewardIcon, MetaData.ITEM, skill.Act.Reward[0].Id, true).Forget();
        }

        _time.SetActive(false);
        if (skill.Time > 0)
        {
            _time.SetActive(true);
            _timeText.text = "Уменьшено время на";
            _timeValue.text = skill.Time.ToString();
        }

        _trigger.SetActive(false);
        _triggerName.gameObject.SetActive(false);
        if (skill.Act != null && skill.Act.Tri.Count > 0)
        {
            TriggerData trigger = skill.Act.Tri[0];
            switch (trigger.Tp)
            {
                case TriggerData.ITEM:
                    _trigger.SetActive(true);
                    _triggerText.text = trigger.Choice == 0 ? "За" : "За каждый";
                    Services.Assets.SetSpriteIntoImageData(_triggerIcon, MetaData.ITEM, trigger.Id, true).Forget();
                    break;
                case TriggerData.CARD:
                    _trigger.SetActive(true);
                    _triggerName.gameObject.SetActive(true);
                    _triggerText.text = "С карты";
                    _triggerName.text = "sdfsdfsdf";
                    Services.Assets.SetSpriteIntoImageData(_triggerIcon, MetaData.ITEM, 1, true).Forget();
                    break;
            }
        }

        _condition.SetActive(false);
        _conditionSign.gameObject.SetActive(false);
        _conditionValue.gameObject.SetActive(false);
        if (skill.Act != null && skill.Act.Con.Count > 0)
        {
            ConditionData c = skill.Act.Con[0];
            _condition.SetActive(true);
            _conditionText.text = c.Count == 1 ? "Если есть" : "Если";
            _conditionSign.text = c.Sign == ">" ? "больше" : "меньше";
            if (c.Sign != "==")
                _conditionSign.gameObject.SetActive(true);
            if (c.Count > 1)
                _conditionValue.gameObject.SetActive(true);

            Services.Assets.SetSpriteIntoImageData(_conditionIcon, MetaData.ITEM, c.Id, true).Forget();
        }

        _chance.SetActive(false);
        if (skill.Act.Chance > 0)
        {
            _chance.SetActive(true);
            _chanceValue.text = $"{skill.Act.Chance}%";
        }

        _once.SetActive(skill.One);

        _timeLeft.SetActive(false);
        if (skill.Act != null && skill.Act.Time > 0)
        {
            _timeLeft.SetActive(true);
            _timeValue.text = Services.Player.SwipeCountLeft(vo.Activated, skill.Act.Time).ToString();
        }
    }
}
