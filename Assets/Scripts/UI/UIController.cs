using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject startScreen;
    public void ShowStart()
    {
        startScreen.SetActive(true);
    }
    public void HideStart()
    {
        startScreen.SetActive(false);
    }

    [SerializeField] private GameObject pauseScreen;
    public void ShowPause()
    {
        pauseScreen.SetActive(true);
    }
    public void HidePause()
    {
        pauseScreen.SetActive(false);
    }   

    [SerializeField] private GameObject winScreen;
    public void ShowWin()
    {
        winScreen.SetActive(true);
    }
    public void HideWin()
    {
        winScreen.SetActive(false);
    }

    [SerializeField] private GameObject loseScreen;
    public void ShowLose()
    {
        loseScreen.SetActive(true);
    }
    public void HideLose()
    {
        loseScreen.SetActive(false);
    }

    internal void ResetWindows()
    {
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
        pauseScreen.SetActive(false);
    }

    [SerializeField] private TextMeshProUGUI customerCountText;
    [SerializeField] private Image customerProgressBar;

    public void UpdateCustomersInfo(int enterCustomers, int limitCustomers)
    {
        customerCountText.text = $" {enterCustomers} / {limitCustomers}";
        customerProgressBar.fillAmount = (float)enterCustomers / limitCustomers;
    }

    [SerializeField] private TextMeshProUGUI customerLimitText;
    public void UpdateCustomerLimit(int customerLimit)
    {
        customerLimitText.text = $"{customerLimit}";
    }

    [SerializeField] private TextMeshProUGUI boosterCountText;
    public void UpdateBoostersCount(int boosterCount)
    {
        boosterCountText.text = $"{boosterCount}";
    }

    [SerializeField] private TextMeshProUGUI timerText;
    internal void UpdateLevelTimer(string time)
    {
        timerText.text = time;
    }
}
