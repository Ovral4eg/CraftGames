using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableController : MonoBehaviour
{
    [SerializeField] private List<OrderPoint> orderPoints = new List<OrderPoint>();

    /// <summary>
    /// �������� ������ ����� � �����
    /// </summary>
    /// <returns></returns>
    public OrderPoint GetFirstOrderPoint()
    {
        return orderPoints[0];
    }

    /// <summary>
    /// ����� � �������� ��������� ����� � �����
    /// </summary>
    /// <returns></returns>
    public OrderPoint GetFreeOrderPoint()
    {
        foreach(var p in orderPoints)
        {
            if (p.GetCustomer() == null)
            {
                return p;
            }
        }

        return null;
    }

    //���������, � ������� ��������� ��������������� ��� �� �����
    [SerializeField] private Transform foodContainer;

    /// <summary>
    /// �������� ������ ��������� �� ����� ���, �� ������� ����� ������������� ������
    /// </summary>
    /// <returns></returns>
    public List<Food> GetAvailableFood()
    {
        var foods = GetComponentsInChildren<FoodOnTable>();

        var result = new List<Food>();

        foreach(var f in foods)
        {
            result.Add(f.GetFoodComponent());
        }

        return result;
    }
}
