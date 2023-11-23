using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization;
using Cysharp.Threading.Tasks;
using Meta;
using UnityEngine;
using UnityEngine.UI;

public class UI_InventoryTooltip : MonoBehaviour, ITick
{
    [SerializeField] private TooltipBack _background;

    protected Image icon;
    protected Text description;
    protected Text header;

    protected ItemMeta _meta;

    public void HideTooltip()
    {
        _background.Hide();
        _background.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    void Start()
    {

    }

    public void ShowTooltip(ItemMeta meta)
    {
        _background.Show("red", meta.Name);
        _background.gameObject.SetActive(true);

        gameObject.SetActive(true);
        this._meta = meta;
        //header.text = LocalizationManager.Localize(this.itemData.Nam);
        //description.text = LocalizationManager.Localize(this._meta.descr);

        Services.Assets.SetSpriteIntoImage(icon, AssetsService.ITEM_ADDRESS(meta.Id), true).Forget();
        // meta.Id, true).Forget();
        //LoadSprite ().Forget ();
    }

    protected virtual void Awake()
    {
        icon = transform.Find("Item").GetComponent<Image>();
        //header = transform.Find("Name").GetComponent<Text>();
        //description = transform.Find("Description").GetComponent<Text>();

        /// gameObject.GetComponent<Button> ().onClick.AddListener (HideTooltip);
    }

    public virtual void Update()
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