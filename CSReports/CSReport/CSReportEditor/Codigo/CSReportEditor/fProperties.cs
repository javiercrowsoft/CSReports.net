using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CSReportEditor
{
    public partial class fProperties : Form
    {
        private bool m_ok;
        private bool m_done;

        private int m_index = 0;
        private int m_fieldType = 0;

        private String m_formulaHide = "";
        private String m_formulaValue = "";

        private String m_formulaName = "";

        private bool m_isAccounting;

        private cMouse m_mouse;

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

        private int[] m_chartIndex = 0;
        private int[] m_chartFieldType = 0;

        private int m_chartGroupIndex = 0;
        private int m_chartGroupFieldType = 0;

        public fProperties()
        {
            InitializeComponent();
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

  private void cb_align_Click(object sender, EventArgs e) {
    m_alignChanged = true;
  }

  private void cb_borderType_Click(object sender, EventArgs e) {
    m_borderTypeChanged = true;
  }

  private void chk_borderRounded_Click(object sender, EventArgs e) {
    m_borderRoundedChanged = true;
  }

  private void chk_formulaHide_Click(object sender, EventArgs e) {
    m_setFormulaHideChanged = true;
  }

  private void chk_formulaValue_Click(object sender, EventArgs e) {
    m_setFormulaValueChanged = true;
  }

  private void cmd_formulaHide_Click(object sender, EventArgs e) {
    bool cancel = false;
    m_formulaName = "Ocultar";
    showFormula(m_formulaHide, out cancel);
    if (!cancel) {
      m_formulaHideChanged = true;
      lb_formulaHide.Text = m_formulaHide;
    }
  }

  private void cmd_formulaValue_Click(object sender, EventArgs e) {
    bool cancel = false;
    m_formulaName = "Valor";
    showFormula(m_formulaValue, out cancel);
    if (!cancel) {
      m_formulaValueChanged = true;
      lbFormulaValue.Text = m_formulaValue;
    }
  }

  private void showFormula(String formula, out bool cancel) {
    Iterator listeners = m_listeners.iterator();
    while(listeners.hasNext()) {
        ((fPropertiesEventI)listeners.next()).showEditFormula(formula, cancel);
    };
  }

  private void opAfterPrint_Click(object sender, EventArgs e) {
    m_whenEvalChanged = true;
  }

  private void opBeforePrint_Click(object sender, EventArgs e) {
    m_whenEvalChanged = true;
  }

  private void tx_Border3D_LostFocus() {
    try {
      shBorder3D.cReportAspect.setBackColor(txBorder3D.csValue);
}
}

  private void tx_Border3D_ButtonClick(bool cancel) {
    try {

      cancel = true;

      //*TODO:** can't found type for with block
      //*With CommDialog
      __TYPE_NOT_FOUND w___TYPE_NOT_FOUND = CommDialog;
      w___TYPE_NOT_FOUND.CancelError = true;
      w___TYPE_NOT_FOUND.Color = txBorder3D.csValue;
      w___TYPE_NOT_FOUND.Flags = cdlCCRGBInit;
      VBA.ex.clear();
      w___TYPE_NOT_FOUND.ShowColor;
      if (VBA.ex.Number != 0) { return; }
      txBorder3D.cReportPaintObject.setText(w___TYPE_NOT_FOUND.Color);

      shBorder3D.cReportAspect.setBackColor(txBorder3D.csValue);
}
}

  private void tx_BorderColor_LostFocus() {
    try {
      shBorderColor.cReportAspect.setBackColor(txBorderColor.csValue);
}
}

  private void tx_BorderColor_ButtonClick(bool cancel) {
    try {

      cancel = true;

      //*TODO:** can't found type for with block
      //*With CommDialog
      __TYPE_NOT_FOUND w___TYPE_NOT_FOUND = CommDialog;
      w___TYPE_NOT_FOUND.CancelError = true;
      w___TYPE_NOT_FOUND.Color = txBorderColor.csValue;
      w___TYPE_NOT_FOUND.Flags = cdlCCRGBInit;
      VBA.ex.clear();
      w___TYPE_NOT_FOUND.ShowColor;
      if (VBA.ex.Number != 0) { return; }
      txBorderColor.cReportPaintObject.setText(w___TYPE_NOT_FOUND.Color);

      shBorderColor.cReportAspect.setBackColor(txBorderColor.csValue);
}
}

  private void tx_BorderShadow_LostFocus() {
    try {
      shBorderShadow.cReportAspect.setBackColor(txBorderShadow.csValue);
}
}

  private void tx_BorderShadow_ButtonClick(bool cancel) {
    try {

      cancel = true;

      //*TODO:** can't found type for with block
      //*With CommDialog
      __TYPE_NOT_FOUND w___TYPE_NOT_FOUND = CommDialog;
      w___TYPE_NOT_FOUND.CancelError = true;
      w___TYPE_NOT_FOUND.Color = txBorderShadow.csValue;
      w___TYPE_NOT_FOUND.Flags = cdlCCRGBInit;
      VBA.ex.clear();
      w___TYPE_NOT_FOUND.ShowColor;
      if (VBA.ex.Number != 0) { return; }
      txBorderShadow.cReportPaintObject.setText(w___TYPE_NOT_FOUND.Color);

      shBorderShadow.cReportAspect.setBackColor(txBorderShadow.csValue);
}
}

  private void tx_BorderWidth_TextChanged(object sender, EventArgs e) {
    m_borderWidthChanged = true;
  }

  private void tx_ChartGroupValue_TextChanged(object sender, EventArgs e) {
    m_chartGroupValueChanged = true;
  }

  private void tx_ChartTop_TextChanged(object sender, EventArgs e) {
    m_chartTopChanged = true;
  }

  private synchronized void txDbField_ButtonClick(bool cancel) { // TODO: Use of ByRef founded Private Sub TxDbField_ButtonClick(ByRef Cancel As Boolean)
    cancel = true;
    bool cancel2 = null;
    Iterator listeners = m_listeners.iterator();
    while(listeners.hasNext()) {
        ((fPropertiesEventI)listeners.next()).showHelpDbField(cancel2);
    };
    if (!cancel2) {
      m_dbFieldChanged = true;
    }
  }

  private synchronized void txDbFieldGroupValue_ButtonClick(bool cancel) {
    cancel = true;
    bool cancel2 = null;
    Iterator listeners = m_listeners.iterator();
    while(listeners.hasNext()) {
        ((fPropertiesEventI)listeners.next()).showHelpChartGroupField(cancel2);
    };
    if (!cancel2) {
      m_chartFieldGroupChanged = true;
    }
  }

  private synchronized void txDbFieldLbl1_ButtonClick(bool cancel) {
    cancel = true;
    bool cancel2 = null;
    Iterator listeners = m_listeners.iterator();
    while(listeners.hasNext()) {
        ((fPropertiesEventI)listeners.next()).showHelpChartField(cancel2, TxDbFieldLbl1, 2);
    };
    if (!cancel2) {
      m_chartFieldLbl1Changed = true;
    }
  }

  private synchronized void txDbFieldLbl2_ButtonClick(bool cancel) {
    cancel = true;
    bool cancel2 = null;
    Iterator listeners = m_listeners.iterator();
    while(listeners.hasNext()) {
        ((fPropertiesEventI)listeners.next()).showHelpChartField(cancel2, TxDbFieldLbl2, 3);
    };
    if (!cancel2) {
      m_chartFieldLbl2Changed = true;
    }
  }

  private synchronized void txDbFieldVal1_ButtonClick(bool cancel) { // TODO: Use of ByRef founded Private Sub TxDbFieldVal1_ButtonClick(ByRef Cancel As Boolean)
    cancel = true;
    bool cancel2 = null;
    Iterator listeners = m_listeners.iterator();
    while(listeners.hasNext()) {
        ((fPropertiesEventI)listeners.next()).showHelpChartField(cancel2, TxDbFieldVal1, 0);
    };
    if (!cancel2) {
      m_chartFieldVal1Changed = true;
    }
  }

  private synchronized void txDbFieldVal2_ButtonClick(bool cancel) { // TODO: Use of ByRef founded Private Sub TxDbFieldVal2_ButtonClick(ByRef Cancel As Boolean)
    cancel = true;
    bool cancel2 = null;
    Iterator listeners = m_listeners.iterator();
    while(listeners.hasNext()) {
        ((fPropertiesEventI)listeners.next()).showHelpChartField(cancel2, TxDbFieldVal2, 1);
    };
    if (!cancel2) {
      m_chartFieldVal2Changed = true;
    }
  }

  private void tx_ForeColor_ButtonClick(bool cancel) { // TODO: Use of ByRef founded Private Sub TxForeColor_ButtonClick(ByRef Cancel As Boolean)
    try {

      cancel = true;

      //*TODO:** can't found type for with block
      //*With CommDialog
      __TYPE_NOT_FOUND w___TYPE_NOT_FOUND = CommDialog;
      w___TYPE_NOT_FOUND.CancelError = true;
      w___TYPE_NOT_FOUND.Color = TxForeColor.csValue;
      w___TYPE_NOT_FOUND.Flags = cdlCCRGBInit;
      VBA.ex.clear();
      w___TYPE_NOT_FOUND.ShowColor;
      if (VBA.ex.Number != 0) { return; }
      TxForeColor.cReportPaintObject.setText(w___TYPE_NOT_FOUND.Color);

      shForeColor.cReportAspect.setBackColor(TxForeColor.csValue);
}
}

  private void tx_ForeColor_LostFocus() {
    try {
      shForeColor.cReportAspect.setBackColor(TxForeColor.csValue);
}
}

  private void tx_BackColor_ButtonClick(bool cancel) { // TODO: Use of ByRef founded Private Sub TxBackColor_ButtonClick(ByRef Cancel As Boolean)
    try {

      cancel = true;
      //*TODO:** can't found type for with block
      //*With CommDialog
      __TYPE_NOT_FOUND w___TYPE_NOT_FOUND = CommDialog;
      w___TYPE_NOT_FOUND.CancelError = true;
      w___TYPE_NOT_FOUND.Color = TxBackColor.csValue;
      VBA.ex.clear();
      w___TYPE_NOT_FOUND.ShowColor;
      if (VBA.ex.Number != 0) { return; }
      TxBackColor.cReportPaintObject.setText(w___TYPE_NOT_FOUND.Color);

      shBackColor.cReportAspect.setBackColor(TxBackColor.csValue);
}
}

  private void tx_BackColor_LostFocus() {
    try {
      shBackColor.cReportAspect.setBackColor(TxBackColor.csValue);
}
}

  private void tx_Font_ButtonClick(bool cancel) { // TODO: Use of ByRef founded Private Sub TxFont_ButtonClick(ByRef Cancel As Boolean)
    try {

      cancel = true;
      //*TODO:** can't found type for with block
      //*With CommDialog
      __TYPE_NOT_FOUND w___TYPE_NOT_FOUND = CommDialog;
      w___TYPE_NOT_FOUND.CancelError = true;
      w___TYPE_NOT_FOUND.Flags = cdlCFBoth || cdlCFEffects;
      w___TYPE_NOT_FOUND.FontName = txFont.cReportPaintObject.getText();
      w___TYPE_NOT_FOUND.FontBold = chkFontBold.cColumnInfo.getValue() == vbChecked;
      w___TYPE_NOT_FOUND.FontItalic = chkFontItalic.cColumnInfo.getValue() == vbChecked;
      w___TYPE_NOT_FOUND.FontUnderline = chkFontUnderline.cColumnInfo.getValue() == vbChecked;
      w___TYPE_NOT_FOUND.FontStrikethru = chkFontStrike.cColumnInfo.getValue() == vbChecked;
      w___TYPE_NOT_FOUND.FontSize = TxFontSize.csValue;
      w___TYPE_NOT_FOUND.Color = TxForeColor.csValue;
      VBA.ex.clear();
      w___TYPE_NOT_FOUND.ShowFont;

      if (VBA.ex.Number != 0) { return; }

      txFont.cReportPaintObject.setText(w___TYPE_NOT_FOUND.FontName);
      chkFontBold.cColumnInfo.setValue(w___TYPE_NOT_FOUND.FontBold ? vbChecked : vbUnchecked));
      chkFontItalic.cColumnInfo.setValue(w___TYPE_NOT_FOUND.FontItalic ? vbChecked : vbUnchecked));
      chkFontUnderline.cColumnInfo.setValue(w___TYPE_NOT_FOUND.FontUnderline ? vbChecked : vbUnchecked));
      chkFontStrike.cColumnInfo.setValue(w___TYPE_NOT_FOUND.FontStrikethru ? vbChecked : vbUnchecked));
      TxFontSize.cReportPaintObject.setText(w___TYPE_NOT_FOUND.FontSize);
      TxForeColor.cReportPaintObject.setText(w___TYPE_NOT_FOUND.Color);


}
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
            tabMain.TabVisible(C_FIELD) = false;
        }

        public void hideTabImage()
        {
            tabMain.TabVisible(C_IMAGE) = false;
        }

        public void hideTabChart()
        {
            tabMain.TabVisible(C_CHART) = false;
        }

        //------------------------------------------------------------------------------------------------------------------

        // setters and getters for no control properties

        //------------------------------------------------------------------------------------------------------------------


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

		public string getDbFieldGroupValue ()
		{
			throw new NotImplementedException ();
		}

		public int getChartGroupFieldType ()
		{
			throw new NotImplementedException ();
		}

		public int getChartGroupIndex ()
		{
			throw new NotImplementedException ();
		}

		public void setDbFieldGroupValue (string sField)
		{
			throw new NotImplementedException ();
		}

		public void setChartGroupFieldType (int nFieldType)
		{
			throw new NotImplementedException ();
		}

		public void setChartGroupIndex (int nIndex)
		{
			throw new NotImplementedException ();
		}

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
        /*
        public System.Windows.Forms.TextBox txDbField
        {
            get
            {
                return tx_dbField;
            }
        }

        public System.Windows.Forms.TextBox txDbFieldGroupValue
        {
            get
            {
                return tx_dbFieldGroupValue;
            }
        }

        public System.Windows.Forms.TextBox txChartTop
        {
            get 
            {
                return tx_chartTop;
            }
        }

        public System.Windows.Forms.TextBox txChartGroupValue
        {
            get
            {
                return tx_chartGroupValue;
            }
        }        
         
        private System.Windows.Forms.TextBox tx_exportColIdx;
        private System.Windows.Forms.CheckBox chk_isFreeCtrl;
        private System.Windows.Forms.CheckBox chk_wordWrap;
        private System.Windows.Forms.CheckBox chk_canGrow;
        private System.Windows.Forms.TextBox tx_width;
        private System.Windows.Forms.TextBox tx_height;
        private System.Windows.Forms.TextBox tx_top;
        private System.Windows.Forms.TextBox tx_left;
        private System.Windows.Forms.TextBox tx_symbol;
        private System.Windows.Forms.TextBox tx_format;
        private System.Windows.Forms.CheckBox chk_transparent;
        private System.Windows.Forms.Label sh_backColor;
        private System.Windows.Forms.TextBox tx_backColor;
        private System.Windows.Forms.CheckBox chk_fontStrike;
        private System.Windows.Forms.CheckBox chk_fontItalic;
        private System.Windows.Forms.Label sh_foreColor;
        private System.Windows.Forms.TextBox tx_foreColor;
        private System.Windows.Forms.CheckBox chk_fontUnderline;
        private System.Windows.Forms.CheckBox chk_fontBold;
        private System.Windows.Forms.ComboBox cb_align;
        private System.Windows.Forms.TextBox tx_fontSize;
        private System.Windows.Forms.TextBox tx_font;
        private System.Windows.Forms.TextBox tx_tag;
        private System.Windows.Forms.TextBox tx_text;
        private System.Windows.Forms.TextBox tx_name;
        private System.Windows.Forms.Button cmd_apply;
        private System.Windows.Forms.Button cmd_cancel;
        private System.Windows.Forms.RadioButton op_afterPrint;
        private System.Windows.Forms.RadioButton op_beforePrint;
        private System.Windows.Forms.TextBox tx_idxGroup;
        private System.Windows.Forms.Label lb_formulaValue;
        private System.Windows.Forms.CheckBox chk_formulaValue;
        private System.Windows.Forms.Button cmd_formulaValue;
        private System.Windows.Forms.Label lb_formulaHide;
        private System.Windows.Forms.CheckBox chk_formulaHide;
        private System.Windows.Forms.Button cmd_formulaHide;
        private System.Windows.Forms.Button cmd_dbField;
        private System.Windows.Forms.TextBox tx_dbField;
        private System.Windows.Forms.PictureBox pic_image;
        private System.Windows.Forms.Button cmd_imageFile;
        private System.Windows.Forms.TextBox tx_imageFile;
        private System.Windows.Forms.CheckBox chk_borderRounded;
        private System.Windows.Forms.TextBox tx_borderWidth;
        private System.Windows.Forms.Label sh_borderShadow;
        private System.Windows.Forms.TextBox tx_borderShadow;
        private System.Windows.Forms.Label sh_border3D;
        private System.Windows.Forms.TextBox tx_border3D;
        private System.Windows.Forms.Label sh_borderColor;
        private System.Windows.Forms.TextBox tx_borderColor;
        private System.Windows.Forms.ComboBox cb_borderType;
        private System.Windows.Forms.ComboBox cb_chartThickness;
        private System.Windows.Forms.ComboBox cb_chartSize;
        private System.Windows.Forms.ComboBox cb_linesType;
        private System.Windows.Forms.ComboBox cb_formatType;
        private System.Windows.Forms.ComboBox cb_type;
        private System.Windows.Forms.CheckBox chk_sort;
        private System.Windows.Forms.CheckBox chk_showOutlines;
        private System.Windows.Forms.CheckBox chk_showBarValues;
        private System.Windows.Forms.TextBox tx_chartTop;
        private System.Windows.Forms.ComboBox cb_colorSerie2;
        private System.Windows.Forms.Button cmd_dbFieldLbl2;
        private System.Windows.Forms.TextBox tx_dbFieldLbl2;
        private System.Windows.Forms.Button cmd_dbFieldVal2;
        private System.Windows.Forms.TextBox tx_dbFieldVal2;
        private System.Windows.Forms.ComboBox cb_colorSerie1;
        private System.Windows.Forms.Button cmd_dbFieldLbl1;
        private System.Windows.Forms.TextBox tx_dbFieldLbl1;
        private System.Windows.Forms.Button cmd_dbFieldVal1;
        private System.Windows.Forms.TextBox tx_dbFieldVal1;
        private System.Windows.Forms.TextBox tx_chartGroupValue;
        private System.Windows.Forms.Button cmd_dbFieldGroupValue;
        private System.Windows.Forms.TextBox tx_dbFieldGroupValue;         
         
         */

    }
}
