<%@ Page Title="Product Details"
    Language="C#"
    MasterPageFile="~/Master_Pages/Site.Master"
    AutoEventWireup="true"
    CodeBehind="productDetails.aspx.cs"
    Inherits="SmashZone.Pages.User.productDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<style>
    .pd-wrap { max-width: 1200px; margin: 22px auto; }
    .crumbs { font-size: .92rem; color: #6c757d; }
    .crumbs a { color: #6c757d; text-decoration: none; }
    .crumbs a:hover { text-decoration: underline; }

    .pd-grid { display: grid; grid-template-columns: 1.1fr 0.9fr; gap: 40px; margin-top: 18px; }
    @media (max-width: 992px) { .pd-grid { grid-template-columns: 1fr; } }

    .img-box { background:#fff; border: 1px solid #eee; border-radius: 12px; padding: 18px; }
    .img-main { width: 100%; max-height: 540px; object-fit: contain; }

    .type { text-transform: lowercase; color:#6c757d; font-size:.95rem; }
    .title { font-weight: 800; letter-spacing: .02em; font-size: 1.8rem; margin-top: 6px; }
    .stock { margin-top: 10px; font-size: .95rem; display:flex; align-items:center; gap:10px; }
    .dot { width:10px; height:10px; border-radius:50%; display:inline-block; }
    .dot-green { background:#22c55e; }
    .dot-red { background:#ef4444; }

    .price { margin-top: 10px; font-size: 1.35rem; font-weight: 700; }
    .tax { color:#6c757d; font-size:.9rem; margin-top: 2px; }

    .rating { margin-top: 10px; color:#111; display:flex; align-items:center; gap:10px; }
    .stars { letter-spacing: .08em; }
    .small-muted { color:#6c757d; font-size:.9rem; }

    .meta { margin-top: 14px; }
    .meta .row2 { display:flex; gap:10px; flex-wrap:wrap; }
    .chip { border:1px solid #ddd; padding:.35rem .6rem; border-radius: 999px; font-size: .9rem; background:#fff; }

    .qty-row { margin-top: 18px; display:flex; align-items:center; gap:12px; flex-wrap:wrap; }
    .qty-input { width: 90px; }

    .btn-wide { min-width: 210px; padding: 12px 14px; border-radius: 0; font-weight: 600; letter-spacing: .06em; }
    .btn-wishlist { background:#fff; border:1px solid #111; color:#111; }
    .btn-cart { background:#111; border:1px solid #111; color:#fff; }
    .btn-cart:hover { opacity: .92; }
    .btn-wishlist:hover { background:#f8f9fa; }

    .note { margin-top: 16px; border: 1px solid #e5e7eb; background:#f8fafc; padding: 12px 14px; display:flex; align-items:center; gap:10px; }
    .note i { color:#f59e0b; }

    .desc { margin-top: 18px; line-height: 1.6; }
</style>

<div class="pd-wrap">

    <!-- Breadcrumb -->
    <div class="crumbs">
        <a runat="server" href="~/Pages/User/Default.aspx">Home</a>
        &nbsp;›&nbsp;
        <a runat="server" href="~/Pages/User/Products.aspx">All Products</a>
        &nbsp;›&nbsp;
        <asp:Label ID="lblCrumbTitle" runat="server" Text="Product"></asp:Label>
    </div>

    <asp:Label ID="lblMsg" runat="server"></asp:Label>

    <div class="pd-grid">

        <!-- LEFT: IMAGE -->
        <div class="img-box">
            <asp:Image ID="imgProduct" runat="server" CssClass="img-main" AlternateText="Product image" />
        </div>

        <!-- RIGHT: DETAILS -->
        <div>

            <div class="type">
                <asp:Label ID="lblType" runat="server" Text="product"></asp:Label>
            </div>

            <div class="title">
                <asp:Label ID="lblTitle" runat="server"></asp:Label>
            </div>

            <div class="stock">
                <span id="stockDot" runat="server" class="dot dot-green"></span>
                <asp:Label ID="lblStockText" runat="server" Text="In stock, ready to ship"></asp:Label>
            </div>

            <div class="price">
                <asp:Label ID="lblPrice" runat="server"></asp:Label>
            </div>
            <div class="tax">Tax included.</div>

            <div class="meta">
                <div class="row2">
                    <span class="chip">
                        Category:
                        <strong><asp:Label ID="lblCategory" runat="server"></asp:Label></strong>
                    </span>

                    <span class="chip">
                        Sport:
                        <strong><asp:Label ID="lblSport" runat="server"></asp:Label></strong>
                    </span>
                </div>
            </div>

            <div class="note">
                <i class="bi bi-exclamation-triangle-fill"></i>
                <div><strong>Note:</strong> Image shown is for reference. Actual product may vary slightly.</div>
            </div>

            <!-- Quantity + buttons -->
            <div class="qty-row">
                <div>
                    <div class="small-muted mb-1">Quantity</div>
                    <asp:TextBox ID="txtQty" runat="server" CssClass="form-control qty-input"
                        Text="1" TextMode="Number" />
                </div>

                <div class="mt-4 mt-sm-0 d-flex gap-2 flex-wrap" style="align-items:flex-end;">
                    <asp:Button ID="btnWishlist" runat="server"
                        CssClass="btn btn-wishlist btn-wide"
                        Text="ADD TO WISHLIST"
                        OnClick="btnWishlist_Click" />

                    <asp:Button ID="btnAddToCart" runat="server"
                        CssClass="btn btn-cart btn-wide"
                        Text="ADD TO CART"
                        OnClick="btnAddToCart_Click" />
                </div>
            </div>

            <!-- Description -->
            <div class="desc">
                <h5 class="mt-4">Description</h5>
                <asp:Label ID="lblDesc" runat="server"></asp:Label>
            </div>

            <!-- ================= PRODUCT RATINGS (DYNAMIC) ================= -->
            <div class="desc" style="margin-top:26px;">

                <h5 class="mt-4">Ratings & Reviews</h5>

                <!-- Summary Row -->
                <div class="d-flex align-items-center gap-3 mb-3">
                    <div style="font-size:1.2rem; letter-spacing:.08em;">
                        <asp:Label ID="lblStarsBottom" runat="server" Text="☆☆☆☆☆"></asp:Label>
                    </div>

                    <div class="small-muted">
                        <strong><asp:Label ID="lblAvgRatingBottom" runat="server" Text="0.0"></asp:Label></strong>/5
                        · <asp:Label ID="lblRatingCountBottom" runat="server" Text="0"></asp:Label> review(s)
                    </div>
                </div>

                <!-- Message area -->
                <asp:Label ID="lblRatingMsg" runat="server"></asp:Label>

                <!-- Add Review Form -->
                <div class="card p-3 mb-4" style="border-radius:12px;">
                    <h6 class="mb-3" style="font-weight:700;">Leave a review</h6>

                    <div class="row g-3">
                        <div class="col-sm-4">
                            <label class="form-label">Your Rating</label>
                            <asp:DropDownList ID="ddlRating" runat="server" CssClass="form-select">
                                <asp:ListItem Text="5 - Excellent" Value="5" />
                                <asp:ListItem Text="4 - Good" Value="4" />
                                <asp:ListItem Text="3 - Okay" Value="3" />
                                <asp:ListItem Text="2 - Poor" Value="2" />
                                <asp:ListItem Text="1 - Terrible" Value="1" />
                            </asp:DropDownList>
                        </div>

                        <div class="col-sm-8">
                            <label class="form-label">Comment (optional)</label>
                            <asp:TextBox ID="txtReview" runat="server" CssClass="form-control"
                                TextMode="MultiLine" Rows="3" MaxLength="1000"
                                placeholder="Share your thoughts..." />
                        </div>
                    </div>

                    <div class="mt-3">
                        <asp:Button ID="btnSubmitReview" runat="server"
                            CssClass="btn btn-dark"
                            Text="Submit Review"
                            OnClick="btnSubmitReview_Click" />
                    </div>
                </div>

                <!-- Reviews List (Dynamic) -->
                <asp:Repeater ID="rptReviews" runat="server">
                    <HeaderTemplate>
                        <div class="list-group">
                    </HeaderTemplate>

                    <ItemTemplate>
                        <div class="list-group-item">
                            <div class="d-flex justify-content-between align-items-center">
                                <div style="letter-spacing:.08em;">
                                    <%# Eval("Stars") %>
                                </div>
                                <div class="small-muted">
                                    <%# Eval("CreatedAt", "{0:dd MMM yyyy}") %>
                                </div>
                            </div>

                            <div class="small-muted mt-1">
                                <%# Eval("UserLabel") %>
                            </div>

                            <div class="mt-2">
                                <%# Eval("CommentSafe") %>
                            </div>
                        </div>
                    </ItemTemplate>

                    <FooterTemplate>
                        </div>
                    </FooterTemplate>
                </asp:Repeater>

                <asp:Panel ID="pnlNoReviews" runat="server" Visible="false" CssClass="small-muted">
                    No reviews yet. Be the first to review!
                </asp:Panel>

            </div>
            <!-- ================= END PRODUCT RATINGS ================= -->

            <!-- Hidden fields -->
            <asp:HiddenField ID="hfId" runat="server" />
            <asp:HiddenField ID="hfTable" runat="server" />
            <asp:HiddenField ID="hfImage" runat="server" />

        </div>
    </div>
</div>

</asp:Content>
