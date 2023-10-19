using Microsoft.AspNetCore.Mvc;
using Samman.DataBase;
using Samman.Models;
using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Net.Mime;

namespace Samman.Controllers
{
    public class ArchivController : Controller
    {
        private const string PdfMimeType = "application/pdf";
        private const string DocMimeType = "application/msword";
        private const string JpgMimeType = "image/jpeg";
        private const string PngMimeType = "image/png";

        public IActionResult ArchAdm() => View();

        public IActionResult ArchDown() => View(new DocFileViewModel());

        public IActionResult ArchViewer(int id)
        {
            var docFile = new DocFileDbContext().DocFile.FirstOrDefault(pf => pf.Id == id);
            return docFile != null ? View(docFile) : NotFound();
        }

        public IActionResult ArchChange(int id)
        {
            var docFile = new DocFileDbContext().DocFile.FirstOrDefault(pf => pf.Id == id);
            if (docFile != null)
            {
                TempData["Id"] = docFile.Id;
                return View(docFile);
            }
            return NotFound();
        }

        public IActionResult ArchRebuild() => View(new DocFileViewModel());

        [HttpPost]
        public IActionResult ArchAdd(DocFileViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var _docFileDbContext = new DocFileDbContext())
                    using (var _docNameDbContext = new DocNamesDbContext())
                    {
                        var docFileName = model.DocFileName;
                        byte[] docFileBytesPdf;
                        byte[] docFileBytesDoc;
                        byte[] docFileBytesJpg;
                        byte[] docFileBytesPng;

                        using (var memoryStream = new MemoryStream())
                        {
                            model.PdfFile?.CopyTo(memoryStream);
                            docFileBytesPdf = memoryStream.ToArray();
                        }
                        using (var memoryStream = new MemoryStream())
                        {
                            model.DocFile?.CopyTo(memoryStream);
                            docFileBytesDoc = memoryStream.ToArray();
                        }
                        using (var memoryStream = new MemoryStream())
                        {
                            model.JpgFile?.CopyTo(memoryStream);
                            docFileBytesJpg = memoryStream.ToArray();
                        }
                        using (var memoryStream = new MemoryStream())
                        {
                            model.PngFile?.CopyTo(memoryStream);
                            docFileBytesPng = memoryStream.ToArray();
                        }

                        var docFile = new DocFile
                        {
                            FileName = docFileName,
                            FileContentPDF = docFileBytesPdf,
                            FileContentDOC = docFileBytesDoc,
                            FileContentPNG = docFileBytesJpg,
                            FileContentJPG = docFileBytesPng,
                            DateCreated = model.DateCreate
                        };

                        _docFileDbContext.DocFile.Add(docFile);
                        _docFileDbContext.SaveChanges();

                        foreach (var category in model.Categores)
                        {
                            var archiveItem = new ArchiveItem
                            {
                                DocFilename = docFileName,
                                DocFileTruename = model.DocFileName,
                                Category = category,
                                DocFile = docFile
                            };
                            _docNameDbContext.DocNames.Add(archiveItem);
                        }

                        _docNameDbContext.SaveChanges();

                        return RedirectToAction("ArchAdm", "Archiv");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Произошла ошибка при сохранении данных: " + ex.Message);
                }
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult ArchNews(DocFileViewModel model)
        {
            int? id = TempData.Peek("Id") as int?;
            if (id.HasValue && ModelState.IsValid)
            {
                try
                {
                    using (var _docFileDbContext = new DocFileDbContext())
                    using (var _docNameDbContext = new DocNamesDbContext())
                    {
                        var docFileName = model.DocFileName;
                        byte[] docFileBytesPdf;
                        byte[] docFileBytesDoc;
                        byte[] docFileBytesJpg;
                        byte[] docFileBytesPng;

                        using (var memoryStream = new MemoryStream())
                        {
                            model.PdfFile?.CopyTo(memoryStream);
                            docFileBytesPdf = memoryStream.ToArray();
                        }
                        using (var memoryStream = new MemoryStream())
                        {
                            model.DocFile?.CopyTo(memoryStream);
                            docFileBytesDoc = memoryStream.ToArray();
                        }
                        using (var memoryStream = new MemoryStream())
                        {
                            model.JpgFile?.CopyTo(memoryStream);
                            docFileBytesJpg = memoryStream.ToArray();
                        }
                        using (var memoryStream = new MemoryStream())
                        {
                            model.PngFile?.CopyTo(memoryStream);
                            docFileBytesPng = memoryStream.ToArray();
                        }

                        var docFiles = _docFileDbContext.DocFile.FirstOrDefault(pf => pf.Id == id);
                        if (docFiles != null)
                        {
                            docFiles.FileName = docFileName;
                            docFiles.FileContentPDF = docFileBytesPdf;
                            docFiles.FileContentDOC = docFileBytesDoc;
                            docFiles.FileContentPNG = docFileBytesJpg;
                            docFiles.FileContentJPG = docFileBytesPng;
                            docFiles.DateCreated = model.DateCreate;
                        }

                        _docFileDbContext.SaveChanges();

                        var docName = _docNameDbContext.DocNames.FirstOrDefault(pf => pf.Id == id);
                        if (docName != null)
                        {
                            foreach (var category in model.Categores)
                            {
                                docName.DocFilename = docFileName;
                                docName.DocFileTruename = model.DocFileName;
                                docName.Category = category;

                                var docFail = _docNameDbContext.DocFiles.FirstOrDefault(pf => pf.Id == id);
                                if (docFail != null)
                                {
                                    docName.DocFile.FileName = docFileName;
                                    docName.DocFile.FileContentPDF = docFileBytesPdf;
                                    docName.DocFile.FileContentDOC = docFileBytesDoc;
                                    docName.DocFile.FileContentPNG = docFileBytesJpg;
                                    docName.DocFile.FileContentJPG = docFileBytesPng;
                                    docName.DocFile.DateCreated = model.DateCreate;
                                }

                                _docNameDbContext.SaveChanges();
                            }
                        }
                        return RedirectToAction("Archiv", "Service");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Произошла ошибка при сохранении данных: " + ex.Message);
                }
            }
            return RedirectToAction("ArchAdm", "Archiv");
        }

        public IActionResult ArchRemove(int id)
        {
            using (var docFileDbContext = new DocFileDbContext())
            {
                var docFile = docFileDbContext.DocFile.FirstOrDefault(pf => pf.Id == id);
                if (docFile != null)
                {
                    docFileDbContext.DocFile.Remove(docFile);
                    docFileDbContext.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult Pdf(int id) => GetFileResult(id, PdfMimeType, ".pdf");

        [HttpGet]
        public IActionResult Docx(int id) => GetFileResult(id, DocMimeType, ".docx");

        [HttpGet]
        public IActionResult Jpg(int id) => GetFileResult(id, JpgMimeType, ".jpg");

        [HttpGet]
        public IActionResult Png(int id) => GetFileResult(id, PngMimeType, ".png");

        private IActionResult GetFileResult(int id, string mimeType, string fileExtension)
        {
            var docFile = new DocFileDbContext().DocFile.FirstOrDefault(pf => pf.Id == id);
            if (docFile != null)
            {
                Response.Headers.Add("Content-Disposition", new ContentDisposition
                {
                    Inline = true,
                    FileName = docFile.FileName + fileExtension
                }.ToString());
                return File(mimeType switch
                {
                    PdfMimeType => docFile.FileContentPDF,
                    DocMimeType => docFile.FileContentDOC,
                    JpgMimeType => docFile.FileContentJPG,
                    PngMimeType => docFile.FileContentPNG,
                    _ => throw new InvalidOperationException("Unsupported MIME type")
                }, mimeType);
            }
            return RedirectToAction("Error", "Home");
        }
    }
}
