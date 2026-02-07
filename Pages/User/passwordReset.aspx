<%@ Page Title="Reset Password" Language="C#" AutoEventWireup="true"
    CodeBehind="passwordReset.aspx.cs" Inherits="SmashZone.Pages.User.passwordReset" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Reset Password</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <!-- ✅ Bootstrap 5.3 CSS -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />

    <!-- ✅ Bootstrap Icons -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css" rel="stylesheet" />

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

        .btn-login:disabled,
        .btn-login.disabled {
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

        /* Password rules */
        .pw-rules {
            width: 380px;
            margin: 0 auto 12px auto;
            text-align: left;
            font-size: 0.9rem;
            color: #6c757d;
        }

        .pw-rule {
            display: flex;
            align-items: center;
            gap: 8px;
            margin-bottom: 4px;
        }

        .pw-rule i { width: 18px; }

        .pw-rule.pass { color: #198754; }
        .pw-rule.fail { color: #dc3545; }

        .validator-spacing {
            display: block;
            padding-bottom: 16px;
        }
    </style>
</head>

<body>
<form id="form1" runat="server">

    <div class="container min-vh-100 d-flex justify-content-center align-items-center">
        <div class="login-wrapper w-100 text-center">

            <h2 class="login-title mb-3">RESET PASSWORD</h2>
            <p class="text-muted mb-4">Enter a new password below:</p>

            <!-- ✅ MESSAGE PANEL -->
            <asp:Panel ID="pnlMsg" runat="server" Visible="false" CssClass="mt-3">
                <asp:Label ID="lblMsg" runat="server" />
            </asp:Panel>

            <asp:Panel ID="pnlForm" runat="server">
                <div class="center-box">

                    <div class="mx-auto" style="width: 390px; margin-left:10px; padding-left:5px">

                        <!-- New Password -->
                        <div class="input-group mb-3" style="width: 380px;">
                            <asp:TextBox ID="txtPassword" runat="server"
                                CssClass="form-control"
                                placeholder="New Password"
                                TextMode="Password" />
                            <span class="input-group-text" id="togglePasswordBtn" style="cursor:pointer;">
                                <i class="bi bi-eye" id="togglePasswordIcon"></i>
                            </span>
                        </div>

                        <!-- Rules -->
                        <div class="pw-rules mb-2">
                            <div class="pw-rule fail" id="ruleLen"><i class="bi bi-x-circle"></i> At least 12 characters</div>
                            <div class="pw-rule fail" id="ruleUpper"><i class="bi bi-x-circle"></i> Contains Uppercase letter</div>
                            <div class="pw-rule fail" id="ruleLower"><i class="bi bi-x-circle"></i> Contains Lowercase letter</div>
                            <div class="pw-rule fail" id="ruleNum"><i class="bi bi-x-circle"></i> Contains Number</div>
                            <div class="pw-rule fail" id="ruleSym"><i class="bi bi-x-circle"></i> Contains Symbol</div>
                        </div>

                        <!-- Confirm Password -->
                        <div class="input-group mb-3" style="width: 380px;">
                            <asp:TextBox ID="txtConfirmPassword" runat="server"
                                CssClass="form-control"
                                placeholder="Confirm New Password"
                                TextMode="Password" />
                            <span class="input-group-text" id="toggleConfirmBtn" style="cursor:pointer;">
                                <i class="bi bi-eye" id="toggleConfirmIcon"></i>
                            </span>
                        </div>

                        <asp:CompareValidator
                            ID="valConfirmPassword"
                            runat="server"
                            ControlToValidate="txtConfirmPassword"
                            ControlToCompare="txtPassword"
                            Operator="Equal"
                            Type="String"
                            ErrorMessage="Passwords must match!"
                            ForeColor="Red"
                            Display="Dynamic"
                            CssClass="validator-spacing" />
                    </div>

                    <!-- RESET BUTTON -->
                    <asp:Button ID="btnReset" runat="server"
                        CssClass="btn btn-login w-100 mb-4"
                        Style="max-width: 380px;"
                        OnClick="btnReset_Click"
                        Text="RESET PASSWORD" />

                    <p class="text-muted">
                        Remember your password?
                        <a href="Login.aspx" class="login-link"><u>Login</u></a>
                    </p>

                </div>
            </asp:Panel>

        </div>
    </div>

    <script>
        (function () {
            const pwd = document.getElementById("<%= txtPassword.ClientID %>");
            const confirmPwd = document.getElementById("<%= txtConfirmPassword.ClientID %>");
            const btnReset = document.getElementById("<%= btnReset.ClientID %>");

            const togglePasswordBtn = document.getElementById("togglePasswordBtn");
            const togglePasswordIcon = document.getElementById("togglePasswordIcon");

            const toggleConfirmBtn = document.getElementById("toggleConfirmBtn");
            const toggleConfirmIcon = document.getElementById("toggleConfirmIcon");

            const rules = {
                len: document.getElementById("ruleLen"),
                upper: document.getElementById("ruleUpper"),
                lower: document.getElementById("ruleLower"),
                num: document.getElementById("ruleNum"),
                sym: document.getElementById("ruleSym")
            };

            function toggleVisibility(input, icon) {
                const isHidden = input.type === "password";
                input.type = isHidden ? "text" : "password";
                icon.classList.toggle("bi-eye", !isHidden);
                icon.classList.toggle("bi-eye-slash", isHidden);
            }

            togglePasswordBtn?.addEventListener("click", () => toggleVisibility(pwd, togglePasswordIcon));
            toggleConfirmBtn?.addEventListener("click", () => toggleVisibility(confirmPwd, toggleConfirmIcon));

            function setRule(el, ok) {
                const icon = el.querySelector("i");
                el.classList.toggle("pass", ok);
                el.classList.toggle("fail", !ok);
                icon.className = ok ? "bi bi-check-circle-fill" : "bi bi-x-circle";
            }

            function validate() {
                const v = pwd.value || "";
                const checks = {
                    len: v.length >= 12,
                    upper: /[A-Z]/.test(v),
                    lower: /[a-z]/.test(v),
                    num: /[0-9]/.test(v),
                    sym: /[^A-Za-z0-9]/.test(v)
                };

                Object.keys(checks).forEach(k => setRule(rules[k], checks[k]));

                const allRulesOk = Object.values(checks).every(Boolean);
                const matchOk = (confirmPwd.value === v) && v.length > 0;

                btnReset.disabled = !(allRulesOk && matchOk);
            }

            pwd.addEventListener("input", validate);
            confirmPwd.addEventListener("input", validate);
            validate();
        })();
    </script>

    <!-- ✅ Bootstrap JS (optional, but safe to include) -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>

</form>
</body>
</html>
