<%@ Page Title="Checkout Success" Language="C#"
    MasterPageFile="~/Master_Pages/Site.Master"
    AutoEventWireup="true"
    CodeBehind="checkoutSuccess.aspx.cs"
    Inherits="SmashZone.Pages.User.checkoutSuccess" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .wrap { max-width: 1050px; margin: 22px auto; }
        .box { border:1px solid #e5e7eb; border-radius:14px; background:#fff; padding:18px; }
        .rowline { display:flex; align-items:center; gap:12px; padding:10px 0; border-top:1px solid #f2f2f2; }
        .rowline:first-child{ border-top:none; }
        .right { margin-left:auto; font-weight:800; }
    </style>

    <div class="wrap">
        <h2 class="fw-bold mb-3">Payment Successful ✅</h2>

        <asp:Panel ID="pnlErr" runat="server" Visible="false" CssClass="alert alert-danger">
            <asp:Label ID="lblErr" runat="server"></asp:Label>
        </asp:Panel>

        <asp:Panel ID="pnlOk" runat="server" Visible="false">
            <div class="box mb-3">
                <div class="rowline">
                    <div><b>Order ID:</b> <asp:Label ID="lblOrderId" runat="server" /></div>
                    <div class="right"><asp:Label ID="lblAmount" runat="server" /></div>
                </div>

                <div class="rowline">
                    <div><b>Receipt Link:</b></div>
                    <div class="right">
                        <asp:HyperLink ID="lnkReceipt" runat="server" Target="_blank" Text="Open Stripe receipt"></asp:HyperLink>
                    </div>
                </div>

                <asp:Panel ID="pnlEmailStatus" runat="server" Visible="false" CssClass="alert mt-3 mb-0">
                    <asp:Label ID="lblEmailStatus" runat="server"></asp:Label>
                </asp:Panel>
            </div>

            <div class="box">
                <h5 class="fw-bold mb-2">What’s next?</h5>
                <div class="text-muted">A receipt will be emailed automatically if Stripe provided a customer email.</div>
            </div>
        </asp:Panel>
    </div>

</asp:Content>
