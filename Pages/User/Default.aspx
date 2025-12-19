<%@ Page Title="Home Page"
    Language="C#"
    MasterPageFile="~/Master_Pages/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Default.aspx.cs"
    Inherits="SmashZone._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- ===================== STYLES ===================== -->
    <style>
        /* ===================== HERO CAROUSEL ===================== */
        .carousel-img {
            max-height: 300px;
            width: 100%;
            object-fit: cover;
        }

        /* ===================== BROWSE BY SPORT STRIP ===================== */
        .sport-strip-wrap {
            position: relative;
        }

        .sport-strip {
            display: flex;
            gap: 1rem;
            overflow-x: auto;
            scroll-behavior: smooth;
            padding: 0.25rem 0;
            scrollbar-width: none;
        }

        .sport-strip::-webkit-scrollbar {
            display: none;
        }

        .sport-card {
            flex: 0 0 calc(25% - 0.75rem);
            min-width: calc(25% - 0.75rem);
        }

        .sport-card .card-img-top {
            height: 240px;
            object-fit: cover;
        }

        .sport-arrow {
            position: absolute;
            top: 50%;
            transform: translateY(-50%);
            width: 44px;
            height: 44px;
            border-radius: 50%;
            border: none;
            background: rgba(0,0,0,0.55);
            color: #fff;
            z-index: 5;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 22px;
            cursor: pointer;
        }

        .sport-arrow.left {
            left: -10px;
        }

        .sport-arrow.right {
            right: -10px;
        }

        @media (max-width: 991px) {
            .sport-card {
                flex: 0 0 calc(50% - 0.5rem);
                min-width: calc(50% - 0.5rem);
            }
        }

        @media (max-width: 575px) {
            .sport-card {
                flex: 0 0 100%;
                min-width: 100%;
            }
        }

        /* ===================== FEATURED PRODUCTS (ARCSABER STYLE) ===================== */
        .featured-card {
            border: 1px solid #dee2e6;
            border-radius: 8px;
            overflow: hidden;
            background: #fff;
        }

        .featured-img-wrap {
            height: 260px;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 24px;
            background: #fff;
        }

        .featured-img {
            width: 100%;
            height: 100%;
            object-fit: contain;
        }

        .featured-body {
            padding: 26px 18px 30px;
        }

        .featured-title {
            font-weight: 700;
            font-size: 1.35rem;
            line-height: 1.2;
            margin-bottom: 14px;
            min-height: 3.2em;
            display: -webkit-box;
            -webkit-line-clamp: 2;
            -webkit-box-orient: vertical;
            overflow: hidden;
        }

        .featured-price {
            font-weight: 700;
            font-size: 1.2rem;
            margin: 0;
        }
    </style>

    <!-- ===================== HERO CAROUSEL ===================== -->
    <div id="carouselExampleInterval" class="carousel slide mb-5" data-bs-ride="carousel">

        <div class="carousel-indicators">
            <button type="button" data-bs-target="#carouselExampleInterval" data-bs-slide-to="0" class="active"></button>
            <button type="button" data-bs-target="#carouselExampleInterval" data-bs-slide-to="1"></button>
            <button type="button" data-bs-target="#carouselExampleInterval" data-bs-slide-to="2"></button>
        </div>

        <div class="carousel-inner">
            <div class="carousel-item active" data-bs-interval="5000">
                <img src="/Images/4.jpg" class="d-block carousel-img" alt="Slide 1" />
            </div>
            <div class="carousel-item" data-bs-interval="5000">
                <img src="/Images/3.jpg" class="d-block carousel-img" alt="Slide 2" />
            </div>
            <div class="carousel-item" data-bs-interval="5000">
                <img src="/Images/2.jpg" class="d-block carousel-img" alt="Slide 3" />
            </div>
        </div>

        <button class="carousel-control-prev" type="button" data-bs-target="#carouselExampleInterval" data-bs-slide="prev">
            <span class="carousel-control-prev-icon"></span>
        </button>

        <button class="carousel-control-next" type="button" data-bs-target="#carouselExampleInterval" data-bs-slide="next">
            <span class="carousel-control-next-icon"></span>
        </button>
    </div>

    <!-- ===================== BROWSE BY SPORT ===================== -->
    <h4 class="mb-4">Browse By Sport</h4>

    <div class="sport-strip-wrap mb-5">

        <button class="sport-arrow left" type="button" onclick="scrollSports(-1)">‹</button>
        <button class="sport-arrow right" type="button" onclick="scrollSports(1)">›</button>

        <div id="sportStrip" class="sport-strip">

            <div class="sport-card">
                <div class="card h-100">
                    <img src="/Images/badminton.png" class="card-img-top" alt="Badminton" />
                    <div class="card-body text-center">
                        <h6 class="card-title">Badminton</h6>
                        <a href="Products.aspx?category=Badminton" class="btn btn-sm btn-primary">View</a>
                    </div>
                </div>
            </div>

            <div class="sport-card">
                <div class="card h-100">
                    <img src="/Images/squash.jpg" class="card-img-top" alt="Squash" />
                    <div class="card-body text-center">
                        <h6 class="card-title">Squash</h6>
                        <a href="Products.aspx?category=Squash" class="btn btn-sm btn-primary">View</a>
                    </div>
                </div>
            </div>

            <div class="sport-card">
                <div class="card h-100">
                    <img src="/Images/tennis.jpg" class="card-img-top" alt="Tennis" />
                    <div class="card-body text-center">
                        <h6 class="card-title">Tennis</h6>
                        <a href="Products.aspx?category=Tennis" class="btn btn-sm btn-primary">View</a>
                    </div>
                </div>
            </div>

            <div class="sport-card">
                <div class="card h-100">
                    <img src="/Images/pickleball.jpg" class="card-img-top" alt="Pickleball" />
                    <div class="card-body text-center">
                        <h6 class="card-title">Pickleball</h6>
                        <a href="Products.aspx?category=Pickleball" class="btn btn-sm btn-primary">View</a>
                    </div>
                </div>
            </div>

        </div>
    </div>

    <!-- ===================== FEATURED PRODUCTS ===================== -->
    <h4 class="mb-4">Featured Products</h4>

    <div class="row g-4">
        <asp:Repeater ID="rptFeaturedProducts" runat="server">
            <ItemTemplate>
                <div class="col-md-4">
                    <div class="card featured-card h-100">

                        <div class="featured-img-wrap">
                            <img src='<%# Eval("ProductImage") %>'
                                 class="featured-img"
                                 alt='<%# Eval("ProductTitle") %>' />
                        </div>

                        <div class="card-body featured-body text-center">
                            <h5 class="featured-title">
                                <%# Eval("ProductTitle") %>
                            </h5>

                            <p class="featured-price">
                                $<%# Eval("ProductPrice", "{0:F2}") %>
                            </p>
                        </div>

                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>

    <!-- ===================== SCRIPT ===================== -->
    <script>
        function scrollSports(direction) {
            const strip = document.getElementById("sportStrip");
            const card = strip.querySelector(".sport-card");
            if (!card) return;

            const gap = 16;
            const amount = card.getBoundingClientRect().width + gap;
            strip.scrollLeft += direction * amount;
        }
    </script>

</asp:Content>
