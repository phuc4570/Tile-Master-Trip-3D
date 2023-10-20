using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelScriptableObject : ScriptableObject
{
    [Serializable]
    public class TileData
    {
        [SerializeField]
        public Sprite tileSprite;
        [SerializeField]
        public int number;
    }

    public TileData[] tileDatas;
    public int time;
}
