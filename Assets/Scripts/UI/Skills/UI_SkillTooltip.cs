using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization;
using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

public class UI_SkillTooltip : MonoBehaviour, ITick
{

    protected Image image;
    protected Text description;
    protected Text header;

    protected ItemData itemData;

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    protected virtual void Start()
    {

    }

    public virtual void ShowTooltip(ItemData itemData)
    {

        gameObject.SetActive(true);
        this.itemData = itemData;
        header.text = LocalizationManager.Localize(this.itemData.Name);
        description.text = LocalizationManager.Localize(this.itemData.Des);

        //Services.Assets.SetSpriteIntoImage(I)
        //LoadSprite ().Forget ();
    }

    protected virtual void Awake()
    {
        //image = transform.Find ("Image").GetComponent<Image> ();
        header = transform.Find("Name").GetComponent<Text>();
        description = transform.Find("Description").GetComponent<Text>();
        /// gameObject.GetComponent<Button> ().onClick.AddListener (HideTooltip);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HideTooltip();
        }
    }

    public virtual void Tick(int timestamp)
    {

    }

    public virtual bool IsTickble()
    {
        return false;
    }
}