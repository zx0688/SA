using System;
using System.Collections.Generic;


[Serializable]
public class PlayerVO
{

    public TutorVO tutorVO;
    public List<CardVO> cards;
    public List<QuestVO> quests;
    public List<InventoryVO> items;
    public List<SkillVO> skills;
    public List<BuildingVO> buildings;
    public string[] tags;
    public int timestamp;
    public bool first;

    public int locationId;
    public List<int> locations;
}

[Serializable]
public class TutorVO
{

    public TutorVO()
    {

    }
    public bool swipeCard = false;
}

[Serializable]
public class QuestVO : CardVO
{
    public const int STATE_ACTIVE = 0;
    public const int STATE_COMPLETE = 1;
    public const int STATE_EXPIRED = 2;
    public const int STATE_FAILED = 3;
    public QuestVO()
    {
        id = activated = executed = choice = 0;
        rew1 = rew2 = false;
        state = 0;
    }
    public int state;
    public bool rew1;
    public bool rew2;
}

[Serializable]
public class CardVO
{

    public CardVO()
    {
        id = activated = executed = choice = left = right = 0;
    }
    public int left;
    public int right;
    public int id;
    public int activated;
    public int executed;
    public int choice;
    public int rewardIndex;
}

[Serializable]
public class BuildingVO : InventoryVO
{
    public BuildingVO(int id, int count) : base(id, count)
    {

    }
    public int executed;
    public int stact;
    public int pos;
}

[Serializable]
public class SkillVO : InventoryVO
{
    public SkillVO(int id, int count) : base(id, count) { }
    public int time;
    public int activated;
}

[Serializable]
public class InventoryVO : ItemVO
{

    public const int IDLE_STATE = 0;
    public const int PROCESS_STATE = 1;

    public InventoryVO(int id, int count) : base(id, count) { }

}

[Serializable]
public class ItemVO
{

    public ItemVO(int id, int count)
    {
        this.id = id;
        this.count = count;
    }

    public int id;
    public int count;
}