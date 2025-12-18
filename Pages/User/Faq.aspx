<%@ Page Title="" Language="C#" MasterPageFile="~/Master_Pages/Site.Master" AutoEventWireup="true" CodeBehind="Faq.aspx.cs" Inherits="SmashZone.Pages.User.Faq" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>How Can We Help ?</h2>
    <h3>FAQs</h3>

    <!-- General Questions -->
    <h4>General Questions</h4>
    <div class="accordion accordion-flush" id="accordionFlushExample">
        <div class="accordion-item">
            <h2 class="accordion-header" id="flush-headingOne">
                <button class="accordion-button collapsed" type="button"
                        data-bs-toggle="collapse"
                        data-bs-target="#flush-collapseOne"
                        aria-expanded="false"
                        aria-controls="flush-collapseOne">
                    How can I contact you?
                </button>
            </h2>
            <div id="flush-collapseOne" class="accordion-collapse collapse"
                 aria-labelledby="flush-headingOne"
                 data-bs-parent="#accordionFlushExample">
                <div class="accordion-body">
                    If you have any questions about your order, please feel free to call (+65) 1234578
                    or email us at support@smashzone.com. We are open Mon–Fri from 9am to 5pm.
                </div>
            </div>
        </div>

        <div class="accordion-item">
            <h2 class="accordion-header" id="flush-headingTwo">
                <button class="accordion-button collapsed" type="button"
                        data-bs-toggle="collapse"
                        data-bs-target="#flush-collapseTwo"
                        aria-expanded="false"
                        aria-controls="flush-collapseTwo">
                    Accordion Item #2
                </button>
            </h2>
            <div id="flush-collapseTwo" class="accordion-collapse collapse"
                 aria-labelledby="flush-headingTwo"
                 data-bs-parent="#accordionFlushExample">
                <div class="accordion-body">
                    Placeholder content for this accordion, which is intended to demonstrate the
                    <code>.accordion-flush</code> class. This is the second item’s accordion body.
                </div>
            </div>
        </div>

        <div class="accordion-item">
            <h2 class="accordion-header" id="flush-headingThree">
                <button class="accordion-button collapsed" type="button"
                        data-bs-toggle="collapse"
                        data-bs-target="#flush-collapseThree"
                        aria-expanded="false"
                        aria-controls="flush-collapseThree">
                    Accordion Item #3
                </button>
            </h2>
            <div id="flush-collapseThree" class="accordion-collapse collapse"
                 aria-labelledby="flush-headingThree"
                 data-bs-parent="#accordionFlushExample">
                <div class="accordion-body">
                    Placeholder content for this accordion, which is intended to demonstrate the
                    <code>.accordion-flush</code> class. This is the third item’s accordion body.
                </div>
            </div>
        </div>
    </div>

    <!-- Ordering Questions -->
    <h4 class="mt-4">Ordering Questions</h4>
    <div class="accordion accordion-flush" id="accordionFlushExample2">
        <div class="accordion-item">
            <h2 class="accordion-header" id="flush2-headingOne">
                <button class="accordion-button collapsed" type="button"
                        data-bs-toggle="collapse"
                        data-bs-target="#flush2-collapseOne"
                        aria-expanded="false"
                        aria-controls="flush2-collapseOne">
                    How do I place an order?
                </button>
            </h2>
            <div id="flush2-collapseOne" class="accordion-collapse collapse"
                 aria-labelledby="flush2-headingOne"
                 data-bs-parent="#accordionFlushExample2">
                <div class="accordion-body">
                    Placeholder content for ordering question #1. Replace this with actual ordering FAQ content.
                </div>
            </div>
        </div>

        <div class="accordion-item">
            <h2 class="accordion-header" id="flush2-headingTwo">
                <button class="accordion-button collapsed" type="button"
                        data-bs-toggle="collapse"
                        data-bs-target="#flush2-collapseTwo"
                        aria-expanded="false"
                        aria-controls="flush2-collapseTwo">
                    Accordion Item #2
                </button>
            </h2>
            <div id="flush2-collapseTwo" class="accordion-collapse collapse"
                 aria-labelledby="flush2-headingTwo"
                 data-bs-parent="#accordionFlushExample2">
                <div class="accordion-body">
                    Placeholder content for ordering question #2. Replace this with actual ordering FAQ content.
                </div>
            </div>
        </div>

        <div class="accordion-item">
            <h2 class="accordion-header" id="flush2-headingThree">
                <button class="accordion-button collapsed" type="button"
                        data-bs-toggle="collapse"
                        data-bs-target="#flush2-collapseThree"
                        aria-expanded="false"
                        aria-controls="flush2-collapseThree">
                    Accordion Item #3
                </button>
            </h2>
            <div id="flush2-collapseThree" class="accordion-collapse collapse"
                 aria-labelledby="flush2-headingThree"
                 data-bs-parent="#accordionFlushExample2">
                <div class="accordion-body">
                    Placeholder content for ordering question #3. Replace this with actual ordering FAQ content.
                </div>
            </div>
        </div>
    </div>
</asp:Content>
