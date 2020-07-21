using Svg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using VGCore;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private const double PageWidth = 210;
        private const double PageHeight = 297;
        private const double MaxWidth = 80;
        private const double MaxHeight = 95;
        private const double MaxTextWidth = 70;

        private const double MarginLeft = 5;
        private const double MarginTop = (PageHeight - (MaxHeight * 3)) / 2;
        private const double MarginRight = 40;
        private const double MarginBottom = MarginTop;

        private string extractPath = "";
        private string processedZipFilesPath = "";

        private int counter = 0;

        private Dictionary<string, Dictionary<string, string>> inputTexts;
        
        public Form1()
        {
            InitializeComponent();
            ZipsFolderPathTxt.Text = @"F:\Personal\QuickDesignerResources\Zips";
            CDRFilesLocationTxt.Text = @"F:\Personal\QuickDesignerResources\Templates";

        }



        private async void GenerateBtn_Click(object sender, EventArgs e)
        {
            //transformImageFile(@"D:\Projects\CorelDRAWAddon\Resources\Sample\New bactch of files\026-6084095-0164338\0e921bbb-d250-14b9-27a3-b85e631688b5.svg");

            if (CDRFilesLocationTxt.Text == "")
            {
                MessageBox.Show("Please select CDR files location");
                return;
            }
            if (ZipsFolderPathTxt.Text == "")
            {
                MessageBox.Show("Please select zip files location");
                return;
            }
            try
            {

                disableEveryThing();
                var progress = new Progress<object>(v =>
                {
                    if (v is int)
                    {
                        progressBar1.Value = (int)v;
                    }
                    else
                    {
                        errorConsoleTxt.Text += v.ToString();
                    }
                });
                processedZipFilesPath = ZipsFolderPathTxt.Text + @"\processed\";
                extractPath = CDRFilesLocationTxt.Text + @"\temp\";
                await Task.Run(() => generate(progress));
                MessageBox.Show("Files generated successfully!");
                enableEveryThing();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                enableEveryThing();
            }
        }

        private void ImagesBrowse_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                ZipsFolderPathTxt.Text = folderBrowserDialog1.SelectedPath;
            }



        }

        private void CDRBrowse_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                CDRFilesLocationTxt.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private string[] getZipFiles(string directoryPath)
        {
            Directory.CreateDirectory(processedZipFilesPath);
            DirectoryInfo directory = new DirectoryInfo(directoryPath);
            return directory.GetFiles("*.zip").OrderBy(p => p.CreationTime).Select(f => f.FullName).ToArray();
        }
        private List<string> getImagesFromZip(string zipFilePath)
        {
            Directory.CreateDirectory(extractPath);
            emptyTempDirectory();
            List<string> otherFiles = new List<string>();
            List<string> svgFiles = new List<string>();
            List<string> imageFiles = new List<string>();
            List<string> inputTextsList = new List<string>();
            List<string> fontFamilyList = new List<string>();
            List<string> fontColorList = new List<string>();
            List<string> svgNameList = new List<string>();

            inputTexts = new Dictionary<string, Dictionary<string, string>>();
            using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(".svg", StringComparison.OrdinalIgnoreCase)
                            || entry.FullName.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                            || entry.FullName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                            || entry.FullName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                            )
                    {
                        // Gets the full path to ensure that relative segments are removed.
                        string destinationPath = Path.GetFullPath(Path.Combine(extractPath, entry.FullName));

                        // Ordinal match is safest, case-sensitive volumes can be mounted within volumes that
                        // are case-insensitive.
                        if (destinationPath.StartsWith(extractPath, StringComparison.Ordinal))
                        {
                            entry.ExtractToFile(destinationPath);
                            if (Path.GetExtension(destinationPath).EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
                            {
                                svgFiles.Add(destinationPath);
                            }
                            else {
                                otherFiles.Add(destinationPath);
                            }
                            //imageFiles.Add(destinationPath);
                        }
                    } else if(entry.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                    {
                        string destinationPath = Path.GetFullPath(Path.Combine(extractPath, entry.FullName));
                        if (destinationPath.StartsWith(extractPath, StringComparison.Ordinal))
                        {
                            entry.ExtractToFile(destinationPath);
                            string xmlFileData = File.ReadAllText(destinationPath).Replace("\n", "~`~").Replace("\r", "~`~");
                            Regex svgRegex = new Regex(@"<svg>(((?!<\/svg>).)*)<\/svg>|<svg\/>");
                            foreach (Match match in svgRegex.Matches(xmlFileData))
                            {
                                svgNameList.Add(match.Groups[1].Value.Replace("~`~", Environment.NewLine));
                            }
                            Regex inputRegex = new Regex(@"<inputValue>(((?!<\/inputValue>).)*)<\/inputValue>|<inputValue\/>");
                            foreach(Match match in inputRegex.Matches(xmlFileData))
                            {
                                inputTextsList.Add(match.Groups[1].Value.Replace("~`~", Environment.NewLine));
                            }
                            Regex familyRegex = new Regex(@"<family>(((?!<\/family>).)*)<\/family>");
                            foreach (Match match in familyRegex.Matches(xmlFileData))
                            {
                                fontFamilyList.Add(match.Groups[1].Value.Replace("~`~", Environment.NewLine));
                            }
                            Regex colorRegex = new Regex(@"<value>(((?!<\/value>).)*)<\/value>");
                            foreach (Match match in colorRegex.Matches(xmlFileData))
                            {
                                fontColorList.Add(match.Groups[1].Value.Replace("~`~", Environment.NewLine));
                            }

                        }
                    }
                }
            }
            List<string> svgsText = new List<string>();
            foreach(string file in svgFiles) {
                imageFiles.Add(file);
                svgsText.Add(File.ReadAllText(file));
            }
            for (int i = 0; i < svgNameList.Count; i++) {
                Dictionary<string, string> textData = new Dictionary<string, string>();
                if (inputTextsList.Count > i)
                {
                    textData.Add("text", System.Net.WebUtility.HtmlDecode(inputTextsList[i]));
                }
                if (fontFamilyList.Count > i)
                {
                    textData.Add("family", fontFamilyList[i]);
                }
                if (fontColorList.Count > i)
                {
                    textData.Add("color", fontColorList[i]);
                }
                inputTexts.Add(Path.GetFileNameWithoutExtension(svgNameList[i]), textData);
            }
            foreach (string file in otherFiles)
            {
                bool existsInSvg = false;
                foreach (string text in svgsText)
                {
                    if (text.Contains(Path.GetFileNameWithoutExtension(file))) {
                        existsInSvg = true;
                    }
                }
                if (!existsInSvg) { 
                    imageFiles.Add(file);
                }
            }
            return imageFiles;
        }

        private VGCore.Shape loadResizedImage(VGCore.Layer layer, string imageFilePath)
        {
            double adjustedMaxWidth = MaxWidth;
            if (imageFilePath.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
            {
                adjustedMaxWidth = getMaxWidth(imageFilePath);
                transformImageFile(imageFilePath);
            }
            layer.ImportEx(imageFilePath).Finish();
            VGCore.Shape image = layer.FindShape(Path.GetFileName(imageFilePath));
            if (imageFilePath.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
            {
                if (image.SizeWidth > adjustedMaxWidth || image.SizeHeight > MaxHeight)
                {
                    if (image.SizeWidth >= image.SizeHeight)
                    {
                        double diff = (image.SizeWidth - adjustedMaxWidth) / image.SizeWidth;
                        image.SizeWidth = adjustedMaxWidth;
                        image.SizeHeight = (1 - diff) * image.SizeHeight;

                    }
                    else
                    {
                        double diff = (image.SizeHeight - MaxHeight) / image.SizeHeight;
                        image.SizeHeight = MaxHeight;
                        image.SizeWidth = (1 - diff) * image.SizeWidth;
                    }
                }
                if (image.SizeWidth > adjustedMaxWidth)
                {
                    double diff = (image.SizeWidth - adjustedMaxWidth) / image.SizeWidth;
                    image.SizeWidth = adjustedMaxWidth;
                    image.SizeHeight = (1 - diff) * image.SizeHeight;
                }
                if (image.SizeHeight > MaxHeight)
                {
                    double diff = (image.SizeHeight - MaxHeight) / image.SizeHeight;
                    image.SizeHeight = MaxHeight;
                    image.SizeWidth = (1 - diff) * image.SizeWidth;
                }
            }
            // if it's a jpg file of MUG
            else {
                image.SizeWidth = MaxHeight;
                image.SizeHeight = MaxHeight;
            }

            return image;
        }

        private void transformImageFile(string imageFilePath)
        {
            SvgDocument document = SvgDocument.Open(imageFilePath);
            int i = 0; 
            while(i < document.Children.Count)
            {
                var child = document.Children[i];
                if(child is SvgGroup)
                {
                    if(child.Children.Where(c => c is SvgRectangle).Count() > 0) {
                        document.Children.RemoveAt(i);
                        continue;
                    }
                    if(child.Transforms != null)
                    {
                        child.Transforms.RemoveAll(t => true);
                    }
                    if (child.ContainsAttribute("clip-path") && child.Children.Count > 0)
                    {
                        SvgElement innerGroup = document.Children[i] = child.Children[0];
                        innerGroup.Transforms.RemoveAll(t => true);
                        List<SvgElement> toBeRemoved = new List<SvgElement>();
                        foreach (SvgElement innerElement in innerGroup.Children)
                        {
                            if (innerElement is SvgImage)
                            {
                                SvgImage image = (SvgImage)innerElement;
                                if (image.X > 360)
                                {
                                    image.X = 360;
                                }
                                else if (image.X < -360)
                                {
                                    image.X = -360;
                                }
                                if (image.Y > 360)
                                {
                                    image.Y = 360;
                                }
                                else if (image.Y < -360)
                                {
                                    image.Y = -360;
                                }
                            }
                            else if (innerElement is SvgText)
                            {
                                //innerElement.FontWeight = SvgFontWeight.Normal;
                                innerElement.FontSize = 20;
                                foreach (SvgElement potentialTextSpanElement in innerElement.Children)
                                {
                                    if (potentialTextSpanElement is SvgTextSpan)
                                    {
                                        SvgTextSpan textSpan = (SvgTextSpan)potentialTextSpanElement;
                                        if (textSpan.X.Count > 0 && textSpan.X[0] > 360)
                                        {
                                            textSpan.X[0] = 360;
                                        }
                                        else if (textSpan.X.Count > 0 && textSpan.X[0] < -360)
                                        {
                                            textSpan.X[0] = -360;
                                        }
                                        if (textSpan.Y.Count > 0 && textSpan.Y[0] > 360)
                                        {
                                            textSpan.Y[0] = 360;
                                        }
                                        else if (textSpan.Y.Count > 0 && textSpan.Y[0] < -360)
                                        {
                                            textSpan.Y[0] = -360;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                toBeRemoved.Add(innerElement);
                            }

                        }
                        foreach (var e in toBeRemoved)
                        {
                            innerGroup.Children.Remove(e);
                        }
                    }
                }

                i++;
            }
            //for (int i = 0; i < document.Children.Count; i++)
            //{
            //    SvgElement child = document.Children[i];
            //    if(child.GetType().Name == "SvgGroup" && child.Transforms != null) {
                    
            //        child.Transforms.RemoveAll(t => true);
                    
            //    }
            //    if (child.GetType().Name == "SvgGroup" && child.ContainsAttribute("clip-path") && child.Children.Count > 0)
            //    {
            //        SvgElement innerGroup = document.Children[i] = child.Children[0];
            //        innerGroup.Transforms.RemoveAll(t => true);
            //        List<SvgElement> toBeRemoved = new List<SvgElement>();
            //        foreach (SvgElement innerElement in innerGroup.Children)
            //        {
            //            if (innerElement.GetType().Name == "SvgImage")
            //            {
            //                SvgImage image = (SvgImage)innerElement;
            //                if (image.X > 360)
            //                {
            //                    image.X = 360;
            //                }
            //                else if (image.X < -360)
            //                {
            //                    image.X = -360;
            //                }
            //                if (image.Y > 360)
            //                {
            //                    image.Y = 360;
            //                }
            //                else if (image.Y < -360)
            //                {
            //                    image.Y = -360;
            //                }
            //            }
            //            else if (innerElement.GetType().Name == "SvgText") {
            //                //innerElement.FontWeight = SvgFontWeight.Normal;
            //                innerElement.FontSize = 20;
            //                foreach (SvgElement potentialTextSpanElement in innerElement.Children) {
            //                    if (potentialTextSpanElement is SvgTextSpan) {
            //                        SvgTextSpan textSpan = (SvgTextSpan)potentialTextSpanElement;
            //                        if (textSpan.X.Count > 0 && textSpan.X[0] > 360)
            //                        {
            //                            textSpan.X[0] = 360;
            //                        }
            //                        else if (textSpan.X.Count > 0 && textSpan.X[0] < -360) {
            //                            textSpan.X[0] = -360;
            //                        }
            //                        if (textSpan.Y.Count > 0 && textSpan.Y[0] > 360)
            //                        {
            //                            textSpan.Y[0] = 360;
            //                        }
            //                        else if (textSpan.Y.Count > 0 && textSpan.Y[0] < -360)
            //                        {
            //                            textSpan.Y[0] = -360;
            //                        }
            //                    }
            //                }
            //            } else {
            //                toBeRemoved.Add(innerElement);
            //            }

            //        }
            //        foreach (var e in toBeRemoved)
            //        {
            //            innerGroup.Children.Remove(e);
            //        }
            //    }
            //}
            File.WriteAllText(imageFilePath, document.GetXML());
        }

        private double getMaxWidth(string imageFilePath)
        {
            double maxWidth = MaxWidth;
            SvgDocument document = SvgDocument.Open(imageFilePath);
            if (document.GetXML().Contains("<text") && !document.GetXML().Contains("<image"))
            {
                return MaxTextWidth;
            }
            return maxWidth;
        }

        private void emptyTempDirectory()
        {
            string tempDirectoryPath = CDRFilesLocationTxt.Text + @"\temp\";
            DirectoryInfo di = new DirectoryInfo(tempDirectoryPath);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }

        private VGCore.Document newDocument(VGCore.Application application)
        {
            VGCore.Document document = application.CreateDocument();
            document.Name = ++counter + "_" + Guid.NewGuid().ToString();
            document.Activate();
            document.Unit = VGCore.cdrUnit.cdrMillimeter;
            document.ActivePage.SetSize(210, 297);
            document.Rulers.VUnits = document.Rulers.HUnits = VGCore.cdrUnit.cdrMillimeter;
            createGuideLines(document);
            return document;
        }

        private void saveCloseDocument(VGCore.Document document)
        {
            document.SaveAs(CDRFilesLocationTxt.Text + "\\" + document.Name + ".cdr");
            document.Close();
        }
        private void disableEveryThing()
        {
            ZipsFolderPathTxt.Enabled =
                ImagesBrowse.Enabled =
                CDRFilesLocationTxt.Enabled =
                CDRBrowse.Enabled =
                GenerateBtn.Enabled =
                false;
            errorConsoleTxt.Text = "";
            counter = 0;
        }
        private void enableEveryThing()
        {
            ZipsFolderPathTxt.Enabled =
                ImagesBrowse.Enabled =
                CDRFilesLocationTxt.Enabled =
                CDRBrowse.Enabled =
                GenerateBtn.Enabled =
                true;
            progressBar1.Value = 0;
        }

        private void generate(IProgress<object> progress)
        {
            VGCore.Application application;
            try
            {
                application = new VGCore.Application();
            }
            catch (Exception ex)
            {
                throw new Exception("CorelDRAW is not started or its version is not compatible with the software, please start CorelDRAW if not started or install compatible version");
            }
            VGCore.Document document = null;
            double positionX = 0;
            double positionY = 0;
            List<string> processedZipFiles = new List<string>();
            string[] zipFiles = getZipFiles(ZipsFolderPathTxt.Text);
            int zipCount = 1;
            foreach (string zipFilePath in zipFiles)
            {
                if ((zipCount - 1) % 3 == 0)
                {
                    if (document != null)
                    {
                        saveCloseDocument(document);
                        moveZipFiles(processedZipFiles);
                    }
                    document = newDocument(application);
                    positionX = MarginLeft;
                    //positionY = PageHeight + MaxHeight;
                    positionY = PageHeight - MarginTop + MaxHeight;
                }
                VGCore.Layer layer = document.ActiveLayer;
                List<string> imageFiles = getImagesFromZip(zipFilePath);

                bool isFirstImage = true;
                foreach (string imageFilePath in imageFiles)
                {
                    VGCore.Shape image;
                    try
                    {
                        image = loadResizedImage(layer, imageFilePath);
                    }
                    catch (Exception ex)
                    {
                        string message = "Failed to load file '" + Path.GetFileName(imageFilePath) + "' from zip file '" + Path.GetFileName(zipFilePath);
                        progress.Report(message + "' for template file '" + document.Name + ".cdr'" + Environment.NewLine);
                        //image = layer.CreateArtisticText(0, 0, message);
                        //image.SizeWidth = MaxWidth;
                        //image.SizeHeight = MaxHeight;
                        //image.WrapText = VGCore.cdrWrapStyle.cdrWrapSquareAboveBelow;
                        continue;

                    }
                    double x;
                    double y;
                    if (isFirstImage)
                    {
                        positionX = MarginLeft;
                        //positionY -= MaxHeight + MarginTop;
                        positionY -= MaxHeight;
                        x = positionX + ((MaxWidth - image.SizeWidth) / 2);
                        y = positionY - (MaxHeight - image.SizeHeight) / 2;
                        isFirstImage = false;

                        // insert order id from zip file name into the row
                        layer.CreateArtisticText(-150, y-15, Path.GetFileNameWithoutExtension(zipFilePath).Split('_')[0]
                            , VGCore.cdrTextLanguage.cdrLanguageNone, VGCore.cdrTextCharSet.cdrCharSetMixed
                            , "Arial", 30);
                    }
                    else
                    {
                        //positionX = PageWidth - MarginRight - image.SizeWidth;
                        positionX += MaxWidth + MarginRight;
                        x = positionX + ((MaxWidth - image.SizeWidth) / 2);
                        y = positionY - (MaxHeight - image.SizeHeight) / 2;
                    }

                    image.SetPosition(x, y);

                    // Insert text from xml file
                    Dictionary<string, string> textData;
                    bool hasValue = inputTexts.TryGetValue(Path.GetFileNameWithoutExtension(imageFilePath), out textData);
                    if(hasValue && textData != null && textData["text"] != null && textData["text"].Length > 0)
                    {
                        VGCore.Shape text = layer.CreateArtisticText(positionX, positionY-25, textData["text"]
                            , VGCore.cdrTextLanguage.cdrLanguageNone, VGCore.cdrTextCharSet.cdrCharSetMixed
                            , textData["family"], 25);
                        String color = textData["color"];
                        if (color != null && color != "") {
                            //.FromArgb(Convert.ToInt32(color.Replace("#", ""), 16))
                            VGCore.Color c = layer.Color;
                            c.HexValue = color;
                            text.Fill.ApplyUniformFill(c);
                        }
                        
                        
                    }
                }
                progress.Report(((100 - 10) / zipFiles.Length) * zipCount);
                zipCount++;
                processedZipFiles.Add(zipFilePath);
            }
            saveCloseDocument(document);
            moveZipFiles(processedZipFiles);
            application.Refresh();
            //emptyTempDirectory();
            progress.Report(100);
        }
        private void moveZipFiles(List<string> zipFiles)
        {
            foreach (string zipFile in zipFiles)
            {
                File.Move(zipFile, processedZipFilesPath + Path.GetFileName(zipFile));
            }
            zipFiles.RemoveAll(s => true);
        }
        private void createGuideLines(VGCore.Document document)
        {
            //Horizontal Guides
            double y = PageHeight - MarginTop;
            document.ActiveLayer.CreateGuide(0, y, PageWidth, y);
            for (int i = 0; i < 7; i++)
            {
                y -= MaxHeight / 2;
                document.ActiveLayer.CreateGuide(0, y, PageWidth, y);
            }
            //Vertical Guides
            double x = MarginLeft;
            for (int i = 0; i < 6; i++)
            {
                document.ActiveLayer.CreateGuide(x, 0, x, PageHeight);
                x += MaxWidth;
                document.ActiveLayer.CreateGuide(x, 0, x, PageHeight);
                x += MarginRight;
            }
        }

    }
}
