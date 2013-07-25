using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace $safeprojectname$
{
    class DebugHelper
    {
        public static async void showMessage(String msg) 
        {
            try
            {
                MessageDialog dialog = new MessageDialog(msg, "DEBUG");
                await dialog.ShowAsync();
            }
            catch (Exception ex) {
            };
        }
    }
}
