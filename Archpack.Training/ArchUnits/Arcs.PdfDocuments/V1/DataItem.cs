using Archpack.Training.ArchUnits.Contracts.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Arcs.PdfDocuments.V1
{
    /// <summary>
    /// DataItem
    /// </summary>
    public class DataItem
    {
        public string ItemName { get; private set; }

        public object Value { get; private set; }
        /// <summary>
        /// DataItemのコンストラクタ
        /// </summary>
        public DataItem()
        {

        }

        /// <summary>
        /// DataItemのコンストラクタ
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="value"></param>
        public DataItem(string itemName, object value)
        {            
            Contract.NotEmpty(itemName, "itemName");

            Contract.NotNull(value, "value");

            if (!value.GetType().IsValueType && !(value is string))
            {
                throw new ArgumentException(string.Format(Properties.Resources.SpecifyValueType, "value", "string/double/datetime/int/long/decimal"));
            }

            this.ItemName = itemName;
            this.Value = value;
        }
    }
}