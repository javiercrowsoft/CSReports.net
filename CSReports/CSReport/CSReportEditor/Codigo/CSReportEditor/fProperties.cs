using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CSKernelClient;
using CSReportGlobals;

namespace CSReportEditor
{
    public partial class fProperties : Form, cIDatabaseFieldSelector
    {
        private cEditor m_editor;

        private bool m_ok;
        private bool m_done;

        private int m_index = 0;
        private int m_fieldType = 0;

        private String m_formulaHide = "";
        private String m_formulaValue = "";

        private String m_formulaName = "";

        private bool m_isAccounting;

        private cMouseWait m_mouse;

        private bool m_textChanged;
        private bool m_tagChanged;
        private bool m_fontChanged;
        private bool m_foreColorChanged;
        private bool m_backColorChanged;
        private bool m_formatChanged;
        private bool m_leftChanged;
        private bool m_topChanged;
        private bool m_heightChanged;
        private bool m_widthChanged;
        private bool m_symbolChanged;
        private bool m_transparentChanged;
        private bool m_strikeChanged;
        private bool m_underlineChanged;
        private bool m_wordWrapChanged;
        private bool m_italicChanged;
        private bool m_boldChanged;
        private bool m_alignChanged;
        private bool m_fontSizeChanged;
        private bool m_canGrowChanged;
        private bool m_formulaHideChanged;
        private bool m_formulaValueChanged;
        private bool m_idxGroupChanged;
        private bool m_whenEvalChanged;
        private bool m_dbFieldChanged;
        private bool m_setFormulaHideChanged;
        private bool m_setFormulaValueChanged;
        private bool m_pictureChanged;
        private bool m_borderTypeChanged;
        private bool m_border3DChanged;
        private bool m_border3DShadowChanged;
        private bool m_borderRoundedChanged;
        private bool m_borderWidthChanged;
        private bool m_borderColorChanged;

        private bool m_chartFieldVal1Changed;
        private bool m_chartFieldVal2Changed;
        private bool m_chartFieldLbl1Changed;
        private bool m_chartFieldLbl2Changed;
        private bool m_chartSizeChanged;
        private bool m_chartThicknessChanged;
        private bool m_chartColorSerie1Changed;
        private bool m_chartColorSerie2Changed;
        private bool m_chartFormatTypeChanged;
        private bool m_chartLinesTypeChanged;
        private bool m_chartTypeChanged;
        private bool m_chartShowLinesChanged;
        private bool m_chartShowValuesChanged;
        private bool m_chartTopChanged;
        private bool m_chartSortChanged;

        private bool m_chartFieldGroupChanged;
        private bool m_chartGroupValueChanged;

        private bool m_isFreeCtrlChanged;
        private bool m_exportColIdxChanged;

        private int[] m_chartIndex;
        private int[] m_chartFieldType;

        private int m_chartGroupIndex = 0;
        private int m_chartGroupFieldType = 0;

        public fProperties()
        {
            InitializeComponent();

            cb_align.Items.Clear();
            cUtil.listAdd(cb_align, "Left", (int)CSReportGlobals.HorizontalAlignment.Left);
            cUtil.listAdd(cb_align, "Right", (int)CSReportGlobals.HorizontalAlignment.Right);
            cUtil.listAdd(cb_align, "Center", (int)CSReportGlobals.HorizontalAlignment.Center);

            cb_borderType.Items.Clear();
            cUtil.listAdd(cb_borderType, "Flat", (int)csReportBorderType.CSRPTBSFIXED);
            cUtil.listAdd(cb_borderType, "3D", (int)csReportBorderType.CSRPTBS3D);
            cUtil.listAdd(cb_borderType, "(Ninguno)", (int)csReportBorderType.CSRPTBSNONE);

            G.redim(ref m_chartFieldType, 3);
            G.redim(ref m_chartIndex, 3);

            initChart();
        }

        // properties

        public bool getPictureChanged()
        {
            return m_pictureChanged;
        }

        public void setPictureChanged(bool rhs)
        {
            m_pictureChanged = rhs;
        }

        public bool getOk()
        {
            return m_ok;
        }

        public int getIndex()
        {
            return m_index;
        }

        public int getChartGroupIndex()
        {
            return m_chartGroupIndex;
        }

        public int getChartIndex(int idx)
        {
            return m_chartIndex[idx];
        }

        public int getFieldType()
        {
            return m_fieldType;
        }

        public int getChartFieldType(int idx)
        {
            return m_chartFieldType[idx];
        }

        public int getChartGroupFieldType()
        {
            return m_chartGroupFieldType;
        }

        public void setIndex(int rhs)
        {
            m_index = rhs;
        }

        public void setChartGroupIndex(int rhs)
        {
            m_chartGroupIndex = rhs;
        }

        public void setChartIndex(int idx, int rhs)
        {
            m_chartIndex[idx] = rhs;
        }

        public void setFieldType(int rhs)
        {
            m_fieldType = rhs;
        }

        public void setChartGroupFieldType(int rhs)
        {
            m_chartGroupFieldType = rhs;
        }

        public void setChartFieldType(int idx, int rhs)
        {
            m_chartFieldType[idx] = rhs;
        }

        public String getFormulaHide()
        {
            return m_formulaHide;
        }

        public void setFormulaHide(String rhs)
        {
            m_formulaHide = rhs;
        }

        public String getFormulaValue()
        {
            return m_formulaValue;
        }

        public void setFormulaValue(String rhs)
        {
            m_formulaValue = rhs;
        }

        public String getFormulaName()
        {
            return m_formulaName;
        }

        public void setFormulaName(String rhs)
        {
            m_formulaName = rhs;
        }

        public bool getIsAccounting()
        {
            return m_isAccounting;
        }

        public void setIsAccounting(bool rhs)
        {
            m_isAccounting = rhs;
        }

        public bool getTextChanged()
        {
            return m_textChanged;
        }

        public void setTextChanged(bool rhs)
        {
            m_textChanged = rhs;
        }

        public bool getTagChanged()
        {
            return m_tagChanged;
        }

        public void setTagChanged(bool rhs)
        {
            m_tagChanged = rhs;
        }

        public bool getFontChanged()
        {
            return m_fontChanged;
        }

        public void setFontChanged(bool rhs)
        {
            m_fontChanged = rhs;
        }

        public bool getForeColorChanged()
        {
            return m_foreColorChanged;
        }

        public void setForeColorChanged(bool rhs)
        {
            m_foreColorChanged = rhs;
        }

        public bool getBackColorChanged()
        {
            return m_backColorChanged;
        }

        public void setBackColorChanged(bool rhs)
        {
            m_backColorChanged = rhs;
        }

        public bool getFormatChanged()
        {
            return m_formatChanged;
        }

        public void setFormatChanged(bool rhs)
        {
            m_formatChanged = rhs;
        }

        public bool getLeftChanged()
        {
            return m_leftChanged;
        }

