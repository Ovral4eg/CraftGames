using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodOnTable : MonoBehaviour, IUse
{
    [SerializeField] private Food food;

    public Food GetFoodComponent()
    {
        return food;
    }

    public void Use()
    {
        //���� ���� �� ����� ������ �� ������
        if (GameController.ins.GameIsPaused()) return;

        //
        LevelController.ins.UseFood(food.GetFoodId());
    }
}
