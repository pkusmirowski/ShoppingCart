<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="Shop.Dashboard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="userInfo" runat="server" Text=""></asp:Label>
            <br />
            <asp:Button ID="btnLogout" runat="server" Text="Wyloguj" OnClick="btnLogout_Click" />
            &nbsp;
            <asp:Button ID="btnBack" runat="server" Text="Powrót" OnClick="btnBack_Click" style="height: 26px" />
        </div>
    </form>
</body>
</html>