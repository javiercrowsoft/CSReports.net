using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CSKernelClient;
using CSChartServer;
using System.IO;
using System.Drawing;
using System.Data;
using CSReportGlobals;

namespace CSReportDll
{

    public class cReportChart
    {

        private const String C_MODULE = "cReportChart";

        private cReportChartSeries m_series = new cReportChartSeries();
        private csRptChartLineStyle m_chartLineStyle;
        private bool m_chartBarOutline;
        private bool m_chartShowValues;
        private csRptChartPieThickness m_pieThickness;
        private csRptChartPieDiameter m_pieDiameter;
        private csRptChartFormat m_imageFormat = csRptChartFormat.PNG;
        private String m_copyright = "";
        private String m_chartTitle = "";
        private csRptChartType m_chartType;
        private int m_top = 0;
        private bool m_chartCreated;
        private String m_groupFieldName = "";
        private String m_groupValue = "";
        private int m_groupFieldIndex = 0;
        private bool m_sort;
        private Image m_image;

        public String getLastErrorDescription()
        {
            return cError.getLastErrorDescription();
        }

        public String getLastErrorInfoAdd()
        {
            return cError.getLastErrorInfoAdd();
        }

        public String getLastErrorModule()
        {
            return cError.getLastErrorModule();
        }

        public String getLastErrorNumber()
        {
            return cError.getLastErrorNumber();
        }

        public String getLastErrorLine()
        {
            return cError.getLastErrorLine();
        }

        public String getLastErrorFunction()
        {
            return cError.getLastErrorFunction();
        }

        public cReportChartSeries getSeries()
        {
            return m_series;
        }

        public void setSeries(cReportChartSeries rhs)
        {
            m_series = rhs;
        }

        public csRptChartLineStyle getGridLines()
        {
            return m_chartLineStyle;
        }

        public void setGridLines(csRptChartLineStyle value)
        {
            m_chartLineStyle = value;
        }

        public bool getOutlineBars()
        {
            return m_chartBarOutline;
        }

        public void setOutlineBars(bool value)
        {
            m_chartBarOutline = value;
        }

        public bool getShowValues()
        {
            return m_chartShowValues;
        }

        public void setShowValues(bool value)
        {
            m_chartShowValues = value;
        }

        public csRptChartPieThickness getThickness()
        {
            return m_pieThickness;
        }

        public void setThickness(csRptChartPieThickness value)
        {
            m_pieThickness = value;
        }

        public csRptChartPieDiameter getDiameter()
        {
            return m_pieDiameter;
        }

        public void setDiameter(csRptChartPieDiameter value)
        {
            m_pieDiameter = value;
        }

        public csRptChartFormat getFormat()
        {
            return m_imageFormat;
        }

        public void setFormat(csRptChartFormat value)
        {
            m_imageFormat = value;
        }

        public String getCopyRight()
        {
            return m_copyright;
        }

        public void setCopyRight(String value)
        {
            m_copyright = value;
        }

        public String getGroupFieldName()
        {
            return m_groupFieldName;
        }

        public void setGroupFieldName(String value)
        {
            m_groupFieldName = value;
        }

        public String getGroupValue()
        {
            return m_groupValue;
        }

        public void setGroupValue(String value)
        {
            m_groupValue = value;
        }

        public int getGroupFieldIndex()
        {
            return m_groupFieldIndex;
        }

        public void setGroupFieldIndex(int value)
        {
            m_groupFieldIndex = value;
        }

        public String getChartTitle()
        {
            return m_chartTitle;
        }

        public void setChartTitle(String rhs)
        {
            m_chartTitle = rhs;
        }

        public bool getSort()
        {
            return m_sort;
        }

        public void setSort(bool rhs)
        {
            m_sort = rhs;
        }

        public csRptChartType getChartType()
        {
            return m_chartType;
        }

        public void setChartType(csRptChartType rhs)
        {
            m_chartType = rhs;
        }

        public int getTop()
        {
            return m_top;
        }

        public void setTop(int rhs)
        {
            m_top = rhs;
        }

        public bool getChartCreated()
        {
            return m_chartCreated;
        }

        public void setChartCreated(bool rhs)
        {
            m_chartCreated = rhs;
        }

        public Image getImage()
        {
            return m_image;
        }

        public void setImage(Image rhs)
        {
            m_image = rhs;
        }

