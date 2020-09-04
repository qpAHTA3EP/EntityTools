using Astral.Quester.Classes;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EntityCore.Extensions
{
    public static class FormCollectionExtentions
    {
        public static Form Find<T>(this FormCollection forms) where T : Form
        {
            Type searchedFormType = typeof(T);
            foreach (Form form in forms)
            {
                if (searchedFormType.Equals(form))
                    return form;
            }
            return null;
        }
    }
}
