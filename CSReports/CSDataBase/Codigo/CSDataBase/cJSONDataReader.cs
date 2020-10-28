using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Reflection;
using System.Data;
using Newtonsoft.Json.Linq;

namespace CSDataBase
{
    class cJSONDataReader : DbDataReader, IDataReader, IDisposable, IDataRecord
    {
        private cJSONDataSource m_dataSource;
        private JArray m_cols;
        private JArray m_rows;

        private int m_resultIndex = 0;
        private int m_rowIndex = -1;
        private bool m_closed = false;
        private DateTime m_start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public cJSONDataReader(cJSONDataSource dataSource)
        {
            m_dataSource = dataSource;
            m_cols = m_dataSource.getData()["columns"] as JArray;
            m_rows = m_dataSource.getData()["rows"] as JArray;
        }

        //
        // Summary:
        //     Gets the value of the specified column in its native format given the column
        //     name.
        //
        // Parameters:
        //   name:
        //     The column name.
        //
        // Returns:
        //     The value of the specified column in its native format.
        //
        // Exceptions:
        //   T:System.IndexOutOfRangeException:
        //     No column with the specified name was found.
        public override object this[string name]
        {
            get
            {
                throw new NotImplementedException();
            }
        } 
        
        //
        // Summary:
        //     Gets the value of the specified column in its native format given the column
        //     ordinal.
        //
        // Parameters:
        //   i:
        //     The zero-based column ordinal.
        //
        // Returns:
        //     The value of the specified column in its native format.
        //
        // Exceptions:
        //   T:System.IndexOutOfRangeException:
        //     The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.
        public override object this[int i]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        //
        // Summary:
        //     Gets a value that indicates the depth of nesting for the current row.
        //
        // Returns:
        //     The depth of nesting for the current row.
        public override int Depth { get; }
        //
        // Summary:
        //     Gets the number of columns in the current row.
        //
        // Returns:
        //     When not positioned in a valid recordset, 0; otherwise the number of columns
        //     in the current row. The default is -1.
        //
        // Exceptions:
        //   T:System.NotSupportedException:
        //     There is no current connection to an instance of SQL Server.
        public override int FieldCount
        {
            get
            {
                return (m_dataSource.getData()["columns"] as JArray).Count;
            }
        }
        //
        // Summary:
        //     Gets a value that indicates whether the System.Data.SqlClient.SqlDataReader contains
        //     one or more rows.
        //
        // Returns:
        //     true if the System.Data.SqlClient.SqlDataReader contains one or more rows; otherwise
        //     false.
        public override bool HasRows {
            get
            {
                return (m_dataSource.getData()["rows"] as JArray).Count > 0;
            }
        }
        //
        // Summary:
        //     Retrieves a Boolean value that indicates whether the specified System.Data.SqlClient.SqlDataReader
        //     instance has been closed.
        //
        // Returns:
        //     true if the specified System.Data.SqlClient.SqlDataReader instance is closed;
        //     otherwise false.
        public override bool IsClosed
        {
            get
            {
                return m_closed;
            }
        }
        //
        // Summary:
        //     Gets the number of rows changed, inserted, or deleted by execution of the Transact-SQL
        //     statement.
        //
        // Returns:
        //     The number of rows changed, inserted, or deleted; 0 if no rows were affected
        //     or the statement failed; and -1 for SELECT statements.
        public override int RecordsAffected { get; }
        //
        // Summary:
        //     Gets the number of fields in the System.Data.SqlClient.SqlDataReader that are
        //     not hidden.
        //
        // Returns:
        //     The number of fields that are not hidden.
        public override int VisibleFieldCount {
            get
            {
                return (m_dataSource.getData()["columns"] as JArray).Count;
            }
        }

