using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;

public static class EnumGenerator
{
    public enum GenerateMode
    {
        Override,
        Append
    }

    public static void Generate(string enumName, string path, string element, GenerateMode mode = GenerateMode.Override, Action<List<int>> onGenerate = null)
    {
        Generate(enumName, path, new[] { element }, mode, onGenerate);
    }

    public static void Remove(string enumName, string path, string element, Action<List<int>> onRemove)
    {
        string[] elements = GetExistingElements(enumName, path)
            .Where(e => e != element)
            .ToArray();
        Generate(enumName, path, elements, GenerateMode.Override, onRemove);
    }
    public static async void Generate(string enumName, string path, string[] elements, GenerateMode mode = GenerateMode.Override, Action<List<int>> onFinishGenerate = null)
    {
        try
        {
            ValidateInput(enumName, elements);
            string normalizedPath = NormalizePath(path);
            string filePath = CreateFilePath(normalizedPath, enumName);

            List<int> newEnumIndices = new List<int>();
            string[] finalElements;

            if (mode == GenerateMode.Append && File.Exists(filePath))
            {
                var existingElements = GetExistingElements(enumName, path);
                var startIndex = existingElements.Length;

                finalElements = existingElements.Concat(elements)
                    .Distinct()
                    .ToArray();

                for (int i = startIndex; i < finalElements.Length; i++)
                {
                    newEnumIndices.Add(i);
                }
            }
            else
            {
                finalElements = elements;
                newEnumIndices = Enumerable.Range(0, elements.Length).ToList();
            }

            CreateAndWriteEnum(filePath, enumName, finalElements);
            AssetDatabase.Refresh();

            await WaitForCompilation(normalizedPath, finalElements.Length);

            string operation = mode == GenerateMode.Override ? "generated" : "updated";
            Debug.Log($"Enum '{enumName}' successfully {operation} at: {filePath}");

            onFinishGenerate?.Invoke(newEnumIndices);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to generate enum: {ex.Message}");
            onFinishGenerate?.Invoke(new List<int>());
        }
    }

    private static int GetEnumCount(string path)
    {
        if (!Directory.Exists(path)) return 0;

        try
        {
            var files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var lines = File.ReadAllLines(file);
                foreach (var line in lines)
                {
                    if (line.Contains("public enum"))
                    {
                        return lines.Count(l => l.Trim().EndsWith(","));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error counting enums: {ex.Message}");
        }

        return 0;
    }

    public static string[] GetExistingElements(string enumName, string path)
    {
        string filePath = CreateFilePath(NormalizePath(path), enumName);

        if (!File.Exists(filePath))
            return new string[0];

        try
        {
            return ParseEnumElements(filePath, enumName);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to get existing elements: {ex.Message}");
            return new string[0];
        }
    }

    private static void ValidateInput(string enumName, string[] elements)
    {
        if (string.IsNullOrEmpty(enumName))
            throw new System.ArgumentException("Enum name cannot be empty", nameof(enumName));

        if (elements == null || elements.Length == 0)
            throw new System.ArgumentException("Elements cannot be empty", nameof(elements));
    }

    private static string NormalizePath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return "Assets/Scripts/Enums";

        path = path.Replace('\\', '/');
        return path.StartsWith("Assets/") ? path.TrimEnd('/') : "Assets/" + path.TrimEnd('/');
    }

    private static string CreateFilePath(string path, string enumName)
        => Path.Combine(path, $"{enumName}.cs");

    private static void CreateAndWriteEnum(string filePath, string enumName, string[] elements)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath, GenerateEnumContent(enumName, elements), Encoding.UTF8);
    }

    private static string GenerateEnumContent(string enumName, string[] elements)
    {
        var builder = new StringBuilder();
        AppendHeader(builder);
        AppendNamespace(builder);
        AppendEnumDeclaration(builder, enumName);
        AppendElements(builder, elements);
        AppendClosing(builder);
        return builder.ToString();
    }

    private static void AppendHeader(StringBuilder builder)
    {
        builder.AppendLine("// This file was auto-generated");
        builder.AppendLine("// Any changes made to this file may be lost");
        builder.AppendLine();
    }

    private static void AppendNamespace(StringBuilder builder)
    {
        builder.AppendLine("namespace Game.Editor");
        builder.AppendLine("{");
    }

    private static void AppendEnumDeclaration(StringBuilder builder, string enumName)
    {
        builder.AppendLine("\t/// <summary>");
        builder.AppendLine($"\t/// Represents {enumName} options");
        builder.AppendLine("\t/// </summary>");
        builder.AppendLine($"\tpublic enum {enumName}");
        builder.AppendLine("\t{");
    }

    private static void AppendElements(StringBuilder builder, string[] elements)
    {
        var validElements = elements
            .Where(e => !string.IsNullOrEmpty(e))
            .Select(e => e.Trim())
            .Distinct();

        foreach (var element in validElements)
        {
            builder.AppendLine($"\t\t{ValidateEnumName(element)},");
        }
    }

    private static void AppendClosing(StringBuilder builder)
    {
        builder.AppendLine("\t}");
        builder.AppendLine("}");
    }

    private static string ValidateEnumName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return "Unknown";

        name = name.Replace(" ", "");

        return char.IsDigit(name[0])
            ? "_" + name
            : new string(name.Where(c => char.IsLetterOrDigit(c)).ToArray());
    }

    private static string[] ParseEnumElements(string filePath, string enumName)
    {
        return File.ReadAllLines(filePath)
            .SkipWhile(line => !line.Contains($"enum {enumName}"))
            .Skip(2) // enum adý ve açýlýþ parantezini atla
            .TakeWhile(line => !line.Trim().Equals("}"))
            .Select(line => line.Trim().TrimEnd(','))
            .Where(line => !string.IsNullOrEmpty(line))
            .ToArray();
    }
    private static async Task WaitForCompilation(string enumPath, int expectedCount)
    {
        var timeoutTask = Task.Delay(5000);
        var startTime = DateTime.Now;

        while (true)
        {
            await Task.Delay(1000);

            int currentAmount = GetEnumCount(enumPath);
            Debug.Log($"Current enum count: {currentAmount}, Expected: {expectedCount}");

            if (currentAmount >= expectedCount)
            {
                Debug.Log("Enum generation completed successfully");
                return;
            }

            if (await Task.WhenAny(Task.Delay(100), timeoutTask) == timeoutTask)
            {
                Debug.LogWarning($"Enum generation timed out after {(DateTime.Now - startTime).TotalSeconds:F1} seconds");
                return;
            }
        }
    }

}