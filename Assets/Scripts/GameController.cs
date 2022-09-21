using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private TMP_Text _balanceTxt;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Transform _target;
    [SerializeField] private List<Data> _companies;

    private List<SaveCompany> _saveCompany;

    private float _balance;

    private void Start()
    {
        _saveCompany = new List<SaveCompany>();

        _balanceTxt.text = ($"Баланс: " + _balance + "$");

        Vector2 size = new(1, 1);

        if (_companies.Count > 0)
        {
            for (int i = 0; i < _companies.Count; i++)
            {
                GameObject obj = Instantiate(_prefab, _target.position, Quaternion.identity);

                obj.transform.SetParent(_target);
                obj.transform.localScale = size;
                obj.GetComponent<CompanyController>().data = _companies[i];
                obj.GetComponent<CompanyController>().nameCompany = _companies[i].NameCompany;
            }
        }

        LoadGame();
    }

    private void OnEnable()
    {
        Events.OnBalance.AddListener(ReplenishmentOfTheBalance);
        Events.OnBuy.AddListener(CanBuy);
        Events.OnSaveCompany.AddListener(AddSaveCompany);
    }

    private void OnDisable()
    {
        Events.OnBalance.RemoveListener(ReplenishmentOfTheBalance);
        Events.OnBuy.RemoveListener(CanBuy);
        Events.OnSaveCompany.RemoveListener(AddSaveCompany);
    }

    private void ReplenishmentOfTheBalance(float value)
    {
        _balance += value;

        ShowBalance();

        SaveGame();
    }

    private void ShowBalance()
    {
        if (_balance < 1000000) _balanceTxt.text = ($"Баланс: " + _balance + "$");
        else _balanceTxt.text = ($"Баланс: " + _balance * 0.000001 + "M$");
    }

    private void CanBuy(float value, Button button, int id)
    {
        if (value <= _balance)
        {
            Events.OnCanBuy.Invoke(button, id, true);

            _balance -= value;

            ShowBalance();

            SaveGame();
        }
        else Events.OnCanBuy.Invoke(button, id, false);
    }

    private void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath
          + "/MySaveData.dat");
        SaveData data = new SaveData();

        data.CompaniesList = new List<SaveCompany>();

        data.Balance = _balance;

        foreach (var item in _saveCompany)
        {
            data.CompaniesList.Add(item);
        }

        bf.Serialize(file, data);
        file.Close();
    }

    private void AddSaveCompany(SaveCompany company)
    {
        if (_saveCompany.Count == 0) _saveCompany.Add(company);
        else
        {
            foreach (var item in _saveCompany)
            {
                if (company.Name == item.Name)
                {
                    _saveCompany.Remove(item);
                    break;
                }
            }
            _saveCompany.Add(company);
        }
    }

    private void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath
          + "/MySaveData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file =
              File.Open(Application.persistentDataPath
              + "/MySaveData.dat", FileMode.Open);

            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();

            _balance = data.Balance;

            foreach (var item in data.CompaniesList)
            {
                _saveCompany.Add(item);
            }

            Events.OnLoadCompany.Invoke(data);

            ShowBalance();
        }
    }

    public void ResetData()
    {
        if (File.Exists(Application.persistentDataPath
          + "/MySaveData.dat"))
        {
            File.Delete(Application.persistentDataPath
              + "/MySaveData.dat");

            _balance = 0;

            _saveCompany.Clear();

            ShowBalance();
        }
    }
}

[Serializable]
public class SaveData
{
    public float Balance;
    public List<SaveCompany> CompaniesList;
}

