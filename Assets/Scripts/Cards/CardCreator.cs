using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public static class CardCreator
{
    public static CardMeta NewLevel(ExpMeta newLevel)
    {
        CardMeta cardData = new CardMeta();
        /*cardData.Id = 9999;
        //cardData.drops = newLevel.reward;
        cardData.prior = 100;
        cardData.nosave = true;
        cardData.type = 4;
        cardData.left1 = new ActionData();
        cardData.left1.reward = newLevel.reward;
        //cardData.right1.reward = newLevel.reward;
        */
        return cardData;
    }
}