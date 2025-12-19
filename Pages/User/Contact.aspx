<%@ Page Title="Contact" Language="C#" MasterPageFile="~/Master_Pages/Site.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="SmashZone.Contact" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main aria-labelledby="title">
        <h2 id="title"><%: Title %>.</h2>
        <address>
            128 Queensway Drive<br />
            #05-12 Queensway Tower<br />
            Singapore 149234<br />
            +65 8332 3919<br />
        </address>

        <address>
            <strong>Support:</strong>   <a href="mailto:Support@example.com">Support@smashzone.com</a><br />
        </address>
    </main>
</asp:Content>
