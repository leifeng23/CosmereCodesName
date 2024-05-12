using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum CodesName
    {
        Cosmere,
        CosmereCharacter,
        CosmereSpot,
        CosmereConcept,
        Cytonic,
        CytonicCharacter,
        CytonicSpot,
        CytonicConcept,
    }

    public CosmereCodesName cosmereCodesName;
    public List<Toggle> toggleList;
    // 字典储存每个CodesName的布尔值状态
    public Dictionary<CodesName, bool> codesNameEnableDic;
    public Dictionary<int, (string, string)> codesNameDic;     // 中文and英文 

    private void Awake()
    {
        codesNameEnableDic = new Dictionary<CodesName, bool>();

        // 初始化字典，确保每个CodesName都有初始值
        foreach (CodesName code in Enum.GetValues(typeof(CodesName)))
        {
            string key = code.ToString();
            if (PlayerPrefs.HasKey(key))
            {
                codesNameEnableDic[code] = PlayerPrefs.GetInt(key) == 1;
            }
            else
            {
                // 未找到保存的值，设置默认值（例如false）
                codesNameEnableDic[code] = true;
            }
        }
        Init();
    }

    private void Init()
    {
        for (int i = 0; i < toggleList.Count; i++)
        {
            InitToggle(toggleList[i], (CodesName)i);
        }

        void InitToggle(Toggle t, CodesName cN)
        {
            t.isOn = codesNameEnableDic[cN];
            t.onValueChanged.AddListener((isOn) => OnToggleValueChanged(cN, isOn));
        }
        CheckoutFatherAndSon();
    }

    private void OnToggleValueChanged(CodesName codeName, bool isOn)
    {
        codesNameEnableDic[codeName] = isOn;
        SaveCodesNameEnable(codeName, isOn); // 保存状态
        CheckoutFatherAndSon();
    }

    public void CheckoutFatherAndSon()
    {
        toggleList[1].gameObject.SetActive(codesNameEnableDic[CodesName.Cosmere]);
        toggleList[2].gameObject.SetActive(codesNameEnableDic[CodesName.Cosmere]);
        toggleList[3].gameObject.SetActive(codesNameEnableDic[CodesName.Cosmere]);
        toggleList[5].gameObject.SetActive(codesNameEnableDic[CodesName.Cytonic]);
        toggleList[6].gameObject.SetActive(codesNameEnableDic[CodesName.Cytonic]);
        toggleList[7].gameObject.SetActive(codesNameEnableDic[CodesName.Cytonic]);
    }

    public void SaveCodesNameEnable(CodesName cN, bool isOn)
    {
        PlayerPrefs.SetInt(cN.ToString(), isOn ? 1 : 0);
        PlayerPrefs.Save();
        // foreach (var kvp in codesNameEnable)
        // {
        // PlayerPrefs.SetInt(kvp.Key.ToString(), kvp.Value ? 1 : 0);
        // }
    }

    public void UpdateCodesNameDic()
    {
        codesNameDic = new Dictionary<int, (string, string)>();
        int currentId = 0;

        foreach (CodesName codeName in Enum.GetValues(typeof(CodesName)))
        {
            if (IsMainCategory(codeName) && !codesNameEnableDic[codeName])
            {
                continue; // 大标题未启用，跳过该类别
            }

            if (!IsMainCategory(codeName) && !codesNameEnableDic[GetParentCategory(codeName)])
            {
                continue; // 对应的大标题未启用，跳过该子类别
            }

            List<CodesNameEntity> entityList = codeName.ToString().StartsWith("Cosmere") ? cosmereCodesName.Cosmere : cosmereCodesName.Cytonic;

            foreach (CodesNameEntity entity in entityList)
            {
                string chinese = GetNonEmptyString(entity, codeName);
                string english = GetNonEmptyString(entity, codeName, true);

                if (!string.IsNullOrEmpty(chinese) && !string.IsNullOrEmpty(english))
                {
                    codesNameDic[currentId] = (chinese, english);
                    currentId++;
                }
            }
        }
    }

    private bool IsMainCategory(CodesName codeName)
    {
        return codeName == CodesName.Cosmere || codeName == CodesName.Cytonic;
    }

    private CodesName GetParentCategory(CodesName codeName)
    {
        return codeName.ToString().StartsWith("Cosmere") ? CodesName.Cosmere : CodesName.Cytonic;
    }

    private string GetNonEmptyString(CodesNameEntity entity, CodesName codeName, bool isEnglish = false)
    {
        switch (codeName)
        {
            case CodesName.CosmereCharacter:
            case CodesName.CytonicCharacter:
                return isEnglish ? entity.character_EN : entity.character;
            case CodesName.CosmereSpot:
            case CodesName.CytonicSpot:
                return isEnglish ? entity.spot_EN : entity.spot;
            case CodesName.CosmereConcept:
            case CodesName.CytonicConcept:
                return isEnglish ? entity.concept_EN : entity.concept;
            default:
                return string.Empty;
        }
    }
}