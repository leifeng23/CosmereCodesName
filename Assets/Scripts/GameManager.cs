using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CodesName
{
    Cosmere,
    CosmereBooks,
    CosmereCharacters,
    CosmereCulture,
    CosmereEventsAndEras,
    CosmereLocations,
    CosmereMagic,
    CosmereObjectAndMaterial,
    Cytonic,
    CytonicCharacters,
    CytonicSpots,
    CytonicConcepts,
}

public class GameManager : MonoBehaviour
{
    private bool isSpy = false;
    private List<Faction> randomColor;
    public CosmereCodesName codesName;
    public List<Toggle> toggleList;
    public List<Slate> slates;
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
        toggleList[4].gameObject.SetActive(codesNameEnableDic[CodesName.Cosmere]);
        toggleList[5].gameObject.SetActive(codesNameEnableDic[CodesName.Cosmere]);
        toggleList[6].gameObject.SetActive(codesNameEnableDic[CodesName.Cosmere]);
        toggleList[7].gameObject.SetActive(codesNameEnableDic[CodesName.Cosmere]);
        toggleList[9].gameObject.SetActive(codesNameEnableDic[CodesName.Cytonic]);
        toggleList[10].gameObject.SetActive(codesNameEnableDic[CodesName.Cytonic]);
        toggleList[11].gameObject.SetActive(codesNameEnableDic[CodesName.Cytonic]);
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

    #region GameCode
    public void GameBegin()
    {
        UpdateCodesNameDic(); // 更新字典
        RandomColor();

        // 从字典中随机选取 25 个不重复的单词
        List<int> selectedKeys = GetRandomKeys(codesNameDic, 25);

        // 设置每个 Slate 的单词、颜色和初始化
        for (int i = 0; i < slates.Count; i++)
        {
            if (i < selectedKeys.Count)
            {
                (string chinese, string english) = codesNameDic[selectedKeys[i]];
                Faction faction = GetFaction(i); // 获取阵营颜色
                slates[i].Init(chinese, english, faction);
                // slates[i].UpdateColor(); // 更新颜色
            }
            else
            {
                // 没有足够的单词，隐藏多余的 Slate
                slates[i].gameObject.SetActive(false);
            }
        }
    }

    private List<int> GetRandomKeys(Dictionary<int, (string, string)> dictionary, int count)
    {
        List<int> keys = new List<int>(dictionary.Keys);
        if (count >= keys.Count) // 如果 count 超过字典的键数量，直接返回所有键
        {
            return keys;
        }

        // 使用 Fisher-Yates 洗牌算法打乱键的顺序
        for (int i = keys.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (keys[i], keys[j]) = (keys[j], keys[i]);
        }

        return keys.GetRange(0, count);
    }

    // 辅助函数：根据索引获取阵营颜色
    private void RandomColor()
    {
        randomColor = new List<Faction>();
        for (int i = 0; i < 9; i++) randomColor.Add(Faction.Red);
        for (int i = 0; i < 8; i++) randomColor.Add(Faction.Blue);
        for (int i = 0; i < 7; i++) randomColor.Add(Faction.Neutral);
        randomColor.Add(Faction.Boom);

        // 打乱列表顺序
        for (int i = randomColor.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (randomColor[i], randomColor[j]) = (randomColor[j], randomColor[i]);
        }
    }

    private Faction GetFaction(int index)
    {
        return randomColor[index];
    }

    public void ClickSpyButton()
    {
        isSpy = !isSpy;
        foreach (var s in slates)
        {
            if (!s.isClicked)
            {
                s.UpdateColor(isSpy);
            }
        }
    }

    public void ClickExitButton()
    {
        isSpy = false;
    }

    #endregion

    #region UpdateDic
    public void UpdateCodesNameDic()
    {
        codesNameDic = new Dictionary<int, (string, string)>();
        int currentId = 0;

        foreach (CodesName codeName in Enum.GetValues(typeof(CodesName)))
        {
            if (IsMainCategory(codeName))
            {
                continue; // 跳过大标题
            }

            if (!codesNameEnableDic[GetParentCategory(codeName)])
            {
                continue; // 对应的大标题未启用，跳过该子类别
            }

            if (codeName.ToString().StartsWith("Cosmere"))
            {
                List<CosmereEntity> entityList = codesName.Cosmere;
                foreach (CosmereEntity entity in entityList)
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
            else if (codeName.ToString().StartsWith("Cytonic"))
            {
                List<CytonicEntity> entityList = codesName.Cytonic;
                foreach (CytonicEntity entity in entityList)
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
    }

    private bool IsMainCategory(CodesName codeName)
    {
        return codeName == CodesName.Cosmere || codeName == CodesName.Cytonic;
    }

    private CodesName GetParentCategory(CodesName codeName)
    {
        return codeName.ToString().StartsWith("Cosmere") ? CodesName.Cosmere : CodesName.Cytonic;
    }

    private string GetNonEmptyString(CosmereEntity entity, CodesName codeName, bool isEnglish = false)
    {
        switch (codeName)
        {
            case CodesName.CosmereBooks:
                return isEnglish ? entity.books_EN : entity.books;
            case CodesName.CosmereCharacters:
                return isEnglish ? entity.characters_EN : entity.characters;
            case CodesName.CosmereCulture:
                return isEnglish ? entity.culture_EN : entity.culture;
            case CodesName.CosmereEventsAndEras:
                return isEnglish ? entity.eventsAndEras_EN : entity.eventsAndEras;
            case CodesName.CosmereLocations:
                return isEnglish ? entity.locations_EN : entity.locations;
            case CodesName.CosmereMagic:
                return isEnglish ? entity.magic_EN : entity.magic;
            case CodesName.CosmereObjectAndMaterial:
                return isEnglish ? entity.objectAndMaterial_EN : entity.objectAndMaterial;
            default:
                return string.Empty;
        }
    }

    private string GetNonEmptyString(CytonicEntity entity, CodesName codeName, bool isEnglish = false)
    {
        switch (codeName)
        {
            case CodesName.CytonicCharacters:
                return isEnglish ? entity.character_EN : entity.character;
            case CodesName.CytonicSpots:
                return isEnglish ? entity.spot_EN : entity.spot;
            case CodesName.CytonicConcepts:
                return isEnglish ? entity.concept_EN : entity.concept;
            default:
                return string.Empty;
        }
    }
    #endregion
}