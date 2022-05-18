using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;
using UnityEngine.UI;


public class UI_MapTooltip : ServiceBehaviour, ITick
{

    private Image image;
    private Text description;
    private Text _name;
    private Text buttonText;
    private GameObject youAreHere;

    private Button changeLocation;
    private LocationMeta _location;

    private UI_Reward cost;

    protected override void OnServicesInited()
    {
        base.OnServicesInited();

        Services.Player.OnOpenedLocation += ShowTooltip;
    }
    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    public void ShowTooltip(LocationMeta location)
    {

        int time = GameTime.Current;
        gameObject.SetActive(true);
        _location = location;

        //if (location.condi != null && location.condi.Count > 0)
        //    cost.SetAsConditions (location.condi);
        // else
        //    cost.SetAsItems (location.reward);

        youAreHere.SetActive(isCurrentLocation());
        changeLocation.gameObject.SetActive(!isCurrentLocation());
        buttonText.text = isOpenedLocation() ? LocalizationManager.Localize("Location.Move") : LocalizationManager.Localize("Location.Open");

        changeLocation.interactable = isAvailable();

        _name.text = location.name;
        description.text = location.descr;
        Services.Assets.SetSpriteIntoImage(image, "Locations/" + location.id + "/icon", true);
        //LoadSprite ().Forget ();*/
    }

    public bool isAvailable()
    {
        return Services.Player.IsRewardApplicable(_location.reward) && Services.Data.CheckConditions(_location.condi, GameTime.Current);
    }
    public bool isOpenedLocation()
    {
        return Services.Player.GetVO.locations.Exists(l => l == _location.id);
    }
    public bool isCurrentLocation()
    {
        return Services.Player.GetVO.locationId == _location.id;
    }

    protected override void Awake()
    {
        base.Awake();

        image = transform.Find("Image").GetComponent<Image>();
        changeLocation = transform.Find("Button").GetComponent<Button>();
        buttonText = changeLocation.GetComponentInChildren<Text>();
        _name = transform.Find("Name").GetComponent<Text>();
        description = transform.Find("Description").GetComponent<Text>();
        cost = transform.Find("Cost").GetComponent<UI_Reward>();
        youAreHere = transform.Find("YouAreHere").gameObject;

        changeLocation.onClick.AddListener(() => OnChangeLocation());
        gameObject.GetComponent<Button>().onClick.AddListener(() => HideTooltip());

    }

    void OnChangeLocation()
    {

        if (isOpenedLocation())
        {
            Services.Player.ChangeLocation(_location);
            HideTooltip();
        }
        else
            Services.Player.OpenLocation(_location);
    }

    public bool IsTickble()
    {
        return false;
    }

    public void Tick(int timestamp)
    {

    }

}