        public void setLeftChanged(bool rhs)
        {
            m_leftChanged = rhs;
        }

        public bool getTopChanged()
        {
            return m_topChanged;
        }

        public void setTopChanged(bool rhs)
        {
            m_topChanged = rhs;
        }

        public bool getHeightChanged()
        {
            return m_heightChanged;
        }

        public void setHeightChanged(bool rhs)
        {
            m_heightChanged = rhs;
        }

        public bool getWidthChanged()
        {
            return m_widthChanged;
        }

        public void setWidthChanged(bool rhs)
        {
            m_widthChanged = rhs;
        }

        public bool getSymbolChanged()
        {
            return m_symbolChanged;
        }

        public void setSymbolChanged(bool rhs)
        {
            m_symbolChanged = rhs;
        }

        public bool getTransparentChanged()
        {
            return m_transparentChanged;
        }

        public void setTransparentChanged(bool rhs)
        {
            m_transparentChanged = rhs;
        }

        public bool getStrikeChanged()
        {
            return m_strikeChanged;
        }

        public void setStrikeChanged(bool rhs)
        {
            m_strikeChanged = rhs;
        }

        public bool getUnderlineChanged()
        {
            return m_underlineChanged;
        }

        public void setUnderlineChanged(bool rhs)
        {
            m_underlineChanged = rhs;
        }

        public bool getWordWrapChanged()
        {
            return m_wordWrapChanged;
        }

        public void setWordWrapChanged(bool rhs)
        {
            m_wordWrapChanged = rhs;
        }

        public bool getItalicChanged()
        {
            return m_italicChanged;
        }

        public void setItalicChanged(bool rhs)
        {
            m_italicChanged = rhs;
        }

        public bool getBoldChanged()
        {
            return m_boldChanged;
        }

        public void setBoldChanged(bool rhs)
        {
            m_boldChanged = rhs;
        }

        public bool getAlignChanged()
        {
            return m_alignChanged;
        }

        public void setAlignChanged(bool rhs)
        {
            m_alignChanged = rhs;
        }

        public bool getFontSizeChanged()
        {
            return m_fontSizeChanged;
        }

        public void setFontSizeChanged(bool rhs)
        {
            m_fontSizeChanged = rhs;
        }

        public bool getCanGrowChanged()
        {
            return m_canGrowChanged;
        }

        public void setCanGrowChanged(bool rhs)
        {
            m_canGrowChanged = rhs;
        }

        public bool getFormulaHideChanged()
        {
            return m_formulaHideChanged;
        }

        public void setFormulaHideChanged(bool rhs)
        {
            m_formulaHideChanged = rhs;
        }

        public bool getFormulaValueChanged()
        {
            return m_formulaValueChanged;
        }

        public void setFormulaValueChanged(bool rhs)
        {
            m_formulaValueChanged = rhs;
        }

        public bool getWhenEvalChanged()
        {
            return m_whenEvalChanged;
        }

        public void setWhenEvalChanged(bool rhs)
        {
            m_whenEvalChanged = rhs;
        }

        public bool getIdxGroupChanged()
        {
            return m_idxGroupChanged;
        }

        public void setIdxGroupChanged(bool rhs)
        {
            m_idxGroupChanged = rhs;
        }

        public bool getDbFieldChanged()
        {
            return m_dbFieldChanged;
        }

        public void setDbFieldChanged(bool rhs)
        {
            m_dbFieldChanged = rhs;
        }

        public bool getSetFormulaHideChanged()
        {
            return m_setFormulaHideChanged;
        }

        public void setSetFormulaHideChanged(bool rhs)
        {
            m_setFormulaHideChanged = rhs;
        }

        public bool getSetFormulaValueChanged()
        {
            return m_setFormulaValueChanged;
        }

        public void setSetFormulaValueChanged(bool rhs)
        {
            m_setFormulaValueChanged = rhs;
        }

        public bool getBorderTypeChanged()
        {
            return m_borderTypeChanged;
        }

        public void setBorderTypeChanged(bool rhs)
        {
            m_borderTypeChanged = rhs;
        }

        public bool getBorder3DChanged()
        {
            return m_border3DChanged;
        }

        public void setBorder3DChanged(bool rhs)
        {
            m_border3DChanged = rhs;
        }

        public bool getBorder3DShadowChanged()
        {
            return m_border3DShadowChanged;
        }

        public void setBorder3DShadowChanged(bool rhs)
        {
            m_border3DShadowChanged = rhs;
        }

        public bool getBorderRoundedChanged()
        {
            return m_borderRoundedChanged;
        }

        public void setBorderRoundedChanged(bool rhs)
        {
            m_borderRoundedChanged = rhs;
        }

        public bool getBorderWidthChanged()
        {
            return m_borderWidthChanged;
        }

        public void setBorderWidthChanged(bool rhs)
        {
            m_borderWidthChanged = rhs;
        }

        public bool getBorderColorChanged()
        {
            return m_borderColorChanged;
        }

        public void setBorderColorChanged(bool rhs)
        {
            m_borderColorChanged = rhs;
        }

        public bool getChartFieldVal1Changed()
        {
            return m_chartFieldVal1Changed;
        }

        public void setChartFieldVal1Changed(bool rhs)
        {
            m_chartFieldVal1Changed = rhs;
        }

        public bool getChartFieldVal2Changed()
        {
            return m_chartFieldVal2Changed;
        }

        public void setChartFieldVal2Changed(bool rhs)
        {
            m_chartFieldVal2Changed = rhs;
        }

        public bool getChartFieldLbl1Changed()
        {
            return m_chartFieldLbl1Changed;
        }

        public void setChartFieldLbl1Changed(bool rhs)
        {
            m_chartFieldLbl1Changed = rhs;
        }

        public bool getChartFieldGroupChanged()
        {
            return m_chartFieldGroupChanged;
        }

        public void setChartFieldGroupChanged(bool rhs)
        {
            m_chartFieldGroupChanged = rhs;
        }

        public bool getChartGroupValueChanged()
        {
            return m_chartGroupValueChanged;
        }

        public void setChartGroupValueChanged(bool rhs)
        {
            m_chartGroupValueChanged = rhs;
        }

        public bool getChartFieldLbl2Changed()
        {
            return m_chartFieldLbl2Changed;
        }

        public void setChartFieldLbl2Changed(bool rhs)
        {
            m_chartFieldLbl2Changed = rhs;
        }

        public bool getChartSizeChanged()
        {
            return m_chartSizeChanged;
        }

        public void setChartSizeChanged(bool rhs)
        {
            m_chartSizeChanged = rhs;
        }

        public bool getChartThicknessChanged()
        {
            return m_chartThicknessChanged;
        }

        public void setChartThicknessChanged(bool rhs)
        {
            m_chartThicknessChanged = rhs;
        }

        public bool getChartColorSerie1Changed()
        {
            return m_chartColorSerie1Changed;
        }

