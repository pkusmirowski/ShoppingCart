<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Shop.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Panel Klienta</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table style="margin:auto;border:5px solid white">
                <tr>
                    <td>
                        <asp:Label ID="Label1" runat="server" Text="Login"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="txtLogin" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label2" runat="server" Text="Hasło"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:Button ID="btnLogin2" runat="server" Text="Zarejestruj się" OnClick="btnLoginBack_Click" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:Button ID="sendEmail" runat="server" Text="Przypomnij hasło" OnClick="btnSendEmail_Click" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:Label ID="lblError" runat="server" ForeColor="Red" Text="Nieprawidłowy login lub hasło"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:Button ID="Button1" runat="server" Text="Pokaż listę produktów" OnClick="ShowProduct_Click"/>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:Label ID="Label3" runat="server" Text="Pokaż wszystko według:   " ></asp:Label>
                        <asp:DropDownList ID="ProductList" runat="server">
                            <asp:ListItem Text="Nazwa" Value="name"></asp:ListItem>
                            <asp:ListItem Text="Kraj" Value="country"></asp:ListItem>
                            <asp:ListItem Text="Cena" Value="price"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:Button ID="Button4" runat="server" Text="Sortuj" OnClick="SortProduct_Click" />
                    </td>
                </tr>
                 <tr>
                    <td></td>
                    <td>
                        Cena od
                        <asp:TextBox ID="modA" runat="server" Width="90"></asp:TextBox>
                        do
                        <asp:TextBox ID="modB" runat="server" Width="90"></asp:TextBox>
                        <asp:Button ID="sortBetween" runat="server" Text="Pokaż" OnClick="SortBetween_Click" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:GridView ID="gridProductList" runat="server" AutoGenerateColumns="false">
                            <Columns>
                                <asp:BoundField DataField="code" HeaderText="Kod"/>
                                <asp:BoundField DataField="name" HeaderText="Nazwa Produktu"/>
                                <asp:BoundField DataField="price" HeaderText="Cena Produktu"/>
                                <asp:BoundField DataField="country" HeaderText="Pochodzenie"/>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:TextBox ID="quantityBox" runat="server"></asp:TextBox>
                                        <asp:Button ID="btnAdd" runat="server" Text="Dodaj do koszyka" OnClick="Row_Selected" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                           </Columns>
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:Button ID="btnShowCart" runat="server" Text="Koszyk" OnClick="btnShowCart_Click" />
                        &nbsp;
                        <asp:Button ID="btnPlaceOrder" runat="server" Text="Złóż zamówienie" OnClick="btnPlaceOrder_Click" />
                    </td>
                </tr>
                <tr>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:GridView ID="gridCart" runat="server"  AutoGenerateColumns="false">

                            <Columns>
                                <asp:TemplateField HeaderText="Kod">
                                    <ItemTemplate>
                                        <asp:Label ID="quantityBox1" runat="server" Text='<%# Bind("Id") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField HeaderText="Ilość">
                                    <ItemTemplate>
                                        <asp:Label ID="quantityBox2" runat="server" Text='<%# Bind("Quantity") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField HeaderText="Cena">
                                    <ItemTemplate>
                                        <asp:Label ID="quantityBox3" runat="server" Text='<%# Bind("Price") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                           </Columns>
                        </asp:GridView>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>