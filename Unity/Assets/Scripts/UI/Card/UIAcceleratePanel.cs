using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Cysharp.Text;
using GameServer;
using haxe.root;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UIAcceleratePanel : ServiceBehaviour
    {
        [SerializeField] private ClickButton accelerateBtn;
        [SerializeField] private Text buttonText;
        [SerializeField] private UIRewardItem item;
        [SerializeField] private Text rerollGone;
        [SerializeField] private Text competitionRecord;
        [SerializeField] private GameObject background;

        private int duration = 0;
        private RewardMeta priceRerollItem;

        private int timestampEndReroll => 0;

        private Coroutine timer;

        void OnEnable()
        {
            StopAllCoroutines();

            if (!Services.isInited)
                return;

            timer = StartCoroutine(Tick());
            timer = null;
        }

        public void Hide()
        {
            StopAllCoroutines();
            timer = null;
            background.SetActive(false);
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            background.SetActive(true);

            TickUpdate(GameTime.Get());
            rerollGone.text = ZString.Format("{0} {1}", Services.Player.Profile.Rerolls + 1, "Reroll.RerollGone".Localize().ToLower());
            competitionRecord.text = "рекорд: 999";
        }

        protected override void OnServicesInited()
        {
            base.OnServicesInited();

            duration = Services.Meta.Game.Config.DurationReroll;
            var r = Services.Meta.Game.Config.PriceReroll[0];
            priceRerollItem = new RewardMeta(r.Id, ConditionMeta.ITEM, r.Count);
            item.SetItem(priceRerollItem);
            accelerateBtn.OnClick += Accelerate;
        }


        private void TickUpdate(int time)
        {
            int timeLeft = GameTime.Left(time, Services.Player.Profile.Cooldown, duration);
            if (timeLeft <= 0)
            {
                buttonText.Localize("Reroll.Reroll");
            }
            else
            {
                buttonText.text = TimeFormat.ONE_CELL_FULLNAME(timeLeft);
                priceRerollItem.Count = SL.GetPriceReroll(timeLeft, Services.Meta.Game);
                item.SetItem(priceRerollItem);
            }
        }

        IEnumerator Tick()
        {
            while (true)
            {
                TickUpdate(GameTime.Get());
                yield return new WaitForSeconds(1f);
            }
        }

        private void Accelerate()
        {
            if (Services.Player.Profile.Deck.Count > 0)
            {
                Debug.LogWarning("can't accelerate if cards are available");
                return;
            }

            int timeLeft = GameTime.Left(GameTime.Get(), Services.Player.Profile.Cooldown, duration);
            if (timeLeft > 0)
            {
                int price = SL.GetPriceReroll(GameTime.Left(GameTime.Get(), Services.Player.Profile.Cooldown, duration), Services.Meta.Game);
                if (!Services.Player.Profile.Items.TryGetValue(priceRerollItem.Id, out ItemData i) || i.Count < price)
                {
                    Debug.LogWarning("can't accelerate if not enough price");
                    return;
                }
            }

            Services.Player.Accelerate();

            Hide();
        }


    }
}