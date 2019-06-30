using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class PHPClassWriter
{
    public const string defaultPath = "../Server/api/generated";
    public static ProductionFunction[] ProductionFunctions;
    public static CostFunction[] CostFunctions;

    public static Dictionary<string, bool> WriteAll(string path = defaultPath)
    {
        var res = new Dictionary<string, bool>();
        foreach (var task in GetAllTasks(path))
        {
            res[task.Key] = false;
            try
            {
                task.Value();
                res[task.Key] = true;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.Message);
            }
        }
        return res;
    }

    public static Dictionary<string, Action> GetAllTasks(string path = defaultPath)
    {
        var tasks = new Dictionary<string, Action>();
        var classes = new List<Type>{
            typeof(API.AuthenticateResponse),
            typeof(API.CollectResourcesResponse),
            typeof(API.CreateBuildingResponse),
            typeof(API.GetCityResponse),
            typeof(API.GetPlayerResponse),
            typeof(API.GetStashForTileResponse),
            typeof(API.GetVersionResponse),
            typeof(API.UpgradeResponse),
            typeof(API.DeleteAccountResponse),
            typeof(API.RegistrationResponse),
            typeof(Currency),
            typeof(API.City),
            typeof(API.BuildingType),
            typeof(API.ResourceType),
            typeof(FieldType),
            typeof(API.Field),
            typeof(Player)
        };
        foreach (var task in classes)
        {
            tasks[task.Name] = () => PHPClassWriter.Write(task, task.Name, path);
        }

        var productionFunctions = Resources.LoadAll("Production", typeof(ProductionFunction));
        var costFunctions = Resources.LoadAll("Cost", typeof(CostFunction));
        // var productionFunctions = AssetDatabase.FindAssets("t:ProductionFunction").Select(a => AssetDatabase.LoadAssetAtPath<ProductionFunction>(AssetDatabase.GUIDToAssetPath(a)));
        // var costFunctions = AssetDatabase.FindAssets("t:CostFunction").Select(a => AssetDatabase.LoadAssetAtPath<CostFunction>(AssetDatabase.GUIDToAssetPath(a)));

        foreach (var task in productionFunctions)
        {
            tasks[task.name] = () => (task as ProductionFunction).WriteToPhp();
        }
        foreach (var task in costFunctions)
        {
            tasks[task.name] = () => (task as CostFunction).WriteToPhp();
        }

        tasks["Building functions"] = () => WriteBuildingFunctions(path);

        return tasks;
    }

    private static void WriteBuildingFunctions(string path = defaultPath)
    {
        var filename = "BuildingFunctions";
        UnityEngine.Debug.Log($"Writing building functions to {filename}.php");

        var buildings = Resources.LoadAll("Building", typeof(Building));
        // var buildings = AssetDatabase.FindAssets("t:Building").Select(a => AssetDatabase.LoadAssetAtPath<Building>(AssetDatabase.GUIDToAssetPath(a)));

        var costs = new List<string>();
        var prods = new List<string>();
        foreach (var building in buildings)
        {
            var b = building as Building;
            var type = ((int)b.ApiType);
            var cost = b.BuildCostFunction.name;
            var prod = b.ProductionFunction.name;

            costs.Add($"{type} => \"{cost}\"");
            prods.Add($"{type} => \"{prod}\"");
        }

        var template = $@"<?php
/**
    This class was automatically generated from Unity, do not change!
**/
class {filename} {{    
    public static $costFunctions = array({String.Join(",", costs)});
    public static $productionFunctions = array({String.Join(",", prods)});
}}
        ";

        Directory.CreateDirectory(path);
        File.WriteAllText(Path.Combine(path, $"{filename}.php"), template);
    }

    public static void Write(Type type, string filename, string path = defaultPath)
    {
        UnityEngine.Debug.Log($"Writing {type.Name} to {filename}.php");

        if (type.IsEnum)
        {
            WriteEnum(type, filename, path);
            return;
        }
        var members = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
        var membersText = String.Join("\n", members.Select(m => MemberToLine(m)));
        var constructorParameters = String.Join(", ", members.Select(m => "$" + m.Name + " = null"));
        var constructorBody = String.Join("\n", members.Select(m => $"\t\tif (${m.Name} !== null) $this->{m.Name} = ${m.Name};"));

        var template = $@"<?php
/**
    This class was automatically generated from Unity, do not change!
**/
class {type.Name} {{    
    public function __construct({constructorParameters}) {{
{constructorBody}        
    }}
{membersText}
}}
        ";
        Directory.CreateDirectory(path);
        File.WriteAllText(Path.Combine(path, $"{filename}.php"), template);
    }

    private static void WriteEnum(Type type, string filename, string path = defaultPath)
    {
        var values = type.GetEnumValues();
        var body = "";
        for (int i = 0; i < values.Length; i++)
        {
            body += "const " + values.GetValue(i) + " = " + i + ";\n";
        }
        var template = $@"<?php
/**
    This class was automatically generated from Unity, do not change!
**/
class {type.Name} {{    
{body}
}}
        ";
        Directory.CreateDirectory(path);
        File.WriteAllText(Path.Combine(path, $"{filename}.php"), template);

    }

    private static String MemberToLine(FieldInfo info, object obj = null)
    {
        var typeMap = new Dictionary<Type, string>{
            {typeof(System.Boolean), "bool"},
            {typeof(System.Byte), "int"},
            {typeof(System.SByte), "int"},
            {typeof(System.Char), "string"},
            {typeof(System.Decimal), "int"},
            {typeof(System.Double), "float"},
            {typeof(System.Single), "float"},
            {typeof(System.Int32), "int"},
            {typeof(System.UInt32), "int"},
            {typeof(System.Int64), "int"},
            {typeof(System.UInt64), "int"},
            {typeof(System.Object), "object"},
            {typeof(System.Int16), "int"},
            {typeof(System.UInt16), "int"},
            {typeof(System.String), "string"}
        };

        var valueConverters = new Dictionary<Type, Func<object, string>> {
            {typeof(Currency), c => "new Currency()"}
        };

        var typeString = "/**  */";
        if (typeMap.ContainsKey(info.FieldType))
        {
            typeString = $"/** @var {typeMap[info.FieldType]} */";
        }
        else
        {
            typeString = $"/** @var {info.FieldType.Name} */";
        }

        return $"\t{typeString} public ${info.Name};";
    }
}
