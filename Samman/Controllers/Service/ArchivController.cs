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

        public IActionResult ArchAdm()
        {
            return View();
        }

        public IActionResult ArchDown()
        {
            var model = new DocFileViewModel();
            return View(model);
        }

        public IActionResult ArchViewer(int id)
        {
            var docFileDbContext = new DocFileDbContext();
            var docFile = docFileDbContext.DocFile.FirstOrDefault(pf => pf.Id == id);

            if (docFile != null)
            {
                return View(docFile);
            }
            else
            {
                return NotFound();
            }
        }

        public IActionResult ArchChange(int id)
        {
            var docFileDbContext = new DocFileDbContext();
            var docFile = docFileDbContext.DocFile.FirstOrDefault(pf => pf.Id == id);

            if (docFile != null)
            {
                TempData["Id"] = docFile.Id;
                return View(docFile);
            }
            else
            {
                return NotFound();
            }
        }

        public IActionResult ArchRebuild()
        {
            var model = new DocFileViewModel();
            return View(model);
        }

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
                        string docFileName = model.DocFileName;
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
                        string docFileName = model.DocFileName;
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
        public IActionResult Pdf(int id)
        {
            var docFileDbContext = new DocFileDbContext();
            var docFile = docFileDbContext.DocFile.FirstOrDefault(pf => pf.Id == id);

            if (docFile != null)
            {
                Response.Headers.Add("Content-Disposition", new ContentDisposition
                {
                    Inline = true,
                    FileName = docFile.FileName + ".pdf"
                }.ToString());

                return File(docFile.FileContentPDF, PdfMimeType);
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult Doc(int id)
        {
            var docFileDbContext = new DocFileDbContext();
            var docFile = docFileDbContext.DocFile.FirstOrDefault(pf => pf.Id == id);

            if (docFile != null)
            {
                Response.Headers.Add("Content-Disposition", new ContentDisposition
                {
                    Inline = true,
                    FileName = docFile.FileName + ".doc"
                }.ToString());

                return File(docFile.FileContentDOC, DocMimeType);
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult Jpg(int id)
        {
            var docFileDbContext = new DocFileDbContext();
            var docFile = docFileDbContext.DocFile.FirstOrDefault(pf => pf.Id == id);

            if (docFile != null)
            {
                Response.Headers.Add("Content-Disposition", new ContentDisposition
                {
                    Inline = true,
                    FileName = docFile.FileName + ".jpg"
                }.ToString());

                return File(docFile.FileContentJPG, JpgMimeType);
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult Png(int id)
        {
            var docFileDbContext = new DocFileDbContext();
            var docFile = docFileDbContext.DocFile.FirstOrDefault(pf => pf.Id == id);

            if (docFile != null)
            {
                Response.Headers.Add("Content-Disposition", new ContentDisposition
                {
                    Inline = true,
                    FileName = docFile.FileName + ".png"
                }.ToString());

                return File(docFile.FileContentPNG, PngMimeType);
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
