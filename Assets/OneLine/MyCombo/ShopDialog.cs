using UnityEngine;
using UnityEngine.UI;

public class ShopDialog : MonoBehaviour
{
    public Text[] numHintTexts, contentTexts, priceTexts;
    public Text removeAdPriceText;
    public Text balanceText;

    private void Start()
    {
#if IAP && UNITY_PURCHASING
        Purchaser.instance.onItemPurchased += OnItemPurchased;

        for(int i = 0; i < numHintTexts.Length; i++)
        {
            numHintTexts[i].text = Purchaser.instance.iapItems[i+2].value.ToString();
            contentTexts[i].text = Purchaser.instance.iapItems[i+2].value + " hints";
            priceTexts[i].text = "$" + Purchaser.instance.iapItems[i+2].price;
        }

        removeAdPriceText.text = "$" + Purchaser.instance.iapItems[0].price;
#endif
    }

    public void OnBuyProduct(int index)
	{
#if IAP && UNITY_PURCHASING
        Sound.instance.PlayButton();
        Purchaser.instance.BuyProduct(index);
#else
        Debug.LogError("Please enable, import and install Unity IAP to use this function");
#endif
    }

    public void Show()
    {
        gameObject.SetActive(true);
        UpdateBalance();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        Sound.instance.PlayButton();
    }

    public void RestorePurchases()
    {
#if IAP && UNITY_PURCHASING
        Sound.instance.PlayButton();
        Purchaser.instance.RestorePurchases();
#else
        Debug.LogError("Please enable, import and install Unity IAP to use this function");
#endif
    }

    private void UpdateBalance()
    {
        balanceText.text = "" + PlayerData.instance.NumberOfHints;
    }

#if IAP && UNITY_PURCHASING
    private void OnItemPurchased(IAPItem item, int index)
    {
        // A consumable product has been purchased by this user.
        if (item.productType == PType.Consumable)
        {
            if (index != 1)
            {
                PlayerData.instance.NumberOfHints += item.value;
                PlayerData.instance.SaveData();
                UpdateBalance();
                Toast.instance.ShowMessage("Your purchase is successful", 2.5f);
                CUtils.SetBuyItem();
            }
        }
        // Or ... a non-consumable product has been purchased by this user.
        else if (item.productType == PType.NonConsumable)
        {
            if (index == 0)
            {
                CUtils.SetRemoveAds(true);
            }
        }
        // Or ... a subscription product has been purchased by this user.
        else if (item.productType == PType.Subscription)
        {
            // TODO: The subscription item has been successfully purchased, grant this to the player.
        }
    }
#endif

#if IAP && UNITY_PURCHASING
    private void OnDestroy()
    {
        Purchaser.instance.onItemPurchased -= OnItemPurchased;
    }
#endif
}
