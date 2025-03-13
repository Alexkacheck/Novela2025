using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;


public class IndependentTreeDrawer<Story, Prefab> : MonoBehaviour where Prefab : Component
{
    private class TreeNodeInternal<S>
    {
        public S story;
        public List<TreeNodeInternal<S>> children = new();
        public float x;
        public float y;
        public float mod;
    }

    private TreeNodeInternal<Story> rootNode;
    public ChapterView nodePrefab;
    public RectTransform panel;
    public float horizontalSpacing = 50f;
    public float verticalSpacing = 100f;

    public IndependentTreeDrawer(Func<Story> GetChildNodeFunc, Action<Prefab, Story> InitPrefabCallback)
    {
        
    }

    [Button]
    public void GenerateTree()
    {
    }

    private void Init(ChapterView chapterView, StoryNode storyNode)
    {

    }
}