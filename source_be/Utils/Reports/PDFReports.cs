using System;
using System.Collections.Generic;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using Vauction.Models;
using Image = MigraDoc.DocumentObjectModel.Shapes.Image;

namespace Vauction.Utils.Reports
{
  public static class PdfReports
  {

    public static void ConsignmentContract(string filePath, string logoPath, string title, string author, string subject, Address lelandsAddress, string lelandsSignaturePath, Address sellerAddress, string sellerEmail, string sellerSignaturePath, DateTime date, List<IdTitleValue> items, string contract)
    {
      Document document = new Document();
      document.Info.Title = title;
      document.Info.Author = author;
      document.Info.Subject = subject;

      Section section = document.AddSection();
      Table table = section.AddTable();
      table.Style = "Table";
      Column column = table.AddColumn("5cm");
      column.Format.Alignment = ParagraphAlignment.Left;
      column = table.AddColumn("6cm");
      column.Format.Alignment = ParagraphAlignment.Left;
      column = table.AddColumn("6cm");
      column.Format.Alignment = ParagraphAlignment.Left;

      Row row = table.AddRow();
      row.HeadingFormat = true;
      row.Format.Alignment = ParagraphAlignment.Center;
      row.Format.Font.Bold = true;
      Image image = row.Cells[0].AddImage(logoPath);
      image.LockAspectRatio = true;
      image.RelativeVertical = RelativeVertical.Line;
      image.RelativeHorizontal = RelativeHorizontal.Margin;
      image.Top = ShapePosition.Top;
      image.Left = ShapePosition.Left;
      image.WrapFormat.Style = WrapStyle.Through;
      row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
      row.Cells[0].VerticalAlignment = VerticalAlignment.Top;
      row.Cells[0].MergeDown = 5;

      Paragraph paragraph = row.Cells[1].AddParagraph();
      paragraph.AddText("Consignment Agreement Between");
      row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
      row.Cells[1].MergeRight = 1;

      row = table.AddRow();
      row.Cells[1].AddParagraph(lelandsAddress.FirstName);
      row.Cells[1].Format.Font.Size = 9;
      row.Cells[1].Format.Font.Bold = true;
      row.Cells[2].AddParagraph(string.Format("{0} {1}", sellerAddress.FirstName, sellerAddress.LastName));
      row.Cells[2].Format.Font.Size = 9;
      row.Cells[2].Format.Font.Bold = true;

      row = table.AddRow();
      row.Cells[1].AddParagraph(lelandsAddress.Address_1);
      row.Cells[1].Format.Font.Size = 9;
      row.Cells[2].AddParagraph(string.Format("{0} {1}", sellerAddress.Address_1, sellerAddress.Address_2));
      row.Cells[2].Format.Font.Size = 9;

      row = table.AddRow();
      row.Cells[1].AddParagraph(string.Format("{0}, {1} {2}", lelandsAddress.City, lelandsAddress.State, lelandsAddress.Zip));
      row.Cells[1].Format.Font.Size = 9;
      row.Cells[2].AddParagraph(string.Format("{0}, {1} {2}", sellerAddress.City, sellerAddress.State, sellerAddress.Zip));
      row.Cells[2].Format.Font.Size = 9;

      row = table.AddRow();
      row.Cells[1].AddParagraph(string.Format("Tel: {0}", lelandsAddress.HomePhone));
      row.Cells[1].Format.Font.Size = 9;
      row.Cells[2].AddParagraph(string.Format("Tel (Home): {0}", sellerAddress.HomePhone));
      row.Cells[2].Format.Font.Size = 9;

      row = table.AddRow();
      row.Cells[1].AddParagraph(string.Format("Fax: {0}", lelandsAddress.WorkPhone));
      row.Cells[1].Format.Font.Size = 9;
      row.Cells[2].AddParagraph(string.Format("Tel (Work): {0}", !string.IsNullOrWhiteSpace(sellerAddress.WorkPhone) ? sellerAddress.WorkPhone : "---"));
      row.Cells[2].Format.Font.Size = 9;

      row = table.AddRow();
      row.Cells[1].AddParagraph(lelandsAddress.LastName);
      row.Cells[1].Format.Font.Size = 9;
      row.Cells[2].AddParagraph(sellerEmail);
      row.Cells[2].Format.Font.Size = 9;


      paragraph = section.Footers.Primary.AddParagraph();
      paragraph.AddText(string.Format("{0} - {1}", author, DateTime.Now.Year));
      paragraph.Format.Font.Size = 9;
      paragraph.Format.Alignment = ParagraphAlignment.Center;

      section.AddParagraph(" ");
      paragraph = section.AddParagraph();
      paragraph.Format.Font.Bold = true;
      paragraph.AddText("SCHEDULE A");
      paragraph.AddLineBreak();
      paragraph.AddText("PROPERTY");
      paragraph.Format.Alignment = ParagraphAlignment.Center;

      table = section.AddTable();
      table.Borders.Color = new Color(0, 0, 0);
      table.Borders.Width = 0.5;
      table.Style = "Table";
      column = table.AddColumn("13cm");
      column.Format.Font.Size = 8;
      column = table.AddColumn("4cm");
      column.Format.Alignment = ParagraphAlignment.Center;
      column.Format.Font.Size = 8;
      row = table.AddRow();
      row.Cells[0].AddParagraph("Description");
      row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
      row.Cells[0].Format.Font.Bold = true;
      row.Cells[1].AddParagraph("Seller's Reserve");
      row.Cells[1].Format.Font.Bold = true;
      foreach (var item in items)
      {
        row = table.AddRow();
        row.Cells[0].AddParagraph(item.Title);
        row.Cells[0].Format.Font.Size = 7;
        row.Cells[1].AddParagraph(item.Value.GetCurrency(false));
        row.Cells[1].Format.Font.Size = 7;
      }
      var y = 38 - items.Count;
      for (int i = 0; i < y; i++)
      {
        table.AddRow();
      }

      section.AddParagraph(" ");
      paragraph = section.AddParagraph();
      paragraph.Format.Font.Bold = true;
      paragraph.AddText("Seller agrees with the foregoing, and agrees to keep confidential all of the terms and conditions of this agreement by signing in the space provided below:");
      paragraph.Format.Alignment = ParagraphAlignment.Center;
      paragraph.Format.Font.Size = 7;

      section.AddParagraph(" ");
      section.AddParagraph(" ");

      table = section.AddTable();
      column = table.AddColumn("3cm");
      column.Format.Alignment = ParagraphAlignment.Right;
      column = table.AddColumn("6cm");
      column.Format.Alignment = ParagraphAlignment.Center;
      column = table.AddColumn("6cm");
      column.Format.Alignment = ParagraphAlignment.Center;
      row = table.AddRow();
      row.Cells[0].AddParagraph("Date:");
      row.Cells[0].Format.Font.Size = 6;
      paragraph = row.Cells[1].AddParagraph(date.ToShortDateString());
      paragraph.Format.Font.Size = 6;
      paragraph.AddLineBreak();
      FormattedText formattedText = paragraph.AddFormattedText("___________________________");
      formattedText.Superscript = true;
      formattedText.Font.Size = 10;

      row = table.AddRow();
      row.Cells[0].AddParagraph("Agreed to by:");
      row.Cells[0].Format.Font.Size = 6;
      row.Cells[0].VerticalAlignment = VerticalAlignment.Center;
      row.Cells[1].VerticalAlignment = VerticalAlignment.Bottom;
      if (string.IsNullOrWhiteSpace(sellerSignaturePath))
      {
        paragraph = row.Cells[1].AddParagraph();
        formattedText = paragraph.AddFormattedText("___________________________");
        formattedText.Superscript = true;
      }
      else
      {
        row.Cells[1].AddImage(sellerSignaturePath);
      }
      paragraph = row.Cells[1].AddParagraph();
      formattedText = paragraph.AddFormattedText("Seller");
      formattedText.Superscript = true;
      row.Cells[2].VerticalAlignment = VerticalAlignment.Bottom;
      row.Cells[2].AddImage(lelandsSignaturePath);
      paragraph = row.Cells[2].AddParagraph();
      formattedText = paragraph.AddFormattedText("Lelands.com");
      formattedText.Superscript = true;

      section = document.AddSection();

      paragraph = section.AddParagraph("TERMS & CONDITIONS");
      paragraph.Format.Alignment = ParagraphAlignment.Center;
      paragraph.Format.Font.Bold = true;
      paragraph.Format.Font.Size = 10;
      paragraph.Format.SpaceAfter = 15;

      string[] lines = contract.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
      foreach (var line in lines)
      {
        paragraph = section.AddParagraph(line);
        paragraph.Format.Font.Size = 8;
        paragraph.Format.SpaceAfter = 10;
        //section.AddParagraph(" ");
      }

      // A flag indicating whether to create a Unicode PDF or a WinAnsi PDF file.
      // This setting applies to all fonts used in the PDF document.
      // This setting has no effect on the RTF renderer.
      bool unicode = true;

      // An enum indicating whether to embed fonts or not.
      // This setting applies to all font programs used in the document.
      // This setting has no effect on the RTF renderer.
      // (The term 'font program' is used by Adobe for a file containing a font. Technically a 'font file'
      // is a collection of small programs and each program renders the glyph of a character when executed.
      // Using a font in PDFsharp may lead to the embedding of one or more font programms, because each outline
      // (regular, bold, italic, bold+italic, ...) has its own fontprogram)
      PdfFontEmbedding embedding = PdfFontEmbedding.Always;  // Set to PdfFontEmbedding.None or PdfFontEmbedding.Always only

      // ----------------------------------------------------------------------------------------
      // Create a renderer for the MigraDoc document.
      PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(unicode, embedding);
      // Associate the MigraDoc document with a renderer
      pdfRenderer.Document = document;
      // Layout and render document to PDF
      pdfRenderer.RenderDocument();

      // Save the document...
      pdfRenderer.PdfDocument.Save(filePath);
    }
  }
}