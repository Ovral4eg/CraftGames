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
        //устанавливаем кол-во клиентов на уровне
        customerLimit = settings.customersLimit;
        //кол-во блюд на уровне
        foodLimit = settings.foodLimit;
        //устанавливаем доступное для игры время
        levelTimer = settings.levelTimer;

        //ограничиваем кол-во еды в заказе клиента
        foodInOrderLimit = settings.foodInOrderLimit;
        //фиксированный список заказов
        useFixedOrderList = settings.useFixedOrderList;
        fixedOrderList = settings.fixedOrderList;
        //количество бустеров
        boosterCount = settings.boosterCount;
    }

    public Sprite FindIco(int id)
    {
        var food = availableFood.Find(x => x.GetFoodId()==id);

        return food.GetFoodIco();
    }

    public void StartLevel()
    {
        //применяем настройки
        ApplySettings(currentSettings);

        //вычисляем скорость движения клиента
        CalculateCustomerSpeed();

        //конфигурируем стол, исходя из кол-ва блюд на уровне
        //в доп задании написано 
        //"количество блюд на уровне"
        //но не указано, каким образом настроить стол

        //получаем список достпуной на столе еды
        availableFood = table.GetAvailableFood();

        //обнуляем таймер уровня
        nextTimerTick = 0;
        //устанавливаем таймер до появления след клиента
        nextSpawnPeriod = 0;
        //сбрасываем кол-во вышедших на сцену заказчиков
        enterCustomersCount = 0;
        //сбрасываем счетчик блюд из фиксированного заказа
        fixedOrderCount = 0;

        //обновляем в UI макс количество клиентов на этом уровне
        ui.UpdateCustomerLimit(customerLimit);

        //обновляем в UI кол-во бустеров
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

        //действия клиентов
        CustomersProcessing();

        //проверяем куда нажал игрок
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
                //если игрок нажал на используемый объект, используем его
                var useObject = hit.collider.GetComponent<IUse>();

                if (useObject != null)
                {                    
                    useObject.Use();
                }
            }
        }
    }

    /// <summary>
    /// использование бустера заказов
    /// </summary>
    public void UseBooster()
    {
        //проверяем наличие бустеров
        if (boosterCount > 0)
        {
            //перебираем список клиентов около стола, готовых к заказу
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
            //отображаем паузу и меню покупки бустеров
            PauseLevel();
        }

        //Debug.Log("use booster");
    }

    /// <summary>
    /// добавление бустеров заказа
    /// </summary>
    /// <param name="value"></param>
    internal void AddBooster(int value)
    {
        boosterCount += value;
        ui.UpdateBoostersCount(boosterCount);
    }    

    /// <summary>
    /// таймер уровня
    /// </summary>
    private float timerPeriod = 1;
    private float nextTimerTick = 0;
    private void UpdateTimer()
    {
        if (Time.time >= nextTimerTick)
        {
            nextTimerTick = Time.time + timerPeriod;
            levelTimer -= 1;

            //обновляем в ui оставшееся время
            ui.UpdateLevelTimer(TimerToText()); 

            //если время закончилось
            if(levelTimer==0)
            {
                //показываем экран поражения
                LoseLevel();
            }
        }        
    }
    private string TimerToText()
    {
        //определяем кол-во минут
        var min = levelTimer / 60;
        var minText = min.ToString();
        if (min < 10) minText = $"0{minText}";

        //определяем кол-во секунд
        var sec = levelTimer - min * 60;
        var secText = sec.ToString();
        if (sec < 10) secText = $"0{secText}";

        var result = $"{minText}:{secText}";

        return result;
    }

    [Header("customers")]
    //интервал плявления клиентов на сцене
    [SerializeField] private float customerSpawnPeriod = 1;
    //время до появления след клиента
    private float nextSpawnPeriod = 0;
    //клиенты около стола
    [SerializeField] private List<Customer> customersNearTable = new List<Customer>(); 
    public void RemoveTableCustomer(Customer customer)
    {
        customersNearTable.Remove(customer);
    }
   
    //кол-во клиентов, вышедших на сцену после старта уровня
    private int enterCustomersCount = 0;

    //все "живые" клиенты в данный момент, находящиеся на сцене
    [SerializeField] private List<Customer> liveCustomers = new List<Customer>();
    public void RemoveLiveCustomer(Customer customer)
    {
        liveCustomers.Remove(customer);
    }

    //логика появления клиентов на сцене
    private void CustomersProcessing()
    {
        //проверяем лимит посетителей для уровня
        if(enterCustomersCount == customerLimit)
        {
            return;
        }

        //проверяем пришло ли время для спавна след клиента
        if (Time.time < nextSpawnPeriod) return;

        //проверяем есть ли свободные места у стола заказов
        var freePoint = table.GetFreeOrderPoint();
        if (freePoint == null) return;

        //определяем время спавна след клиента
        nextSpawnPeriod = Time.time + customerSpawnPeriod;

        //создаем нового клиента
        var newCustomer = CustomerController.ins.GenerateNewCustomer();

        //запоминаем нового клиента у стола, чтобы проверять его заказ при выдаче еды
        customersNearTable.Add(newCustomer);

        //добавляем клиента в список всех клиентв на сцене
        liveCustomers.Add(newCustomer);

        //считаем сколько клиентов вышло
        enterCustomersCount++;

        //обновляем в UI инфо о клиентах
        UpdateCustomersCount();

        //добавляем клиента в "место у стола"
        freePoint.SetCustomer(newCustomer);

        //добавляем "место у стола" в клиента
        newCustomer.GetCustomerActionComponent().SetOrderPoint(freePoint);

        //создаем для клиента новый заказ
        GenerateNewOrder(newCustomer);

        //настраиваем и отправляем клиента к столу
        newCustomer.GetCustomerActionComponent().SetupAndGo(enterPoints, freePoint, exitPoints, customerSpeed);
    }

    //фиксированный список заказов
    [SerializeField] private bool useFixedOrderList;
   
    //счетчик блюд в фиксированном списке заказов
    private int fixedOrderCount;
    
    //вся доступная на столе еда
    [SerializeField] private List<Food> availableFood = new List<Food>();
    
    /// <summary>
    /// создаем новый заказ для клиента
    /// </summary>
    /// <param name="customer"></param>
    private void GenerateNewOrder(Customer customer)
    {
        //если используется фиксированный список заказов, то выдаем след заказ из списка
        if (useFixedOrderList && fixedOrderList.Count > 0)
        {
            //проверяем, что фиксированный список не закончился
            if (fixedOrderCount < fixedOrderList.Count)
            {
                //получаем заказ из списка
                var foodOrder = fixedOrderList[fixedOrderCount].order;
                //выдаем клиенту заказ
                customer.GetCustomerOrderComponent().SetFoodOrder(foodOrder);

                fixedOrderCount++;
            }
            else
            {
                //если в фиксированном списке закончились заказы

                //можно выдавать последний заказ из списка 
                //fixedOrderCount = fixedOrderList.Count - 1;

                //или случайный заказ
                GenerateRandomOrder(customer);
            }
        }
        else
        {
            //выдем случайный заказ
            GenerateRandomOrder(customer);
        }
    }

    private void GenerateRandomOrder(Customer customer)
    {
        //создаем новый список заказа из доступной еды
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
    /// ставим уровень на паузу
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
    /// заканчиваем игру и отображаем окно победы
    /// </summary>
    private void WinLevel()
    {
        GameController.ins.SetGameState(GameState.Pause);
        ui.ShowWin();
    }

    /// <summary>
    /// заканчиваем игру и отображаем окно поражения
    /// </summary>
    private void LoseLevel()
    {
        GameController.ins.SetGameState(GameState.Pause);
        ui.ShowLose();
    }

    /// <summary>
    /// перезапускаем уровень
    /// </summary>
    public void RestartLevel()
    {
        ResetLevel();

        StartLevel();
    }

    /// <summary>
    /// сбрасываем уровень в начальное состояние
    /// </summary>
    private void ResetLevel()
    {
        //прячем все окна
        ui.ResetWindows();

        //удаляем всех посетителей
        for (int i = liveCustomers.Count - 1; i >= 0; i--) 
        {
            Destroy(liveCustomers[i].gameObject);
        }

        //забываем всех клиентов у стола
        customersNearTable.Clear();

        //забываем всех клиентов на сцене
        liveCustomers.Clear();
    }
   
    /// <summary>
    /// обновляем в UI кол-во, вышедших к столу, клиентов
    /// </summary>
    private void UpdateCustomersCount()
    {
        ui.UpdateCustomersInfo(enterCustomersCount, customerLimit);       
    }

    /// <summary>
    /// использование еды на столе
    /// </summary>
    /// <param name="id"></param>
    public void UseFood(int id)
    {
        //Debug.Log($"use food {id}");
        //перебираем заказчиков у стола и проверяем кому нужна эта еда
        foreach(var c in customersNearTable)
        {
            //проверяем, что клиент у стола в состоянии заказа
            if(c.GetCustomerState()== CustomerState.Order)
            {
                var foodTransfered = c.GetCustomerOrderComponent().CheckFoodInOrder(id);

                //если кто-то забрал еду, прерываем цикл
                if (foodTransfered) break;
            }           
        }
    }

    public void CheckWin()
    {
        //если из ожидания вышли все клиенты
        if (enterCustomersCount == customerLimit)
        {
            //и если около стола никого не осталось
            if (customersNearTable.Count == 0)
            {
                //победа
                WinLevel();
            }
        }
    }

   
}
