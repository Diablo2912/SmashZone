<%@ Page Title="FAQ" Language="C#" MasterPageFile="~/Master_Pages/Site.Master"
    AutoEventWireup="true" CodeBehind="Faq.aspx.cs" Inherits="SmashZone.Pages.User.Faq" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>How Can We Help ?</h2>
    <h3>FAQs</h3>

    <!-- ===================== GENERAL QUESTIONS ===================== -->
    <h4>General Questions</h4>

    <div class="accordion accordion-flush" id="accGeneral">
        <!-- Q1 -->
        <div class="accordion-item">
            <h2 class="accordion-header" id="accGeneralHeadingOne">
                <button class="accordion-button collapsed" type="button"
                        data-bs-toggle="collapse"
                        data-bs-target="#accGeneralCollapseOne"
                        aria-expanded="false"
                        aria-controls="accGeneralCollapseOne">
                    How can I contact you?
                </button>
            </h2>

            <div id="accGeneralCollapseOne" class="accordion-collapse collapse"
                 aria-labelledby="accGeneralHeadingOne"
                 data-bs-parent="#accGeneral">
                <div class="accordion-body">
                    If you have any questions about your order, please feel free to call (+65) 1234578
                    or email us at support@smashzone.com. We are open Mon–Fri from 9am to 5pm.
                </div>
            </div>
        </div>

        <!-- Q2 -->
        <div class="accordion-item">
            <h2 class="accordion-header" id="accGeneralHeadingTwo">
                <button class="accordion-button collapsed" type="button"
                        data-bs-toggle="collapse"
                        data-bs-target="#accGeneralCollapseTwo"
                        aria-expanded="false"
                        aria-controls="accGeneralCollapseTwo">
                    Where are you located?
                </button>
            </h2>

            <div id="accGeneralCollapseTwo" class="accordion-collapse collapse"
                 aria-labelledby="accGeneralHeadingTwo"
                 data-bs-parent="#accGeneral">
                <div class="accordion-body">
                    We are based in Singapore. You can find our latest store location and opening hours on the Contact page.
                </div>
            </div>
        </div>

        <!-- Q3 -->
        <div class="accordion-item">
            <h2 class="accordion-header" id="accGeneralHeadingThree">
                <button class="accordion-button collapsed" type="button"
                        data-bs-toggle="collapse"
                        data-bs-target="#accGeneralCollapseThree"
                        aria-expanded="false"
                        aria-controls="accGeneralCollapseThree">
                    Do you have a physical store?
                </button>
            </h2>

            <div id="accGeneralCollapseThree" class="accordion-collapse collapse"
                 aria-labelledby="accGeneralHeadingThree"
                 data-bs-parent="#accGeneral">
                <div class="accordion-body">
                    Yes. We have a showroom for selected items. Availability may vary, so we recommend checking stock online first.
                </div>
            </div>
        </div>
    </div>


    <!-- ===================== ORDERING QUESTIONS ===================== -->
    <h4 class="mt-4">Ordering Questions</h4>

    <div class="accordion accordion-flush" id="accOrdering">
        <!-- Q1 -->
        <div class="accordion-item">
            <h2 class="accordion-header" id="accOrderingHeadingOne">
                <button class="accordion-button collapsed" type="button"
                        data-bs-toggle="collapse"
                        data-bs-target="#accOrderingCollapseOne"
                        aria-expanded="false"
                        aria-controls="accOrderingCollapseOne">
                    How do I place an order?
                </button>
            </h2>

            <div id="accOrderingCollapseOne" class="accordion-collapse collapse"
                 aria-labelledby="accOrderingHeadingOne"
                 data-bs-parent="#accOrdering">
                <div class="accordion-body">
                    Browse products, add items to your cart, then proceed to checkout. You’ll receive an email confirmation after payment.
                </div>
            </div>
        </div>

        <!-- Q2 -->
        <div class="accordion-item">
            <h2 class="accordion-header" id="accOrderingHeadingTwo">
                <button class="accordion-button collapsed" type="button"
                        data-bs-toggle="collapse"
                        data-bs-target="#accOrderingCollapseTwo"
                        aria-expanded="false"
                        aria-controls="accOrderingCollapseTwo">
                    Is your ordering system secure?
                </button>
            </h2>

            <div id="accOrderingCollapseTwo" class="accordion-collapse collapse"
                 aria-labelledby="accOrderingHeadingTwo"
                 data-bs-parent="#accOrdering">
                <div class="accordion-body">
                    Yes. We use industry-standard security practices and trusted payment providers to protect your payment information.
                </div>
            </div>
        </div>

        <!-- Q3 -->
        <div class="accordion-item">
            <h2 class="accordion-header" id="accOrderingHeadingThree">
                <button class="accordion-button collapsed" type="button"
                        data-bs-toggle="collapse"
                        data-bs-target="#accOrderingCollapseThree"
                        aria-expanded="false"
                        aria-controls="accOrderingCollapseThree">
                    Can I change or cancel my order?
                </button>
            </h2>

            <div id="accOrderingCollapseThree" class="accordion-collapse collapse"
                 aria-labelledby="accOrderingHeadingThree"
                 data-bs-parent="#accOrdering">
                <div class="accordion-body">
                    If your order has not been processed, we may be able to change or cancel it. Please contact support as soon as possible.
                </div>
            </div>
        </div>
    </div>


    <!-- ===================== SHIPPING QUESTIONS ===================== -->
    <h4 class="mt-4">Shipping Questions</h4>

    <div class="accordion accordion-flush" id="accShipping">
        <!-- Q1 -->
        <div class="accordion-item">
            <h2 class="accordion-header" id="accShippingHeadingOne">
                <button class="accordion-button collapsed" type="button"
                        data-bs-toggle="collapse"
                        data-bs-target="#accShippingCollapseOne"
                        aria-expanded="false"
                        aria-controls="accShippingCollapseOne">
                    Is worldwide shipping available?
                </button>
            </h2>

            <div id="accShippingCollapseOne" class="accordion-collapse collapse"
                 aria-labelledby="accShippingHeadingOne"
                 data-bs-parent="#accShipping">
                <div class="accordion-body">
                    Unfortunately, worldwide shipping is currently not available.
                </div>
            </div>
        </div>

        <!-- Q2 -->
        <div class="accordion-item">
            <h2 class="accordion-header" id="accShippingHeadingTwo">
                <button class="accordion-button collapsed" type="button"
                        data-bs-toggle="collapse"
                        data-bs-target="#accShippingCollapseTwo"
                        aria-expanded="false"
                        aria-controls="accShippingCollapseTwo">
                    How long does delivery take?
                </button>
            </h2>

            <div id="accShippingCollapseTwo" class="accordion-collapse collapse"
                 aria-labelledby="accShippingHeadingTwo"
                 data-bs-parent="#accShipping">
                <div class="accordion-body">
                    Most orders are processed within 1–3 business days. Delivery time depends on your selected shipping option.
                </div>
            </div>
        </div>

        <!-- Q3 -->
        <div class="accordion-item">
            <h2 class="accordion-header" id="accShippingHeadingThree">
                <button class="accordion-button collapsed" type="button"
                        data-bs-toggle="collapse"
                        data-bs-target="#accShippingCollapseThree"
                        aria-expanded="false"
                        aria-controls="accShippingCollapseThree">
                    How can I track my shipment?
                </button>
            </h2>

            <div id="accShippingCollapseThree" class="accordion-collapse collapse"
                 aria-labelledby="accShippingHeadingThree"
                 data-bs-parent="#accShipping">
                <div class="accordion-body">
                    Once your order ships, you’ll receive a tracking link via email. You can also check order status in your account.
                </div>
            </div>
        </div>
    </div>

</asp:Content>
