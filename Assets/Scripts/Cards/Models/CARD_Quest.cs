using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace Cards
{
    public class CARD_Quest : CARD_Base
    {

        private CardMeta questData;
        private int state;

        private Text status;
        private Image border;

        private Text header;
        private UI_Reward reward;
        private UI_Reward reward2;

        /*  void Awake () {
            foreach (Transform g in transform.GetComponentsInChildren<Transform> ()) {
                switch (g.name) {
                    case "Status":
                        status = g.GetComponent<Text> ();
                        break;
                    case "Border":
                        border = g.GetComponent<Image> ();
                        break;
                    case "Image":
                        image = g.GetComponent<Image> ();
                        break;
                    case "Header":
                        header = g.GetComponent<Text> ();
                        break;
                    case "UI_Reward":
                        reward = g.GetComponent<UI_Reward> ();
                        break;
                    case "UI_Reward2":
                        reward2 = g.GetComponent<UI_Reward> ();
                        break;
                    case "Description":
                        description = g.GetComponent<Text> ();
                        break;
                    default:
                        break;
                }
            }

        }
    */

        public void UpdateData(SwipeData data)
        {
            this.Data = data;
            //questData = Services.Data.QuestInfo (data.cardData.id);
            // state = data.cardData.deck;
            UpdateHUD();
        }
        protected override async void UpdateHUD()
        {
            base.UpdateHUD();

            header.text = questData.Name;
            /*  description.text = questData.act.text;
             reward.gameObject.SetActive (false);
             reward2.gameObject.SetActive (false);

             Services.Assets.SetSpriteIntoImage (image, "Quests/" + questData.id + "/image", true).Forget ();

             status.gameObject.SetActive (true);
             switch (state) {
                 case 0:
                     status.text = "Новый квест";
                     reward.SetAsItems (questData.act.reward);
                     reward.SetHeader ("Card.Reward");

                     if (questData.right.condi != null)
                         reward2.SetAsConditions (questData.left.condi.Union(questData.right.condi).ToList());
                     else
                         reward2.SetAsConditions (questData.left.condi);

                     reward2.SetHeader ("Quest.Goal");

                     break;
                 case 1:
                     status.text = "Пройдена 1 часть квеста";
                     break;
                 case 2:
                     status.text = "Пройдена 2 часть квеста";
                     break;
                 case 3:
                     status.text = "Квест пройден";
                     reward.SetAsItems (questData.act.reward);
                     reward.SetHeader ("Card.Reward");
                     // reward2.SetAsItems (questData.creward);

                     break;
                 case 4:
                     status.text = "Квест истек";
                     break;
                 case 5:
                     status.text = "Квест провален";
                     break;
                 default:
                     status.gameObject.SetActive (false);
                     break;
             }*/
        }

    }
}