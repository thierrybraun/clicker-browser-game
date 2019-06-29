using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;

public class PHPClassWriter
{
    public const string defaultPath = "../Server/api/generated";

    public static Dictionary<Type, bool> WriteAll(string path = defaultPath)
    {
        var res = new Dictionary<Type, bool>();
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

    public static Dictionary<Type, Action> GetAllTasks(string path = defaultPath)
    {
        var tasks = new Dictionary<Type, Action> {
            {typeof(API.AuthenticateResponse), null},
            {typeof(API.CollectResourcesResponse), null},
            {typeof(API.CreateBuildingResponse), null},
            {typeof(API.GetCityResponse), null},
            {typeof(API.GetPlayerResponse), null},
            {typeof(API.GetStashForTileResponse), null},
            {typeof(API.GetVersionResponse), null},
            {typeof(API.UpgradeResponse), null},
            {typeof(Currency), null},
            {typeof(API.City), null},
            {typeof(API.BuildingType), null},
            {typeof(API.ResourceType), null},
            {typeof(API.Field), null}
        };

        foreach (var task in tasks.Keys.ToList())
        {
            tasks[task] = () => PHPClassWriter.Write(task, task.Name, path);
        }

        var productionFunctions = AssetDatabase.FindAssets("t:ProductionFunction").Select(a => AssetDatabase.LoadAssetAtPath<ProductionFunction>(AssetDatabase.GUIDToAssetPath(a)));
        var costFunctions = AssetDatabase.FindAssets("t:CostFunction").Select(a => AssetDatabase.LoadAssetAtPath<CostFunction>(AssetDatabase.GUIDToAssetPath(a)));

        foreach (var task in productionFunctions)
        {
            tasks[task.GetType()] = () => task.WriteToPhp();
        }
        foreach (var task in costFunctions)
        {
            tasks[task.GetType()] = () => task.WriteToPhp();
        }
        return tasks;
    }

    public static void Write(Type type, string filename, string path = defaultPath)
    {
        UnityEngine.Debug.Log($"Writing {type.Name} to {filename}.php");

        var members = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
        var membersText = String.Join("\n", members.Select(m => MemberToLine(m)));
        var constructorParameters = String.Join(", ", members.Select(m => "$" + m.Name));
        var constructorBody = String.Join("\n", members.Select(m => $"\t\t$this->{m.Name} = ${m.Name};"));

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
