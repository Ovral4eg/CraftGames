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

        //ставим игру на паузу
        SetGameState(GameState.Pause);

        //прячем все окна, если они остались открытыми в редакторе
        ui.ResetWindows();

        //отображаем стартовый экран
        ui.ShowStart();     

        //инициализация генератора клиентов
        customerController.Setup();

        //инициализация уровня
        levelController.Setup();
    }
   
    /// <summary>
    /// старт грового уровня
    /// </summary>
    public void ButtonStart()
    {
        //загружаем настройки уровня
        //var settings = settingsController.LoadSettings();

        //применяем настройки уровня
        levelController.SetSettings(settingsController.GetCurrentSettings());

        //запускаем уровень
        levelController.StartLevel();

        //скрываем стартовый экран
        ui.HideStart();
    }

    /// <summary>
    /// состояние игры
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
