using Archpack.Training.ArchUnits.Contracts.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Arcs.PdfDocuments.V1
{
    /// <summary>
    /// DataGroup
    /// </summary>
    public class DataGroup
    {
        public string GroupName { get; private set; }

        public List<DataItem> Items { get; private set; }
        /// <summary>
        /// DataGroupのコンストラクタ
        /// </summary>
        public DataGroup()
        {
            this.Items = new List<DataItem>();
        }

        public void AddItem(string itemName, object value)
        {
            var item = new DataItem(itemName, value);
            this.Items.Add(item);
        }

        /// <summary>
        /// DataGroupのコンストラクタ
        /// </summary>
        /// <param name="groupName"></param>
        public DataGroup(string groupName){                       
            Contract.NotEmpty(groupName, "groupName");
            this.GroupName = groupName;
            this.Items = new List<DataItem>();
        }

        
    }
}