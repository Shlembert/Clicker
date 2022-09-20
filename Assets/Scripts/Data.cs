using UnityEngine;

[CreateAssetMenu(fileName = "Company", menuName = "Create Company")]

public class Data : ScriptableObject
{
    public string NameCompany;
    public int Lvl;
    public int Profit;
    public int Price;
    public float Delay;
    public string FirstUpgradeName;
    public int FirstMultiplier;
    public int FirstPrice;
    public string SecondUpgradeName;
    public int SecondMultiplier;
    public int SecondPrice;
}
