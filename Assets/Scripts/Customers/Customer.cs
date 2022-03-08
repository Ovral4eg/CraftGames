using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CustomerOrder))]
[RequireComponent(typeof(CustomerAction))]
public class Customer : MonoBehaviour
{
    /// <summary>
    /// состояние клиента
    /// </summary>
    private CustomerState customerState;
    public void SetCustomerState(CustomerState customerState)
    {
        this.customerState = customerState;
    }
    public CustomerState GetCustomerState()
    {
        return customerState;
    }

    /// <summary>
    /// установка и настройка компонента заказа 
    /// </summary>   
    private CustomerOrder customerOrder;
    public void SetCustomerOrder()
    {
        //получаем компонент управления заказом
        customerOrder = GetComponent<CustomerOrder>();
        customerOrder.SetCustomer(this);
        //подписываемся на ивент выполнения заказа
        customerOrder.onOrderComplete += OrderComponent_onOrderComplete;
    }   

    public CustomerOrder GetCustomerOrderComponent()
    {
        return customerOrder;
    }

    /// <summary>
    /// небольшая визуальная кастомизация клиента
    /// </summary>
    [SerializeField] private Transform headSlot;

    public void AddHead(GameObject head)
    {
        head.transform.SetParent(headSlot);

        head.transform.localPosition = Vector3.zero;
        head.transform.rotation = headSlot.rotation;
    }

    /// <summary>
    /// действия и перемещения клиента на сцене
    /// </summary>
    private CustomerAction customerAction; 
    
    public void SetCustomerAction()
    {
        //получаем компонент движения и действия клиента
        customerAction= GetComponent<CustomerAction>();
        customerAction.SetCustomer(this);

        //подписываемся на ивент, когда клиент готов сделать заказ
        customerAction.onReadyToOrder += CustomerAction_onReadyToOrder;

        //подписываемся на ивент, когда клиент покидает кухню
        customerAction.onReadyToDestroy += CustomerAction_onReadyToDestroy;
    }

    

    public CustomerAction GetCustomerActionComponent()
    {
        return customerAction;
    }


    private void Update()
    {
        customerAction.Action();

        customerAction.Move();      
    }

    /// <summary>
    /// обработка ивента готовности к заказу
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CustomerAction_onReadyToOrder(object sender, EventArgs e)
    {
        customerOrder.ShowOrder();
    }

    /// <summary>
    /// обработка ивента выполнения заказа
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OrderComponent_onOrderComplete(object sender, EventArgs e)
    {
        //отправляем клиента на выход
        SetCustomerState(CustomerState.Exit);
        customerAction.GoToExit();
        //освобождаем место у стола
        customerAction.LeaveTable();
        //скрываем UI заказа
        customerOrder.HideOrder();
        //забываем клиента в очереди заказов
        LevelController.ins.RemoveTableCustomer(this);

        LevelController.ins.CheckWin();
    }   

    /// <summary>
    /// обработка ивена выхода их ресторана/кухни
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CustomerAction_onReadyToDestroy(object sender, EventArgs e)
    {
        LevelController.ins.RemoveLiveCustomer(this);

        Destroy(gameObject);
    }
}
