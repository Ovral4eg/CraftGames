using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController ins;

    [SerializeField] private SettingsController settingsController;
    [SerializeField] private LevelController levelController;
    [SerializeField] private CustomerController customerController;

    [SerializeField] private UIController ui;

    void Start()
    {
        ins = this;

        //������ ���� �� �����
        SetGameState(GameState.Pause);

        //������ ��� ����, ���� ��� �������� ��������� � ���������
        ui.ResetWindows();

        //���������� ��������� �����
        ui.ShowStart();     

        //������������� ���������� ��������
        customerController.Setup();

        //������������� ������
        levelController.Setup();
    }
   
    /// <summary>
    /// ����� ������� ������
    /// </summary>
    public void ButtonStart()
    {
        //��������� ��������� ������
        //var settings = settingsController.LoadSettings();

        //��������� ��������� ������
        levelController.SetSettings(settingsController.GetCurrentSettings());

        //��������� �������
        levelController.StartLevel();

        //�������� ��������� �����
        ui.HideStart();
    }

    /// <summary>
    /// ��������� ����
    /// </summary>
    private GameState gameState;

    public void SetGameState(GameState gameState)
    {
        this.gameState = gameState;
    }

    public GameState GetGamestate()
    {
        return gameState;
    }

    public bool GameIsPaused()
    {
        return gameState == GameState.Pause;       
    }   
}
