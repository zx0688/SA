using System.Collections;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

public class UI_SkillTooltip : MonoBehaviour
{
    [SerializeField] private TooltipBack _background;
    [SerializeField] private Text _description;
    [SerializeField] private Image _icon;
    [SerializeField] private Text _type;

    private UI_SKillBilder _skillBilder;
    //private SkillMeta _meta;
    private ItemData data;

    void Awake()
    {
        _skillBilder = GetComponent<UI_SKillBilder>();
    }

    public void HideTooltip()
    {
        _background.Hide();
        _background.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void ShowTooltip(SkillMeta meta, ItemData data)
    {
        /*
        _background.Show("pink", meta.Name);
        _background.gameObject.SetActive(true);

        switch (meta.Type)
        {
            case 1:
                _type.text = "Предмет";
                break;
            case 2:
                _type.text = "Репутация";
                break;
            case 3:
                _type.text = "Навык";
                break;
            case 4:
                _type.text = "Помощник";
                break;
        }

        gameObject.SetActive(true);
        _meta = meta;
        _vo = vo;

        _skillBilder.Build(meta, vo);
        //_description.text = "";LocalizationManager.Localize(this._meta.Des);
*/
        //Services.Assets.SetSpriteIntoImage(_icon, "Skills/" + meta. + "/icon", true).Forget();
        //   UpdateTime();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HideTooltip();
        }
    }

}