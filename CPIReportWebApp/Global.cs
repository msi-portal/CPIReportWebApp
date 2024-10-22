using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;

namespace CPIReportWebApp
{
    public class Global
    {

        public Global()
        {

        }

        public DataTable ConvertToDataTable<T>(IEnumerable<T> data)
        {
            List<IDataRecord> list = data.Cast<IDataRecord>().ToList();

            PropertyDescriptorCollection props = null;
            DataTable table = new DataTable();
            if (list != null && list.Count > 0)
            {
                props = TypeDescriptor.GetProperties(list[0]);
                for (int i = 0; i < props.Count; i++)
                {
                    PropertyDescriptor prop = props[i];
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }
            }
            if (props != null)
            {
                object[] values = new object[props.Count];
                foreach (T item in data)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = props[i].GetValue(item) ?? DBNull.Value;
                    }
                    table.Rows.Add(values);
                }
            }
            return table;
        }

    }

    public class Configs
    {
        public string Message { get; set; }
        public string Success { get; set; }
        public List<string> Configurations { get; set; }
    }

    public class Tokens
    {
        public string Message { get; set; }
        public string Success { get; set; }
        public string Token { get; set; }
    }

    public class UpdateCollectionRequest
    {
        public IDOUpdateItem[] Changes { get; set; }
        public bool RefreshAfterSave { get; set; }
        public string CustomInsert { get; set; }
        public string CustomUpdate { get; set; }
        public string CustomDelete { get; set; }
    }
    public class IDOUpdateItem
    {
        public UpdateAction Action { get; set; }
        public string ItemId { get; set; }
        public UpdateProperty[] Properties { get; set; }
        public UpdateLocking UpdateLocking { get; set; }
    }
    public class UpdateProperty
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string OriginalValue { get; set; }
        public bool Modified { get; set; }
        public bool IsNull { get; set; }
    }

    public enum UpdateAction
    {
        Insert = 1,
        Update = 2,
        Delete = 4
    }
    public enum UpdateLocking
    {
        Row = 0,
        Property = 1
    }

    public class SimpleIDOItemList
    {
        public string IDOName { get; set; }
        public string[] PropertyList { get; set; }
        public List<SimpleIDOItem> Items;
    }
    public class SimpleIDOItem
    {
        public enum Modified
        {
            Unmodified,
            Modified,
            Deleted,
            Inserted
        }
        public string ID { get; set; }
        public List<PropertyStatusPair> Properties;
        public Modified EditStatus { get; set; }
    }
    public struct PropertyStatusPair
    {
        public string Property { get; set; }
        public bool Updated { get; set; }
    }

}