        public void setChartColorSerie1Changed(bool rhs)
        {
            m_chartColorSerie1Changed = rhs;
        }

        public bool getChartColorSerie2Changed()
        {
            return m_chartColorSerie2Changed;
        }

        public void setChartColorSerie2Changed(bool rhs)
        {
            m_chartColorSerie2Changed = rhs;
        }

        public bool getChartFormatTypeChanged()
        {
            return m_chartFormatTypeChanged;
        }

        public void setChartFormatTypeChanged(bool rhs)
        {
            m_chartFormatTypeChanged = rhs;
        }

        public bool getChartLinesTypeChanged()
        {
            return m_chartLinesTypeChanged;
        }

        public void setChartLinesTypeChanged(bool rhs)
        {
            m_chartLinesTypeChanged = rhs;
        }

        public bool getChartTypeChanged()
        {
            return m_chartTypeChanged;
        }

        public void setChartTypeChanged(bool rhs)
        {
            m_chartTypeChanged = rhs;
        }

        public bool getChartShowLinesChanged()
        {
            return m_chartShowLinesChanged;
        }

        public void setChartShowLinesChanged(bool rhs)
        {
            m_chartShowLinesChanged = rhs;
        }

        public bool getChartShowValuesChanged()
        {
            return m_chartShowValuesChanged;
        }

        public void setChartShowValuesChanged(bool rhs)
        {
            m_chartShowValuesChanged = rhs;
        }

        public bool getChartTopChanged()
        {
            return m_chartTopChanged;
        }

        public void setChartTopChanged(bool rhs)
        {
            m_chartTopChanged = rhs;
        }

        public bool getChartSortChanged()
        {
            return m_chartSortChanged;
        }

        public void setChartSortChanged(bool rhs)
        {
            m_chartSortChanged = rhs;
        }

        public bool getIsFreeCtrlChanged()
        {
            return m_isFreeCtrlChanged;
        }

        public void setIsFreeCtrlChanged(bool rhs)
        {
            m_isFreeCtrlChanged = rhs;
        }

        public bool getExportColIdxChanged()
        {
            return m_exportColIdxChanged;
        }

        public void setExportColIdxChanged(bool rhs)
        {
            m_exportColIdxChanged = rhs;
        }        

        //------------------------------------------------------------------------------------------------------------------

        // change events

        //------------------------------------------------------------------------------------------------------------------

        private void cb_align_Click(object sender, EventArgs e)
        {
            m_alignChanged = true;
        }

        private void cb_borderType_Click(object sender, EventArgs e)
        {
            m_borderTypeChanged = true;
        }

        private void chk_borderRounded_Click(object sender, EventArgs e)
        {
            m_borderRoundedChanged = true;
        }

        private void chk_formulaHide_Click(object sender, EventArgs e)
        {
            m_setFormulaHideChanged = true;
        }

        private void chk_formulaValue_Click(object sender, EventArgs e)
        {
            m_setFormulaValueChanged = true;
        }

        private void cmd_formulaHide_Click(object sender, EventArgs e)
        {
            m_formulaName = "Ocultar";
            if (m_editor.showEditFormula(ref m_formulaHide))
            {
                m_formulaHideChanged = true;
                lb_formulaHide.Text = m_formulaHide;
            }
        }

        private void cmd_formulaValue_Click(object sender, EventArgs e)
        {
            m_formulaName = "Valor";
            if (m_editor.showEditFormula(ref m_formulaValue))
            {
                m_formulaValueChanged = true;
                lbFormulaValue.Text = m_formulaValue;
            }
        }

        private void op_afterPrint_Click(object sender, EventArgs e)
        {
            m_whenEvalChanged = true;
        }

        private void op_beforePrint_Click(object sender, EventArgs e)
        {
            m_whenEvalChanged = true;
        }

        private void tx_border3D_LostFocus(object sender, EventArgs e)
        {
            try
            {
                shBorder3D.BackColor = Color.FromArgb(Int32.Parse(txBorder3D.Text));
            }
            catch (Exception ignore) { }
        }

        private void cmd_border3D_click(object sender, EventArgs e)
        {
            try
            {
                // TODO: fix me
                /*
                __TYPE_NOT_FOUND w___TYPE_NOT_FOUND = CommDialog;
                w___TYPE_NOT_FOUND.CancelError = true;
                w___TYPE_NOT_FOUND.Color = txBorder3D.csValue;
                w___TYPE_NOT_FOUND.Flags = cdlCCRGBInit;
                VBA.ex.clear();
                w___TYPE_NOT_FOUND.ShowColor;
                if (VBA.ex.Number != 0) { return; }
                txBorder3D.cReportPaintObject.setText(w___TYPE_NOT_FOUND.Color);
                shBorder3D.cReportAspect.setBackColor(txBorder3D.csValue);
                 */
            }
            catch (Exception ignore) { }
        }

        private void tx_borderColor_LostFocus(object sender, EventArgs e)
        {
            try
            {
                shBorderColor.BackColor = Color.FromArgb(Int32.Parse(txBorderColor.Text));
            }
            catch (Exception ignore) { }
        }

        private void cmd_borderColor_Click(object sender, EventArgs e)
        {
            try
            {
                // TODO: fix me
                /*
                __TYPE_NOT_FOUND w___TYPE_NOT_FOUND = CommDialog;
                w___TYPE_NOT_FOUND.CancelError = true;
                w___TYPE_NOT_FOUND.Color = txBorderColor.csValue;
                w___TYPE_NOT_FOUND.Flags = cdlCCRGBInit;
                VBA.ex.clear();
                w___TYPE_NOT_FOUND.ShowColor;
                if (VBA.ex.Number != 0) { return; }
                txBorderColor.cReportPaintObject.setText(w___TYPE_NOT_FOUND.Color);
                shBorderColor.cReportAspect.setBackColor(txBorderColor.csValue);
                 */
            }
            catch (Exception ignore) { }
        }

        private void tx_borderShadow_LostFocus(object sender, EventArgs e)
        {
            try
            {
                shBorderShadow.BackColor = Color.FromArgb(Int32.Parse(txBorderShadow.Text));
            }
            catch (Exception ignore) { }
        }

        private void cmd_borderShadow_Click(object sender, EventArgs e)
        {
            try
            {
                // TODO: fix me
                /*
                __TYPE_NOT_FOUND w___TYPE_NOT_FOUND = CommDialog;
                w___TYPE_NOT_FOUND.CancelError = true;
                w___TYPE_NOT_FOUND.Color = txBorderShadow.csValue;
                w___TYPE_NOT_FOUND.Flags = cdlCCRGBInit;
                VBA.ex.clear();
                w___TYPE_NOT_FOUND.ShowColor;
                if (VBA.ex.Number != 0) { return; }
                txBorderShadow.cReportPaintObject.setText(w___TYPE_NOT_FOUND.Color);
                shBorderShadow.cReportAspect.setBackColor(txBorderShadow.csValue);
                 */
            }
            catch (Exception ignore) { }
        }

