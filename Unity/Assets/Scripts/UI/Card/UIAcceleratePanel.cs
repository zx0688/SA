using System;
using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization;
using Meta;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UIAcceleratePanel : ServiceBehaviour
    {
        [SerializeField] private Button accelerateBtn;
        [SerializeField] private Text buttonText;
        [SerializeField] private UI_RewardItem item;

        private int duration = 0;
        private RewardMeta priceReroll;

        private int timestampEndReroll => 0;

        private Coroutine timer;

        void Start()
        {
            accelerateBtn.onClick.AddListener(Accelerate);

            gameObject.SetActive(false);
        }

        void OnEnable()
        {
            StopAllCoroutines();

            if (!Services.isInited)
                return;

            Services.Player.OnItemReceived += OnItemReceived;
            timer = StartCoroutine(Tick());
            timer = null;
        }

        void OnDisable()
        {
            Services.Player.OnItemReceived -= OnItemReceived;
            StopAllCoroutines();
            timer = null;
        }

        public void ShowMe()
        {
            gameObject.SetActive(true);

            priceReroll.Count = Services.Data.GameMeta.Config.PriceReroll[0].Count;
            item.SetItem(priceReroll);
        }

        protected override void OnServicesInited()
        {
            base.OnServicesInited();

            duration = Services.Data.GameMeta.Config.DurationReroll;
            priceReroll = Services.Data.GameMeta.Config.PriceReroll[0].Clone();

            item.SetItem(priceReroll);

            //iconItemVO = new ItemVO(ItemMeta.ACCELERATE_ID, 0);
            //Services.Player.OnProfileUpdated += OnProfileUpdated;
            /*timePerItem = Services.Data.Meta.Config.Accelerate;

            List<RewardData> priceData = new List<RewardData>();//Services.Data.GameData.Config.Price;
            ItemVO ivo = new ItemVO(ItemMeta.ACCELERATE_ID, priceData[0].Id);
            buyBtn.transform.Find("Price").GetComponent<Text>().text = priceData[0].Count.ToString();
            buyBtn.GetComponent<UI_InventoryItem>().SetItem(ivo);

            accelerateItemVO = new ItemVO(ItemMeta.ACCELERATE_ID, 0);
            accelerateItem.SetItem(accelerateItemVO);

            availableCount = Services.Player.AvailableItem(iconItemVO.Id);
            SetItem(availableCount, GameTime.Current); */
        }


        private void OnItemReceived(List<RewardMeta> reward)
        {
            if (gameObject.activeInHierarchy == false)
                return;

            // item.SetItem(Services.Player.GetItemVOByID(itemId).CreateReward());
            //availableCount = Services.Player.AvailableItem(iconItemVO.Id);
            /*if (GameTime.Left(GameTime.Current, timestampEndReroll, timeReroll) > 0)
            {
                timer = StartCoroutine(Tick());
                timer = null;
            }
            else
            {
                StopAllCoroutines();
                timer = null;
            }*/

            //SetItem(availableCount, GameTime.Current);
        }

        private void TickUpdate(int time)
        {

            int timeLeft = GameTime.Left(time, Services.Player.GetPlayerVO.TimestampStartReroll, duration);
            if (timeLeft <= 0)
            {
                buttonText.text = "Reroll";
            }
            else
            {
                buttonText.text = TimeFormat.ONE_CELL_FULLNAME(timeLeft);

                priceReroll.Count = Services.Data.GetPriceReroll(timeLeft);
                item.SetItem(priceReroll);
            }





            /*        accelerateBtn.gameObject.SetActive(availableCount > 0);
                    buyBtn.gameObject.SetActive(availableCount <= 0);
                    actionTf.text = LocalizationManager.Localize(availableCount > 0 ? "accelerate" : "buyfor");

                    if (availableCount > 0)
                    {
                        float timeLeft = GameTime.Left(timestamp, start, wait);
                        int count = Mathf.CeilToInt(timeLeft / timePerItem);
                        count = count < 0 ? 0 : count;
                        accelerateItemVO.Count = Math.Min(availableCount, count);
                        accelerateItem.SetItem(accelerateItemVO);
                    }

                    iconItemVO.Count = availableCount;
                    iconItem.SetItem(iconItemVO);
                    */
        }

        IEnumerator Tick()
        {
            while (true)
            {
                TickUpdate(GameTime.Current);
                yield return new WaitForSeconds(1f);
            }
        }

        private void Accelerate()
        {
            Services.Player.ApplyReroll(GameTime.Current, 2);
            /*if (accelerateItemVO.Count == 0)
                return;
            if (wait <= 0)
                return;
            if (wait <= timePerItem)
                return;
            //int available = Services.Player.AvailableItem(ItemMeta.ACCELERATE_ID);
            //if (available <= 0)
            //    return;

            int timestamp = GameTime.Current;
            //SetItem(available, timestamp);

            Services.Player.Accelerate(timestamp, accelerateItemVO.Count);
            */

            gameObject.SetActive(false);

        }


    }
}