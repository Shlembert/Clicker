
using UnityEngine.UI;

public static class Events
{
    public static readonly Registration OnGameStart = new Registration();

    public static readonly Registration<float> OnBalance = new Registration<float>();

    public static readonly Registration<Button, int, bool> OnCanBuy = new Registration<Button, int, bool>();

    public static readonly Registration<SaveCompany> OnSaveCompany = new Registration<SaveCompany>();

    public static readonly Registration<SaveData> OnLoadCompany = new Registration<SaveData>();

    public static readonly Registration <float, Button, int> OnBuy = new Registration <float, Button, int>();
   
    public static readonly Registration OnGameOver = new Registration();
}

