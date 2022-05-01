using System.Collections;
using System.Collections.Generic;
using Meta;
using UnityEngine;


public class LocationHandler
{

    private PlayerVO player;
    private DataService dataManager;
    public LocationHandler(PlayerVO playerVO)
    {
        this.player = playerVO;
        dataManager = Services.Data;
    }
    public int AvailableItem(int id, int type)
    {
        return player.locations.IndexOf(id) != -1 ? id : 0;
    }

    public void ChangeLocation(LocationData locationData)
    {
        player.locationId = locationData.id;
    }

    public int Add(int id, int count, LocationData data, int time)
    {

        if (player.locations.IndexOf(data.id) != -1)
            return -1;

        foreach (RewardData r in data.reward)
        {
            //Services.Player.itemHandler.Add (r.id, -r.count, null, 0);
        }

        player.locations.Add(data.id);

        return data.id;
    }

    //public void Trigger(List<CardData> queue, TriggerVO trigger, List<RewardData> reward, int time)


    public int GetVO(int id, int type)
    {
        return id;
    }

    public CardData CreateCard(LocationData data, int vo, int state, int time)
    {
        throw new System.NotImplementedException();
    }

    public int Add(LocationData data, int count, int time)
    {
        throw new System.NotImplementedException();
    }

    public int Change(LocationData data, int time)
    {
        throw new System.NotImplementedException();
    }
}