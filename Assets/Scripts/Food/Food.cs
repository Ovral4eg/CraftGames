using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Food 
{
    public Food(Food food)
    {
        foodId = food.GetFoodId();
        foodIco = food.GetFoodIco();
    }

    [SerializeField] private int foodId;

    public int GetFoodId()
    {
        return foodId;
    }

    [SerializeField] private Sprite foodIco; 

    public Sprite GetFoodIco()
    {
        return foodIco;
    }

    private GameObject icoInOrder; 

    public void SetIcoInOrder(GameObject icoInOrder)
    {
        this.icoInOrder = icoInOrder;
    }

    public GameObject GetIcoInOrder()
    {
        return icoInOrder;
    }
}
