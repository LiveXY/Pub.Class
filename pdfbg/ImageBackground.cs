using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Drawing = System.Drawing;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace pdfbg {
    public class ImageBackground {
        public void SetBackground(string pdfFile, string destFile, Drawing.Image image, int type = 0) {
            var stream = new FileStream(destFile, FileMode.Create, FileAccess.ReadWrite);
            SetBackground(pdfFile, stream, image, type);
        }

        public void SetBackground(string pdfFile, Stream stream, Drawing.Image image, int type = 0) {
            PdfReader reader = null;
            PdfStamper stamper = null;
            try {
                reader = new PdfReader(pdfFile);
                stamper = new PdfStamper(reader, stream);

                var totalPage = reader.NumberOfPages;
                for (int current = 1; current <= totalPage; current++) {
                    var canvas = stamper.GetUnderContent(current);
                    var page = stamper.GetImportedPage(reader, current);

                    var img = Image.GetInstance(image, Drawing.Imaging.ImageFormat.Png);
                    switch (type) {
                        case 1: //top left
                            img.SetAbsolutePosition(0, page.Height - img.Height);
                            canvas.AddImage(img);
                            break;
                        case 2: //top center
                            img.SetAbsolutePosition((page.Width - img.Width) / 2, page.Height - img.Height);
                            canvas.AddImage(img);
                            break;
                        case 3: //top right
                            img.SetAbsolutePosition(page.Width - img.Width, page.Height - img.Height);
                            canvas.AddImage(img);
                            break;
                        case 4: //middle left
                            img.SetAbsolutePosition(0, (page.Height - img.Height) / 2);
                            canvas.AddImage(img);
                            break;
                        case 5: //middle center
                            img.SetAbsolutePosition((page.Width - img.Width) / 2, (page.Height - img.Height) / 2);
                            canvas.AddImage(img);
                            break;
                        case 6: //middle right
                            img.SetAbsolutePosition(page.Width - img.Width, (page.Height - img.Height) / 2);
                            canvas.AddImage(img);
                            break;
                        case 7: //bottom left
                            img.SetAbsolutePosition(0, 0);
                            canvas.AddImage(img);
                            break;
                        case 8: //bottom center
                            img.SetAbsolutePosition((page.Width - img.Width) / 2, 0);
                            canvas.AddImage(img);
                            break;
                        case 9: //bottom right
                            img.SetAbsolutePosition(page.Width - img.Width, 0);
                            canvas.AddImage(img);
                            break;
                        default: //平扑
                            int xRepeats = (int)((page.Width + img.Width - 1) / image.Width);
                            int yRepeats = (int)((page.Height + img.Height - 1) / image.Height);

                            for (int i = 0; i < xRepeats; i++) {
                                for (int j = 0; j < yRepeats; j++) {
                                    img.SetAbsolutePosition(img.Width * i, image.Height * j);
                                    canvas.AddImage(img);
                                }
                            }
                            break;
                    }

                    //img.SetAbsolutePosition(120, 120);
                    //canvas.AddImage(img);
                    //ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase("Hello people!"), 36, 540, 0);
                    OnProgress(current, totalPage);
                }
                stamper.Close();
            } catch (Exception ex) {
                OnError(ex);
            } finally {
                stamper.Close();
                reader.Close();
            }
            OnFinish();
        }

        protected void OnProgress(int currentPage, int totalPage) {
            if (Progress != null)
                Progress(this, new SetBackgroundProgressEventArgs(currentPage, totalPage));
        }

        protected void OnError(Exception ex) {
            if (Error != null) Error(this, new SetBackgroundErrorEventArgs(ex));
        }

        protected void OnFinish() {
            if (Finish != null) Finish(this, new EventArgs());
        }


        public event EventHandler<SetBackgroundProgressEventArgs> Progress;

        public event EventHandler<SetBackgroundErrorEventArgs> Error;

        public event EventHandler Finish;
    }

    public class SetBackgroundProgressEventArgs : EventArgs {
        public int CurrentPage { get; private set; }

        public int TotalPage { get; private set; }

        public SetBackgroundProgressEventArgs(int currentPage, int totalPage) {
            this.CurrentPage = currentPage;
            this.TotalPage = totalPage;
        }
    }

    public class SetBackgroundErrorEventArgs : EventArgs {
        public Exception Error { get; private set; }

        public SetBackgroundErrorEventArgs(Exception error) {
            this.Error = error;
        }
    }

}
