<%@ Page Title="Products" Language="C#"
    MasterPageFile="~/Master_Pages/Site.Master"
    AutoEventWireup="true"
    CodeBehind="badmintonProducts.aspx.cs"
    Inherits="SmashZone.Pages.User.badmintonProducts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<style>
    .filter-wrap { position: sticky; top: 90px; }

    .p-card {
        position: relative; /* REQUIRED for badge */
        border: 1px solid #e5e7eb;
        border-radius: 14px;
        overflow: hidden;
        background: #fff;
        transition: transform .15s ease, box-shadow .15s ease;
    }

    .p-card:hover {
        transform: translateY(-2px);
        box-shadow: 0 10px 22px rgba(0,0,0,.08);
    }

    .p-img-wrap {
        height: 220px;
        padding: 18px;
        display: flex;
        align-items: center;
        justify-content: center;
        background: #fff;
    }

    .p-img {
        width: 100%;
        height: 100%;
        object-fit: contain;
    }

    .p-title {
        font-weight: 700;
        line-height: 1.2;
        min-height: 2.4em;
        display: -webkit-box;
        -webkit-line-clamp: 2;
        -webkit-box-orient: vertical;
        overflow: hidden;
        margin-bottom: .5rem;
    }

    .p-price {
        font-weight: 800;
        margin: 0;
    }

    /* ===== BADGES ===== */
    .p-badge {
        position: absolute;
        top: 12px;
        right: 12px;
        padding: .35rem .6rem;
        border-radius: 999px;
        font-size: .75rem;
        font-weight: 800;
        letter-spacing: .06em;
        z-index: 2;
        border: 1px solid transparent;
    }

    .p-badge-soldout {
        background: #fee2e2;
        border-color: #fecaca;
        color: #991b1b;
    }

    .p-badge-low {
        background: #fef3c7;
        border-color: #fde68a;
        color: #92400e;
    }
</style>

<h2 class="mb-3">Badminton Products</h2>

<div class="row g-4">

    <!-- LEFT FILTER -->
    <div class="col-lg-3 col-xl-2">
        <div class="filter-wrap">
            <div class="accordion">

                <div class="accordion-item">
                    <h2 class="accordion-header">
                        <button class="accordion-button">Product Type</button>
                    </h2>
                    <div class="accordion-body">
                        <div class="form-check">
                            <input class="form-check-input" type="checkbox" checked />
                            <label class="form-check-label">All Products</label>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </div>

    <!-- RIGHT PRODUCTS -->
    <div class="col-lg-9 col-xl-10">

        <div class="d-flex justify-content-between align-items-center mb-3">
            <i>
                <asp:Label ID="lblProductCount" runat="server" Text="0" /> Product(s) Found
            </i>
        </div>

        <div class="row g-4">
            <asp:Repeater ID="rptProducts" runat="server">
                <ItemTemplate>

                    <div class="col-sm-6 col-md-4 col-xl-3">
                        <div class="card p-card h-100">

                            <!-- STOCK BADGE -->
                            <!-- STOCK BADGE -->
                            <asp:Literal runat="server"
                                Text='<%# GetStockBadge(Eval("ProductStock")) %>' />

                            <div class="p-img-wrap">
                                <img src='<%# ResolveUrl("~/" + Eval("ProductImage")) %>'
                                     class="p-img"
                                     alt="Product" />
                            </div>

                            <div class="card-body d-flex flex-column">
                                <div class="p-title"><%# Eval("ProductTitle") %></div>

                                <p class="p-price mb-3">
                                    $<%# Eval("ProductPrice", "{0:F2}") %>
                                </p>

                                <a class="btn btn-dark mt-auto"
                                   href='<%# ResolveUrl("~/Pages/User/productDetails.aspx?id=" + Eval("Id")) %>'>
                                    View
                                </a>
                            </div>

                        </div>
                    </div>

                </ItemTemplate>
            </asp:Repeater>
        </div>

    </div>
</div>

</asp:Content>
