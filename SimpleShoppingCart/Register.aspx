<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="Shop.Register" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table style="margin:auto;border:5px solid white">
                <tr>
                    <td>
                        <asp:Label ID="Label3" runat="server" Text="Login"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="txtLoginRegister" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label4" runat="server" Text="Hasło"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtPasswordRegister" runat="server" TextMode="Password" OnTextChanged="txtPasswordRegister_TextChanged"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label5" runat="server" Text="Powtórz hasło"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtPasswordRegister2" runat="server" TextMode="Password"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:Button ID="btnRegister" runat="server" Text="Zarejestruj" OnClick="btnRegister_Click" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:Button ID="btnRegister2" runat="server" Text="Panel logowania" OnClick="btnRegisterBack_Click" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:Label ID="lblError2" runat="server" ForeColor="Red" Text="Hasła się nie zgadzają"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>