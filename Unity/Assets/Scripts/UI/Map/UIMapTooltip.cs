using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization;
using Cysharp.Threading.Tasks;
using Meta;
using UI.Components;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI.Map
{
    public class UIMapTooltip : MonoBehaviour
    {
        [SerializeField] private TooltipBack background;

        [SerializeField] private Image icon;
        [SerializeField] private Text description;
        [SerializeField] private Button button;
        [SerializeField] private UIConditions conditions;
        [SerializeField] private Text textYouHere;
        [SerializeField] private Text conditionText;
        [SerializeField] private Button closeTouch;

        private CardMeta meta;

        private void OnClick()
        {
            Services.Player.ApplyChangeLocation(meta);
            HideTooltip();
        }

        public void HideTooltip()
        {
            button.onClick.RemoveAllListeners();
            closeTouch.onClick.RemoveAllListeners();

            background.Hide();
            background.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        public void ShowTooltip(CardMeta meta)
        {
            this.meta = meta;
            button.onClick.AddListener(OnClick);
            closeTouch.onClick.AddListener(() => HideTooltip());

            background.Show("blue", meta.Name);
            background.gameObject.SetActive(true);
            gameObject.SetActive(true);
            description.Localize(meta.Des);

            icon.SetImage($"Cards/{meta.Image}");


            textYouHere.gameObject.SetActive(false);
            button.gameObject.SetActive(false);

            conditionText.gameObject.SetActive(meta.Act.Con != null && meta.Act.Con.Count > 0);
            conditions.gameObject.SetActive(meta.Act.Con != null && meta.Act.Con.Count > 0);
            conditions.SetItem(meta.Act.Con);

            if (Services.Player.GetPlayerVO.Location == meta.Id)
            {
                textYouHere.gameObject.SetActive(true);
            }
            else if (Services.Data.CheckConditions(meta.Act.Con, meta, null, Services.Player, DataService.EMPTY_REWARD))
            {
                button.gameObject.SetActive(true);
                button.interactable = true;//SetActiveButton(true);
                conditionText.Localize("Условие выполнено");
            }
            else
            {
                button.gameObject.SetActive(true);
                //button.interactable = false;  //SetActiveButton(false);
                conditionText.Localize("Выполните условие");
            }

            button.interactable = true;

        }
    }
}