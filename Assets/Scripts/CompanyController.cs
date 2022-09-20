using DG.Tweening;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompanyController : MonoBehaviour
{
    [SerializeField] private TMP_Text _txtCompanyName, _txtLevel, _txtPriceUp, _txtProfit;
    [SerializeField] private TMP_Text _txtUpgradeName_1, _txtProfit_1, _txtPrice_1;
    [SerializeField] private TMP_Text _txtUpgradeName_2, _txtProfit_2, _txtPrice_2;
    [SerializeField] private Button _button1, _button2;
    [SerializeField] private Slider _slider;

    public Data data;
    public string nameCompany;
    private float _lvl;

    private float _profit;
    private float _delay;
    private float _currentDelay;
    private float _currentProfit;
    private float _currentPrice;
    private bool _load;

    private float _basePrice;
    private string _done = "Куплено";

    private string _nameUp_1;
    private float _multiplay_1;
    private float _priceUp_1;
    private float _currentUp1 = 0;
    private bool _press1;

    private string _nameUp_2;
    private float _multiplay_2;
    private float _priceUp_2;
    private float _currentUp2 = 0;
    private bool _press2;

    private int _id;
    private bool _breakTimer;

    private void Start()
    {
        if (data && !_load) SetParameters();
       
        _basePrice = data.Price;
        _profit = data.Profit;
        _delay = data.Delay;
        _slider.maxValue = _delay;
        _slider.minValue = 0;

        _id = GetInstanceID();

        ShowUI();
    }

    private void OnEnable()
    {
        Events.OnCanBuy.AddListener(CanBuy);
        Events.OnLoadCompany.AddListener(LoadCompany);
    }

    private void OnDisable()
    {
        Events.OnCanBuy.RemoveListener(CanBuy);
        Events.OnLoadCompany.RemoveListener(LoadCompany);
    }

    private void SetParameters()
    {
        _nameUp_1 = data.FirstUpgradeName;
        _nameUp_2 = data.SecondUpgradeName;

        _multiplay_1 = data.FirstMultiplier;
        _priceUp_1 = data.FirstPrice;

        _multiplay_2 = data.SecondMultiplier;
        _priceUp_2 = data.SecondPrice;
        _currentPrice = data.Price;
        _lvl = data.Lvl;
    }

    private void ShowUI()
    {
        _currentProfit = _lvl * _profit * (1 + _currentUp1 + _currentUp2);
        _txtLevel.text = _lvl.ToString();

        _txtCompanyName.text = nameCompany;
        _txtUpgradeName_1.text = _nameUp_1;
        _txtUpgradeName_2.text = _nameUp_2;
       
        _txtProfit_1.text = ($"Доход: " + _multiplay_1 + "%");
        _txtProfit_2.text = ($"Доход: " + _multiplay_2 + "%");

        _txtPrice_1.text = ($"Цена: " + _priceUp_1 + "$");
        _txtPrice_2.text = ($"Цена: " + _priceUp_2 + "$");

        if (_currentPrice < 1000000) _txtPriceUp.text = ($"Цена: " + _currentPrice + "$");
        else _txtPriceUp.text = ($"{ _currentProfit * 0.000001}M$");

        if (_currentProfit < 1000000) _txtProfit.text = ($"" + _currentProfit + "$");
        else _txtProfit.text = ($"{ _currentProfit * 0.000001}M$");
    }

    public void ClickButtonSend(Button button)
    {
        float currentPrice = 0;

        switch (button.name)
        {
            case "LevelUp":
                currentPrice = _currentPrice;
                break;
            case "Upgrade1":
                currentPrice = _priceUp_1;
                break;
            case "Upgrade2":
                currentPrice = _priceUp_2;
                break;
            default:
                break;
        }
        Events.OnBuy.Invoke(currentPrice, button, _id);
    }

    private void CanBuy(Button button, int id, bool canBuy)
    {
        if (id == _id)
        {
            if (canBuy)
            {
                switch (button.name)
                {
                    case "LevelUp":
                        _lvl++;
                        _currentPrice += (_lvl + 1) * _basePrice;
                        break;
                    case "Upgrade1":
                        _currentUp1 = Upgrade(button, _multiplay_1);
                        _txtPrice_1.text = _done;
                        _press1 = true;
                        break;
                    case "Upgrade2":
                        _currentUp2 = Upgrade(button, _multiplay_2);
                        _txtPrice_2.text = _done;
                        _press2 = true;
                        break;
                    default:
                        break;
                }
                ClickBuy(button);
            }
            else Shaking(button);
        }
    }

    private void Shaking(Button button)
    {
        Transform item = button.transform;

        float center = item.position.x;
        float left = item.position.x - 0.05f;
        float right = item.position.x + 0.05f;

        var Seq = DOTween.Sequence();

        for (int i = 0; i < 3; i++)
        {
            Seq.Append(item.DOMoveX(left, 0.05f).From());
            Seq.Append(item.DOMoveX(right, 0.05f).From());
            Seq.Append(item.DOMoveX(center, 0.01f));
        }
    }

    private void ClickBuy(Button button)
    {
        ShowUI();
        SendDataCompany();

        Transform item = button.transform;
        Vector2 big = new(1.3f, 1.3f);
        Vector2 normal = new(1, 1f);

        var Seq = DOTween.Sequence();

        Seq.Append(item.DOScale(big, 0.03f));
        Seq.Append(item.DOScale(normal, 0.3f));
    }

    private float Upgrade(Button button, float multiplay)
    {
        float currentUp;
        currentUp = _profit * multiplay * 0.01f;
        button.interactable = false;
        return currentUp;
    }

    private void Update()
    {
        if (_lvl > 0 && !_breakTimer)
        {
            if (_currentDelay >= 0)
            {
                _currentDelay -= Time.deltaTime;
                _slider.value = _currentDelay;
            }
            else
            {
                _currentDelay = _delay;
                Events.OnBalance.Invoke(_currentProfit);
            }
        }
        else _slider.value = _delay;
    }

    private async Task<SaveCompany> SaveCompany()
    {
        await Task.Yield();

        SaveCompany data = new()
        {
            Name = nameCompany,
            Name1 = _nameUp_1,
            Name2 = _nameUp_2,

            Lvl = _lvl,
            Price = _currentPrice,
            Profit = _currentProfit,

            Price1 = _priceUp_1,
            Price2 = _priceUp_2,

            Multiplay1 = _multiplay_1,
            Multiplay2 = _multiplay_2,

            Press1 = _press1,
            Press2 = _press2
        };

        return data;
    }

    private async void SendDataCompany()
    {
        var saveCompany = await SaveCompany();
        Events.OnSaveCompany.Invoke(saveCompany);
    }

    private void LoadCompany(SaveData data)
    {
        foreach (var saveData in data.CompaniesList)
        {
            if (saveData.Name == nameCompany)
            {
                _load = true;

                _lvl = saveData.Lvl;
                _currentPrice = saveData.Price;

                _profit = saveData.Profit;

                _nameUp_1 = saveData.Name1;
                _nameUp_2 = saveData.Name2;

                _press1 = saveData.Press1;
                _press2 = saveData.Press2;

                _priceUp_1 = saveData.Price1;
                _priceUp_2 = saveData.Price2;

                _multiplay_1 = saveData.Multiplay1;
                _multiplay_2 = saveData.Multiplay2;

                if (_press1) _button1.interactable = false;
                if (_press2) _button2.interactable = false;

                break;
            }
        }
    }
}

[Serializable]
public class SaveCompany
{
    public string Name;
    public string Name1;
    public string Name2;

    public float Lvl;
    public float Profit;
    public float Price;

    public float Price1;
    public float Price2;

    public float Multiplay1;
    public float Multiplay2;

    public bool Press1;
    public bool Press2;
}
