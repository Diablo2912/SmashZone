<%@ Page Title="Products" Language="C#"
    MasterPageFile="~/Master_Pages/Site.Master"
    AutoEventWireup="true"
    CodeBehind="badmintonProducts.aspx.cs"
    Inherits="SmashZone.Pages.User.badmintonProducts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<style>
    .filter-wrap { position: sticky; top: 90px; }

    .p-card {
        position: relative;
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
    .p-img { width: 100%; height: 100%; object-fit: contain; }

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
    .p-price { font-weight: 800; margin: 0; }

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
    .p-badge-soldout { background:#fee2e2; border-color:#fecaca; color:#991b1b; }
    .p-badge-low { background:#fef3c7; border-color:#fde68a; color:#92400e; }

</style>

<h2 class="mb-3">Badminton Products</h2>

<div class="row g-4">

    <!-- LEFT FILTER -->
    <div class="col-lg-3 col-xl-2">
        <div class="filter-wrap">
            <div class="accordion" id="filtersAccordion">

                <!-- Product Type -->
                <div class="accordion-item">
                    <h2 class="accordion-header">
                        <button class="accordion-button" type="button"
                                data-bs-toggle="collapse"
                                data-bs-target="#collapseType"
                                aria-expanded="true">
                            Product Type
                        </button>
                    </h2>

                    <div id="collapseType" class="accordion-collapse collapse show"
                         data-bs-parent="#filtersAccordion">
                        <div class="accordion-body">

                            <div class="form-check">
                                <input class="form-check-input" type="checkbox" id="filterAll" checked />
                                <label class="form-check-label" for="filterAll">All Products</label>
                            </div>

                            <div class="form-check">
                                <input class="form-check-input" type="checkbox" id="filterRackets" />
                                <label class="form-check-label" for="filterRackets">Rackets</label>
                            </div>

                            <div class="form-check">
                                <input class="form-check-input" type="checkbox" id="filterShoes" />
                                <label class="form-check-label" for="filterShoes">Shoes</label>
                            </div>

                            <div class="form-check">
                                <input class="form-check-input" type="checkbox" id="filterBags" />
                                <label class="form-check-label" for="filterBags">Bags</label>
                            </div>

                        </div>
                    </div>
                </div>

                <!-- Price Filter -->
                <div class="accordion-item">
                    <h2 class="accordion-header">
                        <button class="accordion-button collapsed" type="button"
                                data-bs-toggle="collapse"
                                data-bs-target="#collapsePrice">
                            Price Filter
                        </button>
                    </h2>

                    <div id="collapsePrice" class="accordion-collapse collapse"
                         data-bs-parent="#filtersAccordion">
                        <div class="accordion-body">
                            <label class="form-label mb-2">Max Price</label>
                            <input type="range" class="form-range" min="0" max="500" value="500" id="priceRange" />
                            <div class="d-flex justify-content-between small text-muted">
                                <span>$0</span>
                                <span id="priceValue">$500</span>
                            </div>
                        </div>
                    </div>
                </div>

            </div>

            <!-- Hidden fields -->
            <asp:HiddenField ID="hfTypes" runat="server" />
            <asp:HiddenField ID="hfMaxPrice" runat="server" />

            <asp:Button ID="btnApplyFilters" runat="server"
                Text="Apply Filters"
                CssClass="btn btn-dark w-100 mt-3"
                OnClick="btnApplyFilters_Click" />

        </div>
    </div>

    <!-- RIGHT PRODUCTS -->
    <div class="col-lg-9 col-xl-10">

        <div class="d-flex justify-content-between align-items-center mb-3">
            <i><asp:Label ID="lblProductCount" runat="server" Text="0" /> Product(s) Found</i>

            <div class="dropdown">
                <button class="btn btn-outline-dark dropdown-toggle" type="button" data-bs-toggle="dropdown">
                    Sort By
                </button>
                <ul class="dropdown-menu dropdown-menu-end">
                    <li><a class="dropdown-item" href="<%= GetSortUrl("price_asc") %>">Price: Low - High</a></li>
                    <li><a class="dropdown-item" href="<%= GetSortUrl("price_desc") %>">Price: High - Low</a></li>
                    <li><a class="dropdown-item" href="<%= GetSortUrl("name_asc") %>">Name: A - Z</a></li>
                    <li><a class="dropdown-item" href="<%= GetSortUrl("name_desc") %>">Name: Z - A</a></li>
                </ul>
            </div>
        </div>

        <div class="row g-4">
            <asp:Repeater ID="rptProducts" runat="server">
                <ItemTemplate>
                    <div class="col-sm-6 col-md-4 col-xl-3">
                        <div class="card p-card h-100">

                            <!-- STOCK BADGE -->
                            <asp:Literal runat="server" Text='<%# GetStockBadge(Eval("ProductStock")) %>' />

                            <div class="p-img-wrap">
                                <img src='<%# ResolveUrl("~/" + Eval("ProductImage")) %>'
                                     class="p-img" alt="Product" />
                            </div>

                            <div class="card-body d-flex flex-column">
                                <div class="p-title"><%# Eval("ProductTitle") %></div>

                                <p class="p-price mb-3">
                                    $<%# Eval("ProductPrice", "{0:F2}") %>
                                </p>

                                <a class='btn btn-dark mt-auto <%# GetViewBtnClass(Eval("ProductStock")) %>'
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

<script>
    (function () {
        const range = document.getElementById("priceRange");
        const label = document.getElementById("priceValue");

        const hfTypes = document.getElementById("<%= hfTypes.ClientID %>");
        const hfMaxPrice = document.getElementById("<%= hfMaxPrice.ClientID %>");

        const all = document.getElementById("filterAll");
        const rackets = document.getElementById("filterRackets");
        const shoes = document.getElementById("filterShoes");
        const bags = document.getElementById("filterBags");

        function syncFilters() {
            const types = [];
            if (all.checked) types.push("All");
            if (rackets.checked) types.push("Rackets");
            if (shoes.checked) types.push("Shoes");
            if (bags.checked) types.push("Bags");

            hfTypes.value = types.join(",");
            hfMaxPrice.value = range.value;

            if (label) label.textContent = "$" + range.value;
        }

        all.addEventListener("change", () => {
            if (all.checked) [rackets, shoes, bags].forEach(x => x.checked = false);
            syncFilters();
        });

        [rackets, shoes, bags].forEach(cb => {
            cb.addEventListener("change", () => {
                if (cb.checked) all.checked = false;
                if (![rackets, shoes, bags].some(x => x.checked)) all.checked = true;
                syncFilters();
            });
        });

        range.addEventListener("input", syncFilters);

        // restore UI from server values if present
        const savedTypes = (hfTypes.value || "All").split(",");
        const savedMax = hfMaxPrice.value || "500";
        range.value = savedMax;

        all.checked = savedTypes.includes("All");
        rackets.checked = savedTypes.includes("Rackets");
        shoes.checked = savedTypes.includes("Shoes");
        bags.checked = savedTypes.includes("Bags");

        if (!rackets.checked && !shoes.checked && !bags.checked) all.checked = true;

        syncFilters();
    })();
</script>

</asp:Content>
