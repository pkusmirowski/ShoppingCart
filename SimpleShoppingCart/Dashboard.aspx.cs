namespace Shop
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    public partial class Dashboard : System.Web.UI.Page
    {
        protected void PageLoad(object sender, EventArgs e)
        {
            if (this.Session["email"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            this.userInfo.Text = $"Login klienta: {Session["email"]}";
        }

        protected void BtnLogoutClick(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }

        protected void BtnBackClick(object sender, EventArgs e)
        {
            Response.Redirect("Login.aspx");
        }
    }
}