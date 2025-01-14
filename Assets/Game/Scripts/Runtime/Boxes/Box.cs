using Game.Collectable;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Game.Boxes.Editor;


namespace Game.Boxes
{
    public class Box : SerializedScriptableObject
    {
        [HorizontalGroup("Box Settings", Width = 75)]
        [PreviewField(75)]
        [HideLabel]
        public Sprite boxSprite;

        [HorizontalGroup("Box Settings")]
        [VerticalGroup("Box Settings/Details")]
        [Title("Box Details")]
        [NameCheck(typeof(Box))]
        public string boxName;

        [HorizontalGroup("Box Settings")]
        [VerticalGroup("Box Settings/Details")]
        [Title("Box Description")]
        [TextArea(3, 5)]
        public string description;

        [BoxGroup("Contents")]
        [Searchable]
        [TableList(ShowIndexLabels = true)]
        public List<BoxContent> contents = new List<BoxContent>();

        public BoxEditorExtension editorExtension;
        private void OnEnable()
        {
            editorExtension = new BoxEditorExtension(this);
        }

#if UNITY_EDITOR
        public void UpdateContents()
        {
            for (int i = 0; i < contents.Count; i++)
            {
                var content = contents[i];
                if (content != null)
                {
                    content.Initialize(new BoxContent.Dependency(this, i));
                    EditorUtility.SetDirty(content);
                }
            }

            Debug.Log($"Box '{boxName}' contents updated. Count: {contents.Count}");
        }
#endif
    }
}