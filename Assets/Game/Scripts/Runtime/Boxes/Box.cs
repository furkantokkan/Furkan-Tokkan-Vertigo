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
        [BoxGroup("Box Visuals")]
        [HorizontalGroup("Box Visuals/Row", Width = 75, MarginRight = 5)]
        [PreviewField(75)]
        [Title("Box Sprite")]
        public Sprite boxSprite;

        [BoxGroup("Box Visuals")]
        [HorizontalGroup("Box Visuals/Row", Width = 75)]
        [PreviewField(75)]
        [Title("Background")]
        public Sprite background;

        [BoxGroup("Wheel Types")]
        [HorizontalGroup("Wheel Types/Row", Width = 75, MarginRight = 5)]
        [PreviewField(75)]
        [Title("Default")]
        public Sprite defaultWheel;

        [BoxGroup("Wheel Types")]
        [HorizontalGroup("Wheel Types/Row", Width = 75, MarginRight = 5)]
        [PreviewField(75)]
        [Title("Safe")]
        public Sprite safeWheel;

        [BoxGroup("Wheel Types")]
        [HorizontalGroup("Wheel Types/Row", Width = 75)]
        [PreviewField(75)]
        [Title("Super")]
        public Sprite superWheel;

        [BoxGroup("Box Information")]
        [Title("Box Details")]
        [NameCheck(typeof(Box))]
        public string boxName;

        [BoxGroup("Box Information")]
        [Title("Box Description")]
        [TextArea(3, 5)]
        public string description;

        [BoxGroup("Contents")]
        [Searchable]
        [TableList(ShowIndexLabels = true)]
        [ListDrawerSettings(ShowFoldout = true)]
        [InfoBox("Add box contents here. Each content represents a wave.", InfoMessageType.Info)]
        public List<BoxContent> contents = new List<BoxContent>();

#if UNITY_EDITOR

        [SerializeField]
        private BoxEditorExtension editorExtension;

        private void OnEnable()
        {
            editorExtension = new BoxEditorExtension(this);
        }

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