        public bool makeChartFromRs(DataTable rs, String fileName)
        {
            cError.setSilent(true);
            return make(rs.Rows, "###,###,##0.00", true, fileName);
        }

        internal bool load(CSXml.cXml xDoc, XmlNode nodeObj)
        {
            nodeObj = xDoc.getNodeFromNode(nodeObj, "Chart");

            if (nodeObj != null)
            {
                m_chartLineStyle = (csRptChartLineStyle)xDoc.getNodeProperty(nodeObj, "LineStyle").getValueInt(eTypes.eInteger);
                m_chartBarOutline = xDoc.getNodeProperty(nodeObj, "BarOutline").getValueBool(eTypes.eBoolean);
                m_chartShowValues = xDoc.getNodeProperty(nodeObj, "ShowValues").getValueBool(eTypes.eBoolean);
                m_pieThickness = (csRptChartPieThickness)xDoc.getNodeProperty(nodeObj, "PieThickness").getValueInt(eTypes.eInteger);
                m_pieDiameter = (csRptChartPieDiameter)xDoc.getNodeProperty(nodeObj, "PieDiameter").getValueInt(eTypes.eInteger);
                m_imageFormat = (csRptChartFormat)xDoc.getNodeProperty(nodeObj, "ImageFormat").getValueInt(eTypes.eInteger);
                m_copyright = xDoc.getNodeProperty(nodeObj, "Copyright").getValueString(eTypes.eText);
                m_chartTitle = xDoc.getNodeProperty(nodeObj, "ChartTitle").getValueString(eTypes.eText);
                m_chartType = (csRptChartType)xDoc.getNodeProperty(nodeObj, "ChartType").getValueInt(eTypes.eInteger);
                m_top = xDoc.getNodeProperty(nodeObj, "Top").getValueInt(eTypes.eInteger);
                m_groupValue = xDoc.getNodeProperty(nodeObj, "GroupValue").getValueString(eTypes.eText);
                m_groupFieldName = xDoc.getNodeProperty(nodeObj, "GroupFieldName").getValueString(eTypes.eText);
                m_groupFieldIndex = xDoc.getNodeProperty(nodeObj, "GroupFieldIndex").getValueInt(eTypes.eInteger);
                m_sort = xDoc.getNodeProperty(nodeObj, "Sort").getValueBool(eTypes.eBoolean);

                XmlNode nodeObjAux = null;
                XmlNode nodeObjSerie = null;
                int index = 0;

                nodeObj = xDoc.getNodeFromNode(nodeObj, "Series");

                if (xDoc.nodeHasChild(nodeObj))
                {
                    nodeObjSerie = xDoc.getNodeChild(nodeObj);

                    while (nodeObjSerie != null)
                    {
                        index = index + 1;
                        nodeObjAux = nodeObjSerie;
                        if (!getSeries().add(null, "").load(xDoc, nodeObjAux, index))
                        {
                            return false;
                        }
                        nodeObjSerie = xDoc.getNextNode(nodeObjSerie);
                    }
                }
            }

            return true;
        }

        internal bool save(CSXml.cXml xDoc, XmlNode nodeFather)
        {
            CSXml.cXmlProperty xProperty = null;
            XmlNode nodeObj = null;

            xProperty = new CSXml.cXmlProperty();

            xProperty.setName("Chart");
            nodeObj = xDoc.addNodeToNode(nodeFather, xProperty);

            xProperty.setName("LineStyle");
            xProperty.setValue(eTypes.eInteger, m_chartLineStyle);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("BarOutline");
            xProperty.setValue(eTypes.eBoolean, m_chartBarOutline);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("ShowValues");
            xProperty.setValue(eTypes.eBoolean, m_chartShowValues);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("PieThickness");
            xProperty.setValue(eTypes.eInteger, m_pieThickness);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("PieDiameter");
            xProperty.setValue(eTypes.eInteger, m_pieDiameter);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("ImageFormat");
            xProperty.setValue(eTypes.eInteger, m_imageFormat);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Copyright");
            xProperty.setValue(eTypes.eText, m_copyright);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("ChartTitle");
            xProperty.setValue(eTypes.eText, m_chartTitle);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("ChartType");
            xProperty.setValue(eTypes.eInteger, m_chartType);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Top");
            xProperty.setValue(eTypes.eInteger, m_top);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("GroupFieldName");
            xProperty.setValue(eTypes.eText, m_groupFieldName);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("GroupFieldIndex");
            xProperty.setValue(eTypes.eInteger, m_groupFieldIndex);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("GroupValue");
            xProperty.setValue(eTypes.eText, m_groupValue);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Sort");
            xProperty.setValue(eTypes.eBoolean, m_sort);
            xDoc.addPropertyToNode(nodeObj, xProperty);

            xProperty.setName("Series");
            nodeObj = xDoc.addNodeToNode(nodeObj, xProperty);

            cReportChartSerie serie = null;
            int index = 0;

            for (int _i = 0; _i < m_series.count(); _i++)
            {
                serie = m_series.item(_i);
                index = index + 1;
                serie.save(xDoc, nodeObj, index);
            }

            return true;
        }

