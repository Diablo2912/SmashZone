<%@ Page Title="Edit Product"
    Language="C#"
    MasterPageFile="~/Master_Pages/AdminLogin.Master"
    AutoEventWireup="true"
    CodeBehind="editProducts.aspx.cs"
    Inherits="SmashZone.Pages.Admin.editProducts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<style>
    .form-wrap { max-width: 820px; margin: 20px auto; padding: 18px; border: 1px solid #ddd; border-radius: 10px; background:#fff; }
    .row { display: flex; gap: 14px; flex-wrap: wrap; }
    .col { flex: 1; min-width: 240px; }
    .label { font-weight: 600; margin-top: 10px; display: block; }
    .input, .txt { width: 100%; padding: 10px; border: 1px solid #ccc; border-radius: 8px; }
    .txt { min-height: 110px; resize: vertical; }
    .thumb { width: 160px; height: 160px; object-fit: cover; border-radius: 12px; border: 1px solid #ddd; }
</style>

<div class="form-wrap">
    <h2>Edit Product</h2>

    <asp:Label ID="lblMsg" runat="server"></asp:Label>

    <!-- hidden fields to keep id/table/image -->
    <asp:HiddenField ID="hfId" runat="server" />
    <asp:HiddenField ID="hfTable" runat="server" />
    <asp:HiddenField ID="hfOldImage" runat="server" />

    <div class="row">
        <div class="col">
            <label class="label">Product Title</label>
            <asp:TextBox ID="txtTitle" runat="server" CssClass="input" MaxLength="150" />
        </div>

        <div class="col">
            <label class="label">Price</label>
            <asp:TextBox ID="txtPrice" runat="server" CssClass="input" />
        </div>
    </div>

    <div class="row">
        <div class="col">
            <label class="label">Stock</label>
            <asp:TextBox ID="txtStock" runat="server" CssClass="input" />
        </div>

        <div class="col">
            <label class="label">Category</label>
            <asp:TextBox ID="txtCategory" runat="server" CssClass="input" MaxLength="50" />
        </div>
    </div>

    <label class="label">Description</label>
    <asp:TextBox ID="txtDesc" runat="server" CssClass="txt" TextMode="MultiLine" MaxLength="500" />

    <div class="row" style="margin-top:14px;">
        <div class="col">
            <label class="label">Current Image</label>
            <asp:Image ID="imgPreview" runat="server" CssClass="thumb" />
        </div>

        <div class="col">
            <label class="label">Replace Image (optional)</label>
            <asp:FileUpload ID="fuImage" runat="server" />
            <div class="text-muted" style="font-size:.9rem; margin-top:6px;">
                Leave empty if you don't want to change the image.
            </div>
        </div>
    </div>

    <div style="margin-top:16px; display:flex; gap:10px;">
        <asp:Button ID="btnSave" runat="server" Text="Save Changes" CssClass="btn btn-dark" OnClick="btnSave_Click" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary"
            CausesValidation="false" OnClick="btnCancel_Click" />
    </div>
</div>

</asp:Content>
