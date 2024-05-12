using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Slate : MonoBehaviour
{
    public bool isClicked;
    public Faction factionColor;
    public List<GameObject> colors;
    public TextMeshProUGUI ch;
    public TextMeshProUGUI en;


    public void Init(string c, string e, Faction f)
    {
        ch.text = c;
        en.text = e;
        factionColor = f;
    }

    public void UpdateColor(bool isShow)
    {
        foreach (var c in colors)
        {
            c.SetActive(false);
        }
        Debug.Log((int)factionColor);
        colors[(int)factionColor].SetActive(isShow);
    }

    public void ClickSlate()
    {
        isClicked = !isClicked;
        UpdateColor(isClicked);
    }
}

public enum Faction
{
    Red,
    Blue,
    Neutral,
    Boom,
};