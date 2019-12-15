using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using CSReportGlobals;

namespace CSReportDll
{

    public class cReportFormulaTypes : NameObjectCollectionBase
    {

        // Creates an empty collection.
        public cReportFormulaTypes()
        {
            initialize();
        }

        // Adds elements from an IDictionary into the new collection.
        public cReportFormulaTypes(IDictionary d, Boolean bReadOnly)
        {
            foreach (DictionaryEntry de in d)
            {
                this.BaseAdd((String)de.Key, de.Value);
            }
            this.IsReadOnly = bReadOnly;
        }

        // Gets a key-and-value pair (DictionaryEntry) using an index.
        public DictionaryEntry this[int index]
        {
            get
            {
                return (new DictionaryEntry(
                    this.BaseGetKey(index), this.BaseGet(index)));
            }
        }

        // Gets or sets the value associated with the specified key.
        public Object this[String key]
        {
            get
            {
                return (this.BaseGet(key));
            }
            set
            {
                this.BaseSet(key, value);
            }
        }

        // Gets a String array that contains all the keys in the collection.
        public String[] AllKeys
        {
            get
            {
                return (this.BaseGetAllKeys());
            }
        }

        // Gets an Object array that contains all the values in the collection.
        public Array AllValues
        {
            get
            {
                return (this.BaseGetAllValues());
            }
        }

        // Gets a String array that contains all the values in the collection.
        public String[] AllStringValues
        {
            get
            {
                return ((String[])this.BaseGetAllValues(typeof(String)));
            }
        }

        // Gets a value indicating if the collection contains keys that are not null.
        public Boolean HasKeys
        {
            get
            {
                return (this.BaseHasKeys());
            }
        }

        // Adds an entry to the collection.
        public void Add(String key, Object value)
        {
            this.BaseAdd(key, value);
        }

        // Removes an entry with the specified key from the collection.
        public void Remove(String key)
        {
            this.BaseRemove(key);
        }

        // Removes an entry in the specified index from the collection.
        public void Remove(int index)
        {
            this.BaseRemoveAt(index);
        }

        // Clears all the elements in the collection.
        public void Clear()
        {
            this.BaseClear();
        }

        // Removes an entry with the specified key from the collection.
        public void remove(String key)
        {
            this.BaseRemove(key);
        }

        // Removes an entry in the specified index from the collection.
        public void remove(int index)
        {
            this.BaseRemoveAt(index);
        }

        // Clears all the elements in the collection.
        public void clear()
        {
            this.BaseClear();
        }

        public int count()
        {
            return this.Count;
        }

        public cReportFormulaType item(String key)
        {
            try
            {
                return (cReportFormulaType)this.BaseGet(key);
            }
            catch
            {
                return null;
            }
        }

        public cReportFormulaType item(int index)
        {
            try
            {
                return (cReportFormulaType)this.BaseGet(index);
            }
            catch
            {
                return null;
            }
        }

