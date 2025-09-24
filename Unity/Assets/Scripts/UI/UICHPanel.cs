using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using haxe.root;
using UI.ActionPanel;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UICHPanel : MonoBehaviour
    {
        [HideInInspector] public SwipeData Data;
        [HideInInspector] public int C;

        //[SerializeField] private UICurrent backpack;

        [SerializeField] private Text madeAChoice;

        private ProfileData profile => Services.Player.Profile;

        void Awake()
        {

        }

        public void State(int i)
        {
            this.madeAChoice.gameObject.SetActive(i == 1);
        }

        public void Show(SwipeData data)
        {
            this.Data = data;

            this.madeAChoice.gameObject.SetActive(true);
            gameObject.SetActive(true);

            if (data.Choices.Count == 0)
                throw new Exception($"State Choice should have any next card {data.Card.Id}");


            //             List<string> allReward = new List<string>();
            // 
            //             up.FadeIn();
            //             down.FadeIn();
            // 
            //             madeAChoice.text = data.Choices.Exists(c => c.Reward != null || c.Cost != null) ?
            //                 "MadeAction.UI".Localize(LocalizePartEnum.GUI) : "MadeChocie.UI".Localize(LocalizePartEnum.GUI);
            // 
            //             down.ShowChoice(data.Choices[0], profile.Choices[0], data.FollowPrompt == CardMeta.LEFT, allReward);
            //             up.ShowChoice(data.Choices[1], profile.Choices[1], data.FollowPrompt == CardMeta.RIGHT, allReward);
            // 
            //             backpack.SetItems(allReward.Distinct().Select(s => new ItemData(s, 0)).ToList(), profile, Services.Meta.Game);

        }

        public void Hide()
        {
            //backpack.Hide();
            // down.HideAll();
            // up.HideAll();
            this.madeAChoice.gameObject.SetActive(false);

            gameObject.SetActive(false);
        }

    }
}