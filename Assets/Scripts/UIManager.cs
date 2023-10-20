using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;

    [SerializeField] Canvas itemCanvas;
    [SerializeField] Canvas levelInfoCanvas;
    [SerializeField] Canvas homeInfoCanvas;
    [SerializeField] Canvas loseInfoCanvas;
    [SerializeField] Canvas winInfoCanvas;
    [SerializeField] Canvas buyCanvas;
    [SerializeField] Canvas buyNotificationCanvas;

    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI levelText;

    [SerializeField] TextMeshProUGUI loseText;
    [SerializeField] TextMeshProUGUI scoreAddedText;

    [SerializeField] TextMeshProUGUI heartText;
    [SerializeField] TextMeshProUGUI totalScoreText;
    [SerializeField] TextMeshProUGUI coinText;
    [SerializeField] TextMeshProUGUI levelNumberText;

    [SerializeField] TextMeshProUGUI buyText;
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] TextMeshProUGUI buyNotificationText;
    [SerializeField] TextMeshProUGUI returnTileText;
    [SerializeField] TextMeshProUGUI findTileText;
    [SerializeField] GameObject[] buySlotObjectList;
    [SerializeField] GameObject buyReturnTileObject;
    [SerializeField] GameObject buyFindTileObject;

    [SerializeField] GameObject[] gameObjectNeedToHardCodeFixed;

    private void Start()
    {
        foreach(GameObject gameObject in gameObjectNeedToHardCodeFixed)
        {
            gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y - 100, gameObject.transform.localPosition.z);
        }
        GameObject exclusionObject = gameObjectNeedToHardCodeFixed[gameObjectNeedToHardCodeFixed.Length - 1];
        exclusionObject.transform.localPosition = new Vector3(exclusionObject.transform.localPosition.x, exclusionObject.transform.localPosition.y + 150, exclusionObject.transform.localPosition.z);
    }

    public void DisplayHomeLayout()
    {
        homeInfoCanvas.gameObject.SetActive(true);
    }

    public void DisplayLevelInfo()
    {
        DisplayLevel(gameManager.level);
        DisplayTime(gameManager.time);
        DisplayScore(gameManager.score);
        DisplaySlot(gameManager.maxSlot);
        DisplayReturnTileHelp(gameManager.returnTile);
        DisplayFindTileHelp(gameManager.findTile);
    }

    public void DisplayHomeInfo()
    {
        DisplayLevel(gameManager.level);
        DisplayHeart(gameManager.heart);
        DisplayTotalScore(gameManager.totalScore);
        DisplayCoin(gameManager.coin);
    }

    public void DisplayScore(int score)
    {
        scoreText.SetText(score.ToString());
    }

    public void DisplayTime(int time)
    {
        timeText.SetText(ConvertTime(time));
    }

    public void DisplayLevel(int level)
    {
        levelNumberText.SetText(level.ToString());
        levelText.SetText("Lv." + level);
    }

    public void DisplayHeart(int heart)
    {
        heartText.SetText(heart.ToString());
    }

    public void DisplayTotalScore(int totalScore)
    {
        totalScoreText.SetText(totalScore.ToString());
    }

    public void DisplayCoin(int coin)
    {
        coinText.SetText(coin.ToString());
    }

    public void DisplaySlot(int maxSlot)
    {
        for (int i = 0; i < buySlotObjectList.Length; i++)
        {
            if (i < maxSlot)
            {
                buySlotObjectList[i].SetActive(false);
            }
            else
            {
                buySlotObjectList[i].SetActive(true);
            }
        }
    }

    public void DisplayReturnTileHelp(int returnTile)
    {
        returnTileText.SetText(returnTile.ToString());
        if(returnTile == 0)
        {
            buyReturnTileObject.SetActive(true);
        }
        else
        {
            buyReturnTileObject.SetActive(false);
        }
    }

    public void DisplayFindTileHelp(int findTile)
    {
        findTileText.SetText(findTile.ToString());
        if (findTile == 0)
        {
            buyFindTileObject.SetActive(true);
        }
        else
        {
            buyFindTileObject.SetActive(false);
        }
    }

    public void DisplayLose(string info)
    {
        loseText.SetText(info);
        loseInfoCanvas.gameObject.SetActive(true);
    }

    public void DisplayWin(int scoreAdded)
    {
        scoreAddedText.SetText("+ " + scoreAdded);
        winInfoCanvas.gameObject.SetActive(true);
    }

    public void DisplayBuyInfo(string titleThing, int price)
    {
        buyText.SetText(titleThing);
        priceText.SetText("- " + price);
        buyCanvas.gameObject.SetActive(true);
    }

    public void DisplayBuyNotification(bool success)
    {
        string notification = "Buy Successfully";
        if (!success)
        {
            notification = "You don't have enough coins";
        }
        buyNotificationText.SetText(notification);
        buyNotificationCanvas.gameObject.SetActive(true);
    }

    private string ConvertTime(int time)
    {
        int minutes = time / 60;
        int seconds = time % 60;
        string result = minutes + ":" + seconds;
        return result;
    }
}
