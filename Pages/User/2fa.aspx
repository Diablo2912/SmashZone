<%@ Page Title="2FA Verification" Language="C#" MasterPageFile="~/Master_Pages/Site.Master"
    AutoEventWireup="true" CodeBehind="2fa.aspx.cs" Inherits="SmashZone.Pages.User._2fa" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .otp-field {
            flex-direction: row;
            column-gap: 10px;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .otp-field input {
            height: 45px;
            width: 42px;
            border-radius: 6px;
            outline: none;
            font-size: 1.125rem;
            text-align: center;
            border: 1px solid #ddd;
        }

        .otp-field input:focus {
            box-shadow: 0 1px 0 rgba(0, 0, 0, 0.1);
        }

        .resend {
            font-size: 12px;
        }

        .resend a.disabled {
            pointer-events: none;
            opacity: 0.6;
            text-decoration: none;
            cursor: not-allowed;
        }
    </style>

    <section class="container-fluid bg-body-tertiary d-block">
        <div class="row justify-content-center">
            <div class="col-12 col-md-6 col-lg-4" style="min-width: 500px;">
                <div class="card bg-white mb-5 mt-5 border-0" style="box-shadow: 0 12px 15px rgba(0, 0, 0, 0.02);">
                    <div class="card-body p-5 text-center">
                        <h4>Verify</h4>
                        <p>Your code was sent to you via email</p>

                        <div class="otp-field mb-4">
                            <input type="text" inputmode="numeric" maxlength="1" pattern="[0-9]*" />
                            <input type="text" inputmode="numeric" maxlength="1" pattern="[0-9]*" disabled />
                            <input type="text" inputmode="numeric" maxlength="1" pattern="[0-9]*" disabled />
                            <input type="text" inputmode="numeric" maxlength="1" pattern="[0-9]*" disabled />
                            <input type="text" inputmode="numeric" maxlength="1" pattern="[0-9]*" disabled />
                            <input type="text" inputmode="numeric" maxlength="1" pattern="[0-9]*" disabled />
                        </div>

                        <asp:Button ID="btnVerify" runat="server"
                            CssClass="btn btn-login w-100 mb-4"
                            OnClick="btnOtp_Click"
                            Text="Verify" />

                        <p class="resend text-muted mb-0">
                            Didn't receive code?
                            <a id="lnkResend" class="disabled" href="2fa.aspx">
                                Request again in <span id="countdown">01:00</span>
                            </a>
                        </p>

                        <asp:HiddenField ID="hfOtp" runat="server" />
                    </div>
                </div>
            </div>
        </div>
    </section>

    <script>
        (function () {

            // ========= Countdown (60s) =========
            const lnkResend = document.getElementById("lnkResend");
            const cd = document.getElementById("countdown");

            let remaining = 60;

            function formatTime(sec) {
                const m = Math.floor(sec / 60);
                const s = sec % 60;
                return String(m).padStart(2, "0") + ":" + String(s).padStart(2, "0");
            }

            function tick() {
                cd.textContent = formatTime(remaining);

                if (remaining <= 0) {
                    lnkResend.classList.remove("disabled");
                    lnkResend.textContent = "Request again";
                    return;
                }

                remaining -= 1;
                setTimeout(tick, 1000);
            }

            tick();

            // ========= OTP Inputs =========
            const inputs = document.querySelectorAll(".otp-field > input");

            // ✅ IMPORTANT: use ASP.NET ClientID for server control
            const button = document.getElementById("<%= btnVerify.ClientID %>");
            const hfOtp = document.getElementById("<%= hfOtp.ClientID %>");

            window.addEventListener("load", () => inputs[0].focus());

            // disable verify initially
            if (button) button.disabled = true;

            // keep only digits
            inputs.forEach(i => {
                i.addEventListener("input", () => {
                    i.value = (i.value || "").replace(/\D/g, "");
                    if (i.value.length > 1) i.value = i.value.charAt(0);
                });
            });

            function refreshOtpState() {
                const otp = Array.from(inputs).map(i => i.value || "").join("");
                if (hfOtp) hfOtp.value = otp;

                const last = inputs[inputs.length - 1];
                const ready = (!last.disabled && last.value !== "");

                if (button) button.disabled = !ready;
            }

            inputs[0].addEventListener("paste", function (event) {
                event.preventDefault();

                const pasted = (event.clipboardData || window.clipboardData)
                    .getData("text")
                    .replace(/\D/g, "");

                const otpLength = inputs.length;

                // reset
                inputs.forEach((inp, idx) => {
                    inp.value = "";
                    if (idx === 0) inp.removeAttribute("disabled");
                    else inp.setAttribute("disabled", true);
                });

                // fill
                for (let i = 0; i < otpLength; i++) {
                    if (i < pasted.length) {
                        inputs[i].value = pasted[i];
                        inputs[i].removeAttribute("disabled");
                    }
                }

                // enable next
                for (let i = 1; i < otpLength; i++) {
                    if (inputs[i - 1].value !== "") inputs[i].removeAttribute("disabled");
                }

                const nextEmpty = Array.from(inputs).find(i => i.value === "");
                (nextEmpty || inputs[otpLength - 1]).focus();

                refreshOtpState();
            });

            inputs.forEach((input) => {
                input.addEventListener("keyup", (e) => {
                    const nextInput = input.nextElementSibling;
                    const prevInput = input.previousElementSibling;

                    if (nextInput && nextInput.hasAttribute("disabled") && input.value !== "") {
                        nextInput.removeAttribute("disabled");
                        nextInput.focus();
                    }

                    if (e.key === "Backspace") {
                        if (input.value === "" && prevInput) {
                            input.setAttribute("disabled", true);
                            prevInput.focus();
                            prevInput.value = "";
                        } else {
                            input.value = "";
                        }
                    }

                    refreshOtpState();
                });
            });

        })();
    </script>

</asp:Content>
