using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SettingsController : MonoBehaviour
{
    /// <summary>
    /// открыть фаил настроек
    /// </summary>
    public void OpenFile()
    {
        var saveFolder = $"{Application.dataPath}/Settings/";

        string path = EditorUtility.OpenFilePanel("Import settings file", saveFolder, "json");

        if (path.Length != 0)
        {
            //Debug.Log($" open settings file {path}");
            LoadSettings(path);
        }
    }

    public Settings currentSettings;

    /// <summary>
    /// получить текущие настройки
    /// </summary>
    /// <returns></returns>
    public Settings GetCurrentSettings()
    {
        return currentSettings;
    }

    /// <summary>
    /// загрузить настройки из файла
    /// </summary>
    /// <param name="filePath"></param>
    public void LoadSettings(string filePath)
    {
        var jsonData = "";

        if (File.Exists(filePath))
        {
            jsonData = File.ReadAllText(filePath);

            currentSettings = new Settings();

            currentSettings = JsonUtility.FromJson<Settings>(jsonData);
        }       
    }

    /// <summary>
    /// сохранить текущие настройки в фаил
    /// </summary>
    public void SaveFile()
    {
        var saveFolder = $"{Application.dataPath}/Settings/";

        var path = EditorUtility.SaveFilePanel("Export settings file", saveFolder, name + ".json", "json");

        if (path.Length != 0)
        {
            SaveSettings(path);
        }
    }

    public void SaveSettings(string filePath)
    {
        var jsonData = JsonUtility.ToJson(currentSettings);

        File.WriteAllText(filePath, jsonData);
    } 
}

[Serializable]
public class Settings
{
    [Tooltip("кол-во посетителей на уровне")]
    public int customersLimit=10;

    [Tooltip("кол-во блюд на уровне")]
    public int foodLimit=4;

    [Tooltip("таймер уровня")]
    public int levelTimer=30;

    //bonus

    [Tooltip("макс кол-во блюд в заказе")]
    public int foodInOrderLimit=3;

    [Tooltip("использовать фиксированный список заказов")]
    public bool useFixedOrderList=false;
    [Tooltip("фиксированный список заказов")]
    public List<FoodOrder> fixedOrderList = new List<FoodOrder>();

    [Tooltip("количество бустеров")]
    public int boosterCount=0;
}
