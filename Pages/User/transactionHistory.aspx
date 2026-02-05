<%@ Page Title="Transaction History" Language="C#"
    MasterPageFile="~/Master_Pages/UserLogin.Master"
    AutoEventWireup="true"
    CodeBehind="transactionHistory.aspx.cs"
    Inherits="SmashZone.Pages.User.transactionHistory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .wrap { max-width: 1100px; margin: 22px auto; }
        .box { border:1px solid #e5e7eb; border-radius:14px; background:#fff; padding:18px; }
        .grid { width: 100%; }
    </style>

    <div class="wrap">
        <h2 class="fw-bold mb-3">Transaction History</h2>

        <asp:Panel ID="pnlErr" runat="server" Visible="false" CssClass="alert alert-danger">
            <asp:Label ID="lblErr" runat="server"></asp:Label>
        </asp:Panel>

        <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="alert alert-info">
            No transactions found yet.
        </asp:Panel>

        <div class="box">
            <asp:GridView ID="gvTx" runat="server"
                CssClass="table table-striped table-hover align-middle grid"
                AutoGenerateColumns="false"
                EmptyDataText="No transactions found."
                OnRowDataBound="gvTx_RowDataBound">
                <Columns>

                    <asp:BoundField DataField="CreatedAt" HeaderText="Date" />

                    <asp:BoundField DataField="StripeSessionId" HeaderText="Order ID" />

                    <asp:BoundField DataField="AmountTotal" HeaderText="Amount (SGD)" />

                    <asp:BoundField DataField="Status" HeaderText="Status" />

                    <asp:TemplateField HeaderText="Receipt">
                        <ItemTemplate>
                            <asp:HyperLink ID="lnkReceipt" runat="server" Target="_blank" Text="Open" />
                        </ItemTemplate>
                    </asp:TemplateField>

                </Columns>
            </asp:GridView>
        </div>
    </div>

</asp:Content>
