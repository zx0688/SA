using System.Collections;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

public class UI_SkillTooltip : MonoBehaviour
{
    [SerializeField] private TooltipBack background;
    [SerializeField] private Text description;
    [SerializeField] private Image icon;
    [SerializeField] private Text type;
    [SerializeField] private Text effect;

    private SkillEffectBuilder _effectBuilder;
    private SkillEffectBuilder effectBuilder
    {
        get
        {
            if (_effectBuilder == null)
                _effectBuilder = new SkillEffectBuilder();
            return _effectBuilder;
        }
    }
    //private  _skillBilder;

    private ItemData data;

    public void HideTooltip()
    {
        background.Hide();
        background.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void ShowTooltip(SkillMeta meta, SkillItem data)
    {
        background.Show("pink", meta.Name);
        background.gameObject.SetActive(true);
        type.Localize($"SkillType{meta.Slot}.UI", LocalizePartEnum.GUI);
        icon.LoadSkillImage(data.Level == 2 ? meta.Image : meta.Icon);

        description.Localize(meta.Descs[data.Level == 2 ? 1 : 0], LocalizePartEnum.CardDescription);
        effect.text = effectBuilder.Apply(meta, data);
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HideTooltip();
        }
    }

}