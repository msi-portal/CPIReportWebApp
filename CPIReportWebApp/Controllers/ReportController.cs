using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace CPIReportWebApp.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LabelItem()
        {

            return View();
        }

        [HttpPost]
        public ActionResult ReportLabelItem(FormCollection collection)
        {            

            ViewBag.Title = "Label Item";
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ShowParameterPrompts = false;
            reportViewer.SizeToReportContent = false;
            reportViewer.Width = Unit.Pixel(800);
            reportViewer.Height = Unit.Pixel(500);

            QRCoder.QRCodeGenerator qRCodeGenerator = new QRCoder.QRCodeGenerator();
            string strQRCode = collection["item"].ToString() + collection["lot"].ToString();
            QRCoder.QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(strQRCode, QRCoder.QRCodeGenerator.ECCLevel.Q);
            QRCoder.QRCode qRCode = new QRCoder.QRCode(qRCodeData);
            Bitmap bmp = qRCode.GetGraphic(5);
            using (MemoryStream ms = new MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Bmp);
                ReportData reportData = new ReportData();
                ReportData.LabelItemDSRow labelItemDSRow = reportData.LabelItemDS.NewLabelItemDSRow();
                labelItemDSRow.QRCode = ms.ToArray();
                labelItemDSRow.item = collection["item"].ToString();
                labelItemDSRow.description = collection["description"].ToString();
                labelItemDSRow.Uf_ItmManfctrName_CPI = collection["Uf_ItmManfctrName_CPI"];
                labelItemDSRow.Uf_Color_CPI = collection["Uf_Color_CPI"];
                labelItemDSRow.Uf_PainType_CPI = collection["Uf_PainType_CPI"];
                labelItemDSRow.Uf_MixingRatio_CPI = collection["Uf_MixingRatio_CPI"];
                labelItemDSRow.Uf_NetBeratBersih_CPI = collection["Uf_NetBeratBersih_CPI"];
                labelItemDSRow.Uf_KemasanKGset_Cpi = collection["Uf_KemasanKGset_Cpi"];
                labelItemDSRow.Uf_KemasanLTset_CPI = collection["Uf_KemasanLTset_CPI"];
                labelItemDSRow.Uf_ExtDesc1_CPI = collection["Uf_ExtDesc1_CPI"];
                labelItemDSRow.Uf_ExtDesc2_CPI = collection["Uf_ExtDesc2_CPI"];
                labelItemDSRow.lot = collection["lot"];

                reportData.LabelItemDS.AddLabelItemDSRow(labelItemDSRow);

                ReportDataSource rds1 = new ReportDataSource();
                rds1.Name = "LabelItemDS";
                rds1.Value = reportData.LabelItemDS;
                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\LabelItem_RPT.rdlc";
                reportViewer.LocalReport.DataSources.Clear();
                reportViewer.LocalReport.DataSources.Add(rds1);
            }

            //DataSet ds = new IDOWebServices().GetDataSet("ue_RPT_LabelItemSp_CPI", "item, description, Uf_ItmManfctrName_CPI, Uf_Color_CPI, Uf_PainType_CPI, Uf_MixingRatio_CPI, Uf_NetBeratBersih_CPI, Uf_KemasanKGset_Cpi, Uf_KemasanLTset_CPI, Uf_ExtDesc1_CPI, Uf_ExtDesc2_CPI, lot, rcvd_qty, exp_date", "item='UM21800132-25B'");

            //foreach (DataTable table in ds.Tables)
            //{
            //    foreach (DataRow dr in table.Rows)
            //    {
            //        QRCoder.QRCodeGenerator qRCodeGenerator = new QRCoder.QRCodeGenerator();
            //        QRCoder.QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(dr["item"].ToString() + dr["lot"].ToString(), QRCoder.QRCodeGenerator.ECCLevel.Q);
            //        QRCoder.QRCode qRCode = new QRCoder.QRCode(qRCodeData);
            //        Bitmap bmp = qRCode.GetGraphic(5);
            //        using (MemoryStream ms = new MemoryStream())
            //        {
            //            bmp.Save(ms, ImageFormat.Bmp);
            //            ReportData reportData = new ReportData();
            //            ReportData.LabelItemDSRow labelItemDSRow = reportData.LabelItemDS.NewLabelItemDSRow();
            //            labelItemDSRow.QRCode = ms.ToArray();
            //            labelItemDSRow.item = dr["item"].ToString();
            //            reportData.LabelItemDS.AddLabelItemDSRow(labelItemDSRow);

            //            ReportDataSource rds1 = new ReportDataSource();
            //            rds1.Name = "LabelItemDS";
            //            rds1.Value = reportData.LabelItemDS;
            //            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\LabelItem_RPT.rdlc";
            //            reportViewer.LocalReport.DataSources.Clear();
            //            reportViewer.LocalReport.DataSources.Add(rds1);
            //        }
            //    }
            //}

            ViewBag.ReportViewer = reportViewer;
            return View("ReportView");
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult FindItem(string item)
        {

            string filter = "Item='" + item + "'";

            ItemModel.Root listItem = new ServiceRepository<ItemModel.Root>().GetList("SLItems", "Item, Description, itmUf_ItmManfctrName_CPI, itmUf_Color_CPI, itmUf_PainType_CPI, itmUf_MixingRatio_CPI, itmUf_NetBeratBersih_CPI, itmUf_KemasanKGset_Cpi, itmUf_KemasanLTset_CPI, itmUf_ExtDesc1_CPI, itmUf_ExtDesc2_CPI", filter);
            return Json(listItem.Items);
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult ItemAutoCompleteAll(string prefix)
        {

            string filter = "Item like '" + prefix + "%'";

            ItemModel.Root listItem = new ServiceRepository<ItemModel.Root>().GetList("SLItems", "Item, Description", filter);

            List<SelectListItem> resData = new List<SelectListItem>();
            if (listItem.Items != null)
            {
                listItem.Items.ForEach(row =>
                {

                    resData.Add(new SelectListItem
                    {
                        Value = (string)row.Item,
                        Text = row.Item + " - " + row.Description
                    });

                });
            }

            return Json(resData);
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult DescAutoCompleteAll(string prefix)
        {

            string filter = "Description like '%" + prefix + "%'";

            ItemModel.Root listItem = new ServiceRepository<ItemModel.Root>().GetList("SLItems", "Item, Description", filter);

            List<SelectListItem> resData = new List<SelectListItem>();
            if (listItem.Items != null)
            {
                listItem.Items.ForEach(row =>
                {

                    resData.Add(new SelectListItem
                    {
                        Value = (string)row.Item,
                        Text = row.Item + " - " + row.Description
                    });

                });
            }

            return Json(resData);
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult LotAutoComplete(string prefix, string item)
        {

            string filter = "Item='" + item + "' and Lot like '" + prefix + "%'";

            LotModel.Root listLot = new ServiceRepository<LotModel.Root>().GetList("SLLots", "Item, Lot", filter);

            List<SelectListItem> resData = new List<SelectListItem>();
            if (listLot.Items != null)
            {
                listLot.Items.ForEach(row =>
                {

                    resData.Add(new SelectListItem
                    {
                        Value = (string)row.Lot,
                        Text = row.Lot
                    });

                });
            }

            return Json(resData);
        }
    }

    public class LotModel
    {
        public LotModel()
        {

        }

        public class Items
        {
            public string Lot { get; set; }
            public string _ItemId { get; set; }
        }

        public class Root
        {
            public string Message { get; set; }
            public int MessageCode { get; set; }
            public List<Items> Items { get; set; }
            public string Bookmark { get; set; }
        }
    }

    public class ItemModel
    {
        public ItemModel()
        {

        }

        public class Items
        {
            public string Item { get; set; }
            public string Description { get; set; }
            public string itmUf_ItmManfctrName_CPI { get; set; }
            public string itmUf_Color_CPI { get; set; }
            public string itmUf_PainType_CPI { get; set; }
            public string itmUf_MixingRatio_CPI { get; set; }
            public string itmUf_NetBeratBersih_CPI { get; set; }
            public string itmUf_KemasanKGset_Cpi { get; set; }
            public string itmUf_KemasanLTset_CPI { get; set; }
            public string itmUf_ExtDesc1_CPI { get; set; }
            public string itmUf_ExtDesc2_CPI { get; set; }
            public string _ItemId { get; set; }
        }

        public class Root
        {
            public string Message { get; set; }
            public int MessageCode { get; set; }
            public List<Items> Items { get; set; }
            public string Bookmark { get; set; }
        }
    }

    public class LabelItemClass
    {
        public string item { get; set; }
        public string description { get; set; }
        public string Uf_ItmManfctrName_CPI { get; set; }
        public string Uf_Color_CPI { get; set; }
        public string Uf_PainType_CPI { get; set; }
        public string Uf_MixingRatio_CPI { get; set; }
        public string Uf_NetBeratBersih_CPI { get; set; }
        public string Uf_KemasanKGset_Cpi { get; set; }
        public string Uf_KemasanLTset_CPI { get; set; }
        public string Uf_ExtDesc1_CPI { get; set; }
        public string Uf_ExtDesc2_CPI { get; set; }
        public string lot { get; set; }
        public string rcvd_qty { get; set; }
        public DateTime exp_date { get; set; }
    }
}