        private void tx_BorderWidth_TextChanged(object sender, EventArgs e)
        {
            m_borderWidthChanged = true;
        }

        private void tx_ChartGroupValue_TextChanged(object sender, EventArgs e)
        {
            m_chartGroupValueChanged = true;
        }

        private void tx_ChartTop_TextChanged(object sender, EventArgs e)
        {
            m_chartTopChanged = true;
        }

        private void cmd_dbFieldGroupValue_Click(object sender, EventArgs e)
        {
            /* TODO: fix me
            bool cancel = false;
            Iterator listeners = m_listeners.iterator();
            while(listeners.hasNext()) {
                ((fPropertiesEventI)listeners.next()).showHelpChartGroupField(cancel);
            };
            if (!cancel) {
              m_chartFieldGroupChanged = true;
            }
             * */
        }

        private void cmd_dbFieldLbl1_Click(object sender, EventArgs e)
        {
            /* TODO: fix me
            bool cancel = false;
            Iterator listeners = m_listeners.iterator();
            while(listeners.hasNext()) {
                ((fPropertiesEventI)listeners.next()).showHelpChartField(cancel, TxDbFieldLbl1, 2);
            };
            if (!cancel) {
              m_chartFieldLbl1Changed = true;
            }
             * */
        }

        private void cmd_dbFieldLbl2_Click(object sender, EventArgs e)
        {
            /* TODO: fix me
            bool cancel = false;
            Iterator listeners = m_listeners.iterator();
            while(listeners.hasNext()) {
                ((fPropertiesEventI)listeners.next()).showHelpChartField(cancel, TxDbFieldLbl2, 3);
            };
            if (!cancel) {
              m_chartFieldLbl2Changed = true;
            }
             * */
        }

        private void cmd_dbFieldVal1_Click(object sender, EventArgs e)
        {
            /* TODO: fix me
            bool cancel = false;
            Iterator listeners = m_listeners.iterator();
            while(listeners.hasNext()) {
                ((fPropertiesEventI)listeners.next()).showHelpChartField(cancel, TxDbFieldVal1, 0);
            };
            if (!cancel) {
              m_chartFieldVal1Changed = true;
            }
             * */
        }

        private void cmd_dbFieldVal2_Click(object sender, EventArgs e)
        {
            /* TODO: fix me
            bool cancel = false;
            Iterator listeners = m_listeners.iterator();
            while(listeners.hasNext()) {
                ((fPropertiesEventI)listeners.next()).showHelpChartField(cancel, TxDbFieldVal2, 1);
            };
            if (!cancel) {
              m_chartFieldVal2Changed = true;
            }
             * */
        }

        private void tx_foreColor_LostFocus(object sender, EventArgs e)
        {
            try
            {
                shForeColor.BackColor = Color.FromArgb(Int32.Parse(tx_foreColor.Text));
            }
            catch (Exception ignore) { }
        }

        private void tx_backColor_LostFocus(object sender, EventArgs e)
        {
            try
            {
                shBackColor.BackColor = Color.FromArgb(Int32.Parse(tx_backColor.Text));
            }
            catch (Exception ignore) { }
        }

        //------------------------------------------------------------------------------------------------------------------

        // initializers

        //------------------------------------------------------------------------------------------------------------------

        public void resetChangedFlags()
        {
            m_textChanged = false;
            m_tagChanged = false;
            m_fontChanged = false;
            m_foreColorChanged = false;
            m_backColorChanged = false;
            m_formatChanged = false;
            m_leftChanged = false;
            m_topChanged = false;
            m_heightChanged = false;
            m_widthChanged = false;
            m_symbolChanged = false;
            m_transparentChanged = false;
            m_strikeChanged = false;
            m_underlineChanged = false;
            m_wordWrapChanged = false;
            m_italicChanged = false;
            m_boldChanged = false;
            m_alignChanged = false;
            m_fontSizeChanged = false;
            m_canGrowChanged = false;
            m_formulaHideChanged = false;
            m_formulaValueChanged = false;
            m_idxGroupChanged = false;
            m_whenEvalChanged = false;
            m_dbFieldChanged = false;
            m_setFormulaHideChanged = false;
            m_setFormulaValueChanged = false;
            m_pictureChanged = false;
            m_borderTypeChanged = false;
            m_border3DChanged = false;
            m_border3DShadowChanged = false;
            m_borderRoundedChanged = false;
            m_borderWidthChanged = false;
            m_borderColorChanged = false;

            m_chartFieldGroupChanged = false;
            m_chartFieldLbl1Changed = false;
            m_chartFieldLbl2Changed = false;
            m_chartFieldVal1Changed = false;
            m_chartFieldVal2Changed = false;

            m_chartSizeChanged = false;
            m_chartThicknessChanged = false;
            m_chartColorSerie1Changed = false;
            m_chartColorSerie2Changed = false;
            m_chartFormatTypeChanged = false;
            m_chartLinesTypeChanged = false;
            m_chartTypeChanged = false;
            m_chartShowLinesChanged = false;
            m_chartShowValuesChanged = false;
            m_chartTopChanged = false;
            m_chartTopChanged = false;

            m_chartFieldGroupChanged = false;
            m_chartGroupValueChanged = false;

            m_isFreeCtrlChanged = false;
            m_exportColIdxChanged = false;

        }

        public void hideTabField()
        {
            tab_main.TabPages.Remove(tbpDatabase);
        }

        public void hideTabImage()
        {
            tab_main.TabPages.Remove(tbpImage);
        }

        public void hideTabChart()
        {
            tab_main.TabPages.Remove(tbpChart);
        }

        //------------------------------------------------------------------------------------------------------------------

        // setters and getters for no control properties

        //------------------------------------------------------------------------------------------------------------------

        /*
		public string getFormulaName ()
		{
			throw new NotImplementedException ();
		}

		public int getChartFieldType (int idx)
		{
			throw new NotImplementedException ();
		}

		public int getChartIndex (int idx)
		{
			throw new NotImplementedException ();
		}

		public void setChartFieldType (int idx, int nFieldType)
		{
			throw new NotImplementedException ();
		}

		public void setChartIndex (int idx, int nIndex)
		{
			throw new NotImplementedException ();
		}
        */
		public string getDbFieldGroupValue ()
		{
			throw new NotImplementedException ();
		}
        /*
		public int getChartGroupFieldType ()
		{
			throw new NotImplementedException ();
		}

		public int getChartGroupIndex ()
		{
			throw new NotImplementedException ();
		}
        */
		public void setDbFieldGroupValue (string sField)
		{
			throw new NotImplementedException ();
		}
        /*
		public void setChartGroupFieldType (int nFieldType)
		{
			throw new NotImplementedException ();
		}

		public void setChartGroupIndex (int nIndex)
		{
			throw new NotImplementedException ();
		}
        */

        //------------------------------------------------------------------------------------------------------------------

        // expose controls

