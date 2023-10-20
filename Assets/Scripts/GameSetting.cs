using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class GameSetting : EditorWindow
{
    public VisualTreeAsset uxml;
    public VisualTreeAsset templateSprite;

    private VisualElement root;
    private LevelScriptableObject[] SOs;

    [MenuItem("Window/Game Toolkit/GameSetting")]
    public static void ShowExample()
    {
        GameSetting wnd = GetWindow<GameSetting>();
        wnd.titleContent = new GUIContent("GameSetting");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        root = rootVisualElement;
        uxml.CloneTree(root);

        Toolbar functionalToolbar = root.Q<Toolbar> ("MapToolbar");
        
        //Get LevelScriptableObject
        string[] GUIDs = AssetDatabase.FindAssets("t:ScriptableObject", new string[] { "Assets/Scripts/LevelScript" });
        SOs = new LevelScriptableObject[GUIDs.Length];

        for (int i = 0; i < GUIDs.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(GUIDs[i]);
            SOs[i] = (LevelScriptableObject)AssetDatabase.LoadAssetAtPath(path, typeof(LevelScriptableObject));
        }

        //Display LevelScriptableObject
        for (int i = 0; i < SOs.Length; i++)
        {
            //Add ToolbarButton
            ToolbarButton toolbarButton = new ToolbarButton();
            toolbarButton.text = SOs[i].name;
            int t = i;
            toolbarButton.clicked += (delegate { DisplayData(t); });
            functionalToolbar.Add(toolbarButton);
        }

        DisplayData(0);
    }

    private void DisplayData(int k)
    {
        TextField mapName = root.Q<TextField>("MapName");
        IntegerField mapTime = root.Q<IntegerField>("MapTime");
        Toggle showSpriteToggle = root.Q<Toggle>("ShowSpriteToggle");
        PropertyField tilesData = root.Q<PropertyField>();

        SerializedObject serializedObject = new SerializedObject(SOs[k]);
        mapName.BindProperty(serializedObject);
        mapName.SetEnabled(false);
        mapTime.BindProperty(serializedObject);
        tilesData.BindProperty(serializedObject);

        showSpriteToggle.value = true;
        showSpriteToggle.RegisterCallback<ChangeEvent<bool>>((evt) =>
        {
            tilesData.visible = evt.newValue;
        });
    }
}