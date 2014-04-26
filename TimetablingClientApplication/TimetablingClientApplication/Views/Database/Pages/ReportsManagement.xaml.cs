using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text;
using System.Diagnostics;
using TimetablingClientApplication.TimetablingService;

namespace TimetablingClientApplication.Views.Database.Pages
{
    /// <summary>
    /// Interaction logic for ReportsManagement.xaml
    /// </summary>
    public partial class ReportsManagement 
    {
        private TimetablingServiceClient _client = new TimetablingServiceClient();
        public ReportsManagement()
        {
            InitializeComponent();

        }

        public void GenerateBuildingsDocument(object sender, EventArgs e)
        {
            var buildings = _client.ReturnBuildings();
            if (buildings.Any())
            {
                using (var ms = new MemoryStream())
                {
                    var document = new Document(PageSize.A4, 0, 0, 0, 0);
                    PdfWriter.GetInstance(document, new FileStream("pdfFile.pdf", FileMode.Create));
                    PdfWriter.GetInstance(document, ms).SetFullCompression();
                    document.Open();

                    var table = new PdfPTable(6);
                    table.TotalWidth = 400;

                    var cell = new PdfPCell(new Phrase("Buildings List Report"));
                    cell.Colspan = 6;
                    cell.HorizontalAlignment = 1;
                    table.AddCell(cell);

                    foreach (var b in buildings)
                    {
                        table.AddCell(b.BuildingName);
                        table.AddCell(b.BuildingNumber.ToString("D"));
                        table.AddCell(b.AddressLine1);
                        table.AddCell(b.AddressLine2);
                        table.AddCell(b.City);
                        table.AddCell(b.PostalCode);
                    }
                    
                    
                    document.Add(table);
                    document.Close();

                    //open pdf file
                    Process.Start("AcroRd32.exe", "pdfFile.pdf");
                }

            }
        }
        
        public void GenerateStaffDocument(object sender, EventArgs e)
        {
            using (var ms = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 0, 0, 0, 0);
                PdfWriter.GetInstance(document, new FileStream("pdfFile.pdf", FileMode.Create));
                PdfWriter.GetInstance(document, ms).SetFullCompression();
                document.Open();

                var page = new iTextSharp.text.Paragraph(new Chunk("Report 1"));
                document.Add(page);

                
                document.Close();

                //open pdf file
                Process.Start("AcroRd32.exe", "pdfFile.pdf");
            }


        }
        
        public void GenerateStudentsDocument(object sender, EventArgs e)
        {
            using (var ms = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 0, 0, 0, 0);
                PdfWriter.GetInstance(document, new FileStream("pdfFile.pdf", FileMode.Create));
                PdfWriter.GetInstance(document, ms).SetFullCompression();
                document.Open();

                var page = new iTextSharp.text.Paragraph(new Chunk("Report 1"));
                document.Add(page);

                
                document.Close();

                //open pdf file
                Process.Start("AcroRd32.exe", "pdfFile.pdf");
            }


        }
    }
}
