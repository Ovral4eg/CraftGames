using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderUI : MonoBehaviour
{
    [SerializeField] private Transform icoContainer;

    [SerializeField] private GameObject icoPrefab;

    [SerializeField] private Image progressBar;

    public GameObject AddIco(Sprite icoSprite)
    {
        var result = Instantiate(icoPrefab, icoContainer);

        var icoImage = result.GetComponent<Image>();

        icoImage.sprite = icoSprite;

        return result;
    }

    public Image GetProgressBar()
    {
        return progressBar;
    }
}
