using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.Drawing;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Configuration;
using System.Globalization;
using System.EnterpriseServices;
using System.Data.Odbc;

namespace Shop
{
    public partial class Login : System.Web.UI.Page
    {
        SqlCommand cmd = new SqlCommand();
		
        SqlConnection sqlCon = new SqlConnection(@"nazwa_bazy_danych; Initial Catalog=Account; Integrated Security=True;");
        SqlDataAdapter sqlDa = new SqlDataAdapter();
        DataTable dT = new DataTable();

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

        protected void Page_Load(object sender, EventArgs e)
        {
            lblError.Visible = false;
            sqlCon.Open();
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {

            string pass = txtPassword.Text; //ComputeSha256Hash(txtPassword.Text);
            SqlDataReader reader;
            cmd.Connection = sqlCon;
            cmd.CommandText = $"SELECT * FROM [User] WHERE Email='{txtLogin.Text}' AND Password='{pass}'";
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                Session["email"] = txtLogin.Text;
                string valueFromSession = Session["email"].ToString();

                sqlCon.Close();
                Response.Redirect("Dashboard.aspx");
            }
            else
            {
                lblError.Visible = true;
                sqlCon.Close();
            }
        }

        protected void btnLoginBack_Click(object sender, EventArgs e) => Response.Redirect("Register.aspx");
        protected void btnSendEmail_Click(object sender, EventArgs e)
        {
            string email = string.Empty;
            string password = string.Empty;

            using (sqlCon)
            {
                using (SqlCommand cmd2 = new SqlCommand("SELECT Email, Password FROM [User] WHERE Email = @Email"))
                {
                    cmd2.Parameters.AddWithValue("@Email", txtLogin.Text.Trim());
                    cmd2.Connection = sqlCon;
                    using (SqlDataReader sdr = cmd2.ExecuteReader())
                    {
                        if (sdr.Read())
                        {
                            email = sdr["Email"].ToString();
                            password = sdr["Password"].ToString();
                        }
                    }
                    sqlCon.Close();
                }
            }

            if (!string.IsNullOrEmpty(password))
            {
                MailMessage emailMessage = GenerateMailMessage(email, password);
                emailMessage.To.Add(txtLogin.Text);
                SmtpClient smtpConnection = CreateSmptClient();
                smtpConnection.Send(emailMessage);
                lblError.Visible = true;
                lblError.ForeColor = Color.Green;
                lblError.Text = "Hasło zostało wysłane na twój adres Email.";
            }
            else
            {
                lblError.Visible = true;
                lblError.ForeColor = Color.Red;
                lblError.Text = "Adres Email nie zgadza się..";
            }
            sqlCon.Close();
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

        protected void ShowProduct_Click(object sender, EventArgs e)
        {
            sqlDa = new SqlDataAdapter("SELECT Product.code, Product.name, Product.price, Country.country FROM Product INNER JOIN Country ON Product.id_country = Country.id", sqlCon);
            ShowDataFromDB(sqlDa);
        }

        protected void SortProduct_Click(object sender, EventArgs e)
        {
            string order = ProductList.SelectedValue;
            sqlDa = new SqlDataAdapter();
            sqlDa.SelectCommand = new SqlCommand($"SELECT Product.code, Product.name, Product.price, Country.country FROM Product INNER JOIN Country ON Product.id_country = Country.id ORDER BY {order}", sqlCon);
            ShowDataFromDB(sqlDa);
        }

        protected void SortBetween_Click(object sender, EventArgs e)
        {
            string input_num1 = modA.Text;
            string input_num2 = modB.Text;
            string order = ProductList.SelectedValue;

            try
            {
                
                sqlDa = new SqlDataAdapter();
                sqlDa.SelectCommand = new SqlCommand($"SELECT Product.code, Product.name, Product.price, Country.country FROM Product INNER JOIN Country ON Product.id_country = Country.id WHERE price > {input_num1} and price < {input_num2} ORDER BY {order}", sqlCon);
                ShowDataFromDB(sqlDa);
            }
            catch (Exception)
            {
                lblError.Visible = true;
                lblError.ForeColor = Color.Red;
                lblError.Text = "Błąd. Brak danych lub użyto przecinka zamiast kropki !!!";
            }
        }

        private void ShowDataFromDB(SqlDataAdapter sqlDA)
        {
            dT = new DataTable();
            sqlDA.Fill(dT);
            gridProductList.DataSource = dT;
            gridProductList.DataBind();
        }


        protected void Row_Selected(object sender, EventArgs e)
        {
            Button btnAdd = (sender as Button);
            GridViewRow row = (btnAdd.NamingContainer as GridViewRow);
            TextBox details = row.FindControl("quantityBox") as TextBox;

            try
            {
                int codeint = Convert.ToInt32(row.Cells[0].Text);
                float priceFloat = (float)Convert.ToDouble(row.Cells[2].Text);
                int quantityInt = Convert.ToInt32(details.Text);

                if (Session["cart"] == null)
                {
                    List<IOrder> cart = new List<IOrder>();

                    Order newOrder = new Order(codeint, priceFloat, quantityInt);
                    cart.Add(newOrder);
                    Session["cart"] = cart;

                    lblError.Visible = true;
                    lblError.ForeColor = Color.Green;
                    lblError.Text = "Produkt dodany do koszyka!";
                }
                else
                {
                    List<IOrder> cart = (List<IOrder>)Session["cart"];

                    Order newOrder = new Order(codeint, priceFloat, quantityInt);
                    cart.Add(newOrder);
                    Session["cart"] = cart;

                    lblError.Visible = true;
                    lblError.ForeColor = Color.Green;
                    lblError.Text = "Produkt dodany do koszyka!";
                }
            }
            catch (Exception)
            {
                lblError.Visible = true;
                lblError.ForeColor = Color.Red;
                lblError.Text = "Pole ilość puste!";
            }
        }

        protected void btnPlaceOrder_Click(object sender, EventArgs e)
        {
            if (Session["email"] == null)
            {
                lblError.Visible = true;
                lblError.ForeColor = Color.Red;
                lblError.Text = "Aby złożyć zamówienie musisz być zalogowany!";
            }
            else
            {
                List<IOrder> cart = (List<IOrder>)Session["cart"];
                int orderNumber = 0;
                string orderNumberDB = string.Empty;

                using (var cmd2 = new SqlCommand("SELECT MAX([order_number]) FROM [Account].[dbo].[Order]", sqlCon))
                {
                    using (SqlDataReader reader = cmd2.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orderNumberDB = reader[0].ToString();
                        }
                    }
                }

                if (orderNumberDB != "")
                    orderNumber = Convert.ToInt32(orderNumberDB) + 1;
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

                            command = new SqlCommand(sql, sqlCon);
                            adapter.InsertCommand = new SqlCommand(sql, sqlCon);
                            adapter.InsertCommand.ExecuteNonQuery();
                            command.Dispose();
                        }

                        lblError.Visible = true;
                        lblError.ForeColor = Color.Green;
                        lblError.Text = "Zamówienie złożone pomyślnie!";
                        Session["cart"] = null;
                        sqlCon.Close();
                    }
                    else
                    {
                        lblError.Visible = true;
                        lblError.ForeColor = Color.Red;
                        lblError.Text = "Koszyk jest pusty!";
                    }

                }
                catch(Exception)
                {
                    lblError.Visible = true;
                    lblError.ForeColor = Color.Red;
                    lblError.Text = "Koszyk jest pusty!";
                }
            }
        }

        protected void btnShowCart_Click(object sender, EventArgs e)
        {
            List<IOrder> cart = (List<IOrder>)Session["cart"];
            if (cart != null)
            {
                gridCart.DataSource = cart;
                gridCart.DataBind();
            }
            else
            {
                lblError.Visible = true;
                lblError.ForeColor = Color.Red;
                lblError.Text = "Koszyk jest pusty!";
            }
        }
    }
}