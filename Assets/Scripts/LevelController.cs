using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LevelController : MonoBehaviour
{
    public static LevelController ins;
    internal void Setup()
    {
        ins = this;
    }

    [Header("UI")]
    [SerializeField] private UIController ui;

    [Header("Walk settings")]
    [SerializeField] private float customerMoveTime;
    private float customerSpeed;
    [SerializeField] private List<Transform> enterPoints = new List<Transform>();
    [SerializeField] private List<Transform> exitPoints = new List<Transform>();

    [Header("Table")]
    [SerializeField] private TableController table;   

    [Header("level settings")]
    private int customerLimit;
    private int foodLimit;
    private int levelTimer;

    //bonus
    private int foodInOrderLimit;
    private List<FoodOrder> fixedOrderList = new List<FoodOrder>();
    private int boosterCount;

    private Settings currentSettings;

    public void SetSettings(Settings settings)
    {
        currentSettings = settings;
    }

    internal void ApplySettings(Settings settings)
    {
        //������������� ���-�� �������� �� ������
        customerLimit = settings.customersLimit;
        //���-�� ���� �� ������
        foodLimit = settings.foodLimit;
        //������������� ��������� ��� ���� �����
        levelTimer = settings.levelTimer;

        //������������ ���-�� ��� � ������ �������
        foodInOrderLimit = settings.foodInOrderLimit;
        //������������� ������ �������
        useFixedOrderList = settings.useFixedOrderList;
        fixedOrderList = settings.fixedOrderList;
        //���������� ��������
        boosterCount = settings.boosterCount;
    }

    public Sprite FindIco(int id)
    {
        var food = availableFood.Find(x => x.GetFoodId()==id);

        return food.GetFoodIco();
    }

    public void StartLevel()
    {
        //��������� ���������
        ApplySettings(currentSettings);

        //��������� �������� �������� �������
        CalculateCustomerSpeed();

        //������������� ����, ������ �� ���-�� ���� �� ������
        //� ��� ������� �������� 
        //"���������� ���� �� ������"
        //�� �� �������, ����� ������� ��������� ����

        //�������� ������ ��������� �� ����� ���
        availableFood = table.GetAvailableFood();

        //�������� ������ ������
        nextTimerTick = 0;
        //������������� ������ �� ��������� ���� �������
        nextSpawnPeriod = 0;
        //���������� ���-�� �������� �� ����� ����������
        enterCustomersCount = 0;
        //���������� ������� ���� �� �������������� ������
        fixedOrderCount = 0;

        //��������� � UI ���� ���������� �������� �� ���� ������
        ui.UpdateCustomerLimit(customerLimit);

        //��������� � UI ���-�� ��������
        ui.UpdateBoostersCount(boosterCount);

        GameController.ins.SetGameState(GameState.Game);
    }

    private void CalculateCustomerSpeed()
    {
        var distance = 0f;
        var nextPoint = enterPoints[0];
        for (int i = 1; i < enterPoints.Count; i++)
        {
            distance += Vector3.Distance(nextPoint.position, enterPoints[i].position);
            nextPoint = enterPoints[i];
        }
        distance += Vector3.Distance(nextPoint.position, table.GetFirstOrderPoint().transform.position);
        customerSpeed = distance / customerMoveTime;
    }

    private void Update()
    {
        if (GameController.ins.GameIsPaused()) return;

        UpdateTimer();

        //�������� ��������
        CustomersProcessing();

        //��������� ���� ����� �����
        CheckPlayerTap();
    }

    [SerializeField] private LayerMask tapLayer;
    private void CheckPlayerTap()
    {
        if (Input.GetMouseButtonDown(0))
        {            
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, 100, tapLayer))
            {
                //���� ����� ����� �� ������������ ������, ���������� ���
                var useObject = hit.collider.GetComponent<IUse>();

                if (useObject != null)
                {                    
                    useObject.Use();
                }
            }
        }
    }

    /// <summary>
    /// ������������� ������� �������
    /// </summary>
    public void UseBooster()
    {
        //��������� ������� ��������
        if (boosterCount > 0)
        {
            //���������� ������ �������� ����� �����, ������� � ������
            foreach(var c in customersNearTable)
            {
                if(c.GetCustomerState()== CustomerState.Order)
                {
                    c.GetCustomerOrderComponent().BoostOrder();
                    boosterCount--;
                    ui.UpdateBoostersCount(boosterCount);
                    break;
                }
            }          
        }
        else
        {
            //���������� ����� � ���� ������� ��������
            PauseLevel();
        }

        //Debug.Log("use booster");
    }

    /// <summary>
    /// ���������� �������� ������
    /// </summary>
    /// <param name="value"></param>
    internal void AddBooster(int value)
    {
        boosterCount += value;
        ui.UpdateBoostersCount(boosterCount);
    }    

    /// <summary>
    /// ������ ������
    /// </summary>
    private float timerPeriod = 1;
    private float nextTimerTick = 0;
    private void UpdateTimer()
    {
        if (Time.time >= nextTimerTick)
        {
            nextTimerTick = Time.time + timerPeriod;
            levelTimer -= 1;

            //��������� � ui ���������� �����
            ui.UpdateLevelTimer(TimerToText()); 

            //���� ����� �����������
            if(levelTimer==0)
            {
                //���������� ����� ���������
                LoseLevel();
            }
        }        
    }
    private string TimerToText()
    {
        //���������� ���-�� �����
        var min = levelTimer / 60;
        var minText = min.ToString();
        if (min < 10) minText = $"0{minText}";

        //���������� ���-�� ������
        var sec = levelTimer - min * 60;
        var secText = sec.ToString();
        if (sec < 10) secText = $"0{secText}";

        var result = $"{minText}:{secText}";

        return result;
    }

    [Header("customers")]
    //�������� ��������� �������� �� �����
    [SerializeField] private float customerSpawnPeriod = 1;
    //����� �� ��������� ���� �������
    private float nextSpawnPeriod = 0;
    //������� ����� �����
    [SerializeField] private List<Customer> customersNearTable = new List<Customer>(); 
    public void RemoveTableCustomer(Customer customer)
    {
        customersNearTable.Remove(customer);
    }
   
    //���-�� ��������, �������� �� ����� ����� ������ ������
    private int enterCustomersCount = 0;

    //��� "�����" ������� � ������ ������, ����������� �� �����
    [SerializeField] private List<Customer> liveCustomers = new List<Customer>();
    public void RemoveLiveCustomer(Customer customer)
    {
        liveCustomers.Remove(customer);
    }

    //������ ��������� �������� �� �����
    private void CustomersProcessing()
    {
        //��������� ����� ����������� ��� ������
        if(enterCustomersCount == customerLimit)
        {
            return;
        }

        //��������� ������ �� ����� ��� ������ ���� �������
        if (Time.time < nextSpawnPeriod) return;

        //��������� ���� �� ��������� ����� � ����� �������
        var freePoint = table.GetFreeOrderPoint();
        if (freePoint == null) return;

        //���������� ����� ������ ���� �������
        nextSpawnPeriod = Time.time + customerSpawnPeriod;

        //������� ������ �������
        var newCustomer = CustomerController.ins.GenerateNewCustomer();

        //���������� ������ ������� � �����, ����� ��������� ��� ����� ��� ������ ���
        customersNearTable.Add(newCustomer);

        //��������� ������� � ������ ���� ������� �� �����
        liveCustomers.Add(newCustomer);

        //������� ������� �������� �����
        enterCustomersCount++;

        //��������� � UI ���� � ��������
        UpdateCustomersCount();

        //��������� ������� � "����� � �����"
        freePoint.SetCustomer(newCustomer);

        //��������� "����� � �����" � �������
        newCustomer.GetCustomerActionComponent().SetOrderPoint(freePoint);

        //������� ��� ������� ����� �����
        GenerateNewOrder(newCustomer);

        //����������� � ���������� ������� � �����
        newCustomer.GetCustomerActionComponent().SetupAndGo(enterPoints, freePoint, exitPoints, customerSpeed);
    }

    //������������� ������ �������
    [SerializeField] private bool useFixedOrderList;
   
    //������� ���� � ������������� ������ �������
    private int fixedOrderCount;
    
    //��� ��������� �� ����� ���
    [SerializeField] private List<Food> availableFood = new List<Food>();
    
    /// <summary>
    /// ������� ����� ����� ��� �������
    /// </summary>
    /// <param name="customer"></param>
    private void GenerateNewOrder(Customer customer)
    {
        //���� ������������ ������������� ������ �������, �� ������ ���� ����� �� ������
        if (useFixedOrderList && fixedOrderList.Count > 0)
        {
            //���������, ��� ������������� ������ �� ����������
            if (fixedOrderCount < fixedOrderList.Count)
            {
                //�������� ����� �� ������
                var foodOrder = fixedOrderList[fixedOrderCount].order;
                //������ ������� �����
                customer.GetCustomerOrderComponent().SetFoodOrder(foodOrder);

                fixedOrderCount++;
            }
            else
            {
                //���� � ������������� ������ ����������� ������

                //����� �������� ��������� ����� �� ������ 
                //fixedOrderCount = fixedOrderList.Count - 1;

                //��� ��������� �����
                GenerateRandomOrder(customer);
            }
        }
        else
        {
            //����� ��������� �����
            GenerateRandomOrder(customer);
        }
    }

    private void GenerateRandomOrder(Customer customer)
    {
        //������� ����� ������ ������ �� ��������� ���
        var foodOrder = new List<Food>();

        var foodCount = Random.Range(1, foodInOrderLimit + 1);

        for (int i = 1; i <= foodCount; i++)
        {
            var randomFoodIndex = Random.Range(0, availableFood.Count);

            var randomFood = availableFood[randomFoodIndex];

            var newOrderFood = new Food(randomFood);

            foodOrder.Add(newOrderFood);
        }

        customer.GetCustomerOrderComponent().SetFoodOrder(foodOrder);
    }

    
    /// <summary>
    /// ������ ������� �� �����
    /// </summary>
    private void PauseLevel()
    {
        GameController.ins.SetGameState(GameState.Pause);
        ui.ShowPause();
    }

    public void ContinueLevel()
    {
        GameController.ins.SetGameState(GameState.Game);
        ui.HidePause();
    }
   
    /// <summary>
    /// ����������� ���� � ���������� ���� ������
    /// </summary>
    private void WinLevel()
    {
        GameController.ins.SetGameState(GameState.Pause);
        ui.ShowWin();
    }

    /// <summary>
    /// ����������� ���� � ���������� ���� ���������
    /// </summary>
    private void LoseLevel()
    {
        GameController.ins.SetGameState(GameState.Pause);
        ui.ShowLose();
    }

    /// <summary>
    /// ������������� �������
    /// </summary>
    public void RestartLevel()
    {
        ResetLevel();

        StartLevel();
    }

    /// <summary>
    /// ���������� ������� � ��������� ���������
    /// </summary>
    private void ResetLevel()
    {
        //������ ��� ����
        ui.ResetWindows();

        //������� ���� �����������
        for (int i = liveCustomers.Count - 1; i >= 0; i--) 
        {
            Destroy(liveCustomers[i].gameObject);
        }

        //�������� ���� �������� � �����
        customersNearTable.Clear();

        //�������� ���� �������� �� �����
        liveCustomers.Clear();
    }
   
    /// <summary>
    /// ��������� � UI ���-��, �������� � �����, ��������
    /// </summary>
    private void UpdateCustomersCount()
    {
        ui.UpdateCustomersInfo(enterCustomersCount, customerLimit);       
    }

    /// <summary>
    /// ������������� ��� �� �����
    /// </summary>
    /// <param name="id"></param>
    public void UseFood(int id)
    {
        //Debug.Log($"use food {id}");
        //���������� ���������� � ����� � ��������� ���� ����� ��� ���
        foreach(var c in customersNearTable)
        {
            //���������, ��� ������ � ����� � ��������� ������
            if(c.GetCustomerState()== CustomerState.Order)
            {
                var foodTransfered = c.GetCustomerOrderComponent().CheckFoodInOrder(id);

                //���� ���-�� ������ ���, ��������� ����
                if (foodTransfered) break;
            }           
        }
    }

    public void CheckWin()
    {
        //���� �� �������� ����� ��� �������
        if (enterCustomersCount == customerLimit)
        {
            //� ���� ����� ����� ������ �� ��������
            if (customersNearTable.Count == 0)
            {
                //������
                WinLevel();
            }
        }
    }

   
}
