using System;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Diagnostics;
using TimetablingClientApplication.TimetablingService;

namespace TimetablingClientApplication.Views.Database.Pages
{
    /// <summary>
    /// Interaction logic for ReportsManagement.xaml
    /// </summary>
    public partial class ReportsManagement 
    {
        //Webservice functionality exposed through service reference
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();
        public ReportsManagement()
        {
            InitializeComponent();
        }

        //Buildings List Report
        public void GenerateBuildingsDocument(object sender, EventArgs e)
        {
            //return buildings from database
            var buildings = _client.ReturnBuildings();
            //check to ensure buildings are available
            if (buildings.Any())
            {
                //Created memory stream for document generation
                using (var ms = new MemoryStream())
                {
                    //new document setup
                    var document = new Document(PageSize.A4, 0, 0, 0, 0);
                    PdfWriter.GetInstance(document, new FileStream("pdfFile.pdf", FileMode.Create));
                    PdfWriter.GetInstance(document, ms).SetFullCompression();
                    document.Open();

                    //content for document setup
                    var table = new PdfPTable(6);
                    table.TotalWidth = 400;
                    var cell = new PdfPCell(new Phrase("Buildings List Report"));
                    cell.Colspan = 6;
                    cell.HorizontalAlignment = 1;
                    table.AddCell(cell);

                    //buildings populated into page content
                    foreach (var b in buildings)
                    {
                        table.AddCell(b.BuildingName);
                        table.AddCell(b.BuildingNumber.ToString("D"));
                        table.AddCell(b.AddressLine1);
                        table.AddCell(b.AddressLine2);
                        table.AddCell(b.City);
                        table.AddCell(b.PostalCode);
                    }
                    
                    //content added to document
                    document.Add(table);
                    document.Close();

                    //open pdf file
                    Process.Start("AcroRd32.exe", "pdfFile.pdf");
                }
                //No buildings exist popup to relect this
                NoRecords.IsOpen = true;
                Line2.Text = "No Buildings exist in the database";
            }
        }
        
        //Generate Staff report
        public void GenerateStaffDocument(object sender, EventArgs e)
        {
            //Staff memebers returned
            var staff = _client.ReturnStaff();
            //chcek to sensure staff members exist
            if (staff.Any())
            {
                //memory stream setup for document generation
                using (var ms = new MemoryStream())
                {
                    //Document setup
                    var document = new Document(PageSize.A4, 0, 0, 0, 0);
                    PdfWriter.GetInstance(document, new FileStream("staffReport.pdf", FileMode.Create));
                    PdfWriter.GetInstance(document, ms).SetFullCompression();
                    document.Open();

                    //Document content setup
                    var table = new PdfPTable(5);
                    table.TotalWidth = 400;
                    //Title added to page table
                    var cell = new PdfPCell(new Phrase("Staff List Report"));
                    cell.Colspan = 5;
                    cell.HorizontalAlignment = 1;
                    table.AddCell(cell);

                    //Staff members added to the table
                    foreach (var s in staff)
                    {
                        table.AddCell(s.StaffTitle);
                        table.AddCell(s.StaffForename);
                        table.AddCell(s.StaffSurname);
                        table.AddCell(s.StaffId.ToString("D"));
                        table.AddCell(s.StaffEmail);
                    }

                    //document added content
                    document.Add(table);
                    document.Close();

                    //open pdf file
                    Process.Start("AcroRd32.exe", "staffReport.pdf");
                }
                //No staff exist popup displayed
                NoRecords.IsOpen = true;
                Line2.Text = "No Staff exist in the database";
            }
        }

        //Generate Student report
        public void GenerateStudentsDocument(object sender, EventArgs e)
        {
            //Generate students List
          var students = _client.ReturnStudents();
            //Check to ensure students list is not empty
            if (students.Any())
            {
                //memory stream used to generate document
                using (var ms = new MemoryStream())
                {
                    //Docuemnt created
                    var document = new Document(PageSize.A4, 0, 0, 0, 0);
                    PdfWriter.GetInstance(document, new FileStream("studentReport.pdf", FileMode.Create));
                    PdfWriter.GetInstance(document, ms).SetFullCompression();
                    document.Open();
                    //docuemnt content created
                    var table = new PdfPTable(5);
                    table.TotalWidth = 400;
                    var cell = new PdfPCell(new Phrase("Student List Report"));
                    cell.Colspan = 5;
                    cell.HorizontalAlignment = 1;
                    table.AddCell(cell);
                    //students added to table
                    foreach (var s in students)
                    {
                        table.AddCell(s.StudentTitle);
                        table.AddCell(s.StudentForename);
                        table.AddCell(s.StudentSurname);
                        table.AddCell(s.StudentId.ToString("D"));
                        table.AddCell(s.Course.ToString("D"));
                    }

                    //content added to page
                    document.Add(table);
                    document.Close();

                    //open pdf file
                    Process.Start("AcroRd32.exe", "staffReport.pdf");
                }
                //no students exist
                NoRecords.IsOpen = true;
                Line2.Text = "No Students exist in the database";
            }
        }
        
        //generate events report
        public void GenerateEventsDocument(object sender, EventArgs e)
        {
            //events and event types returned
            var eventsList = _client.ReturnEvents();
            var eventTypes = _client.ReturnEventTypes();

            //check to ensure events and event types not null
            if (eventsList != null && eventTypes != null)
            {
                //memeory strem created to generate document
                using (var ms = new MemoryStream())
                {
                    //document created
                    var document = new Document(PageSize.A4, 0, 0, 0, 0);
                    PdfWriter.GetInstance(document, new FileStream("pdfFile.pdf", FileMode.Create));
                    PdfWriter.GetInstance(document, ms).SetFullCompression();
                    document.Open();

                    var page = new Paragraph(new Chunk(""));
                    document.Add(page);
                    //content setup
                    var table = new PdfPTable(5);
                    table.TotalWidth = 400;
                   
                    var cell = new PdfPCell(new Phrase("Staff List Report"));
                    cell.Colspan = 5;
                    cell.HorizontalAlignment = 1;
                    table.AddCell(cell);
                    var orderedEvents = eventsList.OrderBy(x => x.Status);
                    //events added to table
                    foreach (var ev in orderedEvents)
                    {
                        var eventType = eventTypes.SingleOrDefault(x => x.TypeId == ev.EventType);

                        table.AddCell(ev.EventTitle);
                        table.AddCell(ev.EventDescription);
                        table.AddCell(ev.Status);
                        //event types setup
                        if (eventType != null)
                        {
                            table.AddCell(eventType.TypeName);
                            
                        }
                        else
                        {
                            table.AddCell("N/A");               
                        }

                        table.AddCell(ev.Duration.ToString("D"));
                    }

                    document.Add(table);
                    document.Close();

                    //open pdf file
                    Process.Start("AcroRd32.exe", "pdfFile.pdf");
                }

            }
        }

        //Clsoe No records popup
        public void CloseNoRecordsPopup(Object sender, EventArgs e)
        {
            NoRecords.IsOpen = false;
        }
    }
}
