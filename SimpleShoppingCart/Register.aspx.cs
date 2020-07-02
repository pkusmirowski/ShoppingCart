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

namespace Shop
{
    public partial class Register : System.Web.UI.Page
    {
        SqlCommand cmd = new SqlCommand();
		//Połączenie z bazą danych
        SqlConnection sqlCon = new SqlConnection(@"Data Source=nazwa_bazy_danych; Initial Catalog=Account; Integrated Security=True;");

        //Funkcja haszująca
        /*
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
            lblError2.Visible = false;
            sqlCon.Open();
        }
        protected void btnRegister_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(txtLoginRegister.Text) || string.IsNullOrEmpty(txtPasswordRegister.Text) || string.IsNullOrEmpty(txtPasswordRegister2.Text))
            {
                lblError2.Visible = true;
                lblError2.Text = "Pola nie mogą być puste.";
            }
            else
            {
                if (txtPasswordRegister.Text == txtPasswordRegister2.Text)
                {
                    try
                    {
                        //Wstawienie użytkownika do bazy danych
                        SqlCommand cmd = new SqlCommand("INSERT INTO [User]" + "(Email,Password)values(@email,@password)", sqlCon);
                        cmd.Parameters.AddWithValue("@email", txtLoginRegister.Text);
                        cmd.Parameters.AddWithValue("@password", txtPasswordRegister.Text);
                        cmd.ExecuteNonQuery();
                        lblError2.Visible = true;
                        lblError2.Text = "Konto zostało utworzone";
                        sqlCon.Close();
                    }
                    catch (SqlException ex)
                    {
                        //Email jako Primary Key
                        //Sprawdza czy się powtarza w bazie
                        if (ex.Number == 2627)
                        {
                            lblError2.Visible = true;
                            lblError2.Text = "Użytkownik już istnieje.";
                        }
                        else
                        {
                            lblError2.Visible = true;
                            lblError2.Text = $"Błąd: {ex.Message}";
                        }
                        sqlCon.Close();
                    }
                }
                else
                {
                    lblError2.Visible = true;
                    lblError2.Text = "Podane hasła są różne.";
                }
            }
        }

        protected void btnRegisterBack_Click(object sender, EventArgs e) => Response.Redirect("Login.aspx");
        protected void txtPasswordRegister_TextChanged(object sender, EventArgs e) => txtPasswordRegister.Text = txtPasswordRegister.Text;

    }
}