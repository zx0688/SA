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

        [SerializeField] private UICurrent backpack;
        [SerializeField] private UIChoicePanel down;
        [SerializeField] private UIChoicePanel up;

        private ProfileData profile => Services.Player.Profile;

        void Awake()
        {
            down.OnEndTap += t =>
            {
                up.HideAll();
            };
            up.OnEndTap += t =>
            {
                down.HideAll();
            };
        }

        public int CurrentChoice()
        {
            if (down.IsTaped)
                return CardMeta.LEFT;
            if (up.IsTaped)
                return CardMeta.RIGHT;
            return -1;
        }

        public void Show(SwipeData data)
        {
            this.Data = data;

            if (data.Choices.Count == 0)
                throw new Exception($"State Choice should have any next card {data.Card.Id}");


            List<string> allReward = new List<string>();
            down.ShowChoice(data.Choices[0], profile.Choices[0], data.FollowPrompt == CardMeta.LEFT, allReward);
            up.ShowChoice(data.Choices[1], profile.Choices[1], data.FollowPrompt == CardMeta.RIGHT, allReward);

            backpack.SetItems(allReward.Distinct().Select(s => new ItemData(s, 0)).ToList(), profile, Services.Meta.Game);

        }

        public void Hide()
        {
            backpack.Hide();
            down.HideAll();
            up.HideAll();
        }

    }
}