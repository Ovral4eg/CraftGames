using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderPoint : MonoBehaviour
{
    private Customer currentCustomer;
    [SerializeField] private Transform lookPoint;

    public void SetCustomer(Customer customer)
    {
        currentCustomer = customer;
    }

    public Customer GetCustomer()
    {
        return currentCustomer;
    }

    public Transform GetLookPoint()
    {
        return lookPoint;
    }
}
