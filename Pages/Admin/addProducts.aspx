<%@ Page Title="Add Product"
    Language="C#"
    MasterPageFile="~/Master_Pages/AdminLogin.Master"
    AutoEventWireup="true"
    CodeBehind="addProducts.aspx.cs"
    Inherits="SmashZone.Pages.Admin.addProducts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .form-wrap { max-width: 820px; margin: 20px auto; padding: 18px; border: 1px solid #ddd; border-radius: 10px; }
        .row { display: flex; gap: 14px; flex-wrap: wrap; }
        .col { flex: 1; min-width: 240px; }
        .label { font-weight: 600; margin-top: 10px; display: block; }
        .input, .drop, .txt { width: 100%; padding: 10px; border: 1px solid #ccc; border-radius: 8px; }
        .txt { min-height: 110px; resize: vertical; }
        .btn { padding: 10px 14px; border: none; border-radius: 8px; cursor: pointer; }
        .btn-primary { background: #111; color: #fff; }
        .btn-secondary { background: #eee; }
        .msg { margin-top: 12px; }
        .preview { margin-top: 10px; max-width: 200px; border-radius: 10px; border: 1px solid #ddd; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="form-wrap">
        <h2>Add Product</h2>

        <asp:Label ID="lblMsg" runat="server" CssClass="msg"></asp:Label>

        <!-- SPORT -->
        <label class="label">Sport / Table</label>
        <asp:DropDownList ID="ddlSport" runat="server" CssClass="drop">
            <asp:ListItem Text="-- Select Sport --" Value=""></asp:ListItem>
            <asp:ListItem Text="Badminton" Value="Badminton_Products"></asp:ListItem>
            <asp:ListItem Text="Tennis" Value="Tennis_Products"></asp:ListItem>
            <asp:ListItem Text="Squash" Value="Squash_Products"></asp:ListItem>
        </asp:DropDownList>
        <asp:RequiredFieldValidator ID="rfvSport" runat="server"
            ControlToValidate="ddlSport"
            InitialValue=""
            ErrorMessage="Please choose a sport."
            ForeColor="Red" Display="Dynamic" />

        <!-- TITLE + PRICE -->
        <div class="row">
            <div class="col">
                <label class="label">Product Title</label>
                <asp:TextBox ID="txtTitle" runat="server" CssClass="input" MaxLength="150" />
                <asp:RequiredFieldValidator ID="rfvTitle" runat="server"
                    ControlToValidate="txtTitle"
                    ErrorMessage="Title is required."
                    ForeColor="Red" Display="Dynamic" />
            </div>

            <div class="col">
                <label class="label">Price (e.g. 49.90)</label>
                <asp:TextBox ID="txtPrice" runat="server"
                    CssClass="input"
                    TextMode="Number"
                    step="0.01"
                    min="0" />
                <asp:RequiredFieldValidator ID="rfvPrice" runat="server"
                    ControlToValidate="txtPrice"
                    ErrorMessage="Price is required."
                    ForeColor="Red" Display="Dynamic" />
            </div>
        </div>

        <!-- STOCK + CATEGORY -->
        <div class="row">
            <div class="col">
                <label class="label">Stock</label>
                <asp:TextBox ID="txtStock" runat="server" CssClass="input" TextMode="Number" />
                <asp:RequiredFieldValidator ID="rfvStock" runat="server"
                    ControlToValidate="txtStock"
                    ErrorMessage="Stock is required."
                    ForeColor="Red" Display="Dynamic" />
            </div>

            <div class="col">
                <label class="label">Category</label>
                <asp:TextBox ID="txtCategory" runat="server" CssClass="input" MaxLength="50" />
                <asp:RequiredFieldValidator ID="rfvCategory" runat="server"
                    ControlToValidate="txtCategory"
                    ErrorMessage="Category is required."
                    ForeColor="Red" Display="Dynamic" />
            </div>
        </div>

        <!-- DESCRIPTION -->
        <label class="label">Description</label>
        <asp:TextBox ID="txtDesc" runat="server"
            CssClass="txt"
            TextMode="MultiLine"
            MaxLength="500" />
        <asp:RequiredFieldValidator ID="rfvDesc" runat="server"
            ControlToValidate="txtDesc"
            ErrorMessage="Description is required."
            ForeColor="Red" Display="Dynamic" />

        <!-- IMAGE -->
        <label class="label">Product Image (JPG / PNG / WebP)</label>
        <asp:FileUpload ID="fuImage" runat="server" />
        <asp:RequiredFieldValidator ID="rfvImage" runat="server"
            ControlToValidate="fuImage"
            ErrorMessage="Image is required."
            ForeColor="Red" Display="Dynamic" />

        <asp:Image ID="imgPreview" runat="server"
            CssClass="preview"
            Visible="false" />

        <!-- BUTTONS -->
        <div style="margin-top:14px; display:flex; gap:10px;">
            <asp:Button ID="btnAdd" runat="server"
                Text="Add Product"
                CssClass="btn btn-primary"
                OnClick="btnAdd_Click" />

            <asp:Button ID="btnClear" runat="server"
                Text="Clear"
                CssClass="btn btn-secondary"
                CausesValidation="false"
                OnClick="btnClear_Click" />
        </div>

    </div>

</asp:Content>
