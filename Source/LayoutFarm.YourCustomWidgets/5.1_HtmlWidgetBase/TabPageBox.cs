﻿// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.WebDom;
using LayoutFarm.WebDom.Extension;
using LayoutFarm.UI;
using LayoutFarm.RenderBoxes;
using LayoutFarm.CustomWidgets;
namespace LayoutFarm.HtmlWidgets
{
    //tab page similar to listview

    public class TabPageContainer : LightHtmlWidgetBase
    {
        DomElement pnode;
        DomElement titleBar;
        DomElement contentNode;
        DomElement tabTitleList;

        Color backColor = Color.LightGray;
        List<TabPage> tabPageCollection = new List<TabPage>();

        TabPage currentPage;
        int currentSelectedIndex;

        public TabPageContainer(int width, int height)
            : base(width, height)
        {
        }
        public List<TabPage> TabPageList
        {
            get { return this.tabPageCollection; }
        }
        public override WebDom.DomElement GetPresentationDomNode(WebDom.DomElement hostNode)
        {
            if (pnode != null) return pnode;
            //------------------------------
            var ownerdoc = hostNode.OwnerDocument;
            pnode = ownerdoc.CreateElement("div");
            pnode.SetAttribute("style", "font:10pt tahoma");
            //------------------------------ 
            titleBar = ownerdoc.CreateElement("div");
            titleBar.AddTextContent("hello tabPage");
            pnode.AddChild(titleBar);
            //------------------------------ 
            tabTitleList = ownerdoc.CreateElement("div");
            pnode.AddChild(tabTitleList);
            //------------------------------ 
            contentNode = ownerdoc.CreateElement("div");
            pnode.AddChild(contentNode);
            //------------------------------
            return pnode;
        }
        public void AddItem(TabPage tabPage)
        {
            //1. store in collection

            tabPageCollection.Add(tabPage);
            tabPage.OwnerContainer = this;

            if (pnode != null)
            {
                if (currentPage == null)
                {
                    currentPage = tabPage;
                    //add tab button into list
                    this.tabTitleList.AddChild(tabPage.GetTitleNode(pnode));
                    //add page body
                    contentNode.AddChild(tabPage.GetPageBody(pnode));
                }
                else
                {
                    //add tab button into list
                    this.tabTitleList.AddChild(tabPage.GetTitleNode(pnode));
                }
            }
        }
        public void RemoveItem(TabPage p)
        {
            //p.Owner = null;
            //tabPageCollection.Remove(p);
            //panel.RemoveChildBox(p);
        }
        public void ClearPages()
        {
            //TODO: implement this
        }
        public int SelectedIndex
        {
            get { return this.currentSelectedIndex; }
            set
            {
                if (value > -1 && value < this.tabPageCollection.Count
                    && this.currentSelectedIndex != value)
                {
                    this.currentSelectedIndex = value;
                    TabPage selectednedSelectedPage = this.tabPageCollection[value];
                    //if (currentPage != null)
                    //{
                    //    this.panel.RemoveChildBox(currentPage);
                    //}
                    //this.currentPage = selectednedSelectedPage;
                    //this.panel.AddChildBox(currentPage);
                }
            }
        }
        //------------------------
        internal void ChildNotifyTabMouseDown(TabPage childPage)
        {
            //change content ***
            contentNode.ClearAllElements();
            contentNode.AddChild(childPage.GetPageBody(contentNode));

        }
    }


    public class TabPage
    {
        DomElement titleNode;
        DomElement contentNode;
        UIElement contentUI;
        public TabPage()
        {

        }
        public string PageTitle
        {
            get;
            set;
        }
        internal TabPageContainer OwnerContainer
        {
            get;
            set;
        }
        public DomElement GetTitleNode(DomElement hostNode)
        {
            //-------------------------------------
            if (titleNode != null) return titleNode;
            //create dom node
            var ownerdoc = hostNode.OwnerDocument;
            this.titleNode = ownerdoc.CreateElement("div");
            titleNode.SetAttribute("style", "display:inline");
            titleNode.AddChild("span", span =>
            {
                if (PageTitle == null)
                {
                    span.AddTextContent("");
                }
                else
                {
                    span.AddTextContent(this.PageTitle);
                }
                span.AttachMouseDownEvent(e =>
                {
                    if (this.OwnerContainer != null)
                    {
                        this.OwnerContainer.ChildNotifyTabMouseDown(this);
                    }
                });
            });
            ////mouse down on title
            //titleNode.AttachMouseDownEvent(e =>
            //{


            //});
            //-------------------------------------
            return titleNode;
        }
        public DomElement GetPageBody(DomElement hostNode)
        {
            if (contentNode != null) return contentNode;
            var ownerdoc = hostNode.OwnerDocument;
            contentNode = ownerdoc.CreateElement("div");
            if (this.contentUI != null)
            {
                //add content ui to the body of page
                //creat html wrapper for this ...        

                HtmlDocument htmldoc = (HtmlDocument)ownerdoc;
                var wrapperElement = htmldoc.CreateWrapperElement((RootGraphic rootgfx, out RenderElement renderE, out object controller) =>
                {
                    renderE = contentUI.GetPrimaryRenderElement(rootgfx);
                    controller = contentUI;

                });
                contentNode.AddChild(wrapperElement);

            }
            return contentNode;
        }
        public UIElement ContentUI
        {
            get { return this.contentUI; }
            set
            {
                this.contentUI = value;
                //add ui to content node if 
                if (this.contentNode != null)
                {
                }

            }
        }


    }
}