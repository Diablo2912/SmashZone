<%@ Page Title="Account Info" Language="C#"
    MasterPageFile="~/Master_Pages/UserLogin.Master"
    AutoEventWireup="true"
    CodeBehind="accountDetails.aspx.cs"
    Inherits="SmashZone.Pages.User.accountDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .acc-wrap { max-width: 980px; margin: 0 auto; }
        .acc-card { border-radius: 16px; border: 1px solid #e5e7eb; overflow: hidden; }
        .acc-header {
            background: #111; color: #fff; padding: 18px 22px;
            display: flex; align-items: center; justify-content: space-between;
        }
        .acc-header .title { margin: 0; font-weight: 700; letter-spacing: .04em; }
        .acc-avatar {
            width: 70px; height: 70px; border-radius: 50%;
            background: #f1f5f9; border: 1px solid #e2e8f0;
            display: grid; place-items: center; font-size: 30px; color: #0f172a;
        }
        .acc-meta h5 { margin: 0; font-weight: 700; }
        .acc-meta small { color: #6b7280; }
        .acc-section-title {
            font-weight: 700; letter-spacing: .06em; font-size: .85rem;
            color: #6b7280; margin-bottom: 12px;
        }
        .acc-label { font-size: .85rem; color: #6b7280; margin-bottom: 6px; }
        .acc-value { font-weight: 600; color: #111827; margin: 0; }
        .badge-soft {
            background: #eef2ff; color: #3730a3;
            border: 1px solid #c7d2fe; font-weight: 600;
        }
        .btn-red { background: #FF0000; color: #fff; }
        .btn-black:hover { background: #fff; color: #111; border: 1px solid #111; }
    </style>

    <div class="acc-wrap my-4">

        <!-- Top actions -->
        <div class="d-flex align-items-center justify-content-between mb-3">
            <a class="text-decoration-none text-dark" href="~/Pages/User/Default.aspx" runat="server">
                <i class="bi bi-arrow-left"></i><span class="ms-1">Back</span>
            </a>

            <div class="d-flex gap-2">
                <asp:Button ID="btnEdit" runat="server" Text="Edit"
                    CssClass="btn btn-outline-dark btn-sm" />

                <asp:Button ID="btnLogout" runat="server" Text="Logout"
                    CssClass="btn btn-red btn-sm"
                    OnClick="btnLogout_Click" />
            </div>
        </div>

        <div class="card acc-card shadow-sm">

            <!-- Header -->
            <div class="acc-header">
                <h5 class="title">ACCOUNT INFO</h5>
            </div>

            <div class="card-body p-4">

                <!-- Profile summary -->
                <div class="d-flex align-items-center gap-3 mb-4">
                    <div class="acc-avatar">
                        <i class="bi bi-person"></i>
                    </div>

                    <div class="acc-meta">
                        <h5>
                            <asp:Label ID="lblFullName" runat="server" Text="-" />
                        </h5>
                        <small>
                            <asp:Label ID="lblEmailTop" runat="server" Text="-" />
                        </small>

                        <div class="mt-2 d-flex gap-2 flex-wrap">
                            <span class="badge badge-soft px-3 py-2">
                                Role:
                                <asp:Label ID="lblRole" runat="server" Text="-" />
                            </span>

                            <span class="badge bg-light text-dark border px-3 py-2">
                                Status:
                                <asp:Label ID="lblStatus" runat="server" Text="Active" />
                            </span>
                        </div>
                    </div>
                </div>

                <div class="row g-4">

                    <!-- LEFT: PROFILE DETAILS -->
                    <div class="col-lg-6">
                        <div class="acc-section-title">PROFILE DETAILS</div>

                        <div class="p-3 border rounded-3">
                            <div class="row g-3">

                                <div class="col-12">
                                    <div class="acc-label">Account ID</div>
                                    <p class="acc-value">
                                        <asp:Label ID="lblAccountId" runat="server" Text="-" />
                                    </p>
                                </div>

                                <div class="col-6">
                                    <div class="acc-label">First Name</div>
                                    <p class="acc-value">
                                        <asp:Label ID="lblFirstName" runat="server" Text="-" />
                                    </p>
                                </div>

                                <div class="col-6">
                                    <div class="acc-label">Last Name</div>
                                    <p class="acc-value">
                                        <asp:Label ID="lblLastName" runat="server" Text="-" />
                                    </p>
                                </div>

                                <div class="col-12">
                                    <div class="acc-label">Email</div>
                                    <p class="acc-value">
                                        <asp:Label ID="lblEmail" runat="server" Text="-" />
                                    </p>
                                </div>

                                <div class="col-12">
                                    <div class="acc-label">Phone</div>
                                    <p class="acc-value">
                                        <asp:Label ID="lblPhone" runat="server" Text="-" />
                                    </p>
                                </div>

                            </div>
                        </div>
                    </div>

                    <!-- RIGHT: SECURITY -->
                    <div class="col-lg-6">
                        <div class="acc-section-title">SECURITY</div>

                        <div class="p-3 border rounded-3 mb-3">
                            <div class="d-flex align-items-center justify-content-between">
                                <div>
                                    <div class="fw-bold">Two-Factor Authentication</div>
                                    <div class="text-muted small">
                                        Protect your account with OTP / email verification.
                                    </div>
                                </div>

                                <!-- IMPORTANT: badge2FA must be server-side -->
                                <span id="badge2FA" runat="server"
                                      class="badge bg-danger text-white px-3 py-2">
                                    <asp:Label ID="lbl2FA" runat="server" Text="-" />
                                </span>
                            </div>

                            <div class="mt-3 d-flex gap-2 flex-wrap">
                                <asp:Button ID="btnEnable2FA" runat="server"
                                    Text="Enable 2FA"
                                    CssClass="btn btn-outline-dark btn-sm"
                                    OnClick="btnEnable2FA_Click" />

                                <asp:Button ID="btnDisable2FA" runat="server"
                                    Text="Disable 2FA"
                                    CssClass="btn btn-outline-danger btn-sm"
                                    OnClick="btnDisable2FA_Click" />
                            </div>
                        </div>
                    </div>

                </div>

                <!-- Message -->
                <asp:Label ID="lblMsg" runat="server"
                    CssClass="text-danger d-block mt-3" />

            </div>
        </div>
    </div>

</asp:Content>
