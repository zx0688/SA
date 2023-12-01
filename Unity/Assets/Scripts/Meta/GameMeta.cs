using System;
using System.Collections.Generic;
using Core;

// namespace Meta
// {
//     [Serializable] 
//     public class GameMeta
//     {
//         public const int ANY = -1;
//         public const int CARD = 0;
//         public const int ITEM = 1;
//         public const int BUILDING = 2;
//         public const int SKILL = 3;
//         public const int LOCATION = 4;
//         public const int QUEST = 5;

//         public CardMeta[] Cards;
//         public CardMeta[] All;
//         public CardMeta[] Quests;
//         public ItemMeta[] Items;
//         public SkillMeta[] Skills;
//         public CardMeta[] Locations;

//         public BuildingMeta[] Buildings;
//         public ExpMeta[] Exp;
//         public PlayerMeta Profile;
//         public ConfigMeta Config;
//         public int Timestamp;
//         public int Version;
//     }

//     [Serializable]
//     public class PlayerMeta
//     {

//         public List<int> Cards;
//         public List<RewardMeta> Reward;
//     }


//     [Serializable]
//     public class InteractiveData
//     {

//         public ActionMeta act;
//         public string[] sound;
//         public bool once;
//         public int type;
//     }

//     [Serializable]
//     public class ConfigMeta
//     {
//         public int DurationReroll;
//         public List<RewardMeta> PriceReroll;
//     }

//     [Serializable]
//     public class RewardMeta
//     {
//         public int Id;
//         public int Tp;
//         public string[] Tags;
//         public float Chance;
//         public int Count;
//         public int[] Random;
//         public List<ConditionMeta> Condi;

//         public RewardMeta Clone()
//         {
//             RewardMeta r = new RewardMeta();
//             r.Id = Id;
//             r.Tp = Tp;
//             r.Tags = Tags != null ? (string[])Tags.Clone() : new string[0];
//             r.Chance = Chance;
//             r.Count = Count;
//             r.Random = Random != null ? (int[])Random.Clone() : new int[0];
//             r.Condi = Condi != null ? new List<ConditionMeta>(Condi) : null;
//             return r;
//         }

//         public ConditionMeta ToCondition()
//         {
//             ConditionMeta c = new ConditionMeta();
//             c.Id = Id;
//             c.Tp = Tp;
//             c.Count = Math.Abs(Count);
//             return c;
//         }
//     }


//     [Serializable]
//     public class BuildingMeta : ItemMeta
//     {
//         //public UpgradeData prod;
//         public ActionMeta Act;

//     }

//     [Serializable]
//     public class SkillMeta : ItemMeta
//     {
//         public int Slot;
//         public string Image;
//     }

//     [Serializable]
//     public class ItemMeta
//     {
//         //public const int SIMPLE_ID = 0;
//         //public const int BUILDING_ID = 1;
//         public string Name;
//         public string[] Tags;
//         public int Particle;
//         public string Descr;
//         public int Id;
//         public const int EXP_ID = 2;
//         public const int LEVEL_ID = 3;
//         public const int ACCELERATE_ID = 4;

//         public bool hide;
//         public string Des;
//         public int Type;

//     }

//     [Serializable]
//     public class ActionMeta
//     {
//         public List<RewardMeta> Reward;
//         public List<TriggerMeta> Tri;
//         public List<ConditionMeta> Con;
//         public string Text;

//         public float Chance;

//         public ActionMeta Clone()
//         {
//             ActionMeta action = new ActionMeta();
//             /*action.Time = Time;
//             action.Text = Text;
//             action.image = image;
//             action.Chance = Chance;
//             action.reward = new List<RewardData>(reward);
//             action.trigg = new List<TriggerData>(trigg);
//             action.Con = new List<ConditionData>(Con);
//             */
//             return action;
//         }
//         public List<RewardMeta> GetCost()
//         {
//             return new List<RewardMeta>();//reward != null && reward.Count > 0 ? reward.FindAll(r => r.count < 0) : new List<RewardData>();
//         }
//         public List<RewardMeta> GetReward()
//         {
//             return new List<RewardMeta>();//reward != null && reward.Count > 0 ? reward.FindAll(r => r.count > 0) : new List<RewardData>();
//         }
//     }

//     [Serializable]
//     public class ChoiceMeta
//     {
//         public string Text;
//         public List<RewardMeta> Reward;

//         /*public List<RewardMeta> GetCost()
//         {
//             return new List<RewardMeta>();//reward != null && reward.Count > 0 ? reward.FindAll(r => r.count < 0) : new List<RewardData>();
//         }
//         public List<RewardMeta> GetReward()
//         {
//             return new List<RewardMeta>();//reward != null && reward.Count > 0 ? reward.FindAll(r => r.count > 0) : new List<RewardData>();
//         }*/
//     }

//     [Serializable]
//     public class TriggerMeta
//     {

//         // public const int ITEM = MetaData.ITEM;
//         // public const int BUILDING = MetaData.BUILDING;

//         public const int START_GAME = 10;
//         public const int REROLL = 11;
//         //public const int SWIPE = 12;

//         public int Id;
//         public int Tp;
//         public string[] tags;
//         public int Choice;
//     }

//     [Serializable]
//     public class ExpMeta
//     {
//         public int Id;
//         public int Count;
//         public List<RewardMeta> Reward;
//     }

//     [Serializable]
//     public class ConditionMeta
//     {
//         // public const int CARD = MetaData.CARD;
//         // public const int ITEM = MetaData.ITEM;
//         // public const int BUILDING = MetaData.BUILDING;

//         public int Id;
//         public int Tp;
//         public string[] Tags;
//         public bool Invert;
//         public string Sign;
//         public int Choice;
//         public int Count;
//         public string[] Loc;

//         public RewardMeta ToReward()
//         {
//             RewardMeta r = new RewardMeta();
//             r.Id = Id;
//             r.Tp = Tp;
//             r.Count = Count;
//             return r;
//         }

//     }

//     [Serializable]
//     public class CardMeta
//     {
//         public int Id;
//         public string[] Tags;
//         public int Pri;
//         public bool Once;
//         public bool OnceRoll;
//         public bool Event;
//         public bool Group;

//         public string Name;
//         public string Des = "";
//         public string Image = "";

//         public List<RewardMeta> Reward
//         {
//             get => Act.Reward;
//             set => Act.Reward = value;
//         }

//         public ActionMeta Act;

//         public ChoiceMeta Left;
//         public ChoiceMeta Right;
//     }

/*[Serializable]
public class TriggerVO
{

    public TriggerVO(int tp, int id, int choice, SwipeData swiped, CardData quest, ItemData item, List<RewardData> reward)
    {
        this.tp = tp;
        this.id = id;
        this.swiped = swiped;
        this.choice = choice;
        this.quest = quest;
        this.reward = reward;
        this.item = item;
    }
    public int tp;
    public int id;
    public int choice;
    public ItemData item;
    public SwipeData swiped;
    public CardData quest;
    public List<RewardData> reward;
}*/
//}