        public bool make(DataRowCollection rows, String strFormat, bool bIsForWeb, String fileName)
        {
            // we need to delete any previous work image
            //
            pDestroyImage();

            if (rows == null)
            {
                return false;
            }

            cWebChart chart = new cWebChart();

            chart.newChartType((csRptChartType)m_chartType, m_chartTitle);

            pFill(chart, rows, strFormat);

            chart.setColorPrimary((csColors)m_series.item(0).getColor());
            chart.setLabelPrimary(cReportGlobals.getRealName(m_series.item(0).getValueFieldName()));
            if (m_series.count() > 1)
            {
                chart.setColorAlternate(m_series.item(1).getColor());
                chart.setLabelAlternate(cReportGlobals.getRealName(m_series.item(1).getValueFieldName()));
            }
            chart.setGridLines(m_chartLineStyle);
            chart.setOutlineBars(m_chartBarOutline);
            chart.setShowValues(m_chartShowValues);
            chart.setShowLegend((m_chartType == csRptChartType.BAR) ? false : m_chartShowValues);

            chart.setThickness(m_pieThickness);
            chart.setDiameter(m_pieDiameter);

            if (!bIsForWeb)
            {
                fileName = cUtil.getValidPath(System.IO.Path.GetTempPath()) + "~ChartImage";
            }

            chart.setFormat(m_imageFormat);

            // saveToFile
            chart.setSaveTo(1);
            chart.setFileName(fileName);

            pKillFile(fileName);

            chart.setCopyRight(m_copyright);
            chart.renderWebChartImage();

            if (!bIsForWeb)
            {
                loadChart(fileName);
            }

            m_chartCreated = true;
            return true;

            chart.Dispose();
        }

        private String pGetExt()
        {
            String _rtn = "";
            switch (m_imageFormat)
            {
                case csRptChartFormat.BMP:
                    _rtn = ".bmp";
                    break;
                case csRptChartFormat.JPEG:
                    _rtn = ".jpg";
                    break;
                case csRptChartFormat.GIF:
                    _rtn = ".gif";
                    break;
                case csRptChartFormat.PNG:
                    _rtn = ".png";
                    break;
            }
            return _rtn;
        }

        private void pKillFile(String fileName)
        {
            try { File.Delete(fileName); }
            catch { }
        }

        private void loadChart(String fileName)
        {
            // we need to delete any previous work image
            //
            pDestroyImage();

            if (fileName.Length > 0)
            {
                Image image = Image.FromFile(fileName);
            }
        }

        private void pDestroyImage()
        {
            m_chartCreated = false;
        }