        internal cReportFormulaType add(cReportFormulaType c, csRptFormulaType key)
        {
            return add(c, key.ToString());
        }
        internal cReportFormulaType add(cReportFormulaType c, String key)
        {
            try
            {
                if (c == null)
                {
                    c = new cReportFormulaType();
                }
                if (key == "")
                {
                    key = cReportGlobals.getNextKey().ToString();
                }
                else
                {
                    cReportGlobals.refreshNextKey(key);
                }

                key = cReportGlobals.getKey(key);

                Add(key, c);

                return c;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void initialize()
        {

            const string C_LANGUAGE_DESCRIPT = "language: 1 Spanish, 2 English y 3 French";
            const string C_CONTROL_NAME_DESCRIPT = "control_name: an string which identifies the control.";
            const string C_COMPARE_DESCRIPT = "It returns a boolean after comparing a control's value with the second argument ";
            const string C_VALUE_TO_COMPARE_DESCRIPT = "value: a number or a text to by compared with.";
            const string C_GROUP_FUNCTION_DESCRIPT = "It function calculates its value before processing the group."
                                                    + "\r\nWhen CSReport found this function it iterates through the "
                                                    + "main recordset to the last row in the group and calculates "
                                                    + "the $1  of the values in the column refered by the "
                                                    + "column_name parameter.";
            const string C_COLUMN_NAME = "column_name: name of the column in the main recordset.\r\n";
            const string C_GROUP_INDEX = "group_index: index of the group"
                                        + "\r\nWhen group_index is equal to -1 the index of the group section in which the control is contained will be used."
                                        + "\r\nWhen group_index is equal to 0 the $1 of the column of every row in the main recordset will be returned."
                                        + "\r\nWhen group_index is greater than zero the $1 of the column of every row in the main recordset contained in the group which index is equal to index_group will be returned.";

            const string C_GROUP_FUNCTION_DESCRIPT2 = "It function calculates its value before processing the group."
                                                    + "\r\nWhen CSReport found this function it iterates through the "
                                                    + "main recordset to the last row in the group and calculates "
                                                    + "the $1.";
            const string C_COLUMN_NAME1 = "column_name1: name of the column in the main recordset to summarize.\r\n";
            const string C_COLUMN_NAME2 = "column_name2: name of the column in the main recordset to compare with the total.\r\n";
            const string C_GROUP_INDEX2 = "group_index: index of the group"
                                        + "\r\nWhen group_index is equal to -1 the index of the group section in which the control is contained will be used."
                                        + "\r\nWhen group_index is equal to 0 the $1 will be evaluated using every row in the main recordset."
                                        + "\r\nWhen group_index is greater than zero the $1 will be evaluated using every row contained in the group which index is equal to index_group.\r\n";

            // we load the collection with all the predefined functions

            //----------------
            // A

            cReportFormulaType fi = add(null, csRptFormulaType.CSRPTF_SET_VAR);
            fi.setName("_setvar");
            fi.setNameUser("Set a variable");
            fi.setDecrip("It sets the value of a variable.\r\n\r\nSyntax: _setVar(variable_name, value)");
            fi.setId(csRptFormulaType.CSRPTF_SET_VAR);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_SET_VAR);

            //----------------
            // B

            fi = add(null, csRptFormulaType.CSRPTF_GET_BARCODE);
            fi.setName("_getbarcode");
            fi.setNameUser("Takes an string and returns a barcode");
            fi.setDecrip("It returns a barcode.\r\n\r\nSyntax: _getBarcode(value)");
            fi.setId(csRptFormulaType.CSRPTF_GET_BARCODE);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_GET_BARCODE);

            //----------------
            // C

            fi = add(null, csRptFormulaType.CSRPTF_CALCULO);
            fi.setName("_calculo");
            fi.setNameUser("Calculo");
            fi.setDecrip("It returns a double after applying an aritmetical operation to ther first two arguments.\r\n\r\nSyntax: _calc(control_1, control_2, value, operator)\n1 addition, 2 substraction, 3 multiplication, 4 division, 5 power");
            fi.setId(csRptFormulaType.CSRPTF_CALCULO);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_CALCULO);

            fi = add(null, csRptFormulaType.CSRPTF_TOTAL_PAGES);
            fi.setName("_totalPages");
            fi.setNameUser("Page count");
            fi.setDecrip("It returns an int with the amount of pages in the report.\r\n\r\nSyntax: _totalPages()");
            fi.setId(csRptFormulaType.CSRPTF_TOTAL_PAGES);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_TOTAL_PAGES);

