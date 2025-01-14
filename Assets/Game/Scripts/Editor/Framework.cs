using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using Game.Boxes;
using System.Linq;
using Game.Collectable;

namespace Game.Editor
{
    public class Framework : OdinMenuEditorWindow
    {
        private readonly BoxCreationController boxCreationController = new BoxCreationController();
        private readonly RewardCreationController rewardCreationController = new RewardCreationController();
        private readonly ItemCreationController itemCreationController = new ItemCreationController();

        private readonly MenuStateController menuStateController = new MenuStateController();
        private readonly MenuTreeController menuTreeService = new MenuTreeController();

        [MenuItem("Tools/Game Framework", false, 1)]
        private static void OpenWindow()
        {
            var window = GetWindow<Framework>("Game Framework");
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1200, 600);
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(true)
            {
                Config = { DrawSearchToolbar = true },
                DefaultMenuStyle = { IconSize = 28f }
            };

            menuTreeService.AddMenuToTree<Box>(tree, EditorConst.BOX_PATH, EditorConst.BOX_CREATION_MENU);
            menuTreeService.AddMenuToTree<AbstractReward>(tree, EditorConst.REWARD_PATH, EditorConst.REWARD_CREATION_MENU);
            menuTreeService.AddMenuToTree<WheelItem>(tree, EditorConst.ITEM_PATH, EditorConst.ITEM_CREATION_MENU);

            Debug.Log($"Menu items loaded: {tree.MenuItems.Count}");
            return tree;
        }

        protected override void OnBeginDrawEditors()
        {
            var selected = this.MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);

            if (selected != null)
            {
                GUILayout.Label(selected.Name, GUILayout.Width(300));
                menuStateController.UpdateState(selected);

                if (menuStateController.IsInMenu(EditorMenuTypes.BoxCreationMenu))
                {
                    boxCreationController.CreateToolbar(selected, menuStateController.GetCurrentState());
                }
                else if (menuStateController.IsInMenu(EditorMenuTypes.RewardCreationMenu))
                {
                    rewardCreationController.CreateToolbar(selected, menuStateController.GetCurrentState());
                }
                else if (menuStateController.IsInMenu(EditorMenuTypes.ItemCreationMenu))
                {
                    itemCreationController.CreateToolbar(selected, menuStateController.GetCurrentState());
                }
                else
                {
                    rewardCreationController.DefaultContent(selected);
                    boxCreationController.DefaultContent(selected);
                    itemCreationController.DefaultContent(selected);
                }
            }

            SirenixEditorGUI.EndHorizontalToolbar();
        }
    }
}
