<%@ Page Title="" Language="C#" MasterPageFile="~/Master_Pages/Site.Master" AutoEventWireup="true" CodeBehind="Products.aspx.cs" Inherits="SmashZone.Pages.User.Products" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2 class="mb-3">Products</h2>

    <!-- MAIN ROW -->
    <div class="row">

        <!-- ================= LEFT FILTER COLUMN ================= -->
        <div class="col-md-2">
            <div class="accordion" id="accordionExample" style="max-width:160px;">
                <div class="accordion-item">
                    <h2 class="accordion-header">
                        <button class="accordion-button" type="button"
                            data-bs-toggle="collapse"
                            data-bs-target="#collapseOne"
                            aria-expanded="true">
                            Product Type
                        </button>
                    </h2>

                    <div id="collapseOne" class="accordion-collapse collapse show">
                        <div class="accordion-body">

                            <div class="form-check">
                                <input class="form-check-input" type="checkbox" id="filterAll" checked>
                                <label class="form-check-label" for="filterAll">
                                    All Products
                                </label>
                            </div>

                            <div class="form-check">
                                <input class="form-check-input" type="checkbox" id="filterRackets">
                                <label class="form-check-label" for="filterRackets">
                                    Rackets
                                </label>
                            </div>

                            <div class="form-check">
                                <input class="form-check-input" type="checkbox" id="filterShoes">
                                <label class="form-check-label" for="filterShoes">
                                    Shoes
                                </label>
                            </div>

                            <div class="form-check">
                                <input class="form-check-input" type="checkbox" id="filterBags">
                                <label class="form-check-label" for="filterBags">
                                    Bags
                                </label>
                            </div>

                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- ================= RIGHT PRODUCTS COLUMN ================= -->
        <div class="col-md-10">

            <!-- TOP BAR -->
            <div class="d-flex justify-content-between align-items-center mb-2">
                <!-- Product Count -->
                <i>__ Product(s) Found</i>

                <div class="dropdown">
                    <button class="btn btn-secondary dropdown-toggle" type="button"
                        data-bs-toggle="dropdown">
                        Sort By
                    </button>

                    <ul class="dropdown-menu dropdown-menu-end">
                        <li><a class="dropdown-item" href="#">Price: Low - High</a></li>
                        <li><a class="dropdown-item" href="#">Price: High - Low</a></li>
                        <li><a class="dropdown-item" href="#">Name: A - Z</a></li>
                        <li><a class="dropdown-item" href="#">Name: Z - A</a></li>
                    </ul>
                </div>
            </div>

            <!-- PRODUCTS GRID -->
            <div class="row g-4">

                <!-- Product 1 -->
                <div class="col-md-4">
                    <div class="card h-100 w-100">
                        <img src="..." class="card-img-top" alt="Product Image">
                        <div class="card-body">
                            <h5 class="card-title">Product Name</h5>
                            <p class="card-text">Short description here.</p>
                        </div>
                    </div>
                </div>

                <!-- Product 2 -->
                <div class="col-md-4">
                    <div class="card h-100 w-100">
                        <img src="..." class="card-img-top" alt="Product Image">
                        <div class="card-body">
                            <h5 class="card-title">Product Name</h5>
                            <p class="card-text">Short description here.</p>
                            <a href="#" class="btn btn-primary">View</a>
                        </div>
                    </div>
                </div>

                <!-- Product 3 -->
                <div class="col-md-4">
                    <div class="card h-100 w-100">
                        <img src="..." class="card-img-top" alt="Product Image">
                        <div class="card-body">
                            <h5 class="card-title">Product Name</h5>
                            <p class="card-text">Short description here.</p>
                            <a href="#" class="btn btn-primary">View</a>
                        </div>
                    </div>
                </div>

                <!-- Product 4 -->
                <div class="col-md-4">
                    <div class="card h-100 w-100">
                        <img src="..." class="card-img-top" alt="Product Image">
                        <div class="card-body">
                            <h5 class="card-title">Product Name</h5>
                            <p class="card-text">Short description here.</p>
                            <a href="#" class="btn btn-primary">View</a>
                        </div>
                    </div>
                </div>

                <!-- Product 5 -->
                <div class="col-md-4">
                    <div class="card h-100 w-100">
                        <img src="..." class="card-img-top" alt="Product Image">
                        <div class="card-body">
                            <h5 class="card-title">Product Name</h5>
                            <p class="card-text">Short description here.</p>
                            <a href="#" class="btn btn-primary">View</a>
                        </div>
                    </div>
                </div>

                <!-- Product 6 -->
                <div class="col-md-4">
                    <div class="card h-100 w-100">
                        <img src="..." class="card-img-top" alt="Product Image">
                        <div class="card-body">
                            <h5 class="card-title">Product Name</h5>
                            <p class="card-text">Short description here.</p>
                            <a href="#" class="btn btn-primary">View</a>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </div>
</asp:Content>
