using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Cysharp.Threading.Tasks;
using Meta;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class DataService
{
    //==========
    // public static readonly string EFFECT = "effect";
    // public static readonly string BUILDING = "build";
    // public static readonly string CRAFT = "craft";

#if UNITY_STANDALONE_WIN
        private static readonly string GOOGLE_DRIVE = "https://drive.google.com/uc?export=download&id=1dXlv4S11TvwJTnc6qqFJIJJ_K6yXgzel";
#else
    private static readonly string GOOGLE_DRIVE = "https://drive.google.com/uc?export=download&id=1dXlv4S11TvwJTnc6qqFJIJJ_K6yXgzel"; //"https://drive.google.com/uc?export=download&id=1tWCVbt3hUimhZPh6lABPLJefNZYotS8K";
#endif
    private static readonly string URL_META = "";
    private static readonly string URL_VERSION = "";

    public GameMeta GameMeta;
    public int Version;

    public event Action OnUpdate;

    public Dictionary<List<ConditionMeta>, List<RewardMeta>> Recipes = new Dictionary<List<ConditionMeta>, List<RewardMeta>>();

    public static readonly List<RewardMeta> EMPTY_REWARD = new List<RewardMeta>();
    public static readonly List<ConditionMeta> EMPTY_CONDITIONS = new List<ConditionMeta>();

    public CardMeta[] CardDeckMeta => Services.Player.GetPlayerVO.Location switch
    {
        27912732 => GameMeta.Cards,
        _ => throw new Exception("undefined location ID")
    };
    // public ItemData accelerateItem; 


    public ItemMeta ItemInfo(int id) => Array.Find(GameMeta.Items, i => i.Id == id);
    public SkillMeta SkillInfo(int id) => Array.Find(GameMeta.Skills, i => i.Id == id);

    public List<ItemMeta> ItemInfoByType(int type)
    {
        return GameMeta.Items.Where(c => c.Type == type).ToList();
    }

    public CardMeta GetCardMetaByID(int id)
    {
        //Debug.Log("== " + id);
        //return game.cards[id - 2];
        return Array.Find(GameMeta.Cards, c => c.Id == id);
    }
    public CardMeta QuestInfo(int id)
    {
        return Array.Find(GameMeta.Quests, q => q.Id == id);
    }

    public bool MatchReward(List<RewardMeta> reward1, List<RewardMeta> reward2)
    {
        for (int i = 0; i < reward1.Count; i++)
        {
            RewardMeta r1 = reward1[i];
            RewardMeta r2 = reward2.Find(r => r.Tp == r1.Tp);
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

    /*public bool CheckOpenCondition(InteractiveData eData, CardVO cardVO, int time)
    {

        if (cardVO == null)
            return true;
        // else if (cardVO.executed == 0)
        //    return false;
        //else if(cardVO.activated < time && cardVO.activated > cardVO.executed)
        //    return false;
        else if (eData.once == true && cardVO.Activated > 0)
            return false;
        else if (eData.act.Time > 0 && GameTime.Left(time, cardVO.Executed, eData.act.Time) > 0)
            return false;
        return true;
    }*/

    public bool CheckTrigger(int Tp, int Id, int choice, ActionMeta action, PlayerService player)
    {
        if (action.Chance > 0)
        {
            int r = UnityEngine.Random.Range(0, 100);
            if (r >= action.Chance)
                return false;
        }

        if (action.Tri == null || action.Tri.Count == 0)
        {
            if (action.Con == null || action.Con.Count == 0)
                return false;

            return true;//CheckConditions(action.Con, cardMeta, cardVO, player);
        }

        //if (!CheckConditions(action.Con, cardMeta, cardVO, player))
        //    return false;

        foreach (TriggerMeta tr in action.Tri)
        {
            if (tr.Tp != Tp && tr.Tp != GameMeta.ANY)
                continue;

            if (tr.Tp == GameMeta.ANY)
                return true;

            switch (Tp)
            {
                case GameMeta.CARD:
                    if (tr.Id == Id && (tr.Choice == 0 || tr.Choice == choice))
                        return true;
                    break;

                case TriggerMeta.START_GAME:
                    return true;

                case GameMeta.ANY:
                    return true;

                default:
                    if (tr.Id == Id)
                        return true;
                    break;
            }
        }
        return false;
    }

    public bool CheckConditions(List<ConditionMeta> conditions, CardMeta cardMeta, CardVO cardVO, PlayerService player, List<RewardMeta> reward)
    {
        if (cardMeta != null && cardVO != null)
        {
            if (cardMeta.Once && cardVO.Executed > 0)
                return false;
            if (cardMeta.OnceRoll && cardVO.Executed > player.GetPlayerVO.SwipeReroll)
                return false;
        }

        if (conditions == null || conditions.Count == 0)
            return true;

        foreach (ConditionMeta c in conditions)
        {
            switch (c.Tp)
            {
                case GameMeta.CARD:
                    CardVO cardVO1 = player.GetCardVOByID(c.Id);
                    if (cardVO1 == null || cardVO1.Activated == 0 || cardVO1.Executed == 0)
                        return false;
                    break;

                case GameMeta.ITEM:
                    int value = player.GetCountItemByID(c.Id);
                    if (reward != null)
                    {
                        RewardMeta rm = reward.Find(rv => rv.Id == c.Id && rv.Tp == GameMeta.ITEM);
                        if (rm != null)
                            value += rm.Count;
                    }

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

        GameMeta = JsonUtility.FromJson<Meta.GameMeta>(asset);

        //Recipes = new Dictionary<List<RewardMeta>, List<RewardMeta>>();
        // quests = new List<CardData> ();
        //List<CardMeta> _asyncDurationCardList = new List<CardMeta>();

        foreach (CardMeta card in GameMeta.Cards)
        {
            if (card.Tags == null)
                card.Tags = new string[0];

            if (card.Reward == null)
                card.Reward = EMPTY_REWARD;

            /*if (card.Right.Reward == null)
                card.Right.Reward = EMPTY_REWARD;
            if (card.Left.Reward == null)
                card.Left.Reward = EMPTY_REWARD;
            */
            //else if (Array.IndexOf (card.tags, QUEST) != -1)
            //  quests.Add (card);

            /* List<ConditionMeta> cardcost = card.Act.Con.Where(c => c.Count > 0).ToList();
             if (cardcost.Count > 0)
             {
                 List<RewardMeta> totalReward = new List<RewardMeta>();
                 ApplyReward(totalReward, card.Left.Reward, 1f);
                 ApplyReward(totalReward, card.Right.Reward, 1f);
                 ApplyReward(totalReward, card.Act.Reward, 1f);
                 totalReward = totalReward.Where(r => r.Count > 0).ToList();
                 Recipes.Add(cost, reward);
             }
    */

            //totalReward.Where(r => r.Count > 0);
        }

        //--resources
        //itemIdByTag = new Dictionary<string, List<int>>();

        foreach (ItemMeta item in GameMeta.Items)
        {
            //if (item.T == null)
            //    item.Tags = new string[0];

        }

        Version = GameMeta.Timestamp;
        if (mversion != Version)
        {
            //SecurePlayerPrefs.SetInt("meta_version", version);
            OnUpdate?.Invoke();
        }
    }

    public int GetPriceReroll(int timeLeft) => (int)Math.Floor(timeLeft / (double)(GameMeta.Config.DurationReroll / GameMeta.Config.PriceReroll[0].Count));
}
