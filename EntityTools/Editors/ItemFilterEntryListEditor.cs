using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using EntityTools.Forms;
using EntityTools.Tools.BuySellItems;

namespace EntityTools.Editors
{
	// Token: 0x02000092 RID: 146
	public class ItemFilterEntryListEditor : UITypeEditor
	{
		// Token: 0x060005C8 RID: 1480 RVA: 0x000274FC File Offset: 0x000256FC
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			List<ItemFilterEntryExt> collection;
			bool flag = (collection = (value as List<ItemFilterEntryExt>)) != null;
			if (flag)
			{
				bool flag2 = ItemFilterEditorForm.GUIRequiest(ref collection);
				if (flag2)
				{
					return new List<ItemFilterEntryExt>(collection);
				}
			}
			return value;
		}

		// Token: 0x060005C9 RID: 1481 RVA: 0x00027534 File Offset: 0x00025734
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
	}
}
