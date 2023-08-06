using Meter.View;
using MvvmHelpers.Commands;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.Mail;
using System.Net.Mime;
using System.Printing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using Testing.Controller;
using Testing.DB;
using File = System.IO.File;

namespace Testing.Customs
{

    public partial class ReportViewer : DocumentViewer, INotifyPropertyChanged {
        public ICommand PrintDocCmd { get; }
        public ICommand SendViaWhatsAppCmd { get; }
        public ICommand SendViaOutlookCmd { get; }
        private EmailError emailError { get; set; } = null!;
        bool _isloading;
        public event PropertyChangedEventHandler? PropertyChanged;

        public string EmailTo { get; set; }

        public bool IsLoading { get=>_isloading; set {
                _isloading= value;
                Set(nameof(IsLoading));
            } }

        private void Set(string PropName) {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(PropName));
        }

        public ReportViewer()
        {
            NameScope.SetNameScope(this, new NameScope());
            InitializeComponent();
            PrintDocCmd = new Command(PrintAndOpen);
            SendViaWhatsAppCmd = new Command(SendViaWhatsApp);
            SendViaOutlookCmd = new Command(SendViaOutlook);
            FixedDoc.Name = "Doc1";
            RegisterName(FixedDoc.Name, FixedDoc);           
            IsLoading = false;
        }

        private async void SendViaOutlook()
        {
            if (string.IsNullOrEmpty(EmailTo)) {
                MessageBox.Show("No email has been provided","Something is missing");
                return;
            }

                IsLoading = true;
                await Task.Run(()=> t1());
            MicrosoftPDFManager.Reset();
           
            try
            {
                SmtpClient client = new();
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.EnableSsl = true;
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                var credentials=MainDB.Email.RecordSource.FirstOrDefault();
                if (credentials == null) {
                    emailError = new();
                    emailError.ShowDialog();
                }
                client.Credentials = new System.Net.NetworkCredential(credentials.EmailAddress, credentials.Pwd);
                MailMessage mailMessage = new MailMessage(credentials.EmailAddress, EmailTo);
                var rec = MainDB.Email.RecordSource.FirstOrDefault();
                mailMessage.Subject = rec.Subject;
                mailMessage.Body = rec.MainBody;
                await Task.Delay(3000);
                Attachment data = new Attachment(@MicrosoftPDFManager.FileName, MediaTypeNames.Application.Pdf);
                ContentDisposition disposition = data.ContentDisposition;
                disposition.CreationDate = System.IO.File.GetCreationTime(@MicrosoftPDFManager.FileName);
                disposition.ModificationDate = System.IO.File.GetLastWriteTime(@MicrosoftPDFManager.FileName);
                disposition.ReadDate = System.IO.File.GetLastAccessTime(@MicrosoftPDFManager.FileName);

                mailMessage.Attachments.Add(data);
                client.Send(mailMessage);
                IsLoading = false;
                MessageBox.Show("Email successfully sent", "Done!");
                WindowTracker.InvoiceViewer.Close();
                
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                WindowTracker.InvoiceViewer.Close();
                emailError = new();
                emailError.ShowDialog();
            }


        }

        private void SendViaWhatsApp()
        {

        }

        private async void OpenFile() {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(@MicrosoftPDFManager.FileName)
            {
                UseShellExecute = true
            };
            p.Start();

            await App.Current.Dispatcher.InvokeAsync(() =>
            {
                IsLoading = false;
            });

        }

        private async void PrintAndOpen() {
            await t1();
            MicrosoftPDFManager.Reset();
            OpenFile();
        }

      
        public Task pro()
        {
            string temp = "C:\\Users\\Salvatore\\OneDrive\\Desktop\\MyFile.xps";
            if (File.Exists(temp) == true)
                File.Delete(temp);

            //create a XPS document 
            XpsDocument xpsDoc = new XpsDocument(temp, FileAccess.ReadWrite);

            //create a XPS document writer that writes to the XPS document
            XpsDocumentWriter xpsWriter = XpsDocument.CreateXpsDocumentWriter(xpsDoc);

            //write the flow document to the XPS document
            //   use the flow documents default DocumentPaginator to do that
            xpsWriter.Write((FixedDoc as IDocumentPaginatorSource).DocumentPaginator);

            //display the XPS document in the DocumentViewer
            //   use the GetFixedDocumentSequence method to return a 
            //   FixedDocument representation of the XPS file
          //  xpsView.Document = xpsDoc.GetFixedDocumentSequence();

            //close the XPS file
            xpsDoc.Close();
            return Task.CompletedTask;  
        }

