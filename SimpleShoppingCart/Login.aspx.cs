namespace Shop
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Configuration;
    using System.Data;
    using System.Data.Odbc;
    using System.Data.SqlClient;
    using System.Drawing;
    using System.EnterpriseServices;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    public partial class Login : System.Web.UI.Page
    {
        public readonly SqlCommand Cmd = new SqlCommand();
        public readonly SqlConnection SqlCon = new SqlConnection(@"Data Source=DESKTOP-G2VNIQS; Initial Catalog=Account; Integrated Security=True;");
        private SqlDataAdapter sqlDa = new SqlDataAdapter();
        private DataTable dt = new DataTable();

        /*
        //Funkcja haszująca
        static string ComputeSha256Hash(string value)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(value));
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        */

        protected void PageLoad(object sender, EventArgs e)
        {
            this.lblError.Visible = false;
            this.SqlCon.Open();
        }

        protected void BtnLoginClick(object sender, EventArgs e)
        {
            this.SqlCon.Open();
            string pass = this.txtPassword.Text; ////ComputeSha256Hash(txtPassword.Text);
            SqlDataReader reader;
            this.Cmd.Connection = this.SqlCon;
            this.Cmd.Parameters.Add("txtLogin", SqlDbType.NChar).Value = txtLogin.Text;
            this.Cmd.Parameters.Add("pass", SqlDbType.NChar).Value = pass;
            this.Cmd.CommandText = "SELECT * FROM [User] " + "WHERE Email=@txtLogin AND Password=@pass";

            /*
             *          someCommand.CommandText = "SELECT AccountNumber FROM Users " + "WHERE Username='" + name + "' AND Password='" + password + "'";
             *          
             *          someCommand.Parameters.Add("@username", SqlDbType.NChar).Value = name;
                        someCommand.Parameters.Add("@password", SqlDbType.NChar).Value = password;
                        someCommand.CommandText = "SELECT AccountNumber FROM Users " + "WHERE Username=@username AND Password=@password";
             */

            reader = this.Cmd.ExecuteReader();
            if (reader.Read())
            {
                this.Session["email"] = this.txtLogin.Text;

                this.SqlCon.Close();
                Response.Redirect("Dashboard.aspx");
            }
            else
            {
                this.lblError.Visible = true;
                this.SqlCon.Close();
            }
        }

        protected void BtnLoginBackClick(object sender, EventArgs e) => Response.Redirect("Register.aspx");

        protected void BtnSendEmailClick(object sender, EventArgs e)
        {
            string email = string.Empty;
            string password = string.Empty;
            this.SqlCon.Open();
            using (this.SqlCon)
            {
                using (SqlCommand cmd2 = new SqlCommand("SELECT Email, Password FROM [User] WHERE Email = @Email"))
                {
                    cmd2.Parameters.AddWithValue("@Email", this.txtLogin.Text.Trim());
                    cmd2.Connection = this.SqlCon;
                    using (SqlDataReader sdr = cmd2.ExecuteReader())
                    {
                        if (sdr.Read())
                        {
                            email = sdr["Email"].ToString();
                            password = sdr["Password"].ToString();
                        }
                    }

                    this.SqlCon.Close();
                }
            }

            if (!string.IsNullOrEmpty(password))
            {
                MailMessage emailMessage = GenerateMailMessage(email, password);
                emailMessage.To.Add(this.txtLogin.Text);
                SmtpClient smtpConnection = CreateSmptClient();
                smtpConnection.Send(emailMessage);
                this.lblError.Visible = true;
                this.lblError.ForeColor = Color.Green;
                this.lblError.Text = "Hasło zostało wysłane na twój adres Email.";
            }
            else
            {
                this.lblError.Visible = true;
                this.lblError.ForeColor = Color.Red;
                this.lblError.Text = "Adres Email nie zgadza się..";
            }

            this.SqlCon.Close();
        }

        protected void ShowProductClick(object sender, EventArgs e)
        {
            this.sqlDa = new SqlDataAdapter("SELECT Product.code, Product.name, Product.price, Country.country FROM Product INNER JOIN Country ON Product.id_country = Country.id", this.SqlCon);
            this.ShowDataFromDB(this.sqlDa);
        }

        protected void SortProductClick(object sender, EventArgs e)
        {
            string order = this.productList.SelectedValue;
            this.sqlDa = new SqlDataAdapter
            {
                SelectCommand = new SqlCommand($"SELECT Product.code, Product.name, Product.price, Country.country FROM Product INNER JOIN Country ON Product.id_country = Country.id ORDER BY {order}", this.SqlCon)
                /*
 *          someCommand.CommandText = "SELECT AccountNumber FROM Users " + "WHERE Username='" + name + "' AND Password='" + password + "'";
 *          
 *          someCommand.Parameters.Add("@username", SqlDbType.NChar).Value = name;
            someCommand.Parameters.Add("@password", SqlDbType.NChar).Value = password;
            someCommand.CommandText = "SELECT AccountNumber FROM Users " + "WHERE Username=@username AND Password=@password";
 */
            };
            this.ShowDataFromDB(this.sqlDa);
        }

        protected void SortBetweenClick(object sender, EventArgs e)
        {
            string input_num1 = this.modA.Text;
            string input_num2 = this.modB.Text;
            string order = this.productList.SelectedValue;

            try
            {
                this.sqlDa = new SqlDataAdapter
                {
                    SelectCommand = new SqlCommand($"SELECT Product.code, Product.name, Product.price, Country.country FROM Product INNER JOIN Country ON Product.id_country = Country.id WHERE price > {input_num1} and price < {input_num2} ORDER BY {order}", this.SqlCon)
                };
                this.ShowDataFromDB(this.sqlDa);
            }
            catch (Exception)
            {
                this.lblError.Visible = true;
                this.lblError.ForeColor = Color.Red;
                this.lblError.Text = "Błąd. Brak danych lub użyto przecinka zamiast kropki !!!";
            }
        }

        protected void RowSelected(object sender, EventArgs e)
        {
            Button btnAdd = sender as Button;
            GridViewRow row = btnAdd.NamingContainer as GridViewRow;
            TextBox details = row.FindControl("quantityBox") as TextBox;

            try
            {
                int codeint = Convert.ToInt32(row.Cells[0].Text);
                float priceFloat = (float)Convert.ToDouble(row.Cells[2].Text);
                int quantityInt = Convert.ToInt32(details.Text);

                if (this.Session["cart"] == null)
                {
                    List<IOrder> cart = new List<IOrder>();

                    Order newOrder = new Order(codeint, priceFloat, quantityInt);
                    cart.Add(newOrder);
                    this.Session["cart"] = cart;

                    this.lblError.Visible = true;
                    this.lblError.ForeColor = Color.Green;
                    this.lblError.Text = "Produkt dodany do koszyka!";
                }
                else
                {
                    List<IOrder> cart = (List<IOrder>)Session["cart"];

                    Order newOrder = new Order(codeint, priceFloat, quantityInt);
                    cart.Add(newOrder);
                    this.Session["cart"] = cart;

                    this.lblError.Visible = true;
                    this.lblError.ForeColor = Color.Green;
                    this.lblError.Text = "Produkt dodany do koszyka!";
                }
            }
            catch (Exception)
            {
                this.lblError.Visible = true;
                this.lblError.ForeColor = Color.Red;
                this.lblError.Text = "Pole ilość puste!";
            }
        }

        protected void BtnPlaceOrderClick(object sender, EventArgs e)
        {
            if (this.Session["email"] == null)
            {
                this.lblError.Visible = true;
                this.lblError.ForeColor = Color.Red;
                this.lblError.Text = "Aby złożyć zamówienie musisz być zalogowany!";
            }
            else
            {
                this.SqlCon.Open();
                List<IOrder> cart = (List<IOrder>)Session["cart"];
                
                int orderNumber = 0;
                string orderNumberDB = string.Empty;

                using (var cmd2 = new SqlCommand("SELECT MAX([order_number]) FROM [Account].[dbo].[Order]", this.SqlCon))
                {
                    using (SqlDataReader reader = cmd2.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orderNumberDB = reader[0].ToString();
                        }
                    }
                }

                if (!string.IsNullOrEmpty(orderNumberDB))
                {
                    orderNumber = Convert.ToInt32(orderNumberDB) + 1;
                }

                try
                {
                    if (cart != null)
                    {
                        foreach (var order in cart)
                        {
                            int codeint = order.Id;
                            float priceFloat = order.Price;
                            int quantityInt = order.Quantity;
                            decimal finalPrice = Convert.ToDecimal(priceFloat * quantityInt);

                            SqlCommand command;
                            SqlDataAdapter adapter = new SqlDataAdapter();

                            string finalWithDot = finalPrice.ToString();
                            finalWithDot = finalWithDot.Replace(",", ".");

                            string sql = $"INSERT INTO [Order] VALUES ({orderNumber}, {codeint}, {quantityInt}, {finalWithDot});";

                            command = new SqlCommand(sql, this.SqlCon);
                            adapter.InsertCommand = new SqlCommand(sql, this.SqlCon);
                            adapter.InsertCommand.ExecuteNonQuery();
                            command.Dispose();
                        }

                        this.lblError.Visible = true;
                        this.lblError.ForeColor = Color.Green;
                        this.lblError.Text = "Zamówienie złożone pomyślnie!";
                        this.Session["cart"] = null;
                        this.SqlCon.Close();
                    }
                    else
                    {
                        this.lblError.Visible = true;
                        this.lblError.ForeColor = Color.Red;
                        this.lblError.Text = "Koszyk jest pusty!";
                    }
                }
                catch (Exception)
                {
                    this.lblError.Visible = true;
                    this.lblError.ForeColor = Color.Red;
                    this.lblError.Text = "Koszyk jest pusty!";
                }
            }
        }

        protected void BtnShowCartClick(object sender, EventArgs e)
        {
            List<IOrder> cart = (List<IOrder>)Session["cart"];
            cart = cart.OrderBy(x => x.Price).ToList();
            if (cart != null)
            {
                this.gridCart.DataSource = cart;
                this.gridCart.DataBind();
            }
            else
            {
                this.lblError.Visible = true;
                this.lblError.ForeColor = Color.Red;
                this.lblError.Text = "Koszyk jest pusty!";
            }
        }

        private static SmtpClient CreateSmptClient()
        {
            return new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential("support@gmail.com", "<password>")
            };
        }

        private static MailMessage GenerateMailMessage(string email, string password)
        {
            return new MailMessage
            {
                From = new MailAddress("support@gmail.com"),
                Subject = "Odzyskanie hasła",
                Body = $"Cześć {email},<br /><br />Twoje hasło to: {password}.<br /><br />Dziękuje.",
                IsBodyHtml = true
            };
        }

        private void ShowDataFromDB(SqlDataAdapter sqlDA)
        {
            this.dt = new DataTable();
            sqlDA.Fill(this.dt);
            this.gridProductList.DataSource = this.dt;
            this.gridProductList.DataBind();
        }
    }
}