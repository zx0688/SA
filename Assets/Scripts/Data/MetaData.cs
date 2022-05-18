using System;
using System.Collections.Generic;

namespace Data
{
    [Serializable]
    public class MetaData
    {
        public const int CARD = 0;
        public const int ITEM = 1;
        public const int BUILDING = 2;
        public const int SKILL = 3;

        public CardMeta[] Cards;
        public CardMeta[] Quests;
        public ItemMeta[] Items;
        public BuildingMeta[] Buildings;
        public SkillMeta[] Skills;
        public List<LocationMeta> Locations;
        public ExpMeta[] Exp;
        public PlayerVO Profile;
        public ConfigMeta Config;
        public int Timestamp;
    }

    [Serializable]
    public class InteractiveData
    {

        public ActionMeta act;
        public string[] sound;
        public bool once;
        public int type;
    }

    [Serializable]
    public class ConfigMeta
    {
        public int Accelerate;
        public List<RewardData> Price;

    }


    [Serializable]
    public class LocationMeta
    {
        public int id;
        public List<RewardData> reward;
        public List<ConditionData> condi;
        public string descr;
        public string name;
        public string image;

    }

    [Serializable]
    public class RewardData
    {
        public int Id;
        public int Tp;
        public string[] Tags;
        public float Chance;
        public int Count;
        public List<ConditionData> Condi;

        public RewardData Clone()
        {
            RewardData r = new RewardData();
            r.Id = Id;
            r.Tp = Tp;
            r.Tags = Tags != null ? (string[])Tags.Clone() : new string[0];
            r.Chance = Chance;
            r.Count = Count;
            r.Condi = Condi != null ? new List<ConditionData>(Condi) : null;
            return r;
        }
    }


    [Serializable]
    public class BuildingMeta : ItemMeta
    {
        //public UpgradeData prod;
        public ActionMeta Act;

    }

    [Serializable]
    public class SkillMeta : ItemMeta
    {
        public bool One;
        public ActionMeta Act;
        public int Time;
    }

    [Serializable]
    public class ItemMeta
    {
        //public const int SIMPLE_ID = 0;
        //public const int BUILDING_ID = 1;
        public string Name;
        public string Descr;
        public int Id;
        public const int EXP_ID = 2;
        public const int LEVEL_ID = 3;
        public const int ACCELERATE_ID = 4;

        public bool hide;
        public string Des;
        public int Type;

    }

    [Serializable]
    public class ActionMeta
    {
        public int Time;
        public string Text;
        public string Image;
        public int Chance;
        public List<RewardData> Reward;
        public List<TriggerData> Tri;
        public List<ConditionData> Con;

        public ActionMeta Clone()
        {
            ActionMeta action = new ActionMeta();
            /*action.Time = Time;
            action.Text = Text;
            action.image = image;
            action.Chance = Chance;
            action.reward = new List<RewardData>(reward);
            action.trigg = new List<TriggerData>(trigg);
            action.Con = new List<ConditionData>(Con);
            */
            return action;
        }
        public List<RewardData> GetCost()
        {
            return new List<RewardData>();//reward != null && reward.Count > 0 ? reward.FindAll(r => r.count < 0) : new List<RewardData>();
        }
        public List<RewardData> GetReward()
        {
            return new List<RewardData>();//reward != null && reward.Count > 0 ? reward.FindAll(r => r.count > 0) : new List<RewardData>();
        }
    }

    [Serializable]
    public class TriggerData
    {
        public const int CARD = 0;
        public const int ITEM = 1;
        public const int BUILDING = 2;
        public const int QUEST = 3;
        public const int ACTION = 4;
        public const int START_GAME = 10;
        public const int GOT_LEVEL = 11;
        public const int ALLOW = -1;
        //===============================
        public const int CHOICE_GET = 0;
        public const int CHOICE_SPEND = 1;
        public const int CHOICE_PRODUCTION = 2;

        public int Id;
        public int Tp;
        public string[] tags;
        public int Choice;
    }

    [Serializable]
    public class ExpMeta
    {
        public int Id;
        public int Count;
        public List<RewardData> Reward;
    }

    [Serializable]
    public class ConditionData
    {
        public int Id;
        public int Tp;
        public string[] Tags;
        public bool Invert;
        public string Sign;
        public int Choice;
        public int Count;
        public string[] Loc;

    }
    [Serializable]
    public class CardMeta
    {

        public int Id;
        public int Pri;
        public bool Once;
        public int Hero;
        public string Name;
        public List<RewardData> Drop;
        public ActionMeta Act;
        public ActionMeta Left;
        public ActionMeta Right;
    }

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
}