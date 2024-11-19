using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UI;
using UnityEngine;
using UnityEngine.UI;


public class UIInventoryItem : MonoBehaviour, ITick, ISetData<string>
{
    [SerializeField] protected Text count;
    [SerializeField] protected Text newText;
    [SerializeField] protected GameObject newItem;

    [SerializeField] private Button showTooltipBtn;
    [SerializeField] protected Image icon;

    private ItemMeta data;
    private ItemData item;
    protected bool isEmpty;
    protected UIInventoryTooltip tooltip;

    public string Data => data.Id;

    private void OnIconLoaded()
    {
        string prefName = ZString.Format("I_{0}", data.Id);
        if (!PlayerPrefs.HasKey(prefName))
        {
            count.gameObject.SetActive(false);
            PlayerPrefs.SetString(prefName, "");
            icon.rectTransform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            icon.rectTransform.DOScale(1.1f, 1f).SetEase(Ease.OutElastic).OnComplete(() =>
            {
                icon.rectTransform.DOScale(1f, 0.1f).OnComplete(() => count.gameObject.SetActive(true));
            });
            //newText.text = data.Name;
            newItem.SetActive(true);
            //newText.color = Color.yellow;
            newItem.GetComponent<CanvasGroup>().DOFade(0f, 0.5f).SetDelay(1.4f).SetEase(Ease.OutCirc).OnComplete(() => newText.gameObject.SetActive(false));
        }
    }

    public bool IsEmpty()
    {
        return isEmpty;
    }

    public int GetId()
    {
        return 0;//data != null ? data. : 0;
    }

    public virtual void Clear()
    {
        isEmpty = true;
        icon.enabled = false;
        icon.sprite = null;
        count.gameObject.SetActive(false);
        newItem.SetActive(false);

        count.enabled = false;

        if (showTooltipBtn)
            showTooltipBtn.interactable = false;
    }

    void Start()
    {

    }

    void Awake()
    {
        isEmpty = true;
        Clear();
    }

    protected void UpdateView(int timestamp)
    {

    }

    protected void OnClick()
    {
        if (data != null)
            tooltip.ShowTooltip(data);
    }

    public void Tick(int timestamp)
    {

    }

    public bool IsTickble()
    {
        return false;
    }

    public void SetTooltip(UIInventoryTooltip tooltip)
    {
        this.tooltip = tooltip;
        showTooltipBtn.onClick.AddListener(OnClick);
    }

    public void Hide()
    {
        Clear();
    }

    public void SetItem(string data)
    {
        item = Services.Player.GetItemData(data);
        if (item == null)
        {
            Clear();
            return;
        }

        count.text = item.Count.ToString();
        isEmpty = false;

        if (this.data != null && this.data.Id == item.Id)
            return;

        if (!Services.Meta.Game.Items.TryGetValue(item.Id, out this.data))
            throw new Exception("unexpected id in inventory item");


        icon.enabled = true;
        count.enabled = true;
        count.gameObject.SetActive(true);

        newItem.SetActive(false);
        showTooltipBtn.interactable = true;

        icon.LoadItemIcon(item.Id, OnIconLoaded);
    }
}