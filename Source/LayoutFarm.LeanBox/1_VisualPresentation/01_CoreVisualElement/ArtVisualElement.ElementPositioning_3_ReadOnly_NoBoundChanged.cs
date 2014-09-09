﻿//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

using LayoutFarm.Presentation;


namespace LayoutFarm.Presentation
{


    partial class ArtVisualElement
    {
        //----------------------
        //rectangle boundary area

        int b_top;
        int b_left;
        int b_width;
        int b_Height;


        public bool IntersectsWith(InternalRect r)
        {
            int left = this.b_left;

            if (((left <= r._left) && (this.Right > r._left)) ||
                ((left >= r._left) && (left < r._right)))
            {
                int top = this.b_top;
                if (((top <= r._top) && (this.Bottom > r._top)) ||
               ((top >= r._top) && (top < r._bottom)))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IntersectsWith(Rectangle r)
        {
            int left = this.b_left;

            if (((left <= r.Left) && (this.Right > r.Left)) ||
                ((left >= r.Left) && (left < r.Right)))
            {
                int top = this.b_top;
                if (((top <= r.Top) && (this.Bottom > r.Top)) ||
               ((top >= r.Top) && (top < r.Bottom)))
                {
                    return true;
                }
            }
            return false;
        }



        public bool IntersectOnHorizontalWith(InternalRect r)
        {
            int left = this.b_left;

            if (((left <= r._left) && (this.Right > r._left)) ||
                ((left >= r._left) && (left < r._right)))
            {
                return true;
            }
            return false;
        }



        protected Rectangle GetLocalArea()
        {
            return new Rectangle(0, 0, b_width, b_Height);
        }

        public Rectangle BoundRect
        {
            get
            {
                return new Rectangle(b_left, b_top, b_width, b_Height);

            }
        }
        public Size Size
        {
            get
            {
                return new Size(b_width, b_Height);
            }
        }
        public int X
        {
            get
            {
                return b_left;
            }

        }
        public int Y
        {
            get
            {
                return b_top;
            }

        }
        public int Right
        {
            get
            {
                return b_left + b_width;
            }
        }
        public int Bottom
        {
            get
            {
                return b_top + b_Height;
            }
        }
        public Point Location
        {
            get
            {
                return new Point(b_left, b_top);
            }
        }
        public int Width
        {
            get
            {
                return b_width;
            }
        }
        public int Height
        {
            get
            {
                return b_Height;
            }
        }





        public Point GetGlobalLocation()
        {

            return GetGlobalLocationStatic(this);
        }
        public Point GetLocationLimitTo(ArtVisualElement parentHint)
        {
            ArtVisualElement parentVisualElement = this.ParentVisualElement;
            if (parentVisualElement == parentHint)
            {
                return new Point(this.b_left, this.b_top);
            }
            else
            {
                if (parentVisualElement != null)
                {
                    Point parentPos = parentVisualElement.GetLocationLimitTo(parentHint); return new Point(b_left + parentPos.X, b_top + parentPos.Y);
                }
                else
                {
                    return new Point(b_left, b_top);
                }
            }
        }
        public Point GetGlobalLocationRelativeTo(ArtVisualElement relativeElement)
        {

            Point relativeElemLoca = relativeElement.Location;
            Point relativeElementGlobalLocation = relativeElement.GetGlobalLocation();
            relativeElementGlobalLocation.Offset(
               b_left - relativeElemLoca.X, b_top - relativeElemLoca.Y); return relativeElementGlobalLocation;
        }
        public Point GetLocationAsChildOf(ArtVisualElement relativeElement)
        {
            Point relativeElementGlobalLocation = relativeElement.GetGlobalLocation();
            Point thisGlobalLoca = GetGlobalLocation();
            return new Point(thisGlobalLoca.X - relativeElementGlobalLocation.X, thisGlobalLoca.Y - relativeElementGlobalLocation.Y);
        }
        public Point GetLocationAsSiblingOf(ArtVisualElement relativeElement)
        {
            ArtVisualElement parent = relativeElement.ParentVisualElement;
            return GetLocationAsChildOf(parent);
        }
        public Rectangle GetGlobalRect()
        {
            return new Rectangle(GetGlobalLocationStatic(this), Size);
        }

        static Point GetGlobalLocationStatic(ArtVisualElement ui)
        {

            ArtVisualElement parentVisualElement = ui.ParentVisualElement;
            if (parentVisualElement != null)
            {
                Point parentGlobalLocation = GetGlobalLocationStatic(parentVisualElement);
                ui.visualParentLink.AdjustParentLocation(ref parentGlobalLocation);

                if (parentVisualElement.IsVisualContainerBase)
                {
                    ArtVisualContainerBase parentAsContainerBase = (ArtVisualContainerBase)parentVisualElement;
                    return new Point(ui.b_left + parentGlobalLocation.X - parentAsContainerBase.ViewportX,
                        ui.b_top + parentGlobalLocation.Y - parentAsContainerBase.ViewportY);
                }
                else
                {
                    return new Point(ui.b_left + parentGlobalLocation.X, ui.b_top + parentGlobalLocation.Y);
                }
            }
            else
            {
                return ui.Location;
            }
        }

        public bool HitTestCoreNoRecursive(Point testPoint)
        {
            if ((uiFlags & HIDDEN) != 0)
            {
                return false;
            }
            return ContainPoint(testPoint.X, testPoint.Y);
        }
        public bool HitTestCore(ArtHitPointChain artHitResult)
        {

            if ((uiFlags & HIDDEN) != 0)
            {
                return false;
            }
            switch (this.ElementNature)
            {   
                case VisualElementNature.Shapes:
                case VisualElementNature.CustomContainer:
                default:
                    {
                        int testX;
                        int testY;
                        artHitResult.GetTestPoint(out testX, out testY);
                        if ((testY >= b_top && testY <= (b_top + b_Height)
                        && (testX >= b_left && testX <= (b_left + b_width))))
                        {
                            ArtVisualContainerBase scContainer = null;
                            if (this.IsScrollable)
                            {
                                scContainer = (ArtVisualContainerBase)this;
                                artHitResult.OffsetTestPoint(-b_left + scContainer.ViewportX,
                                    -b_top + scContainer.ViewportY);
                            }
                            else
                            {
                                artHitResult.OffsetTestPoint(-b_left, -b_top);
                            }

                            artHitResult.AddHit(this);

                            if (this.IsVisualContainerBase)
                            {
                                ((ArtVisualContainerBase)this).ChildrenHitTestCore(artHitResult);
                            }
                            if (this.IsScrollable)
                            {
                                artHitResult.OffsetTestPoint(
                                        b_left - scContainer.ViewportX,
                                        b_top - scContainer.ViewportY);
                            }
                            else
                            {
                                artHitResult.OffsetTestPoint(b_left, b_top);
                            }

                            if ((uiFlags & TRANSPARENT_FOR_ALL_EVENTS) != 0 && artHitResult.CurrentHitElement == this)
                            {
                                artHitResult.RemoveHit(artHitResult.CurrentHitNode);
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else
                        {

                            return false;
                        }
                    }
            }

        }

        public bool FindUnderlingSibling(LinkedList<ArtVisualElement> foundElements)
        {
            throw new NotSupportedException();



        }
        public bool ContainPoint(int x, int y)
        {
            return ((x >= b_left && x < Right) && (y >= b_top && y < Bottom));
        }
        public bool ContainPoint(Point p)
        {
            return ContainPoint(p.X, p.Y);
        }
        public bool ContainRect(InternalRect testRect)
        {
            return testRect._left >= b_left &&
                    testRect._top >= b_top &&
                    testRect._right <= b_left + b_width &&
                    testRect._bottom <= b_top + b_Height;
        }
        public bool ContainRect(Rectangle r)
        {
            return r.Left >= b_left &&
                    r.Top >= b_top &&
                    r.Right <= b_left + b_width &&
                    r.Bottom <= b_top + b_Height;
        }
        public bool ContainRect(int x, int y, int width, int height)
        {
            return x >= b_left &&
                    y >= b_top &&
                    x + width <= b_left + b_width &&
                    y + height <= b_top + b_Height;
        }








        int uiLayoutFlags;
        public const int LY_HAS_SPC_WIDTH = 1 << (1 - 1);
        public const int LY_HAS_SPC_HEIGHT = 1 << (2 - 1);
        public const int LY_HAS_SPC_SIZE = LY_HAS_SPC_WIDTH | LY_HAS_SPC_HEIGHT;
        public const int LY_REACH_MIN_WIDTH = 1 << (3 - 1);
        public const int LY_REACH_MAX_WIDTH = 1 << (4 - 1);
        public const int LY_REACH_MIN_HEIGHT = 1 << (5 - 1);
        public const int LY_REACH_MAX_HEIGHT = 1 << (6 - 1);
        public const int LY_HAS_ARRANGED_CONTENT = 1 << (7 - 1);
        public const int LAY_HAS_CALCULATED_SIZE = 1 << (8 - 1);
        public const int LY_SUSPEND = 1 << (9 - 1);

        public const int LY_SUSPEND_GRAPHIC = 1 << (12 - 1);
        public const int LY_IN_LAYOUT_QUEUE = 1 << (13 - 1);
        public const int LY_IN_LAYOUT_QCHAIN_UP = 1 << (10 - 1);


        public int ElementDesiredWidth
        {
            get
            {
                return this.b_width;
            }
        }
        public int ElementDesiredRight
        {
            get
            {
                return b_left + this.ElementDesiredWidth;
            }
        }

        public int ElementDesiredBottom
        {
            get
            {
                return b_top + this.ElementDesiredHeight;

            }
        }
        public int ElementDesiredHeight
        {
            get
            {
                return b_Height;
            }
        }


        public bool HasSpecificWidth
        {
            get
            {
                return ((uiLayoutFlags & LY_HAS_SPC_WIDTH) == LY_HAS_SPC_WIDTH);
            }
            set
            {

                if (value)
                {
                    uiLayoutFlags |= LY_HAS_SPC_WIDTH;
                }
                else
                {
                    uiLayoutFlags &= ~LY_HAS_SPC_WIDTH;
                }
            }
        }

        public bool HasSpecificHeight
        {
            get
            {
                return ((uiLayoutFlags & LY_HAS_SPC_HEIGHT) == LY_HAS_SPC_HEIGHT);
            }
            set
            {

                if (value)
                {
                    uiLayoutFlags |= LY_HAS_SPC_HEIGHT;
                }
                else
                {
                    uiLayoutFlags &= ~LY_HAS_SPC_HEIGHT;
                }
            }
        }
        public bool HasSpecificSize
        {
            get
            {
                return ((uiLayoutFlags & LY_HAS_SPC_SIZE) != 0);
            }
            set
            {
                if (value)
                {
                    uiLayoutFlags |= LY_HAS_SPC_SIZE;
                }
                else
                {
                    uiLayoutFlags &= ~LY_HAS_SPC_SIZE;
                }
            }
        }
        public static int GetLayoutSpecificDimensionType(ArtVisualElement visualElement)
        {
            return visualElement.uiLayoutFlags & 0x3;
        }





        public bool HasCalculatedSize
        {
            get
            {
                return ((uiLayoutFlags & LAY_HAS_CALCULATED_SIZE) != 0);
            }
        }
        protected void MarkHasValidCalculateSize()
        {

            uiLayoutFlags |= LAY_HAS_CALCULATED_SIZE;
#if DEBUG
            this.dbug_ValidateRecalculateSizeEpisode++;
#endif
        }



        public bool IsInLayoutQueue
        {
            get
            {
                return (uiLayoutFlags & LY_IN_LAYOUT_QUEUE) != 0;
            }
            set
            {

                if (value)
                {
                    uiLayoutFlags |= LY_IN_LAYOUT_QUEUE;
                }
                else
                {
                    uiLayoutFlags &= ~LY_IN_LAYOUT_QUEUE;
                }
            }
        }
        bool IsInLayoutQueueChainUp
        {
            get
            {
                return (uiLayoutFlags & LY_IN_LAYOUT_QCHAIN_UP) != 0;
            }
            set
            {

                if (value)
                {
                    uiLayoutFlags |= LY_IN_LAYOUT_QCHAIN_UP;
                }
                else
                {
                    uiLayoutFlags &= ~LY_IN_LAYOUT_QCHAIN_UP;
                }
            }
        }
        public void InvalidateLayoutAndStartBubbleUp()
        {
            MarkInvalidContentSize();
            MarkInvalidContentArrangement();
            if (this.visualParentLink != null)
            {
                StartBubbleUpLayoutInvalidState();
            }
        }
        public static void InnerInvalidateLayoutAndStartBubbleUp(ArtVisualElement ve)
        {
            ve.InvalidateLayoutAndStartBubbleUp();
        }
        static ArtVisualElement BubbleUpInvalidLayoutToTopMost(ArtVisualElement ve, ArtVisualRootWindow winroot)
        {
#if DEBUG
            VisualRoot dbugVRoot = ve.dbugVRoot;
#endif

            ve.MarkInvalidContentSize();

            if (ve.visualParentLink == null)
            {
#if DEBUG
                if (ve.IsWindowRoot)
                {
                }

                dbugVRoot.dbug_PushLayoutTraceMessage(VisualRoot.dbugMsg_NO_OWNER_LAY);
#endif
                return null;
            }
            if (winroot != null)
            {
                if (winroot.IsLayoutQueueClearing)
                {
                    return null;
                }
                else if (winroot.IsInLayoutQueue)
                {


                    ve.IsInLayoutQueueChainUp = true;
                    winroot.AddToLayoutQueue(ve);
                }
            }
#if DEBUG
            dbugVRoot.dbug_LayoutTraceBeginContext(VisualRoot.dbugMsg_E_CHILD_LAYOUT_INV_BUB_enter, ve);
#endif

            bool goFinalExit;
            ArtVisualElement parentVisualElem = ve.visualParentLink.NotifyParentToInvalidate(out goFinalExit
#if DEBUG
,
ve
#endif
);


            if (!goFinalExit)
            {
                if (parentVisualElem.IsTextEditContainer)
                {
                    parentVisualElem.MarkInvalidContentArrangement();
                    parentVisualElem.IsInLayoutQueueChainUp = true;
                    goto finalExit;
                }
                else if (parentVisualElem.ActAsFloatingWindow)
                {
                    parentVisualElem.MarkInvalidContentArrangement();
                    parentVisualElem.IsInLayoutQueueChainUp = true;
                    goto finalExit;
                }

                parentVisualElem.MarkInvalidContentSize();
                parentVisualElem.MarkInvalidContentArrangement();

                if (!parentVisualElem.IsInLayoutQueueChainUp
                    && !parentVisualElem.IsInLayoutQueue
                    && !parentVisualElem.IsInLayoutSuspendMode)
                {

                    parentVisualElem.IsInLayoutQueueChainUp = true;

                    ArtVisualElement upper = BubbleUpInvalidLayoutToTopMost(parentVisualElem, winroot);

                    if (upper != null)
                    {
                        upper.IsInLayoutQueueChainUp = true;
                        parentVisualElem = upper;
                    }
                }
                else
                {
                    parentVisualElem.IsInLayoutQueueChainUp = true;
                }
            }

        finalExit:
#if DEBUG
            dbugVRoot.dbug_LayoutTraceEndContext(VisualRoot.dbugMsg_E_CHILD_LAYOUT_INV_BUB_exit, ve);
#endif

            return parentVisualElem;
        }

        ArtVisualRootWindow InternalGetWinRootElement()
        {
            if (visualParentLink == null)
            {
                if (this.IsWindowRoot)
                {
                    return (ArtVisualRootWindow)this;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return visualParentLink.GetWindowRoot();
            }
        }
        public void StartBubbleUpLayoutInvalidState()
        {

#if DEBUG
            dbugVRoot.dbug_LayoutTraceBeginContext(VisualRoot.dbugMsg_E_LAYOUT_INV_BUB_FIRST_enter, this);
#endif
            ArtVisualRootWindow winroot = this.InternalGetWinRootElement();
            ArtVisualElement tobeAddToLayoutQueue = BubbleUpInvalidLayoutToTopMost(this, winroot);
#if DEBUG

#endif

            if (tobeAddToLayoutQueue != null
                && winroot != null
                && !tobeAddToLayoutQueue.IsInLayoutQueue)
            {
                winroot.AddToLayoutQueue(tobeAddToLayoutQueue);
            }

#if DEBUG
            dbugVRoot.dbug_LayoutTraceEndContext(VisualRoot.dbugMsg_E_LAYOUT_INV_BUB_FIRST_exit, this);
#endif

        }

        public bool NeedReCalculateContentSize
        {
            get
            {
                return (uiLayoutFlags & LAY_HAS_CALCULATED_SIZE) == 0;
            }
        }
#if DEBUG
        public bool dbugNeedReCalculateContentSize
        {
            get
            {
                return this.NeedReCalculateContentSize;
            }
        }
#endif
        public int GetReLayoutState()
        {
            return (uiLayoutFlags >> (7 - 1)) & 0x3;
        }


        public void MarkInvalidContentArrangement()
        {
            uiLayoutFlags &= ~LY_HAS_ARRANGED_CONTENT;
#if DEBUG

            this.dbug_InvalidateContentArrEpisode++;
            dbug_totalInvalidateContentArrEpisode++;
#endif
        }
        public void MarkInvalidContentSize()
        {

            uiLayoutFlags &= ~LAY_HAS_CALCULATED_SIZE;
#if DEBUG
            this.dbug_InvalidateRecalculateSizeEpisode++;
#endif
        }
        public void MarkValidContentArrangement()
        {

#if DEBUG
            this.dbug_ValidateContentArrEpisode++;
#endif
            this.IsInLayoutQueueChainUp = false;
            uiLayoutFlags |= LY_HAS_ARRANGED_CONTENT;
        }
        public bool NeedContentArrangement
        {
            get
            {
                return (uiLayoutFlags & LY_HAS_ARRANGED_CONTENT) == 0;
            }
        }
#if DEBUG
        public bool dbugNeedContentArrangement
        {
            get
            {
                return this.NeedContentArrangement;
            }
        }
#endif
    }
}