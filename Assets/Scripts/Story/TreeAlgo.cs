using System;
using System.Collections.Generic;
using System.Linq;

namespace TreeAlgorithms
{
    public class Model
    {
        public string Name;
    }
    public class ScreenTreeNode<T>
    {
        //Node base
        public T Element;
        public ScreenTreeNode<T> Parent;
        public List<ScreenTreeNode<T>> Children = new();

        //Tree positioning for Reingold-Tilford Algorithm
        public float X;
        public int Y;
        public float Mod;
        public int Height;
        public float Width;
    }
    public static class TreeNodeExtensions
    {
        // Check if the node is the leftmost child
        public static bool IsLeftmost<T>(this ScreenTreeNode<T> node)
        {
            return node.Parent == null || node.Parent.Children.Count == 0 || node.Parent.Children[0] == node;
        }

        // Check if the node is the rightmost child
        public static bool IsRightmost<T>(this ScreenTreeNode<T> node)
        {
            return node.Parent == null || node.Parent.Children.Count == 0 || node.Parent.Children[^1] == node;
        }

        // Check if the node is a leaf (i.e., has no children)
        public static bool IsLeaf<T>(this ScreenTreeNode<T> node)
        {
            return node.Children == null || node.Children.Count == 0;
        }

        // Get the leftmost child of the node
        public static ScreenTreeNode<T> GetLeftmostChild<T>(this ScreenTreeNode<T> node)
        {
            if (node.Children == null || node.Children.Count == 0) return null;
            return node.Children[0];
        }

        // Get the rightmost child of the node
        public static ScreenTreeNode<T> GetRightmostChild<T>(this ScreenTreeNode<T> node)
        {
            if (node.Children == null || node.Children.Count == 0) return null;
            return node.Children[^1];
        }

        // Get the left sibling of the node
        public static ScreenTreeNode<T> GetLeftSibling<T>(this ScreenTreeNode<T> node)
        {
            if (node.Parent == null || node.IsLeftmost()) return null;
            return node.Parent.Children[node.Parent.Children.IndexOf(node) - 1];
        }

        // Get the right sibling of the node
        public static ScreenTreeNode<T> GetRightSibling<T>(this ScreenTreeNode<T> node)
        {
            if (node.Parent == null || node.IsRightmost()) return null;
            return node.Parent.Children[node.Parent.Children.IndexOf(node) + 1];
        }

        // Get the leftmost sibling of the node
        public static ScreenTreeNode<T> GetLeftmostSibling<T>(this ScreenTreeNode<T> node)
        {
            if (node.Parent == null) return null;
            return node.Parent.Children[0]; // Always return the first child of the parent
        }
    }

    #region 3rdParty
    // ALGORITHM CODE FROM: https://github.com/paulpach/DrawTree
    public static class ScreenTreeNodeHelper
    {
        private static int nodeSize = 1;
        private static float siblingDistance = 0.0F;
        private static float treeDistance = 0.0F;

        public static void CalculateNodePositions<T>(ScreenTreeNode<T> rootNode)
        {
            // initialize node x, y, and mod values
            InitializeNodes(rootNode, 0);

            // assign initial X and Mod values for nodes
            CalculateInitialX(rootNode);

            // ensure no node is being drawn off screen
            CheckAllChildrenOnScreen(rootNode);

            // assign final X values to nodes
            CalculateFinalPositions(rootNode, 0);
        }

        // recusrively initialize x, y, and mod values of nodes
        private static void InitializeNodes<T>(ScreenTreeNode<T> node, int depth)
        {
            node.X = -1;
            node.Y = depth;
            node.Mod = 0;

            foreach (var child in node.Children)
                InitializeNodes(child, depth + 1);
        }

        private static void CalculateFinalPositions<T>(ScreenTreeNode<T> node, float modSum)
        {
            node.X += modSum;
            modSum += node.Mod;

            foreach (var child in node.Children)
                CalculateFinalPositions(child, modSum);

            if (node.Children.Count == 0)
            {
                node.Width = node.X;
                node.Height = node.Y;
            }
            else
            {
                node.Width = node.Children.OrderByDescending(p => p.Width).First().Width;
                node.Height = node.Children.OrderByDescending(p => p.Height).First().Height;
            }
        }

