using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Cysharp.Threading.Tasks;

using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;


public class DataService
{

    public const int CARD_ID = 0;
    public const int ITEM_ID = 1;
    public const int QUEST_ID = 2;
    public const int BUILDING_ID = 3;
    public const int ACTION_ID = 4;
    public const int SKILL_ID = 5;

    //public const int ACTION_ID = 3;
    public static DataService instance = null;

    //==========
    public static readonly string EFFECT = "effect";
    public static readonly string BUILDING = "build";
    public static readonly string CRAFT = "craft";

#if UNITY_STANDALONE_WIN
        private static readonly string GOOGLE_DRIVE = "https://drive.google.com/uc?export=download&id=1dXlv4S11TvwJTnc6qqFJIJJ_K6yXgzel";
#else
    private static readonly string GOOGLE_DRIVE = "https://drive.google.com/uc?export=download&id=1dXlv4S11TvwJTnc6qqFJIJJ_K6yXgzel"; //"https://drive.google.com/uc?export=download&id=1tWCVbt3hUimhZPh6lABPLJefNZYotS8K";
#endif
    private static readonly string URL_META = "";
    private static readonly string URL_VERSION = "";

    //private delegate 

    public GameData Game;
    public int Version;
    public event Action OnUpdate;


    // public ItemData accelerateItem;

    public List<RewardData> ApplyReward(List<RewardData> reward, List<RewardData> concated, float multi)
    {
        for (int i = 0; i < concated.Count; i++)
        {
            RewardData c = concated[i];
            RewardData r = reward.Find(_r => _r.Id == c.Id && _r.Tp == c.Tp);
            if (r == null)
            {
                r = c.Clone();
                reward.Add(r);
            }
            else
            {
                // r.chance = r.chance > 0 ? (r.chance + c.chance) / 2 : 0;
                r.Count += c.Count; //Mathf.FloorToInt (c.count * multi);
            }

        }
        return reward;
    }

    public BuildingData BuildingInfo(int id)
    {
        return Game.Buildings[id - 2];
    }
    public ItemData ItemInfo(int id)
    {
        return Array.Find(Game.Items, i => i.Id == id);
    }
    public SkillData SkillInfo(int id)
    {
        return Array.Find(Game.Skills, s => s.Id == id);
    }

    public List<ItemData> ItemInfoByType(int type)
    {
        return Game.Items.Where(c => c.Type == type).ToList();
    }

    public CardData CardInfo(int id)
    {
        Debug.Log("== " + id);
        //return game.cards[id - 2];
        return Array.Find(Game.Cards, c => c.Id == id);
    }
    public CardData QuestInfo(int id)
    {
        return Array.Find(Game.Quests, q => q.Id == id);
    }

    public bool MatchReward(List<RewardData> reward1, List<RewardData> reward2)
    {
        for (int i = 0; i < reward1.Count; i++)
        {
            RewardData r1 = reward1[i];
            RewardData r2 = reward2.Find(r => r.Tp == r1.Tp);
            if (r2 == null || r1.Count != r2.Count)
                return false;
        }
        return true;
    }

    public bool CheckGlobalState(PlayerService player, PlayerService enemy)
    {
        return true;
    }

    /*  public ActionData GetChoiceData (CardData card, int choice) {
         switch (choice) {
             case Swipe.LEFT_CHOICE:
                 return card.left;
             case Swipe.RIGHT_CHOICE:
                 return card.right;
             case Swipe.UP_CHOICE:
                 return card.up;
             case Swipe.DOWN_CHOICE:
                 return card.down;
         }
         return null;
     }*/

    public bool CheckOpenCondition(InteractiveData eData, CardVO cardVO, int time)
    {

        if (cardVO == null)
            return true;
        // else if (cardVO.executed == 0)
        //    return false;
        //else if(cardVO.activated < time && cardVO.activated > cardVO.executed)
        //    return false;
        else if (eData.once == true && cardVO.activated > 0)
            return false;
        else if (eData.act.Time > 0 && GameTime.Left(time, cardVO.executed, eData.act.Time) > 0)
            return false;
        return true;
    }

    public bool ActionTrigger(TriggerVO trigger, ActionData data, int startTime, int timestamp)
    {

        if (startTime > 0 && GameTime.Left(timestamp, startTime, data.Time) <= 0)
            return false;

        if (data.Tri == null || data.Tri.Count == 0)
        {
            if (data.Con == null || data.Con.Count == 0)
                return false;
            return CheckConditions(data.Con, timestamp);
        }

        foreach (TriggerData tr in data.Tri)
        {

            if (tr.tp == TriggerData.ALLOW)
                return true;

            if (tr.tp != trigger.tp)
                continue;
            switch (trigger.tp)
            {
                case TriggerData.ACTION:
                    //if (tr.id == trigger.id && CheckConditions (data.condi, timestamp))
                    //    return true;
                    break;
                case TriggerData.CARD:
                    if (tr.id == trigger.id && (tr.choice == 0 || tr.choice == trigger.choice) && CheckConditions(data.Con, timestamp))
                        return true;
                    break;
                case TriggerData.ITEM:

                    if (trigger.reward.Exists(r => r.Id == tr.id) && CheckConditions(data.Con, timestamp))
                        return true;

                    break;
                case TriggerData.BUILD:

                    if ((tr.choice == 0 || tr.choice == trigger.choice) && trigger.reward.Exists(r => r.Id == tr.id) && CheckConditions(data.Con, timestamp))
                        return true;

                    break;
                case TriggerData.QUEST:
                    //if (tr.id == currentTrigger.id) {
                    //return startTime > 0 ? GameTime.TimeLeft (timestamp, startTime, data.time) <= 0 && CheckConditions (data.condi, timestamp) : CheckConditions (data.condi, timestamp);
                    //}
                    break;
                case TriggerData.START_GAME:
                    if (CheckConditions(data.Con, timestamp))
                        return true;
                    break;
            }
        }
        return false;
    }

