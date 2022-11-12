using System;
using System.Windows.Forms;

namespace EntityTools.Tools.Extensions
{
    public static class FormCollectionExtensions
    {
        public static T Find<T>(this FormCollection forms) where T : Form
        {
            Type searchedFormType = typeof(T);
            foreach (Form form in forms)
            {
                if (searchedFormType == form.GetType())
                    return form as T;
            }
            return null;
        }
    }
}
