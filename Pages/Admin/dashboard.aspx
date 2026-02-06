<%@ Page Title="Admin Sales Dashboard" Language="C#"
    MasterPageFile="~/Master_Pages/AdminLogin.Master"
    AutoEventWireup="true"
    CodeBehind="dashboard.aspx.cs"
    Inherits="SmashZone.Pages.Admin.dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .wrap { max-width: 1200px; margin: 22px auto; }
        .cardbox { border:1px solid #e5e7eb; border-radius:14px; background:#fff; padding:16px; }
        .grid { width:100%; }
        .kpi { font-size: 22px; font-weight: 900; }
        .muted { color:#6b7280; }
        .rowgap { display:grid; gap:12px; }
        .kpiGrid { display:grid; grid-template-columns: repeat(4, 1fr); gap:12px; }
        @media (max-width: 900px){ .kpiGrid { grid-template-columns: repeat(2, 1fr); } }
    </style>

    <div class="wrap">

        <h2 class="fw-bold mb-3">Admin Sales Dashboard</h2>

        <asp:Panel ID="pnlErr" runat="server" Visible="false" CssClass="alert alert-danger">
            <asp:Label ID="lblErr" runat="server"></asp:Label>
        </asp:Panel>

        <!-- Filters -->
        <div class="cardbox mb-3">
            <div class="d-flex flex-wrap align-items-end gap-2">
                <div>
                    <div class="fw-bold">From</div>
                    <asp:TextBox ID="txtFrom" runat="server" CssClass="form-control" TextMode="Date" />
                </div>
                <div>
                    <div class="fw-bold">To</div>
                    <asp:TextBox ID="txtTo" runat="server" CssClass="form-control" TextMode="Date" />
                </div>
                <div>
                    <asp:Button ID="btnApply" runat="server" CssClass="btn btn-primary"
                        Text="Apply Filter" OnClick="btnApply_Click" />
                </div>
                <div class="ms-auto muted">
                    Paid transactions only (Status = <b>paid</b>)
                </div>
            </div>
        </div>

        <!-- KPI Cards -->
        <div class="kpiGrid mb-3">
            <div class="cardbox">
                <div class="muted">Today Sales</div>
                <div class="kpi">S$ <asp:Label ID="lblToday" runat="server" Text="0.00" /></div>
                <div class="muted">Orders: <asp:Label ID="lblTodayOrders" runat="server" Text="0" /></div>
            </div>
            <div class="cardbox">
                <div class="muted">Last 7 Days</div>
                <div class="kpi">S$ <asp:Label ID="lbl7" runat="server" Text="0.00" /></div>
                <div class="muted">Orders: <asp:Label ID="lbl7Orders" runat="server" Text="0" /></div>
            </div>
            <div class="cardbox">
                <div class="muted">Last 30 Days</div>
                <div class="kpi">S$ <asp:Label ID="lbl30" runat="server" Text="0.00" /></div>
                <div class="muted">Orders: <asp:Label ID="lbl30Orders" runat="server" Text="0" /></div>
            </div>
            <div class="cardbox">
                <div class="muted">All Time</div>
                <div class="kpi">S$ <asp:Label ID="lblAll" runat="server" Text="0.00" /></div>
                <div class="muted">Orders: <asp:Label ID="lblAllOrders" runat="server" Text="0" /></div>
            </div>
        </div>

        <div class="rowgap">

            <!-- Chart: Daily Sales Trend -->
            <div class="cardbox">
                <div class="d-flex align-items-center">
                    <h5 class="fw-bold mb-2">Daily Sales Trend</h5>
                    <div class="ms-auto muted">
                        Range: <asp:Label ID="lblRange" runat="server" />
                    </div>
                </div>

                <canvas id="salesChart" height="90"></canvas>

                <asp:HiddenField ID="hfLabels" runat="server" />
                <asp:HiddenField ID="hfData" runat="server" />
            </div>

            <!-- Chart: Popular Products (THIS MONTH) -->
            <div class="cardbox">
                <div class="d-flex align-items-center">
                    <h5 class="fw-bold mb-2">Popular Products (This Month)</h5>
                    <div class="ms-auto muted">
                        Month: <asp:Label ID="lblMonth" runat="server" />
                    </div>
                </div>

                <canvas id="popularChart" height="110"></canvas>

                <asp:HiddenField ID="hfPopLabels" runat="server" />
                <asp:HiddenField ID="hfPopData" runat="server" />
            </div>

            <!-- Recent Transactions -->
            <div class="cardbox">
                <h5 class="fw-bold mb-2">Recent Transactions</h5>
                <asp:GridView ID="gvRecent" runat="server"
                    CssClass="table table-striped table-hover align-middle grid"
                    AutoGenerateColumns="false">
                    <Columns>
                        <asp:BoundField DataField="CreatedAt" HeaderText="Date" />
                        <asp:BoundField DataField="AccountId" HeaderText="AccountId" />
                        <asp:BoundField DataField="StripeSessionId" HeaderText="Order ID" />
                        <asp:BoundField DataField="AmountTotal" HeaderText="Amount (SGD)" />
                        <asp:BoundField DataField="Status" HeaderText="Status" />
                        <asp:TemplateField HeaderText="Receipt">
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkReceipt" runat="server"
                                    NavigateUrl='<%# Eval("ReceiptUrl") %>'
                                    Text="Open" Target="_blank" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

        </div>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>

    <script>
        (function () {

            // ----------------- Sales Trend Chart -----------------
            const labelsJson = document.getElementById('<%= hfLabels.ClientID %>').value || "[]";
            const dataJson = document.getElementById('<%= hfData.ClientID %>').value || "[]";
            const labels = JSON.parse(labelsJson);
            const data = JSON.parse(dataJson);

            const ctx = document.getElementById('salesChart').getContext('2d');

            if (window.salesChartInstance) window.salesChartInstance.destroy();

            window.salesChartInstance = new Chart(ctx, {
                type: 'line',
                data: {
                    labels: labels,
                    datasets: [{
                        label: 'Sales (SGD)',
                        data: data,
                        tension: 0.25
                    }]
                },
                options: {
                    responsive: true,
                    plugins: { legend: { display: true } },
                    scales: { y: { beginAtZero: true } }
                }
            });

            // ----------------- Popular Products Chart (Bar) -----------------
            const popLabelsJson = document.getElementById('<%= hfPopLabels.ClientID %>').value || "[]";
            const popDataJson = document.getElementById('<%= hfPopData.ClientID %>').value || "[]";
            const popLabels = JSON.parse(popLabelsJson);
            const popData = JSON.parse(popDataJson);

            const ctx2 = document.getElementById('popularChart').getContext('2d');

            if (window.popularChartInstance) window.popularChartInstance.destroy();

            window.popularChartInstance = new Chart(ctx2, {
                type: 'bar',
                data: {
                    labels: popLabels,  // ✅ X axis: product names
                    datasets: [{
                        label: 'Units Sold',
                        data: popData       // ✅ Y axis: counters
                    }]
                },
                options: {
                    responsive: true,
                    plugins: { legend: { display: true } },
                    scales: {
                        x: { title: { display: true, text: 'Product' } },
                        y: {
                            beginAtZero: true,
                            title: { display: true, text: 'Units Sold' },
                            ticks: { precision: 0 } // ✅ integer counters
                        }
                    }
                }
            });

        })();
    </script>

</asp:Content>