        //------------------------------------------------------------------------------------------------------------------

        public System.Windows.Forms.TextBox txExportColIdx
        {
            get
            {
                return tx_exportColIdx;
            }
        }
        public System.Windows.Forms.CheckBox chkIsFreeCtrl
        {
            get
            {
                return chk_isFreeCtrl;
            }
        }
        public System.Windows.Forms.CheckBox chkWordWrap
        {
            get
            {
                return chk_wordWrap;
            }
        }
        public System.Windows.Forms.CheckBox chkCanGrow
        {
            get
            {
                return chk_canGrow;
            }
        }
        public System.Windows.Forms.TextBox txWidth
        {
            get
            {
                return tx_width;
            }
        }
        public System.Windows.Forms.TextBox txHeight
        {
            get
            {
                return tx_height;
            }
        }
        public System.Windows.Forms.TextBox txTop
        {
            get
            {
                return tx_top;
            }
        }
        public System.Windows.Forms.TextBox txLeft
        {
            get
            {
                return tx_left;
            }
        }
        public System.Windows.Forms.TextBox txSymbol
        {
            get
            {
                return tx_symbol;
            }
        }
        public System.Windows.Forms.TextBox txFormat
        {
            get
            {
                return tx_format;
            }
        }
        public System.Windows.Forms.CheckBox chkTransparent
        {
            get
            {
                return chk_transparent;
            }
        }
        public System.Windows.Forms.Label shBackColor
        {
            get
            {
                return sh_backColor;
            }
        }
        public System.Windows.Forms.TextBox txBackColor
        {
            get
            {
                return tx_backColor;
            }
        }
        public System.Windows.Forms.CheckBox chkFontStrike
        {
            get
            {
                return chk_fontStrike;
            }
        }
        public System.Windows.Forms.CheckBox chkFontItalic
        {
            get
            {
                return chk_fontItalic;
            }
        }
        public System.Windows.Forms.Label shForeColor
        {
            get
            {
                return sh_foreColor;
            }
        }
        public System.Windows.Forms.TextBox txForeColor
        {
            get
            {
                return tx_foreColor;
            }
        }
        public System.Windows.Forms.CheckBox chkFontUnderline
        {
            get
            {
                return chk_fontUnderline;
            }
        }
        public System.Windows.Forms.CheckBox chkFontBold
        {
            get
            {
                return chk_fontBold;
            }
        }
        public System.Windows.Forms.ComboBox cbAlign
        {
            get
            {
                return cb_align;
            }
        }
        public System.Windows.Forms.TextBox txFontSize
        {
            get
            {
                return tx_fontSize;
            }
        }
        public System.Windows.Forms.TextBox txFont
        {
            get
            {
                return tx_font;
            }
        }
        public System.Windows.Forms.TextBox txTag
        {
            get
            {
                return tx_tag;
            }
        }
        public System.Windows.Forms.TextBox txText
        {
            get
            {
                return tx_text;
            }
        }
        public System.Windows.Forms.Label lbControl
        {
            get
            {
                return lb_control;
            }
        }
        public System.Windows.Forms.TextBox txName
        {
            get
            {
                return tx_name;
            }
        }
        public System.Windows.Forms.Button cmdApply
        {
            get
            {
                return cmd_apply;
            }
        }
        public System.Windows.Forms.Button cmdCancel
        {
            get
            {
                return cmd_cancel;
            }
        }
        public System.Windows.Forms.RadioButton opAfterPrint
        {
            get
            {
                return op_afterPrint;
            }
        }
        public System.Windows.Forms.RadioButton opBeforePrint
        {
            get
            {
                return op_beforePrint;
            }
        }
        public System.Windows.Forms.TextBox txIdxGroup
        {
            get
            {
                return tx_idxGroup;
            }
        }
        public System.Windows.Forms.Label lbFormulaValue
        {
            get
            {
                return lb_formulaValue;
            }
        }
        public System.Windows.Forms.CheckBox chkFormulaValue
        {
            get
            {
                return chk_formulaValue;
            }
        }
        public System.Windows.Forms.Button cmdFormulaValue
        {
            get
            {
                return cmd_formulaValue;
            }
        }
        public System.Windows.Forms.Label lbFormulaHide
        {
            get
            {
                return lb_formulaHide;
            }
        }
        public System.Windows.Forms.CheckBox chkFormulaHide
        {
            get
            {
                return chk_formulaHide;
            }
        }
        public System.Windows.Forms.Button cmdFormulaHide
        {
            get
            {
                return cmd_formulaHide;
            }
        }
        public System.Windows.Forms.Button cmdDbField
        {
            get
            {
                return cmd_dbField;
            }
        }
        public System.Windows.Forms.TextBox txDbField
        {
            get
            {
                return tx_dbField;
            }
        }
        public System.Windows.Forms.PictureBox picImage
        {
            get
            {
                return pic_image;
            }
        }
        public System.Windows.Forms.Button cmdImageFile
        {
            get
            {
                return cmd_imageFile;
            }
        }
        public System.Windows.Forms.TextBox txImageFile
        {
            get
            {
                return tx_imageFile;
            }
        }
        public System.Windows.Forms.CheckBox chkBorderRounded
        {
            get
            {
                return chk_borderRounded;
            }
        }
        public System.Windows.Forms.TextBox txBorderWidth
        {
            get
            {
                return tx_borderWidth;
            }
        }
        public System.Windows.Forms.Label shBorderShadow
        {
            get
            {
                return sh_borderShadow;
            }
        }
        public System.Windows.Forms.TextBox txBorderShadow
        {
            get
            {
                return tx_borderShadow;
            }
        }
        public System.Windows.Forms.Label shBorder3D
        {
            get
            {
                return sh_border3D;
            }
        }
        public System.Windows.Forms.TextBox txBorder3D
        {
            get
            {
                return tx_border3D;
            }
        }
        public System.Windows.Forms.Label shBorderColor
        {
            get
            {
                return sh_borderColor;
            }
        }
        public System.Windows.Forms.TextBox txBorderColor
        {
            get
            {
                return tx_borderColor;
            }
        }
        public System.Windows.Forms.ComboBox cbBorderType
        {
            get
            {
                return cb_borderType;
            }
        }
        public System.Windows.Forms.ComboBox cbChartThickness
        {
            get
            {
                return cb_chartThickness;
            }
        }
        public System.Windows.Forms.ComboBox cbChartSize
        {
            get
            {
                return cb_chartSize;
            }
        }
        public System.Windows.Forms.ComboBox cbLinesType
        {
            get
            {
                return cb_linesType;
            }
        }
        public System.Windows.Forms.ComboBox cbFormatType
        {
            get
            {
                return cb_formatType;
            }
        }
        public System.Windows.Forms.ComboBox cbType
        {
            get
            {
                return cb_type;
            }
        }
        public System.Windows.Forms.CheckBox chkSort
        {
            get
            {
                return chk_sort;
            }
        }
        public System.Windows.Forms.CheckBox chkShowOutlines
        {
            get
            {
                return chk_showOutlines;
            }
        }
        public System.Windows.Forms.CheckBox chkShowBarValues
        {
            get
            {
                return chk_showBarValues;
            }
        }
        public System.Windows.Forms.TextBox txChartTop
        {
            get
            {
                return tx_chartTop;
            }
        }
        public System.Windows.Forms.ComboBox cbColorSerie2
        {
            get
            {
                return cb_colorSerie2;
            }
        }
        public System.Windows.Forms.Button cmdDbFieldLbl2
        {
            get
            {
                return cmd_dbFieldLbl2;
            }
        }
        public System.Windows.Forms.TextBox txDbFieldLbl2
        {
            get
            {
                return tx_dbFieldLbl2;
            }
        }
        public System.Windows.Forms.Button cmdDbFieldVal2
        {
            get
            {
                return cmd_dbFieldVal2;
            }
        }
        public System.Windows.Forms.TextBox txDbFieldVal2
        {
            get
            {
                return tx_dbFieldVal2;
            }
        }
        public System.Windows.Forms.ComboBox cbColorSerie1
        {
            get
            {
                return cb_colorSerie1;
            }
        }
        public System.Windows.Forms.Button cmdDbFieldLbl1
        {
            get
            {
                return cmd_dbFieldLbl1;
            }
        }
        public System.Windows.Forms.TextBox txDbFieldLbl1
        {
            get
            {
                return tx_dbFieldLbl1;
            }
        }
        public System.Windows.Forms.Button cmdDbFieldVal1
        {
            get
            {
                return cmd_dbFieldVal1;
            }
        }
        public System.Windows.Forms.TextBox txDbFieldVal1
        {
            get
            {
                return tx_dbFieldVal1;
            }
        }
        public System.Windows.Forms.TextBox txChartGroupValue
        {
            get
            {
                return tx_chartGroupValue;
            }
        }
        public System.Windows.Forms.Button cmdDbFieldGroupValue
        {
            get
            {
                return cmd_dbFieldGroupValue;
            }
        }
        public System.Windows.Forms.TextBox txDbFieldGroupValue
        {
            get
            {
                return tx_dbFieldGroupValue;
            }
        }

