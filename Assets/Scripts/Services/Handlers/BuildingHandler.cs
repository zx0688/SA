using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingHandler
{

    private PlayerVO player;
    private DataService dataManager;
    public BuildingHandler(PlayerVO playerVO)
    {
        this.player = playerVO;
        dataManager = Services.Data;
    }
    public int AvailableItem(int id, int type)
    {
        BuildingVO b = player.buildings.Find(_r => _r.id == id);
        return b != null ? b.count : 0;
    }

    public BuildingVO Add(BuildingData data, int count, int time)
    {
        BuildingData d = dataManager.BuildingInfo(data.Id);
        BuildingVO current = null;

        current = player.buildings.Find(_r => _r.id == data.Id);
        if (current == null)
        {
            current = new BuildingVO(data.Id, 0);
            player.buildings.Add(current as BuildingVO);
        }

        BuildingVO r = new BuildingVO(data.Id, current.id);
        int max = 9999999;
        if (count + current.count < 0)
            r.count = -current.count;
        else if (count + current.count > max)
            r.count = max - current.count;
        else
            r.count = count;
        current.count += count;
        if (current.count < 0)
            current.count = 0;
        return r;
    }

    public void Trigger(List<CardData> queue, TriggerVO trigger, List<RewardData> reward, int time)
    {

        if (trigger.tp == TriggerData.BUILD && trigger.choice == TriggerData.CHOICE_PRODUCTION)
        {
            BuildingVO buildVO = GetVO(trigger.id, 0);
            buildVO.executed = time;
            buildVO.stact = 0;

            BuildingData buildingData = Services.Data.BuildingInfo(buildVO.id);
            Services.Data.ApplyReward(reward, buildingData.Act.Reward, buildVO.count);
        }

        //for any start trigger
        foreach (BuildingVO buildVO in player.buildings)
        {
            if (buildVO.count <= 0 || (buildVO.stact > 0 && buildVO.executed < buildVO.stact))
                continue;
            BuildingData buildData = dataManager.BuildingInfo(buildVO.id);
            if (Services.Data.ActionTrigger(trigger, buildData.Act, buildVO.stact, time))
            {

                buildVO.executed = 0;
                buildVO.stact = time;
            }
        }

    }

    public BuildingVO GetVO(int id, int type)
    {
        BuildingVO b = player.buildings.Find(_r => _r.id == id);
        return b;
    }

    public CardData CreateCard(BuildingData data, BuildingVO vo, int state, int time)
    {
        throw new System.NotImplementedException();
    }

    public BuildingVO Change(BuildingData data, int time)
    {
        throw new System.NotImplementedException();
    }
}