        //
        // Summary:
        //     Closes the System.Data.SqlClient.SqlDataReader object.
        public override void Close()
        {
            // TODO: we should release resources here (unregister the datasource in cJSONServer)
            m_closed = true;
        }
        //
        // Summary:
        //     Gets the value of the specified column as a Boolean.
        //
        // Parameters:
        //   i:
        //     The zero-based column ordinal.
        //
        // Returns:
        //     The value of the column.
        //
        // Exceptions:
        //   T:System.InvalidCastException:
        //     The specified cast is not valid.
        public override bool GetBoolean(int i) { throw new NotImplementedException(); }
        //
        // Summary:
        //     Gets the value of the specified column as a byte.
        //
        // Parameters:
        //   i:
        //     The zero-based column ordinal.
        //
        // Returns:
        //     The value of the specified column as a byte.
        //
        // Exceptions:
        //   T:System.InvalidCastException:
        //     The specified cast is not valid.
        public override byte GetByte(int i) { throw new NotImplementedException(); }
        //
        // Summary:
        //     Reads a stream of bytes from the specified column offset into the buffer an array
        //     starting at the given buffer offset.
        //
        // Parameters:
        //   i:
        //     The zero-based column ordinal.
        //
        //   dataIndex:
        //     The index within the field from which to begin the read operation.
        //
        //   buffer:
        //     The buffer into which to read the stream of bytes.
        //
        //   bufferIndex:
        //     The index within the buffer where the write operation is to start.
        //
        //   length:
        //     The maximum length to copy into the buffer.
        //
        // Returns:
        //     The actual number of bytes read.
        public override long GetBytes(int i, long dataIndex, byte[] buffer, int bufferIndex, int length)
        { throw new NotImplementedException(); }
        //
        // Summary:
        //     Gets the value of the specified column as a single character.
        //
        // Parameters:
        //   i:
        //     The zero-based column ordinal.
        //
        // Returns:
        //     The value of the specified column.
        //
        // Exceptions:
        //   T:System.InvalidCastException:
        //     The specified cast is not valid.
        public override char GetChar(int i) { throw new NotImplementedException(); }
        //
        // Summary:
        //     Reads a stream of characters from the specified column offset into the buffer
        //     as an array starting at the given buffer offset.
        //
        // Parameters:
        //   i:
        //     The zero-based column ordinal.
        //
        //   dataIndex:
        //     The index within the field from which to begin the read operation.
        //
        //   buffer:
        //     The buffer into which to read the stream of bytes.
        //
        //   bufferIndex:
        //     The index within the buffer where the write operation is to start.
        //
        //   length:
        //     The maximum length to copy into the buffer.
        //
        // Returns:
        //     The actual number of characters read.
        public override long GetChars(int i, long dataIndex, char[] buffer, int bufferIndex, int length)
        { throw new NotImplementedException(); }
        //
        // Summary:
        //     Gets a string representing the data type of the specified column.
        //
        // Parameters:
        //   i:
        //     The zero-based ordinal position of the column to find.
        //
        // Returns:
        //     The string representing the data type of the specified column.
        public override string GetDataTypeName(int i) { throw new NotImplementedException(); }
        //
        // Summary:
        //     Gets the value of the specified column as a System.DateTime object.
        //
        // Parameters:
        //   i:
        //     The zero-based column ordinal.
        //
        // Returns:
        //     The value of the specified column.
        //
        // Exceptions:
        //   T:System.InvalidCastException:
        //     The specified cast is not valid.
        public override DateTime GetDateTime(int i) { throw new NotImplementedException(); }
        //
        // Summary:
        //     Retrieves the value of the specified column as a System.DateTimeOffset object.
        //
        // Parameters:
        //   i:
        //     The zero-based column ordinal.
        //
        // Returns:
        //     The value of the specified column.
        //
        // Exceptions:
        //   T:System.InvalidCastException:
        //     The specified cast is not valid.
        public virtual DateTimeOffset GetDateTimeOffset(int i) { throw new NotImplementedException(); }
        //
        // Summary:
        //     Gets the value of the specified column as a System.Decimal object.
        //
        // Parameters:
        //   i:
        //     The zero-based column ordinal.
        //
        // Returns:
        //     The value of the specified column.
        //
        // Exceptions:
        //   T:System.InvalidCastException:
        //     The specified cast is not valid.
        public override decimal GetDecimal(int i) { throw new NotImplementedException(); }
        //
        // Summary:
        //     Gets the value of the specified column as a double-precision floating point number.
        //
        // Parameters:
        //   i:
        //     The zero-based column ordinal.
        //
        // Returns:
        //     The value of the specified column.
        //
        // Exceptions:
        //   T:System.InvalidCastException:
        //     The specified cast is not valid.
        public override double GetDouble(int i) { throw new NotImplementedException(); }
        //
        // Summary:
        //     Returns an System.Collections.IEnumerator that iterates through the System.Data.SqlClient.SqlDataReader.
        //
        // Returns:
        //     An System.Collections.IEnumerator for the System.Data.SqlClient.SqlDataReader.
        public override IEnumerator GetEnumerator() { throw new NotImplementedException(); }
        //
        // Summary:
        //     Gets the System.Type that is the data type of the object.
        //
        // Parameters:
        //   i:
        //     The zero-based column ordinal.
        //
        // Returns:
        //     The System.Type that is the data type of the object. If the type does not exist
        //     on the client, in the case of a User-Defined Type (UDT) returned from the database,
        //     GetFieldType returns null.
        public override Type GetFieldType(int i) { throw new NotImplementedException(); }
        //
        // Summary:
        //     Gets the value of the specified column as a single-precision floating point number.
        //
        // Parameters:
        //   i:
        //     The zero-based column ordinal.
        //
        // Returns:
        //     The value of the specified column.
        //
        // Exceptions:
        //   T:System.InvalidCastException:
        //     The specified cast is not valid.
        public override float GetFloat(int i) { throw new NotImplementedException(); }
        //
        // Summary:
        //     Gets the value of the specified column as a globally unique identifier (GUID).
        //
        // Parameters:
        //   i:
        //     The zero-based column ordinal.
        //
        // Returns:
        //     The value of the specified column.
        //
        // Exceptions:
        //   T:System.InvalidCastException:
        //     The specified cast is not valid.
        public override Guid GetGuid(int i) { throw new NotImplementedException(); }
        //
        // Summary:
        //     Gets the value of the specified column as a 16-bit signed integer.
        //
        // Parameters:
        //   i:
        //     The zero-based column ordinal.
        //
        // Returns:
        //     The value of the specified column.
        //
        // Exceptions:
        //   T:System.InvalidCastException:
        //     The specified cast is not valid.
        public override short GetInt16(int i) { throw new NotImplementedException(); }
        //
        // Summary:
        //     Gets the value of the specified column as a 32-bit signed integer.
        //
        // Parameters:
        //   i:
        //     The zero-based column ordinal.
        //
        // Returns:
        //     The value of the specified column.
        //
        // Exceptions:
        //   T:System.InvalidCastException:
        //     The specified cast is not valid.
        public override int GetInt32(int i) { throw new NotImplementedException(); }
        //
        // Summary:
        //     Gets the value of the specified column as a 64-bit signed integer.
        //
        // Parameters:
        //   i:
        //     The zero-based column ordinal.
        //
        // Returns:
        //     The value of the specified column.
        //
        // Exceptions:
        //   T:System.InvalidCastException:
        //     The specified cast is not valid.
        public override long GetInt64(int i) { throw new NotImplementedException(); }
        //
        // Summary:
        //     Gets the name of the specified column.
        //
        // Parameters:
        //   i:
        //     The zero-based column ordinal.
        //
        // Returns:
        //     The name of the specified column.
        public override string GetName(int i) {
            return m_cols[i]["name"].ToString();
        }
        //
        // Summary:
        //     Gets the column ordinal, given the name of the column.
        //
        // Parameters:
        //   name:
        //     The name of the column.
        //
        // Returns:
        //     The zero-based column ordinal.
        //
        // Exceptions:
        //   T:System.IndexOutOfRangeException:
        //     The name specified is not a valid column name.
        public override int GetOrdinal(string name) { throw new NotImplementedException(); }
        //
        // Summary:
        //     Gets an Object that is a representation of the underlying provider-specific field
        //     type.
        //
        // Parameters:
        //   i:
        //     An System.Int32 representing the column ordinal.
        //
        // Returns:
        //     Gets an System.Object that is a representation of the underlying provider-specific
        //     field type.
        public override Type GetProviderSpecificFieldType(int i) { throw new NotImplementedException(); }
        //
        // Summary:
        //     Gets an Object that is a representation of the underlying provider specific value.
        //
        // Parameters:
        //   i:
        //     An System.Int32 representing the column ordinal.
        //
        // Returns:
        //     An System.Object that is a representation of the underlying provider specific
        //     value.
        public override object GetProviderSpecificValue(int i) { throw new NotImplementedException(); }
        //
        // Summary:
        //     Gets an array of objects that are a representation of the underlying provider
        //     specific values.
        //
        // Parameters:
        //   values:
        //     An array of System.Object into which to copy the column values.
        //
        // Returns:
        //     The array of objects that are a representation of the underlying provider specific
        //     values.
        public override int GetProviderSpecificValues(object[] values) { throw new NotImplementedException(); }
        //
        // Summary:
        //     Returns a System.Data.DataTable that describes the column metadata of the System.Data.SqlClient.SqlDataReader.
        //
        // Returns:
        //     A System.Data.DataTable that describes the column metadata.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The System.Data.SqlClient.SqlDataReader is closed.
        /*public override DataTable GetSchemaTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("ColumnName");
            dt.Columns.Add("ColumnOrdinal");
            dt.Columns.Add("ColumnType");

            var i = 0;
            foreach (var col in m_cols)
            {
                var row = dt.NewRow();
                row["ColumnName"] = col["name"].ToString();
                row["ColumnType"] = col["columnType"].ToString();
                row["ColumnOrdinal"] = i;
                i += 1;
            }

            return dt;
        }*/