        private static void CalculateInitialX<T>(ScreenTreeNode<T> node)
        {
            foreach (var child in node.Children)
                CalculateInitialX(child);

            // if no children
            if (node.IsLeaf())
            {
                // if there is a previous sibling in this set, set X to prevous sibling + designated distance
                if (!node.IsLeftmost())
                    node.X = node.GetLeftSibling().X + nodeSize + siblingDistance;
                else
                    // if this is the first node in a set, set X to 0
                    node.X = 0;
            }
            // if there is only one child
            else if (node.Children.Count == 1)
            {
                // if this is the first node in a set, set it's X value equal to it's child's X value
                if (node.IsLeftmost())
                {
                    node.X = node.Children[0].X;
                }
                else
                {
                    node.X = node.GetLeftSibling().X + nodeSize + siblingDistance;
                    node.Mod = node.X - node.Children[0].X;
                }
            }
            else
            {
                var leftChild = node.GetLeftmostChild();
                var rightChild = node.GetRightmostChild();
                var mid = (leftChild.X + rightChild.X) / 2;

                if (node.IsLeftmost())
                {
                    node.X = mid;
                }
                else
                {
                    node.X = node.GetLeftSibling().X + nodeSize + siblingDistance;
                    node.Mod = node.X - mid;
                }
            }

            if (node.Children.Count > 0 && !node.IsLeftmost())
            {
                // Since subtrees can overlap, check for conflicts and shift tree right if needed
                CheckForConflicts(node);
            }

        }

        private static void CheckForConflicts<T>(ScreenTreeNode<T> node)
        {
            var minDistance = treeDistance + nodeSize;
            var shiftValue = 0F;

            var nodeContour = new Dictionary<int, float>();
            GetLeftContour(node, 0, ref nodeContour);

            var sibling = node.GetLeftmostSibling();
            while (sibling != null && sibling != node)
            {
                var siblingContour = new Dictionary<int, float>();
                GetRightContour(sibling, 0, ref siblingContour);

                for (int level = node.Y + 1; level <= Math.Min(siblingContour.Keys.Max(), nodeContour.Keys.Max()); level++)
                {
                    var distance = nodeContour[level] - siblingContour[level];
                    if (distance + shiftValue < minDistance)
                    {
                        shiftValue = Math.Max(minDistance - distance, shiftValue);
                    }
                }

                if (shiftValue > 0)
                {

                    CenterNodesBetween(node, sibling);

                }

                sibling = sibling.GetRightSibling();
            }

            if (shiftValue > 0)
            {
                node.X += shiftValue;
                node.Mod += shiftValue;
                shiftValue = 0;

            }
        }

        private static void CenterNodesBetween<T>(ScreenTreeNode<T> leftNode, ScreenTreeNode<T> rightNode)
        {
            var leftIndex = leftNode.Parent.Children.IndexOf(leftNode);
            var rightIndex = leftNode.Parent.Children.IndexOf(rightNode);

            var numNodesBetween = (rightIndex - leftIndex) - 1;

            if (numNodesBetween > 0)
            {
                var distanceBetweenNodes = (leftNode.X - rightNode.X) / (numNodesBetween + 1);

                int count = 1;
                for (int i = leftIndex + 1; i < rightIndex; i++)
                {
                    var middleNode = leftNode.Parent.Children[i];

                    var desiredX = rightNode.X + (distanceBetweenNodes * count);
                    var offset = desiredX - middleNode.X;
                    middleNode.X += offset;
                    middleNode.Mod += offset;

                    count++;
                }

                CheckForConflicts(leftNode);
            }
        }

        private static void CheckAllChildrenOnScreen<T>(ScreenTreeNode<T> node)
        {
            var nodeContour = new Dictionary<int, float>();
            GetLeftContour(node, 0, ref nodeContour);

            float shiftAmount = 0;
            foreach (var y in nodeContour.Keys)
            {
                if (nodeContour[y] + shiftAmount < 0)
                    shiftAmount = (nodeContour[y] * -1);
            }

            if (shiftAmount > 0)
            {
                node.X += shiftAmount;
                node.Mod += shiftAmount;
            }
        }

        private static void GetLeftContour<T>(ScreenTreeNode<T> node, float modSum, ref Dictionary<int, float> values)
        {
            if (!values.ContainsKey(node.Y))
                values.Add(node.Y, node.X + modSum);
            else
                values[node.Y] = Math.Min(values[node.Y], node.X + modSum);

            modSum += node.Mod;
            foreach (var child in node.Children)
            {
                GetLeftContour(child, modSum, ref values);
            }
        }

        private static void GetRightContour<T>(ScreenTreeNode<T> node, float modSum, ref Dictionary<int, float> values)
        {
            if (!values.ContainsKey(node.Y))
                values.Add(node.Y, node.X + modSum);
            else
                values[node.Y] = Math.Max(values[node.Y], node.X + modSum);

            modSum += node.Mod;
            foreach (var child in node.Children)
            {
                GetRightContour(child, modSum, ref values);
            }
        }
    }
    #endregion
}