        private void fProperties_Load(object sender, EventArgs e)
        {
            m_done = false;
            tab_main.SelectedTab = tbpFormat;
            cWindow.centerForm(this);
            m_ok = false;            

            lb_formulaHide.Text = m_formulaHide;
            lb_formulaValue.Text = m_formulaValue;
        }

        private void initChart() 
        {
            cUtil.listAdd(cb_formatType, "BMP", (int)csRptChartFormat.BMP);
            cUtil.listAdd(cb_formatType, "JPG", (int)csRptChartFormat.JPEG);
            cUtil.listAdd(cb_formatType, "GIF", (int)csRptChartFormat.GIF);
            cUtil.listAdd(cb_formatType, "PNG", (int)csRptChartFormat.PNG);
            cUtil.listSetListIndex(cbFormatType, 0);

            cUtil.listAdd(cb_type, "Pie", (int)csRptChartType.PIE);
            cUtil.listAdd(cb_type, "Bar", (int)csRptChartType.BAR);
            cUtil.listSetListIndex(cb_type, 0);

            chk_showOutlines.Checked = true;
            chk_showBarValues.Checked = true;

            pFillColors(cbColorSerie1);
            cUtil.listSetListIndex(cb_colorSerie1, 10);

            pFillColors(cbColorSerie2);
            cUtil.listSetListIndex(cb_colorSerie2, 69);

            cUtil.listAdd(cb_chartSize, "Smallest", 50);
            cUtil.listAdd(cb_chartSize, "Smaller", 100);
            cUtil.listAdd(cb_chartSize, "Small", 150);
            cUtil.listAdd(cb_chartSize, "Medium", 200);
            cUtil.listAdd(cb_chartSize, "Large", 250);
            cUtil.listAdd(cb_chartSize, "Big", 350);
            cUtil.listSetListIndex(cb_chartSize, 3);

            cUtil.listAdd(cb_chartThickness, "None", 0);
            cUtil.listAdd(cb_chartThickness, "Wafer", 2);
            cUtil.listAdd(cb_chartThickness, "Thin", 4);
            cUtil.listAdd(cb_chartThickness, "Medium", 8);
            cUtil.listAdd(cb_chartThickness, "Thick", 16);
            cUtil.listAdd(cb_chartThickness, "Thickest", 32);
            cUtil.listSetListIndex(cb_chartThickness, 2);

            cUtil.listAdd(cb_linesType, "None", (int)csRptChartLineStyle.NONE);
            cUtil.listAdd(cb_linesType, "Horizontal", (int)csRptChartLineStyle.HORIZONTAL);
            cUtil.listAdd(cb_linesType, "Numbered", (int)csRptChartLineStyle.NUMBERED);
            cUtil.listAdd(cb_linesType, "Both", (int)csRptChartLineStyle.BOTH);
            cUtil.listSetListIndex(cb_linesType, 3);

          }

