<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CRViewer.aspx.cs" Inherits="AccountingSystem.ReportingForms.CRInvoice" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        .fixed-font {
            font-size: 7px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <center>
        <div>
            <CR:CrystalReportViewer ID="CRReportViewer" runat="server" AutoDataBind="true" EnableDatabaseLogonPrompt="False" EnableParameterPrompt="False" OnInit="CRReportViewer_Init" Width="350px" />
        </div>
        </center>
    </form>
</body>
</html>