    public bool CheckConditions(List<ConditionData> conditions, int time)
    {

        if (conditions == null || conditions.Count == 0)
            return true;

        PlayerService player = Services.Player;

        foreach (ConditionData c in conditions)
        {

            switch (c.Tp)
            {
                case -1:
                    return true;
                // case ACTION_ID:
                /*ItemVO action = player.currentAction;
                if (action == null || action.id != c.id)
                    return false;
                break;*/
                case CARD_ID:
                    CardVO cardVO1 = player.playerVO.cards.Find(crt => crt.id == c.Id);
                    if (cardVO1 == null || cardVO1.activated == 0 || cardVO1.executed == 0)
                        return false;
                    break;
                case QUEST_ID:
                    QuestVO cardVO2 = player.playerVO.quests.Find(crt => crt.id == c.Id);
                    //if (cardVO2 == null || cardVO2.executed == 0 || c.choice == 0)
                    return false;
                    break;
                case BUILDING_ID:
                    int value = 0;//player.buildingHandler.AvailableItem(c.id, 0);
                    switch (c.Sign)
                    {
                        case ">":
                            if (!(value > c.Count))
                                return false;
                            break;
                        case "==":
                            if (!(c.Count == value))
                                return false;
                            break;
                        default:
                            if (value == 0)
                                return false;
                            break;
                    }
                    break;

                case ITEM_ID:
                    value = 0;//player.itemHandler.AvailableItem(c.id);
                    switch (c.Sign)
                    {
                        case ">":
                            if (!(value > c.Count))
                                return false;
                            break;
                        case "==":
                            if (!(c.Count == value))
                                return false;
                            break;
                        case "<=":
                            if (!(value <= c.Count))
                                return false;
                            break;
                        case ">=":
                            if (!(value >= c.Count))
                                return false;
                            break;
                        case "<":
                            if (!(value < c.Count))
                                return false;
                            break;
                        default:
                            if (value == 0)
                                return false;
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        return true;
    }

    public async UniTask Init(IProgress<float> progress = null)
    {
        int mversion = SecurePlayerPrefs.GetInt("meta_version");

        var asset = await Services.Assets.GetJson("meta", false, GOOGLE_DRIVE, false, progress);

        Debug.Log(asset);

        Game = JsonUtility.FromJson<GameData>(asset);

        // quests = new List<CardData> ();
        List<CardData> _asyncDurationCardList = new List<CardData>();

        foreach (CardData card in Game.Cards)
        {
            //if (card.tags == null)
            //    card.tags = new string[0];

#if UNITY_EDITOR
            // meta validation
            /*     if (card.Hero == -1 || card.Hero > 0)
                 {
                     if ((card.Act.reward.Count > 0) || (card.left1.reward != null && card.left1.reward.Count > 0) || (card.Right.reward != null && card.right1.reward.Count > 0))
                     {
                         throw new Exception("Карта " + card.Id + " установлен герой и награда одновременно, нельзя");
                     }
                 }
     */
            //if (card.left1 != null && card.left1.reward != null && card.left1.reward.Count > 0 && card.right1 != null && card.right1.reward != null && card.right1.reward.Count == 0)
            //   throw new Exception("Карта " + card.Id + " награда лево, без награды вправо не предусмотрена");

#endif
            /* else if(card.tags.Length > 0)
             {
                 foreach(string tag in card.tags)
                 {
                     List<int> list = new List<int>();
                     cardIdByTag.TryGetValue(tag, out list);
                     list.Add(card.id);
                     cardIdByTag[tag] = list;
                 }
             } */
            //else if (Array.IndexOf (card.tags, QUEST) != -1)
            //  quests.Add (card);
        }

        //--resources
        //itemIdByTag = new Dictionary<string, List<int>>();

        foreach (ItemData item in Game.Items)
        {
            //if (item.T == null)
            //    item.Tags = new string[0];

        }

        Version = Game.Timestamp;
        if (mversion != Version)
        {
            //SecurePlayerPrefs.SetInt("meta_version", version);
            OnUpdate?.Invoke();
        }
    }

    void Awake()
    {
        Version = -1;
    }

}
