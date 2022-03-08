using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CustomerOrder))]
[RequireComponent(typeof(CustomerAction))]
public class Customer : MonoBehaviour
{
    /// <summary>
    /// ��������� �������
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
    /// ��������� � ��������� ���������� ������ 
    /// </summary>   
    private CustomerOrder customerOrder;
    public void SetCustomerOrder()
    {
        //�������� ��������� ���������� �������
        customerOrder = GetComponent<CustomerOrder>();
        customerOrder.SetCustomer(this);
        //������������� �� ����� ���������� ������
        customerOrder.onOrderComplete += OrderComponent_onOrderComplete;
    }   

    public CustomerOrder GetCustomerOrderComponent()
    {
        return customerOrder;
    }

    /// <summary>
    /// ��������� ���������� ������������ �������
    /// </summary>
    [SerializeField] private Transform headSlot;

    public void AddHead(GameObject head)
    {
        head.transform.SetParent(headSlot);

        head.transform.localPosition = Vector3.zero;
        head.transform.rotation = headSlot.rotation;
    }

    /// <summary>
    /// �������� � ����������� ������� �� �����
    /// </summary>
    private CustomerAction customerAction; 
    
    public void SetCustomerAction()
    {
        //�������� ��������� �������� � �������� �������
        customerAction= GetComponent<CustomerAction>();
        customerAction.SetCustomer(this);

        //������������� �� �����, ����� ������ ����� ������� �����
        customerAction.onReadyToOrder += CustomerAction_onReadyToOrder;

        //������������� �� �����, ����� ������ �������� �����
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
    /// ��������� ������ ���������� � ������
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CustomerAction_onReadyToOrder(object sender, EventArgs e)
    {
        customerOrder.ShowOrder();
    }

    /// <summary>
    /// ��������� ������ ���������� ������
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OrderComponent_onOrderComplete(object sender, EventArgs e)
    {
        //���������� ������� �� �����
        SetCustomerState(CustomerState.Exit);
        customerAction.GoToExit();
        //����������� ����� � �����
        customerAction.LeaveTable();
        //�������� UI ������
        customerOrder.HideOrder();
        //�������� ������� � ������� �������
        LevelController.ins.RemoveTableCustomer(this);

        LevelController.ins.CheckWin();
    }   

    /// <summary>
    /// ��������� ����� ������ �� ���������/�����
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CustomerAction_onReadyToDestroy(object sender, EventArgs e)
    {
        LevelController.ins.RemoveLiveCustomer(this);

        Destroy(gameObject);
    }
}
