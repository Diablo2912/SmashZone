<%@ Page Title="" Language="C#" MasterPageFile="~/Master_Pages/Site.Master" AutoEventWireup="true" CodeBehind="signUp.aspx.cs" Inherits="SmashZone.Pages.User.signUp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
/* Page background */
body {
    background-color: #f7f7f7;
}

/* Wrapper */
.login-wrapper {
    max-width: 480px;
    margin-top: 50px;
}

/* Title */
.login-title {
    letter-spacing: 0.4em;
    font-weight: 500;
}

/* Center container */
.center-box {
    display: flex;
    justify-content: center;
    flex-direction: column;
    align-items: center;
}

.center-box input {
    width: 100%;
    max-width: 380px;
}

/* Links */
.login-link {
    color: #6c757d;
    text-decoration: none;
}
.login-link:hover {
    text-decoration: underline;
}

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

.pw-rule i {
    width: 18px;
}

.pw-rule.pass { color: #198754; }
.pw-rule.fail { color: #dc3545; }

/* Validator spacing */
.validator-spacing {
    display: block;
    padding-bottom: 16px;
}

/* ================= BUTTON ================= */

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

/* sliding background */
.btn-login::before {
    content: "";
    position: absolute;
    inset: 0;
    background-color: #fff;

    transform: translateX(-100%);
    transition: transform 0.7s ease;

    z-index: 0; /* stays behind text */
}

/* text must be above overlay */
.btn-login span {
    position: relative;
    z-index: 1; /* ✅ FIX */
}

/* hover state */
.btn-login:hover::before {
    transform: translateX(0);
}

.btn-login:hover {
    color: #000;
    border-color: #000;
}

/* disabled */
.btn-login:disabled,
.btn-login.disabled {
    background-color: #111 !important;
    color: #fff !important;
    opacity: 1 !important;
    cursor: not-allowed;
}
</style>

<div class="container min-vh-100 d-flex justify-content-center align-items-center">
    <div class="login-wrapper w-100 text-center">

        <h2 class="login-title mb-3">SIGN UP</h2>
        <p class="text-muted mb-4">Please fill in the information below:</p>

        <div class="center-box">

            <div class="mx-auto" style="width: 390px;">

                <asp:TextBox ID="txtFirstName" runat="server"
                    CssClass="form-control mb-3"
                    placeholder="First Name" />

                <asp:TextBox ID="txtLastName" runat="server"
                    CssClass="form-control mb-3"
                    placeholder="Last Name" />

                <asp:TextBox ID="txtEmail" runat="server"
                    CssClass="form-control mb-3"
                    placeholder="E-mail" />

                <asp:TextBox ID="txtPhone" runat="server"
                    CssClass="form-control mb-3"
                    placeholder="Phone Number" />

                <!-- Password -->
                <div class="input-group mb-3">
                    <asp:TextBox ID="txtPassword" runat="server"
                        CssClass="form-control"
                        placeholder="Password"
                        TextMode="Password" />
                    <span class="input-group-text" style="cursor:pointer;">
                        <i class="bi bi-eye" id="togglePassword"></i>
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
                <div class="input-group mb-3">
                    <asp:TextBox ID="txtConfirmPassword" runat="server"
                        CssClass="form-control"
                        placeholder="Confirm Password"
                        TextMode="Password" />
                    <span class="input-group-text" style="cursor:pointer;">
                        <i class="bi bi-eye" id="toggleConfirmPassword"></i>
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

            <!-- SIGN UP BUTTON -->
            <asp:LinkButton ID="btnSignUp" runat="server"
                CssClass="btn btn-login w-100 mb-4 mt-3"
                style="max-width: 380px;"
                OnClick="btnSignUp_Click">
                <span>SIGN UP</span>
            </asp:LinkButton>

        </div>

        <p class="text-muted">
            Already have an account?
            <a href="Login.aspx" class="login-link"><u>Login</u></a>
        </p>

    </div>
</div>


<script>
    const pwd = document.getElementById("<%= txtPassword.ClientID %>");
    const confirmPwd = document.getElementById("<%= txtConfirmPassword.ClientID %>");
    const btn = document.getElementById("<%= btnSignUp.ClientID %>");

    const togglePassword = document.getElementById("togglePassword");
    const toggleConfirmPassword = document.getElementById("toggleConfirmPassword");

    function toggleVisibility(input, icon) {
        const isHidden = input.type === "password";
        input.type = isHidden ? "text" : "password";

        // swap icon class
        icon.classList.toggle("bi-eye", !isHidden);
        icon.classList.toggle("bi-eye-slash", isHidden);
    }

    // ✅ Click handlers
    togglePassword.addEventListener("click", () => toggleVisibility(pwd, togglePassword));
    toggleConfirmPassword.addEventListener("click", () => toggleVisibility(confirmPwd, toggleConfirmPassword));

    // ---------------- your existing rule validation ----------------
    const rules = {
        len: document.getElementById("ruleLen"),
        upper: document.getElementById("ruleUpper"),
        lower: document.getElementById("ruleLower"),
        num: document.getElementById("ruleNum"),
        sym: document.getElementById("ruleSym")
    };

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
        btn.disabled = !(Object.values(checks).every(Boolean) && confirmPwd.value === v);
    }

    pwd.addEventListener("input", validate);
    confirmPwd.addEventListener("input", validate);
    validate();
</script>
</asp:Content>
