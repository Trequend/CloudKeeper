using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class Score : MonoBehaviour
{
    private int _value = 0;

    private Text _text;

    private static Score _instance;

    public static int Value
    {
        get => _instance._value;
        set => _instance.SetValue(value);
    }

    private void Awake()
    {
        if (_instance != null)
        {
            throw new Exception("Score already exists");
        }

        _instance = this;
        _text = GetComponent<Text>();
        SetValue(0);
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    private void SetValue(int value)
    {
        if (_value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }

        _value = value;
        _text.text = $"Score: {_value}";
    }
}
