using System.Collections;
using System.Collections.Generic;
using Meta;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Map
{
    public enum LocationStatus
    {
        Active,
        Locked,
        Available
    }

    public class UILocationButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image image;
        [SerializeField] private int id;
        [SerializeField] private GameObject active;
        [SerializeField] private GameObject available;
        [SerializeField] private GameObject lockedBorder;
        [SerializeField] private GameObject locked;

        public int Id => id;

        private UIMapTooltip tooltip;
        private CardMeta meta;

        public void UpdateStatus(LocationStatus status, CardMeta meta)
        {
            this.meta = meta;
            active.gameObject.SetActive(false);
            available.gameObject.SetActive(false);
            lockedBorder.gameObject.SetActive(false);
            locked.gameObject.SetActive(false);
            image.SetImage($"Cards/{meta.Image}");

            switch (status)
            {
                case LocationStatus.Active:
                    active.gameObject.SetActive(true);
                    break;
                case LocationStatus.Locked:
                    lockedBorder.gameObject.SetActive(true);
                    locked.gameObject.SetActive(true);
                    break;
                case LocationStatus.Available:
                    available.gameObject.SetActive(true);
                    break;
            }

        }

        public void SetTooltip(UIMapTooltip tooltip)
        {
            this.tooltip = tooltip;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            tooltip.ShowTooltip(meta);
        }
    }
}

