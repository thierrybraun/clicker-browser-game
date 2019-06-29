using System;
using System.IO;
using UnityEngine;

public abstract class CostFunction : ScriptableObject
{
    public Currency Base;

    public abstract Currency GetCost(int level);

    public Currency GetCostRange(int currentLevel, int targetLevel)
    {
        var current = new Currency();
        for (int i = currentLevel + 1; i <= targetLevel; i++)
        {
            current += GetCost(i);
        }

        return current;
    }

    public int GetMaxLevel(int currentLevel, Currency available)
    {
        if (available == new Currency()) throw new Exception("Currency is 0");
        int targetLevel = currentLevel;
        var cost = GetCost(targetLevel + 1);

        while (cost <= available)
        {
            targetLevel++;
            cost += GetCost(targetLevel + 1);
        }

        return targetLevel;
    }

    private void OnValidate()
    {
        WriteToPhp();
    }

    public void WriteToPhp()
    {
        var path = PHPClassWriter.defaultPath;
        var template = $@"<?php
require_once 'Currency.php';

/**
    This class was automatically generated from Unity, do not change!
**/
class {name} {{        
    /** @var Currency */ public $Base;
    /** @var string */ public $Function = '{GetType().Name}';
    public function __construct() {{
        $this->Base = new Currency({Base.Food}, {Base.Wood}, {Base.Metal});
    }}
}}
        ";
        Directory.CreateDirectory(path);
        File.WriteAllText(Path.Combine(path, $"{name}.php"), template);
    }
}
