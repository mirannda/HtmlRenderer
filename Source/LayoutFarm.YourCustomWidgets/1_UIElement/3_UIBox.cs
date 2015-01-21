﻿// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;


namespace LayoutFarm.UI
{
    public abstract class UIBox : UIElement
    {
        int _left;
        int _top;
        int _width;
        int _height;
        bool _hide;

#if DEBUG
        static int dbugTotalId;
        public readonly int dbugId = dbugTotalId++;
#endif
        public UIBox(int width, int height)
        {
            this._width = width;
            this._height = height;
        }

        public void SetLocation(int left, int top)
        {
            this._left = left;
            this._top = top;

            if (this.HasReadyRenderElement)
            {
                this.CurrentPrimaryRenderElement.SetLocation(left, top);
            }
        }
        public virtual void SetSize(int width, int height)
        {
            this._width = width;
            this._height = height;

            if (this.HasReadyRenderElement)
            {
                this.CurrentPrimaryRenderElement.SetSize(_width, _height);
            }
        }
        public void SetBounds(int left, int top, int width, int height)
        {
            SetLocation(left, top);
            SetSize(width, height);
        }
      

       

        public int Left
        {
            get
            {

                if (this.HasReadyRenderElement)
                {
                    return this.CurrentPrimaryRenderElement.X;
                }
                else
                {
                    return this._left;
                }
            }
        }
        public int Top
        {
            get
            {
                if (this.HasReadyRenderElement)
                {
                    return this.CurrentPrimaryRenderElement.Y;
                }
                else
                {
                    return this._top;
                }
            }
        }
        public Point Position
        {
            get
            {
                if (this.HasReadyRenderElement)
                {
                    return new Point(CurrentPrimaryRenderElement.X, CurrentPrimaryRenderElement.Y);
                }
                else
                {
                    return new Point(this._left, this._top);
                }
            }
        }
        public int Width
        {
            get
            {
                if (this.HasReadyRenderElement)
                {
                    return this.CurrentPrimaryRenderElement.Width;
                }
                else
                {
                    return this._width;
                }
            }
        }
        public int Height
        {
            get
            {
                if (this.HasReadyRenderElement)
                {
                    return this.CurrentPrimaryRenderElement.Height;
                }
                else
                {
                    return this._height;
                }
            }
        }

        public override void InvalidateGraphics()
        {
            if (this.HasReadyRenderElement)
            {
                this.CurrentPrimaryRenderElement.InvalidateGraphics();
            }
        }
        public virtual int ViewportX
        {
            get { return 0; }
        }
        public virtual int ViewportY
        {
            get { return 0; }
        }
        public virtual void SetViewport(int x, int y)
        {

        }

        public bool Visible
        {
            get { return !this._hide; }
            set
            {
                this._hide = !value;
                if (this.HasReadyRenderElement)
                {
                    this.CurrentPrimaryRenderElement.SetVisible(value);
                }
            }
        }

        public virtual void PerformContentLayout()
        {
        }
        public virtual int DesiredHeight
        {
            get { return this.Height; }
        }
        public virtual int DesiredWidth
        {
            get { return this.Width; }
        }


        //-----------------------------------

        
    }

}