        private void pFillColors(ComboBox cb_list)
        {
            cUtil.listAdd(cb_list, "AliceBlue", (int)0xF0F8FF);
            cUtil.listAdd(cb_list, "AntiqueWhite ", (int)0xFAEBD7);
            cUtil.listAdd(cb_list, "Aqua ", (int)0x00FFFF);
            cUtil.listAdd(cb_list, "Aquamarine ", (int)0x7FFFD4);
            cUtil.listAdd(cb_list, "Azure ", (int)0xF0FFFF);
            cUtil.listAdd(cb_list, "Beige ", (int)0xF5F5DC);
            cUtil.listAdd(cb_list, "Bisque ", (int)0xFFE4C4);
            cUtil.listAdd(cb_list, "Black ", (int)0x000000);
            cUtil.listAdd(cb_list, "BlanchedAlmond ", (int)0xFFEBCD);
            cUtil.listAdd(cb_list, "Blue ", (int)0x0000FF);
            cUtil.listAdd(cb_list, "BlueViolet ", (int)0x8A2BE2);
            cUtil.listAdd(cb_list, "Brown ", (int)0xA52A2A);
            cUtil.listAdd(cb_list, "BurlyWood ", (int)0xDEB887);
            cUtil.listAdd(cb_list, "CadetBlue ", (int)0x5F9EA0);
            cUtil.listAdd(cb_list, "Chartreuse ", (int)0x7FFF00);
            cUtil.listAdd(cb_list, "Chocolate ", (int)0xD2691E);
            cUtil.listAdd(cb_list, "Coral ", (int)0xFF7F50);
            cUtil.listAdd(cb_list, "CornflowerBlue ", (int)0x6495ED);
            cUtil.listAdd(cb_list, "Cornsilk ", (int)0xFFF8DC);
            cUtil.listAdd(cb_list, "Crimson ", (int)0xDC143C);
            cUtil.listAdd(cb_list, "Cyan ", (int)0x00FFFF);
            cUtil.listAdd(cb_list, "DarkBlue ", (int)0x00008B);
            cUtil.listAdd(cb_list, "DarkCyan ", (int)0x008B8B);
            cUtil.listAdd(cb_list, "DarkGoldenrod ", (int)0xB8860B);
            cUtil.listAdd(cb_list, "DarkGray ", (int)0xA9A9A9);
            cUtil.listAdd(cb_list, "DarkGreen ", (int)0x006400);
            cUtil.listAdd(cb_list, "DarkKhaki ", (int)0xBDB76B);
            cUtil.listAdd(cb_list, "DarkMagenta ", (int)0x8B008B);
            cUtil.listAdd(cb_list, "DarkOliveGreen ", (int)0x556B2F);
            cUtil.listAdd(cb_list, "DarkOrange ", (int)0xFF8C00);
            cUtil.listAdd(cb_list, "DarkOrchid ", (int)0x9932CC);
            cUtil.listAdd(cb_list, "DarkRed ", (int)0x8B0000);
            cUtil.listAdd(cb_list, "DarkSalmon ", (int)0xE9967A);
            cUtil.listAdd(cb_list, "DarkSeaGreen ", (int)0x8FBC8B);
            cUtil.listAdd(cb_list, "DarkSlateBlue ", (int)0x483D8B);
            cUtil.listAdd(cb_list, "DarkSlateGray ", (int)0x2F4F4F);
            cUtil.listAdd(cb_list, "DarkTurquoise ", (int)0x00CED1);
            cUtil.listAdd(cb_list, "DarkViolet ", (int)0x9400D3);
            cUtil.listAdd(cb_list, "DeepPink ", (int)0xFF1493);
            cUtil.listAdd(cb_list, "DeepSkyBlue ", (int)0x00BFFF);
            cUtil.listAdd(cb_list, "DimGray ", (int)0x696969);
            cUtil.listAdd(cb_list, "DodgerBlue ", (int)0x1E90FF);
            cUtil.listAdd(cb_list, "Firebrick ", (int)0xB22222);
            cUtil.listAdd(cb_list, "FloralWhite ", (int)0xFFFAF0);
            cUtil.listAdd(cb_list, "ForestGreen ", (int)0x228B22);
            cUtil.listAdd(cb_list, "Fuchsia ", (int)0xFF00FF);
            cUtil.listAdd(cb_list, "Gainsboro ", (int)0xDCDCDC);
            cUtil.listAdd(cb_list, "GhostWhite ", (int)0xF8F8FF);
            cUtil.listAdd(cb_list, "Gold ", (int)0xFFD700);
            cUtil.listAdd(cb_list, "Goldenrod ", (int)0xDAA520);
            cUtil.listAdd(cb_list, "Gray ", (int)0x808080);
            cUtil.listAdd(cb_list, "Green ", (int)0x008000);
            cUtil.listAdd(cb_list, "GreenYellow ", (int)0xADFF2F);
            cUtil.listAdd(cb_list, "Honeydew ", (int)0xF0FFF0);
            cUtil.listAdd(cb_list, "HotPink ", (int)0xFF69B4);
            cUtil.listAdd(cb_list, "IndianRed ", (int)0xCD5C5C);
            cUtil.listAdd(cb_list, "Indigo ", (int)0x4B0082);
            cUtil.listAdd(cb_list, "Ivory ", (int)0xFFFFF0);
            cUtil.listAdd(cb_list, "Khaki ", (int)0xF0E68C);
            cUtil.listAdd(cb_list, "Lavender ", (int)0xE6E6FA);
            cUtil.listAdd(cb_list, "LavenderBlush ", (int)0xFFF0F5);
            cUtil.listAdd(cb_list, "LawnGreen ", (int)0x7CFC00);
            cUtil.listAdd(cb_list, "LemonChiffon ", (int)0xFFFACD);
            cUtil.listAdd(cb_list, "LightBlue ", (int)0xADD8E6);
            cUtil.listAdd(cb_list, "LightCoral ", (int)0xF08080);
            cUtil.listAdd(cb_list, "LightCyan ", (int)0xE0FFFF);
            cUtil.listAdd(cb_list, "LightGoldenrodYellow ", (int)0xFAFAD2);
            cUtil.listAdd(cb_list, "LightGray ", (int)0xD3D3D3);
            cUtil.listAdd(cb_list, "LightGreen ", (int)0x90EE90);
            cUtil.listAdd(cb_list, "LightPink ", (int)0xFFB6C1);
            cUtil.listAdd(cb_list, "LightSalmon ", (int)0xFFA07A);
            cUtil.listAdd(cb_list, "LightSeaGreen ", (int)0x20B2AA);
            cUtil.listAdd(cb_list, "LightSkyBlue ", (int)0x87CEFA);
            cUtil.listAdd(cb_list, "LightSlateGray ", (int)0x778899);
            cUtil.listAdd(cb_list, "LightSteelBlue ", (int)0xB0C4DE);
            cUtil.listAdd(cb_list, "LightYellow ", (int)0xFFFFE0);
            cUtil.listAdd(cb_list, "Lime ", (int)0x00FF00);
            cUtil.listAdd(cb_list, "LimeGreen ", (int)0x32CD32);
            cUtil.listAdd(cb_list, "Linen ", (int)0xFAF0E6);
            cUtil.listAdd(cb_list, "Magenta ", (int)0xFF00FF);
            cUtil.listAdd(cb_list, "Maroon ", (int)0x800000);
            cUtil.listAdd(cb_list, "MediumAquamarine ", (int)0x66CDAA);
            cUtil.listAdd(cb_list, "MediumBlue ", (int)0x0000CD);
            cUtil.listAdd(cb_list, "MediumOrchid ", (int)0xBA55D3);
            cUtil.listAdd(cb_list, "MediumPurple ", (int)0x9370DB);
            cUtil.listAdd(cb_list, "MediumSeaGreen ", (int)0x3CB371);
            cUtil.listAdd(cb_list, "MediumSlateBlue ", (int)0x7B68EE);
            cUtil.listAdd(cb_list, "MediumSpringGreen ", (int)0x00FA9A);
            cUtil.listAdd(cb_list, "MediumTurquoise ", (int)0x48D1CC);
            cUtil.listAdd(cb_list, "MediumVioletRed ", (int)0xC71585);
            cUtil.listAdd(cb_list, "MidnightBlue ", (int)0x191970);
            cUtil.listAdd(cb_list, "MintCream ", (int)0xF5FFFA);
            cUtil.listAdd(cb_list, "MistyRose ", (int)0xFFE4E1);
            cUtil.listAdd(cb_list, "Moccasin ", (int)0xFFE4B5);
            cUtil.listAdd(cb_list, "NavajoWhite ", (int)0xFFDEAD);
            cUtil.listAdd(cb_list, "Navy ", (int)0x000080);
            cUtil.listAdd(cb_list, "OldLace ", (int)0xFDF5E6);
            cUtil.listAdd(cb_list, "Olive ", (int)0x808000);
            cUtil.listAdd(cb_list, "OliveDrab ", (int)0x6B8E23);
            cUtil.listAdd(cb_list, "Orange ", (int)0xFFA500);
            cUtil.listAdd(cb_list, "OrangeRed ", (int)0xFF4500);
            cUtil.listAdd(cb_list, "Orchid ", (int)0xDA70D6);
            cUtil.listAdd(cb_list, "PaleGoldenrod ", (int)0xEEE8AA);
            cUtil.listAdd(cb_list, "PaleGreen ", (int)0x98FB98);
            cUtil.listAdd(cb_list, "PaleTurquoise ", (int)0xAFEEEE);
            cUtil.listAdd(cb_list, "PaleVioletRed ", (int)0xDB7093);
            cUtil.listAdd(cb_list, "PapayaWhip ", (int)0xFFEFD5);
            cUtil.listAdd(cb_list, "PeachPuff ", (int)0xFFDAB9);
            cUtil.listAdd(cb_list, "Peru ", (int)0xCD853F);
            cUtil.listAdd(cb_list, "Pink ", (int)0xFFC0CB);
            cUtil.listAdd(cb_list, "Plum ", (int)0xDDA0DD);
            cUtil.listAdd(cb_list, "PowderBlue ", (int)0xB0E0E6);
            cUtil.listAdd(cb_list, "Purple ", (int)0x800080);
            cUtil.listAdd(cb_list, "Red ", (int)0xFF0000);
            cUtil.listAdd(cb_list, "RosyBrown ", (int)0xBC8F8F);
            cUtil.listAdd(cb_list, "RoyalBlue ", (int)0x4169E1);
            cUtil.listAdd(cb_list, "SaddleBrown ", (int)0x8B4513);
            cUtil.listAdd(cb_list, "Salmon ", (int)0xFA8072);
            cUtil.listAdd(cb_list, "SandyBrown ", (int)0xF4A460);
            cUtil.listAdd(cb_list, "SeaGreen ", (int)0x2E8B57);
            cUtil.listAdd(cb_list, "SeaShell ", (int)0xFFF5EE);
            cUtil.listAdd(cb_list, "Sienna ", (int)0xA0522D);
            cUtil.listAdd(cb_list, "Silver ", (int)0xC0C0C0);
            cUtil.listAdd(cb_list, "SkyBlue ", (int)0x87CEEB);
            cUtil.listAdd(cb_list, "SlateBlue ", (int)0x6A5ACD);
            cUtil.listAdd(cb_list, "SlateGray ", (int)0x708090);
            cUtil.listAdd(cb_list, "Snow ", (int)0xFFFAFA);
            cUtil.listAdd(cb_list, "SpringGreen ", (int)0x00FF7F);
            cUtil.listAdd(cb_list, "SteelBlue ", (int)0x4682B4);
            cUtil.listAdd(cb_list, "Tan ", (int)0xD2B48C);
            cUtil.listAdd(cb_list, "Teal ", (int)0x008080);
            cUtil.listAdd(cb_list, "Thistle ", (int)0xD8BFD8);
            cUtil.listAdd(cb_list, "Tomato ", (int)0xFF6347);
            cUtil.listAdd(cb_list, "Transparent ", (int)0xFFFF);
            cUtil.listAdd(cb_list, "Turquoise ", (int)0x40E0D0);
            cUtil.listAdd(cb_list, "Violet ", (int)0xEE82EE);
            cUtil.listAdd(cb_list, "Wheat ", (int)0xF5DEB3);
            cUtil.listAdd(cb_list, "White ", (int)0xFFFFFF);
            cUtil.listAdd(cb_list, "WhiteSmoke ", (int)0xF5F5F5);
            cUtil.listAdd(cb_list, "Yellow ", (int)0xFFFF00);
            cUtil.listAdd(cb_list, "YellowGreen ", (int)0x9ACD32);
        }

