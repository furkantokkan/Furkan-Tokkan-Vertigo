using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

namespace Game.Editor
{
    public static class EditorConst 
    {
        public const string BOX_CREATION_MENU = "List of All Boxes";
        public const string REWARD_CREATION_MENU = "List of All Rewards";
        public const string ITEM_CREATION_MENU = "List of All Items";

        public const string BOX_TITLE = "Box";
        public const string Reward_TITLE = "Rewad";
        public const string ITEM_TITLE = "Item";

        public const string REWARD_TYPE_NAME = "RewardType";
        public const string REWARD_TYPE_PATH = "Assets/Game/Scripts/Runtime/Collectable/Rewards";

        public const string ITEM_TYPE_NAME = "ItemType";
        public const string ITEM_TYPE_PATH = "Assets/Game/Scripts/Runtime/Collectable/Items";

        public const string BOX_PATH = "Assets/Game/ScriptableObjects/Boxes";
        public const string REWARD_PATH = "Assets/Game/ScriptableObjects/Rewards";
        public const string ITEM_PATH = "Assets/Game/ScriptableObjects/Items";
    }
}
