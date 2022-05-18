using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace Controllers
{
    public class ItemStateController : MonoBehaviour
    {
        [SerializeField]
        public int itemId;


        // [SerializeField]
        //private Sprite customSprite;

        protected PlayerService player;

        protected GameObject icon;
        protected GameObject image;

        private Text valueText;
        private Slider slider;
        private SliderQuant sliderQuant;

        void Awake()
        {

            if (Services.isInited)
                Init();
            else
                Services.OnInited += Init;
        }

        public void ChangeID(int id)
        {
            itemId = id;
            InitHUD().Forget();
            OnUpdateCountP();
        }

        void Start()
        {

        }

        private void Init()
        {
            Services.OnInited -= Init;

            if (!isAvailable())
                return;

            player = Services.Player;
            player.OnProfileUpdated += OnUpdateCountP;
            //player.OnRewardReceived += OnUpdateCount;

            slider = gameObject.GetComponent<Slider>();
            sliderQuant = gameObject.GetComponent<SliderQuant>();

            valueText = transform.Find("Value")?.gameObject.GetComponent<Text>();
            icon = transform.Find("Icon")?.gameObject;
            image = transform.Find("Image")?.gameObject;

            InitHUD().Forget();
            OnUpdateCountP();
        }

        private void OnUpdateCountP()
        {
            // ItemVO r = new ItemVO(itemId, 0);
            //List<ItemVO> rr = new List<ItemVO>();
            //rr.Add(r);
            //OnUpdateCount(rr);
        }

        async UniTaskVoid InitHUD()
        {
            ItemMeta resinfo = Services.Data.ItemInfo(itemId);
            Text t = transform.Find("Name")?.gameObject.GetComponent<Text>();
            if (t != null)
            {
                t.text = resinfo.Name;
            }

            Text d = transform.Find("Description")?.gameObject.GetComponent<Text>();
            if (d != null)
            {
                d.text = resinfo.Name;
            }

            if (icon != null)
            {
                Image ic = icon.GetComponent<Image>();
                ic.sprite = await Services.Assets.GetSprite("Items/" + itemId + "/icon", true);
            }

            if (image != null)
            {
                Image im = image.GetComponent<Image>();
                im.sprite = await Services.Assets.GetSprite("Items/" + itemId + "/icon", true);
            }
        }

        void OnDisable()
        {
            if (Services.isInited && isAvailable())
            {
                player.OnProfileUpdated -= OnUpdateCountP;
                //player.OnRewardReceived -= OnUpdateCount;
            }
        }

        void OnDestroy()
        {
            if (Services.isInited && isAvailable())
            {
                player.OnProfileUpdated -= OnUpdateCountP;
                //player.OnGetReward -= OnUpdateCount;
            }
        }
        void OnEnable()
        {

            if (Services.isInited && isAvailable())
            {
                player.OnProfileUpdated += OnUpdateCountP;
                // player.OnGetReward += OnUpdateCount;
            }

        }

        public virtual bool isAvailable()
        {
            return itemId > 0;
        }

        public virtual void OnUpdateCount(List<ItemVO> rs)
        {

            if (!isAvailable())
                return;

            ItemVO res = rs.Find(r => r.Id == itemId);
            if (res == null)
                return;

            int _value = 0;//player.itemHandler.AvailableItem(itemId);

            if (sliderQuant != null)
            {
                sliderQuant.SetValue(_value);
            }
            else if (slider != null)
            {
                int maxValue = 99999;
                slider.minValue = 0;
                slider.maxValue = maxValue;
                slider.value = _value;
            }

            if (valueText != null)
                valueText.text = _value.ToString();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}