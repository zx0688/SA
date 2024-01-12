using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace UI.Map
{
    public class UIMap : ServiceBehaviour, IPage
    {
        [SerializeField] private UIMapTooltip tooltip;
        [SerializeField] protected UILocationButton[] items;
        [SerializeField] protected GameObject background;

        protected override void Awake()
        {
            base.Awake();

            foreach (UILocationButton item in items)
            {
                item.SetTooltip(tooltip);
            }

            gameObject.SetActive(false);
            tooltip.HideTooltip();
        }

        protected override void OnServicesInited()
        {
            base.OnServicesInited();

            Services.Player.OnChangedLocation += cm => UpdateList();
        }

        public void UpdateList()
        {
            if (!Services.isInited)
                return;

            for (int i = 0; i < items.Length; i++)
            {
                CardMeta locationMeta = null;//Services.Meta.Game.Locations[items[i].Id];
                if (locationMeta == null)
                {
                    items[i].gameObject.SetActive(false);
                    continue;
                }
                if (Services.Player.Profile.CurrentLocation == locationMeta.Id)
                {
                    items[i].UpdateStatus(LocationStatus.Active, locationMeta);
                }
                // else if (Services.Meta.CheckConditions(
                //     locationMeta.Act.Con,
                //     locationMeta,
                //     null,
                //     Services.Player,
                //     DataService.EMPTY_REWARD))
                // {
                //     items[i].UpdateStatus(LocationStatus.Available, locationMeta);
                // }
                else
                    items[i].UpdateStatus(LocationStatus.Locked, locationMeta);
            }
        }

        public void Show()
        {
            background.SetActive(true);
            tooltip?.HideTooltip();
            UpdateList();
        }

        public void Hide()
        {
            background.SetActive(false);
            Services.Player.OnChangedLocation -= cm => UpdateList();
        }

        public string GetName() => "Menu.Map".Localize();
        public GameObject GetGameObject() => gameObject;

    }
}