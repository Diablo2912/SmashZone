<%@ Page Title="Checkout" Language="C#"
    MasterPageFile="~/Master_Pages/Site.Master"
    AutoEventWireup="true"
    CodeBehind="checkout.aspx.cs"
    Inherits="SmashZone.Pages.User.checkout" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .co-wrap { max-width: 1100px; margin: 22px auto; }
        .co-grid { display:grid; grid-template-columns: 1.2fr .8fr; gap:18px; }
        @media(max-width: 992px){ .co-grid{ grid-template-columns:1fr; } }
        .box { border:1px solid #e5e7eb; border-radius:14px; background:#fff; padding:18px; }
        .sum-row { display:flex; align-items:center; gap:12px; padding:10px 0; border-top:1px solid #f2f2f2; }
        .sum-row:first-child{ border-top:none; }
        .sum-img{ width:46px; height:46px; border:1px solid #eee; border-radius:10px; object-fit:contain; padding:6px; }
        .sum-title{ font-weight:700; }
        .sum-right{ margin-left:auto; font-weight:800; }
        .paybtn{ width:100%; padding:14px; font-weight:800; border-radius:12px; }
    </style>

    <div class="co-wrap">
        <h2 class="fw-bold mb-3">Checkout</h2>

        <div class="co-grid">

            <!-- LEFT: Payment -->
            <div class="box">
                <h5 class="fw-bold mb-2">Payment</h5>
                <div class="text-muted mb-3">Pay securely with Stripe (SGD).</div>

                <asp:Button ID="btnPay" runat="server"
                    Text="Pay"
                    CssClass="btn btn-dark paybtn"
                    OnClick="btnPay_Click" />

                <!-- ✅ Promo code info -->
                <div class="text-muted mt-2" style="font-size: 0.95rem;">
                    You can enter a promo code on the next page (Stripe Checkout).
                </div>

                <asp:Panel ID="pnlErr" runat="server" Visible="false"
                    CssClass="alert alert-danger mt-3 mb-0">
                    <asp:Label ID="lblErr" runat="server"></asp:Label>
                </asp:Panel>
            </div>

            <!-- RIGHT: Order summary -->
            <div class="box">
                <h5 class="fw-bold mb-3">Order Summary</h5>

                <asp:Repeater ID="rptSummary" runat="server">
                    <ItemTemplate>
                        <div class="sum-row">
                            <img class="sum-img" alt="img" src='<%# ResolveUrl("~/" + Eval("Image")) %>' />
                            <div>
                                <div class="sum-title"><%# Eval("Title") %></div>
                                <div class="text-muted">Qty: <%# Eval("Qty") %> · $<%# Eval("Price","{0:0.00}") %></div>
                            </div>
                            <div class="sum-right">
                                $<%# (Convert.ToDecimal(Eval("Price")) * Convert.ToInt32(Eval("Qty"))).ToString("0.00") %>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <hr />
                <div class="d-flex justify-content-between fw-bold">
                    <div>Total</div>
                    <div>$<asp:Label ID="lblTotal" runat="server" Text="0.00"></asp:Label> SGD</div>
                </div>
            </div>

        </div>
    </div>

</asp:Content>
