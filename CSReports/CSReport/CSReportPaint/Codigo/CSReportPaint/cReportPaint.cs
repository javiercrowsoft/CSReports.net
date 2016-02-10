using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using CSKernelClient;
using CSReportDll;
using CSReportGlobals;

namespace CSReportPaint
{

    public class cReportPaint
    {

        private const String C_MODULE = "cReportPaint";

        private const float C_GRID_AREA_WIDTH = 200;
        private const float C_GRID_AREA_HEIGHT = 67;

        private const String C_KEY_PAINT_OBJ = "P";
        private const String C_KEY_PAINT_SEC = "S";

        private cReportPaintObjects m_paintObjects = new cReportPaintObjects();
        private cReportPaintObjects m_paintSections = new cReportPaintObjects();
        private cReportPaintObjects m_paintGridAreas = new cReportPaintObjects();

        private int m_nextKey = 0;
        private HatchBrush m_brushGrid;

        private float m_x1 = 0;
        private float m_y1 = 0;
        private float m_y2 = 0;
        private float m_x2 = 0;

        private float m_x1Ex = 0;
        private float m_y1Ex = 0;
        private float m_y2Ex = 0;
        private float m_x2Ex = 0;

        private bool m_beginMoveDone;

        private String m_keyFocus = "";
        private String[,] m_vGridObjs = null;
        private bool m_notBorder;

        private Font[] m_fnt;

        private float m_gridHeight = 0;

        private String[] m_vSelectedKeys = null;

        private int m_zoom = 0;

        private float m_scaleX = 0;
        private float m_scaleY = 0;

        private Bitmap m_bitmap;

        public cReportPaint() 
        {
            try 
            {
                m_scaleX = 1;
                m_scaleY = 1;

                G.redim(ref m_vGridObjs, 0, 0);
                cGlobals.redim(ref m_fnt, 0);
                G.redim(ref m_vSelectedKeys, 0);

                m_zoom = 100;
            } 
            catch (Exception ex) {
                cError.mngError(ex, "constructor", C_MODULE, "");
            }
        }

        public void setGridHeight(float rhs)
        {
            m_gridHeight = rhs;
        }

        public cReportPaintObjects getPaintSections()
        {
            return m_paintSections;
        }

        public cReportPaintObjects getPaintObjects()
        {
            return m_paintObjects;
        }


        internal bool getNotBorder()
        {
            return m_notBorder;
        }

        internal void setNotBorder(bool rhs)
        {
            m_notBorder = rhs;
        }

        internal int getZoom()
        {
            return m_zoom;
        }

        internal void setZoom(int rhs)
        {
            m_zoom = rhs;
        }

        internal void setScaleY(float rhs)
        {
            m_scaleY = rhs;
        }

        internal void setScaleX(float rhs)
        {
            m_scaleX = rhs;
        }

        internal float getScaleY()
        {
            return m_scaleY;
        }

        internal float getScaleX()
        {
            return m_scaleX;
        }

        /*
         * TODO: delete
         * 
        public int copyBitmap(int hDCSource, int width, int height, int hCurrentBmp) {
            int hDCDest = 0;
            int hBmpOld = 0;
            int hBmp = 0;

            width = width / Screen.TwipsPerPixelX;
            height = height / Screen.TwipsPerPixelY;

            hDCDest = CreateCompatibleDC(hDCSource);
            hBmp = CreateCompatibleBitmap(hDCSource, width, height);
            hBmpOld = SelectObject(hDCDest, hBmp);

            BitBlt(hDCDest, 0, 0, width, height, hDCSource, 0, 0, vbSrcCopy);

            SelectObject(hDCDest, hBmpOld);
            DeleteObject(hDCDest);

            if (VBA.ex.LastDllError != 0) {
                VBA.ex.Raise(vbObjectError, C_MODULE, "Error al copiar el bitmap. Numero: "+ VBA.ex.LastDllError);
            }

            if (hCurrentBmp != 0) {
                DeleteObject(hCurrentBmp);
            }

            return hBmp;
        }
        */
        public cReportPaintObject getPaintObject(String sKey)
        {
            if (sKey.Substring(0, C_KEY_PAINT_OBJ.Length) == C_KEY_PAINT_OBJ)
            {
                return m_paintObjects.item(sKey);
            }
            else
            {
                return m_paintSections.item(sKey);
            }
        }

        public cReportPaintObject getPaintObjectForTag(String tag)
        {
            for (int i = 0; i < m_paintObjects.count(); i++)
            {
                cReportPaintObject paintObj = m_paintObjects.item(i);
                if (paintObj.getTag() == tag)
                {
                    return paintObj;
                }
            }
            return null;
        }

        public cReportPaintObject getPaintSectoinForTag(String tag)
        {
            for (int i = 0; i < m_paintSections.count(); i++)
            {
                cReportPaintObject paintObj = m_paintSections.item(i);
                if (paintObj.getTag() == tag)
                {
                    return paintObj;
                }
            }
            return null;
        }

        public cReportPaintObject getNewObject(csRptPaintObjType paintTypeObject)
        {
            String key = "";
            key = getKeyPaintObj();
            cReportPaintObject paintObj = null;
            paintObj = m_paintObjects.add(paintObj, key);
            paintObj.setKey(key);
            paintObj.setPaintType(paintTypeObject);
            return paintObj;
        }

        public cReportPaintObject getNewSection(csRptPaintObjType paintTypeObject)
        {
            String key = "";
            key = getKeyPaintSec();
            cReportPaintObject paintObj = null;
            paintObj = m_paintSections.add(paintObj, key);
            paintObj.setKey(key);
            paintObj.setPaintType(paintTypeObject);
            return paintObj;
        }

        public bool paintObjIsSection(String sKey)
        {
            return sKey.Substring(0, C_KEY_PAINT_SEC.Length) == C_KEY_PAINT_SEC;
        }

		public bool pointIsInObject(float x, float y, ref String sKey)
		{
			csRptPaintRegionType regionType = csRptPaintRegionType.CRPTPNTRGNTYPEBODY;
			return pointIsInObject(x, y, ref sKey, ref regionType);
		}
        
		public bool pointIsInObject(float x, float y, ref String sKey, ref csRptPaintRegionType regionType)
        { // TODO: Use of ByRef founded Public Function PointIsInObject(ByVal x As Single, ByVal y As Single, ByRef sKey As String, Optional ByRef RegionType As csRptPaintRegionType = 0) As Boolean
            if (pointIsInObjectAux(m_paintSections, x, y, ref sKey, ref regionType))
            {
                return true;
            }
            if (pointIsInObjectAux(m_paintObjects, x, y, ref sKey, ref regionType))
            {
                return true;
            }
            return false;
        }

        public bool pointIsInThisObject(float x, float y, ref String sKey, ref csRptPaintRegionType regionType)
        { // TODO: Use of ByRef founded Public Function PointIsInThisObject(ByVal x As Single, ByVal y As Single, ByVal sKey As String, Optional ByRef RegionType As csRptPaintRegionType = 0) As Boolean
            if (pointIsInThisObjectAux(m_paintObjects.item(sKey), x, y, ref sKey, ref regionType))
            {
                return true;
            }
            if (pointIsInThisObjectAux(m_paintObjects.item(sKey), x, y, ref sKey, ref regionType))
            {
                return true;
            }
            return false;
        }

        private bool pointIsInObjectAux(
            cReportPaintObjects paintObjs,
            float x,
            float y,
            ref String sKey,
            ref csRptPaintRegionType regionType)
        {
            for (int i = paintObjs.count()-1; i > -1; i--)
            {
                if (pointIsInThisObjectAux(paintObjs.getNextPaintObjForZOrder(i), x, y, ref sKey, ref regionType))
                {
                    return true;
                }
            }
            return false;
        }

