<%@ Application Language="C#" %>

<script runat="server">
    void Application_BeginRequest(object sender, EventArgs e)
    {
        string currentPath = Request.AppRelativeCurrentExecutionFilePath.ToLower();

        if (currentPath == "~/" || currentPath == "~/default.aspx" || currentPath == "~/default")
        {
            Response.Redirect("~/Login.aspx", true);
        }
        else if (currentPath == "~/login")
        {
            Response.Redirect("~/Login.aspx", true);
        }
        else if (currentPath == "~/dashboard")
        {
            Response.Redirect("~/Dashboard.aspx", true);
        }
        else if (currentPath == "~/indexing")
        {
            Response.Redirect("~/Indexing.aspx", true);
        }
    }
</script>