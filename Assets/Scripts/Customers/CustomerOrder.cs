using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomerOrder : MonoBehaviour
{
    public event EventHandler onOrderComplete;

    private Customer customer;

    public void SetCustomer(Customer customer)
    {
        this.customer = customer;
    }

    [SerializeField] private Transform orderSlot;

    public void SetCustomerOrder(GameObject orderGameObject)
    {
        //�������� ui ������ ������ � ����� ���� � ���������
        orderGameObject.transform.SetParent(orderSlot);
        orderGameObject.transform.localPosition = Vector3.zero;
        orderGameObject.transform.rotation = orderSlot.rotation;

        //���������� UI ������
        SetOrderGameObject(orderGameObject);
        //���������� UI ���������
        SetOrderUIComponent(orderGameObject.GetComponent<OrderUI>());
    }

    private GameObject orderGameObject;

    public void SetOrderGameObject(GameObject orderGameObject)
    {
        this.orderGameObject = orderGameObject;
    }

    public void ShowOrder()
    {
        orderGameObject.SetActive(true);
    }

    public void HideOrder()
    {
        orderGameObject.SetActive(false);
    }

    private OrderUI orderUI;

    public void SetOrderUIComponent(OrderUI orderUI)
    {
        this.orderUI = orderUI;
    }

    [SerializeField] private List<Food> foodOrder = new List<Food>();
    private int foodInOrderCount = 0;

    public void SetFoodOrder(List<Food> foodOrder)
    {
        this.foodOrder = foodOrder;

        //���������� ��� � ������, ����� ���������� � ������ � UI
        foreach(var f in foodOrder)
        {
            //��������� � UI ������ ������ � ����������� �� � ������
            var icoSprite = LevelController.ins.FindIco(f.GetFoodId());
            var icoInOrder = orderUI.AddIco(icoSprite); 
            //���������� � ��� � ������ � ������
            f.SetIcoInOrder(icoInOrder);
        }

        //���������� ������� ��� � ������, ����� ����������� ��������
        foodInOrderCount = foodOrder.Count;

        //�������� �������� ������ � UI
        orderUI.GetProgressBar().fillAmount = 0;
    }

    public bool CheckFoodInOrder(int foodId)
    {
        //���� � ������ ������� ����� ��� �� ID
        var result = foodOrder.Find(x => x.GetFoodId() == foodId);

        //���� ����� ��� ���
        if(result == null)
        {
            return false;
        }
        else
        {
            //���� � ������� ���� ����� ��� � ������, ������� � �� ������
            RemoveFoodFromOrder(result);
            return true;
        }
    }

    private void RemoveFoodFromOrder(Food food)
    {
        //������� ������ �� UI ������
        Destroy(food.GetIcoInOrder());

        //������� ��� �� ������
        foodOrder.Remove(food);

        //���������� �������� ���������� ������ � UI
        var amount = ((float)foodInOrderCount - (float)foodOrder.Count) / foodInOrderCount;
        orderUI.GetProgressBar().fillAmount = amount;

        //���� � ������ ������ �� ��������, �� ���������� ������� �� �����
        if (foodOrder.Count == 0)
        {
            onOrderComplete?.Invoke(this, EventArgs.Empty);
        }
    }

    public void BoostOrder()
    {
        //���������� ������� �� �����
        onOrderComplete?.Invoke(this, EventArgs.Empty);        
    }
}
