using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SettingsController : MonoBehaviour
{
    /// <summary>
    /// ������� ���� ��������
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
    /// �������� ������� ���������
    /// </summary>
    /// <returns></returns>
    public Settings GetCurrentSettings()
    {
        return currentSettings;
    }

    /// <summary>
    /// ��������� ��������� �� �����
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
    /// ��������� ������� ��������� � ����
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
    [Tooltip("���-�� ����������� �� ������")]
    public int customersLimit=10;

    [Tooltip("���-�� ���� �� ������")]
    public int foodLimit=4;

    [Tooltip("������ ������")]
    public int levelTimer=30;

    //bonus

    [Tooltip("���� ���-�� ���� � ������")]
    public int foodInOrderLimit=3;

    [Tooltip("������������ ������������� ������ �������")]
    public bool useFixedOrderList=false;
    [Tooltip("������������� ������ �������")]
    public List<FoodOrder> fixedOrderList = new List<FoodOrder>();

    [Tooltip("���������� ��������")]
    public int boosterCount=0;
}
