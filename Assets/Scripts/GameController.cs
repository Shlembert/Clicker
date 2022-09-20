using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private TMP_Text balanse;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform target;
    [SerializeField] private List<Data> companies;

    private List<SaveCompany> saveCompany;
    private float _balance;

    private void Start()
    {
        balanse.text = ($"Баланс: " + _balance + "$");

        Vector2 size = new(1, 1);

        if (companies.Count > 0)
        {
            for (int i = 0; i < companies.Count; i++)
            {
                GameObject obj = Instantiate(prefab, target.position, Quaternion.identity);

                obj.transform.SetParent(target);
                obj.transform.localScale = size;
                obj.GetComponent<CompanyController>().data = companies[i];
                obj.GetComponent<CompanyController>().nameCompany = companies[i].NameCompany;
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
        if (_balance < 1000000) balanse.text = ($"Баланс: " + _balance + "$");
        else balanse.text = ($"Баланс: " + _balance * 0.000001 + "M$");
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

        foreach (var item in saveCompany)
        {
            data.CompaniesList.Add(item);
        }

        bf.Serialize(file, data);
        file.Close();
    }

    private void AddSaveCompany(SaveCompany company)
    {
        if (saveCompany.Count == 0) saveCompany.Add(company);
        else
        {
            foreach (var item in saveCompany)
            {
                if (company.Name == item.Name)
                {
                    saveCompany.Remove(item);
                    break;
                }
            }
            saveCompany.Add(company);
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
                saveCompany.Add(item);
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

            saveCompany.Clear();

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

