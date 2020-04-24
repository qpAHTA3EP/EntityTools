using System;
using System.Windows.Forms;

namespace EntityTools.Extensions
{
    public static class FormCollectionExtentions
    {
        public static T Find<T>(this FormCollection forms) where T : Form
        {
            Type searchedFormType = typeof(T);
            foreach (Form form in forms)
            {
                if (searchedFormType.Equals(form.GetType()))
                    return form as T;
            }
            return null;
        }
    }
}
