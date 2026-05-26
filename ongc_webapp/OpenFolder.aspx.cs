using System;
using System.Diagnostics;
using System.IO;

namespace ongc_webapp
{
    public partial class OpenFolder : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string path =
                Request.QueryString["path"];

            if (!string.IsNullOrWhiteSpace(path))
            {
                if (File.Exists(path))
                {
                    Process.Start(
                        "explorer.exe",
                        "/select,\"" + path + "\"");
                }
            }

            Response.Write(
                "<script>window.close();</script>");
        }
    }
}