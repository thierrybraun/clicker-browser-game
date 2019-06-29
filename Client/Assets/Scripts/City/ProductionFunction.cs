using System.IO;
using UnityEngine;

public abstract class ProductionFunction : ScriptableObject
{
    public Currency Base;

    public abstract Currency GetProduction(int level);

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
