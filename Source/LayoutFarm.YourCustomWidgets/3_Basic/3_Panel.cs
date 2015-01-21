﻿// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

using LayoutFarm.UI;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm.CustomWidgets
{
    public enum PanelLayoutKind
    {
        Absolute,
        VerticalStack,
        HorizontalStack
    }

    public class Panel : UIBox
    {
        public event EventHandler<UIMouseEventArgs> MouseDown;
        public event EventHandler<UIMouseEventArgs> MouseUp;
        PanelLayoutKind panelLayoutKind;

        CustomRenderBox primElement;
        Color backColor = Color.LightGray;
        int viewportX;
        int viewportY;

        List<UICollection> layers = new List<UICollection>(1);

        public Panel(int width, int height)
            : base(width, height)
        {
            UICollection plainLayer = new UICollection(this);
            this.layers.Add(plainLayer);
        }


        protected override bool HasReadyRenderElement
        {
            get { return this.primElement != null; }
        }
        protected override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.primElement; }
        }
        public Color BackColor
        {
            get { return this.backColor; }
            set
            {
                this.backColor = value;
                if (HasReadyRenderElement)
                {
                    this.primElement.BackColor = value;
                }
            }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (primElement == null)
            {
                var renderE = new CustomRenderBox(rootgfx, this.Width, this.Height);
                renderE.SetController(this);

                renderE.SetLocation(this.Left, this.Top);
                renderE.BackColor = backColor;
                renderE.HasSpecificSize = true;

                //------------------------------------------------
                //create visual layer
                renderE.Layers = new VisualLayerCollection();
                int layerCount = this.layers.Count;
                for (int m = 0; m < layerCount; ++m)
                {
                    UICollection plain = (UICollection)this.layers[m];
                    var groundLayer = new PlainLayer(renderE);
                    renderE.Layers.AddLayer(groundLayer);
                    renderE.SetViewport(this.viewportX, this.viewportY);
                    //---------------------------------
                    int j = plain.Count;
                    for (int i = 0; i < j; ++i)
                    {
                        groundLayer.AddUI(plain.GetElement(i));
                    }
                }

                //---------------------------------
                primElement = renderE;
            }
            return primElement;
        }

        public PanelLayoutKind PanelLayoutKind
        {
            get { return this.panelLayoutKind; }
            set
            {
                this.panelLayoutKind = value;
            }
        }
        public void AddChildBox(UIElement ui)
        {
            UICollection layer0 = (UICollection)this.layers[0];
            layer0.AddUI(ui);
            if (this.HasReadyRenderElement)
            {
                PlainLayer plain1 = this.primElement.Layers.Layer0 as PlainLayer;
                plain1.AddUI(ui);
                if (this.panelLayoutKind != PanelLayoutKind.Absolute)
                {
                    this.InvalidateLayout();
                }
            }
        }
        public void RemoveChildBox(UIElement ui)
        {
            UICollection layer0 = (UICollection)this.layers[0];
            layer0.RemoveUI(ui);
            if (this.HasReadyRenderElement)
            {
                PlainLayer plain1 = this.primElement.Layers.Layer0 as PlainLayer;
                plain1.RemoveUI(ui);
                if (this.panelLayoutKind != PanelLayoutKind.Absolute)
                {
                    this.InvalidateLayout();
                }
            }
        }
        //----------------------------------------------------
        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            if (this.MouseDown != null)
            {
                this.MouseDown(this, e);
            }
        }
        protected override void OnMouseUp(UIMouseEventArgs e)
        {
            if (this.MouseUp != null)
            {
                MouseUp(this, e);
            }
            base.OnMouseUp(e);
        }

        public override int ViewportX
        {
            get { return this.viewportX; }

        }
        public override int ViewportY
        {
            get { return this.viewportY; }

        }
        public override void SetViewport(int x, int y)
        {
            this.viewportX = x;
            this.viewportY = y;
            if (this.HasReadyRenderElement)
            {
                primElement.SetViewport(viewportX, viewportY);

            }
        }
        protected override void OnContentLayout()
        {
            this.PerformContentLayout();
        }
        public override void PerformContentLayout()
        {
            //temp : arrange as vertical stack***
            switch (this.PanelLayoutKind)
            {
                case CustomWidgets.PanelLayoutKind.VerticalStack:
                    {
                        UICollection layer0 = (UICollection)this.layers[0];
                        int count = layer0.Count;
                        int ypos = 0;
                        for (int i = 0; i < count; ++i)
                        {
                            var element = layer0.GetElement(i) as UIBox;
                            if (element != null)
                            {
                                element.PerformContentLayout();
                                element.SetBounds(0, ypos, element.Width, element.DesiredHeight);
                                ypos += element.DesiredHeight;
                            }
                        }
                        this.desiredHeight = ypos;
                    } break;
                default:
                    {
                    } break;
            }

        }
        public override int DesiredHeight
        {
            get
            {
                return this.desiredHeight;
            }
        }
        //temp***
        int desiredHeight;

    }



}