        private bool pointIsInThisObjectAux(
            cReportPaintObject paintObj,
            float x,
            float y,
            ref String sKey,
            ref csRptPaintRegionType regionType)
        { /* ByRef PaintObj As cReportPaintObject, 
           * ByVal x As Single, 
           * ByVal y As Single, 
           * ByRef sKey As String, 
           * Optional ByRef RegionType As csRptPaintRegionType */

            const int C_WIDTH_REGION = 3;

            float yY = 0;
            float xX = 0;

            float top = 0;
            float height = 0;
            float width = 0;
            float left = 0;

            if (paintObj == null)
            {
                return false;
            }
            else
            {
                CSReportDll.cReportAspect w_aspect = paintObj.getAspect();
                left = w_aspect.getLeft();
                width = w_aspect.getWidth();
                top = w_aspect.getTop() - w_aspect.getOffset();
                height = w_aspect.getHeight();

                if (pointIsInRegion(left - C_WIDTH_REGION,
                                    top - C_WIDTH_REGION,
                                    left + width + C_WIDTH_REGION,
                                    top + height + C_WIDTH_REGION,
                                    x, y))
                {
                    sKey = paintObj.getKey();

                    yY = top + height / 2;
                    yY = yY - C_WIDTH_REGION;

                    xX = left + width / 2;
                    xX = xX - C_WIDTH_REGION;

                    // we need to know in which region it is
                    //

                    // body
                    //
                    if (pointIsInRegion(left + C_WIDTH_REGION,
                                        top + C_WIDTH_REGION,
                                        left + width - C_WIDTH_REGION,
                                        top + height - C_WIDTH_REGION,
                                        x, y))
                    {
                        regionType = csRptPaintRegionType.CRPTPNTRGNTYPEBODY;
                    }
                    // Left
                    else if (pointIsInRegion(left - C_WIDTH_REGION * 2,
                                                yY,
                                                left + C_WIDTH_REGION * 2,
                                                yY + C_WIDTH_REGION * 2,
                                                x, y))
                    {
                        regionType = csRptPaintRegionType.CRPTPNTRGNTYPELEFT;
                    }
                    // Rigth
                    else if (pointIsInRegion(left + width - C_WIDTH_REGION * 2,
                                                yY,
                                                left + width + C_WIDTH_REGION * 2,
                                                yY + C_WIDTH_REGION * 2,
                                                x, y))
                    {
                        regionType = csRptPaintRegionType.CRPTPNTRGNTYPERIGHT;
                    }
                    // Up
                    else if (pointIsInRegion(xX,
                                                top - C_WIDTH_REGION * 2,
                                                xX + C_WIDTH_REGION * 2,
                                                top + C_WIDTH_REGION * 2,
                                                x, y))
                    {
                        regionType = csRptPaintRegionType.CRPTPNTRGNTYPEUP;
                    }
                    // Down
                    else if (pointIsInRegion(xX,
                                                top + height - C_WIDTH_REGION * 2,
                                                xX + C_WIDTH_REGION * 2,
                                                top + height + C_WIDTH_REGION * 2,
                                                x, y))
                    {
                        regionType = csRptPaintRegionType.CRPTPNTRGNTYPEDOWN;
                    }
                    // LeftUp
                    else if (pointIsInRegion(left - C_WIDTH_REGION,
                                                top - C_WIDTH_REGION,
                                                left + C_WIDTH_REGION,
                                                top + C_WIDTH_REGION,
                                                x, y))
                    {
                        regionType = csRptPaintRegionType.CRPTPNTRGNTYPELEFTUP;
                    }
                    // LeftDown
                    else if (pointIsInRegion(left - C_WIDTH_REGION,
                                                top + height - C_WIDTH_REGION,
                                                left + C_WIDTH_REGION,
                                                top + height + C_WIDTH_REGION,
                                                x, y))
                    {
                        regionType = csRptPaintRegionType.CRPTPNTRGNTYPELEFTDOWN;
                    }
                    // RigthUp
                    else if (pointIsInRegion(left + width - C_WIDTH_REGION,
                                                top - C_WIDTH_REGION,
                                                left + width + C_WIDTH_REGION,
                                                top + C_WIDTH_REGION,
                                                x, y))
                    {
                        regionType = csRptPaintRegionType.CRPTPNTRGNTYPERIGHTUP;
                    }
                    // RitgthDown
                    else if (pointIsInRegion(left + width - C_WIDTH_REGION,
                                                top + height - C_WIDTH_REGION,
                                                left + width + C_WIDTH_REGION,
                                                top + height + C_WIDTH_REGION,
                                                x, y))
                    {
                        regionType = csRptPaintRegionType.CRPTPNTRGNTYPERIGHTDOWN;
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private String getKeyPaintObj()
        {
            m_nextKey = m_nextKey + 1;
            return C_KEY_PAINT_OBJ + m_nextKey;
        }

        private String getKeyPaintSec()
        {
            m_nextKey = m_nextKey + 1;
            return C_KEY_PAINT_SEC + m_nextKey;
        }

        private String getKey()
        {
            m_nextKey = m_nextKey + 1;
            return "K" + m_nextKey;
        }

        private bool pointIsInRegion(float x1, float y1, float x2, float y2, float x, float y)
        {
            return x >= x1 && x <= x2 && y >= y1 && y <= y2;
        }
        
        // we have four points for every region. we need to know if at least one point
        // of region A is in region B or viceversa
        //
        private bool regionIsInRegion(
            float x1,
            float y1,
            float x2,
            float y2,
            float z1,
            float w1,
            float z2,
            float w2)
        {
            // first B in A
            //
            if (x1 <= z1 && x2 >= z1 && w1 <= y1 && w2 >= y1)
            {
                return true;
            }
            if (x1 <= z2 && x2 >= z2 && w1 <= y1 && w2 >= y1)
            {
                return true;
            }
            if (x1 <= z1 && x2 >= z1 && w1 <= y2 && w2 >= y2)
            {
                return true;
            }
            if (x1 <= z2 && x2 >= z2 && w1 <= y2 && w2 >= y2)
            {
                return true;
            }
            // then A in B
            //
            if (z1 <= x1 && z2 >= x1 && y1 <= w1 && y2 >= w1)
            {
                return true;
            }
            if (z1 <= x2 && z2 >= x2 && y1 <= w1 && y2 >= w1)
            {
                return true;
            }
            if (z1 <= x1 && z2 >= x1 && y1 <= w2 && y2 >= w2)
            {
                return true;
            }
            if (z1 <= x2 && z2 >= x2 && y1 <= w2 && y2 >= w2)
            {
                return true;
            }
            return false;
        }

        //-----------------------------------------------------------------------------------------------
        // Grid
        //
        public void initGrid(Graphics picGrid, csETypeGrid typeGrid)
        {
            int x = 0;
            int y = 0;
            cReportPaintObject c = null;
            float top = 0;
            float left = 0;
            int i = 0;

            pCreateBrushGrid(picGrid, typeGrid);

            y = (int)(picGrid.VisibleClipBounds.Height / C_GRID_AREA_HEIGHT);
            x = (int)(picGrid.VisibleClipBounds.Width / C_GRID_AREA_WIDTH);

            x = x + 1;
            y = y + 1;

            G.redim(ref m_vGridObjs, x, y);

            int l = 0;
            int t = 0;

            for (i = 0; i < y * x; i++)
            {
                c = m_paintGridAreas.add(c, getKey());

                left = C_GRID_AREA_WIDTH * l;
                top = C_GRID_AREA_HEIGHT * t;
                CSReportDll.cReportAspect w_aspect = c.getAspect();
                w_aspect.setLeft(left);
                w_aspect.setTop(top);
                w_aspect.setWidth(C_GRID_AREA_WIDTH);
                w_aspect.setHeight(C_GRID_AREA_HEIGHT);

                m_vGridObjs[l, t] = c.getKey();

                c = null;

                l = l + 1;
                if (l >= x)
                {
                    l = 0;
                    t = t + 1;
                }
            }

            refreshBackgroundPicture(picGrid, (int)csColors.C_COLOR_WHITE);
        }

        //----------------------------------------------------------------------------------
        // Align

        // to align an object we need to know in which
        // grid it is located every one of this three points:
        //
        //          a------------------b
        //          |                  |
        //          |                  |
        //          c-------------------

        // a define top and left
        // b define widht
        // c define heigth
        public void alingObjTopToGrid(String sKey)
        {
            alingObjToGrid(sKey, false, true, false, false, true);
        }

        public void alingObjLeftToGrid(String sKey)
        {
            alingObjToGrid(sKey, true, false, false, false, true);
        }

        public void alingObjBottomToGrid(String sKey)
        {
            alingObjToGrid(sKey, false, false, true, false, true);
        }

        public void alingObjRightToGrid(String sKey)
        {
            alingObjToGrid(sKey, false, false, false, true, true);
        }

        public void alingObjLeftTopToGrid(String sKey)
        {
            alingObjToGrid(sKey, true, true, false, false, true);
        }

        public void alingObjLeftBottomToGrid(String sKey)
        {
            alingObjToGrid(sKey, true, false, true, false, true);
        }

        public void alingObjRightTopToGrid(String sKey)
        {
            alingObjToGrid(sKey, false, true, false, true, true);
        }

        public void alingObjRightBottomToGrid(String sKey)
        {
            alingObjToGrid(sKey, false, false, true, true, true);
        }

        public void alingToGrid(String sKey)
        {
            alingObjToGrid(sKey, true, true, false, false, false);
        }

        private void alingObjToGrid(
            String sKey, 
            bool toLeft, 
            bool toTop, 
            bool toBottom, 
            bool toRight, 
            bool resizing)
        {
            int z1 = 0;
            int q1 = 0;
            int maxY = 0;
            int maxX = 0;
            CSReportDll.cReportAspect gridObjAspect = null;

            maxX = m_vGridObjs.GetLength(0)-1;
            maxY = m_vGridObjs.GetLength(1)-1;            

            float top = 0;
            float left = 0;
            float width = 0;
            float height = 0;
            float offset = 0;
            const float pointSeparation = 0.6f;
            const float offSetPointSep = 0.3f;

            cReportPaintObjects paintObjs = null;

            if (sKey.Substring(0, 1) == C_KEY_PAINT_SEC)
            {
                paintObjs = m_paintSections;
            }
            else
            {
                paintObjs = m_paintObjects;
            }

            float nLeft = 0;
            float nTop = 0;

            cReportPaintObject w_item = paintObjs.item(sKey);
            CSReportDll.cReportAspect w_aspect = w_item.getAspect();
            nLeft = w_aspect.getLeft() - offSetPointSep;
            nTop = w_aspect.getTop() - w_aspect.getOffset() - offSetPointSep;

            if (nLeft < 0) { nLeft = 0; }
            if (nTop < 0) { nTop = 0; }

            if (toTop || toLeft)
            {
                // we get the grid where the point A is located
                //
                z1 = Convert.ToInt32(nLeft / C_GRID_AREA_WIDTH);
                q1 = Convert.ToInt32(nTop / C_GRID_AREA_HEIGHT);

                if (nLeft > z1 * C_GRID_AREA_WIDTH) { z1 = z1 + 1; }
                if (nTop > q1 * C_GRID_AREA_HEIGHT) { q1 = q1 + 1; }

                if (z1 < 0) { z1 = 0; }
                if (q1 < 0) { q1 = 0; }

                if (z1 > maxX) { z1 = maxX; }
                if (q1 > maxY) { q1 = maxY; }

                gridObjAspect = m_paintGridAreas.item(m_vGridObjs[z1, q1]).getAspect();

                if (toTop)
                {
                    // now we need to get which is the nearest point
                    //
                    top = (w_aspect.getTop() - w_aspect.getOffset()) - gridObjAspect.getTop();
                    top = Convert.ToInt32(top / pointSeparation) * pointSeparation;
                    offset = gridObjAspect.getTop() 
                                + top 
                                - offSetPointSep 
                                - (w_aspect.getTop() - w_aspect.getOffset());
                    w_aspect.setTop((gridObjAspect.getTop() + top - offSetPointSep) + w_aspect.getOffset());

                    if (resizing)
                    {
                        w_aspect.setHeight(w_aspect.getHeight() - offset);
                    }
                }

                if (toLeft)
                {
                    left = w_aspect.getLeft() - gridObjAspect.getLeft();
                    left = Convert.ToInt32(left / pointSeparation) * pointSeparation;
                    offset = gridObjAspect.getLeft() + left - offSetPointSep - w_aspect.getLeft();
                    w_aspect.setLeft(gridObjAspect.getLeft() + left - offSetPointSep);

                    if (resizing)
                    {
                        w_aspect.setWidth(w_aspect.getWidth() - offset);
                    }
                }
            }

            if (toRight)
            {
                // we get the grid where the point B is located
                //
                z1 = Convert.ToInt32((nLeft + w_aspect.getWidth()) / C_GRID_AREA_WIDTH);
                if (nLeft + w_aspect.getWidth() > z1 * C_GRID_AREA_WIDTH) { z1 = z1 + 1; }

                q1 = Convert.ToInt32(nTop / C_GRID_AREA_HEIGHT);
                if (nTop > q1 * C_GRID_AREA_HEIGHT) { q1 = q1 + 1; }

                if (z1 < 1) { z1 = 0; }
                if (q1 < 1) { q1 = 0; }

                if (z1 > maxX) { z1 = maxX; }
                if (q1 > maxY) { q1 = maxY; }

                gridObjAspect = m_paintGridAreas.item(m_vGridObjs[z1, q1]).getAspect();

                // now we need to get which is the nearest point
                //
                width = w_aspect.getLeft() + w_aspect.getWidth() - gridObjAspect.getLeft();
                width = Convert.ToInt32(width / pointSeparation) * pointSeparation - offSetPointSep;
                w_aspect.setWidth(gridObjAspect.getLeft() + width - w_aspect.getLeft());

            }

            if (toBottom)
            {
                // we get the grid where the point C is located
                //
                z1 = Convert.ToInt32(nLeft / C_GRID_AREA_WIDTH);
                q1 = Convert.ToInt32((nTop + w_aspect.getHeight()) / C_GRID_AREA_HEIGHT);

                if (nLeft > z1 * C_GRID_AREA_WIDTH) { z1 = z1 + 1; }
                if (nTop + w_aspect.getHeight() > q1 * C_GRID_AREA_HEIGHT) { q1 = q1 + 1; }

                if (z1 < 1) { z1 = 0; }
                if (q1 < 1) { q1 = 0; }

                if (z1 > maxX) { z1 = maxX; }
                if (q1 > maxY) { q1 = maxY; }

                gridObjAspect = m_paintGridAreas.item(m_vGridObjs[z1, q1]).getAspect();

                // now we need to get which is the nearest point
                //
                height = (w_aspect.getTop() - w_aspect.getOffset()) + w_aspect.getHeight() - gridObjAspect.getTop();
                height = Convert.ToInt32(height / pointSeparation) * pointSeparation - offSetPointSep;
                w_aspect.setHeight(gridObjAspect.getTop() + height - (w_aspect.getTop() - w_aspect.getOffset()));
            }
        }

        // end Align
        //-------------------------------

        // Drawing
        public void clearPage(object graph)
        {
            if (graph.GetType() == typeof(cPrinter))
            {
                return;
            }
            refreshBackgroundPicture(graph as Graphics, (int)csColors.C_COLOR_WHITE);
        }

        public bool refreshObject(String key, Graphics graph)
        {
            pClearObject(key, graph);
            return drawObject(key, graph);
        }

        public bool drawObject(String key, Graphics graph)
        {
            return draw(m_paintObjects, key, graph);
        }

        public bool drawSection(String key, Graphics graph)
        {
            // check the width of the paintObject for this section
            // is into the bounds of the page
            //
            cReportAspect aspect = m_paintSections.item(key).getAspect();
            if (aspect.getWidth() > m_bitmap.Size.Width-2)
            {
                aspect.setWidth(m_bitmap.Size.Width-2);
            }
            return draw(m_paintSections, key, graph);
        }

        public bool drawRule(String key, Graphics graph)
        {
            const int LINE_COLOR = 0xcc6600;
            int top = 0;
            float heightSec = 0;
            CSReportDll.cReportAspect aspect = null;

            aspect = new CSReportDll.cReportAspect();

            cReportPaintObject w_item = m_paintSections.item(key);
            heightSec = w_item.getHeightSecLine() * 0.5f;
            CSReportDll.cReportAspect w_aspect = w_item.getAspect();
            aspect.setTop(w_aspect.getTop() + 3 - heightSec);
            aspect.setOffset(w_aspect.getOffset());
            aspect.setTransparent(true);
            aspect.setLeft(0);
            aspect.setHeight(20);
            aspect.setAlign(HorizontalAlignment.Right);
            aspect.setWidth(graph.ClipBounds.Width - 1);

            if (w_item.getTextLine() != "")
            {
                top = - Convert.ToInt32(w_item.getHeightSec());
                w_aspect = w_item.getAspect();
                top += Convert.ToInt32(w_aspect.getTop() - w_aspect.getOffset() - 6 + w_aspect.getHeight() * 2);

                printLine(graph, 
                            true, 
                            0, 
                            top, 
                            aspect.getWidth(), 
                            top,
                            LINE_COLOR, 
                            1, 
                            true,
                            LINE_COLOR, 
                            false);

                // last section line
                //
                printText(graph, w_item.getTextLine(), aspect, w_item.getImage());

                heightSec = w_item.getHeightSec() * 0.5f;

                // print section's name
                //
                w_aspect = m_paintSections.item(key).getAspect();
                aspect.setTop(w_aspect.getTop() - heightSec);
                aspect.setAlign(HorizontalAlignment.Left);

                printText(graph, w_item.getText(), aspect, w_item.getImage());

            }
            else
            {
                top = Convert.ToInt32(aspect.getTop() - aspect.getOffset() - heightSec + w_item.getAspect().getHeight());

                if (w_item.getIsSection())
                {
                    printLine(graph, 
                                true,
                                0, 
                                top, 
                                aspect.getWidth(), 
                                top,
                                LINE_COLOR, 
                                1, 
                                true,
                                LINE_COLOR, 
                                false);
                }

                // every section line except the last one
                //
                printText(graph, w_item.getText(), aspect, w_item.getImage());
            }
            
            if (w_item == m_paintSections.item(m_paintSections.count() - 1)) 
            {
                top = Convert.ToInt32(aspect.getTop() + w_item.getHeightSecLine() - heightSec - aspect.getOffset() + 6);

                if (w_item.getIsSection())
                {
                    printLine(graph,
                                true,
                                0,
                                top,
                                aspect.getWidth(),
                                top,
                                LINE_COLOR,
                                1,
                                true,
                                LINE_COLOR,
                                false);
                }
            }

            return true;
        }

		public void moveObjToXY(String sKey, float x, float y, Graphics graph)
        {
            if (sKey.Substring(0, 1) == C_KEY_PAINT_OBJ)
            {
                CSReportDll.cReportAspect w_aspect = m_paintObjects.item(sKey).getAspect();
                move(x, y, w_aspect.getWidth(), w_aspect.getHeight(), graph);
            }
            else
            {
                CSReportDll.cReportAspect w_aspect = m_paintSections.item(sKey).getAspect();
                move(x, y, w_aspect.getWidth(), w_aspect.getHeight(), graph);
            }
        }

		public void moveObjToXYEx(String sKey, float x, float y, Graphics graph, bool clear)
        {
            if (clear)
            {
                m_x1 = m_x1Ex;
                m_y1 = m_y1Ex;
                m_x2 = m_x2Ex;
                m_y2 = m_y2Ex;
            }
            else
            {
                m_x1 = 0;
                m_x2 = 0;
                m_y1 = 0;
                m_y2 = 0;
            }

            moveObjToXY(sKey, x, y, graph);

            if (m_x1Ex == 0) { m_x1Ex = m_x1; }
            if (m_y1Ex == 0) { m_y1Ex = m_y1; }
            if (m_x2Ex == 0) { m_x2Ex = m_x2; }
            if (m_y2Ex == 0) { m_y2Ex = m_y2; }

            if (m_x1Ex > m_x1 && m_x1 > 0) { m_x1Ex = m_x1; }
            if (m_y1Ex > m_y1 && m_y1 > 0) { m_y1Ex = m_y1; }
            if (m_x2Ex < m_x2 && m_x2 > 0) { m_x2Ex = m_x2; }
            if (m_y2Ex < m_y2 && m_y2 > 0) { m_y2Ex = m_y2; }
        }

		public void moveVertical(String sKey, float y, Graphics graph)
        {
            if (sKey.Substring(0, 1) == C_KEY_PAINT_OBJ)
            {
                CSReportDll.cReportAspect w_aspect = m_paintObjects.item(sKey).getAspect();
                move(w_aspect.getLeft(), y, w_aspect.getWidth(), w_aspect.getHeight(), graph);
            }
            else
            {
                CSReportDll.cReportAspect w_aspect = m_paintSections.item(sKey).getAspect();
                move(w_aspect.getLeft(), y, w_aspect.getWidth(), w_aspect.getHeight(), graph);
            }
        }

		public void moveHorizontal(String sKey, float x, Graphics graph)
        {
            if (sKey.Substring(0, 1) == C_KEY_PAINT_OBJ)
            {
                CSReportDll.cReportAspect w_aspect = m_paintObjects.item(sKey).getAspect();
                move(x, w_aspect.getTop(), w_aspect.getWidth(), w_aspect.getHeight(), graph);
            }
            else
            {
                CSReportDll.cReportAspect w_aspect = m_paintSections.item(sKey).getAspect();
                move(x, w_aspect.getTop(), w_aspect.getWidth(), w_aspect.getHeight(), graph);
            }
        }

        public void endMove(Graphics graph)
        {
            m_x1 = 0;
            m_x2 = 0;
            m_y1 = 0;
            m_y2 = 0;

            m_x1Ex = 0;
            m_x2Ex = 0;
            m_y1Ex = 0;
            m_y2Ex = 0;

            refreshBackgroundPicture(graph, (int)csColors.C_COLOR_WHITE);
            m_beginMoveDone = false;
        }

        // Drawing - Primitive
        private bool draw(cReportPaintObjects collObjs, String key, Graphics graph)
        { // TODO: Use of ByRef founded Private Function Draw(ByRef CollObjs As cReportPaintObjects, ByVal Key As String, ByVal hDC As Long, ByRef Graph As Object) As Boolean
            try
            {
                if (graph == null)
                {
                    throw new ReportPaintException(
                        csRptPaintErrors.CSRPT_PAINT_ERR_OBJ_CLIENT,
                        C_MODULE,
                        cReportPaintError.errGetDescript(
                                        csRptPaintErrors.CSRPT_PAINT_ERR_OBJ_CLIENT));
                }

                cReportPaintObject oPaintObj = null;
                float x1 = 0;
                float y1 = 0;
                float y2 = 0;
                float x2 = 0;
                int colorIn = 0;
                int colorOut = 0;
                bool filled = false;

                oPaintObj = collObjs.item(key);

                if (oPaintObj == null) { return false; }

                CSReportDll.cReportAspect w_aspect = oPaintObj.getAspect();

                x1 = w_aspect.getLeft();
                x2 = x1 + w_aspect.getWidth();
                y1 = w_aspect.getTop() - w_aspect.getOffset();
                y2 = y1 + w_aspect.getHeight();

                if (!w_aspect.getTransparent())
                {
                    colorIn = w_aspect.getBackColor();
                    filled = true;
                }

                colorOut = w_aspect.getBorderColor();

                switch (oPaintObj.getPaintType())
                {
                    case csRptPaintObjType.CSRPTPAINTOBJBOX:

                        pDrawObjBox(graph,
                                    oPaintObj.getAspect(),
                                    x1, y1, x2, y2,
                                    filled,
                                    colorIn,
                                    colorOut);
                        break;

                    case csRptPaintObjType.CSRPTPAINTOBJLINE:

                        printLine(graph, filled, x1, y1, x2, y2, colorIn, 1, false, colorOut, false);
                        break;

                    case csRptPaintObjType.CSRPTPAINTOBJCIRCLE:
                        break;

                    case csRptPaintObjType.CSRPTPAINTOBJIMAGE:

                        pDrawObjBox(graph,
                                    oPaintObj.getAspect(),
                                    x1 - 1, y1 - 1, x2 + 1, y2 + 1,
                                    filled,
                                    colorIn,
                                    0xC0C000);

                        int bmpWidth = 0;
                        int bmpHeight = 0;

                        cGlobals.getBitmapSize(oPaintObj.getImage(), out bmpWidth, out bmpHeight, true);

                        if (bmpWidth > w_aspect.getWidth())
                        {
                            bmpWidth = (int)w_aspect.getWidth();
                        }
                        if (bmpHeight > w_aspect.getHeight())
                        {
                            bmpHeight = (int)w_aspect.getHeight();
                        }

                        drawBMP(graph,
                                oPaintObj.getImage(),
                                x1 * m_scaleX,
                                y1 * m_scaleY,
                                bmpWidth,
                                bmpHeight,
                                bmpWidth * m_scaleX,
                                bmpHeight * m_scaleY);
                        break;
                }

                if (oPaintObj.getText() != "")
                {
                    if (collObjs == m_paintObjects)
                    {
                        printText(graph,
                                    oPaintObj.getText(),
                                    oPaintObj.getAspect(),
                                    oPaintObj.getImage());
                    }
                }

                return true;

            }
            catch (Exception ex)
            {
                cError.mngError(ex, "Draw", C_MODULE, "Error al dibujar un objeto");
                return false;
            }
        }

        private void drawBMP(Graphics graph, Image image, float x, float y, int bmpWidth, int bmpHeight, float destWidth, float destHeight)
        {
            //throw new NotImplementedException();
            /*
              Dim hDC      As Long
              Dim hOldBmp  As Long
  
              hDC = CreateCompatibleDC(0&)
              hOldBmp = SelectObject(hDC, hBmp)
  
              x = x / Screen.TwipsPerPixelX
              y = y / Screen.TwipsPerPixelY
              Width = Width / Screen.TwipsPerPixelX
              Height = Height / Screen.TwipsPerPixelY
              DestWidth = DestWidth / Screen.TwipsPerPixelX
              DestHeight = DestHeight / Screen.TwipsPerPixelY
  
              If DestWidth <> Width Then
                Dim OldStrMode As Long
    
                OldStrMode = SetStretchBltMode(hDCDest, HALFTONE)
  
                StretchBlt hDCDest, x, y, DestWidth, DestHeight, hDC, 0, 0, Width, Height, vbSrcCopy
    
                SetStretchBltMode hDCDest, OldStrMode
    
              Else
                BitBlt hDCDest, x, y, Width, Height, hDC, 0, 0, vbSrcCopy
              End If
  
              SelectObject hDC, hOldBmp
              DeleteObject hDC
             */

            Rectangle sourceRect = new Rectangle(0, 0, bmpWidth, bmpHeight);
            Rectangle destRect = new Rectangle(Convert.ToInt32(x), Convert.ToInt32(y), bmpWidth, bmpHeight);

            graph.DrawImage(image, destRect, sourceRect, GraphicsUnit.Pixel);
        }

        public void setFocus(String sKey, Graphics graph, bool clearSelected)
        {
            if (clearSelected) { G.redim(ref m_vSelectedKeys, 0); }

            if (!pAllreadySelected(sKey))
            {
                G.redimPreserve(ref m_vSelectedKeys, m_vSelectedKeys.Length + 1);
                m_vSelectedKeys[m_vSelectedKeys.Length -1] = sKey;
            }

            m_keyFocus = sKey;
            paintPicture(graph, true);
        }

        public void removeFromSelected(String sKey, Graphics graph)
        {
            int i = 0;

            for (i = 0; i < m_vSelectedKeys.Length; i++)
            {
                if (m_vSelectedKeys[i] == sKey)
                {
                    break;
                }
            }

            if (i >= m_vSelectedKeys.Length) { return; }

            for (i = i + 1; i < m_vSelectedKeys.Length; i++)
            {
                m_vSelectedKeys[i - 1] = m_vSelectedKeys[i];
            }
            if (m_vSelectedKeys.Length > 0)
            {
                G.redimPreserve(ref m_vSelectedKeys, m_vSelectedKeys.Length - 1);
            }
            else
            {
                G.redim(ref m_vSelectedKeys, 0);
            }

            if (m_keyFocus == sKey) { m_keyFocus = ""; }
            
            paintPicture(graph, true);
        }

        private bool pAllreadySelected(String sKey)
        {
            if (sKey == "")
            {
                return true;
            }

            for (int i = 0; i < m_vSelectedKeys.Length; i++)
            {
                if (m_vSelectedKeys[i] == sKey)
                {
                    return true;
                }
            }
            return false;
        }

        private void setFocusAux(String sKey, Graphics graph)
        {
            cReportPaintObject paintObjAsp = null;
            int color = 0;
            bool bCircle = false;

            m_keyFocus = sKey;

            if (m_keyFocus.Substring(0, 1) == C_KEY_PAINT_OBJ)
            {
                paintObjAsp = m_paintObjects.item(m_keyFocus);
                color = 0x80C0FF;
                bCircle = false;
            }
            else
            {
                paintObjAsp = m_paintSections.item(m_keyFocus);
                color = 0x80C0FF;
                bCircle = true;
            }

            if (paintObjAsp == null) { return; }

            CSReportDll.cReportAspect w_aspect = paintObjAsp.getAspect();
            showHandles(graph, 
                        Convert.ToInt32(w_aspect.getLeft()), 
                        Convert.ToInt32(w_aspect.getTop() - w_aspect.getOffset()), 
                        Convert.ToInt32(w_aspect.getLeft() + w_aspect.getWidth()), 
                        Convert.ToInt32(w_aspect.getTop() - w_aspect.getOffset() + w_aspect.getHeight()), 
                        color, 
                        bCircle);
        }

        private void move(float left, float top, float width, float height, Graphics graph)
        {
            if (m_x1 > 0 || m_x2 > 0 || m_y1 > 0 || m_y2 > 0)
            {
                paintPictureMove(graph, cGlobals.newRectangleF(m_x1, m_y1, m_x2, m_y2));
            }

            m_x1 = left;
            m_y1 = top;
            m_x2 = left + width;
            m_y2 = top + height;

            printLine(graph, false, m_x1, m_y1, m_x2, m_y2, 0, 1, true, (int)csColors.C_COLOR_BLACK, false);

            if (m_x1 > 1) { m_x1 = m_x1 - 2; }
            if (m_y1 > 1) { m_y1 = m_y1 - 2; }

            m_x2 = m_x2 + 2;
            m_y2 = m_y2 + 2;
        }

        public void resize(Graphics graph, String sKey, float left, float top, float x2, float y2)
        {
            const int C_MIN_WIDTH = 1;
            const int C_MIN_HEIGHT = 1;

            CSReportDll.cReportAspect paintObjAsp = null;

            if (sKey.Substring(0, 1) == C_KEY_PAINT_OBJ)
            {
                paintObjAsp = m_paintObjects.item(sKey).getAspect();
            }
            else
            {
                paintObjAsp = m_paintSections.item(sKey).getAspect();
            }

            if ((int)left == -32768)
            {
                m_x1 = paintObjAsp.getLeft();
            }
            else
            {
                m_x1 = left;
            }

            if ((int)top == -32768)
            {
                m_y1 = paintObjAsp.getTop() - paintObjAsp.getOffset();
            }
            else
            {
                m_y1 = top;
            }

            m_x2 = paintObjAsp.getLeft();
            if ((int)x2 == -32768)
            {
                m_x2 = m_x2 + paintObjAsp.getWidth();
            }
            else
            {
                m_x2 = x2;
            }

            m_y2 = paintObjAsp.getTop() - paintObjAsp.getOffset();
            if ((int)y2 == -32768)
            {
                m_y2 = m_y2 + paintObjAsp.getHeight();
            }
            else
            {
                m_y2 = y2;
            }

            // validations :

            // x2 can't be lower than Left
            if (m_x2 < paintObjAsp.getLeft() + C_MIN_WIDTH) { m_x2 = paintObjAsp.getLeft() + C_MIN_WIDTH; }

            // y2 can't be lower than Top
            if (m_y2 < paintObjAsp.getTop() - paintObjAsp.getOffset() + C_MIN_HEIGHT) { m_y2 = paintObjAsp.getTop() - paintObjAsp.getOffset() + C_MIN_HEIGHT; }

            paintPicture(graph, false);

            printLine(graph, false, m_x1, m_y1, m_x2, m_y2, (int)csColors.C_COLOR_WHITE, 1, true, (int)csColors.C_COLOR_BLACK, false);

            graph.Dispose();
        }

        public void createPicture(Graphics graph)
        {
            refreshBackgroundPicture(graph, 0);
        }

        private void refreshBackgroundPicture(Graphics graph, int color)
        {
            /*
            int i = 0;
            RECT tR = null;
            RECT tR2 = null;

            if (m_hBmpCopy != 0) { DeleteObject(m_hBmpCopy); }
            if (m_hMemDC != 0) { DeleteObject(m_hMemDC); }

            GetClientRect(graph.hwnd, tR);

            LSet(tR2 == tR);

            m_hMemDC = CreateCompatibleDC(0);
            m_hBmpCopy = CreateCompatibleBitmap(graph.hDC, tR.right, tR.bottom + 56);

            DeleteObject(SelectObject(m_hMemDC, m_hBmpCopy));

            int hBr = 0;
            hBr = CreateSolidBrush(mAux.translateColor(color));
            tR2.bottom = tR2.bottom + 56;
            FillRect(m_hMemDC, tR2, hBr);
            DeleteObject(hBr);

            tR.bottom = m_gridHeight / Screen.TwipsPerPixelY;
            FillRect(m_hMemDC, tR, m_hBrushGrid);

            for (i = 1; i <= getPaintObjects().count(); i++)
            {
                drawObject(getPaintObjects().getNextKeyForZOrder(i), m_hMemDC, graph);
            }

            for (i = 1; i <= getPaintSections().count(); i++)
            {
                drawSection(getPaintSections().getNextKeyForZOrder(i), m_hMemDC, graph);
            }

            paintPicture(graph);
            */
            if (m_bitmap != null)
            {
                m_bitmap.Dispose();
            }

            m_bitmap = new Bitmap((int)graph.VisibleClipBounds.Width + 1, (int)graph.VisibleClipBounds.Height + 3); // TODO check why 56 ???

            Graphics bitmapGraphic = Graphics.FromImage(m_bitmap);

            Rectangle rect = cGlobals.newRectangle(0, 0, (int)graph.VisibleClipBounds.Width, (int)graph.VisibleClipBounds.Height + 3); // TODO check why 56 ???
            Brush brush = new SolidBrush(cGlobals.colorFromRGB(color));
            bitmapGraphic.FillRectangle(brush, rect);
            brush.Dispose();

            bitmapGraphic.FillRectangle(m_brushGrid, rect);
            
            for (int i = 0; i < getPaintObjects().count(); i++)
            {
                drawObject(getPaintObjects().getNextKeyForZOrder(i), bitmapGraphic);
            }
            
            for (int i = 0; i < getPaintSections().count(); i++)
            {
                drawSection(getPaintSections().getNextKeyForZOrder(i), bitmapGraphic);
            }
            
            paintPicture(graph, true);
        }

        //--------------------------------------------------------------------------------------------------
        // Draw - Low Level
        private void printLine(
            Graphics graph,
            bool filled,
            float x1,
            float y1,
            float x2,
            float y2,
            int colorInside,
            int width,
            bool dash,
            int colorOut,
            bool rounded)
        {
            /*
            RECT tR = null;
            int lResult = 0;
            int hRPen = 0;

            if (dash) {
                hRPen = CreatePen(PS_DOT, width, mAux.translateColor(colorOut));
            } 
            else {
                hRPen = CreatePen(PS_SOLID, width, mAux.translateColor(colorOut));
            }
            DeleteObject(SelectObject(hDC, hRPen));

            if (rounded) {

                x1 = x1 / Screen.TwipsPerPixelX;
                x2 = x2 / Screen.TwipsPerPixelX;

                y1 = y1 / Screen.TwipsPerPixelY;
                y2 = y2 / Screen.TwipsPerPixelY;


                y1 = y1 * m_scaleY;
                y2 = y2 * m_scaleY;
                x1 = x1 * m_scaleX;
                x2 = x2 * m_scaleX;

                RoundRect(hDC, x1, y1, x2, y2, 20 * m_scaleX, 20 * m_scaleY);
            } 
            else {

                tR = cGlobals.newRectangle(x1, y1, x2, y2);
                mAux.rectTwipsToPixel(tR, m_scaleX, m_scaleY);

                if (y2 != y1 && x1 != x2) {

                    Rectangle(hDC, tR.left, tR.top, tR.right, tR.bottom);

                    if (filled) {
                        int hBrush2 = 0;
                        InflateRect(tR, -1, -1);
                        hBrush2 = CreateSolidBrush(mAux.translateColor(colorInside));
                        lResult = FillRect(hDC, tR, hBrush2);
                        DeleteObject(hBrush2);
                    }

                } 
                else {
                    if (tR.bottom == 0 || tR.bottom == tR.top) { tR.bottom = tR.top + 1; }
                    if (tR.right == 0 || tR.left == tR.right) { tR.right = tR.left + 1; }
                    Rectangle(hDC, tR.left, tR.top, tR.right, tR.bottom);
                }
            }

            DeleteObject(hRPen);
             */

            Pen pen;

            pen = new Pen(cGlobals.colorFromRGB(colorOut), width);

            if (dash)
            {
                pen.DashStyle = DashStyle.Dot;
            }

            if (rounded)
            {
                y1 = y1 * m_scaleY;
                y2 = y2 * m_scaleY;
                x1 = x1 * m_scaleX;
                x2 = x2 * m_scaleX;

                cGraphics extGraph = new cGraphics(graph);
                extGraph.DrawRoundRectangle(pen, x1, y1, x2-x1, y2-y1, 8f);
            }
            else
            {
                Rectangle rect = cGlobals.newRectangle(Convert.ToInt32(x1), Convert.ToInt32(y1), Convert.ToInt32(x2), Convert.ToInt32(y2));

                if (y2 != y1 && x1 != x2)
                {
                    if (filled)
                    {
                        /* TODO: remove me after some testing
                         * 
                        if (!(rect.Height == 1 && filled))
                        {
                            rect.Inflate(-1, -1);
                        }
                        */
                        Brush brush = new SolidBrush(cGlobals.colorFromRGB(colorInside));
                        graph.FillRectangle(brush, rect);
                        brush.Dispose();
                    }
                    
                    // the original version didn't put a border when the height is 20 twips
                    // we want to preserve that behaviour
                    //
                    if (!(rect.Height == 1 && filled))
                    {
                        graph.DrawRectangle(pen, rect);
                    }
                }
                else
                {
                    if (rect.Height == 0 || rect.Bottom == rect.Top) { rect.Height = 1; }
                    if (rect.Width == 0 || rect.Left == rect.Right) { rect.Width = 1; }
                    
                    graph.DrawRectangle(pen, rect);
                }
            }

            pen.Dispose();
        }

        private void printText(Graphics graph, String sText, CSReportDll.cReportAspect aspect, Image image)
        { // TODO: Use of ByRef founded Private Sub PrintText(ByVal hDC As Long, ByVal sText As String, ByRef Aspect As cReportAspect, ByVal hImage As Long)
            /*
            // Para separarlo del borde
            Const(c_Margen_Y As Integer == 20);
            Const(c_Margen_X As Integer == 80);
            Const(c_Margen_Bottom As Integer == 80);

            StdFont oFont = null;
            oFont = new StdFont();
            RECT tR = null;

            int oldBkColor = 0;
            int oldBkMode = 0;
            int oldFontColor = 0;
            int flags = 0;

            cReportFont w_font = aspect.getFont();
                oFont.Name = w_font.getName();
                oFont.Bold = w_font.getBold();
                oFont.Italic = w_font.getItalic();
                oFont.UnderLine = w_font.getUnderLine();
                oFont.Size = (w_font.getSize() > 0) ? w_font.getSize() : 3);
            // {end with: w_font}

            int stringWidth = 0;
            int stringHeight = 0;
            int nWidth = 0;

            int hFntOld = 0;

            hFntOld = SelectObject(hDC, m_hFnt[mAux.addFontIfRequired(oFont, hDC, m_fontCount, m_fnt, m_hFnt)]);

            // With aspect;
                oldFontColor = SetTextColor(hDC, mAux.translateColor(aspect.getFont().getForeColor()));
                oldBkColor = SetBkColor(hDC, mAux.translateColor(aspect.getBackColor()));
                oldBkMode = SetBkMode(hDC, aspect.getTransparent() ? C_TRANSPARENT : C_OPAQUE));

                if (aspect.getWordWrap()) {
                    flags = ECGTextAlignFlags.DT_WORDBREAK || ECGTextAlignFlags.DT_WORD_ELLIPSIS || ECGTextAlignFlags.DT_LEFT || ECGTextAlignFlags.DT_NOPREFIX || ECGTextAlignFlags.DT_EDITCONTROL;
                } 
                else {
                    flags = ECGTextAlignFlags.DT_SINGLELINE || ECGTextAlignFlags.DT_WORD_ELLIPSIS || ECGTextAlignFlags.DT_LEFT || ECGTextAlignFlags.DT_NOPREFIX;
                }
            // {end with: aspect}

            stringWidth = getPlEvaluateTextWidth(sText, hDC, m_scaleX);
            stringHeight = getPlEvaluateTextHeight(sText, hDC, aspect.getWidth(), flags, m_scaleY, m_scaleX);

            // Esto es por seguridad, ya que
            // cuando imprimo en la impresora (en pantalla esto no pasa)
            // por pequeñas diferencias en la
            // proceso de escalar hasta la resolucion
            // de la impresora en algunos casos
            // pierdo parte del texto si el
            // rectangulo que pido es demasiado pequeño
            //
            stringHeight = stringHeight + 400;

            int margenX = 0;
            int margenY = 0;
            int width = 0;
            int height = 0;

            margenX = c_Margen_X;
            margenY = c_Margen_Y;

            if (hImage != 0) {
                mAux.getBitmapSize(hImage, width, height);
                margenX = margenX + width;
                margenY = height - stringHeight - c_Margen_Bottom;
                // With aspect;
                    if (margenY + stringHeight > aspect.getHeight()) { margenY = aspect.getHeight() - stringHeight - c_Margen_Bottom; }
                // {end with: aspect}
                if (margenY < c_Margen_Y) { margenY = c_Margen_Y; }
            }

            nWidth = aspect.getWidth() - margenX * 2;
            if (stringWidth > nWidth) { stringWidth = nWidth; }

            int x = 0;
            int y = 0;

            switch (aspect.setAlign()) {
                case  AlignmentConstants.vbRightJustify:
                    x = aspect.getLeft() + aspect.getWidth() - stringWidth - margenX;
                    break;
                case  AlignmentConstants.vbCenter:
                    x = aspect.getLeft() + (aspect.getWidth() - stringWidth) * 0.5;
                    break;
                case  AlignmentConstants.vbLeftJustify:
                    x = aspect.getLeft() + margenX;
                    break;
            }

            y = aspect.getTop() - aspect.setOffset() + margenY;

            // With aspect;
                //'.Height)
                tR = cGlobals.newRectangle(x, y, x + aspect.getWidth() - margenX, y + stringHeight);
                mAux.rectTwipsToPixel(tR, m_scaleX, m_scaleY);
                //    If .WordWrap Then
                //      Flags = DT_WORDBREAK Or DT_WORD_ELLIPSIS Or DT_LEFT Or DT_NOPREFIX Or DT_EDITCONTROL
                //    Else
                //      Flags = DT_SINGLELINE Or DT_WORD_ELLIPSIS Or DT_LEFT Or DT_NOPREFIX
                //    End If
                DrawText(hDC, sText+ vbNullChar, -1, tR, flags);
            // {end with: aspect}

            SetBkColor(hDC, oldBkColor);
            SetTextColor(hDC, oldFontColor);
            SetBkMode(hDC, oldBkMode);

            SelectObject(hDC, hFntOld);
             */

            // padding
            const int c_Margen_Y = 1; // 20 twips;
            const int c_Margen_X = 4; // 80 twips;
            const int c_Margen_Bottom = 4; // 80 twips;
            
            int idx = cGlobals.addFontIfRequired(aspect.getFont(), ref m_fnt);
            
            Font font = m_fnt[idx];

            StringFormat format = new StringFormat();

            format.Trimming = StringTrimming.EllipsisWord;
            format.Alignment = StringAlignment.Near;

            if (!aspect.getWordWrap()) {
                format.FormatFlags = StringFormatFlags.NoWrap;
            }

            int stringWidth = getPlEvaluateTextWidth(sText, font, m_scaleX);
            int stringHeight = getPlEvaluateTextHeight(sText, font, aspect.getWidth(), format, m_scaleY, m_scaleX);

            // TODO: translate this to English if it is really needed
            //
            // Esto es por seguridad, ya que
            // cuando imprimo en la impresora (en pantalla esto no pasa)
            // por pequeñas diferencias en la
            // proceso de escalar hasta la resolucion
            // de la impresora en algunos casos
            // pierdo parte del texto si el
            // rectangulo que pido es demasiado pequeño
            //
            stringHeight += 25; //+ 400 the original code was in twips;

            int margenX = c_Margen_X;
            int margenY = c_Margen_Y;

            if (image != null)
            {
                margenX += image.Size.Width;
                margenY = image.Size.Height - stringHeight - c_Margen_Bottom;
                
                if (margenY + stringHeight > aspect.getHeight()) 
                { 
                    margenY = Convert.ToInt32(aspect.getHeight() - stringHeight - c_Margen_Bottom); 
                }                
                if (margenY < c_Margen_Y) 
                { 
                    margenY = c_Margen_Y; 
                }
            }

            int nWidth = Convert.ToInt32(aspect.getWidth() - margenX * 2);
            
            if (stringWidth > nWidth) 
            { 
                stringWidth = nWidth; 
            }

            int x = 0;
            int y = 0;

            switch (aspect.getAlign())
            {
                case HorizontalAlignment.Right:
                    x = Convert.ToInt32(aspect.getLeft() + aspect.getWidth() - stringWidth - margenX);
                    break;
                case HorizontalAlignment.Center:
                    x = Convert.ToInt32(aspect.getLeft() + (aspect.getWidth() - stringWidth) * 0.5);
                    break;
                case HorizontalAlignment.Left:
                    x = Convert.ToInt32(aspect.getLeft() + margenX);
                    break;
            }

            y = Convert.ToInt32(aspect.getTop() - aspect.getOffset() + margenY);

            RectangleF rect = cGlobals.newRectangleF(x, y, Convert.ToInt32(x + aspect.getWidth() - margenX), y + stringHeight);

            SolidBrush brush = new SolidBrush(cGlobals.colorFromRGB(aspect.getFont().getForeColor()));

            graph.DrawString(sText, font, brush, rect, format);

            brush.Dispose();
        }

        private void showHandles(
            Graphics graph,
            int x1,
            int y1,
            int x2,
            int y2,
            int color,
            bool bCircle)
        {
            /*
            const int iSize = 100;
            int hBrush = 0;
            RECT tR = null;
            int x = 0;
            int y = 0;

            int hOldBrush = 0;
            int hOldPen = 0;
            int hPen = 0;

            hBrush = CreateSolidBrush(mAux.translateColor(color));

            if (bCircle)
            {
                hPen = CreatePen(PS_SOLID, 1, mAux.translateColor(color));
                hOldPen = SelectObject(hDC, hPen);
                hOldBrush = SelectObject(hDC, hBrush);
            }

            if (x1 - iSize < 0) { x1 = iSize; }
            if (y1 - iSize < 0) { y1 = iSize; }

            if (x1 - iSize < 0) { x1 = iSize; }
            if (y1 - iSize < 0) { y1 = iSize; }

            tR = cGlobals.newRectangle(x1 - iSize, y1 - iSize - 10, x1, y1);
            mAux.rectTwipsToPixel(tR, m_scaleX, m_scaleY);
            if (bCircle)
            {
                Ellipse(hDC, tR.left, tR.top, tR.right, tR.bottom);
            }
            else
            {
                FillRect(hDC, tR, hBrush);
            }

            tR = cGlobals.newRectangle(x1 - iSize, y2, x1, y2 + iSize);
            mAux.rectTwipsToPixel(tR, m_scaleX, m_scaleY);
            if (bCircle)
            {
                Ellipse(hDC, tR.left, tR.top, tR.right, tR.bottom);
            }
            else
            {
                FillRect(hDC, tR, hBrush);
            }

            tR = cGlobals.newRectangle(x2, y1 - iSize - 10, x2 + iSize, y1);
            mAux.rectTwipsToPixel(tR, m_scaleX, m_scaleY);
            if (bCircle)
            {
                Ellipse(hDC, tR.left, tR.top, tR.right, tR.bottom);
            }
            else
            {
                FillRect(hDC, tR, hBrush);
            }

            tR = cGlobals.newRectangle(x2, y2, x2 + iSize, y2 + iSize);
            mAux.rectTwipsToPixel(tR, m_scaleX, m_scaleY);
            if (bCircle)
            {
                Ellipse(hDC, tR.left, tR.top, tR.right, tR.bottom);
            }
            else
            {
                FillRect(hDC, tR, hBrush);
            }

            x = x1 + (x2 - x1) / 2;
            x = x - iSize / 2;
            tR = cGlobals.newRectangle(x, y2, x + iSize, y2 + iSize);
            mAux.rectTwipsToPixel(tR, m_scaleX, m_scaleY);
            if (bCircle)
            {
                Ellipse(hDC, tR.left, tR.top, tR.right, tR.bottom);
            }
            else
            {
                FillRect(hDC, tR, hBrush);
            }

            tR = cGlobals.newRectangle(x, y1 - iSize - 10, x + iSize, y1);
            mAux.rectTwipsToPixel(tR, m_scaleX, m_scaleY);
            if (bCircle)
            {
                Ellipse(hDC, tR.left, tR.top, tR.right, tR.bottom);
            }
            else
            {
                FillRect(hDC, tR, hBrush);
            }

            y = y1 + (y2 - y1) / 2;
            y = y - iSize / 2;
            tR = cGlobals.newRectangle(x1 - iSize, y, x1, y + iSize);
            mAux.rectTwipsToPixel(tR, m_scaleX, m_scaleY);
            if (bCircle)
            {
                Ellipse(hDC, tR.left, tR.top, tR.right, tR.bottom);
            }
            else
            {
                FillRect(hDC, tR, hBrush);
            }

            tR = cGlobals.newRectangle(x2, y, x2 + iSize, y + iSize);
            mAux.rectTwipsToPixel(tR, m_scaleX, m_scaleY);
            if (bCircle)
            {
                Ellipse(hDC, tR.left, tR.top, tR.right, tR.bottom);
            }
            else
            {
                FillRect(hDC, tR, hBrush);
            }

            if (bCircle)
            {
                DeleteObject(SelectObject(hDC, hOldPen));
                DeleteObject(SelectObject(hDC, hOldBrush));
            }
            else
            {
                DeleteObject(hBrush);
            }
            */
            const int iSize = 7;

            if (x1 - iSize < 0) { x1 = iSize; }
            if (y1 - iSize < 0) { y1 = iSize; }

            if (x1 - iSize < 0) { x1 = iSize; }
            if (y1 - iSize < 0) { y1 = iSize; }            

            Brush brush = new SolidBrush(cGlobals.colorFromRGB(color));

            Rectangle rect = cGlobals.newRectangle(x1 - iSize, y1 - iSize - 1, x1, y1);
            showHandle(graph, brush, rect, bCircle);

            rect = cGlobals.newRectangle(x1 - iSize, y2, x1, y2 + iSize);
            showHandle(graph, brush, rect, bCircle);

            rect = cGlobals.newRectangle(x2, y1 - iSize - 1, x2 + iSize, y1);
            showHandle(graph, brush, rect, bCircle);

            rect = cGlobals.newRectangle(x2, y2, x2 + iSize, y2 + iSize);
            showHandle(graph, brush, rect, bCircle);

            int x = Convert.ToInt32((x1 + (x2 - x1) / 2f) - iSize / 2f);
            rect = cGlobals.newRectangle(x, y2, x + iSize, y2 + iSize);
            showHandle(graph, brush, rect, bCircle);

            rect = cGlobals.newRectangle(x, y1 - iSize - 1, x + iSize, y1);
            showHandle(graph, brush, rect, bCircle);

            int y = Convert.ToInt32((y1 + (y2 - y1) / 2f) - iSize / 2f);
            rect = cGlobals.newRectangle(x1 - iSize, y, x1, y + iSize);
            showHandle(graph, brush, rect, bCircle);

            rect = cGlobals.newRectangle(x2, y, x2 + iSize, y + iSize);
            showHandle(graph, brush, rect, bCircle);

            brush.Dispose();
        }

        private void showHandle(Graphics graph, Brush brush, Rectangle rect, bool circle) 
        {
            if (circle)
            {
                graph.FillEllipse(brush, rect);
            }
            else
            {
                graph.FillRectangle(brush, rect);
            }
        }

        public void paintPicture(Graphics graph, bool disposeGraphicObject)
        {
            /*
            RECT tR = null;

            GetClientRect(graph.hwnd, tR);

            if (m_zoom == 100)
            {
                BitBlt(graph.hDC, 0, 0, tR.right, tR.bottom, m_hMemDC, 0, 0, vbSrcCopy);
            }
            else
            {
                int width = 0;
                int height = 0;
                int oldStrMode = 0;
                POINTAPI lrPoint = null;

                mAux.getBitmapSize(m_hBmpCopy, width, height, false);

                oldStrMode = SetStretchBltMode(graph.hDC, STRETCH_HALFTONE);

                StretchBlt(graph.hDC, 0, 0, tR.right, tR.bottom, m_hMemDC, 0, 0, width, height, vbSrcCopy);
                SetStretchBltMode(graph.hDC, oldStrMode);
            }

            int i = 0;

            for (i = 1; i <= m_vSelectedKeys.Length; i++)
            {
                setFocusAux(m_vSelectedKeys[i], graph);
            }
            */

            Rectangle rect = cGlobals.newRectangle(0, 0, m_bitmap.Size.Width, m_bitmap.Size.Height);
            if (m_zoom == 100)
            {
                //BitBlt(graph.hDC, 0, 0, tR.right, tR.bottom, m_hMemDC, 0, 0, vbSrcCopy);
                graph.DrawImage(m_bitmap, rect, rect, GraphicsUnit.Pixel);
            }
            else
            {

            }
            for (int i = 0; i < m_vSelectedKeys.Length; i++)
            {
                setFocusAux(m_vSelectedKeys[i], graph);
            }

            if (disposeGraphicObject)
            {
                graph.Dispose();
            }
        }

        public void beginMove()
        {
            if (m_beginMoveDone) { return; }

            m_beginMoveDone = true;

            Graphics graphic = Graphics.FromImage(m_bitmap);
            for (int i = 0; i < m_vSelectedKeys.Length; i++)
            {
                setFocusAux(m_vSelectedKeys[i], graphic);
            }
            graphic.Dispose();
        }

        private void paintPictureMove(Graphics graph, RectangleF tR)
        {
            /*
                      InflateRect(tR, 3, 3);

                      if (tR.left < 0) { tR.left = 0; }
                      if (tR.top < 0) { tR.top = 0; }

                      if (tR.right > graph.Width) {
                          tR.right = graph.Width;
                      }

                      if (tR.bottom > graph.Height) {
                          tR.bottom = graph.Height - tR.top;
                      }

                      InflateRect(tR, 3, 3);

                      mAux.rectTwipsToPixel(tR, m_scaleX, m_scaleY);

                      BitBlt(graph.hDC, tR.left, tR.top, tR.right - tR.left, tR.bottom - tR.top, m_hMemDC, tR.left, tR.top, vbSrcCopy);
             * */
            Rectangle rect = cGlobals.newRectangle(0, 0, m_bitmap.Size.Width, m_bitmap.Size.Height);
            if (m_zoom == 100)
            {
                //BitBlt(graph.hDC, 0, 0, tR.right, tR.bottom, m_hMemDC, 0, 0, vbSrcCopy);
                graph.DrawImage(m_bitmap, rect, rect, GraphicsUnit.Pixel);
            }
        }

        // grid
        //
        private void pCreateBrushGrid(Graphics graph, csETypeGrid typeGrid)
        {
            /*
            int i = 0;
            int hBmp = 0;
            int hMemDC = 0;
            int hBrush = 0;

            hMemDC = CreateCompatibleDC(0);
            hBmp = CreateCompatibleBitmap(graph.hDC, 10, 10);
            SelectObject(hMemDC, hBmp);

            hBrush = CreateSolidBrush(mAux.translateColor(vbWhite));
            FillRect(hMemDC, cGlobals.newRectangle(0, 0, 10, 10), hBrush);

            switch (typeGrid)
            {
                case csEGridLines:
                    for (i = 1; i <= 10; i++)
                    {
                        SetPixel(hMemDC, 0, i, mAux.translateColor(&HC0C0C0));
                        SetPixel(hMemDC, i, 0, mAux.translateColor(&HC0C0C0));
                    }
                    break;
                case csEGridPoints:
                    SetPixel(hMemDC, 1, 1, mAux.translateColor(vbBlack));
                    break;
                case csEGridLinesHorizontal:
                    for (i = 0; i <= 10; i++)
                    {
                        SetPixel(hMemDC, i, 0, mAux.translateColor(&HC0C0C0));
                    }
                    break;
                case csEGridLinesVertical:
                    for (i = 0; i <= 10; i++)
                    {
                        SetPixel(hMemDC, 0, i, mAux.translateColor(&HC0C0C0));
                    }
                    break;
            }


            m_hBrushGrid = CreatePatternBrush(hBmp);
            DeleteObject(hMemDC);
            DeleteObject(hBmp);
            DeleteObject(hBrush);
             */
            
            switch (typeGrid)
            {
                case csETypeGrid.CSEGRIDLINES:
                    m_brushGrid = new HatchBrush(
                                            HatchStyle.Cross,
                                            cGlobals.colorFromRGB(0xC0C0C0),
                                            Color.White);
                    break;
                case csETypeGrid.CSEGRIDPOINTS:
                    m_brushGrid = new HatchBrush(
                                            HatchStyle.DottedGrid,
                                            cGlobals.colorFromRGB(0xC0C0C0),
                                            Color.White);
                    break;
                case csETypeGrid.CSEGRIDLINESHORIZONTAL:
                    m_brushGrid = new HatchBrush(
                                            HatchStyle.Horizontal,
                                            cGlobals.colorFromRGB(0xC0C0C0),
                                            Color.White);
                    break;
                case csETypeGrid.CSEGRIDLINESVERTICAL:
                    m_brushGrid = new HatchBrush(
                                            HatchStyle.Vertical,
                                            cGlobals.colorFromRGB(0xC0C0C0),
                                            Color.White);
                    break;
                case csETypeGrid.CSEGRIDNONE:
                    m_brushGrid = new HatchBrush(
                                            HatchStyle.DottedGrid,
                                            cGlobals.colorFromRGB(0xCCCCCC),
                                            Color.White);
                    break;
            }
        }

        //
        //
        private int getPlEvaluateTextWidth(String text, Font font, float scaleX)
        {
            Graphics graph = Graphics.FromImage(m_bitmap);
            SizeF stringSize = graph.MeasureString(text, font);
            graph.Dispose();
            return Convert.ToInt32(stringSize.Width / scaleX); // TODO: check if it is / or *
        }

        private int getPlEvaluateTextHeight(String text, Font font, float width, StringFormat format, float scaleY, float scaleX)
        {
            Graphics graph = Graphics.FromImage(m_bitmap);
            SizeF stringSize = graph.MeasureString(text, font, Convert.ToInt32(width * scaleX), format);
            graph.Dispose();
            return Convert.ToInt32(stringSize.Height / scaleY); // TODO: check if it is / or * the same function in cReportPrint is using * one has to be wrong
        }

        private void pClearObject(String key, Graphics graph)
        {
            cReportPaintObject oPaintObj = null;

            oPaintObj = m_paintObjects.item(key);

            if (oPaintObj == null) { return; }

            CSReportDll.cReportAspect w_aspect = oPaintObj.getAspect();
            RectangleF tR = cGlobals.newRectangleF(w_aspect.getLeft(), w_aspect.getTop(), w_aspect.getLeft() + w_aspect.getWidth(), w_aspect.getTop() + w_aspect.getHeight());

            if (tR.Right > graph.ClipBounds.Width) { tR.Width = cGlobals.setRectangleWidth(graph.ClipBounds.Width - tR.Left); }
            if (tR.Bottom > graph.ClipBounds.Height) { tR.Height = cGlobals.setRectangleHeight(graph.ClipBounds.Height - tR.Top); }

            //TODO: check
            //mAux.rectTwipsToPixel(tR, m_scaleX, m_scaleY);

            // TODO: replace api call
            /*
            int hBr = 0;
            hBr = CreateSolidBrush(mAux.translateColor(vbWhite));
            FillRect(hDC, tR, hBr);
            DeleteObject(hBr);
             */ 
        }

        private void pDrawObjBox(
            Graphics graph, 
            cReportAspect aspect, 
            float x1,
            float y1,
            float x2,
            float y2, 
            bool filled, 
            int colorIn, 
            int colorOut)
        {

            // m_notBorder is used by preview and printing to indicate the controls must be print a border only
            // when BorderType != NONE
            // 
            if ((m_notBorder == false || filled) || (aspect.getBorderType() != csReportBorderType.CSRPTBSNONE))
            {
                if (aspect.getBorderType() == csReportBorderType.CSRPTBS3D)
                {

                    printLine(graph, filled, x1, y1, x2, y2, colorIn, 0, false, (int)csColors.C_COLOR_WHITE, false);

                    // top
                    //
                    printLine(graph, false, x1, y1, x2, y1, (int)csColors.C_COLOR_WHITE, 1, false, aspect.getBorderColor3d(), false);
                    // down
                    //
                    printLine(graph, false, x1, y2 - 1, x2, y2 - 1, (int)csColors.C_COLOR_WHITE, 1, false, aspect.getBorderColor3dShadow(), false);
                    // left
                    //
                    printLine(graph, false, x1 + 1, y1, x1 + 1, y2, (int)csColors.C_COLOR_WHITE, 1, false, aspect.getBorderColor3d(), false);
                    // right
                    //
                    printLine(graph, false, x2 - 1, y1, x2 - 1, y2, (int)csColors.C_COLOR_WHITE, 1, false, aspect.getBorderColor3dShadow(), false);
                }
                else if (aspect.getBorderRounded())
                {
                    printLine(graph, filled, x1, y1, x2, y2, colorIn, (int)aspect.getBorderWidth(), false, colorOut, true);
                }
                else
                {
                    //
                    // we are in the editor window
                    //
                    // TODO: this is a bug. Then only way to get a border is setting BorderType to CSRPTBS3D or 
                    //       BorderRounded == TRUE
                    //
                    //       when BorderType == CSRPTBSFIXED the border is not drawn
                    //
                    //       we need to fix it but the fix will break all reports so first we need to update
                    //       those reports to set the BorderType to CSRPTBSNONE
                    //
                    bool dash = false;
                    const int borderWidth = 1;

                    if (m_notBorder == false && aspect.getBorderType() == csReportBorderType.CSRPTBSFIXED && !aspect.getBorderRounded())
                    {
                        colorOut = Color.Gray.ToArgb(); // 0xff9966; //Color.LightGray.ToArgb();
                        dash = true;
                    }

                    printLine(graph, filled, x1, y1, x2, y2, colorIn, borderWidth, dash, colorOut, false);
                }
            }
        }

        // Testing

        public Image getBitmap()
        {
            return m_bitmap;
        }

    }

}


/*

http://stackoverflow.com/questions/7507602/best-practice-for-onpaint-invalidate-clipping-and-regions
 
Since a lot of people are viewing this question I will go ahead and answer it to the best of my current knowledge.

The Graphics class supplied with PaintEventArgs is always hard-clipped by the invalidation request. This is usually done by the operating system, but it can be done by your code.

You can't reset this clip or escape from these clip bounds, but you shouldn't need to. When painting, you generally shouldn't care about how it's being clipped unless you desperately need to maximize performance.

The graphics class uses a stack of containers to apply clipping and transformations. You can extend this stack yourself by using Graphics.BeginContainer and Graphics.EndContainer. Each time you begin a container, any changes you make to the Transform or the Clip are temporary and they are applied after any previous Transform or Clip which was configured before the BeginContainer. So essentially, when you get an OnPaint event it has already been clipped and you are in a new container so you can't see the clip (your Clip region or ClipRect will show as being infinite) and you can't break out of those clip bounds.

When the state of your visual objects change (for example, on mouse or keyboard events or reacting to data changes), it's normally fine to simply call Invalidate() which will repaint the entire control. Windows will call OnPaint during moments of low CPU usage. Each call to Invalidate() usually will not always correspond to an OnPaint event. Invalidate could be called multiple times before the next paint. So if 10 properties in your data model change all at once, you can safely call Invalidate 10 times on each property change and you'll likely only trigger a single OnPaint event.

I've noticed you should be careful with using Update() and Refresh(). These force a synchronous OnPaint immediately. They're useful for drawing during a single threaded operation (updating a progress bar perhaps), but using them at the wrong times could lead to excessive and unnecessary painting.

If you want to use clip rectangles to improve performance while repainting a scene, you need not keep track of an aggregated clip area yourself. Windows will do this for you. Just invalidate a rectangle or a region that requires invalidation and paint as normal. For example, if an object that you are painting is moved, each time you want to invalidate it's old bounds and it's new bounds, so that you repaint the background where it originally was in addition to painting it in its new location. You must also take into account pen stroke sizes, etc.

And as Hans Passant mentioned, always use 32bppPArgb as the bitmap format for high resolution images. Here's a code snippet on how to load an image as "high performance":

public static Bitmap GetHighPerformanceBitmap(Image original)
{
    Bitmap bitmap;

    bitmap = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppPArgb);
    bitmap.SetResolution(original.HorizontalResolution, original.VerticalResolution);

    using (Graphics g = Graphics.FromImage(bitmap))
    {
        g.DrawImage(original, new Rectangle(new Point(0, 0), bitmap.Size), new Rectangle(new Point(0, 0), bitmap.Size), GraphicsUnit.Pixel);
    }

    return bitmap;
}
 
 */