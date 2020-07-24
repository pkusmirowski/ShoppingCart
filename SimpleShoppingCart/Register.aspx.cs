namespace Shop
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    public partial class Register : System.Web.UI.Page
    {
        ////Połączenie z bazą danych
        public readonly SqlConnection SqlCon = new SqlConnection(@"Data Source=DESKTOP-G2VNIQS; Initial Catalog=Account; Integrated Security=True;");

        ////Funkcja haszująca
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

        protected void PageLoad(object sender, EventArgs e)
        {
            this.lblError2.Visible = false;
            this.SqlCon.Open();
        }

        protected void BtnRegisterClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtLoginRegister.Text) || string.IsNullOrEmpty(this.txtPasswordRegister.Text) || string.IsNullOrEmpty(this.txtPasswordRegister2.Text))
            {
                this.lblError2.Visible = true;
                this.lblError2.Text = "Pola nie mogą być puste.";
            }
            else
            {
                if (this.txtPasswordRegister.Text == this.txtPasswordRegister2.Text)
                {
                    this.SqlCon.Open();
                    try
                    {
                        ////Wstawienie użytkownika do bazy danych
                        SqlCommand cmd = new SqlCommand("INSERT INTO [User]" + "(Email,Password)values(@email,@password)", this.SqlCon);
                        cmd.Parameters.AddWithValue("@email", this.txtLoginRegister.Text);
                        cmd.Parameters.AddWithValue("@password", this.txtPasswordRegister.Text);
                        cmd.ExecuteNonQuery();
                        this.lblError2.Visible = true;
                        this.lblError2.Text = "Konto zostało utworzone";
                        this.SqlCon.Close();
                    }
                    catch (SqlException ex)
                    {
                        ////Email jako Primary Key
                        ////Sprawdza czy się powtarza w bazie
                        if (ex.Number == 2627)
                        {
                            this.lblError2.Visible = true;
                            this.lblError2.Text = "Użytkownik już istnieje.";
                        }
                        else
                        {
                            this.lblError2.Visible = true;
                            this.lblError2.Text = $"Błąd: {ex.Message}";
                        }

                        this.SqlCon.Close();
                    }
                }
                else
                {
                    this.lblError2.Visible = true;
                    this.lblError2.Text = "Podane hasła są różne.";
                }
            }
        }

        protected void BtnRegisterBackClick(object sender, EventArgs e) => Response.Redirect("Login.aspx");

        protected void TxtPasswordRegisterTextChanged(object sender, EventArgs e) => this.txtPasswordRegister.Text = this.txtPasswordRegister.Text;
    }
}