<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
h1 {font-family:monospace; font-size:x-large; }
</style>
</head>
<body>
    <form id="form1" runat="server" >
        <h1>
        <asp:Label ID="Guess_label" runat="server" Text="_ _ _ _ _" BackColor="Turquoise" BorderStyle="Double"></asp:Label>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Label ID="PartOfSpeechText" runat="server" Text="()"></asp:Label>
       <p>
        <asp:Button ID="newGameWord" runat="server" OnClick="Button1_Click" Text="NEW GAME \ GET HINT" Font-Size="X-Large" Height="68px" Width="429px"  />
        <p>
        <asp:Button ID="check" runat="server" Text="Check My Guess:" OnClick="check_Click" Font-Size="Large" Height="35px" Width="222px" />
        <asp:TextBox ID="UserGuessText" runat="server" Font-Size="Large" Height="30px" Width="204px" ></asp:TextBox>
        &nbsp;
        <asp:Label ID="Guess_result" runat="server" Text="" ></asp:Label> 
        <p>
        <asp:Label ID="H1" runat="server" Text="Hint1"></asp:Label> <br>
        <asp:Label ID="H2" runat="server" Text="Hint2"></asp:Label> <br>
        <asp:Label ID="H3" runat="server" Text="Hint3"></asp:Label> <br>
        <asp:Label ID="H4" runat="server" Text="Hint4"></asp:Label> <br>
        <asp:Label ID="H5" runat="server" Text="Hint5"></asp:Label> <br>
        <asp:Label ID="HCount" runat="server" Text=""></asp:Label> <p>
        <asp:Label ID="ExampleS1" runat="server" Text="Example Sentence #1"></asp:Label> <p>
        <asp:Label ID="ExampleS2" runat="server" Text="Example Sentence #2"></asp:Label> <p>
        <asp:Label ID="TypeOfText" runat="server" Text="" IsVisible="False"></asp:Label> <p>
        <asp:Label ID="HasPartsText" runat="server" Text="" IsVisible="False"></asp:Label> <p>
        <asp:Label ID="HasTypesText" runat="server" Text="" IsVisible="False"></asp:Label> <p>
        <asp:Label ID="InstanceOfText" runat="server" Text="" IsVisible="False" ></asp:Label> <p>
        <asp:Label ID="HasInstancesText" runat="server" Text="" IsVisible="False" ></asp:Label> <p>
        <asp:Label ID="MemberOfText" runat="server" Text="" IsVisible="False" ></asp:Label> <p>
        <asp:Label ID="BlankLine2" runat="server" Text="" ></asp:Label> <p>
            <%--            <Slider x:Name="sfr" />--%>
        <asp:Label ID="BlankLine1" runat="server" Text="" ></asp:Label> <p>
            <asp:Label ID="SetLevel" runat="server" Text="Set Level: "></asp:Label>
            <asp:DropDownList ID="LevelDropDownList" runat="server" Font-Size="Large"  OnSelectedIndexChanged="LevelDropDownList_SelectedIndexChanged">
                <asp:ListItem>Easy</asp:ListItem>
                <asp:ListItem>Medium</asp:ListItem>
            </asp:DropDownList>
        <p> <asp:Label ID="Feedback" runat="server" Text="Feedback for this word (0-1)" ></asp:Label> <p>
            <asp:TextBox ID="sfr" runat="server" Text="0.22" Font-Size="Medium" ></asp:TextBox>
            </h1>
    </form>
</body>
</html>
