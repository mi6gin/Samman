using Microsoft.AspNetCore.Mvc;
using Samman.DataBase;
using Samman.Models;
using System.Net.Mime;

namespace Samman.Controllers
{
    public class ArchivController: Controller
    {
        public IActionResult ArchAdm()
        {
            return View();
        }

        public IActionResult ArchDown()
        {
            var model = new DocFileViewModel(); // Создайте экземпляр модели
            return View(model); // Передайте модель в представление
        }
        public IActionResult ArchViewer(int id)
        {
            var docFileDbContext = new Samman.DataBase.DocFileDbContext();

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
            var docFileDbContext = new Samman.DataBase.DocFileDbContext();

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

                        return RedirectToAction("ArchAdm", "Services");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Произошла ошибка при сохранении данных: " + ex.Message);
                }
            }

            return View(model);
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
            var docFileDbContext = new Samman.DataBase.DocFileDbContext();

            var docFile = docFileDbContext.DocFile.FirstOrDefault(pf => pf.Id == id);

            if (docFile != null)
            {
                // Устанавливаем Content-Disposition заголовок в "inline" для отображения PDF в iframe
                Response.Headers.Add("Content-Disposition", new ContentDisposition
                {
                    Inline = true,
                    FileName = docFile.FileName + ".pdf"
                }.ToString());

                return File(docFile.FileContentPDF, "application/doc");
            }
            else
            {
                // Обработка случая, если файл не найден
                return NotFound();
            }
        }
        [HttpGet]
        public IActionResult Doc(int id)
        {
            var docFileDbContext = new Samman.DataBase.DocFileDbContext();

            var docFile = docFileDbContext.DocFile.FirstOrDefault(pf => pf.Id == id);

            if (docFile != null)
            {
                // Устанавливаем Content-Disposition заголовок в "inline" для отображения PDF в iframe
                Response.Headers.Add("Content-Disposition", new ContentDisposition
                {
                    Inline = true,
                    FileName = docFile.FileName + ".doc"
                }.ToString());

                return File(docFile.FileContentDOC, "application/doc");
            }
            else
            {
                // Обработка случая, если файл не найден
                return NotFound();
            }
        }
        [HttpGet]
        public IActionResult Jpg(int id)
        {
            var docFileDbContext = new Samman.DataBase.DocFileDbContext();

            var docFile = docFileDbContext.DocFile.FirstOrDefault(pf => pf.Id == id);

            if (docFile != null)
            {
                // Устанавливаем Content-Disposition заголовок в "inline" для отображения PDF в iframe
                Response.Headers.Add("Content-Disposition", new ContentDisposition
                {
                    Inline = true,
                    FileName = docFile.FileName + ".jpg"
                }.ToString());

                return File(docFile.FileContentJPG, "application/doc");
            }
            else
            {
                // Обработка случая, если файл не найден
                return NotFound();
            }
        }
        [HttpGet]
        public IActionResult Png(int id)
        {
            var docFileDbContext = new Samman.DataBase.DocFileDbContext();

            var docFile = docFileDbContext.DocFile.FirstOrDefault(pf => pf.Id == id);

            if (docFile != null)
            {
                // Устанавливаем Content-Disposition заголовок в "inline" для отображения PDF в iframe
                Response.Headers.Add("Content-Disposition", new ContentDisposition
                {
                    Inline = true,
                    FileName = docFile.FileName + ".png"
                }.ToString());

                return File(docFile.FileContentPNG, "application/doc");
            }
            else
            {
                // Обработка случая, если файл не найден
                return NotFound();
            }
        }
    }
}