        private Type getType(string typeName, string columnName)
        {
            Type type;

            switch (typeName.ToLower())
            {
                case "integer":
                case "int2":
                case "int4":
                case "smallint":
                    type = typeof(int);
                    break;
                case "biginteger":
                case "serial":
                case "bigserial":
                    type = typeof(Int64);
                    break;
                case "decimal":
                case "numeric":
                case "float8":
                case "float4":
                case "real":
                    type = typeof(double);
                    break;
                case "timestamp":
                case "timestamptz":
                case "date":
                case "time":
                    type = typeof(DateTime);
                    break;
                case "character":
                case "char":
                case "varchar":
                case "text":
                case "character varying":
                    type = typeof(string);
                    break;
                case "bytea":
                    type = typeof(byte[]);
                    break;
                default:
                    throw new ArgumentException("The data type for column " + columnName + " of data source " + m_dataSource.getName() + " is not supported");
            }
            return type;
        }

        public override DataTable GetSchemaTable()
        {
            var table = new DataTable("SchemaTable");

            table.Columns.Add("AllowDBNull", typeof(bool));
            table.Columns.Add("BaseCatalogName", typeof(string));
            table.Columns.Add("BaseColumnName", typeof(string));
            table.Columns.Add("BaseSchemaName", typeof(string));
            table.Columns.Add("BaseTableName", typeof(string));
            table.Columns.Add("ColumnName", typeof(string));
            table.Columns.Add("ColumnOrdinal", typeof(int));
            table.Columns.Add("ColumnSize", typeof(int));
            table.Columns.Add("DataType", typeof(Type));
            table.Columns.Add("IsUnique", typeof(bool));
            table.Columns.Add("IsKey", typeof(bool));
            table.Columns.Add("IsAliased", typeof(bool));
            table.Columns.Add("IsExpression", typeof(bool));
            table.Columns.Add("IsIdentity", typeof(bool));
            table.Columns.Add("IsAutoIncrement", typeof(bool));
            table.Columns.Add("IsRowVersion", typeof(bool));
            table.Columns.Add("IsHidden", typeof(bool));
            table.Columns.Add("IsLong", typeof(bool));
            table.Columns.Add("IsReadOnly", typeof(bool));
            table.Columns.Add("NumericPrecision", typeof(int));
            table.Columns.Add("NumericScale", typeof(int));
            table.Columns.Add("ProviderSpecificDataType", typeof(Type));
            table.Columns.Add("ProviderType", typeof(Type));

            var i = 0;
            foreach (var col in m_cols)
            {
                var colName = col["name"].ToString();

                var row = table.NewRow();

                row["AllowDBNull"] = false;
                row["BaseColumnName"] = colName;
                row["BaseCatalogName"] = "";
                row["BaseSchemaName"] = "";
                row["BaseTableName"] = "";
                row["ColumnName"] = colName;
                row["ColumnOrdinal"] = i;
                row["ColumnSize"] = -1;
                row["DataType"] = row["ProviderType"] = getType(col["columnType"].ToString(), colName); // Non-standard
                row["IsUnique"] = false;
                row["IsKey"] = false;
                row["IsAliased"] = false;
                row["IsExpression"] = false;
                row["IsAutoIncrement"] = false;
                row["IsIdentity"] = false;
                row["IsRowVersion"] = false;
                row["IsHidden"] = false;
                row["IsLong"] = false;
                row["NumericPrecision"] = 255;
                row["NumericScale"] = 255;

                table.Rows.Add(row);

                i += 1;
            }

            return table;
        }
        //
        // Summary:
        //     Gets the value of the specified column as a string.
        //
        // Parameters:
        //   i:
        //     The zero-based column ordinal.
        //
        // Returns:
        //     The value of the specified column.
        //
        // Exceptions:
        //   T:System.InvalidCastException:
        //     The specified cast is not valid.
        public override string GetString(int i) { throw new NotImplementedException(); }