        private void pGetSerieValues(
            DataRowCollection rows,
            t_SerieValue[] v,
            int valueIndex,
            int labelIndex,
            bool bOthers)
        {
            int i = 0;
            int j = 0;
            int q = 0;
            double value = 0;
            bool bFound = false;
            bool bCompare = false;
            int newTop = 0;

            if (m_groupFieldIndex >= 0)
            {
                // TODO: we need the rows dimension. remeber rows is a matrix (cols by rows)
                for (j = 0; j < rows.Count; j++)
                {
                    if (cReportGlobals.valVariant(rows[j][m_groupFieldIndex]) == m_groupValue)
                    {
                        newTop++;
                    }
                }

                if (newTop > 0) { newTop--; }

                if (v.Length > newTop)
                {
                    pRedimPreserve(ref v, newTop);
                }
            }

            if (m_sort)
            {

                if (m_groupFieldIndex >= 0)
                {
                    // TODO: we need the rows dimension. remeber rows is a matrix (cols by rows)
                    for (j = 0; j < rows.Count; j++)
                    {

                        if ((String)cReportGlobals.valVariant(rows[j][m_groupFieldIndex]) == m_groupValue)
                        {
                            v[0].value = (double)cReportGlobals.valVariant(rows[j][valueIndex]);
                            v[0].label = (String)cReportGlobals.valVariant(rows[j][labelIndex]);
                            v[0].idx = j;
                            break;
                        }
                    }

                }
                else
                {
                    v[0].value = (double)cReportGlobals.valVariant(rows[0][valueIndex]);
                    v[0].label = (String)cReportGlobals.valVariant(rows[0][labelIndex]);
                    v[0].idx = 0;
                }
                // TODO: we need the rows dimension. remeber rows is a matrix (cols by rows)
                for (j = 0; j < rows.Count; j++)
                {

                    if (m_groupFieldIndex >= 0)
                    {
                        bCompare = (String)cReportGlobals.valVariant(rows[j][m_groupFieldIndex]) == m_groupValue;
                    }
                    else
                    {
                        bCompare = true;
                    }

                    if (bCompare)
                    {
                        value = cReportGlobals.val(cReportGlobals.valVariant(rows[j][valueIndex]));

                        if (value > v[0].value)
                        {
                            v[0].value = value;
                            v[0].label = (String)cReportGlobals.valVariant(rows[j][labelIndex]);
                            v[0].idx = j;
                        }
                    }
                }

                for (i = 0; i < v.Length; i++)
                {

                    v[i].idx = -1;
                    // TODO: we need the rows dimension. remeber rows is a matrix (cols by rows)
                    for (j = 0; j < rows.Count; j++)
                    {

                        if (m_groupFieldIndex >= 0)
                        {
                            bCompare = (String)cReportGlobals.valVariant(rows[j][m_groupFieldIndex]) == m_groupValue;
                        }
                        else
                        {
                            bCompare = true;
                        }

                        if (bCompare)
                        {
                            value = cReportGlobals.val(cReportGlobals.valVariant(rows[j][valueIndex]));

                            if ((value > v[i].value || v[i].idx == -1)
                                && value <= v[i - 1].value && j != v[i - 1].idx)
                            {

                                bFound = false;
                                for (q = 0; q <= i; q++)
                                {
                                    if (j == v[q].idx)
                                    {
                                        bFound = true;
                                        break;
                                    }
                                }

                                if (!bFound)
                                {
                                    v[i].value = value;
                                    v[i].label = cReportGlobals.valVariant(rows[j][labelIndex]).ToString();
                                    v[i].idx = j;
                                }
                            }
                        }
                    }
                }

            }
            else
            {
                i = 0;
                // TODO: we need the rows dimension. remeber rows is a matrix (cols by rows)
                for (j = 0; j < rows.Count; j++)
                {
                    if (m_groupFieldIndex >= 0)
                    {
                        if ((String)cReportGlobals.valVariant(rows[j][m_groupFieldIndex]) == m_groupValue)
                        {
                            if (pGetSerieValuesAux(rows, v, valueIndex, labelIndex, i, j, false)) { break; }
                        }
                    }
                    else
                    {
                        if (pGetSerieValuesAux(rows, v, valueIndex, labelIndex, i, j, false)) { break; }
                    }
                }

                if (bOthers)
                {
                    // TODO: we need the rows dimension. remeber rows is a matrix (cols by rows)
                    if (rows.Count > v.Length)
                    {
                        int n = 0;
                        int k = 0;
                        bool bHaveToRedim = false;
                        bHaveToRedim = true;
                        n = v.Length + 1;
                        // TODO: we need the rows dimension. remeber rows is a matrix (cols by rows)
                        for (j = 0; j < rows.Count; j++)
                        {
                            if (m_groupFieldIndex >= 0)
                            {
                                if ((String)cReportGlobals.valVariant(rows[j][m_groupFieldIndex]) == m_groupValue)
                                {
                                    if (k >= n)
                                    {
                                        if (bHaveToRedim)
                                        {
                                            pRedimPreserve(ref v, n);
                                            bHaveToRedim = false;
                                        }
                                        pGetSerieValuesAux(rows, v, valueIndex, labelIndex, v.Length, j, true);
                                    }
                                    else
                                    {
                                        k = k + 1;
                                    }
                                }
                            }
                            else
                            {
                                if (bHaveToRedim)
                                {
                                    pRedimPreserve(ref v, n);
                                    bHaveToRedim = false;
                                }
                                pGetSerieValuesAux(rows, v, valueIndex, labelIndex, v.Length, j, true);
                            }
                        }
                    }
                }
            }
        }

