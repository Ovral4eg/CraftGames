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
        //помещаем ui объект заказа в соотв слот в заказчике
        orderGameObject.transform.SetParent(orderSlot);
        orderGameObject.transform.localPosition = Vector3.zero;
        orderGameObject.transform.rotation = orderSlot.rotation;

        //запоминаем UI объект
        SetOrderGameObject(orderGameObject);
        //запоминаем UI компонент
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

        //перебираем еду в заказе, чтобы отобразить её иконки в UI
        foreach(var f in foodOrder)
        {
            //добавляем в UI иконки заказа и привязываем их к заказу
            var icoSprite = LevelController.ins.FindIco(f.GetFoodId());
            var icoInOrder = orderUI.AddIco(icoSprite); 
            //запоминаем у еды её иконку в заказе
            f.SetIcoInOrder(icoInOrder);
        }

        //запоминаем сколько еды в заказе, чтобы отслеживать прогресс
        foodInOrderCount = foodOrder.Count;

        //обнуляем прогресс заказа в UI
        orderUI.GetProgressBar().fillAmount = 0;
    }

    public bool CheckFoodInOrder(int foodId)
    {
        //ищем в заказе клиента такую еду по ID
        var result = foodOrder.Find(x => x.GetFoodId() == foodId);

        //если такой еды нет
        if(result == null)
        {
            return false;
        }
        else
        {
            //если у клиента есть такая еда в заказе, удаляем её из заказа
            RemoveFoodFromOrder(result);
            return true;
        }
    }

    private void RemoveFoodFromOrder(Food food)
    {
        //удаляем иконку из UI заказа
        Destroy(food.GetIcoInOrder());

        //удаляем еду из списка
        foodOrder.Remove(food);

        //отображаем прогресс выполнения заказа в UI
        var amount = ((float)foodInOrderCount - (float)foodOrder.Count) / foodInOrderCount;
        orderUI.GetProgressBar().fillAmount = amount;

        //если в заказе ничего не осталось, то отправляем клиента на выход
        if (foodOrder.Count == 0)
        {
            onOrderComplete?.Invoke(this, EventArgs.Empty);
        }
    }

    public void BoostOrder()
    {
        //отправляем клиента на выход
        onOrderComplete?.Invoke(this, EventArgs.Empty);        
    }
}
