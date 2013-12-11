using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace System.Xml
{
    /// <summary>
    /// 扩展XmlNode的方法
    /// </summary>
    public static class XmlNodeExtend
    {
        /// <summary>
        /// 根据名称和属性，获取后辈节点列表
        /// 小心：查找范围不是以当前节点为”根节点”的树
        /// </summary>
        public static XmlNodeList GetChildNodes(this XmlNode node, string nodeName, string attributeName, string attributeValue)
        {
            return node.SelectNodes(string.Format("//{1}[@{0}='{1}']", attributeName, attributeValue, nodeName));
        }

        /// <summary>
        /// 根据名称和属性，获取后辈节点
        /// 小心：查找范围不是以当前节点为”根节点”的树
        /// </summary>
        public static XmlNode GetChildNode(this XmlNode node, string nodeName, string attributeName, string attributeValue)
        {
            return node.SelectSingleNode(string.Format("//{2}[@{0}='{1}']", attributeName, attributeValue, nodeName));
        }

        /// <summary>
        /// 将自己从节点树中移除
        /// </summary>
        public static void RemoveSelf(this XmlNode node)
        {
            node.ParentNode.RemoveChild(node);
        }

        /// <summary>
        /// 是否包含该属性
        /// </summary>
        public static bool HasAttribute(this XmlNode node, string attributeName)
        {
            XmlElement element = node as XmlElement;
            return element.HasAttribute(attributeName);
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        public static string GetAttribute(this XmlNode node, string attributeName)
        {
            XmlElement element = node as XmlElement;
            if (element.HasAttribute(attributeName))
            {
                return element.GetAttribute(attributeName);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        public static void SetAttribute(this XmlNode node, string attributeName, string attributeValue)
        {
            XmlElement element = node as XmlElement;
            element.SetAttribute(attributeName, attributeValue);
        }

        /// <summary>
        /// 移除属性
        /// </summary>
        public static void RemoveAttribute(this XmlNode node, string attributeName)
        {
            XmlElement element = node as XmlElement;
            node.Attributes.Remove(element.GetAttributeNode(attributeName));
        }

        /// <summary>
        /// 获取符合条件的祖先节点
        /// </summary>
        public static XmlNode GetAncestorNode(this XmlNode node, Func<XmlNode, bool> condition)
        {
            XmlNode currentNode = node;
            // 不符合条件，继续寻找
            while (!condition(currentNode) && currentNode.ParentNode != null)
            {
                currentNode = currentNode.ParentNode;
            }

            // 验证查找结果
            if (!condition(currentNode))
            {
                return null;
            }

            return currentNode;
        }

        /// <summary>
        /// 获取符合条件的嫡长孙节点
        /// </summary>
        public static XmlNode GetEldestSonNode(this XmlNode node, Func<XmlNode, bool> condition)
        {
            XmlNode currentNode = node;
            // 不符合条件，继续寻找
            while (!condition(currentNode) && currentNode.FirstChild != null)
            {
                currentNode = currentNode.FirstChild;
            }

            // 验证查找结果
            if (!condition(currentNode))
            {
                return null;
            }

            return currentNode;
        }

        /// <summary>
        /// 从当前节点上溯，直到找到符合要求的祖先节点
        /// </summary>
        /// <param name="node">当前节点</param>
        /// <param name="predicate">要执行的操作，和判断成功的条件</param>
        /// <param name="stopOnTrue">一旦执行成功/失败，停止上溯（为null时，不停止）</param>
        public static void Recursion(this XmlNode node, Func<XmlNode, bool> predicate, bool? stopOnTrue = true)
        {
            XmlNode currentNode = node;
            while (currentNode != null)
            {
                bool result = predicate(node);
                if (stopOnTrue != null)
                {
                    if (!(bool)stopOnTrue ^ result)
                    {
                        break;
                    }
                }

                currentNode = currentNode.ParentNode;
            }
        }

        /// <summary>
        /// 遍历节点树，对每个节点执行predicate操作（中序遍历算法）
        /// </summary>
        /// <param name="node"></param>
        /// <param name="predicate">要执行的操作，和判断成功的条件</param>
        /// <param name="stopOnTrue">一旦执行成功/失败，停止向子节点遍历（为null时，不停止）</param>
        public static void Iterate(this XmlNode node, Func<XmlNode, bool> predicate, bool? stopOnTrue = true)
        {
            if (node == null)
            {
                return;
            }

            bool result = predicate(node);
            if (stopOnTrue != null)
            {
                if (!(bool)stopOnTrue ^ result)
                {
                    return;
                }
            }

            List<XmlNode> childNodes = new List<XmlNode>();
            int childNodesCount = node.ChildNodes.Count;
            foreach (XmlNode childNode in node.ChildNodes)
            {
                childNodes.Add(childNode);
            }
            for (int i = 0; i < childNodesCount; i++)
            {
                childNodes[i].Iterate(predicate);
            }

        }

        /// <summary>
        /// 将节点保存到文件
        /// </summary>
        public static void Save(this XmlNode node, string filePhysicalPath)
        {
            using (XmlWriter writer = XmlWriter.Create(filePhysicalPath))
            {
                node.WriteTo(writer);
            }
        }
    }
}