        private bool pGetSerieValuesAux(
            DataRowCollection rows,
            t_SerieValue[] v,
            int valueIndex,
            int labelIndex,
            int i,
            int j,
            bool bAdd)
        {
            if (bAdd)
            {
                v[i].value = v[i].value + (double)cReportGlobals.valVariant(rows[j][valueIndex]);
            }
            else
            {
                v[i].value = (double)cReportGlobals.valVariant(rows[j][valueIndex]);
            }
            v[i].label = (String)cReportGlobals.valVariant(rows[j][labelIndex]);
            v[i].idx = j;
            i = i + 1;
            return i > v.Length;
        }

        private void pFill(cWebChart chart, DataRowCollection rows, String strFormat) 
        {
            int i = 0;
            t_SerieValue[] values = null;
            cReportChartSerie serie = null;
            int idxSerie = 0;

            if (m_top == 0) { m_top = 50; }

            // TODO: we need the rows dimension. remeber rows is a matrix (cols by rows)
            if (rows.Count < 0) { return; }

            // TODO: we need the rows dimension. remeber rows is a matrix (cols by rows)
            if (rows.Count < m_top) {
                // TODO: we need the rows dimension. remeber rows is a matrix (cols by rows)
                pRedim(ref values, rows.Count);
            } 
            else {
                pRedim(ref values, m_top - 1);
            }

            for (int _i = 0; _i < m_series.count(); _i++) {
                serie = m_series.item(_i);

                // At the time we only support two series
                //
                idxSerie = idxSerie + 1;
                if (idxSerie > 2) { return; }

                pGetSerieValues(rows, 
                                values, 
                                serie.getValueIndex(), 
                                serie.getLabelIndex(), 
                                m_chartType == csRptChartType.PIE);

                for (i = 0; i < values.Length; i++) {

                    if (values[i].idx != -1) {
                        if (idxSerie == 1) {
                            cWebChartItem w_add = chart.getItems().add(null);
                            w_add.setPrimaryValue(values[i].value);
                            w_add.setPrimaryLabel(cReportGlobals.format(values[i].label, strFormat));
                            w_add.setPieLabel(cReportGlobals.format(values[i].label, strFormat));
                            w_add.setAlternateValue(0);
                        } 
                        else if (idxSerie == 2) {
                            cWebChartItem w_item = chart.getItems().item(i);
                            w_item.setAlternateValue(values[i].value);
                            w_item.setPieLabel(cReportGlobals.format(values[i].label, strFormat));
                            w_item.setAltLabel(cReportGlobals.format(values[i].label, strFormat));
                        }
                    }
                }

                if ((values.Length > m_top - 1) && m_chartType == csRptChartType.PIE) {

                    cWebChartItem w_item = chart.getItems().item(chart.getItems().count()-1);
                    w_item.setPrimaryLabel("Otros");
                    w_item.setPieLabel("Otros");
                }

            }

            if (chart.getItems().count() > 0) {
                chart.getItems().item(0).setExplode(true);
            }
        }

        private static void pRedimPreserve(ref t_SerieValue[] vSeries, int size)
        {
            if (size == 0)
            {
                vSeries = null;
            }
            else
            {
                if (vSeries == null)
                {
                    vSeries = new t_SerieValue[size];
                }
                else if (vSeries.Length == 0)
                {
                    vSeries = new t_SerieValue[size];
                }
                else
                {
                    t_SerieValue[] newArray = new t_SerieValue[size];
                    Array.Copy(vSeries, newArray, vSeries.Length);
                    vSeries = newArray;
                }
            }
        }

        private static void pRedim(ref t_SerieValue[] vSeries, int size)
        {
            if (size == 0)
            {
                vSeries = null;
            }
            else
            {
                vSeries = new t_SerieValue[size];
            }
        }

        private class t_SerieValue
        {
            public String label;
            public Double value;
            public long idx;
        }

    }
}
