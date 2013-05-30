using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace WebSite
{
    /// <summary>
    /// 根据（导航节点，当前位置），获取用户的各种形式导航信息
    /// 思路：在一张siteMap地图中,url是坐标；
    /// 通过坐标定位，获取地图中的省(MainMenu)，以及当前在哪个省(selected);
    ///               获取该省下的市县结构(ControlPanel)，以及当前在哪个县(selected);
    /// </summary>
    public class Navigation
    {
        /// <summary>
        /// 根节点
        /// </summary>
        public XmlNode RootNode { get; private set; }

        /// <summary>
        /// 当前位置
        /// </summary>
        public string CurrentUrl { get; set; }

        /// <summary>
        /// 当前节点
        /// </summary>
        public XmlNode CurrentNode
        {
            get
            {
                return RootNode.GetChildNode("siteMapNode", "url", CurrentUrl);
            }
        }

        /// <summary>
        /// 当前级别
        /// </summary>
        public int CurrentLevel
        {
            get { return int.Parse(CurrentNode.GetAttribute("description")); }
        }

        public Navigation(XmlNode rootNode)
        {
            RootNode = rootNode;
        }

        /// <summary>
        /// 面包屑导航
        /// </summary>
        public List<LinkViewModel> Breadcrumb
        {
            get
            {
                List<LinkViewModel> linkList = new List<LinkViewModel>();

                XmlNode node = CurrentNode;
                while (node != null && node != RootNode)
                {
                    linkList.Add(new LinkViewModel
                    {
                        Text = node.GetAttribute("title"),
                        //Url = node.GetAttribute("url"),
                    });
                    node = node.ParentNode;
                }

                linkList.Reverse();
                return linkList;
            }
        }

        /// <summary>
        /// 一级主菜单
        /// </summary>
        public List<LinkViewModel> MainMenu
        {
            get
            {
                List<LinkViewModel> linkList = new List<LinkViewModel>();
                XmlNode level_1_selectedNode = GetLevel_N_SelectedNode(1);
                XmlNodeList mainMenuNodeList = RootNode.ChildNodes;
                foreach (XmlNode node in mainMenuNodeList)
                {
                    linkList.Add(new LinkViewModel
                    {
                        Text = node.GetAttribute("title"),
                        Url = node.GetEldestSonNode(n => n.HasAttribute("url")).GetAttribute("url"),
                        State = (level_1_selectedNode == node) ? LinkState.active : LinkState.common
                    });
                }

                return linkList;
            }
        }

        /// <summary>
        /// 控制面板（二、三级菜单）
        /// </summary>
        public Dictionary<string, List<LinkViewModel>> ControlPanel
        {
            get
            {
                Dictionary<string, List<LinkViewModel>> linkGroup = new Dictionary<string, List<LinkViewModel>>();
                XmlNode level_1_selectedNode = GetLevel_N_SelectedNode(1);
                XmlNode level_2_selectedNode = GetLevel_N_SelectedNode(2);
                XmlNode level_3_selectedNode = GetLevel_N_SelectedNode(3);
                XmlNodeList groupNodeList = level_1_selectedNode.ChildNodes;

                if (level_2_selectedNode == null)
                {
                    // 只有一级菜单
                }
                else if (level_3_selectedNode == null)
                {
                    // 有两级菜单
                    string groupName = level_1_selectedNode.GetAttribute("title");
                    List<LinkViewModel> linkList = new List<LinkViewModel>();
                    foreach (XmlNode level_2_node in groupNodeList)
                    {
                        linkList.Add(new LinkViewModel
                        {
                            Text = level_2_node.GetAttribute("title"),
                            Url = level_2_node.GetAttribute("url"),
                            State = (level_2_selectedNode == level_2_node) ? LinkState.active : LinkState.common
                        });
                    }
                    linkGroup.Add(groupName, linkList);
                }
                else
                {
                    // 有三级菜单
                    foreach (XmlNode groupNode in groupNodeList)
                    {
                        string groupName = groupNode.GetAttribute("title");
                        List<LinkViewModel> linkList = new List<LinkViewModel>();
                        foreach (XmlNode linkNode in groupNode.ChildNodes)
                        {
                            linkList.Add(new LinkViewModel
                            {
                                Text = linkNode.GetAttribute("title"),
                                Url = linkNode.GetAttribute("url"),
                                State = (level_3_selectedNode == linkNode) ? LinkState.active : LinkState.common
                            });
                        }
                        linkGroup.Add(groupName, linkList);
                    }
                }

                return linkGroup;
            }
        }


        #region Private Methods

        private XmlNode GetLevel_N_SelectedNode(int level)
        {
            return CurrentNode.GetAncestorNode(n => n.GetAttribute("description") == level.ToString());
        }

        #endregion
    }

}
