﻿// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

using LayoutFarm.RenderBoxes;
namespace LayoutFarm
{


    partial class RenderElement
    {
        public void SetWidth(int width)
        {
            this.SetSize(width, this.b_height);
        }
        public void SetHeight(int height)
        {
            this.SetSize(this.b_width, height);
        }
        public void SetSize(int width, int height)
        {
            if (parentLink == null)
            {
                //direct set size
                this.b_width = width;
                this.b_height = height;
            }
            else
            {

                var prevBounds = this.RectBounds;

                this.b_width = width;
                this.b_height = height;

                //combine before and after rect 
                //add to invalidate root invalidate queue  
                this.InvalidateGraphicBounds(Rectangle.Union(prevBounds, this.RectBounds));
            }
        }
        
        public void SetLocation(int left, int top)
        {
            if (parentLink == null)
            {
                this.b_left = left;
                this.b_top = top;
            }
            else
            {
                //set location not affect its content size 

                var prevBounds = this.RectBounds;
                //----------------
                
                this.b_left = left;
                this.b_top = top;
                //----------------   
                //combine before and after rect  
                //add to invalidate root invalidate queue
                this.InvalidateGraphicBounds(Rectangle.Union(prevBounds, this.RectBounds));                 
            }
        }

        public void SetBounds(int left, int top, int width, int height)
        {
            if (parentLink == null)
            {
                this.b_left = left;
                this.b_top = top;
                this.b_width = width;
                this.b_height = height;
            }
            else
            {
                var prevBounds = this.RectBounds;
                this.b_left = left;
                this.b_top = top;
                this.b_width = width;
                this.b_height = height;
                this.InvalidateGraphicBounds(Rectangle.Union(prevBounds, this.RectBounds));             
            }
        }

        public void ResumeLayout()
        {
            uiLayoutFlags &= ~RenderElementConst.LY_SUSPEND;

            if (this.MayHasChild)
            {
                if (this.HasParent)
                {
                    if (!IsInTopDownReArrangePhase)
                    {
                        this.StartBubbleUpLayoutInvalidState();
                    }
                }
                else
                {
                    if (this.IsTopWindow)
                    {
                        this.TopDownReCalculateContentSize();
                        ((RenderBoxBase)this).TopDownReArrangeContentIfNeed();
                    }
                }
            }
        }


    }
}