using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Dokan.WebEssentials
{
    public static class EmailTemplate
    {
        public static string WebAddress { get { return HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority); } }

        public static string PrepareInvoice(
            string header,
            string description,
            string buttonText,
            string buttonLink,
            string shippingAddress,
            string shippingDate,
            string orderNumber,
            string productsListHtml,
            string subtotal,
            string tax,
            string shippingCost,
            string total)
        {
            string layout = File.ReadAllText(WebAddress + "/EmailTemplates/Layout.html");
            string invoice = File.ReadAllText(WebAddress + "/EmailTemplates/Invoice.html");

            invoice = invoice
                .Replace("HEADER", header)
                .Replace("DESCRIPTION", description)
                .Replace("BUTTON_TEXT", buttonText)
                .Replace("BUTTON_LINK", buttonLink)
                .Replace("SHIPPING_ADDRESS", shippingAddress)
                .Replace("SHIPPING_DATE", shippingDate)
                .Replace("ORDER_NUMBER", orderNumber)
                .Replace("PRODUCTS_LIST", productsListHtml)
                .Replace("SUBTOTAL", subtotal)
                .Replace("TAX", tax)
                .Replace("SHIPPING_COST", shippingCost)
                .Replace("TOTAL", total);

            layout = layout.Replace("CONTENT", invoice);
            return layout;
        }

        public static string PrepareReply(
            string header,
            string description,
            string message,
            string additionalRowsHtml)
        {
            string layout = File.ReadAllText(WebAddress + "/EmailTemplates/Layout.html");
            string reply = File.ReadAllText(WebAddress + "/EmailTemplates/Reply.html");

            reply = reply
                .Replace("HEADER", header)
                .Replace("DESCRIPTION", description)
                .Replace("MESSAGE", message)
                .Replace("ADDITIONAL_ROWS", additionalRowsHtml);

            layout = layout.Replace("CONTENT", reply);
            return layout;
        }

        public static string PrepareUpdate(
            string header,
            string description,
            string additionalRowsHtml)
        {
            string layout = File.ReadAllText(WebAddress + "/EmailTemplates/Layout.html");
            string update = File.ReadAllText(WebAddress + "/EmailTemplates/Update.html");

            update = update
                .Replace("HEADER", header)
                .Replace("DESCRIPTION", description)
                .Replace("ADDITIONAL_ROWS", additionalRowsHtml);

            layout = layout.Replace("CONTENT", update);
            return layout;
        }


        public static string CreateButton(
            string buttonText,
            string buttonLink)
        {
            return
                    "<tr>" +
                    "<td class=\"button\">" +
                    "<div>" +
                    $"<a href=\"{buttonLink}\"style=\"background-color:#ff6f6f;border-radius:5px;color:#ffffff;display:inline-block;font-family:'Cabin', Helvetica, Arial, sans-serif;font-size:14px;font-weight:regular;line-height:45px;text-align:center;text-decoration:none;width:155px;-webkit-text-size-adjust:none;mso-hide:all;\">" +
                    $"{buttonText}" +
                    "</a>" +
                    "</div></td>" +
                    "</tr>";
        }

        public static string CreateInvoiceProductRow(
            string productLink,
            string productImageName,
            string productTitle,
            int quantity,
            double price)
        {
            return
                "<tr>" +
                "<td class=\"item-col item\">" +
                "<table cellspacing=\"0\" cellpadding=\"0\" width=\"100%\">" +
                "<tbody>" +
                "<tr>" +
                "<td class=\"mobile-hide-img\">" +
                $"<a href=\"{productLink}\">" +
                $"<img width=\"110\" height=\"92\" src=\"{WebAddress}/Files/{productImageName}\" alt=\"item1\">" +
                "</a>" +
                "</td>" +
                "<td class=\"product\">" +
                "<span style=\"color: #4d4d4d; font-weight:bold;\">" +
                $"{productTitle}" +
                "</span>" +
                "<br>" +
                "</td>" +
                "</tr>" +
                "</tbody>" +
                "</table>" +
                "</td><td class=\"item-col quantity\">" +
                $"{quantity}" +
                "</td>" +
                "<td class=\"item-col\">" +
                $"${price:0.00}" +
                "</td>" +
                "</tr>";
        }

        public static string CreateCodeSection(
            string description,
            string code,
            string buttonText,
            string buttonLink)
        {
            return
                "<tr>" +
                "<td class=\"mini-block-container\">" +
                "<table cellspacing=\"0\" cellpadding=\"0\" width=\"100%\" style=\"border-collapse:separate !important;\">" +
                "<tbody><tr>" +
                "<td class=\"mini-block\">" +
                "<table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">" +
                "<tbody><tr>" +
                "<td style=\"padding-bottom: 30px;\">" +
                $"{description}" +
                "</td>" +
                "</tr>" +
                "<tr>" +
                "<td class=\"code-block\">" +
                $"{code}" +
                "</td>" +
                "</tr>" +
                "<tr>" +
                "<td class=\"button\">" +
                "<div><!--[if mso]>" +
                $"<a class=\"button-mobile\" href=\"{buttonLink}\" style=\"background-color:#ff6f6f;border-radius:5px;color:#ffffff;display:inline-block;font-family:'Cabin', Helvetica, Arial, sans-serif;font-size:14px;font-weight:regular;line-height:45px;text-align:center;text-decoration:none;width:155px;-webkit-text-size-adjust:none;mso-hide:all;\">" +
                $"{buttonText}" +
                "</a>" +
                "</div>" +
                "</td>" +
                "</tr>" +
                "</tbody>" +
                "</table>" +
                "</td>" +
                "</tr>" +
                "</tbody>" +
                "</table>" +
                "</td>" +
                "</tr>";
        }
    }
}
