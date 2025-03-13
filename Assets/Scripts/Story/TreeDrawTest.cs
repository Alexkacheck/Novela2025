using TreeAlgorithms;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;

namespace TestDrawer
{
    public class SimpleModel
    {
        public string Name { get; set; }
    }

    public class TreeDrawer : MonoBehaviour
    {
        public int NODE_HEIGHT = 30;
        public int NODE_WIDTH = 30;
        public int NODE_MARGIN_X = 50;
        public int NODE_MARGIN_Y = 40;

        public GameObject nodePrefab;
        public Transform parent;

        private ScreenTreeNode<SimpleModel> _tree;

        private void Start()
        {
            GenerateSampleTree();

            // Calculate node positions
            TreeAlgorithms.ScreenTreeNodeHelper.CalculateNodePositions(_tree);
        }

        private void GenerateSampleTree()
        {
            // Root node
            _tree = new ScreenTreeNode<SimpleModel>();

            // Left node
            var left = new ScreenTreeNode<SimpleModel>() { Parent = _tree };

            // Add children to the left node
            var leftChild1 = new ScreenTreeNode<SimpleModel>() { Parent = left };
            var leftChild2 = new ScreenTreeNode<SimpleModel>() { Parent = left };

            // Further nesting under leftChild2
            var leftChild2a = new ScreenTreeNode<SimpleModel>() { Parent = leftChild2 };
            var leftChild2b = new ScreenTreeNode<SimpleModel>() { Parent = leftChild2 };

            leftChild2.Children.Add(leftChild2a);
            leftChild2.Children.Add(leftChild2b);

            left.Children.Add(leftChild1);
            left.Children.Add(leftChild2);

            // Right node
            var right = new ScreenTreeNode<SimpleModel>() { Parent = _tree };

            // Add children to the right node
            var rightChild1 = new ScreenTreeNode<SimpleModel>() { Parent = right };
            var rightChild2 = new ScreenTreeNode<SimpleModel>() { Parent = right };

            // Further nesting under rightChild2
            var rightChild2a = new ScreenTreeNode<SimpleModel>() { Parent = rightChild2 };
            var rightChild2b = new ScreenTreeNode<SimpleModel>() { Parent = rightChild2 };

            rightChild2.Children.Add(rightChild2a);
            rightChild2.Children.Add(rightChild2b);

            right.Children.Add(rightChild1);
            right.Children.Add(rightChild2);

            // Adding more complexity with additional branches under both left and right
            var leftChild3 = new ScreenTreeNode<SimpleModel>() { Parent = left };
            var rightChild3 = new ScreenTreeNode<SimpleModel>() { Parent = right };

            left.Children.Add(leftChild3);
            right.Children.Add(rightChild3);

            // Adding some children under the newly created branches
            var leftChild3a = new ScreenTreeNode<SimpleModel>() { Parent = leftChild3 };
            var rightChild3a = new ScreenTreeNode<SimpleModel>() { Parent = rightChild3 };

            leftChild3.Children.Add(leftChild3a);
            rightChild3.Children.Add(rightChild3a);

            // Add the left and right subtrees to the root
            _tree.Children.Add(left);
            _tree.Children.Add(right);
        }

        [Button, DisableInEditorMode]
        public void DrawTree()
        {
            DrawRecursively(_tree);
        }

        private void DrawRecursively<T>(ScreenTreeNode<T> data)
        {
            CreateNode(data);

            if (data.Children != null && data.Children.Any())
            {
                foreach (var child in data.Children)
                {
                    DrawRecursively(child);
                }
            }

            Debug.Log($"{data.Y}:{data.X}:{data.Width}");
        }

        private GameObject CreateNode<T>(ScreenTreeNode<T> data)
        {
            return Instantiate(nodePrefab, new Vector3((data.X * NODE_WIDTH) + NODE_MARGIN_X, (-data.Y * NODE_HEIGHT) + NODE_MARGIN_Y, 0), Quaternion.identity, parent);
        }
    }
}