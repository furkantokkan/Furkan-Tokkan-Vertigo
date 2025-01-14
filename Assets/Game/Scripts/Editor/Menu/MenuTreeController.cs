using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
public class MenuTreeController
{
    public void AddMenuToTree<T>(OdinMenuTree tree, string path, string title) where T : ScriptableObject
    {
        var mainAssets = LoadMainAssets<T>(path);

        if (!mainAssets.Any())
        {
            tree.Add(title, null);
            return;
        }

        foreach (var (asset, assetPath) in mainAssets)
        {
            var menuPath = CreateMenuPath(title, asset.name);
            tree.Add(menuPath, asset);
            AddSubMenuToTree(tree, assetPath, menuPath);
        }
    }

    private IEnumerable<(T asset, string path)> LoadMainAssets<T>(string path) where T : ScriptableObject
    {
        var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { path });

        if (!guids.Any())
        {
            Debug.LogWarning($"No assets found at path: {path}");
            yield break;
        }

        foreach (var guid in guids)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            if (asset == null)
            {
                Debug.LogWarning($"Failed to load asset at path: {assetPath}");
                continue;
            }

            yield return (asset, assetPath);
        }
    }

    private void AddSubMenuToTree(OdinMenuTree tree, string assetPath, string menuPath)
    {
        var subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath)
            .Where(subAsset => subAsset != null);

        foreach (var subAsset in subAssets)
        {
            var subMenuPath = CreateMenuPath(menuPath, subAsset.name);
            tree.Add(subMenuPath, subAsset);
        }
    }
    
    private string CreateMenuPath(string basePath, string itemName)
        => $"{basePath}/{itemName}";
}