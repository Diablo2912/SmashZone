<%@ Page Title="Cart" Language="C#"
    MasterPageFile="~/Master_Pages/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Cart.aspx.cs"
    Inherits="SmashZone.Pages.User.Cart" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .cart-wrap { max-width: 1100px; margin: 22px auto; }
        .cart-card { border:1px solid #e5e7eb; border-radius: 14px; background:#fff; }
        .cart-row { display:flex; gap:14px; align-items:center; padding:14px; border-top:1px solid #f1f1f1; }
        .cart-row:first-child { border-top:none; }
        .cart-img { width:70px; height:70px; object-fit:contain; border:1px solid #eee; border-radius:12px; padding:6px; background:#fff; }
        .cart-title { font-weight:700; }
        .cart-meta { color:#6c757d; font-size:.9rem; }
        .cart-right { margin-left:auto; text-align:right; }
        .cart-total { font-weight:800; font-size:1.1rem; }

        .qty-box { display:inline-flex; align-items:center; gap:8px; margin-top:6px; }
        .qty-pill { min-width:38px; text-align:center; padding:4px 10px; border:1px solid #ddd; border-radius:10px; background:#fff; font-weight:700; }
        .qty-btn { width:34px; height:34px; border-radius:10px; border:1px solid #ddd; background:#fff; display:flex; align-items:center; justify-content:center; }
        .qty-btn:hover { background:#f5f5f5; }

        .checkout-bar { display:flex; justify-content:space-between; align-items:center; padding:14px; border-top:1px solid #f1f1f1; gap:12px; flex-wrap:wrap; }
    </style>

    <div class="cart-wrap">
        <h2 class="mb-3 fw-bold">Cart</h2>

        <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="alert alert-light border">
            Your cart is empty.
        </asp:Panel>

        <asp:Panel ID="pnlCart" runat="server" Visible="false" CssClass="cart-card">
            <asp:Repeater ID="rptCart" runat="server" OnItemCommand="rptCart_ItemCommand">
                <ItemTemplate>
                    <div class="cart-row">
                        <img class="cart-img" alt="img"
                             src='<%# ResolveUrl("~/" + Eval("Image")) %>' />

                        <div>
                            <div class="cart-title"><%# Eval("Title") %></div>

                            <div class="cart-meta">
                                Price: $
                                <%# string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.00}", Eval("Price")) %>
                            </div>

                            <!-- Quantity Controls -->
                            <div class="qty-box">
                                <asp:LinkButton runat="server"
                                    CssClass="qty-btn"
                                    CommandName="dec"
                                    CommandArgument='<%# Eval("Id") %>'
                                    CausesValidation="false"
                                    ToolTip="Decrease">
                                    <i class="bi bi-dash"></i>
                                </asp:LinkButton>

                                <span class="qty-pill"><%# Eval("Qty") %></span>

                                <asp:LinkButton runat="server"
                                    CssClass="qty-btn"
                                    CommandName="inc"
                                    CommandArgument='<%# Eval("Id") %>'
                                    CausesValidation="false"
                                    ToolTip="Increase">
                                    <i class="bi bi-plus"></i>
                                </asp:LinkButton>

                                <asp:LinkButton runat="server"
                                    CssClass="btn btn-sm btn-outline-danger ms-2"
                                    CommandName="remove"
                                    CommandArgument='<%# Eval("Id") %>'
                                    CausesValidation="false">
                                    Remove
                                </asp:LinkButton>
                            </div>
                        </div>

                        <div class="cart-right">
                            <div class="cart-total">
                                $<%# string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.00}",
                                      (Convert.ToDecimal(Eval("Price")) * Convert.ToInt32(Eval("Qty"))) ) %>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <!-- Bottom bar -->
            <div class="checkout-bar">
                <div class="d-flex gap-2">
                    <asp:LinkButton ID="btnClear" runat="server"
                        CssClass="btn btn-outline-secondary"
                        OnClick="btnClear_Click"
                        CausesValidation="false">
                        Clear Cart
                    </asp:LinkButton>

                    <asp:LinkButton ID="btnCheckout" runat="server"
                        CssClass="btn btn-dark"
                        OnClick="btnCheckout_Click"
                        CausesValidation="false">
                        Checkout
                    </asp:LinkButton>
                </div>

                <div class="fw-bold">
                    Total: $
                    <asp:Label ID="lblGrandTotal" runat="server" Text="0.00"></asp:Label>
                </div>
            </div>
        </asp:Panel>
    </div>

</asp:Content>
