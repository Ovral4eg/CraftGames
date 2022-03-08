using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CustomerController : MonoBehaviour
{
    public static CustomerController ins;

    [SerializeField] private List<GameObject> bodies = new List<GameObject>();

    [SerializeField] private List<GameObject> heads = new List<GameObject>();
    [SerializeField] private List<GameObject> arms = new List<GameObject>();
    [SerializeField] private List<GameObject> accesories = new List<GameObject>();

    [SerializeField] private Transform charactersContainer;

    [SerializeField] private GameObject orderUIPrefab;

    internal void Setup()
    {
        ins = this;
    }

    /// <summary>
    /// ������� � ���������� ������ ���������� 
    /// </summary>
    /// <returns></returns>
    public Customer GenerateNewCustomer()
    {
        var randomBody = GetRandomBody();

        var result = randomBody.GetComponent<Customer>();

        //����������� ��������� �������� � ��������
        result.SetCustomerAction();

        //����������� ��������� ���������� �������
        result.SetCustomerOrder();

        //��������� ��������� ��������� ��� ������
        AddRandomHead(result);

        //��������� ������ � ��������� UI ������
        AddOrderUI(result);

        return result;
    }

    private int customerId = 0;
    private GameObject GetRandomBody()
    {
        var random = Random.Range(0, bodies.Count);

        var prefab = bodies[random];

        var result = Instantiate(prefab, charactersContainer);

        result.name = $"customer [{customerId++}]";

        return result;
    }

    private void AddRandomHead(Customer customer)
    {
        var random = Random.Range(0, heads.Count);

        var prefab = heads[random];

        var head = Instantiate(prefab);

        var headRenderers = head.GetComponentsInChildren<Renderer>();

        var newMaterial = new Material(Shader.Find("Standard"));

        var newRandomColor = GenerateRandomColor(); 

        foreach (var r in headRenderers)
        {
            r.material = newMaterial;
            r.material.color = newRandomColor;
        }

        customer.AddHead(head);
    }

    private void AddOrderUI(Customer customer)
    {
        var orderUI = Instantiate(orderUIPrefab);

        //����������� ������ ������ �� ���� ��� ����
        var orderRectTransform = orderUI.GetComponent<RectTransform>();
        orderRectTransform.pivot = Vector2.zero;

        //�������� ������ 
        orderUI.SetActive(false);

        //�������� ��������� ������ � ����������� ��� ����������� ������ � UI
        customer.GetCustomerOrderComponent().SetCustomerOrder(orderUI);
    }

    private Color GenerateRandomColor()
    {
        var r = Random.Range(0f, 1f);
        var g = Random.Range(0f, 1f);
        var b = Random.Range(0f, 1f);

        var result = new Color(r, g, b);

        return result;
    }
}
