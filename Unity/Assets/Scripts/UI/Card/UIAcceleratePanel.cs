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
        [SerializeField] private Text timerText;
        [SerializeField] private Image item;
        [SerializeField] private Text rerollGone;
        [SerializeField] private Text competitionRecord;
        [SerializeField] private GameObject background;
        [SerializeField] private Text buttonText;

        private int duration = 0;
        private int currentCount = 0;
        private RewardMeta priceRerollItem;

        private int timestampEndReroll => 0;

        private Coroutine timer;

        void OnEnable()
        {
            StopAllCoroutines();

            if (!Services.isInited)
                return;
        }

        public void Hide()
        {
            StopAllCoroutines();
            timer = null;

            background.SetActive(false);
            gameObject.SetActive(false);
            timerText.gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            background.SetActive(true);
            item.gameObject.SetActive(true);
            timerText.gameObject.SetActive(true);

            accelerateBtn.SetAsDisabled = true;
            Services.Player.Profile.Items.TryGetValue(priceRerollItem.Id, out ItemData i);
            currentCount = i != null ? i.Count : 0;

            TickUpdate(GameTime.Get());
            rerollGone.text = ZString.Format("{0} {1}", Services.Player.Profile.Rerolls + 1, "Reroll.RerollGone".Localize().ToLower());
            competitionRecord.text = "рекорд: 999";

            timer = StartCoroutine(Tick());
            timer = null;
        }

        protected override void OnServicesInited()
        {
            base.OnServicesInited();

            duration = Services.Meta.Game.Config.DurationReroll;
            var r = Services.Meta.Game.Config.PriceReroll[0];
            priceRerollItem = new RewardMeta();
            priceRerollItem.Id = r.Id;
            priceRerollItem.Type = ConditionMeta.ITEM;
            priceRerollItem.Count = r.Count;
            item.LoadItemIcon(r.Id);
            accelerateBtn.OnClick += Accelerate;
        }


        private void TickUpdate(int time)
        {
            int timeLeft = GameTime.Left(time, Services.Player.Profile.Cooldown, duration);
            if (timeLeft <= 0)
            {
                timerText.gameObject.SetActive(false);
                item.gameObject.SetActive(false);
                buttonText.Localize("Reroll.Reroll");
                accelerateBtn.SetAsDisabled = false;
                buttonText.color = Color.green;
            }
            else
            {
                timerText.text = TimeFormat.ONE_CELL_FULLNAME(timeLeft);
                priceRerollItem.Count = SL.GetPriceReroll(timeLeft, Services.Meta.Game);

                buttonText.text = currentCount >= priceRerollItem.Count ? ZString.Format("-{0}", priceRerollItem.Count) : priceRerollItem.Count.ToString();
                buttonText.color = currentCount >= priceRerollItem.Count ? Color.red : Color.white;
                accelerateBtn.SetAsDisabled = currentCount < priceRerollItem.Count;
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

            Services.Player.Accelerate();

            Hide();
        }
    }
}