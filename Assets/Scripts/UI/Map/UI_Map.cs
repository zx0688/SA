using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Map : ServiceBehaviour
{

    [SerializeField]
    public Sprite SpriteCurrent;
    [SerializeField]
    public Sprite SpriteLock;
    [SerializeField]
    public Sprite SpriteOpened;
    [SerializeField]
    public Sprite SpriteAvailable;
    [SerializeField]
    public UI_MapTooltip tooltip;

    private Button[] buttons;

    protected override void Awake()
    {
        buttons = GetComponentsInChildren<Button>();
        base.Awake();
    }

    protected override void OnServicesInited()
    {
        base.OnServicesInited();

        tooltip.HideTooltip();

        Services.Player.OnProfileUpdated += UpdateIcons;

        return;
        foreach (Button b in buttons)
        {
            int id = int.Parse(b.name.Split('_')[1]);
            Image i = b.gameObject.GetComponent<Image>();
            LocationData location = Services.Data.Game.Locations.Find(l => l.id == id);

            if (location == null)
                throw new Exception("Нет локации к кнопке N" + id);

            Services.Assets.SetSpriteIntoImage(i, "Locations/" + id + "/icon", true);

            b.onClick.AddListener(() => OnClick(location));
        }
    }

    void Start()
    {

    }

    void OnEnable()
    {

        if (!Services.isInited)
            return;

        tooltip.HideTooltip();
        UpdateIcons();

    }

    private void UpdateIcons()
    {
        int time = GameTime.Current;
        return;
        foreach (Button b in buttons)
        {
            int id = int.Parse(b.name.Split('_')[1]);

            LocationData location = Services.Data.Game.Locations.Find(l => l.id == id);

            if (Services.Player.playerVO.locationId == location.id)
            {
                SetState(b.image, 0);
            }
            else if (Services.Player.playerVO.locations.IndexOf(id) != -1)
            {
                //opened
                SetState(b.image, 2);
            }
            else if (Services.Data.CheckConditions(location.condi, time))
            {
                //available
                SetState(b.image, 1);
            }
            else
            {
                //lock
                SetState(b.image, 1);
            }
        }
    }

    private void OnClick(LocationData location)
    {

        int time = GameTime.Current;
        Debug.Log("Click");
        tooltip.ShowTooltip(location);
        /*if (Services.Player.playerVO.locationId == location.id) {
            tooltip.ShowTooltip (location);
            //nothing
        } else if (Services.Player.playerVO.locations.IndexOf (location.id) != -1) {
            Services.Player.ChangeLocation (location);
        } else if (Services.Data.CheckConditions (null, location.condi, time)) {
            //show popup
            tooltip.ShowTooltip (location);
        } else {
            //show popup
            tooltip.ShowTooltip (location);
        }*/
    }

    private void SetState(Image image, int state)
    {
        switch (state)
        {
            case 0:
                image.sprite = SpriteCurrent;
                break;
            case 1:
                image.sprite = SpriteLock;
                break;
            case 2:
                image.sprite = SpriteOpened;
                break;
            case 3:
                image.sprite = SpriteAvailable;
                break;
            default:
                throw new System.Exception("state undefined");
        }
    }

    // Update is called once per fram
}