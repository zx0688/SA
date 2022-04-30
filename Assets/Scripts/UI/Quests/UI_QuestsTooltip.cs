using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;



public class UI_QuestsTooltip : MonoBehaviour, ITick
{

    private Image image;
    private Text description;
    private Text title;
    private Text timeLeft;

    private UI_Reward reward;
    private UI_Reward fine;

    private CardData cardData;
    private QuestVO cardItem;

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    public void ShowTooltip(QuestVO item, CardData data)
    {
        gameObject.SetActive(true);
        cardItem = item;
        cardData = data;

        title.text = cardData.Name;
        description.text = cardData.Act.Text;

        // reward.SetAsItems(null);//cardData.right != null ? cardData.right.reward : null);
        //fine.SetAsItems(null);//cardData.left != null ? cardData.left.reward : null);

        LoadSprite().Forget();
        OnEnable();
    }

    void OnEnable()
    {
        if (IsTickble())
            Tick(GameTime.Current);
    }

    async UniTaskVoid LoadSprite()
    {
        image.sprite = await Services.Assets.GetSprite("Cards/" + cardData.Id + "/icon", true);
    }

    void Awake()
    {
        image = transform.Find("Image").GetComponent<Image>();

        title = transform.Find("Name").GetComponent<Text>();
        description = transform.Find("Description").GetComponent<Text>();

        timeLeft = transform.Find("TimeLeft").GetComponent<Text>();
        reward = transform.Find("Reward").GetComponent<UI_Reward>();
        fine = transform.Find("Fine").GetComponent<UI_Reward>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HideTooltip();
        }
    }

    public void Tick(int timestamp)
    {
        timeLeft.text = TimeFormat.TWO_CELLS_FULLNAME(GameTime.Left(timestamp, cardItem.activated, cardData.Act.Time));
    }

    public bool IsTickble()
    {
        return gameObject.activeSelf && cardData != null && cardData.Act.Time > 0;
    }
}