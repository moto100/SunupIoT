// <copyright file="ScriptTable.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>
namespace Sunup.ScriptLibrary
{
    using System.Collections.Generic;
    using Sunup.Contract;

    /// <summary>
    /// ScriptTable.
    /// </summary>
    public class ScriptTable : BaseTable<Script>
    {
        /// <summary>
        /// Gets items.
        /// </summary>
        public List<Script> ScriptItems
        {
            get
            {
                var scripts = new List<Script>();
                scripts.AddRange(this.Items);
                return scripts;
            }
        }

        /// <inheritdoc/>
        public override Script GetItem(int index)
        {
            if (index < 0 || index >= this.Count)
            {
                return null;
            }

            return base.GetItem(index);
        }

        /// <summary>
        /// Add script.
        /// </summary>
        /// <param name="item">Instance of script.</param>
        /// <returns>Index of script.</returns>
        public override int AddItem(Script item)
        {
            if (item.ScriptType == ScriptType.OnShow || item.ScriptType == ScriptType.OnHide || item.ScriptType == ScriptType.WhileShowing)
            {
                var script = this.ScriptItems.Find(x => x.ScriptType == item.ScriptType);
                if (script != null)
                {
                    return -1;
                }
            }

            return base.AddItem(item);
        }
    }
}