        private bool isByteA(int i)
        {
            return m_cols[i]["columnType"].ToString() == "bytea";
        }
        private bool isTimestamptz(int i)
        {
            return m_cols[i]["columnType"].ToString() == "timestamptz";
        }
        //
        // Summary:
        //     Gets the value of the specified column in its native format.
        //
        // Parameters:
        //   i:
        //     The zero-based column ordinal.
        //
        // Returns:
        //     This method returns System.DBNull for null database columns.
        public override object GetValue(int i)
        {
            object value = m_rows[m_rowIndex]["values"][i];
            if (isByteA(i))
            {
                value = Convert.FromBase64String(value.ToString());
            }
            else if (isTimestamptz(i))
            {
                if (value.ToString().Trim().Length == 0)
                {
                    value = m_start;
                }
                else
                {
                    value = m_start.AddMilliseconds(Convert.ToInt64(value.ToString())).ToLocalTime();
                }                
            }
            return value;
        }
        //
        // Summary:
        //     Populates an array of objects with the column values of the current row.
        //
        // Parameters:
        //   values:
        //     An array of System.Object into which to copy the attribute columns.
        //
        // Returns:
        //     The number of instances of System.Object in the array.
        public override int GetValues(object[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            CheckRow();

            var count = Math.Min(FieldCount, values.Length);
            for (var i = 0; i < count; i++)
                values[i] = GetValue(i);
            return count;
        }
        //
        // Summary:
        //     Gets a value that indicates whether the column contains non-existent or missing
        //     values.
        //
        // Parameters:
        //   i:
        //     The zero-based column ordinal.
        //
        // Returns:
        //     true if the specified column value is equivalent to System.DBNull; otherwise
        //     false.
        public override bool IsDBNull(int i) { throw new NotImplementedException(); }
        //
        // Summary:
        //     Advances the data reader to the next result, when reading the results of batch
        //     Transact-SQL statements.
        //
        // Returns:
        //     true if there are more result sets; otherwise false.
        public override bool NextResult()
        {
            m_resultIndex += 1;
            return m_resultIndex < 2;
        }
        //
        // Summary:
        //     Advances the System.Data.SqlClient.SqlDataReader to the next record.
        //
        // Returns:
        //     true if there are more rows; otherwise false.
        public override bool Read() {
            m_rowIndex += 1;
            return m_rowIndex < m_rows.Count;
        }

        public bool IsOnRow => -1 < m_rowIndex && m_rowIndex < m_rows.Count;

        void CheckRow()
        {
            if (!IsOnRow)
                throw new InvalidOperationException("No row is available");
        }
    }
}
