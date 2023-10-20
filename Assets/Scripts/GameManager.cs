using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform[] listPos;
    [SerializeField] UIManager uiManager;
    [SerializeField] AudioClip loseSFX;
    [SerializeField] AudioClip winSFX;
    [SerializeField] AudioClip getPointSFX;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] LevelScriptableObject[] levelDatas;
    
    private AudioSource audioSource;

    private List<TileController> listTiles;

    public int level { get; private set; }
    public int time { get; private set; }
    public int score { get; private set; }
    public int heart { get; private set; }
    public int totalScore { get; private set; }
    public int coin { get; private set; }
    public int maxSlot { get; private set; }
    public int slotPrice { get; private set; } = 500;
    public int returnTile { get; private set; }
    public int returnTilePrice { get; private set; } = 700;
    public int findTile { get; private set; }
    public int findTilePrice { get; private set; } = 1000;

    private int scoreToIncrease = 10;
    private float timeToWait = 0.5f;
    private float xMinRange = -2f;
    private float xMaxRange = 2f;
    private float yMinRange = 3f;
    private float yMaxRange = 4f;
    private float zMinRange = -2.5f;
    private float zMaxRange = 3.5f;
    private int typeThingToBuy;

    private bool isPlaying;
    private bool gameOver;

    public bool isPausing { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        InitGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            if (gameOver)
            {
                GameOver();
            }

            CheckWin();
        }
    }

    public void AddSlot(TileController tile)
    {
        int addIndex = -1;

        for(int i = 0; i < listTiles.Count; i++)
        {
            if (listTiles[i].tileSprite == tile.tileSprite)
            {
                listTiles.Add(tile);
                MoveTile(i);
                addIndex = i;
                break;
            }
        }

        if (addIndex < 0)
        {
            addIndex = listTiles.Count;
            listTiles.Add(tile);
        }

        tile.MoveToPosition(listPos[addIndex], true);

        StartCoroutine(CheckSet());
    }

    private void MoveTile(int i)
    {
        for(int j = i; j < listTiles.Count - 1; j++)
        {
            listTiles[j].MoveToPosition(listPos[j + 1], true);
        }

        for (int j = i; j < listTiles.Count; j++)
        {
            (listTiles[j], listTiles[listTiles.Count - 1]) = (listTiles[listTiles.Count - 1], listTiles[j]);
        }
    }

    private IEnumerator CheckSet()
    {
        yield return new WaitForSeconds(timeToWait);

        bool isChecked = false;
        for (int i = 0; i < listTiles.Count - 2; i++) 
        {
            if (listTiles[i].tileSprite == listTiles[i+1].tileSprite && listTiles[i].tileSprite == listTiles[i + 2].tileSprite)
            {
                listTiles[i].Destroy();
                listTiles[i + 1].Destroy();
                listTiles[i + 2].Destroy();
                listTiles.RemoveRange(i,3);
                isChecked = true;
                break;
            }
        }
        if (isChecked)
        {
            IncreaseScore(scoreToIncrease);
            audioSource.PlayOneShot(getPointSFX);
            for (int i = 0; i < listTiles.Count; i++)
            {
                listTiles[i].MoveToPosition(listPos[i], true);
            }
        }
        else
        {
            if (listTiles.Count == maxSlot)
            {
                gameOver = true;
            }
        }
    }

    private void InitGame()
    {
        level = PlayerPrefs.GetInt("level");
        heart = PlayerPrefs.GetInt("heart");
        totalScore = PlayerPrefs.GetInt("totalScore");
        coin = PlayerPrefs.GetInt("coin");
        returnTile = PlayerPrefs.GetInt("returntile");
        findTile = PlayerPrefs.GetInt("findtile");

        if(level == 0)
        {
            level = 1;
            heart = 9999;
            totalScore = 0;
            coin = 2000;
        }

        uiManager.DisplayHomeLayout();
        uiManager.DisplayHomeInfo();

        audioSource = GameObject.Find("Game_SFX").GetComponent<AudioSource>();
    }

    public void LoadLevel()
    {
        maxSlot = 7;
        time = levelDatas[level - 1].time;
        score = 0;
        heart--;
        listTiles = new List<TileController>();

        ClearLevel();
        SpawnTiles();
        ResumeGame();
        uiManager.DisplayLevelInfo();
        StartCoroutine(DecreaseSecond());
    }

    private void ClearLevel()
    {
        GameObject[] tilesList = GameObject.FindGameObjectsWithTag("Tile");
        if(tilesList.Length > 0)
        {
            foreach (GameObject tile in tilesList)
            {
                Destroy(tile);
            }
        }
    }

    IEnumerator DecreaseSecond()
    {
        while (isPlaying)
        {
            yield return new WaitForSeconds(1);
            time--;
            if (time == 0)
            {
                gameOver = true;
            }
            uiManager.DisplayTime(time);
        }
    }

    private UnityEngine.Vector3 RandomValidPos()
    {
        float x = UnityEngine.Random.Range(xMinRange, xMaxRange);
        float y = UnityEngine.Random.Range(yMinRange, yMaxRange);
        float z = UnityEngine.Random.Range(zMinRange, zMaxRange);
        return new UnityEngine.Vector3(x, y, z);
    }

    public void SpawnTiles()
    {
        for(int i = 0; i < levelDatas[level - 1].tileDatas.Length; i++)
        {
            Sprite sprite = levelDatas[level - 1].tileDatas[i].tileSprite;
            int number = levelDatas[level - 1].tileDatas[i].number;
            for (int j = 0; j < number * 3; j++)
            {
                UnityEngine.Vector3 pos = RandomValidPos();
                GameObject tile = Instantiate(tilePrefab, pos, UnityEngine.Quaternion.identity);
                TileController tileController = tile.GetComponent<TileController>();
                tileController.tileSprite = sprite;
            }
        }
    }

    private void CheckWin()
    {
        if (!gameOver)
        {
            GameObject[] tilesList = GameObject.FindGameObjectsWithTag("Tile");
            if (tilesList.Length == 0)
            {
                level++;
                if(level > 3)
                {
                    level = 1;
                }
                totalScore += score;

                audioSource.PlayOneShot(winSFX);
                uiManager.DisplayWin(score);

                StopAllCoroutines();
                PauseGame();
            }
        }
    }

    private void GameOver()
    {
        PauseGame();
        string info;
        if(time == 0)
        {
            info = "TIME OUT";
        }
        else
        {
            info = "OUT OF SLOTS";
        }

        audioSource.PlayOneShot(loseSFX);

        uiManager.DisplayLose(info);
        gameOver = false;
    }

    public void ReturnHome()
    {
        PauseGame();
        ClearLevel();
        uiManager.DisplayHomeInfo();
    }

    public void PauseGame()
    {
        isPlaying = false;
        isPausing = true;
        //Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        isPlaying = true;
        isPausing = false;
        //Time.timeScale = 1;
    }

    public void BuySomeThing(int type)
    {
        typeThingToBuy = type;
        int price = 0;
        string thingToBuy = "";
        switch (type)
        {
            case 0:
                {
                    thingToBuy = "Slot";
                    price = slotPrice;
                    break;
                }
            case 1:
                {
                    thingToBuy = "Return Tile Help";
                    price = returnTilePrice;
                    break;
                };
            case 2:
                {
                    thingToBuy = "Find Tile Help";
                    price = findTilePrice;
                    break;
                }
        }
        uiManager.DisplayBuyInfo(thingToBuy, price);
        PauseGame();
    }

    public void Buy()
    {
        bool buySuccess = false;
        switch (typeThingToBuy)
        {
            case 0:
                {
                    if(coin >= slotPrice)
                    {
                        coin -= slotPrice;
                        buySuccess = true;
                        maxSlot++;
                    }
                    break;
                }
            case 1:
                {
                    if (coin >= returnTilePrice)
                    {
                        coin -= returnTilePrice;
                        buySuccess = true;
                        returnTile++;
                    }
                    break;
                };
            case 2:
                {
                    if (coin >= findTilePrice)
                    {
                        coin -= findTilePrice;
                        buySuccess = true;
                        findTile++;
                    }
                    break;
                }
        }
        uiManager.DisplayLevelInfo();
        uiManager.DisplayBuyNotification(buySuccess);
        PauseGame();
    }

    public void ReturnTileHelp()
    {
        if(returnTile > 0 && listTiles.Count > 0)
        {
            returnTile--;
            uiManager.DisplayLevelInfo();
            listTiles[listTiles.Count - 1].ReturnTile();
            listTiles.RemoveAt(listTiles.Count - 1);
        }
    }

    public void FindTileHelp()
    {
        if(findTile > 0)
        {
            Sprite tileSpriteToFind = null;
            int numberTileToFind = 0;

            if(listTiles.Count == 0)
            {
                GameObject tile = GameObject.FindGameObjectWithTag("Tile");
                tileSpriteToFind = tile.GetComponent<TileController>().tileSprite;
                numberTileToFind = 3;
            }

            for(int i = 0; i < listTiles.Count; i++)
            {
                for(int j = i; j < listTiles.Count; j++)
                {
                    if (listTiles[j].tileSprite == listTiles[i].tileSprite)
                    {
                        numberTileToFind++;
                    }
                    else
                    {
                        break;
                    }
                }
                numberTileToFind = 3 - numberTileToFind;
                if(numberTileToFind + listTiles.Count <= maxSlot)
                {
                    tileSpriteToFind = listTiles[i].tileSprite;
                    break;
                }
                numberTileToFind = 0;
            }

            if (tileSpriteToFind != null && numberTileToFind != 0)
            {
                GameObject[] validTileList = GameObject.FindGameObjectsWithTag("Tile");
                foreach(GameObject tile in validTileList)
                {
                    Sprite tileSprite = tile.GetComponent<TileController>().tileSprite;
                    if (tileSprite == tileSpriteToFind && tile.transform.localScale == UnityEngine.Vector3.one)
                    {
                        AddSlot(tile.GetComponent<TileController>());
                        numberTileToFind--;
                        if(numberTileToFind == 0)
                        {
                            findTile--;
                            uiManager.DisplayLevelInfo();
                            break;
                        }
                    }
                }
            }
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("level", level);
        PlayerPrefs.SetInt("heart", heart);
        PlayerPrefs.SetInt("totalScore", totalScore);
        PlayerPrefs.SetInt("coin", coin);
        PlayerPrefs.SetInt("returntile", returnTile);
        PlayerPrefs.SetInt("findtile", findTile);
    }

    private void IncreaseScore(int point)
    {
        score += point;
        uiManager.DisplayScore(score);
    }
}
