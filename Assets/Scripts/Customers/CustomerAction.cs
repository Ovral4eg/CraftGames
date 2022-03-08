using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerAction : MonoBehaviour
{
    public event EventHandler onReadyToOrder;
    public event EventHandler onReadyToDestroy;

    private List<Transform> enterPoints;
    private List<Transform> exitPoints;
    private Transform targetPoint;

    private Customer customer;

    internal void SetCustomer(Customer customer)
    {
        this.customer = customer;
    }

    internal void SetupAndGo(
        List<Transform> enterPoints,
        OrderPoint freePoint,
        List<Transform> exitPoints,
        float customerSpeed)
    {
        customer.SetCustomerState(CustomerState.Idle);

        this.enterPoints = enterPoints;
        targetPoint = freePoint.transform;
        this.exitPoints = exitPoints;

        //��������� ��������� � ��������� �����
        transform.position = enterPoints[0].position;

        //������ ������ ��������
        nextPoint = enterPoints[0].position;

        moveSpeed = customerSpeed;

        customer.SetCustomerState(CustomerState.Enter);
    }

    private OrderPoint orderPoint;
    public void SetOrderPoint(OrderPoint orderPoint)
    {
        this.orderPoint = orderPoint;
    }

    private Vector3 nextPoint;
    private int pointCounter = 0;

    public void Action()
    {
        switch (customer.GetCustomerState())
        {
            case CustomerState.Enter:
                {
                    //���� ������ ������ ��� ������ �� ��������
                    if (transform.position == nextPoint)
                    {
                        pointCounter++;

                        if (pointCounter >= enterPoints.Count)
                        {
                            //���� ��� ������ �����������, �� ���������� ������� � �����
                            nextPoint = targetPoint.position;                           
                        }
                        else
                        {
                            //������ ���� ��������
                            nextPoint = enterPoints[pointCounter].position;
                        }

                        //������� � ������� ���������
                        transform.LookAt(nextPoint);
                    }

                    //���� �������� ����� � �����
                    if (transform.position == targetPoint.position)
                    {
                        //��������� ������� � ����� ������
                        customer.SetCustomerState(CustomerState.Order);

                        //���������� ������� ����������
                        pointCounter = 0;

                        //������������ ������� � �����
                        transform.LookAt(orderPoint.GetLookPoint());

                        //���������� ����� � ���������� ������� �����
                        onReadyToOrder?.Invoke(this, EventArgs.Empty);
                    }                    
                }
                break;

            case CustomerState.Order:
                {                   
                     //������� ���������� ������
                }
                break;

            case CustomerState.Exit:
                {
                    if (transform.position == nextPoint)
                    {
                        pointCounter++;

                        if (pointCounter >= exitPoints.Count)
                        {
                            onReadyToDestroy?.Invoke(this, EventArgs.Empty);                           
                        }
                        else
                        {
                            nextPoint = exitPoints[pointCounter].position;
                        }
                    }

                    transform.LookAt(nextPoint);
                }
                break;
        }
    }

    public void GoToExit()
    {
        nextPoint = exitPoints[0].position;
    }

    public void LeaveTable()
    {
        orderPoint.SetCustomer(null);
    }

    private float moveSpeed = 5;

    public void Move()
    {
        if (customer.GetCustomerState() == CustomerState.Order || GameController.ins.GameIsPaused()) return;

        transform.position = Vector3.MoveTowards(transform.position, nextPoint, moveSpeed * Time.deltaTime);
    }
}
