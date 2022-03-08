using UnityEngine;

public class ShopController : MonoBehaviour
{
    public void BuyBooster(int value)
    {
        LevelController.ins.AddBooster(value);
    }
}
