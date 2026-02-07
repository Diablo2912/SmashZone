<%@ Page Title="Login" Language="C#" MasterPageFile="~/Master_Pages/Site.Master"
    AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SmashZone.Pages.User.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        body { background-color: #f7f7f7; }

        .login-wrapper { max-width: 480px; margin-top: -50px; }
        .login-title { letter-spacing: 0.4em; font-weight: 500; }

        .btn-login {
            position: relative;
            overflow: hidden;
            background-color: #111;
            color: #fff;
            border-radius: 0;
            border: 1px solid transparent;
            padding-block: 0.9rem;
            letter-spacing: 0.3em;
            font-weight: 500;
            transition: color 0.4s ease, border-color 0.7s ease;
        }

        .btn-login::before {
            content: "";
            position: absolute;
            inset: 0;
            background-color: #fff;
            transform: translateX(-100%);
            transition: transform 0.7s ease;
            z-index: 0;
        }

        .btn-login span { position: relative; z-index: 1; }

        .btn-login:hover::before { transform: translateX(0); }
        .btn-login:hover { color: #000; border-color: #000; }

        .btn-login:disabled {
            background-color: #111 !important;
            color: #fff !important;
            opacity: 1 !important;
            cursor: not-allowed;
        }

        .login-link { color: #6c757d; text-decoration: none; }
        .login-link:hover { text-decoration: underline; }

        .center-box {
            display: flex;
            justify-content: center;
            flex-direction: column;
            align-items: center;
        }

        .center-box input { width: 100%; max-width: 380px; }
        .center-link { width: 100%; text-align: center; }
    </style>

    <div class="container min-vh-100 d-flex justify-content-center align-items-center">
        <div class="login-wrapper w-100 text-center">

            <h2 class="login-title mb-3">LOGIN</h2>
            <p class="text-muted mb-4">Enter your email and password to login:</p>

            <!-- ✅ MESSAGE PANEL -->
            <asp:Panel ID="pnlMsg" runat="server" Visible="false" CssClass="mt-3">
                <asp:Label ID="lblMsg" runat="server" />
            </asp:Panel>

            <div class="center-box">

                <div class="mx-auto" style="width: 390px; margin-left:10px; padding-left:5px">
                    <asp:TextBox ID="txtEmail" runat="server"
                        CssClass="form-control mb-3"
                        placeholder="E-mail" TextMode="Email" />

                    <div class="input-group mb-3" style="width: 380px;">
                        <asp:TextBox ID="txtPassword" runat="server"
                            CssClass="form-control"
                            placeholder="Password"
                            TextMode="Password" />

                        <span class="input-group-text" id="togglePasswordBtn" style="cursor:pointer;">
                            <i class="bi bi-eye" id="togglePasswordIcon"></i>
                        </span>
                    </div>
                </div>

                <div class="mb-4 center-link">
                    <a href="javascript:void(0)"
                       class="small login-link"
                       data-bs-toggle="modal"
                       data-bs-target="#resetModal">
                        <u>Forgot your password?</u>
                    </a>
                </div>

                <asp:Button ID="btnLogin" runat="server"
                    CssClass="btn btn-login w-100 mb-4"
                    Style="max-width: 380px;"
                    OnClick="btnLogin_Click"
                    Text="LOGIN" />
            </div>

            <p class="text-muted">
                Don't have an account?
                <a runat="server" href="~/Pages/User/signUp.aspx" class="login-link"><u>Sign Up</u></a>
            </p>
        </div>
    </div>

    <script>
        (function () {
            const btn = document.getElementById("togglePasswordBtn");
            const icon = document.getElementById("togglePasswordIcon");
            const input = document.getElementById("<%= txtPassword.ClientID %>");

            if (!btn || !icon || !input) return;

            btn.addEventListener("click", function () {
                const isPassword = input.getAttribute("type") === "password";
                input.setAttribute("type", isPassword ? "text" : "password");

                icon.classList.toggle("bi-eye");
                icon.classList.toggle("bi-eye-slash");
            });
        })();
    </script>

    <!-- ================= RESET PASSWORD MODAL ================= -->
    <div class="modal fade" id="resetModal" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">

                <div class="modal-header">
                    <h5 class="modal-title">Reset Password</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>

                <div class="modal-body">
                    <p class="text-muted mb-3">
                        Enter your email address, a reset link will be sent.
                    </p>

                    <asp:TextBox ID="txtResetEmail"
                        runat="server"
                        CssClass="form-control"
                        placeholder="Email address" />
                </div>

                <div class="modal-footer">
                    <asp:Button ID="btnSendReset"
                        runat="server"
                        CssClass="btn btn-dark"
                        Text="Send Reset Link"
                        OnClick="btnSendReset_Click" />
                </div>

            </div>
        </div>
    </div>

</asp:Content>
