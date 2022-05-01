using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Meta;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class ItemActionController : MonoBehaviour
    {

        public RewardData data;
        public bool add;

        private GameObject addIcon;
        private GameObject subIcon;

        [SerializeField]
        private GameObject[] icons;
        [SerializeField]
        private GameObject value;

        public void UpdateDataSign(RewardData data, bool add)
        {
            this.add = add;
            UpdateData(data);
        }
        public void UpdateData(RewardData data)
        {

            this.data = data;
            this.gameObject.SetActive(false);
            UpdateHUD().Forget();
        }

        async UniTask UpdateHUD()
        {

            if (data.Tp == 3)
            {
                addIcon.SetActive(false);
                subIcon.SetActive(false);
            }
            else
            {
                if (addIcon != null && add)
                {
                    addIcon.SetActive(true);
                    subIcon.SetActive(false);
                }

                if (subIcon != null && add == false)
                {
                    subIcon.SetActive(true);
                    addIcon.SetActive(false);
                }
            }



            for (int i = 0; i < icons.Length; i++)
            {

                if (i > data.Count - 1)
                {
                    icons[i].SetActive(false);
                    continue;
                }

                icons[i].SetActive(true);
                Image icon = icons[i].GetComponent<Image>();
                if (data.Tp == 3)
                {
                    //ActionData ad = Services.Data.ActionInfo (data.id);
                    //icon.sprite = await Services.Assets.GetSprite ("Actions/" + ad.id + "/icon", true);
                }
                else
                {
                    ItemData res = Services.Data.ItemInfo(data.Id);
                    icon.sprite = await Services.Assets.GetSprite("Items/" + res.Id + "/icon", true);
                }
            }

            if (data.Count > 4)
            {
                value.SetActive(true);
                value.GetComponent<Text>().text = data.Count.ToString(); //add ? data.count.ToString () : "-" + data.count.ToString ();
            }
            else
            {
                value.SetActive(false);
            }

            this.gameObject.SetActive(true);
        }

        void Start()
        {
            add = true;

            addIcon = transform.Find("Add")?.gameObject;
            subIcon = transform.Find("Sub")?.gameObject;

        }

        void Update()
        {

        }

    }
}