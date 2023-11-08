using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;

public class LogHelper : StaticInstance<LogHelper>
{
    public Dictionary<string, string> variableValues = new Dictionary<string, string>();
    public TMP_Text logText; // Ссылка на компонент TextMeshPro, где будет выводиться текст

    // Метод для дебага, записи значения переменной в словарь и вывода всех переменных на экран
    public void LogText<T>(Expression<Func<T>> value)
    {
        var me = (MemberExpression)value.Body;
        var variableName = me.Member.Name;
        var variableValue = value.Compile()();

        if (variableValues.ContainsKey(variableName))
        {
            // Если переменная с таким именем уже существует в словаре, заменяем её значение
            variableValues[variableName] = variableValue.ToString();
        }
        else
        {
            // Если переменной с таким именем нет в словаре, добавляем её
            variableValues.Add(variableName, variableValue.ToString());
        }

        // Выводим все переменные на экран
        UpdateLogText();
    }
    
    public void LogText<T>(Expression<Func<T>> value, string variableName)
    {
        var me = (MemberExpression)value.Body;
        var variableValue = value.Compile()();

        if (variableValues.ContainsKey(variableName))
        {
            // Если переменная с таким именем уже существует в словаре, заменяем её значение
            variableValues[variableName] = variableValue.ToString();
        }
        else
        {
            // Если переменной с таким именем нет в словаре, добавляем её
            variableValues.Add(variableName, variableValue.ToString());
        }

        // Выводим все переменные на экран
        UpdateLogText();
    }

    // Метод для обновления текста в компоненте TextMeshPro
    private void UpdateLogText()
    {
        // Очищаем предыдущий текст
        logText.text = string.Empty;

        // Создаем строку для вывода всех переменных
        string logString = string.Empty;
        foreach (var kvp in variableValues)
        {
            logString += kvp.Key + " => " + kvp.Value + "\n";
        }

        // Выводим новый текст
        logText.text = logString;
    }
}