        private void cmd_cancel_Click(object sender, EventArgs e)
        {
            m_ok = false;
            this.Hide();
        }

        private void cmd_foreColor_Click(object sender, EventArgs e)
        {
            picColor(tx_foreColor, sh_foreColor);
        }

        private void cmd_backColor_Click(object sender, EventArgs e)
        {
            picColor(tx_backColor, sh_backColor);
        }

        private void picColor(TextBox txColor, Label shColor)
        {
            // Show the color dialog.
            DialogResult result = colorDialog.ShowDialog();
            // See if user pressed ok.
            if (result == DialogResult.OK)
            {
                // Set form background to the selected color.
                txColor.Text = colorDialog.Color.ToArgb().ToString();
                shColor.BackColor = colorDialog.Color;
            }
        }

        private void cmd_font_Click(object sender, EventArgs e)
        {
            
            fontDialog.ShowEffects = true;

            FontStyle fontStyle = FontStyle.Regular;
            if (chkFontBold.Checked) fontStyle = fontStyle | FontStyle.Bold;
            if (chkFontItalic.Checked) fontStyle = fontStyle | FontStyle.Italic;
            if (chkFontUnderline.Checked) fontStyle = fontStyle | FontStyle.Underline;
            if (chkFontStrike.Checked) fontStyle = fontStyle | FontStyle.Strikeout;

            float fontSize = (float)cUtil.val(txFontSize.Text);
            Font font = new Font(txFont.Text, ((fontSize > 0f) ? fontSize : 3f), fontStyle);

            fontDialog.Font = font;
            fontDialog.Color = cColor.colorFromRGB(cUtil.valAsInt(txForeColor.Text));

	        DialogResult result = fontDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                font = fontDialog.Font;

                txFont.Text = font.Name;
                chkFontBold.Checked = font.Bold;
                chkFontItalic.Checked = font.Italic;
                chkFontUnderline.Checked = font.Underline;
                chkFontStrike.Checked = font.Strikeout;
                txFontSize.Text = font.Size.ToString();
                txForeColor.Text = fontDialog.Color.ToArgb().ToString();
                shForeColor.BackColor = fontDialog.Color;
            }            
        }

        private void cmd_borderColor_Click_1(object sender, EventArgs e)
        {
            picColor(tx_borderColor, sh_borderColor);
        }

        private void cmd_borderColor3d_Click(object sender, EventArgs e)
        {
            picColor(tx_border3D, sh_border3D);
        }

        private void cmd_borderShadowColor_Click(object sender, EventArgs e)
        {
            picColor(tx_borderShadow, sh_borderShadow);
        }

        public void setHandler(cEditor editor)
        {
            m_editor = editor;
        }

        private void cmd_dbField_Click(object sender, EventArgs e)
        {
            if (m_editor.showHelpDbField())
            {
                m_dbFieldChanged = true;
            }
        }
    }
}
