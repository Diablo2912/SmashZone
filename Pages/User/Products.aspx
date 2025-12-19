    <%@ Page Title="Products" Language="C#"
        MasterPageFile="~/Master_Pages/Site.Master"
        AutoEventWireup="true"
        CodeBehind="Products.aspx.cs"
        Inherits="SmashZone.Pages.User.Products" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

        <style>
            /* Left filter panel */
            .filter-wrap {
                position: sticky;
                top: 90px;
            }

            /* Product card UI */
            .p-card {
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
                min-height: 2.4em; /* keeps rows aligned */
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
        </style>

        <h2 class="mb-3">Products</h2>

        <div class="row g-4">

            <!-- ================= LEFT FILTER COLUMN ================= -->
            <div class="col-lg-3 col-xl-2">
                <div class="filter-wrap">

                    <div class="accordion" id="filtersAccordion">

                        <!-- Product Type -->
                        <div class="accordion-item">
                            <h2 class="accordion-header" id="headingType">
                                <button class="accordion-button" type="button"
                                        data-bs-toggle="collapse"
                                        data-bs-target="#collapseType"
                                        aria-expanded="true"
                                        aria-controls="collapseType">
                                    Product Type
                                </button>
                            </h2>

                            <div id="collapseType" class="accordion-collapse collapse show"
                                 aria-labelledby="headingType"
                                 data-bs-parent="#filtersAccordion">
                                <div class="accordion-body">

                                    <div class="form-check">
                                        <input class="form-check-input" type="checkbox" id="filterAll" checked>
                                        <label class="form-check-label" for="filterAll">All Products</label>
                                    </div>

                                    <div class="form-check">
                                        <input class="form-check-input" type="checkbox" id="filterRackets">
                                        <label class="form-check-label" for="filterRackets">Rackets</label>
                                    </div>

                                    <div class="form-check">
                                        <input class="form-check-input" type="checkbox" id="filterShoes">
                                        <label class="form-check-label" for="filterShoes">Shoes</label>
                                    </div>

                                    <div class="form-check">
                                        <input class="form-check-input" type="checkbox" id="filterBags">
                                        <label class="form-check-label" for="filterBags">Bags</label>
                                    </div>

                                </div>
                            </div>
                        </div>

                        <!-- Price Filter -->
                        <div class="accordion-item">
                            <h2 class="accordion-header" id="headingPrice">
                                <button class="accordion-button collapsed" type="button"
                                        data-bs-toggle="collapse"
                                        data-bs-target="#collapsePrice"
                                        aria-expanded="false"
                                        aria-controls="collapsePrice">
                                    Price Filter
                                </button>
                            </h2>

                            <div id="collapsePrice" class="accordion-collapse collapse"
                                 aria-labelledby="headingPrice"
                                 data-bs-parent="#filtersAccordion">
                                <div class="accordion-body">

                                    <label class="form-label mb-2">Max Price</label>
                                    <input type="range" class="form-range" min="0" max="500" value="500" id="priceRange">
                                    <div class="d-flex justify-content-between small text-muted">
                                        <span>$0</span>
                                        <span id="priceValue">$500</span>
                                    </div>

                                </div>
                            </div>
                        </div>

                    </div>

                </div>
            </div>

            <!-- ================= RIGHT PRODUCTS COLUMN ================= -->
            <div class="col-lg-9 col-xl-10">

                <!-- TOP BAR -->
                <div class="d-flex justify-content-between align-items-center mb-3">

                    <i>
                        <asp:Label ID="lblProductCount" runat="server" Text="0" /> Product(s) Found
                    </i>

                    <div class="dropdown">
                        <button class="btn btn-outline-dark dropdown-toggle" type="button"
                                data-bs-toggle="dropdown">
                            Sort By
                        </button>

                        <ul class="dropdown-menu dropdown-menu-end">
                            <li><a class="dropdown-item" href="?sort=price_asc">Price: Low - High</a></li>
                            <li><a class="dropdown-item" href="?sort=price_desc">Price: High - Low</a></li>
                            <li><a class="dropdown-item" href="?sort=name_asc">Name: A - Z</a></li>
                            <li><a class="dropdown-item" href="?sort=name_desc">Name: Z - A</a></li>
                        </ul>
                    </div>
                </div>

                <!-- PRODUCTS GRID (Repeater) -->
                <div class="row g-4">
                    <asp:Repeater ID="rptProducts" runat="server">
                        <ItemTemplate>
                            <div class="col-sm-6 col-md-4 col-xl-3">
                                <div class="card p-card h-100">

                                    <div class="p-img-wrap">
                                        <img src='<%# Eval("ProductImage") %>' class="p-img" alt="Product" />
                                    </div>

                                    <div class="card-body d-flex flex-column">
                                        <div class="p-title"><%# Eval("ProductTitle") %></div>

                                        <p class="p-price mb-3">
                                            $<%# Eval("ProductPrice", "{0:F2}") %>
                                        </p>

                                        <a class="btn btn-dark mt-auto"
                                           href='<%# "ProductDetails.aspx?id=" + Eval("Id") %>'>
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
            // Price range label
            (function () {
                const range = document.getElementById("priceRange");
                const label = document.getElementById("priceValue");
                if (range && label) {
                    label.textContent = "$" + range.value;
                    range.addEventListener("input", () => label.textContent = "$" + range.value);
                }
            })();

            // Make "All Products" behave nicely
            (function () {
                const all = document.getElementById("filterAll");
                const others = ["filterRackets", "filterShoes", "filterBags"]
                    .map(id => document.getElementById(id))
                    .filter(Boolean);

                if (!all || others.length === 0) return;

                all.addEventListener("change", () => {
                    if (all.checked) others.forEach(cb => cb.checked = false);
                });

                others.forEach(cb => {
                    cb.addEventListener("change", () => {
                        if (cb.checked) all.checked = false;
                        if (!others.some(x => x.checked)) all.checked = true;
                    });
                });
            })();
        </script>

    </asp:Content>
