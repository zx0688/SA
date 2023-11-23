using System.Collections;
using System.Collections.Generic;
using Controllers;
using Meta;
using UnityEngine;


namespace Controllers
{
    public class ItemOneStateController : ItemStateController
    {
        // Start is called before the first frame update
        [SerializeField]
        public string[] tags;
        public override bool isAvailable()
        {
            return itemId > 0 || tags.Length > 0;
        }
        public override void OnUpdateCount(List<ItemVO> rs)
        {

            if (!isAvailable())
                return;

            ItemVO r = rs.Find(_r => _r.Id == itemId);
            if (r == null)
                return;

            ItemMeta rd = Services.Data.ItemInfo(r.Id);

            //if (tags.Length > 0 && rd. != null && Utils.Intersection(tags, rd.tags))
            //    return;

            int _value = 0;//player.itemHandler.AvailableItem(r.id);

            icon?.SetActive(_value > 0);
            image?.SetActive(_value > 0);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}