        private Task t1() {
            MicrosoftPDFManager.SetPortNameForFilePath();
           
            this.Dispatcher.Invoke(() =>
            {
                PDFPrintDialog dialog = new(FixedDoc.DocumentPaginator);
            });
            return Task.CompletedTask;
        }
    }

    public class PDFPrintDialog : PrintDialog {

        public PDFPrintDialog(DocumentPaginator documentPaginator) {
            PageRangeSelection = PageRangeSelection.AllPages;
            UserPageRangeEnabled = true;
            PrintTicket.PageOrientation = PageOrientation.Portrait;
            PrintTicket.PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA4);
            PrintQueue = new PrintQueue(new PrintServer(), "Microsoft Print to PDF");
            PrintDocument(documentPaginator, "Printing Invoice");
        }

    }

    public static class MicrosoftPDFManager {
        //SET THIS TO FALSE IN THE APP MANIFEST. YOU CAN ADD THE MANIFEST BY CLICKING ON ADD NEW FILE
        //<requestedExecutionLevel  level="requireAdministrator" uiAccess="false" />

        public static string FileName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Invoice.pdf";
        private static readonly string c_App = @".\Microsoft.VC110.CRT\PDFDriverHelper.exe";
        private static readonly string AddPort = "0";
        private static readonly string DeletePort = "1";
        private static string originalPort = string.Empty;
        private static ManagementScope? scope;
        private static readonly string printerName = "Microsoft Print To PDF";
        private static ProcessStartInfo? StartInfo;
        private static Process Process;

        public static void Reset()
        {
            SetPort(originalPort);
            DealWithPort(DeletePort);
            Process.Kill();
        }

        public static void SetPortNameForFilePath() {
            if (!PDFPrinterIsInstalled())
            {
                MessageBox.Show("No PDF Printer is installed in this computer","Something is missing");
                return;
            }
            GetOriginalPort();
            DealWithPort(AddPort);
            SetPort(FileName);
            Process.Kill();
        }

        private static void DealWithPort(string action)
        {
            StartInfo = new ProcessStartInfo();
            StartInfo.FileName = c_App;
            StartInfo.ArgumentList.Add(FileName);
            StartInfo.ArgumentList.Add(action);
            StartInfo.UseShellExecute = true;
            StartInfo.CreateNoWindow = false;
            StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Process = new();
            Process.StartInfo = StartInfo;
            Process.Start();
        }

        private static bool PDFPrinterIsInstalled()
        {
            for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
                if (PrinterSettings.InstalledPrinters[i].ToLower().Equals(printerName.ToLower())) return true;

            return false;
        }

        private static ConnectionOptions Options() => new()
        {
            Impersonation = ImpersonationLevel.Impersonate,
            Authentication = AuthenticationLevel.PacketPrivacy,
            EnablePrivileges = true
        };

        private static ManagementObjectCollection Collection()
        {
            SelectQuery oSelectQuery = new SelectQuery(@"SELECT * FROM Win32_Printer WHERE Name = '" + printerName.Replace("\\", "\\\\") + "'");
            ManagementObjectSearcher oObjectSearcher = new ManagementObjectSearcher(scope, @oSelectQuery);
            return oObjectSearcher.Get();
        }

        private static void Connect()
        {
            scope = new ManagementScope(ManagementPath.DefaultPath, Options());
            scope.Connect();
        }

        private static void GetOriginalPort()
        {
            Connect();
            var collection = Collection();
            foreach (ManagementObject oItem in collection) 
                originalPort = oItem.Properties["PortName"].Value.ToString();
        }

        private static void SetPort(string port)
        {
            Connect();
            var collection = Collection();

            foreach (ManagementObject oItem in collection)
            {
                oItem.Properties["PortName"].Value = port;
                oItem.Put();
            }

        }

    }
}
