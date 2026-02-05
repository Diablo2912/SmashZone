<%@ Page Title="Manage Products"
    Language="C#"
    MasterPageFile="~/Master_Pages/AdminLogin.Master"
    AutoEventWireup="true"
    CodeBehind="manageProducts.aspx.cs"
    Inherits="SmashZone.Pages.Admin.manageProducts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<style>
    .wrap { max-width: 1100px; margin: 20px auto; }
    .thumb { width: 70px; height: 70px; object-fit: contain; border-radius: 10px; border:1px solid #ddd; }
    .icon-btn { background:none; border:none; cursor:pointer; padding:0; }
    .icon-btn i { font-size:1.3rem; vertical-align:middle; }
    .icon-delete { color:#dc3545; }
    .icon-edit { color:#0d6efd; }
</style>

<div class="wrap">
    <h2 class="mb-3">Manage Products</h2>

    <asp:GridView ID="gvProducts" runat="server"
        AutoGenerateColumns="False"
        CssClass="table table-striped table-hover align-middle"
        GridLines="None"
        AllowPaging="true"
        PageSize="10"
        OnRowCommand="gvProducts_RowCommand"
        OnPageIndexChanging="gvProducts_PageIndexChanging">

        <Columns>

            <asp:BoundField DataField="Sport" HeaderText="Sport" />

            <asp:TemplateField HeaderText="Image">
                <ItemTemplate>
                    <img class="thumb"
                         src='<%# ResolveUrl("~/" + Convert.ToString(Eval("ProductImage")).TrimStart('~','/')) %>'
                         alt="Product" />
                </ItemTemplate>
            </asp:TemplateField>

            <asp:BoundField DataField="ProductTitle" HeaderText="Title" />
            <asp:BoundField DataField="ProductPrice" HeaderText="Price" DataFormatString="{0:C}" HtmlEncode="false" />
            <asp:BoundField DataField="ProductStock" HeaderText="Stock" />
            <asp:BoundField DataField="ProductCategory" HeaderText="Category" />

            <asp:TemplateField HeaderText="Actions">
                <ItemTemplate>

                    <asp:LinkButton ID="lnkEdit" runat="server"
                        CommandName="EditProduct"
                        CommandArgument='<%# Eval("Id") + "|" + Eval("SourceTable") %>'
                        CssClass="icon-btn me-2"
                        CausesValidation="false">
                        <i class="bi bi-pencil icon-edit"></i>
                    </asp:LinkButton>

                    <asp:LinkButton ID="lnkDelete" runat="server"
                        CommandName="DeleteProduct"
                        CommandArgument='<%# Eval("Id") + "|" + Eval("SourceTable") + "|" + Eval("ProductImage") %>'
                        CssClass="icon-btn"
                        OnClientClick="return confirm('Delete this product?');"
                        CausesValidation="false">
                        <i class="bi bi-trash-fill icon-delete"></i>
                    </asp:LinkButton>

                </ItemTemplate>
            </asp:TemplateField>

            <%-- FEATURED (MAX 3 TOTAL) --%>
            <asp:TemplateField HeaderText="Featured">
                <ItemTemplate>
                    <asp:CheckBox ID="chkFeatured" runat="server"
                        Checked='<%# Convert.ToBoolean(Eval("IsFeatured")) %>'
                        AutoPostBack="true"
                        OnCheckedChanged="chkFeatured_CheckedChanged" />

                    <asp:HiddenField ID="hfPid" runat="server" Value='<%# Eval("Id") %>' />
                    <asp:HiddenField ID="hfTbl" runat="server" Value='<%# Eval("SourceTable") %>' />
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>
    </asp:GridView>
</div>

</asp:Content>