            fi = add(null, csRptFormulaType.CSRPTF_COUNT);
            fi.setName("_count");
            fi.setNameUser("Record count");
            fi.setDecrip("It returns an int with the amount of rows in the main recordset of the report.\r\n\r\nSyntax: _count()");
            fi.setId(csRptFormulaType.CSRPTF_COUNT);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_COUNT);

            //----------------
            // D

            fi = add(null, csRptFormulaType.CSRPTF_DECLARE_VAR);
            fi.setName("_declareVar");
            fi.setNameUser("Declare a variable");
            fi.setDecrip("It declars a variable.\r\n\r\nSyntax: _declareVar(variable_name)");
            fi.setId(csRptFormulaType.CSRPTF_DECLARE_VAR);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_DECLARE_VAR);

            //----------------
            // E

            fi = add(null, csRptFormulaType.CSRPTF_IS_EQUAL);
            fi.setName("_isEqual");
            fi.setNameUser("Equal to");
            fi.setDecrip(C_COMPARE_DESCRIPT + "\r\n\r\nSyntax: _isEqual(control_name, value)\r\n" + C_CONTROL_NAME_DESCRIPT + "\r\n" + C_VALUE_TO_COMPARE_DESCRIPT);
            fi.setId(csRptFormulaType.CSRPTF_IS_EQUAL);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_IS_EQUAL);

            fi = add(null, csRptFormulaType.CSRPTF_IS_NOT_EQUAL);
            fi.setName("_isNotEqual");
            fi.setNameUser("It is not equal to");
            fi.setDecrip(C_COMPARE_DESCRIPT + "\r\n\r\nSyntax: _isNotEqual(control_name, value)\r\n" + C_CONTROL_NAME_DESCRIPT + "\r\n" + C_VALUE_TO_COMPARE_DESCRIPT);
            fi.setId(csRptFormulaType.CSRPTF_IS_NOT_EQUAL);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_IS_NOT_EQUAL);

            fi = add(null, csRptFormulaType.CSRPTF_IS_GREATER_THAN);
            fi.setName("_isGreaterThan");
            fi.setNameUser("It is greater than");
            fi.setDecrip(C_COMPARE_DESCRIPT + "\r\n\r\nSyntax: _isGreaterThan(control_name, value)\r\n" + C_CONTROL_NAME_DESCRIPT + "\r\n" + C_VALUE_TO_COMPARE_DESCRIPT);
            fi.setId(csRptFormulaType.CSRPTF_IS_GREATER_THAN);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_IS_GREATER_THAN);

            fi = add(null, csRptFormulaType.CSRPTF_IS_LESS_THAN);
            fi.setName("_iseLowerthan");
            fi.setNameUser("It is lower than");
            fi.setDecrip(C_COMPARE_DESCRIPT + "\r\n\r\nSyntax: _isLowerThan(control_name, value)\r\n" + C_CONTROL_NAME_DESCRIPT + "\r\n" + C_VALUE_TO_COMPARE_DESCRIPT);
            fi.setId(csRptFormulaType.CSRPTF_IS_LESS_THAN);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_IS_LESS_THAN);

            fi = add(null, csRptFormulaType.CSRPTF_IS_IN_RS);
            fi.setName("_isInRS");
            fi.setNameUser("It is contained in the main recordset");
            fi.setDecrip("It returns a boolean value after searching a constant value in a column of the main recordset.\r\n\r\nSyntax: _isInRS(column_name,\"value\")\ncolumn_name: the name of a column in the main recordset\nvalue: an string to be searched (it must be surrounded by double quotes).");
            fi.setId(csRptFormulaType.CSRPTF_IS_IN_RS);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_IS_IN_RS);

            //----------------
            // G

            fi = add(null, csRptFormulaType.CSRPTF_GROUP_TOTAL);
            fi.setName("_groupTotal");
            fi.setNameUser("Group) Group total");
            fi.setDecrip(C_GROUP_FUNCTION_DESCRIPT.Replace("$1", "summatory")
                            + "\r\n\r\nSyntax: _groupTotal(column_name, group_index)"
                            + "\r\n\r\n" + C_COLUMN_NAME
                            + "\r\n" + C_GROUP_INDEX.Replace("$1", "summatory"));
            fi.setId(csRptFormulaType.CSRPTF_GROUP_TOTAL);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_GROUP_TOTAL);

            fi = add(null, csRptFormulaType.CSRPTF_GROUP_MAX);
            fi.setName("_groupMax");
            fi.setNameUser("Group) Group maximum");
            fi.setDecrip(C_GROUP_FUNCTION_DESCRIPT.Replace("$1", "maximum value")
                            + "\r\n\r\nSyntax: _groupTotal(column_name, group_index)"
                            + "\r\n\r\n" + C_COLUMN_NAME
                            + "\r\n" + C_GROUP_INDEX.Replace("$1", "maximum value"));
            fi.setId(csRptFormulaType.CSRPTF_GROUP_MAX);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_GROUP_MAX);

            fi = add(null, csRptFormulaType.CSRPTF_GROUP_MIN);
            fi.setName("_groupMin");
            fi.setNameUser("Group) Group minimum");
            fi.setDecrip(C_GROUP_FUNCTION_DESCRIPT.Replace("$1", "minimum value")
                            + "\r\n\r\nSyntax: _groupTotal(column_name, group_index)"
                            + "\r\n\r\n" + C_COLUMN_NAME
                            + "\r\n" + C_GROUP_INDEX.Replace("$1", "minimum value"));
            fi.setId(csRptFormulaType.CSRPTF_GROUP_MIN);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_GROUP_MIN);

            fi = add(null, csRptFormulaType.CSRPTF_GROUP_AVERAGE);
            fi.setName("_groupAverage");
            fi.setNameUser("Group) Group average");
            fi.setDecrip(C_GROUP_FUNCTION_DESCRIPT.Replace("$1", "average value")
                            + "\r\n\r\nSyntax: _groupAverage(column_name, group_index)"
                            + "\r\n\r\n" + C_COLUMN_NAME
                            + "\r\n" + C_GROUP_INDEX.Replace("$1", "average value"));
            fi.setId(csRptFormulaType.CSRPTF_GROUP_AVERAGE);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_GROUP_AVERAGE);

            fi = add(null, csRptFormulaType.CSRPTF_GROUP_PERCENT);
            fi.setName("_groupPercent");
            fi.setNameUser("Group) Group percent");
            fi.setDecrip(C_GROUP_FUNCTION_DESCRIPT2.Replace("$1", "percent value column_name2 represents in the summatory of column_name1")
                            + "\r\n\r\nSyntax: _groupTotal(column_name1, column_name2, group_index)"
                            + "\r\n\r\n" + C_COLUMN_NAME
                            + "\r\n" + C_COLUMN_NAME
                            + "\r\nNote: usually column_name1 and column_name2 have the same value because it is used to get the perecentage a value in a set represents."
                            + "\r\n" + C_GROUP_INDEX2.Replace("$1", "percent value"));
            fi.setId(csRptFormulaType.CSRPTF_GROUP_PERCENT);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_GROUP_PERCENT);

            fi = add(null, csRptFormulaType.CSRPTF_GROUP_COUNT);
            fi.setName("_groupCount");
            fi.setNameUser("Group) Amount of lines in a group");
            fi.setDecrip(C_GROUP_FUNCTION_DESCRIPT2.Replace("$1", "amunt of lines in the group")
                            + "\r\n\r\nSyntax: _groupCount(column_name, group_index)"
                            + "\r\n\r\n" + C_COLUMN_NAME
                            + "\r\n" + C_GROUP_INDEX2.Replace("$1", "amunt of lines"));
            fi.setId(csRptFormulaType.CSRPTF_GROUP_COUNT);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_GROUP_COUNT);

            fi = add(null, csRptFormulaType.CSRPTF_GROUP_LINE_NUMBER);
            fi.setName("_groupLineNumber");
            fi.setNameUser("Group) Line number in a group");
            fi.setDecrip("It returns the line number in a Group, if when Group is zero it returns the line number in the report."
                            + "\r\n\r\nSyntax: _GroupLineNumber(group_index)"
                            + "\r\n\r\ngroup_index: Group's index"
                            + "\r\nWhen group_index is -1 the group's index where the control is contained will be used."
                            + "\r\nWhen group_index is 0 the line number in the report will be returned."
                            + "\r\nWhen group_index is > 0 the line number in the group will be returned.");

            fi.setDecrip(C_GROUP_FUNCTION_DESCRIPT2.Replace("$1", "line number of the current line in the group.")
                            + "\r\n\r\nSyntax: _groupLineNumber(group_index)"
                            + "\r\n" + C_GROUP_INDEX2.Replace("$1", "line number of the current line in the group"));

            fi.setId(csRptFormulaType.CSRPTF_GROUP_LINE_NUMBER);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_GROUP_LINE_NUMBER);

            //----------------
            // M

            fi = add(null, csRptFormulaType.CSRPTF_MAX);
            fi.setName("_max");
            fi.setNameUser("Maximum value in a column");
            fi.setDecrip("It returns a double with the maximun value in a column.\r\n\r\nSyntax: _max(control_name)\r\n" + C_CONTROL_NAME_DESCRIPT);
            fi.setId(csRptFormulaType.CSRPTF_MAX);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_MAX);

            fi = add(null, csRptFormulaType.CSRPTF_MIN);
            fi.setName("_min");
            fi.setNameUser("Minimum value in a column");
            fi.setDecrip("It returns a double with the minimu valie in a column.\r\n\r\nSyntax: _min(control_name)\r\n" + C_CONTROL_NAME_DESCRIPT);
            fi.setId(csRptFormulaType.CSRPTF_MIN);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_MIN);

            //----------------
            // N

            fi = add(null, csRptFormulaType.CSRPTF_NUMBER_TO_STRING);
            fi.setName("_numberToString");
            fi.setNameUser("Number to String");
            fi.setDecrip("It returns the number expressed in words.\r\n\r\nSyntax: _numberToString(control_name,nLanguage)\r\n" + C_CONTROL_NAME_DESCRIPT + "\n" + C_LANGUAGE_DESCRIPT);
            fi.setId(csRptFormulaType.CSRPTF_NUMBER_TO_STRING);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_NUMBER_TO_STRING);

            fi = add(null, csRptFormulaType.CSRPTF_PAGE_NUMBER);
            fi.setName("_currentPage");
            fi.setNameUser("Page number");
            fi.setDecrip("It returns an int with the number of the current page.\r\n\r\nSyntax: _currentPage()");
            fi.setId(csRptFormulaType.CSRPTF_PAGE_NUMBER);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_PAGE_NUMBER);

            //----------------
            // O

            fi = add(null, csRptFormulaType.CSRPTF_GET_PARAM);
            fi.setName("_getParam");
            fi.setNameUser("Get a parameter value");
            fi.setDecrip("It returns a the value of a parameter from the main connection\r\n\r\nSyntax: _getParam(parameter_name)");
            fi.setId(csRptFormulaType.CSRPTF_GET_PARAM);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_GET_PARAM);

            fi = add(null, csRptFormulaType.CSRPTF_GET_DATA_FROM_RS_AD);
            fi.setName("_getDataFromRSAd");
            fi.setNameUser("Get a value form a column of a row in an additional recordset");
            fi.setDecrip("It returns a value from a column of a row in an additional recordset. "
                            + "\r\n\r\nThe rows of the additional recordset are filtered comparing the value "
                            + "of the column refered by the parameter filter of "
                            + "the current row in the main recordset with the values of the column "
                            + "refered by filter_column_name_add_ds in the additional recordset."
                            + "\r\n\r\nSyntax: (ds means Data Source): "
                            + "_getDataFromRSAd(ds_name, ds_index, column_name, filter)"
                            + "\r\n\r\nds_name: name of the additioanl connection"
                            + "\r\nds_index: index of the recordset in the additioanl connection"
                            + "\r\ncolumn_name: name of the column in the additional recordset which contains the value to return"
                            + "\r\nfilter: an strng containing the relation between one or more columns of the main recordset and the additional recordset"
                            + "\r\n\texample of filter:"
                            + "\r\n\t\tpr_id=pr_id (tipical primary key to foreign key relation)"
                            + "\r\n\t\tpr_id=pr_id|fv_id=fv_id (a two column relation is separated by pipes)"
                            + "\r\n\t\tas_id=as_id_factura (the names of the columns can be differents)"
                            );
            fi.setId(csRptFormulaType.CSRPTF_GET_DATA_FROM_RS_AD);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_GET_DATA_FROM_RS_AD);

            fi = add(null, csRptFormulaType.CSRPTF_GET_DATA_FROM_RS);
            fi.setName("_getDataFromRS");
            fi.setNameUser("Get a value from a column of a row in the main recordset");
            fi.setDecrip("It returns a value from a column of a row in the main recordset. "
                            + "The rows are filtered comparing the value "
                            + "of the column refered by the parameter filter_column_name1 of "
                            + "the current row with the values of the column "
                            + "refered by filter_column_name2."
                            + "\r\n\r\nSyntax: getDataFromRS (column_name, filter_column_name1, filter_column_name2)"
                            + "\r\n\r\ncolumn_name: name of the column which contains the value to return"
                            + "\r\nfilter_column_name1: name of the column in the current record"
                            + "\r\nfilter_column_name2: name of the column in used to filter values");
            fi.setId(csRptFormulaType.CSRPTF_GET_DATA_FROM_RS);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_GET_DATA_FROM_RS);

            fi = add(null, csRptFormulaType.CSRPTF_GET_STRING);
            fi.setName("_getString");
            fi.setNameUser("Get an string");
            fi.setDecrip("It returns the value of the control refered by the control_name parameter surrounded by double quotes"
                            + "\r\n\r\nSyntax: _getString(control_name)\r\n" + C_CONTROL_NAME_DESCRIPT);
            fi.setId(csRptFormulaType.CSRPTF_GET_STRING);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_GET_STRING);

            fi = add(null, csRptFormulaType.CSRPTF_GET_VAR);
            fi.setName("_getVar");
            fi.setNameUser("Get the value of a user variable");
            fi.setDecrip("It returns the value of the variable refered by the variable_name parameter"
                            + "\r\n\r\nSyntax: _getVar(variable_name)");
            fi.setId(csRptFormulaType.CSRPTF_GET_VAR);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_GET_VAR);

            //----------------
            // P

            fi = add(null, csRptFormulaType.CSRPTF_AVERAGE);
            fi.setName("_average");
            fi.setNameUser("Average of a Column");
            fi.setDecrip("It returns a double with the average value of a column"
                            + "\r\n\r\nSyntax: _average(control_name)\r\n" + C_CONTROL_NAME_DESCRIPT);
            fi.setId(csRptFormulaType.CSRPTF_AVERAGE);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_AVERAGE);

            //----------------
            // S

            fi = add(null, csRptFormulaType.CSRPTF_ADD_TO_VAR);
            fi.setName("_addToVar");
            fi.setNameUser("Add a value to a user variable");
            fi.setDecrip("It adds the value of the parameter value to a user variable refered by the parameter variable_name"
                            + "\r\n\r\nSyntax: _addToVar(variable_name, value)");
            fi.setId(csRptFormulaType.CSRPTF_ADD_TO_VAR);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_ADD_TO_VAR);

            fi = add(null, csRptFormulaType.CSRPTF_SUM);
            fi.setName("_sum");
            fi.setNameUser("Totals of a column");
            fi.setDecrip("It returns the total of a column\r\n\r\nSyntax: _sum(control_name)\r\n" + C_CONTROL_NAME_DESCRIPT);
            fi.setId(csRptFormulaType.CSRPTF_SUM);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_SUM);

            fi = add(null, csRptFormulaType.CSRPTF_SUM_TIME);
            fi.setName("_sumTime");
            fi.setNameUser("Totals in time units of a column");
            fi.setDecrip("It returns the amount of hours, minutes and seconds from a column which contains hours and minutes in the format hh:nn"
                            + "\r\n\r\nSyntax: _sumTime(control_name, show_seconds)\r\n" + C_CONTROL_NAME_DESCRIPT);
            fi.setId(csRptFormulaType.CSRPTF_SUM_TIME);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_SUM_TIME);

            //----------------
            // T

            fi = add(null, csRptFormulaType.CSRPTF_LENGTH);
            fi.setName("_length");
            fi.setNameUser("Length of a control's value");
            fi.setDecrip("It returns an int with the length of a control's value\r\n\r\nSyntax: _length(control_name)\r\n\r\n" + C_CONTROL_NAME_DESCRIPT);
            fi.setId(csRptFormulaType.CSRPTF_LENGTH);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_LENGTH);

            fi = add(null, csRptFormulaType.CSRPTF_TEXT_REPLACE);
            fi.setName("_textReplace");
            fi.setNameUser("Replace a control name by its value in a string");
            fi.setDecrip("It replace every occurrence of a control name in the text property of another control. "
                        + "\r\n\r\nThis is the only function which is used in the text property of a control. " 
                        + "the syntax is very weird because you don't call this function using its name "
                        + "but you put in the text property of a control the name of other control "
                        + "surrounded by two ats (@@control_name@@)\r\n\r\nSyntax: @@control_name@@\r\n\r\n" + C_CONTROL_NAME_DESCRIPT);
            fi.setId(csRptFormulaType.CSRPTF_TEXT_REPLACE);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_TEXT_REPLACE);

            //----------------
            // V

            fi = add(null, csRptFormulaType.CSRPTF_VAL);
            fi.setName("_value");
            fi.setNameUser("Value of a control");
            fi.setDecrip("It returns an string with the value of the control refered by the control_name parameter"
                            + "\r\n\r\nSyntax: _value(control_name)\r\n\r\n" + C_CONTROL_NAME_DESCRIPT);
            fi.setId(csRptFormulaType.CSRPTF_VAL);
            fi.setHelpContextId(csRptFormulaType.CSRPTF_VAL);
        }

    }

}
